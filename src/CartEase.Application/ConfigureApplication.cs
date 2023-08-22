using AutoMapper;
using CartEase.Application.Profiles;
using CartEase.Application.Repository;
using CartEase.Application.Services.Cart;
using CartEase.Application.Services.User;
using CartEase.Application.Validators;
using CartEase.Core.Repository;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CartEase.Application;

public static class ApplicationConfiguration
{
    public static void ConfigureApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(CartItemMappingProfile));

        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IRepository, EfRepository>();
        serviceCollection.AddTransient<ICartService, CartService>();
        
        serviceCollection.AddValidatorsFromAssemblyContaining<CartItemValidator>();
    }
}