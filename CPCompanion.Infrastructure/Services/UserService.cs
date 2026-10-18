using AutoMapper;
using CPCompanion.Application.DTOs.Auth;
using CPCompanion.Application.DTOs.User;
using CPCompanion.Application.Exceptions;
using CPCompanion.Application.Interfaces;
using CPCompanion.Domain.Entities;
using CPCompanion.Infrastructure.ExternalServices;
using CPCompanion.Infrastructure.Identity;
using CPCompanion.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly ICodeforcesApiService _codeforcesApiService;
        private readonly ISyncService _syncService;
        public UserService(IMapper mapper, UserManager<ApplicationUser> userManager, AppDbContext context, ITokenService tokenService, IOptions<JwtSettings> jwtSettings, ICodeforcesApiService codeforcesApiService, ISyncService syncService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _codeforcesApiService = codeforcesApiService;
            _syncService = syncService;
        }
        public async Task<UserResponseDto> GetProfileAsync(Guid userId)
        {
            var user = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                throw new NotFoundException("User profile not found.");
            }
            var applicationUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            var responseDto = _mapper.Map<UserResponseDto>(user);
            responseDto.Email = applicationUser!.Email!;

            return responseDto;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if(applicationUser is null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(applicationUser, loginDto.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }
            var user = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdentityUserId == applicationUser.Id);

            if (user is null)
            {
                throw new Exception("User profile not found for this account.");
            }

            var token = _tokenService.GenerateToken(user.Id.ToString(), applicationUser.Email!); // user.Id مش applicationUser.Id

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterUserDto registerUserDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);
                if(existingUser != null)
                {
                    throw new ConflictException("Email is already registered.");
                }
                var ratingInfo = await _codeforcesApiService.GetUserRatingAsync(registerUserDto.CodeforcesHandle);
                if (!ratingInfo.Exists)
                {
                    throw new NotFoundException($"Codeforces handle '{registerUserDto.CodeforcesHandle}' not found.");
                }
                var applicationUser = new ApplicationUser()
                {
                    Email = registerUserDto.Email,
                    UserName = registerUserDto.Email
                };
                var result = await _userManager.CreateAsync(applicationUser, registerUserDto.Password);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = applicationUser.Id,
                    CodeforcesHandle = registerUserDto.CodeforcesHandle,
                    CurrentRating = ratingInfo.CurrentRating,
                    MaxRating = ratingInfo.MaxRating
                };

                _context.UserProfiles.Add(user);
                await _context.SaveChangesAsync();

                var responseDto = _mapper.Map<UserResponseDto>(user);
                responseDto.Email = applicationUser.Email;

                await transaction.CommitAsync();

                return responseDto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

           

        }

        public async Task<UserResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                throw new NotFoundException("User profile not found.");
            }
            if(user.CodeforcesHandle != dto.CodeforcesHandle)
            {
                var handleCheck = await _codeforcesApiService.GetUserRatingAsync(dto.CodeforcesHandle);
                if (!handleCheck.Exists)
                {
                    throw new NotFoundException($"Codeforces handle '{dto.CodeforcesHandle}' not found.");
                }
                user.CodeforcesHandle = dto.CodeforcesHandle;
                await _context.SaveChangesAsync(); 

                await _syncService.SyncUserAsync(userId);
            }

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<UserResponseDto>(user);
            var applicationUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            responseDto.Email = applicationUser!.Email!;

            return responseDto;

        }
    }
}
