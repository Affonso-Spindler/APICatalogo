using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
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

        //eu não posso ter 2 ou + métodos com a mesma anotation exemplo 2 HttpGet
        //Para isso definimos um nome de rota que será composta com a rota padrão, nesse caso "api/[Controller]"
        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            try
            {
                return _context.Categorias.Include(x => x.Produtos).ToList();
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias com produtos do banco de dados");
            }
        }

        [HttpGet] //opcional - Boa prática
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                //AsNoTracking somente em consultas, um ganho de performance
                return _context.Categorias.AsNoTracking().ToList();
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);
                if (categoria == null)
                {
                    return NotFound($"A categoria com id: {id} não foi encontrada");
                }
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter a categoria do banco de dados");
            }


        }

        [HttpPost]
        public ActionResult Post([FromBody] Categoria categoria)
        {
            try
            {
                //desde q use a anotação [ApiController] e seja aspNet core 2.1 ou mais, a validação do modelo é feita automaticamente

                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                //inclui o produto no contexto e SaveChange "commita" essa adição
                _context.Categorias.Add(categoria);
                _context.SaveChanges();

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar criar uma nova categoria");
            }

        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                //Eu preciso validar se o id é o mesmo do produto informado no Body
                if (id != categoria.CategoriaId)
                {
                    return BadRequest($"Não foi possível atualizar a categoria com id: {id}");
                }

                //aqui eu altero o estado da Entidade, para alterado
                _context.Entry(categoria).State = EntityState.Modified;
                //em sequida eu preciso savar salvar, "commitar"
                _context.SaveChanges();

                return Ok($"Categoria com id: {id} foi atualizada com sucesso");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar atualizar a categoria com id: {id}");
            }

        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            try
            {
                //FIRSTORDEFAULT sempre vai no banco de dados
                var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

                // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
                //var categoria = _context.Categorias.Find(id);

                //Verifica se o categoria existe
                if (categoria == null)
                {
                    return NotFound($"A categoria com id: {id} não foi encontrada");
                }

                _context.Categorias.Remove(categoria);
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar excluir a categoria com id: {id}");
            }
        }
    }
}
