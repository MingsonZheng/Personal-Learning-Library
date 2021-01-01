using LighterApi.Data.Question;
using LighterApi.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lighter.Application.Contracts.Dto;

namespace Lighter.Application.Contracts
{
    public interface IQuestionService
    {
        Task<Question> GetAsync(string id, CancellationToken cancellationToken);
        Task<QuestionAnswerReponse> GetWithAnswerAsync(string id, CancellationToken cancellationToken);
        Task<List<Question>> GetListAsync(List<string> tags, CancellationToken cancellationToken, string sort = "createdAt", int skip = 0, int limit = 10);
        Task<Question> CreateAsync(Question question, CancellationToken cancellationToken);
        Task UpdateAsync(string id, QuestionUpdateRequest request, CancellationToken cancellationToken);
        Task<Answer> AnswerAsync(string id, AnswerRequest request, CancellationToken cancellationToken);
        Task CommentAsync(string id, CommentRequest request, CancellationToken cancellationToken);
        Task UpAsync(string id, CancellationToken cancellationToken);
        Task DownAsync(string id, CancellationToken cancellationToken);
    }
}
