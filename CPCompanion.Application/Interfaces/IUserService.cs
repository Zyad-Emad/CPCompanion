using CPCompanion.Application.DTOs.Auth;
using CPCompanion.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<UserResponseDto> GetProfileAsync(Guid userId);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    }
}
