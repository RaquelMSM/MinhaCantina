namespace MinhaCantina.Biblioteca.Modelos;

public class Categoria
{
	// Sintaxe de Propriedade:
	// <modificador_acesso> <tipo_dado> <NomePropriedade>
	// Modificadores de acesso: public, internal e private
	// Tipos de dados: string, int, float/decimal/ bool
	public int Id { get; private set; }
	public string Nome { get; private set; }

	// Sintaxe de Construtor:
	// <modificador_acesso> <NomeClasse>
	
	// Serve para o ORM
	private Categoria() { }

	// Criaremos um metodo de fabrica chamado Criar();
	public static Categoria Criar(string nomeCategoria)
	{
		// Crie uma verificação para ver se o nomeCategoria é nulo ou tem espaços em branco, se tiver, jogue (throw) um novo (new) objeto do tipo Exception com a mensagem: "Nome da categoria não pode ser nulo ou vazio"
		if (string.IsNullOrWhiteSpace(nomeCategoria))
		{
			throw new Exception("Nome da categoria não pode ser nulo ou vazio");
		}
		else
		{
			return new Categoria()
			{
				Nome = nomeCategoria
			};
		}
	}

	// Criaremos um metodo para modificar o nome da categoria
	// <modificador> <retorno> <NomeMetodo>(parametros){
	//	Ações:
	//  1º Se o novoNome (parametro) é nulo ou vazio, se for, joga um novo objeto Exception com os dizeres: "O novo nome da categoria não pode ser nulo ou vazio"
	//  2º Acessar a propriedade Nome e passar para ela o novoNome
	// }
	public void MudarNome(string novoNome)
	{
		if (string.IsNullOrWhiteSpace(novoNome))
		{
			throw new Exception("O novo nome da categoria não pode ser nulo ou vazio");
		}
		else
		{
			// Acessa a propriedade Nome e aplica nela a informação de novoNome
			this.Nome = novoNome;
		}
	}
}
