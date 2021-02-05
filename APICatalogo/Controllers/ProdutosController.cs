using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        // <Injeção de dependencia>
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext contexto)
        {
            _context = contexto;
        }





        [HttpGet] //opcinal - Boa prática
        public ActionResult<IEnumerable<Produto>> Get()
        {
            //AsNoTracking somente em consultas, um ganho de performance
            return _context.Produtos.AsNoTracking().ToList();
        }

        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }
            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            //desde q use a anotação [ApiController] e seja aspNet core 2.1 ou mais, a validação do modelo é feita automaticamente

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //inclui o produto no contexto e SaveChange "commita" essa adição
            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            //Eu preciso validar se o id é o mesmo do produto informado no Body
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            //aqui eu altero o estado da Entidade, para alterado
            _context.Entry(produto).State = EntityState.Modified;
            //em sequida eu preciso savar salvar, "commitar"
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            //FIRSTORDEFAULT sempre vai no banco de dados
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
            //var produto = _context.Produtos.Find(id);
            
            //Verifica se o produto existe
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            return produto;
        }

    }
}
