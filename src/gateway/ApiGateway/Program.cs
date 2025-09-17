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

// Nếu cần thêm swagger riêng cho gateway thì đăng ký riêng
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1"
    });
});

var app = builder.Build();

// Swagger UI cho Ocelot
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs"; // bắt buộc với SwaggerForOcelot
});

// Middleware Ocelot
await app.UseOcelot();

app.Run();
