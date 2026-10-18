using CPCompanion.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CPCompanion.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedException("Invalid user identity.");
            }

            return userId;
        }
    }
}
