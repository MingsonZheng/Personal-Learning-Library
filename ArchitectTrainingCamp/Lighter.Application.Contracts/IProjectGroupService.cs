using LighterApi.Data.Project;
using System.Threading;

namespace Lighter.Application.Contracts
{
    public interface IProjectGroupService
    {
        Task CreateAsync(ProjectGroup group, CancellationToken cancellationToken);
    }
}
