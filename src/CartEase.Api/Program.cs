using CartEase.Api.Configurations;
using CartEase.Application;
using CartEase.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureApplication();
builder.ConfigureLogging();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpLogging();

app.UseCors(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CartEaseContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();