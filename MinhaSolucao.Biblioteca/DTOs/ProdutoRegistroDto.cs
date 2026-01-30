using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaCantina.Biblioteca.DTOs
{

    
        public class ProdutoRegistroDto
        {
            public string Nome { get; set; }
            public decimal Preco { get; set; }
            public string? Descricao { get; set; }
            public int CategoriaId { get; set; }
        }
   
}
