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

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken)
        {
            //// 预先加载
            //var project = await _lighterDbContext.Projects.Include(p => p.Groups)
            //    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            // 显式加载
            var project = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            await _lighterDbContext.Entry(project).Collection(p => p.Groups).LoadAsync(cancellationToken);

            //// 延迟加载
            //project.Groups// 引用到属性时才加载

            //foreach (var project in _lighterDbContext.Projects)
            //{
            //    project.Groups// 多次查询数据库
            //}

            //// 一次性查询
            //var projects = _lighterDbContext.Projects.ToList();

            return Ok(project);
        }
    }
}
