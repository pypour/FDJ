using Assessment.Contract.Interfaces.Common;
using Assessment.Contract.Models.ExchangeRate;
using Assessment.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.API
{
    public static class ConfigureService
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            var config = new ExchangeRateConfig();
            configuration.GetSection("ExchangeRateConfig").Bind(config);
            services.AddSingleton(config);
            services.AddSingleton<ICacheService, CacheService>();
            services.AddDistributedMemoryCache();

            var service = typeof(IService);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                //TODO: We can remove next line to search all files. This line added for better performance on load application
                .Where(x => (x.FullName ?? string.Empty).StartsWith("Assessment"));
            var types = assemblies.SelectMany(x => x.GetTypes());
            var allServices = types
                .Where(x => x.IsClass && !x.IsAbstract && service.IsAssignableFrom(x));

            foreach (var type in allServices)
            {
                var allInterfaces = type.GetInterfaces()
                    .Where(x => service.IsAssignableFrom(x));
                foreach (var @interface in allInterfaces)
                {
                    services.AddScoped(@interface, type);
                }
            }

            return services;
        }
    }
}
