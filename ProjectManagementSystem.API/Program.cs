using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystem.API.AuthService.Configuration;
using ProjectManagementSystem.API.AuthService.Managers;
using ProjectManagementSystem.API.ProjectService.Database;
using ProjectManagementSystem.API.ProjectService.Managers;
using ProjectManagementSystem.API.TaskService.Database;
using ProjectManagementSystem.API.TaskService.Managers;
using ProjectManagementSystem.Infrastructure.Database;
using ProjectManagementSystem.Common.Middlewares;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); 
builder.Logging.AddConsole();    

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// ✅ Add controllers
builder.Services.AddControllers();

// ✅ Configure Cognito AuthService
var awsOptions = builder.Configuration.GetSection("Cognito").Get<AwsConfig>();

// ✅ Register AuthManager
builder.Services.AddSingleton<IAmazonCognitoClient>(provider =>
    new AmazonCognitoClientWrapper(
        awsOptions.AWSAccessKeyId,
        awsOptions.AWSSecretAccessKey,
        awsOptions.Region));

//✅ Add DbContext 
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register Services & Repositories
builder.Services.AddSingleton<IAuthManager, AuthManager>();
builder.Services.AddScoped<ApplicationContext>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectManager, ProjectManager>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskManager, TaskManager>();

// ✅ Configure authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://cognito-idp.{awsOptions.Region}.amazonaws.com/{awsOptions.UserPoolId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://cognito-idp.{awsOptions.Region}.amazonaws.com/{awsOptions.UserPoolId}",
            ValidateAudience = false,
            ValidAudience = awsOptions.ClientId,
            ValidateLifetime = true,
            RoleClaimType = "cognito:groups",
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// ✅ Apply pending migrations with retries
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    
    // retry policy: try 5 times, waiting 20 seconds between each attempt (sql server takes long time to mount)
    RetryPolicy retryPolicy = Policy
        .Handle<SqlException>()
        .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(20), (exception, timeSpan, retryCount, context) =>
        {
            // Log the failure
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning($"Attempt {retryCount}: Unable to connect to the database. Retrying in {timeSpan.TotalSeconds} seconds...");
        });
    
    // Use the retry policy to apply migrations
    retryPolicy.Execute(() =>
    {
        context.Database.Migrate();
    });
}

app.UseRouting();
app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();
