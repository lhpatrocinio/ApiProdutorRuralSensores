using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdutorRuralSensores.Application.Repository;
using ProdutorRuralSensores.Infrastructure.DataBase.Repository;

namespace ProdutorRuralSensores.Infrastructure
{
    public static class InfraBootstrapper
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProdutorRuralSensoresRepository, ProdutorRuralSensoresRepository>();         
        }
    }
}
