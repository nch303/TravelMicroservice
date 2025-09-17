using Microsoft.AspNetCore.Builder;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Thêm Ocelot
builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot(builder.Configuration);

// Thêm Swagger với cấu hình đầy đủ
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiGateway",
        Version = "v1",
        Description = "Gateway for microservices"
    });
});

var app = builder.Build();

// Sử dụng Ocelot
app.UseOcelot().Wait();

// Sử dụng Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiGateway V1");
    c.SwaggerEndpoint("http://localhost:7019/swagger/v1/swagger.json", "UserService API V1"); 
    //Add Service Here
});

app.Run();