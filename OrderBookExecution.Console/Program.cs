using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderBookExecution.Console;
using OrderBookExecution.Core.DI;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddTransient<App>();
builder.Services.AddApplicationServices(builder.Configuration);

var host = builder.Build();

var app = host.Services.GetRequiredService<App>();

await app.RunAsync(args);
