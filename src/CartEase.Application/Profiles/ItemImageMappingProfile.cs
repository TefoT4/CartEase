using AutoMapper;
using CartEase.Application.Domain;
using CartEase.Models;

namespace CartEase.Application.Profiles;

public class ItemImageMappingProfile : Profile
{
    public ItemImageMappingProfile()
    {
        CreateMap<ItemImage, ItemImageModel>();
        CreateMap<ItemImageModel, ItemImage>();
    }
}