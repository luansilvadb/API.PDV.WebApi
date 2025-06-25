/// <summary>
/// Controller para operações de estoque.
/// </summary>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.PDV.Application;
using API.PDV.Domain;
using API.PDV.WebApi.DTOs;

namespace API.PDV.WebApi;

/// <summary>
/// Gerencia endpoints de CRUD e movimentação de estoque.
/// </summary>
[AutoValidateAntiforgeryToken]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstoqueController : ControllerBase
{
    private readonly EstoqueService _service;

    /// <summary>
    /// Construtor do controller de estoque.
    /// </summary>
    public EstoqueController(EstoqueService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todos os registros de estoque.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var estoques = await _service.ListarAsync();
        return Ok(estoques);
    }

    /// <summary>
    /// Obtém um registro de estoque pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var estoque = await _service.ObterPorIdAsync(id);
        if (estoque == null) return NotFound();
        return Ok(estoque);
    }

    /// <summary>
    /// Cria um novo registro de estoque.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Create([FromBody] EstoqueDto dto)
    {
        var estoque = new Estoque
        {
            Id = Guid.NewGuid(),
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade,
            Lote = dto.Lote
        };

        var created = await _service.CriarAsync(estoque);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um registro de estoque existente.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EstoqueDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var estoque = new Estoque
        {
            Id = dto.Id,
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade,
            Lote = dto.Lote
        };

        await _service.AtualizarAsync(estoque);
        return NoContent();
    }

    /// <summary>
    /// Remove um registro de estoque pelo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.RemoverAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Realiza entrada de estoque para um produto.
    /// </summary>
    [HttpPost("entrada")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Entrada([FromQuery] Guid produtoId, [FromQuery] decimal quantidade, [FromQuery] string? lote = null)
    {
        await _service.EntradaAsync(produtoId, quantidade, lote);
        return Ok();
    }

    /// <summary>
    /// Realiza saída de estoque para um produto.
    /// </summary>
    [HttpPost("saida")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Saida([FromQuery] Guid produtoId, [FromQuery] decimal quantidade, [FromQuery] string? lote = null)
    {
        await _service.SaidaAsync(produtoId, quantidade, lote);
        return Ok();
    }
}
