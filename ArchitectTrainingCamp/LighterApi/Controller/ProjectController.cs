using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LighterApi.Data;
using LighterApi.Data.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LighterApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize]
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
            return await _lighterDbContext.Projects.Include(p => p.Groups).ToListAsync(cancellationToken);
        }

        [Authorize(Roles = "Administrators, Mentor")]
        [HttpPost]
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

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Project>> UpdateAsync(string id, [FromBody] Project project, CancellationToken cancellationToken)
        {
            var origin = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            // 通过客户端传入行版本号，解决前端浏览器数据覆盖问题
            _lighterDbContext.Entry(origin).Property(p => p.RowVersion).OriginalValue = project.RowVersion;

            if (origin == null)
                return NotFound();

            _lighterDbContext.Entry(origin).CurrentValues.SetValues(project);

            await _lighterDbContext.SaveChangesAsync(cancellationToken);
            return origin;
        }

        [HttpPatch]
        [Route("{id}/title")]
        public async Task<ActionResult<Project>> SetTitleAsync(string id, [FromQuery] string title, CancellationToken cancellationToken)
        {
            // 查询实体信息
            var originPorject = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            var originGroup = await _lighterDbContext.ProjectGroups.Where(g => g.ProjectId == id).ToListAsync(cancellationToken: cancellationToken);

            // 修改实体属性
            originPorject.Title = title;

            foreach (var group in originGroup)
            {
                group.Name = $"{title} - {group.Name}";
            }

            // 数据提交保存
            await _lighterDbContext.SaveChangesAsync();

            return originPorject;
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<Project>> SetAsync(string id, CancellationToken cancellationToken)
        {
            // 查询实体信息
            var origin = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            var properties = _lighterDbContext.Entry(origin).Properties.ToList();

            // 修改实体属性
            foreach (var query in HttpContext.Request.Query)
            {
                var property = properties.FirstOrDefault(p => p.Metadata.Name == query.Key);
                if (property == null)
                    continue;

                var currentValue = Convert.ChangeType(query.Value.First(), property.Metadata.ClrType);

                _lighterDbContext.Entry(origin).Property(query.Key).CurrentValue = currentValue;
                _lighterDbContext.Entry(origin).Property(query.Key).IsModified = true;
            }

            // 数据提交保存
            await _lighterDbContext.SaveChangesAsync(cancellationToken);

            return origin;
        }
    }
}
