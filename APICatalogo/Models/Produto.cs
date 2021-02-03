using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Models
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public string Descrição { get; set; }
        public decimal  Preco { get; set; }
        public string imagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }


        //propriedade de navegação
        //um produto está relacionado à uma categoria
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }

    }
}
