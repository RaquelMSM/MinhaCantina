using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaCantina.Servidor.Dados;
using MySqlConnector;
using MinhaCantina.Biblioteca.Modelos;
using MinhaCantina.Biblioteca.DTOs;

namespace MinhaCantina.Servidor.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriaController(MinhaCantinaContexto minhaCantinaContexto) : ControllerBase
{
	private MinhaCantinaContexto _contexto = minhaCantinaContexto; // Criar uma propriedade do tipo MinhaCantinaContexto


	// O nome do metodo é CriarCategoria e ele recebe um parametro com um atributo
	// Parametro: [FromBody] Categoria requisicao
	// Dentro do metodo, retorne um StatusCode 200

	// [Atributo] 

	[HttpPost("criar")] //Criar uma rota (criar) do verbo HTTP POST ("CRIAR") pois iremos criar algo novo
	public IActionResult CriarCategoria([FromBody] CategoriaRegistroDto requisicao) //Abaixo do atributo, criar um metódo publico  que retorne um IActionResult
	{
		var categoria = Categoria.Criar(requisicao.Nome);

		try
		{
			_contexto.Categorias.Add(categoria);
			_contexto.SaveChanges();
		}
		catch (DbUpdateException excecao)
		{
			var excecaoInterna = excecao.InnerException;

			if (excecaoInterna is MySqlException excecaoMySql)
			{
				if (excecaoMySql.Number == 1062)
				{
					return StatusCode(400, "Essa categoria já existe");
				}
			}
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Ocorreu um erro inesperado:{excecao.Message}");
		}
		// Salvar dentro do banco de dados

		return StatusCode(201, categoria);
	}


	[HttpGet("pegar/{id}")] // -> Verbo HTTP

	public IActionResult PegarCategoria(int id)
	{
		var categoria = _contexto.Categorias.Find(id);

		if (categoria is null)
		{
			return StatusCode(404, "Categoria não emcontrada");
		}
		return StatusCode(200, categoria);
	}

	[HttpGet("pegar_todos")]
	public IActionResult PegarTodasCategorias()
	{
		var categorias = _contexto.Categorias.ToList();
		return StatusCode(200, categorias);
	}
}


