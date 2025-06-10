using System.ComponentModel.DataAnnotations;
using PimFlow.Shared.Enums;

namespace PimFlow.Shared.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime? DueDate { get; set; }

    public int? ProjectId { get; set; }

    public int? AssignedToUserId { get; set; }
}
