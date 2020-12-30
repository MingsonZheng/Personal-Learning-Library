using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Dto
{
    public class QuestionUpdateRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }

        public string Summary { get; set; }
    }
}
