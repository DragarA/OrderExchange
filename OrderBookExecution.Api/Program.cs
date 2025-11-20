using Scalar.AspNetCore;
using OrderBookExecution.Core.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapHealthChecks("/healthz");

app.Run();

public partial class Program { }