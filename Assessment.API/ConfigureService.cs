using Assessment.Contract.Common;
using NLog.Extensions.Logging;

namespace Assessment.API
{
    public static class ConfigureService
    {
        public static IServiceCollection AddLog(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog("NLog.Config");
            });
        }

        public static IServiceCollection AddCorsDefaults(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                var corsConfig = new CorsConfigOptions();
                var corsSection = configuration.GetSection("CORS");
                if (corsSection.Value == null && !corsSection.GetChildren().Any())
                {
                    builder
                      .SetIsOriginAllowed(s => true)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
                    return;
                }

                builder.AllowCredentials();
                corsSection.Bind(corsConfig);

                if (corsConfig.Headers != null && corsConfig.Headers.Length > 0)
                {
                    builder.WithHeaders(corsConfig.Headers);
                }
                else
                {
                    builder.AllowAnyHeader();
                }

                if (corsConfig.ExposedHeaders?.Any() == true)
                {
                    builder.WithExposedHeaders(corsConfig.ExposedHeaders);
                }

                if (corsConfig.Methods != null && corsConfig.Methods.Length > 0)
                {
                    builder.WithMethods(corsConfig.Methods);

                }
                else
                {
                    builder.AllowAnyMethod();
                }

                if (corsConfig.Origins != null && corsConfig.Origins.Length > 0)
                {
                    builder.WithOrigins(corsConfig.Origins);

                }
                else
                {
                    builder.SetIsOriginAllowed(s => true);

                }
            }));
        }
    }
}
