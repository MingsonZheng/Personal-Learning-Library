using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data
{
    public class LighterDbContext : DbContext
    {
        public LighterDbContext(DbContextOptions<LighterDbContext> options) : base(options)
        {

        }

        public DbSet<Project.Project> Projects { get; set; }

        public DbSet<Project.Member> Members { get; set; }

        public DbSet<Project.Assistant> Assistants { get; set; }

        public DbSet<Project.ProjectGroup> ProjectGroups { get; set; }

        public DbSet<Project.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
