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
            modelBuilder.Entity<Project.Project>().Property(p => p.Id).ValueGeneratedOnAdd();

            //// 一对一
            //modelBuilder.Entity<Project.ProjectGroup>()
            //.HasOne<Project.Project>(g => g.Project);

            // 一对多
            modelBuilder.Entity<Project.ProjectGroup>()
                .HasOne<Project.Project>(g => g.Project)
                .WithMany(p => p.Groups);

            // 多对多（两组一对多）
            modelBuilder.Entity<Project.SubjectProject>()
                .HasOne<Project.Project>(s => s.Project)
                .WithMany(p => p.SubjectProjects)
                .HasForeignKey(s => s.ProjcetId);

            modelBuilder.Entity<Project.SubjectProject>()
                .HasOne<Project.Subject>(s => s.Subject)
                .WithMany(p => p.SubjectProjects)
                .HasForeignKey(s => s.SubjectId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
