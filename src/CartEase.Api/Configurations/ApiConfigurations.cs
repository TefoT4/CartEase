using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.OpenApi.Models;

namespace CartEase.Api.Configurations;

public static class ApiConfigurations
{
    public static void ConfigureAuthentication(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "github";
            })
            .AddCookie("cookie")
            .AddOAuth("github", options =>
            {
                var authSettings = builderConfiguration.GetSection("AuthSettings");
                
                options.ClientId = authSettings.GetValue<string>("ClientId") ?? throw new ArgumentNullException("ClientId");
                options.ClientSecret = authSettings.GetValue<string>("ClientSecret") ?? throw new ArgumentNullException("ClientSecret");
                options.CallbackPath = new PathString("/signin-github");
                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.SaveTokens = true;
                
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Locality, "location");

                options.Events = new OAuthEvents()
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(json.RootElement);
                    }
                };
            });
    }

    public static void ConfigureSwaggerWithOauth(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            // Add authentication to Swagger UI
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("/swagger/index.html", UriKind.Relative),
                        TokenUrl = new Uri("https://github.com/login/oauth/access_token", UriKind.Absolute),
                    },
                },
            });
        });
    }
}