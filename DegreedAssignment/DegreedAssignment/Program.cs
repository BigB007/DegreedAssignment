using Autofac;
using Autofac.Extensions.DependencyInjection;
using DegreedAssignment.Config;
using DegreedAssignment.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Register Autofac modules
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new AutofacModule());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Optionally, add development-specific configurations here
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
