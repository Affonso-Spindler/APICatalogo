using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Models
{
    public class Categoria
    {
        //quando eu tenho uma definição de coleção, eu tenho q definir a inicialização dessa coleção
        //por boas práticas
        public Categoria()
        {
            Produtos = new Collection<Produto>();
        }

        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string ImagemUrl { get; set; }

        //propriedade de navegação
        //Uma categoria possui uma coleção de Produtos
        public ICollection<Produto> Produtos { get; set; }
    }
}
