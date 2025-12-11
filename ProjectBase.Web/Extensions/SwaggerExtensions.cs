using Domain.Abstraction.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace ProjectBase.WebApi.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerGenWithBearer(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        // Swagger enabled/disabled check
        var swaggerEnabled = configuration.GetValue<bool>("SwaggerSettings:Enabled", true);

        if (!swaggerEnabled)
        {
            return services;
        }

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = configuration["SwaggerSettings:Title"] ?? "ProjectBase API",
                Version = configuration["SwaggerSettings:Version"] ?? "v1",
                Description = configuration["SwaggerSettings:Description"] ?? "ProjectBase Web API",
                Contact = new OpenApiContact
                {
                    Name = configuration["SwaggerSettings:ContactName"],
                    Email = configuration["SwaggerSettings:ContactEmail"],
                    Url = string.IsNullOrEmpty(configuration["SwaggerSettings:ContactUrl"])
                        ? null
                        : new Uri(configuration["SwaggerSettings:ContactUrl"]!)
                }
            });

            // JWT Bearer Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"JWT Authorization header using the Bearer scheme. <br />
                                Enter 'Bearer' [space] and then your token in the text input below. <br /><br />
                                Example: 'Bearer 12345abcdef'",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            options.EnableAnnotations();
            options.UseAllOfToExtendReferenceSchemas();

            ConfigureSwaggerServers(options, environment, configuration);
        });

        return services;
    }

    private static void ConfigureSwaggerServers(
        SwaggerGenOptions options,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        if (environment.IsProduction())
        {
            AddProductionServer(options, configuration);
        }
        else if (environment.IsStaging())
        {
            AddStagingServer(options, configuration);
        }
        else // Development
        {
            AddDevelopmentServer(options, configuration);
        }
    }

    private static void AddProductionServer(SwaggerGenOptions options, IConfiguration configuration)
    {
        var productionUrl = configuration["SwaggerSettings:ProductionUrl"];

        if (string.IsNullOrEmpty(productionUrl))
        {
            var urls = configuration["ASPNETCORE_URLS"]
                ?? configuration["Kestrel:Endpoints:Https:Url"]
                ?? configuration["Kestrel:Endpoints:Http:Url"];

            if (!string.IsNullOrEmpty(urls))
            {
                productionUrl = urls.Split(';')[0];
            }
        }

        if (!string.IsNullOrEmpty(productionUrl))
        {
            options.AddServer(new OpenApiServer
            {
                Url = productionUrl,
                Description = "Production Server"
            });
        }
    }

    private static void AddStagingServer(SwaggerGenOptions options, IConfiguration configuration)
    {
        var stagingUrl = configuration["SwaggerSettings:StagingUrl"];

        if (string.IsNullOrEmpty(stagingUrl))
        {
            var urls = configuration["ASPNETCORE_URLS"]
                ?? configuration["Kestrel:Endpoints:Https:Url"]
                ?? configuration["Kestrel:Endpoints:Http:Url"];

            if (!string.IsNullOrEmpty(urls))
            {
                stagingUrl = urls.Split(';')[0];
            }
        }

        if (!string.IsNullOrEmpty(stagingUrl))
        {
            options.AddServer(new OpenApiServer
            {
                Url = stagingUrl,
                Description = "Staging Server"
            });
        }
    }

    private static void AddDevelopmentServer(SwaggerGenOptions options, IConfiguration configuration)
    {
        var servers = new List<OpenApiServer>();

        var httpUrl = configuration["Kestrel:Endpoints:Http:Url"];
        if (!string.IsNullOrEmpty(httpUrl))
        {
            var uri = new Uri(httpUrl);
            var host = uri.Host == "0.0.0.0" || uri.Host == "::" ? "localhost" : uri.Host;

            servers.Add(new OpenApiServer
            {
                Url = $"{uri.Scheme}://{host}:{uri.Port}",
                Description = "Development Server (HTTP)"
            });
        }

        var httpsUrl = configuration["Kestrel:Endpoints:Https:Url"];
        if (!string.IsNullOrEmpty(httpsUrl))
        {
            var uri = new Uri(httpsUrl);
            var host = uri.Host == "0.0.0.0" || uri.Host == "::" ? "localhost" : uri.Host;

            servers.Add(new OpenApiServer
            {
                Url = $"{uri.Scheme}://{host}:{uri.Port}",
                Description = "Development Server (HTTPS)"
            });
        }

        var aspnetUrls = configuration["ASPNETCORE_URLS"];
        if (!string.IsNullOrEmpty(aspnetUrls))
        {
            var urls = aspnetUrls.Split(';');
            foreach (var url in urls)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var uri = new Uri(url);
                    var host = uri.Host == "0.0.0.0" || uri.Host == "::" ? "localhost" : uri.Host;

                    servers.Add(new OpenApiServer
                    {
                        Url = $"{uri.Scheme}://{host}:{uri.Port}",
                        Description = $"Development Server ({uri.Scheme.ToUpper()})"
                    });
                }
            }
        }

        var uniqueServers = servers
            .GroupBy(s => s.Url)
            .Select(g => g.First())
            .ToList();

        foreach (var server in uniqueServers)
        {
            options.AddServer(server);
        }
    }

    public static IServiceCollection AddJwt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOption>(configuration.GetSection("JwtSettings"));

        var jwtSettings = configuration.GetSection("JwtSettings");
        var signingKey = jwtSettings["SecretKey"];

        if (string.IsNullOrEmpty(signingKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is not configured in appsettings.json");
        }

        if (signingKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 characters long");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        services.AddSingleton(key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = !configuration.GetValue<bool>("JwtSettings:AllowHttpInDevelopment", true);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = key,

                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}