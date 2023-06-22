using Hangfire;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using TaskHive.Application;
using TaskHive.Application.Services.SignalR;
using TaskHive.Application.Services.SignalR.Hubs;
using TaskHive.Infrastructure;
using TaskHive.Infrastructure.Persistence;
using TaskHive.WebApi.Clients.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Host.ConfigureAppConfiguration(options => options.AddEnvironmentVariables());
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
});
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ISignalRContract, SignalRClient>(sp =>
{
    ILogger<SignalRClient> logger = sp.GetRequiredService<ILogger<SignalRClient>>();
    string url = builder.Configuration["SignalRService:BaseUrl"];
    string accessToken = builder.Configuration["SignalRService:AccessToken"];

    return new SignalRClient(logger, url, accessToken);
});

var app = builder.Build();
app.UseHttpLogging();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var policy = Policy
            .Handle<SqlException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, _, retryAttempt, _) =>
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning($"Retry #{retryAttempt} due to: {exception.Message}");
                });
    await policy.ExecuteAsync(() =>
    {
        try
        {
            var context = services.GetRequiredService<TaskHiveContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        catch (SqlException ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Failed to connect to the database.");
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }

        return Task.CompletedTask;
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskHive.WebApi v1"));
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.UseRouting();
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins:Origins").Get<string[]>();

app.UseCors(builder =>
{
    builder.WithOrigins(allowedOrigins)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notification-hub");
});

app.UseHangfireDashboard();

app.Run();
