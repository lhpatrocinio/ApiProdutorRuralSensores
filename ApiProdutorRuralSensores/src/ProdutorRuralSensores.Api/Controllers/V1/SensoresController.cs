using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services.Interfaces;

namespace ProdutorRuralSensores.Api.Controllers.V1;

/// <summary>
/// Controller para gerenciamento de sensores IoT
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sensores")]
[Authorize]
public class SensoresController : ControllerBase
{
    private readonly ISensorService _sensorService;
    private readonly ILogger<SensoresController> _logger;

    public SensoresController(ISensorService sensorService, ILogger<SensoresController> logger)
    {
        _sensorService = sensorService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os sensores
    /// </summary>
    /// <returns>Lista de sensores</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorResponse>>> GetAll()
    {
        _logger.LogInformation("Listando todos os sensores");
        var sensores = await _sensorService.GetAllAsync();
        return Ok(sensores);
    }

    /// <summary>
    /// Obtém um sensor por ID
    /// </summary>
    /// <param name="id">ID do sensor</param>
    /// <returns>Dados do sensor</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorResponse>> GetById(Guid id)
    {
        _logger.LogInformation("Buscando sensor por ID: {SensorId}", id);
        var sensor = await _sensorService.GetByIdAsync(id);

        if (sensor == null)
            return NotFound(new { message = "Sensor não encontrado" });

        return Ok(sensor);
    }

    /// <summary>
    /// Obtém um sensor por código
    /// </summary>
    /// <param name="codigo">Código do sensor</param>
    /// <returns>Dados do sensor</returns>
    [HttpGet("codigo/{codigo}")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorResponse>> GetByCodigo(string codigo)
    {
        _logger.LogInformation("Buscando sensor por código: {Codigo}", codigo);
        var sensor = await _sensorService.GetByCodigoAsync(codigo);

        if (sensor == null)
            return NotFound(new { message = "Sensor não encontrado" });

        return Ok(sensor);
    }

    /// <summary>
    /// Lista sensores por talhão
    /// </summary>
    /// <param name="talhaoId">ID do talhão</param>
    /// <returns>Lista de sensores do talhão</returns>
    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorResponse>>> GetByTalhao(Guid talhaoId)
    {
        _logger.LogInformation("Listando sensores do talhão: {TalhaoId}", talhaoId);
        var sensores = await _sensorService.GetByTalhaoIdAsync(talhaoId);
        return Ok(sensores);
    }

    /// <summary>
    /// Lista apenas sensores ativos
    /// </summary>
    /// <returns>Lista de sensores ativos</returns>
    [HttpGet("ativos")]
    [ProducesResponseType(typeof(IEnumerable<SensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorResponse>>> GetAtivos()
    {
        _logger.LogInformation("Listando sensores ativos");
        var sensores = await _sensorService.GetAtivosAsync();
        return Ok(sensores);
    }

    /// <summary>
    /// Obtém sensor com últimas leituras
    /// </summary>
    /// <param name="id">ID do sensor</param>
    /// <param name="ultimasLeituras">Quantidade de leituras a retornar (default: 10)</param>
    /// <returns>Sensor com leituras</returns>
    [HttpGet("{id:guid}/leituras")]
    [ProducesResponseType(typeof(SensorComLeiturasResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorComLeiturasResponse>> GetWithLeituras(Guid id, [FromQuery] int ultimasLeituras = 10)
    {
        _logger.LogInformation("Buscando sensor {SensorId} com últimas {QtdLeituras} leituras", id, ultimasLeituras);
        var sensor = await _sensorService.GetWithLeiturasAsync(id, ultimasLeituras);

        if (sensor == null)
            return NotFound(new { message = "Sensor não encontrado" });

        return Ok(sensor);
    }

    /// <summary>
    /// Cadastra um novo sensor
    /// </summary>
    /// <param name="request">Dados do sensor</param>
    /// <returns>Sensor cadastrado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SensorResponse>> Create([FromBody] SensorCreateRequest request)
    {
        _logger.LogInformation("Cadastrando novo sensor: {Codigo} no talhão {TalhaoId}", request.Codigo, request.TalhaoId);

        // Verificar se já existe sensor com mesmo código
        var existente = await _sensorService.GetByCodigoAsync(request.Codigo);
        if (existente != null)
            return BadRequest(new { message = "Já existe um sensor com este código" });

        var sensor = await _sensorService.CreateAsync(request);
        _logger.LogInformation("Sensor cadastrado com sucesso: {SensorId}", sensor.Id);

        return CreatedAtAction(nameof(GetById), new { id = sensor.Id }, sensor);
    }

    /// <summary>
    /// Atualiza um sensor existente
    /// </summary>
    /// <param name="id">ID do sensor</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Sensor atualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SensorResponse>> Update(Guid id, [FromBody] SensorUpdateRequest request)
    {
        _logger.LogInformation("Atualizando sensor: {SensorId}", id);

        // Verificar se mudou o código e se já existe outro com o mesmo
        var sensorAtual = await _sensorService.GetByIdAsync(id);
        if (sensorAtual == null)
            return NotFound(new { message = "Sensor não encontrado" });

        if (sensorAtual.Codigo != request.Codigo)
        {
            var existente = await _sensorService.GetByCodigoAsync(request.Codigo);
            if (existente != null)
                return BadRequest(new { message = "Já existe outro sensor com este código" });
        }

        var sensor = await _sensorService.UpdateAsync(id, request);
        _logger.LogInformation("Sensor atualizado com sucesso: {SensorId}", id);

        return Ok(sensor);
    }

    /// <summary>
    /// Remove um sensor
    /// </summary>
    /// <param name="id">ID do sensor</param>
    /// <returns>Status da operação</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Removendo sensor: {SensorId}", id);

        var deleted = await _sensorService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = "Sensor não encontrado" });

        _logger.LogInformation("Sensor removido com sucesso: {SensorId}", id);
        return NoContent();
    }

    /// <summary>
    /// Ativa ou desativa um sensor
    /// </summary>
    /// <param name="id">ID do sensor</param>
    /// <param name="ativo">Status desejado</param>
    /// <returns>Status da operação</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtivarDesativar(Guid id, [FromQuery] bool ativo)
    {
        _logger.LogInformation("Alterando status do sensor {SensorId} para {Status}", id, ativo ? "ativo" : "inativo");

        var result = await _sensorService.AtivarDesativarAsync(id, ativo);
        if (!result)
            return NotFound(new { message = "Sensor não encontrado" });

        return Ok(new { message = $"Sensor {(ativo ? "ativado" : "desativado")} com sucesso" });
    }
}
