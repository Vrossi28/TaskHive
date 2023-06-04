using Hangfire;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using TaskHive.Application;
using TaskHive.Infrastructure;
using TaskHive.Infrastructure.Persistence;

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

var app = builder.Build();
app.UseHttpLogging();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<TaskHiveContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
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
});

app.UseHangfireDashboard();

app.Run();
