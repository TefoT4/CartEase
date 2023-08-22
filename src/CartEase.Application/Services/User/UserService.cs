using AutoMapper;
using CartEase.Application.Validators;
using CartEase.Core;
using CartEase.Core.Repository;
using CartEase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CartEase.Application.Services.User;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly UserValidator _userValidator;
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger, IMapper mapper, IRepository repository, UserValidator userValidator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _userValidator = userValidator;
    }

    public async Task<ServiceResponse<Domain.User>> CreateAsync(Domain.User user)
    {
        try
        {
            var validationResult = await _userValidator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Validation errors encountered while adding a user {user}: {ValidationErrors}", user, validationErrors);
                return new ServiceResponse<Domain.User>()
                {
                    IsSuccessful = false, Errors = validationErrors
                };
            }
            
            var userModel = _mapper.Map<UserModel>(user);
            var result = await _repository.AddAsync<UserModel>(userModel);
            var addedUser = _mapper.Map<Domain.User>(result);
            
            _logger.LogInformation("User added successfully {User}", addedUser);
            return new ServiceResponse<Domain.User>() { IsSuccessful = true, Data = addedUser };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while adding a user {UserI", user);
            return await Task.FromResult(new ServiceResponse<Domain.User>() { IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<bool> ExistAsync(string identityName)
    {
        return await _repository.GetAll<Domain.User>().AnyAsync(x => x.Username == identityName);
    }
}