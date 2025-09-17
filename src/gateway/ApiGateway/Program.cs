using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Add Swagger for Ocelot
builder.Services.AddSwaggerForOcelot(builder.Configuration)
    .AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Api Gateway",
            Version = "v1"
        });
    });


var app = builder.Build();

// Enable SwaggerForOcelot
app.UseSwaggerForOcelotUI();

// Enable Ocelot
await app.UseOcelot();

app.Run();

