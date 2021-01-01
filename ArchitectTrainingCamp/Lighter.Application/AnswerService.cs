using Lighter.Application.Contracts;
using LighterApi.Data.Question;
using LighterApi.Dto;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LighterApi.Share;
using Microsoft.EntityFrameworkCore;

namespace Lighter.Application
{
    public class AnswerService : IAnswerService
    {
        private readonly IMongoCollection<Answer> _answerCollection;
        private readonly IMongoCollection<Vote> _voteCollection;

        public AnswerService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");

            _answerCollection = database.GetCollection<Answer>("answers");
            _voteCollection = database.GetCollection<Vote>("votes");
        }

        public async Task<IEnumerable<Answer>> GetListAsync(string questionId, CancellationToken cancellationToken)
        {
            var list = await _answerCollection.AsQueryable().Where(a => a.QuestionId == questionId)
                .ToListAsync(cancellationToken);

            return list;
        }


        public async Task CommentAsync(string id, CommentRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);
            var update = Builders<Answer>.Update.Push(q => q.Comments,
                new Comment { Content = request.Content, CreatedAt = DateTime.Now });

            await _answerCollection.UpdateOneAsync(filter, update, null, cancellationToken);

        }

        public async Task DownAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceType = ConstVoteSourceType.Answer,
                SourceId = id,
                Direction = EnumVoteDirection.Down
            };

            await _voteCollection.InsertOneAsync(vote, cancellationToken);

            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);
            var update = Builders<Answer>.Update.Inc(q => q.VoteCount, -1).AddToSet(q => q.VoteDowns, vote.Id);
            await _answerCollection.UpdateOneAsync(filter, update);
        }

      
        public async Task UpAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceType = ConstVoteSourceType.Answer,
                SourceId = id,
                Direction = EnumVoteDirection.Up
            };

            await _voteCollection.InsertOneAsync(vote, cancellationToken);

            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);
            var update = Builders<Answer>.Update.Inc(q => q.VoteCount, 1).AddToSet(q => q.VoteUps, vote.Id);
            await _answerCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateAsync(string id, string content, string summary, CancellationToken cancellationToken)
        {
            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);

            var update = Builders<Answer>.Update
                .Set(q => q.Content, content)
                .Push(q => q.Comments, new Comment { Content = summary, CreatedAt = DateTime.Now });

            await _answerCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
