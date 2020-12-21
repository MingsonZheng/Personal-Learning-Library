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
    }
}
