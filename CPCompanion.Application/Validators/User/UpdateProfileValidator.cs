using CPCompanion.Application.DTOs.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Validators.User
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileValidator()
        {
            RuleFor(p => p.CodeforcesHandle).NotEmpty();
        }
    }
}
