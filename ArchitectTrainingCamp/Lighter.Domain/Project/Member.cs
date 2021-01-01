using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Project
{
    public class Member : Entity
    {
        public int Progress { get; set; }

        public string ProjectId { get; set; }

        public bool IsAssistant { get; set; }

        public string GroupId { get; set; }

        public ProjectGroup Group { get; set; }
    }
}
