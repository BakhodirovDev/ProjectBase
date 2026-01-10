using Application.Extensions;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.HttpOverrides;
using ProjectBase.WebApi.Extensions;
using ProjectBase.WebApi.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddDependencyInjection(builder.Configuration,builder.Environment); 

builder.Host.UseSerilog();

var app = builder.Build();

// Seed data only in Development or if explicitly enabled
if (builder.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("SeedSettings:EnableInProduction"))
{
    await app.ValidateDatabaseConnectionAsync();
    await app.ApplyMigrationsAsync(maxRetries: 5);
    await SeedDefaultEnums.SeedAsync(app);
    await SeedDefaultInfo.SeedAsync(app);
    await SeedDefaultPersonAndUser.SeedAsync(app);
    await SeedPermissionsAndRoles.SeedAsync(app);
}

app.UseRateLimiter();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger - enabled check
var swaggerEnabled = app.Configuration.GetValue<bool>("SwaggerSettings:Enabled", true);

var allowedSwaggerIps = builder.Configuration
    .GetSection("SwaggerSettings:AllowedSwaggerIPs")
    .Get<string[]>();

if (swaggerEnabled && (app.Environment.IsDevelopment() || app.Environment.IsStaging()))
{
    var swaggerRoutePrefix = app.Configuration["SwaggerSettings:RoutePrefix"] ?? "swagger";
    var swaggerVersion = app.Configuration["SwaggerSettings:Version"] ?? "v1";

    app.UseWhen(
        context => context.Request.Path.StartsWithSegments("/" + swaggerRoutePrefix) ||
                   context.Request.Path.StartsWithSegments("/swagger"),
        appBuilder =>
        {
            appBuilder.Use(async (context, next) =>
            {
                var remoteIp = context.GetClientIpAddress().ToString();

                if (remoteIp == null || !allowedSwaggerIps!.Contains(remoteIp))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Swaggerga kirish uchun ruxsat yo'q. Sizning IP: " + remoteIp);
                    return;
                }

                await next();
            });
        }
    );

    app.UseSwagger(c =>
    {
        c.RouteTemplate = $"{swaggerRoutePrefix}/{{documentName}}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/{swaggerRoutePrefix}/{swaggerVersion}/swagger.json", $"ProjectBase API {swaggerVersion}");
        c.RoutePrefix = swaggerRoutePrefix + "/" + swaggerVersion;
        c.DocumentTitle = app.Configuration["SwaggerSettings:Title"] ?? "ProjectBase API";
        c.DisplayOperationId();
        c.EnableTryItOutByDefault();
        c.EnableFilter();
        c.DefaultModelsExpandDepth(-1); // Models section'ni yashirish
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Endpointlarni yopiq holatda ko'rsatish
        c.EnableDeepLinking();
        c.EnableValidator();

        c.DisplayRequestDuration();
        c.InjectJavascript("/js/swagger-auth.js");
    });

    Log.Information("Swagger is enabled at /{SwaggerRoutePrefix}/{swaggerVersion}/index.html", swaggerRoutePrefix, swaggerVersion);
}
else if (swaggerEnabled && app.Environment.IsProduction())
{
    Log.Warning("Swagger is enabled in Production environment!");
}
app.MapHealthChecks("/health");
app.UseSerilogRequestLogging();
app.UseResponseCompression();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                      ForwardedHeaders.XForwardedProto
});

app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();