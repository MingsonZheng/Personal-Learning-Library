using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Project
{
    /// <summary>
    /// 课题
    /// </summary>
    public class Subject : Entity
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public List<SubjectProject> SubjectProjects { get; set; }
    }
}
