using Moq;
using TicketingSLA.Application.DTOs.SLAPolicies;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Application.Services;
using TicketingSLA.Application.Validators.SLAPolicies;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using Xunit;

namespace TicketingSLA.UnitTests.Application;

public class SLAPolicyServiceTests
{
    private static SLAPolicyService CreateService(ISLAPolicyRepository repository)
    {
        var mapper = TestMapperFactory.Create();

        return new SLAPolicyService(
            repository,
            new CreateSLAPolicyRequestValidator(),
            new UpdateSLAPolicyRequestValidator(),
            mapper);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenNameIsEmpty()
    {
        var repo = new Mock<ISLAPolicyRepository>();
        var service = CreateService(repo.Object);

        var result = await service.CreateAsync(new CreateSLAPolicyRequest { Name = "", Priority = TicketPriority.Low, ResponseTimeHours = 4 });

        Assert.False(result.IsSuccess);
        repo.Verify(r => r.AddAsync(It.IsAny<SLAPolicy>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WithValidRequest()
    {
        var repo = new Mock<ISLAPolicyRepository>();
        var service = CreateService(repo.Object);

        var result = await service.CreateAsync(new CreateSLAPolicyRequest { Name = "Standard", Priority = TicketPriority.Medium, ResponseTimeHours = 4 });

        Assert.True(result.IsSuccess);
        Assert.Equal("Standard", result.Value!.Name);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenPolicyNotFound()
    {
        var repo = new Mock<ISLAPolicyRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((SLAPolicy?)null);
        var service = CreateService(repo.Object);

        var result = await service.UpdateAsync(Guid.NewGuid(), new UpdateSLAPolicyRequest { Name = "New", ResponseTimeHours = 8 });

        Assert.False(result.IsSuccess);
        Assert.Equal("SLA policy not found.", result.Error);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenPolicyExists()
    {
        var policy = new SLAPolicy("Standard", TicketPriority.Low, 4);
        var repo = new Mock<ISLAPolicyRepository>();
        repo.Setup(r => r.GetByIdAsync(policy.Id)).ReturnsAsync(policy);
        var service = CreateService(repo.Object);

        var result = await service.DeleteAsync(policy.Id);

        Assert.True(result.IsSuccess);
        repo.Verify(r => r.Delete(policy), Times.Once);
    }
}
