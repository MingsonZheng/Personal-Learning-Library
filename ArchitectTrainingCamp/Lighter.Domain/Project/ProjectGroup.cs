using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Project
{
    public class ProjectGroup : Entity
    {
        public string Name { get; set; }

        public string ProjectId { get; set; }

        public Project Project { get; set; }

        public List<Member> Members { get; set; }
    }
}
