namespace ProjectBase.WebApi.Extensions;

public static class Policies
{
    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection("CorsSettings");

        services.AddCors(options =>
        {
            var policies = corsSettings.GetSection("Policies").GetChildren();

            foreach (var policySection in policies)
            {
                var policyName = policySection.Key;
                var origins = policySection.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                var methods = policySection.GetSection("AllowedMethods").Get<string[]>();
                var headers = policySection.GetSection("AllowedHeaders").Get<string[]>();
                var exposedHeaders = policySection.GetSection("ExposedHeaders").Get<string[]>();
                var allowCredentials = policySection.GetValue<bool>("AllowCredentials");
                var allowAnyOrigin = policySection.GetValue<bool>("AllowAnyOrigin");

                options.AddPolicy(policyName, policy =>
                {
                    // Origins
                    if (allowAnyOrigin)
                    {
                        policy.AllowAnyOrigin();
                    }
                    else if (origins.Length > 0)
                    {
                        policy.WithOrigins(origins);
                    }

                    // Methods
                    if (methods != null && methods.Length > 0)
                    {
                        policy.WithMethods(methods);
                    }
                    else
                    {
                        policy.AllowAnyMethod();
                    }

                    // Headers
                    if (headers != null && headers.Length > 0)
                    {
                        policy.WithHeaders(headers);
                    }
                    else
                    {
                        policy.AllowAnyHeader();
                    }

                    // Credentials (AllowAnyOrigin bilan ishlamaydi)
                    if (allowCredentials && !allowAnyOrigin)
                    {
                        policy.AllowCredentials();
                    }

                    // Exposed Headers
                    if (exposedHeaders != null && exposedHeaders.Length > 0)
                    {
                        policy.WithExposedHeaders(exposedHeaders);
                    }
                });
            }
        });

        return services;
    }
}