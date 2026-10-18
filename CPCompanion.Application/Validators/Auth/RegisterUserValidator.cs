using CPCompanion.Application.DTOs.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Validators.Auth
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(u => u.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress();

            RuleFor(u => u.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
                .WithMessage("Password must contain at least one special character.");

            RuleFor(u => u.CodeforcesHandle).NotEmpty();
        }
    }
}
