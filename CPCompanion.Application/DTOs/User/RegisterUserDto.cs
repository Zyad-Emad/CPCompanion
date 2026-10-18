using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.User
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string CodeforcesHandle { get; set; } = default!;

    }
}
