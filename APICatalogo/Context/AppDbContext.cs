using APICatalogo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Context
{
    //Classe de contexto, ela vai permitir eu coordenar a funcionalidade do Entity Framework Core para o meu modelo de entidades
    public class AppDbContext : IdentityDbContext
    {
        //essa classe de contexto vai representar uma sessão com o banco subjacentes onde vc vai poder executar as operações CRUD
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        //propriedades de mapeamento das entidades que foram definidas na Models
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
 