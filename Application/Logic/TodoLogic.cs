using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace Application.Logic;

public class TodoLogic : ITodoLogic
{
    private readonly ITodoDao _todoDao;
    private readonly IUserDao _userDao;

    public TodoLogic(ITodoDao todoDao, IUserDao userDao)
    {
        this._todoDao = todoDao;
        this._userDao = userDao;
    }

    public async Task<Todo> CreateAsync(TodoCreationDto dto)
    {
        User? user = await _userDao.GetByIdAsync(dto.OwnerId);
        if (user == null)
        {
            throw new Exception($"User with id {dto.OwnerId} was not found.");
        }

        ValidateTodo(dto);
        Todo todo = new Todo(user, dto.Title);
        Todo created = await _todoDao.CreateAsync(todo);
        return created;
    }

    private void ValidateTodo(TodoCreationDto dto)
    {
        if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Title cannot be empty");
    }
    
    public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameters)
    {
        return _todoDao.GetAsync(searchParameters);
    }

    public async Task UpdateAsync(TodoUpdateDto dto)
    {
        Todo? existing = await _todoDao.GetByIdAsync(dto.Id);

        if (existing == null)
        {
            throw new Exception($"Todo with id {dto.Id} not found!");
        }

        User? user = null;
        if (dto.OwnerId != null)
        {
            user = await _userDao.GetByIdAsync((int)dto.OwnerId);
            if (user == null)
            {
                throw new Exception("Cannot un-complete a completed Todo");
            } 
        }

        if (dto.IsCompleted != null && existing.IsCompleted && !(bool)dto.IsCompleted)
        {
            throw new Exception("Cannot un-complete a completed Todo");
        }

        User userToUse = user ?? existing.Owner;
        string titleToUse = dto.Title ?? existing.Title;
        bool completedToUse = dto.IsCompleted ?? existing.IsCompleted;

        Todo updated = new(userToUse, titleToUse)
        {
            IsCompleted = completedToUse,
            Id = existing.Id,
        };
        
        ValidateTodo(updated);

        await _todoDao.UpdateAsync(updated);
    }

    private void ValidateTodo(Todo dto)
    {
        if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Title cannot be empty");
    }

    public async Task DeleteAsync(int id)
    {
        Todo? todo = await _todoDao.GetByIdAsync(id);
        if (todo == null)
        {
            throw new Exception($"Todo with ID {id} was not found!");
        }

        if (!todo.IsCompleted)
        {
            throw new Exception("Cannot delete un-completed todo");
        }

        await _todoDao.DeleteASync(id);
    }

    public async Task<TodoGetByIdDto> GetByIdAsync(int id)
    {
        Todo? todo = await _todoDao.GetByIdAsync(id);
        if (todo == null)
        {
            throw new Exception($"Todo with {id} not found!");
        }

        return new TodoGetByIdDto(todo.Id, todo.Owner.UserName, todo.Title, todo.IsCompleted);
    }

}
