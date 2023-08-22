using CartEase.Application.Domain;
using CartEase.Core;

namespace CartEase.Application.Services.User;

public interface IUserService
{
    Task<ServiceResponse<Domain.User>> CreateAsync(Domain.User user);
    Task<bool> ExistAsync(string identityName);
}