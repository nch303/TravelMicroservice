//using Microsoft.OpenApi.Models;
//using Ocelot.DependencyInjection;
//using Ocelot.Middleware;
//using MMLib.SwaggerForOcelot.DependencyInjection;
//using MMLib.SwaggerForOcelot.Middleware;

//var builder = WebApplication.CreateBuilder(args);

//// Add Ocelot
//builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
//builder.Services.AddOcelot(builder.Configuration);

//// Add Swagger for Ocelot
//builder.Services.AddSwaggerForOcelot(builder.Configuration)
//    .AddSwaggerGen(opt =>
//    {
//        opt.SwaggerDoc("v1", new OpenApiInfo
//        {
//            Title = "Api Gateway",
//            Version = "v1"
//        });
//    });


//var app = builder.Build();

//// Enable SwaggerForOcelot
//app.UseSwaggerForOcelotUI();

//// Enable Ocelot
//await app.UseOcelot();

//app.Run();


using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot config
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ✅ Đăng ký SwaggerGen riêng biệt (chỉ để satisfy dependency)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1"
    });
});

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Add SwaggerForOcelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// ✅ SwaggerForOcelotUI
app.UseSwaggerForOcelotUI(opt =>
{
    // Endpoint này để SwaggerForOcelot lấy swagger.json từ services
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

// ✅ Ocelot middleware (phải luôn sau UseSwaggerForOcelotUI)
await app.UseOcelot();

app.Run();
