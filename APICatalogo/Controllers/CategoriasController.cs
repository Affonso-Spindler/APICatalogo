using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;

        public CategoriasController(IUnitOfWork contexto, IConfiguration configuration)
        {
            _uof = contexto;
            _configuration = configuration;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            var autor = _configuration["autor"];
            var conexao = _configuration["ConnectionStrings:DefaultConnection"];
            return $"Autor: {autor}, Conexão: {conexao}";
        }


        //eu não posso ter 2 ou + métodos com a mesma anotation exemplo 2 HttpGet
        //Para isso definimos um nome de rota que será composta com a rota padrão, nesse caso "api/[Controller]"
        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            try
            {
                return _uof.CategoriaRepository.GetCategoriasProdutos().ToList();
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias com produtos do banco de dados");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                //AsNoTracking somente em consultas, um ganho de performance
                return _uof.CategoriaRepository.Get().ToList();
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
                var categoria = _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);
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
                _uof.CategoriaRepository.Add(categoria);
                _uof.Commit();

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
                _uof.CategoriaRepository.Update(categoria);
                //em sequida eu preciso savar salvar, "commitar"
                _uof.Commit();

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
                var categoria = _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

                // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
                //var categoria = _uof.Categorias.Find(id);

                //Verifica se o categoria existe
                if (categoria == null)
                {
                    return NotFound($"A categoria com id: {id} não foi encontrada");
                }

                _uof.CategoriaRepository.Delete(categoria);
                _uof.Commit();
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
