using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TicketingSLA.Application.Mappings;

namespace TicketingSLA.UnitTests.Application;

internal static class TestMapperFactory
{
    public static IMapper Create()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddAutoMapper(cfg => { }, typeof(TicketMappingProfile).Assembly);
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
}
