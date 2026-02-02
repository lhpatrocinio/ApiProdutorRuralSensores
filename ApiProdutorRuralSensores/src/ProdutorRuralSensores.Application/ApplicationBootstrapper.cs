using Microsoft.Extensions.DependencyInjection;
using ProdutorRuralSensores.Application.Services;
using ProdutorRuralSensores.Application.Services.Interfaces;

namespace ProdutorRuralSensores.Application
{
    public static class ApplicationBootstrapper
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IProdutorRuralSensoresService, ProdutorRuralSensoresService>();
        }
    }
}
