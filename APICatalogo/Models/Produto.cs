using APICatalogo.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }
        [Required(ErrorMessage ="O nome é obrigatório")]
        [MaxLength(80)]
        //Utilizando a nossa Validação Customizavel
        [PrimeiraLetraMaiuscula]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }
        [Required]
        public decimal Preco { get; set; }
        [Required]
        [MaxLength(300)]
        public string imagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }


        //propriedade de navegação
        //um produto está relacionado à uma categoria
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }

    }
}
