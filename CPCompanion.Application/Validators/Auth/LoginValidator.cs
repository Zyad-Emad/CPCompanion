using CPCompanion.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress();

            RuleFor(l => l.Password).NotEmpty();
        }
    }
}
