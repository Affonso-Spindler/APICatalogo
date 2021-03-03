using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    //Deprecated indica se está obsoleto
    [ApiVersion("1.0", Deprecated = true)]
    //temos de informar a versão via query string. Ex.: https://localhost:44329/api/teste?api-version=2
    [Route("api/teste")]

    //Podemos informar a versão na URL Ex.: https://localhost:44329/api/2/teste
    //[Route("api/{v:apiVersion}/teste")]
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV1Controller - V 1.0 </h2></body></html>", "text/html");
        }
    }
}
