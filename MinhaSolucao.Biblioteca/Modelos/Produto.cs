namespace MinhaCantina.Biblioteca.Modelos;

public class Produto
{
	public int Id { get; private set; }
	public string Nome { get; private set; }
	public decimal Preco { get; private set; }
	public string? Descricao { get; private set; }
	public Categoria Categoria { get; private set; }

	// Construtor
	private Produto() { }

	// Metodo estatico de criação chamado Criar()
	// Parametros: nomeProduto, precoProduto, categoriaProduto
	// <modificador> <static> <retorno> <NomeMetodo>(<parametros>){
	// Ações: 1º Verificar se o parametro nomeProduto é vazio ou nulo
	// 2º Verificar se o precoProduto é negativo
	//}

	public static Produto Criar(string nomeProduto, decimal precoProduto, Categoria categoriaProduto)
	{
		if (string.IsNullOrWhiteSpace(nomeProduto))
		{
			throw new Exception("O nome do produto não pode ser nulo ou vazio");
		}
		else if (decimal.IsNegative(precoProduto))
		{
			throw new Exception("O preço do produto não pode ser abaixo de R$ 0");
		}
		else
		{
			return new Produto()
			{
				Nome = nomeProduto,
				Preco = precoProduto,
				Categoria = categoriaProduto
			};
		}
	}

	// Metodo que altere o nome do produto
	public void MudarNome(string novoNome)
	{
		if (string.IsNullOrWhiteSpace(novoNome))
		{
			throw new Exception("O novo nome do produto não pode ser nulo ou vazio");
		}
		else
		{
			this.Nome = novoNome;
		}
	}

	// Metodo que altere o preço do produto
	public void MudarPreco(decimal novoPreco)
	{
		if (decimal.IsNegative(novoPreco))
		{
			throw new Exception("O novo preço não pode ser negativo");
		}
		else
		{
			this.Preco = novoPreco;
		}
	}

	// Metodo que altere a categoria do produto
	public void MudarCategoria(Categoria novaCategoria)
	{
		this.Categoria = novaCategoria;
	}
}
