using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Question
{
    public class Comment
    {
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }
    }
}
