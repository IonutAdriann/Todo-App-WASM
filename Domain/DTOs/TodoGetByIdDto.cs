namespace Domain.DTOs;

public class TodoGetByIdDto
{
    public int Id { get; }
    public string OwnerName { get; }
    public string Title { get; }
    public bool IsCompleted { get; }

    public TodoGetByIdDto(int id, string ownerName, string title, bool isCompleted)
    {
        Id = id;
        OwnerName = ownerName;
        Title = title;
        IsCompleted = isCompleted;
    }
}