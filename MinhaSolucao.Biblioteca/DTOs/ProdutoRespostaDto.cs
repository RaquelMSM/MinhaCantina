using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaCantina.Biblioteca.DTOs
{
    
    public class ProdutoRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string CategoriaNome { get; set; }
    }

}
