namespace TaskFlow.Shared.DTOs;

public class UpdateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}
