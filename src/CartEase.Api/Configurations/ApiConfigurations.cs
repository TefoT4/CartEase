using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using CartEase.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CartEase.Api.Configurations;

public static class ApiConfigurations
{
    public static void ConfigureDatabase(this IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        var connectionString = builderConfiguration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CartEaseContext>(options =>
            options.UseSqlServer(connectionString));
    }
    
    public static void ConfigureAuthentication(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "cookie";
                options.DefaultSignInScheme = "cookie";
                options.DefaultChallengeScheme = "GitHub"; 
            })
            .AddCookie("cookie")
            .AddOAuth("GitHub", options =>
            {
                var authSettings = builderConfiguration.GetSection("AuthSettings");
                
                options.ClientId = authSettings.GetValue<string>("ClientId") ?? throw new ArgumentNullException("ClientId");
                options.ClientSecret = authSettings.GetValue<string>("ClientSecret") ?? throw new ArgumentNullException("ClientSecret");
                
                options.CallbackPath = "/github-login";

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.SaveTokens = true;
        
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Locality, "location");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user);
                    }
                };
            });
    }

    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) =>
        {
            var seqUrl = ctx.Configuration.GetSection("Seq:Url").Value;

            lc.MinimumLevel.Warning();
            lc.WriteTo.Console();
            if (!string.IsNullOrEmpty(seqUrl))
            {
                lc.WriteTo.Seq(seqUrl);
            }
        });
    
        builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);
    }

}