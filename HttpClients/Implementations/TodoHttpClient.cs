using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain.DTOs;
using Domain.Models;
using HttpClients.ClientInterfaces;

namespace HttpClients.Implementations;

public class TodoHttpClient : ITodoService
{
    private readonly HttpClient _client;

    public TodoHttpClient(HttpClient client)
    {
        this._client = client;
    }

    public async Task CreateAsync(TodoCreationDto dto)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/Todos", dto);
        
        if (!response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            throw new Exception(result);
        }
    }

    public async Task<ICollection<Todo>> GetAsync(string? username, int? userId, bool? completedStatus,
        string? titleContains)
    {
        string query = ConstructQuery(username, userId, completedStatus, titleContains);
        
        HttpResponseMessage response = await _client.GetAsync("/Todos" + query);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        ICollection<Todo> todos = JsonSerializer.Deserialize<ICollection<Todo>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return todos;
    }

    private static string ConstructQuery(string? username, int? userId, bool? completedStatus, string? titleContains)
    {
        string query = "";
        if (!string.IsNullOrEmpty(username))
        {
            query += $"?username={username}";
        }

        if (userId != null)
        {
            query += string.IsNullOrEmpty(query) ? "?" : "&";
            query += $"completedStatus={completedStatus}";
        }

        if (!string.IsNullOrEmpty(titleContains))
        {
            query += string.IsNullOrEmpty(query) ? "?" : "&";
            query += $"titleContains={titleContains}";
        }

        return query;
    }

    public async Task UpdateAsync(TodoUpdateDto dto)
    {
        string dtoAsJson = JsonSerializer.Serialize(dto);
        StringContent body = new StringContent(dtoAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PatchAsync("/todos", body);
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }

    public async Task<TodoGetByIdDto> GetByIdAsync(int id)
    {
        HttpResponseMessage response = await _client.GetAsync($"Todos/{id}");

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        TodoGetByIdDto todo = JsonSerializer.Deserialize<TodoGetByIdDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return todo;
    }

    public async Task DeleteAsync(int id)
    {
        HttpResponseMessage response = await _client.DeleteAsync($"Todos/{id}");

        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
        
    }
}