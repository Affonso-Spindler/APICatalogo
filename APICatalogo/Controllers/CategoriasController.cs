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
    public class CategoriasController : ControllerBase
    {
        // <Injeção de dependencia>
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext contexto)
        {
            _context = contexto;
        }

        [HttpGet] //opcinal - Boa prática
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            //AsNoTracking somente em consultas, um ganho de performance
            return _context.Categorias.AsNoTracking().ToList();
        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return categoria;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Categoria categoria)
        {
            //desde q use a anotação [ApiController] e seja aspNet core 2.1 ou mais, a validação do modelo é feita automaticamente

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //inclui o produto no contexto e SaveChange "commita" essa adição
            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId}, categoria);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            //Eu preciso validar se o id é o mesmo do produto informado no Body
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            //aqui eu altero o estado da Entidade, para alterado
            _context.Entry(categoria).State = EntityState.Modified;
            //em sequida eu preciso savar salvar, "commitar"
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            //FIRSTORDEFAULT sempre vai no banco de dados
            var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

            // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
            //var categoria = _context.Categorias.Find(id);

            //Verifica se o categoria existe
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoria);
            return categoria;
        }

    }
}
