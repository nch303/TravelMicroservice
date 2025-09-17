using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load cấu hình Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Đăng ký Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Đăng ký Swagger + SwaggerForOcelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// Swagger UI cho Ocelot
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs"; // bắt buộc với SwaggerForOcelot
});

// Middleware Ocelot
await app.UseOcelot();

app.Run();
