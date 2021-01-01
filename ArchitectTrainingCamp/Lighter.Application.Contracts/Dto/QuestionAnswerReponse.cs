using LighterApi.Data.Question;
using System.Collections.Generic;

namespace Lighter.Application.Contracts.Dto
{
    public class QuestionAnswerReponse : Question
    {
        //public Question Question { get; set; }
        public IEnumerable<Answer> AnswerList { get; set; }
    }
}
