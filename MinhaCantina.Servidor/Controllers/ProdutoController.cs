using Microsoft.AspNetCore.Mvc;                     // Biblioteca para criar controllers e rotas da API
using Microsoft.EntityFrameworkCore;                 // Biblioteca do Entity Framework (banco de dados)
using MinhaCantina.Servidor.Dados;                  // Onde está o contexto do banco (MinhaCantinaContexto)
using MySqlConnector;                               // Usado para identificar erros específicos do MySQL
using MinhaCantina.Biblioteca.Modelos;
using MinhaCantina.Biblioteca.DTOs;              // Onde está a classe Produto, Categoria, etc.

namespace MinhaCantina.Servidor.Controllers;         // Nome da pasta/namespace onde o controller está

[Route("[controller]")]                             // Define que a rota base será /Produto
[ApiController]                                     // Indica que este é um controlador de API
public class ProdutoController : ControllerBase      // ControllerBase = classe base para APIs
{
	private readonly MinhaCantinaContexto _contexto; // Variável privada que guarda o contexto do banco

	public ProdutoController(MinhaCantinaContexto cantinaContexto)
	{
		_contexto = cantinaContexto;                // Recebe o contexto via injeção de dependência
	}

	// ------------------------ CRIAR PRODUTO ------------------------
	[HttpPost("criar")]                             // Rota = POST /Produto/criar
	public IActionResult CriarProduto([FromBody] ProdutoRegistroDto requisicao)
	{
		var categoria = _contexto.Categorias.Find(requisicao.CategoriaId);
		// Busca no banco a categoria informada no DTO

		if (categoria is null)
		{
			return StatusCode(400, "Categoria não encontrada");
			// Se não existir, retorna erro 400
		}

		Produto produto;                            // Variável para armazenar o novo produto

		try                                         // Tenta criar e salvar o produto
		{
			produto = Produto.Criar(
				requisicao.Nome,
				requisicao.Preco,
				categoria
			);                                      // Cria o produto pelo método factory

			_contexto.Produtos.Add(produto);        // Adiciona o produto no contexto
			_contexto.SaveChanges();                // Salva no banco
		}
		catch (DbUpdateException excecao)           // Se ocorrer erro de banco...
		{
			var excecaoInterna = excecao.InnerException;

			if (excecaoInterna is MySqlException excecaoMySql)
			{
				if (excecaoMySql.Number == 1062)    // Código 1062 = registro duplicado
				{
					return StatusCode(400, "Esse produto já existe");
				}
			}

			throw excecao;                          // Se não for erro de duplicidade, relança
		}
		catch (Exception excecao)                   // Qualquer outro erro inesperado
		{
			return StatusCode(500, $"Erro inesperado: {excecao.Message}");
		}

		return StatusCode(201, produto);            // Retorna sucesso + produto criado
	}

	// ------------------------ PEGAR TODOS OS PRODUTOS ------------------------
	[HttpGet("pegar_todos")]                        // Rota = GET /Produto/pegar_todos
	public IActionResult PegarTodosProdutos()
	{
		var produtos = _contexto.Produtos           // Pega todos os produtos do banco
			.Select(produtoExemplo => new ProdutoRespostaDto()
			{
				Id = produtoExemplo.Id,                 // Preenche o DTO de resposta
				Nome = produtoExemplo.Nome,
				Preco = produtoExemplo.Preco,
				CategoriaNome = produtoExemplo.Categoria.Nome
			}).ToList();

		return StatusCode(200, produtos);           // Retorna os produtos com status 200 OK
	}

	// ------------------------ ALTERAR NOME ------------------------
	[HttpPatch("alterar_nome")]                     // Rota = PATCH /Produto/alterar_nome
	public IActionResult AlterarNomeProduto(int produtoId, string novoNome)
	{
		Produto? produto = _contexto.Produtos.Find(produtoId);
		// Busca o produto no banco

		if (produto is null)
		{
			return StatusCode(400, "Este produto não existe");
			// Produto inválido
		}

		try
		{
			produto.MudarNome(novoNome);            // Atualiza o nome dentro da entidade
			_contexto.Produtos.Update(produto);     // Marca como modificado
			_contexto.SaveChanges();                // Salva no banco
		}
		catch (DbUpdateException excecao)
		{
			if (excecao.InnerException is MySqlException excecaoSql &&
				excecaoSql.Number == 1062)          // Nome duplicado
			{
				return StatusCode(400, "O nome deste produto já existe.");
			}

			throw excecao;                          // Repassa caso não seja duplicidade
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Erro inesperado: {excecao.Message}");
		}

		return StatusCode(204);                     // 204 = sucesso sem conteúdo
	}

	// ------------------------ ALTERAR PREÇO ------------------------
	[HttpPatch("alterar_preco")]                    // Rota = PATCH /Produto/alterar_preco
	public IActionResult AlterarPrecoProduto(int produtoId, decimal novoPreco)
	{
		Produto? produto = _contexto.Produtos.Find(produtoId);
		// Busca o produto

		if (produto is null)
		{
			return StatusCode(400, "Este produto não existe");
		}

		try
		{
			produto.MudarPreco(novoPreco);          // Altera o preço
			_contexto.Produtos.Update(produto);     // Marca para atualização, subir a info?
			_contexto.SaveChanges();                // Salva no banco
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Erro inesperado: {excecao.Message}");
		}

		return StatusCode(204);                     // Sucesso sem resposta
	}

	// ------------------------ ALTERAR CATEGORIA ------------------------
	[HttpPatch("alterar_categoria")]                // Rota = PATCH /Produto/alterar_categoria
	public IActionResult AlterarProdutoCategoria(int produtoId, int novaCategoriaId)
	{
		Produto? produto = _contexto.Produtos.Find(produtoId); // Procura o produto


		if (produto is null)
		{
			return StatusCode(400, "Este produto não existe");
		}

		var novaCategoria = _contexto.Categorias.Find(novaCategoriaId);
		// Procura a nova categoria

		if (novaCategoria is null)
		{
			return StatusCode(400, "A nova categoria não existe");
		}

		try
		{
			produto.MudarCategoria(novaCategoria);   // Altera a categoria
			_contexto.Produtos.Update(produto);       // Atualiza no EF Core
			_contexto.SaveChanges();                  // Salva no banco
		}
		catch (Exception excecao)
		{
			return StatusCode(500, $"Erro inesperado: {excecao.Message}");
		}

		return StatusCode(204);                      // Sucesso sem conteúdo
	}
	// Rota: localhost : 7207/Produto/deletar_produto/{produtoId}
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



