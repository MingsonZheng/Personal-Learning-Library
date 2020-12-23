using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LighterApi.Data;
using LighterApi.Data.Project;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LighterApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly LighterDbContext _lighterDbContext;

        public ProjectController(LighterDbContext lighterDbContext)
        {
            _lighterDbContext = lighterDbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Project>> GetListAsync(CancellationToken cancellationToken)
        {
            return await _lighterDbContext.Projects.ToListAsync(cancellationToken);
        }

        public async Task<ActionResult<Project>> CreateAsync([FromBody] Project project,
            CancellationToken cancellationToken)
        {
            //project.Id = Guid.NewGuid().ToString();
            _lighterDbContext.Projects.Add(project);
            await _lighterDbContext.SaveChangesAsync(cancellationToken);

            return StatusCode((int) HttpStatusCode.Created, project);
        }
    }
}
