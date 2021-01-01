using Lighter.Application.Contracts;
using Lighter.Application.Contracts.Dto;
using LighterApi.Data.Question;
using LighterApi.Dto;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LighterApi.Share;
using MongoDB.Driver.Linq;

namespace Lighter.Application
{
    public class QuestionService : IQuestionService
    {
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<Vote> _voteCollection;
        private readonly IMongoCollection<Answer> _answerCollection;

        public QuestionService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");

            _questionCollection = database.GetCollection<Question>("questions");
            _voteCollection = database.GetCollection<Vote>("votes");
            _answerCollection = database.GetCollection<Answer>("answers");
        }


        public async Task<Question> GetAsync(string id, CancellationToken cancellationToken)
        {
            // linq 查询
            var question = await _questionCollection.AsQueryable()
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken: cancellationToken);

            //// mongo 查询表达式
            ////var filter = Builders<Question>.Filter.Eq(q => q.Id, id);

            //// 构造空查询条件的表达式
            //var filter = string.IsNullOrEmpty(id)
            //    ? Builders<Question>.Filter.Empty
            //    : Builders<Question>.Filter.Eq(q => q.Id, id);

            //// 多段拼接 filter
            //var filter2 = Builders<Question>.Filter.And(filter, Builders<Question>.Filter.Eq(q => q.TenantId, "001"));
            //await _questionCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return question;
        }

        public async Task<List<Question>> GetListAsync(List<string> tags, CancellationToken cancellationToken, string sort = "createdAt", int skip = 0, int limit = 10)
        {
            //// linq 查询
            //await _questionCollection.AsQueryable().Where(q => q.ViewCount > 10)
            //    .ToListAsync(cancellationToken: cancellationToken);

            var filter = Builders<Question>.Filter.Empty;

            if (tags != null && tags.Any())
            {
                filter = Builders<Question>.Filter.AnyIn(q => q.Tags, tags);
            }

            var sortDefinition = Builders<Question>.Sort.Descending(new StringFieldDefinition<Question>(sort));

            var result = await _questionCollection
                .Find(filter)
                .Sort(sortDefinition)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync(cancellationToken: cancellationToken);

            return result;
        }

        public async Task<QuestionAnswerReponse> GetWithAnswerAsync(string id, CancellationToken cancellationToken)
        {
            // linq 查询
            var query = from question in _questionCollection.AsQueryable()
                where question.Id == id
                join a in _answerCollection.AsQueryable() on question.Id equals a.QuestionId into answers
                select new { question, answers };

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            //// mongo 查询表达式
            //var result = await _questionCollection.Aggregate()
            //    .Match(q => q.Id == id)
            //    .Lookup<Answer, QuestionAnswerReponse>(
            //        foreignCollectionName: "answers",
            //        localField: "answers",
            //        foreignField: "questionId",
            //        @as: "AnswerList")
            //    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return new QuestionAnswerReponse {AnswerList = result.answers};
        }

        public async Task<Answer> AnswerAsync(string id, AnswerRequest request, CancellationToken cancellationToken)
        {
            var answer = new Answer { QuestionId = id, Content = request.Content, Id = Guid.NewGuid().ToString() };
            _answerCollection.InsertOneAsync(answer, cancellationToken);

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var update = Builders<Question>.Update.Push(q => q.Answers, answer.Id);

            await _questionCollection.UpdateOneAsync(filter, update, null, cancellationToken);

            return answer;
        }

        public async Task CommentAsync(string id, CommentRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var update = Builders<Question>.Update.Push(q => q.Comments,
                new Comment { Content = request.Content, CreatedAt = DateTime.Now });

            await _questionCollection.UpdateOneAsync(filter, update, null, cancellationToken);
        }

        public async Task<Question> CreateAsync(Question question, CancellationToken cancellationToken)
        {
            question.Id = Guid.NewGuid().ToString();
            await _questionCollection.InsertOneAsync(question, new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);
            return question;
        }

        public async Task DownAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceType = ConstVoteSourceType.Question,
                SourceId = id,
                Direction = EnumVoteDirection.Down
            };

            await _voteCollection.InsertOneAsync(vote, cancellationToken);

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var update = Builders<Question>.Update.Inc(q => q.VoteCount, -1).AddToSet(q => q.VoteDowns, vote.Id);
            await _questionCollection.UpdateOneAsync(filter, update);
        }


        public async Task UpAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceType = ConstVoteSourceType.Question,
                SourceId = id,
                Direction = EnumVoteDirection.Up
            };

            await _voteCollection.InsertOneAsync(vote, cancellationToken);

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var update = Builders<Question>.Update.Inc(q => q.VoteCount, 1).AddToSet(q => q.VoteUps, vote.Id);
            await _questionCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateAsync(string id, QuestionUpdateRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);

            //var update = Builders<Question>.Update
            //    .Set(q => q.Title, request.Title)
            //    .Set(q => q.Content, request.Content)
            //    .Set(q => q.Tags, request.Tags)
            //    .Push(q => q.Comments, new Comment {Content = request.Summary, CreatedAt = DateTime.Now});

            var updateFieldList = new List<UpdateDefinition<Question>>();

            if (!string.IsNullOrWhiteSpace(request.Title))
                updateFieldList.Add(Builders<Question>.Update.Set(q => q.Title, request.Title));

            if (!string.IsNullOrWhiteSpace(request.Content))
                updateFieldList.Add(Builders<Question>.Update.Set(q => q.Content, request.Content));

            if (request.Tags != null && request.Tags.Any())
                updateFieldList.Add(Builders<Question>.Update.Set(q => q.Tags, request.Tags));

            updateFieldList.Add(Builders<Question>.Update.Push(q => q.Comments,
                new Comment { Content = request.Summary, CreatedAt = DateTime.Now }));

            var update = Builders<Question>.Update.Combine(updateFieldList);

            await _questionCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
