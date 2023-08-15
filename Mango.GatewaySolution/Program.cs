using Mango.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppAuthentication();
builder.Services.AddOcelot();

var app = builder.Build();

app.UseOcelot();

app.Run();
