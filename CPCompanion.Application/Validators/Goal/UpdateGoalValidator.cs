using CPCompanion.Application.DTOs.Goal;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Validators.Goal
{
    public class UpdateGoalValidator : AbstractValidator<UpdateGoalDto>
    {
        public UpdateGoalValidator()
        {
            RuleFor(g => g.Title).NotEmpty();
            RuleFor(g => g.TargetCount).GreaterThan(0);
            RuleFor(g => g.Deadline)
                    .Must(deadline =>
                        !deadline.HasValue ||
                        deadline.Value > DateTime.UtcNow)
                    .WithMessage("Deadline must be in the future.");
        }
    }
}
