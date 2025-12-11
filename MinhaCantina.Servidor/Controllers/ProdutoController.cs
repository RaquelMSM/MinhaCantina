using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaCantina.Biblioteca.Modelos;
using MinhaCantina.Servidor.Dados;
using MySqlConnector;

namespace MinhaCantina.Servidor.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutoController(MinhaCantinaContexto cantinaContexto) : ControllerBase
{
	private MinhaCantinaContexto _contexto = cantinaContexto;

	[HttpPost("criar")]
	public IActionResult CriarProduto([FromBody] ProdutoRegistroDto requisicao)
	{
		var categoria = _contexto.Categorias.Find(requisicao.CategoriaId);
		if (categoria is null)
		{
			return StatusCode(400, "Categoria não encontrada");
		}

		Produto produto;

		try
		{
			produto = Produto.Criar(requisicao.Nome, requisicao.Preco, categoria);
			_contexto.Produtos.Add(produto);
			_contexto.SaveChanges();
		}
		catch (DbUpdateException excecao)
		{
			var excecaoInterna = excecao.InnerException;

			if (excecaoInterna is MySqlException excecaoMySql)
			{
				if (excecaoMySql.Number == 1062)
				{
					return StatusCode(400, "Esse produto já existe");
				}
			}

			throw excecao;
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Ocorreu um erro inesperado: {excecao.Message}");
		}

		return StatusCode(201, produto);
	}

	[HttpGet("pegar_todos")]
	public IActionResult PegarTodosProdutos()
	{
		var produtos = _contexto.Produtos.Select(produtoExemplo => new ProdutoRespostaDto()
		{
			Id = produtoExemplo.Id,
			Nome = produtoExemplo.Nome,
			Preco = produtoExemplo.Preco,
			CategoriaNome = produtoExemplo.Categoria.Nome
		}).ToList();

		return StatusCode(200, produtos);
	}

	// Rota: localhost:7207/Produto/alterar_nome
	[HttpPatch("alterar_nome")]
	public IActionResult AlterarNomeProduto(int produtoId, string novoNome)
	{
		Produto? produto = _contexto.Produtos.Find(produtoId);

		if (produto is null)
		{
			return StatusCode(400, "Este produto não existe");
		}

		try
		{
			produto.MudarNome(novoNome);
			_contexto.Produtos.Update(produto);
			_contexto.SaveChanges();
		}
		catch (DbUpdateException excecao)
		{
			var excecaoInterna = excecao.InnerException;

			if (excecaoInterna is MySqlException excecaoSql)
			{
				if (excecaoSql.Number == 1062)
				{
					return StatusCode(400, "O nome deste produto já existe. Tente com um novo nome");
				}
			}

			throw excecao;
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Ocorreu um erro inesperado: {excecao.Message}");
		}

		return StatusCode(204);
	}

	// Rota: localhost:7207/Produto/alterar_preco
	[HttpPatch("alterar_preco")]
	public IActionResult AlterarPrecoProduto(int produtoId, decimal novoPreco)
	{
		// Buscar o produto
		Produto? produto = _contexto.Produtos.Find(produtoId);

		if (produto is null) return StatusCode(400, "O produto procurado não existe");

		try
		{
			produto.MudarPreco(novoPreco);
			_contexto.Produtos.Update(produto);
			_contexto.SaveChanges();
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Ocorreu um erro inesperado: {excecao.Message}");
		}

		return StatusCode(204);
	}

	// Rota: localhost:7207/Produto/alterar_categoria
	[HttpPatch("alterar_categoria")]
	public IActionResult AlterarProdutoCategoria(int produtoId, int novaCategoriaId)
	{
		Produto? produto = _contexto.Produtos.Find(produtoId); // Procurar o produto
		if (produto is null) return StatusCode(400, "Este produto não existe"); // Retorna para o cliente erro bad request se produto não existe

		Categoria? categoria = _contexto.Categorias.Find(novaCategoriaId); // Procurar a categoria nova
		if (categoria is null) return StatusCode(400, "Esta categoria não existe");// Retorna para o cliente erro bad request se categoria não existe

		produto.MudarCategoria(categoria);

		try
		{
			_contexto.Produtos.Update(produto);
			_contexto.SaveChanges();
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Ocorreu um erro inesperado: {excecao.Message}");
		}

		return StatusCode(204);
	}

	// Rota: localhost:7207/Produto/deletar_produto/{produtoId}
	[HttpDelete("deletar_produto/{produtoId}")]
	public IActionResult DeletarProduto(int produtoId)
	{
		Produto? produtoObjeto = _contexto.Produtos.Find(produtoId);
		if (produtoObjeto is null) return StatusCode(400, "Este produto não existe");

		_contexto.Produtos.Remove(produtoObjeto);
		_contexto.SaveChanges();

		return StatusCode(204);
	}
}

public class ProdutoRegistroDto
{
	public string Nome { get; set; }
	public decimal Preco { get; set; }
	public string? Descricao { get; set; }
	public int CategoriaId { get; set; }
}
public class ProdutoRespostaDto
{
	public int Id { get; set; }
	public string Nome { get; set; }
	public decimal Preco { get; set; }
	public string CategoriaNome { get; set; }
}
