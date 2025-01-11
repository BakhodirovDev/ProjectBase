using BoomStream.UI.Extensions;
using BoomStream.UI.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyInjection(builder.Configuration,builder.Environment); 

builder.Host.UseSerilog();

var app = builder.Build();

app.UseRateLimiter();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseResponseCompression(); // Siqishni yoqish

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("SwaggerEnable"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/home/swagger.json", "BoomStream.UI API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                      ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();

app.MapControllers();


app.Run();
