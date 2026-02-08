using Microsoft.Extensions.Logging;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services.Interfaces;
using ProdutorRuralSensores.Infrastructure.Messaging.Events;

namespace ProdutorRuralSensores.Infrastructure.Messaging;

/// <summary>
/// Adaptador que implementa ILeituraEventPublisher e usa o SensorDataPublisher
/// </summary>
public class LeituraEventPublisherAdapter : ILeituraEventPublisher
{
    private readonly ISensorDataPublisher _sensorDataPublisher;
    private readonly ILogger<LeituraEventPublisherAdapter> _logger;

    public LeituraEventPublisherAdapter(
        ISensorDataPublisher sensorDataPublisher,
        ILogger<LeituraEventPublisherAdapter> logger)
    {
        _sensorDataPublisher = sensorDataPublisher;
        _logger = logger;
    }

    public async Task PublishLeituraRecebidaAsync(LeituraResponse leitura)
    {
        try
        {
            var @event = new SensorDataReceivedEvent
            {
                LeituraId = leitura.Id,
                TalhaoId = leitura.TalhaoId,
                SensorId = leitura.SensorId,
                CodigoSensor = leitura.CodigoSensor,
                UmidadeSolo = leitura.UmidadeSolo,
                Temperatura = leitura.Temperatura,
                Precipitacao = leitura.Precipitacao,
                UmidadeAr = leitura.UmidadeAr,
                VelocidadeVento = leitura.VelocidadeVento,
                RadiacaoSolar = leitura.RadiacaoSolar,
                PressaoAtmosferica = leitura.PressaoAtmosferica,
                DataHoraLeitura = leitura.DataHoraLeitura
            };

            await _sensorDataPublisher.PublishAsync(@event);
            
            _logger.LogDebug("Evento de leitura publicado: {LeituraId}", leitura.Id);
        }
        catch (Exception ex)
        {
            // Log do erro mas não propaga - o registro da leitura já foi feito
            _logger.LogWarning(ex, "Falha ao publicar evento de leitura {LeituraId}. O registro não será afetado.", leitura.Id);
        }
    }
}
