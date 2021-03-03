using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    //define o padrão de retorno no swwager
    [Produces("application/json")]
    [EnableCors("PermitirApiRequest")]
    [Authorize(AuthenticationSchemes ="Bearer")]
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        // <Injeção de dependencia>
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork contexto, IConfiguration configuration, IMapper mapper)
        {
            _uof = contexto;
            _configuration = configuration;
            _mapper = mapper;
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
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetCategoriasProdutos();
                var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);
                
                return categoriaDTO;
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias com produtos do banco de dados");
            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                //AsNoTracking somente em consultas, um ganho de performance
                var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);

                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categorias);
                
                return categoriaDTO;
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        /// <summary>
        /// Obtem uma categoria pelo seu Id
        /// </summary>
        /// <param name="id">Código da Categoria</param>
        /// <returns>Objetos Categoria</returns>
        [HttpGet("{id}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);
                if (categoria == null)
                {
                    return NotFound($"A categoria com id: {id} não foi encontrada");
                }

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
             
                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter a categoria do banco de dados");
            }


        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     Post api/categorias
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "https://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDto">Objeto Categoria</param>
        /// <returns>O objeto Categoria incluída</returns>
        /// <remarks>Retorna um objeto Categoria</remarks>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                //desde q use a anotação [ApiController] e seja aspNet core 2.1 ou mais, a validação do modelo é feita automaticamente

                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                //inclui o produto no contexto e SaveChange "commita" essa adição
                var categoria = _mapper.Map<Categoria>(categoriaDto);
                _uof.CategoriaRepository.Add(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar criar uma nova categoria");
            }

        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                //Eu preciso validar se o id é o mesmo do produto informado no Body
                if (id != categoriaDto.CategoriaId)
                {
                    return BadRequest($"Não foi possível atualizar a categoria com id: {id}");
                }

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                //aqui eu altero o estado da Entidade, para alterado
                _uof.CategoriaRepository.Update(categoria);
                //em sequida eu preciso savar salvar, "commitar"
                await _uof.Commit();

                return Ok($"Categoria com id: {id} foi atualizada com sucesso");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar atualizar a categoria com id: {id}");
            }

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            try
            {
                //FIRSTORDEFAULT sempre vai no banco de dados
                var categoria = await _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);

                // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
                //var categoria = _uof.Categorias.Find(id);

                //Verifica se o categoria existe
                if (categoria == null)
                {
                    return NotFound($"A categoria com id: {id} não foi encontrada");
                }

                _uof.CategoriaRepository.Delete(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar excluir a categoria com id: {id}");
            }
        }
    }
}
