﻿using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    //define o padrão de retorno no swwager
    [Produces("application/json")]
    [Route("api/[Controller]")]
    [ApiController]
    //faz com que aplique o conjunto padrão de convensões ao controlador, e não a cada Action
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProdutosController : ControllerBase
    {
        // <Injeção de dependencia>
        //private readonly AppDbContext _context;
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork contexto, IMapper mapper)
        {
            _uof = contexto;
            _mapper = mapper;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco();
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDTO;

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            try
            {
                var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);


                var metadata = new
                {
                    produtos.TotalCount,
                    produtos.PageSize,
                    produtos.CurrentPage,
                    produtos.TotalPages,
                    produtos.HasNext,
                    produtos.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var produtoDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

                //AsNoTracking somente em consultas, um ganho de performance
                return produtoDTO;
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter os produtos do banco de dados");
            }

        }


        //Restrição de rota
        //:int = só aceita inteiros
        //:min(1) = o valor mínimo do parametro é 1
        //[HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        [HttpGet("{id}", Name = "ObterProduto")]        //BindRequired torna o parametro obrigatório, Temos que informar na Url ->  .../produtos/1?nome=Suco
        //Permite definir explicitamente os tipos de retorno adequado, auxiliando o swashbuckle e refletindo na documentação
        [ProducesResponseType(typeof(ProdutoDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoDTO>> Get( int id)
        {
            //Apenas para teste do tratamento global realizado na aula
            //throw new Exception("Exception ao retornar produto pelo id");
            try
            {
                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
                if (produto == null)
                {
                    return NotFound($"O produto com id: {id} não foi encontrado");
                }
                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                return produtoDTO;
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar obter o produto com id: {id} do banco de dados");
            }

        }


        //Permite definir explicitamente os tipos de retorno adequado, auxiliando o swashbuckle e refletindo na documentação
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
        {
            try
            {
                //desde q use a anotação [ApiController] e seja aspNet core 2.1 ou mais, a validação do modelo é feita automaticamente

                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                var produto = _mapper.Map<Produto>(produtoDto);

                //inclui o produto no contexto e SaveChange "commita" essa adição
                _uof.ProdutoRepository.Add(produto);
                await _uof.Commit();

                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

                return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produtoDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar criar um novo produto");
            }

        }


        [HttpPut("{id}")]
        //isso faz com que eu não preciso informar cada tipo de retorno do método
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
        {
            try
            {
                //Eu preciso validar se o id é o mesmo do produto informado no Body
                if (id != produtoDto.ProdutoId)
                {
                    return BadRequest($"Não foi possível atualizar o produto com id: {id}");
                }

                var produto = _mapper.Map<Produto>(produtoDto);
                //aqui eu altero o estado da Entidade, para alterado
                _uof.ProdutoRepository.Update(produto);
                //em sequida eu preciso savar salvar, "commitar"
                await _uof.Commit();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $" Erro ao tentar atualizar o produto com id: {id}");
            }

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            try
            {
                //FIRSTORDEFAULT sempre vai no banco de dados
                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

                // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
                //var produto = _uof.Produtos.Find(id);

                //Verifica se o produto existe
                if (produto == null)
                {
                    return NotFound($"O produto com id: {id} não foi encontrado");
                }

                _uof.ProdutoRepository.Delete(produto);
                await _uof.Commit();

                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

                return produtoDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar excluir o produto com id: {id}");
            }
        }
    }
}
