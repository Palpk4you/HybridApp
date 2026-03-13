using AutoMapper;
using HybridApp.DTOs;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        // Map Role entity to RoleDto
        CreateMap<Role, RoleDto>();

        // If you need reverse mapping
        CreateMap<RoleDto, Role>();

    }
}