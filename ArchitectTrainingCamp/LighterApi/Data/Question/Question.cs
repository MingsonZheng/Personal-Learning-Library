using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LighterApi.Data.Question
{
    public class Question : Entity
    {
        public String ProjectId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public int ViewCount { get; set; }

        public int VoteCount { get; set; }

        public List<string> VoteUps { get; set; } = new List<string>();

        public List<string> VoteDowns { get; set; } = new List<string>();

        public List<string> Answers { get; set; } = new List<string>();

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
