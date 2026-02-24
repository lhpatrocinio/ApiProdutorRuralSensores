using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services.Interfaces;

namespace ProdutorRuralSensores.Api.Controllers.V1;

/// <summary>
/// Controller para gerenciamento de leituras de sensores IoT
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/leituras")]
[Authorize]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraService _leituraService;
    private readonly ILogger<LeiturasController> _logger;

    public LeiturasController(ILeituraService leituraService, ILogger<LeiturasController> logger)
    {
        _leituraService = leituraService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém uma leitura por ID
    /// </summary>
    /// <param name="id">ID da leitura</param>
    /// <returns>Dados da leitura</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeituraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraResponse>> GetById(Guid id)
    {
        _logger.LogInformation("Buscando leitura por ID: {LeituraId}", id);
        var leitura = await _leituraService.GetByIdAsync(id);

        if (leitura == null)
            return NotFound(new { message = "Leitura não encontrada" });

        return Ok(leitura);
    }

    /// <summary>
    /// Lista leituras com filtros
    /// </summary>
    /// <param name="filtros">Filtros de busca</param>
    /// <returns>Lista de leituras</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> GetWithFiltros([FromQuery] LeituraFiltroRequest filtros)
    {
        _logger.LogInformation("Listando leituras com filtros: TalhaoId={TalhaoId}, SensorId={SensorId}",
            filtros.TalhaoId, filtros.SensorId);

        var leituras = await _leituraService.GetWithFiltrosAsync(filtros);
        return Ok(leituras);
    }

    /// <summary>
    /// Lista leituras por talhão
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <param name="limite">Limite de registros (default: 100)</param>
    /// <returns>Lista de leituras do talhão</returns>
    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> GetByTalhao(Guid talhaoId, [FromQuery] int limite = 100)
    {
        _logger.LogInformation("Listando leituras do talhão: {TalhaoId}, limite: {Limite}", talhaoId, limite);
        var leituras = await _leituraService.GetByTalhaoIdAsync(talhaoId, limite);
        return Ok(leituras);
    }

    /// <summary>
    /// Lista leituras por sensor
    /// </summary>
    /// <param name="sensorId">ID do sensor</param>
    /// <param name="limite">Limite de registros (default: 100)</param>
    /// <returns>Lista de leituras do sensor</returns>
    [HttpGet("sensor/{sensorId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> GetBySensor(Guid sensorId, [FromQuery] int limite = 100)
    {
        _logger.LogInformation("Listando leituras do sensor: {SensorId}, limite: {Limite}", sensorId, limite);
        var leituras = await _leituraService.GetBySensorIdAsync(sensorId, limite);
        return Ok(leituras);
    }

    /// <summary>
    /// Lista leituras por período
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <param name="inicio">Data inicial</param>
    /// <param name="fim">Data final</param>
    /// <returns>Lista de leituras no período</returns>
    [HttpGet("talhao/{talhaoId:guid}/periodo")]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> GetByPeriodo(
        Guid talhaoId,
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim)
    {
        if (inicio > fim)
            return BadRequest(new { message = "A data inicial deve ser menor ou igual à data final" });

        _logger.LogInformation("Listando leituras do talhão {TalhaoId} de {Inicio} a {Fim}", talhaoId, inicio, fim);
        var leituras = await _leituraService.GetByPeriodoAsync(talhaoId, inicio, fim);
        return Ok(leituras);
    }

    /// <summary>
    /// Obtém a última leitura de um talhão
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <returns>Última leitura do talhão</returns>
    [HttpGet("talhao/{talhaoId:guid}/ultima")]
    [ProducesResponseType(typeof(LeituraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraResponse>> GetUltimaLeitura(Guid talhaoId)
    {
        _logger.LogInformation("Buscando última leitura do talhão: {TalhaoId}", talhaoId);
        var leitura = await _leituraService.GetUltimaLeituraAsync(talhaoId);

        if (leitura == null)
            return NotFound(new { message = "Nenhuma leitura encontrada para este talhão" });

        return Ok(leitura);
    }

    /// <summary>
    /// Lista leituras das últimas 24 horas de um talhão
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <returns>Leituras das últimas 24 horas</returns>
    [HttpGet("talhao/{talhaoId:guid}/ultimas24h")]
    [ProducesResponseType(typeof(IEnumerable<LeituraResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> GetUltimas24Horas(Guid talhaoId)
    {
        _logger.LogInformation("Listando leituras das últimas 24h do talhão: {TalhaoId}", talhaoId);
        var leituras = await _leituraService.GetUltimas24HorasAsync(talhaoId);
        return Ok(leituras);
    }

    /// <summary>
    /// Obtém status resumido de um talhão
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <returns>Status do talhão com médias e últimas leituras</returns>
    [HttpGet("talhao/{talhaoId:guid}/status")]
    [ProducesResponseType(typeof(TalhaoStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TalhaoStatusResponse>> GetTalhaoStatus(Guid talhaoId)
    {
        _logger.LogInformation("Obtendo status do talhão: {TalhaoId}", talhaoId);
        var status = await _leituraService.GetTalhaoStatusAsync(talhaoId);
        return Ok(status);
    }

    /// <summary>
    /// Registra uma nova leitura de sensor (endpoint principal para IoT)
    /// </summary>
    /// <param name="request">Dados da leitura</param>
    /// <returns>Leitura registrada</returns>
    [HttpPost]
    [AllowAnonymous] // Sensores IoT podem não ter autenticação JWT
    [ProducesResponseType(typeof(LeituraResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeituraResponse>> Create([FromBody] LeituraCreateRequest request)
    {
        _logger.LogInformation("Recebendo leitura do talhão {TalhaoId}, sensor {SensorId}/{CodigoSensor}",
            request.TalhaoId, request.SensorId, request.CodigoSensor);

        var leitura = await _leituraService.CreateAsync(request);

        _logger.LogInformation("Leitura registrada com sucesso: {LeituraId}, Umidade={Umidade}%, Temp={Temp}°C",
            leitura.Id, leitura.UmidadeSolo, leitura.Temperatura);

        return CreatedAtAction(nameof(GetById), new { id = leitura.Id }, leitura);
    }

    /// <summary>
    /// Registra múltiplas leituras em batch (para simuladores ou gateways IoT)
    /// </summary>
    /// <param name="request">Leituras a registrar</param>
    /// <returns>Leituras registradas</returns>
    [HttpPost("batch")]
    [AllowAnonymous] // Sensores IoT podem não ter autenticação JWT
    [ProducesResponseType(typeof(IEnumerable<LeituraResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> CreateBatch([FromBody] LeituraBatchRequest request)
    {
        _logger.LogInformation("Recebendo batch de {Quantidade} leituras", request.Leituras.Count);

        var leituras = await _leituraService.CreateBatchAsync(request);

        _logger.LogInformation("Batch de {Quantidade} leituras registrado com sucesso", request.Leituras.Count);

        return Created("", leituras);
    }

    /// <summary>
    /// Obtém dados agregados por período (hora ou dia)
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <param name="inicio">Data inicial</param>
    /// <param name="fim">Data final</param>
    /// <param name="agregacao">Tipo de agregação: "hora" ou "dia"</param>
    /// <returns>Dados agregados</returns>
    [HttpGet("talhao/{talhaoId:guid}/agregado")]
    [ProducesResponseType(typeof(IEnumerable<LeituraAgregadaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<LeituraAgregadaResponse>>> GetAgregado(
        Guid talhaoId,
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim,
        [FromQuery] string agregacao = "hora")
    {
        if (inicio > fim)
            return BadRequest(new { message = "A data inicial deve ser menor ou igual à data final" });

        if (agregacao.ToLower() != "hora" && agregacao.ToLower() != "dia")
            return BadRequest(new { message = "Agregação deve ser 'hora' ou 'dia'" });

        _logger.LogInformation("Obtendo dados agregados do talhão {TalhaoId} de {Inicio} a {Fim}, agregação: {Agregacao}",
            talhaoId, inicio, fim, agregacao);

        var dados = await _leituraService.GetAgregadoAsync(talhaoId, inicio, fim, agregacao);
        return Ok(dados);
    }
}
