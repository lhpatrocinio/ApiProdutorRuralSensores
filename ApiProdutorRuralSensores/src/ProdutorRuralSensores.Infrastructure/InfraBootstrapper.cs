using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdutorRuralSensores.Application.Services.Interfaces;
using ProdutorRuralSensores.Domain.Interfaces;
using ProdutorRuralSensores.Infrastructure.DataBase.Repository;
using ProdutorRuralSensores.Infrastructure.Messaging;

namespace ProdutorRuralSensores.Infrastructure
{
    public static class InfraBootstrapper
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Repositories
            services.AddScoped<ISensorRepository, SensorRepository>();
            services.AddScoped<ILeituraSensorRepository, LeituraSensorRepository>();

            // RabbitMQ
            services.AddSingleton<RabbitMqSetup>();
            services.AddScoped<ISensorDataPublisher, SensorDataPublisher>();
            services.AddScoped<ILeituraEventPublisher, LeituraEventPublisherAdapter>();
        }
    }
}
