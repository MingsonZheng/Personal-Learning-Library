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
    }
}
