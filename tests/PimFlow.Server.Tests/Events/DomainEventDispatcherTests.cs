using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PimFlow.Domain.Events;
using PimFlow.Domain.Enums;
using PimFlow.Server.Events;
using Xunit;

namespace PimFlow.Server.Tests.Events;

/// <summary>
/// Tests para el Domain Event Dispatcher
/// Verifica que los eventos se publiquen correctamente a los handlers
/// </summary>
public class DomainEventDispatcherTests
{
    private readonly Mock<ILogger<DomainEventDispatcher>> _mockLogger;

    public DomainEventDispatcherTests()
    {
        _mockLogger = new Mock<ILogger<DomainEventDispatcher>>();
    }

    [Fact]
    [Trait("Category", "DomainEventDispatcher")]
    [Trait("Type", "Infrastructure")]
    public async Task PublishAsync_WithNullEvent_ShouldLogWarningAndReturn()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(serviceProvider, _mockLogger.Object);

        // Act
        await dispatcher.PublishAsync((IDomainEvent)null!);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("null domain event")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "DomainEventDispatcher")]
    [Trait("Type", "Infrastructure")]
    public async Task PublishAsync_WithValidEvent_ShouldNotThrow()
    {
        // Arrange
        var testEvent = new ArticleCreatedEvent(1, "TEST-SKU", "Test Article", ArticleType.Footwear, "Test Brand");
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(serviceProvider, _mockLogger.Object);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => dispatcher.PublishAsync(testEvent));
        exception.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "DomainEventDispatcher")]
    [Trait("Type", "Infrastructure")]
    public async Task PublishAsync_WithEmptyEventsList_ShouldNotThrow()
    {
        // Arrange
        var events = new List<IDomainEvent>();
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(serviceProvider, _mockLogger.Object);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => dispatcher.PublishAsync(events));
        exception.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "DomainEventDispatcher")]
    [Trait("Type", "Infrastructure")]
    public async Task PublishAsync_WithMultipleEvents_ShouldNotThrow()
    {
        // Arrange
        var event1 = new ArticleCreatedEvent(1, "TEST-SKU-1", "Test Article 1", ArticleType.Footwear, "Test Brand");
        var event2 = new ArticleCreatedEvent(2, "TEST-SKU-2", "Test Article 2", ArticleType.Clothing, "Test Brand");
        var events = new List<IDomainEvent> { event1, event2 };

        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(serviceProvider, _mockLogger.Object);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => dispatcher.PublishAsync(events));
        exception.Should().BeNull();
    }
}
