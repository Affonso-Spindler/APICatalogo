using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        //quando eu tenho uma definição de coleção, eu tenho q definir a inicialização dessa coleção
        //por boas práticas
        public Categoria()
        {
            Produtos = new Collection<Produto>();
        }
        [Key]
        public int CategoriaId { get; set; }
        [Required]
        [MaxLength(80)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string ImagemUrl { get; set; }

        //propriedade de navegação
        //Uma categoria possui uma coleção de Produtos
        //o EF utiliza para reconhecer que existe um relacionamento entre tabelas 
        public ICollection<Produto> Produtos { get; set; }
    }
}
