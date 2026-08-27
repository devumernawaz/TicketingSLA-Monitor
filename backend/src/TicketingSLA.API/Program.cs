using Microsoft.AspNetCore.Identity;
using TicketingSLA.Application;
using TicketingSLA.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = "simple";
});
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

var app = builder.Build();
app.UseCors("AllowFrontend");
app.UseMiddleware<TicketingSLA.API.Middleware.CorrelationIdMiddleware>();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
