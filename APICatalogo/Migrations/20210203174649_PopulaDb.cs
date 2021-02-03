using Microsoft.EntityFrameworkCore.Migrations;

namespace APICatalogo.Migrations
{
    public partial class PopulaDb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Bebidas', 'http://www.macoratti.com.br/Imagens/1.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Lanches', 'http://www.macoratti.com.br/Imagens/2.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Sobremesas', 'http://www.macoratti.com.br/Imagens/3.jpg')");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaID)" +
                " Values('Coca-Cola Diet', 'Refrigerante de Cola 350ml', 5.45, 'http://www.macoratti.com.br/Imagens/coca.jpg', 50, now()," +
                " (Select CategoriaId from Categorias where nome = 'Bebidas'))");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaID)" +
                " Values('Lanche de atum', 'Lanche de atum com Maionese', 8.50, 'http://www.macoratti.com.br/Imagens/atum.jpg', 10, now()," +
                " (Select CategoriaId from Categorias where nome = 'Lanches'))");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaID)" +
                " Values('Pudim 100g', 'Pudim de Leite condensado 100g', 6.75, 'http://www.macoratti.com.br/Imagens/pudim.jpg', 20, now()," +
                " (Select CategoriaId from Categorias where nome = 'Sobremesas'))");

        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
            mb.Sql("Delete from Produtos");
        }
    }
}
