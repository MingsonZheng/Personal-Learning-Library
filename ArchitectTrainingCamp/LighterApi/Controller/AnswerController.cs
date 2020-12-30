using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LighterApi.Data.Question;
using LighterApi.Dto;
using LighterApi.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace LighterApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly IMongoCollection<Vote> _voteCollection;

        private readonly IMongoCollection<Answer> _answerCollection;

        public AnswerController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");

            _voteCollection = database.GetCollection<Vote>("votes");
            _answerCollection = database.GetCollection<Answer>("answers");
        }

        [HttpGet]
        public async Task<ActionResult<Answer>> GetListAsync([FromQuery] string questionId, CancellationToken cancellationToken)
        {
            var list = await _answerCollection.AsQueryable().Where(a => a.QuestionId == questionId)
                .ToListAsync(cancellationToken);
            return Ok(list);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, string content, string summary,
            CancellationToken cancellationToken)
        {
            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);

            var update = Builders<Answer>.Update
                .Set(q => q.Content, content)
                .Push(q => q.Comments, new Comment { Content = summary, CreatedAt = DateTime.Now });

            await _answerCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("{id}/comment")]
        public async Task<ActionResult> CommentAsync([FromRoute] string id, [FromBody] CommentRequest request,
    CancellationToken cancellationToken)
        {
            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);
            var update = Builders<Answer>.Update.Push(q => q.Comments,
                new Comment { Content = request.Content, CreatedAt = DateTime.Now });

            await _answerCollection.UpdateOneAsync(filter, update, null, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("{id}/up")]
        public async Task<ActionResult> UpAsync([FromBody] string id, CancellationToken cancellationToken)
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

            return Ok();
        }

        [HttpPost]
        [Route("{id}/down")]
        public async Task<ActionResult> DownAsync([FromBody] string id, CancellationToken cancellationToken)
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

            return Ok();
        }
    }
}
