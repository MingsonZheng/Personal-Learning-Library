using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Question
{
    public class Answer : Entity
    {
        public string QuestionId { get; set; }

        public string Content { get; set; }

        public int VoteCount { get; set; }

        public List<string> VoteUps { get; set; } = new List<string>();

        public List<string> VoteDowns { get; set; } = new List<string>();

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
