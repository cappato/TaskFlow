using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskState Status { get; set; } = TaskState.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Foreign keys
    public int? ProjectId { get; set; }
    public int? AssignedToUserId { get; set; }

    // Navigation properties
    public virtual Project? Project { get; set; }
    public virtual User? AssignedToUser { get; set; }
}
