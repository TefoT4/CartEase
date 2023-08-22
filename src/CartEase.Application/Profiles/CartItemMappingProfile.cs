using AutoMapper;
using CartEase.Application.Domain;
using CartEase.Models;

namespace CartEase.Application.Profiles;

public class CartItemMappingProfile : Profile
{
    public CartItemMappingProfile()
    {
        CreateMap<CartItem, CartItemModel>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignoring UserId as it's not in CartItem
            .ForMember(dest => dest.User, opt => opt.Ignore())   // Ignoring User navigation property
            .ForMember(dest => dest.ItemImages, opt => opt.Ignore()); // Ignoring ItemImages navigation property

        CreateMap<CartItemModel, CartItem>();
    }
}