using Microsoft.AspNetCore.Mvc;

namespace Carrefour.API.BusinessIntelligence.Controllers
{
    [ApiController]
    [Route("/")]
    public class HealthzController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            //To-do:
            //Validar se o acesso ao banco também está funcionando,
            //alterando o status code de acordo, e melhorando a observabilidade do microsserviço
            return "This service is online now!";
        }
    }
}
