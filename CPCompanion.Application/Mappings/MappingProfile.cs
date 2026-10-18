using AutoMapper;
using CPCompanion.Application.DTOs.Goal;
using CPCompanion.Application.DTOs.User;
using CPCompanion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDto>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<CreateGoalDto, Goal>();
            CreateMap<Goal, GoalResponseDto>();
            CreateMap<UpdateGoalDto, Goal>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CurrentCount, o => o.Ignore())
            .ForMember(d => d.IsCompleted, o => o.Ignore());
        }

    }
}
