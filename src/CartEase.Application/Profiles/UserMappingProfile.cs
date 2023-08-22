using AutoMapper;
using CartEase.Application.Domain;
using CartEase.Models;

namespace CartEase.Application.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserModel>()
            .ForMember(dest => dest.CartItems, opt => opt.Ignore()); // Ignore CartItems as it's not in User

        CreateMap<UserModel, User>()
            .ForMember(dest => dest.CartItems, opt => opt.Ignore()); // Ignore CartItems as it's not in UserModel
    }
}