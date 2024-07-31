using AutoMapper;
using BankProject.API.DTOs;

namespace BankProject.API;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Account, AccountDto>().ReverseMap();
        CreateMap<CreateUserRequestDto, User>().ReverseMap();
    }
}