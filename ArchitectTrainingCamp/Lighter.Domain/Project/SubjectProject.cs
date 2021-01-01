using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Project
{
    public class SubjectProject : Entity
    {
        public string ProjcetId { get; set; }

        public Project Project { get; set; }

        public string SubjectId { get; set; }

        public Subject Subject { get; set; }
    }
}
