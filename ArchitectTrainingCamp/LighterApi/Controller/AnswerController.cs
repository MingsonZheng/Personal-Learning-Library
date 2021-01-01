using Lighter.Application.Contracts;
using LighterApi.Data.Question;
using LighterApi.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace LighterApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<ActionResult<Answer>> GetListAsync([FromQuery] string questionId, CancellationToken cancellationToken)
        {
            var list = await _answerService.GetListAsync(questionId, cancellationToken);
            return Ok(list);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, string content, string summary, CancellationToken cancellationToken)
        {
            await _answerService.UpdateAsync(id, content, summary, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/comment")]
        public async Task<ActionResult> CommentAsync([FromRoute] string id, [FromBody] CommentRequest request, CancellationToken cancellationToken)
        {
            await _answerService.CommentAsync(id, request, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/up")]
        public async Task<ActionResult> UpAsync([FromBody] string id, CancellationToken cancellationToken)
        {
            await _answerService.UpAsync(id, cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/down")]
        public async Task<ActionResult> DownAsync([FromBody] string id, CancellationToken cancellationToken)
        {
            await _answerService.DownAsync(id, cancellationToken);
            return Ok();
        }
    }
}
