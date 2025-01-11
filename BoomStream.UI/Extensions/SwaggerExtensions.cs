using Domain.Abstraction.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BoomStream.UI.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerGenWithBearer(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("home", new OpenApiInfo { Title = "BoomStream.UI V1", Version = "v1" });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "home" && apiDesc.RelativePath.StartsWith("api/v1/home"))
                    return true;


                return false;
            });

            // JWT uchun xavfsizlik ta'rifi
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = @"JWT Authorization header using the Bearer scheme. <br />
                                Enter 'Bearer' [space] and then your token in the text input below. <br /><br />
                                Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            // Xavfsizlik talabi
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                        new List<string>()
                    }
            });
            options.CustomSchemaIds(type => type.FullName);
            options.EnableAnnotations(); // Annotations-ni yoqish

            /*// Server URL'larini qo'shish
            options.AddServer(new OpenApiServer
            {
                Url = "https://api.example.uz/", // Asosiy domen
                Description = "Production Server"
            });*/

            options.AddServer(new OpenApiServer
            {
                Url = "https://localhost:7175", // Lokal server HTTPS
                Description = "Development Server HTTPS"
            });

            options.AddServer(new OpenApiServer
            {
                Url = "http://localhost:5255", // Lokal server HTTP
                Description = "Development Server HTTP"
            });

        });

    }
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOption>(configuration.GetSection("JwtOptions"));

        var signingKeyString = configuration["JwtOptions:Key"];
        if (string.IsNullOrEmpty(signingKeyString))
        {
            throw new ArgumentNullException(nameof(signingKeyString), "Signing key cannot be null or empty.");
        }

        var signingKey = Encoding.UTF8.GetBytes(signingKeyString);

        services.AddSingleton(_ => new SymmetricSecurityKey(signingKey));

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = configuration["JwtOptions:Issuer"],
                ValidAudience = configuration["JwtOptions:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = services.BuildServiceProvider().GetRequiredService<SymmetricSecurityKey>(),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }
    public static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwt(configuration);
    }
}
