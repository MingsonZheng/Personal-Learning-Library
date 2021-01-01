using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LighterApi.Share;

namespace LighterApi.Data.Project
{
    public class Task : Entity
    {
        public string Title { get; set; }

        public string SectionId { get; set; }

        public string Description { get; set; }

        public string ProjectId { get; set; }

        public string MemberId { get; set; }

        public Member Member { get; set; }

        //public EnumTaskStauts Status { get; set; }
    }
}
