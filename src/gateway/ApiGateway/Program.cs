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

// load cấu hình ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Add Swagger + SwaggerForOcelot
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api Gateway",
        Version = "v1"
    });
});

builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// SwaggerForOcelot UI
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs"; // mặc định
});

// Ocelot
await app.UseOcelot();

app.Run();
