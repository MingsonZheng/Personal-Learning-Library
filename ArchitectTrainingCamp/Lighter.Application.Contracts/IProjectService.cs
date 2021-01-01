using LighterApi.Data.Project;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Application.Contracts
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetListAsync(CancellationToken cancellationToken);
        Task<Project> CreateAsync(Project project, CancellationToken cancellationToken);
        Task<Project> GetProfileAsync(string id, CancellationToken cancellationToken);
        Task<Project> Get(string id, CancellationToken cancellationToken);
        Task<Project> UpdateAsync(string id, Project project, CancellationToken cancellationToken);
        Task<Project> SetTitleAsync(string id, string title, CancellationToken cancellationToken);
        Task<Project> SetAsync(string id, CancellationToken cancellationToken);
    }
}
