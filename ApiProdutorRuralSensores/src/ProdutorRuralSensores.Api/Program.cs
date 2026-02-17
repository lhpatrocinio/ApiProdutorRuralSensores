using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ProdutorRuralSensores.Api.Extensions.Auth;
using ProdutorRuralSensores.Api.Extensions.Auth.Middleware;
using ProdutorRuralSensores.Api.Extensions.Logs;
using ProdutorRuralSensores.Api.Extensions.Logs.ELK;
using ProdutorRuralSensores.Api.Extensions.Logs.Extension;
using ProdutorRuralSensores.Api.Extensions.Mappers;
using ProdutorRuralSensores.Api.Extensions.Migration;
using ProdutorRuralSensores.Api.Extensions.Swagger;
using ProdutorRuralSensores.Api.Extensions.Swagger.Middleware;
using ProdutorRuralSensores.Api.Extensions.Tracing;
using ProdutorRuralSensores.Api.Extensions.Versioning;
using ProdutorRuralSensores.Application;
using ProdutorRuralSensores.Infrastructure;
using ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Context;
using ProdutorRuralSensores.Infrastructure.Monitoring;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();
builder.WebHost.UseUrls("http://*:80");

builder.Services.AddMvcCore(options => options.AddLogRequestFilter());
builder.Services.AddVersioning();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorizationExtension(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


#region [DI]

ApplicationBootstrapper.Register(builder.Services);
InfraBootstrapper.Register(builder.Services, builder.Configuration);

// Prometheus monitoring
builder.Services.AddPrometheusMonitoring();

// ELK Stack integration  
builder.Services.AddELKIntegration(builder.Configuration);

// Distributed Tracing with OpenTelemetry + Jaeger
builder.Services.AddDistributedTracing(builder.Configuration);

#endregion

#region [Consumers]
//builder.Services.AddSingleton<RabbitMqSetup>();
//builder.Services.AddHostedService<UserCreatedConsumer>();

#endregion

var app = builder.Build();

//app.ExecuteMigrations();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseAuthentication();                        // 1�: popula HttpContext.User
app.UseMiddleware<RoleAuthorizationMiddleware>();
app.UseCorrelationId();
app.UseELKIntegration();

app.UseCors("AllowAll");

// Prometheus middleware
app.UsePrometheusMonitoring();

app.UseVersionedSwagger(apiVersionDescriptionProvider);
app.UseAuthorization();                         // 3�: aplica [Authorize]
//app.UseHttpsRedirection();
app.MapControllers();

// Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.Run();
