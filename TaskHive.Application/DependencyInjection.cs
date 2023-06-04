using System.Reflection;
using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskHive.Application.Services.Attachments;
using TaskHive.Application.Services.Hangfire;
using TaskHive.Application.Services.HealthCheck;
using TaskHive.Application.Services.Security;
using TaskHive.Core.Enums;

namespace TaskHive.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            #region HealthCheck
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database");
            #endregion

            #region Hangfire
            services.AddHangfire(setup => setup.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddTransient<HangfireJobActivator>();
            var serviceProvider = services.BuildServiceProvider();
            GlobalConfiguration.Configuration.UseActivator<HangfireJobActivator>(new HangfireJobActivator(serviceProvider));
            #endregion

            #region Attachments
            services.AddScoped<IStorageService, StorageService>();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });
            #endregion

            #region Security
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration[SecurityConstants.JwtSecret])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy =>
                    policy.RequireRole(RoleType.Administrator.ToString()));

                options.AddPolicy("RequireProjectManagerRole", policy =>
                    policy.RequireRole(RoleType.ProjectManager.ToString()));

                options.AddPolicy("RequireTeamMemberRole", policy =>
                    policy.RequireRole(RoleType.TeamMember.ToString()));

                options.AddPolicy("RequireObserverRole", policy =>
                    policy.RequireRole(RoleType.Observer.ToString()));

                options.AddPolicy("RequireCustomerRole", policy =>
                    policy.RequireRole(RoleType.Customer.ToString()));

                options.AddPolicy("RequireAdminOrProjManagerRole", policy =>
                    policy.RequireRole(RoleType.ProjectManager.ToString(), RoleType.Administrator.ToString()));

                options.AddPolicy("RequireAdminProjManagerOrTeamMemberRole", policy =>
                    policy.RequireRole(RoleType.ProjectManager.ToString(), RoleType.Administrator.ToString(), RoleType.TeamMember.ToString()));
            });
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskHive.WebApi", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert your JWT following this way: Bearer {your token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                c.DescribeAllParametersInCamelCase();

                var startupAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => 
                assembly.ManifestModule.Name.Contains("TaskHive.WebApi"));

                if (startupAssembly != null)
                {
                    var xmlFilename = $"{startupAssembly.GetName().Name}.xml";
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                }
            });

            #endregion

            return services;
        }
    }
}