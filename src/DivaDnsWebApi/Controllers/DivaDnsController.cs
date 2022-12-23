using DivaDnsWebApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DivaDnsWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DivaDnsController : ControllerBase
    {
        private readonly IDivaService _divaService;
        private readonly ILogger<DivaDnsController> _logger;

        public DivaDnsController(
            IDivaService divaService, 
            ILogger<DivaDnsController> logger)
        {
            _divaService = divaService;
            _logger = logger;
        }

        [HttpGet("{domainName:regex(^[[a-z0-9-_]]{{3,64}}\\.i2p$)}", Name = $"{nameof(GetDomainName)}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> GetDomainName([FromRoute] string domainName)
        {
            await _divaService.GetAsync(domainName);

            return Ok(domainName);
        }

        [HttpPut("{domainName:regex(^[[a-z0-9-_]]{{3,64}}\\.i2p$)}/{b32String:regex(^[[a-z0-9]]{{52}})}", Name = $"{nameof(PutDomainName)}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(502)]
        public ActionResult PutDomainName([FromRoute] string domainName, [FromRoute] string b32String)
        {
            return Ok();
        }
    }
}
