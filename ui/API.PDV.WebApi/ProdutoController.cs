/// <summary>
/// Controller para operações de produtos.
/// </summary>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.PDV.Application;
using API.PDV.Domain;
using API.PDV.WebApi.DTOs;

namespace API.PDV.WebApi;

/// <summary>
/// Gerencia endpoints de CRUD de produtos.
/// </summary>
// [ApiController] já presente, removido duplicado para evitar erro de compilação[AutoValidateAntiforgeryToken]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutoController : ControllerBase
{
    private readonly ProdutoService _service;

    /// <summary>
    /// Construtor do controller de produtos.
    /// </summary>
    public ProdutoController(ProdutoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todos os produtos.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var produtos = await _service.ListarAsync();
        return Ok(produtos);
    }

    /// <summary>
    /// Obtém um produto pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var produto = await _service.ObterPorIdAsync(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Create([FromBody] ProdutoDto dto)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Preco = dto.Preco,
            Ativo = true
        };

        var created = await _service.CriarAsync(produto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProdutoDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var produto = new Produto
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Preco = dto.Preco,
            Ativo = true
        };

        await _service.AtualizarAsync(produto);
        return NoContent();
    }

    /// <summary>
    /// Remove um produto pelo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.RemoverAsync(id);
        return NoContent();
    }
}
