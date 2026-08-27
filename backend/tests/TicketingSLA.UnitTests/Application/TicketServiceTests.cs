using Moq;
using TicketingSLA.Application.DTOs.Tickets;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Application.Services;
using TicketingSLA.Application.Validators.Tickets;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using Xunit;

namespace TicketingSLA.UnitTests.Application;

public class TicketServiceTests
{
    private static TicketService CreateService(
        ITicketRepository ticketRepository,
        ISLAPolicyRepository slaPolicyRepository,
        ICurrentUserService currentUserService)
    {
        var mapper = TestMapperFactory.Create();

        return new TicketService(
            ticketRepository,
            slaPolicyRepository,
            currentUserService,
            new CreateTicketRequestValidator(),
            new UpdateTicketRequestValidator(),
            new AssignTicketRequestValidator(),
            mapper);
    }

    [Fact]
    public async Task CreateTicketAsync_ReturnsFailure_WhenNoSlaPolicyConfigured()
    {
        var policyRepo = new Mock<ISLAPolicyRepository>();
        policyRepo.Setup(r => r.GetByPriorityAsync(TicketPriority.High)).ReturnsAsync((SLAPolicy?)null);

        var service = CreateService(new Mock<ITicketRepository>().Object, policyRepo.Object, new Mock<ICurrentUserService>().Object);

        var result = await service.CreateTicketAsync(new CreateTicketRequest { Title = "Test", Description = "D", Priority = TicketPriority.High });

        Assert.False(result.IsSuccess);
        Assert.Contains("No SLA policy configured", result.Error);
    }

    [Fact]
    public async Task CreateTicketAsync_StampsCreatedByUserId_OnSuccess()
    {
        Ticket? addedTicket = null;
        var ticketRepo = new Mock<ITicketRepository>();
        ticketRepo.Setup(r => r.AddAsync(It.IsAny<Ticket>()))
            .Callback<Ticket>(t => addedTicket = t)
            .Returns(Task.CompletedTask);

        var policy = new SLAPolicy("Standard", TicketPriority.Medium, 4);
        var policyRepo = new Mock<ISLAPolicyRepository>();
        policyRepo.Setup(r => r.GetByPriorityAsync(TicketPriority.Medium)).ReturnsAsync(policy);

        var userId = Guid.NewGuid();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.UserId).Returns(userId);

        var service = CreateService(ticketRepo.Object, policyRepo.Object, currentUser.Object);

        var result = await service.CreateTicketAsync(
            new CreateTicketRequest { Title = "Broken printer", Description = "desc", Priority = TicketPriority.Medium });

        Assert.True(result.IsSuccess);
        Assert.NotNull(addedTicket);
        Assert.Equal(userId, addedTicket!.CreatedByUserId);
        ticketRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTicketAsync_ReturnsFailure_WhenTitleIsEmpty()
    {
        var policyRepo = new Mock<ISLAPolicyRepository>();

        var service = CreateService(new Mock<ITicketRepository>().Object, policyRepo.Object, new Mock<ICurrentUserService>().Object);

        var result = await service.CreateTicketAsync(new CreateTicketRequest { Title = "", Description = "d", Priority = TicketPriority.Low });

        Assert.False(result.IsSuccess);
        policyRepo.Verify(r => r.GetByPriorityAsync(It.IsAny<TicketPriority>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_FiltersToOwnTickets_WhenClientRole()
    {
        var userId = Guid.NewGuid();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Role).Returns("Client");
        currentUser.Setup(c => c.UserId).Returns(userId);

        var ticketRepo = new Mock<ITicketRepository>();
        ticketRepo
            .Setup(r => r.GetPagedAsync(null, null, userId, 1, 50))
            .ReturnsAsync((Enumerable.Empty<Ticket>(), 0));

        var service = CreateService(ticketRepo.Object, new Mock<ISLAPolicyRepository>().Object, currentUser.Object);

        var result = await service.GetAllAsync(null, null, 1, 50);

        Assert.True(result.IsSuccess);
        ticketRepo.Verify(r => r.GetPagedAsync(null, null, userId, 1, 50), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFailure_WhenClientRequestsAnotherUsersTicket()
    {
        var policy = new SLAPolicy("Standard", TicketPriority.Medium, 4);
        var ticket = new Ticket("Title", "Desc", policy, createdByUserId: Guid.NewGuid());

        var ticketRepo = new Mock<ITicketRepository>();
        ticketRepo.Setup(r => r.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Role).Returns("Client");
        currentUser.Setup(c => c.UserId).Returns(Guid.NewGuid());

        var service = CreateService(ticketRepo.Object, new Mock<ISLAPolicyRepository>().Object, currentUser.Object);

        var result = await service.GetByIdAsync(ticket.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Ticket not found.", result.Error);
    }

    [Fact]
    public async Task AssignTicketAsync_ReturnsFailure_WhenAgentIdIsEmpty()
    {
        var ticketRepo = new Mock<ITicketRepository>();

        var service = CreateService(ticketRepo.Object, new Mock<ISLAPolicyRepository>().Object, new Mock<ICurrentUserService>().Object);

        var result = await service.AssignTicketAsync(Guid.NewGuid(), new AssignTicketRequest { AgentId = Guid.Empty });

        Assert.False(result.IsSuccess);
        ticketRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}
