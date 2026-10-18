using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }
}
