using CartEase.Application.Repository;
using CartEase.Application.Service;
using CartEase.Application.Validators;
using CartEase.Core.Repository;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CartEase.Application;

public static class ApplicationConfiguration
{
    public static void ConfigureApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRepository, EfRepository>();
        serviceCollection.AddTransient<ICartService, CartService>();
        
        serviceCollection.AddValidatorsFromAssemblyContaining<CartItemValidator>();
    }
}