using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace Application.Logic;

public class UserLogic : IUserLogic
{
    private readonly IUserDao _userDao;

    public UserLogic(IUserDao userDao)
    {
        this._userDao = userDao;
    }

    public async Task<User> CreateAsync(UserCreationDto dto)
    {
        User? existing = await _userDao.GetByUsernameAsync(dto.UserName);
        if (existing != null)
        {
            throw new Exception("Username already taken!");
        }

        ValidateData(dto);
        User toCreate = new User
        {
            UserName = dto.UserName
        };
        User created = await _userDao.CreateAsync(toCreate);
        return created;
    }

    private static void ValidateData(UserCreationDto userToCreate)
    {
        string userName = userToCreate.UserName;

        if (userName.Length < 3)
            throw new Exception("Username must be at least 3 characters!");

        if (userName.Length > 15)
            throw new Exception("Username must be less than 16 characters!");
    }

    public Task<IEnumerable<User>> GetAsync(SearchUserParametersDto searchParameters)
    {
        return _userDao.GetAsync(searchParameters);
    }
}