using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Carrefour.API.BusinessIntelligence.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyConsolidatedController : ControllerBase
    {
        private readonly IDailyConsolidatedService _DailyConsolidatedService;

        public DailyConsolidatedController(IDailyConsolidatedService DailyConsolidatedService)
        {
            _DailyConsolidatedService = DailyConsolidatedService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DailyConsolidatedDTO>>> GetAll(
            CancellationToken ct
        )
        {
            return Ok(await _DailyConsolidatedService.ReadAllAsync(ct));
        }
    }
}
