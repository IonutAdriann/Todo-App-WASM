using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class TodoEfcDao : ITodoDao
{
    private readonly TodoContext _context;

    public TodoEfcDao(TodoContext context)
    {
        this._context = context;
    }

    public async Task<Todo> CreateAsync(Todo todo)
    {
        EntityEntry<Todo> added = await _context.Todos.AddAsync(todo);
        await _context.SaveChangesAsync();
        return added.Entity;
    }

    public async Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameters)
    {
        IQueryable<Todo> query = _context.Todos.Include(todo => todo.Owner).AsQueryable();

        if (!string.IsNullOrEmpty(searchParameters.Username))
        {
            query = query.Where(todo => todo.Owner.UserName.ToLower().Equals(searchParameters.Username.ToLower()));
            
        }

        if (searchParameters.UserId != null)
        {
            query = query.Where(t => t.Owner.Id == searchParameters.UserId);
        }

        if (searchParameters.CompletedStatus != null)
        {
            query = query.Where(t => t.IsCompleted == searchParameters.CompletedStatus);
        }

        if (!string.IsNullOrEmpty(searchParameters.TitleContains))
        {
            query = query.Where(t => t.Title.ToLower().Contains(searchParameters.TitleContains.ToLower()));
        }

        List<Todo> result = await query.ToListAsync();
        return result;
    }

    public async Task UpdateAsync(Todo todo)
    {
        _context.ChangeTracker.Clear();
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<Todo?> GetByIdAsync(int todoId)
    {
        Todo? found = await _context.Todos
            .AsNoTracking()
            .Include(todo => todo.Owner)
            .SingleOrDefaultAsync(todo => todo.Id == todoId);
        return found;
    }

    public async Task DeleteASync(int id)
    {
        Todo? existing = await GetByIdAsync(id);
        if (existing == null)
        {
            throw new Exception($"Todo with id {id} not found");
        }

        _context.Todos.Remove(existing);
        await _context.SaveChangesAsync();
    }
}