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
    public class Produto : IValidatableObject
    {
        [Key]
        public int ProdutoId { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(80)]
        //Utilizando a nossa Validação Customizavel
        //[PrimeiraLetraMaiuscula]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8, 2)")]
        [Range(1, 10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //PrimeiraLetraMaiuscula
            if (!string.IsNullOrEmpty(this.Nome))
            {
                var primeiraLetra = this.Nome[0].ToString();
                if (primeiraLetra != primeiraLetra.ToUpper())
                {
                    /* Retorna uma excessão
                    o ValidationResulta inicializa uma nova instancia da Classe ValidationResult
                    , definindo a mensagem de erro e uma lista de membros que possuem erro de validação*/

                    //nameof: obtem o nome da propriedade
                    // yield: utilizamos este retorno para retornar cada elemento individualmente 
                    yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula",
                                                    new[] { nameof(this.Nome) });
                }
            }

            //Estoque = 0
            if (this.Estoque <= 0)
            {
                yield return new ValidationResult("O estoque deve ser maior que zero",
                                                    new[] { nameof(this.Estoque) });
            }
        }
    }
}
