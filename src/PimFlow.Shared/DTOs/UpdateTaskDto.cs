using PimFlow.Shared.Enums;

namespace PimFlow.Shared.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskState? Status { get; set; }
    public Priority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int? ProjectId { get; set; }
    public int? AssignedToUserId { get; set; }
}
