using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            try
            {
                //AsNoTracking somente em consultas, um ganho de performance
                return await _context.Produtos.AsNoTracking().ToListAsync();
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
        public async Task<ActionResult<Produto>> Get(int id,[BindRequired] string nome)
        {
            try
            {
                var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);
                if (produto == null)
                {
                    return NotFound($"O produto com id: {id} não foi encontrado");
                }
                return produto;
            }
            catch (Exception)
            {
                /* consulte https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.statuscodes?view=aspnetcore-5.0 */
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar obter o produto com id: {id} do banco de dados");
            }

        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            try
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar criar um novo produto");
            }

        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            try
            {
                //Eu preciso validar se o id é o mesmo do produto informado no Body
                if (id != produto.ProdutoId)
                {
                    return BadRequest($"Não foi possível atualizar o produto com id: {id}");
                }

                //aqui eu altero o estado da Entidade, para alterado
                _context.Entry(produto).State = EntityState.Modified;
                //em sequida eu preciso savar salvar, "commitar"
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $" Erro ao tentar atualizar o produto com id: {id}");
            }

        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            try
            {
                //FIRSTORDEFAULT sempre vai no banco de dados
                var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

                // o find primeiro busca na memória, se ele acha não vai no banco de dados, mas só serve se o ID for chave primária da tabela
                //var produto = _context.Produtos.Find(id);

                //Verifica se o produto existe
                if (produto == null)
                {
                    return NotFound($"O produto com id: {id} não foi encontrado");
                }

                _context.Produtos.Remove(produto);
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar excluir o produto com id: {id}");
            }
        }
    }
}
