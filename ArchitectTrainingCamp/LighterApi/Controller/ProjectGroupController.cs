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
    public class ProjectGroupController : ControllerBase
    {
        private readonly LighterDbContext _lighterDbContext;

        public ProjectGroupController(LighterDbContext lighterDbContext)
        {
            _lighterDbContext = lighterDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectGroup group)
        {
            _lighterDbContext.ProjectGroups.Add(group);
            await _lighterDbContext.SaveChangesAsync();

            return StatusCode((int) HttpStatusCode.Created, group);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken)
        {
            var project = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            return Ok(project);
        }
    }
}
