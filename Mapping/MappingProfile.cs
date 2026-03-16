using AutoMapper;
using HybridApp.DTOs;
using System.Data;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        // Entity → DTO
        CreateMap<Role, RoleDto>();

        // DTO → Entity
        CreateMap<RoleDto, Role>();


    }
}