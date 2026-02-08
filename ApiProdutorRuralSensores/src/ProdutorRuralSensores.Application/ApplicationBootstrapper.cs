using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.Services;
using ProdutorRuralSensores.Application.Services.Interfaces;
using ProdutorRuralSensores.Application.Validators;

namespace ProdutorRuralSensores.Application
{
    public static class ApplicationBootstrapper
    {
        public static void Register(IServiceCollection services)
        {
            // Services
            services.AddScoped<ISensorService, SensorService>();
            services.AddScoped<ILeituraService, LeituraService>();

            // Validators
            services.AddScoped<IValidator<SensorCreateRequest>, SensorCreateRequestValidator>();
            services.AddScoped<IValidator<SensorUpdateRequest>, SensorUpdateRequestValidator>();
            services.AddScoped<IValidator<LeituraCreateRequest>, LeituraCreateRequestValidator>();
            services.AddScoped<IValidator<LeituraBatchRequest>, LeituraBatchRequestValidator>();
        }
    }
}
