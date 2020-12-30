using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LighterApi.Share;

namespace LighterApi.Data.Question
{
    public class Vote : Entity
    {
        public string SourceType { get; set; }

        public string SourceId { get; set; }

        public EnumVoteDirection Direction { get; set; }
    }
}
