using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [ApiVersion("2.0")]
    //temos de informar a versão via query string. Ex.: https://localhost:44329/api/teste?api-version=2
    [Route("api/teste")]

    //Podemos informar a versão na URL
    //[Route("api/{v:apiVersion}/teste")]
    [ApiController]
    public class TesteV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV1Controller - V 2.0 </h2></body></html>", "text/html");
        }
    }
}
