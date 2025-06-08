using FluentAssertions;
using Moq;
using TaskFlow.Server.Models;
using TaskFlow.Server.Repositories;
using TaskFlow.Server.Services;
using TaskFlow.Shared.DTOs;
using TaskFlow.Shared.Enums;
using Xunit;

namespace TaskFlow.Server.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Task 1", Description = "Description 1", Status = TaskState.Pending, Priority = Priority.Medium, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = 2, Title = "Task 2", Description = "Description 2", Status = TaskState.InProgress, Priority = Priority.High, CreatedAt = DateTime.UtcNow }
        };

        _mockTaskRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetAllTasksAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Task 1");
        result.Last().Title.Should().Be("Task 2");
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            Status = TaskState.Pending,
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockTaskRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _taskService.GetTaskByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateAndReturnTask()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "New Description",
            Priority = Priority.High
        };

        var createdTask = new TaskItem
        {
            Id = 1,
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Priority = createTaskDto.Priority,
            Status = TaskState.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockTaskRepository.Setup(x => x.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateTaskAsync(createTaskDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Description.Should().Be("New Description");
        result.Priority.Should().Be(Priority.High);
        result.Status.Should().Be(TaskState.Pending);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithValidId_ShouldUpdateAndReturnTask()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original Description",
            Status = TaskState.Pending,
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow
        };

        var updateTaskDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Status = TaskState.InProgress
        };

        var updatedTask = new TaskItem
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Original Description",
            Status = TaskState.InProgress,
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(updatedTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(1, updateTaskDto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Status.Should().Be(TaskState.InProgress);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockTaskRepository.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _taskService.DeleteTaskAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTaskAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockTaskRepository.Setup(x => x.DeleteAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _taskService.DeleteTaskAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}
