using Lighter.Application.Contracts;
using LighterApi.Data.Question;
using LighterApi.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LighterApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Question>> GetAsync(string id, CancellationToken cancellationToken)
        {
            var question = await _questionService.GetAsync(id, cancellationToken);

            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpGet]
        [Route("{id}/answers")]
        public async Task<ActionResult> GetWithAnswerAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetWithAnswerAsync(id, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<Question>>> GetListAsync([FromQuery] List<string> tags,
            CancellationToken cancellationToken, [FromQuery] string sort = "createdAt", [FromQuery] int skip = 0,
            [FromQuery] int limit = 10)
        {
            var result = await _questionService.GetListAsync(tags, cancellationToken, sort, skip, limit);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Question>> CreateAsync([FromBody] Question question, CancellationToken cancellationToken)
        {
            question = await _questionService.CreateAsync(question, cancellationToken);
            return StatusCode((int) HttpStatusCode.Created, question);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] QuestionUpdateRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Summary))
                throw new ArgumentNullException(nameof(request.Summary));

            await _questionService.UpdateAsync(id, request, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/answer")]
        public async Task<ActionResult<Answer>> AnswerAsync([FromRoute] string id, [FromBody] AnswerRequest request, CancellationToken cancellationToken)
        {
            var answer = await _questionService.AnswerAsync(id, request, cancellationToken);
            return Ok(answer);
        }

        [HttpPost]
        [Route("{id}/comment")]
        public async Task<ActionResult> CommentAsync([FromRoute] string id, [FromBody] CommentRequest request, CancellationToken cancellationToken)
        {
            await _questionService.CommentAsync(id, request, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/up")]
        public async Task<ActionResult> UpAsync([FromBody] string id, CancellationToken cancellationToken)
        {
            await _questionService.UpAsync(id, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/down")]
        public async Task<ActionResult> DownAsync([FromBody] string id, CancellationToken cancellationToken)
        {
            await _questionService.DownAsync(id, cancellationToken);
            return Ok();
        }
    }
}
