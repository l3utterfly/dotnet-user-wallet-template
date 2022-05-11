using Microsoft.EntityFrameworkCore;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Middleware;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
 
    services.AddDbContext<DataContext>();
 
    services.AddCors();
    services.AddControllers();

    // configure strongly typed settings object
    // app settings
    services.Configure<AppSettings>(options =>
    {
        // disable null check so we can use reflection to set all appsettings
        AppSettings.DisableNullCheck = true;

        builder.Configuration.GetSection("AppSettings").Bind(options);

        // restore null check
        AppSettings.DisableNullCheck = false;
    });

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<UserService, UserService>();
    services.AddScoped<PopulatorService, PopulatorService>();
    services.AddScoped<EmailService, EmailService>();
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();    
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run("http://localhost:4000");