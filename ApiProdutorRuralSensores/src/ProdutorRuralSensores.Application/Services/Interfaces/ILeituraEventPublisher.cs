using ProdutorRuralSensores.Application.DTOs.Response;

namespace ProdutorRuralSensores.Application.Services.Interfaces;

/// <summary>
/// Interface para publicação de eventos de leitura de sensor
/// Implementada na camada de Infrastructure
/// </summary>
public interface ILeituraEventPublisher
{
    Task PublishLeituraRecebidaAsync(LeituraResponse leitura);
}
