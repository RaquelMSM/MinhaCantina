using MinhaCantina.Biblioteca.Modelos;

namespace MinhaCantina.Testes.Biblioteca;

public class CategoriaTestes
{
	[Fact] // ACAO_CENARIO_RESULTADO
	public void CriarCategoria_QuandoNomeForNulo_ReceberUmaExcecao()
	{
		// Preparação
		var nomeInvalido = string.Empty;

		// Fazer ocorrer a exceção
		var excecao = Assert.Throws<Exception>(() => Categoria.Criar(nomeInvalido));

		// Verificar o resultado
		Assert.Contains("Nome da categoria não pode ser nulo ou vazio", excecao.Message);
	}

	[Fact]
	public void CriarCategoria_QuandoNomeForValido_ReceberUmObjetoCategoria()
	{
		// Preparação
		var nomeValido = "Salgados";

		// Ação
		var categoria = Categoria.Criar(nomeValido);

		// Resultado
		Assert.NotNull(categoria);
		Assert.Equal(nomeValido, categoria.Nome);
	}
	[Fact]
	public void MudarNome_QuandoNomeForVazio_ReceberUmaExcecao()
	{
		var nomeValido = "Salgados";
		var nomeInvalido = string.Empty;
		var categoria = Categoria.Criar(nomeValido);

		var excecao = Assert.Throws<Exception>(() => categoria.MudarNome(nomeInvalido));

		Assert.Contains("O novo nome da categoria não pode ser nulo ou vazio", excecao.Message);
	}

	[Fact]
	public void MudarNome_QuandoNomeForValido_AtualizarAtributoNome()
	{
		var nomeValido = "Salgados";
		var novoNomeValido = "Assados";
		var categoria = Categoria.Criar(nomeValido);

		categoria.MudarNome(novoNomeValido);

		Assert.Equal(novoNomeValido, categoria.Nome);
	}
}
