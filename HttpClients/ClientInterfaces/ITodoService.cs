using Domain.DTOs;
using Domain.Models;

namespace HttpClients.ClientInterfaces;

public interface ITodoService
{
    Task CreateAsync(TodoCreationDto dto);

    Task<ICollection<Todo>> GetAsync(
        string? username,
        int? userId, 
        bool? completedStatus,
        string? contains
    );

    Task UpdateAsync(TodoUpdateDto dto);

    Task<TodoGetByIdDto> GetByIdAsync(int id);

    Task DeleteAsync(int id);
}