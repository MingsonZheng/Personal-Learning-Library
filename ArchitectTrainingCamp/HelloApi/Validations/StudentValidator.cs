using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using HelloApi.Models;

namespace HelloApi.Validations
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(s => s.Id).InclusiveBetween(1,10).WithMessage("id需要在1和10之间");
        }
    }
}
