using AutoMapper;
using Server.Models;
using Server.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ServiceTable, ServiceDTO>();
        CreateMap<User, UserDTO>();
    }
}
