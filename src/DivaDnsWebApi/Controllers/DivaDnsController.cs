using DivaDnsWebApi.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            HttpResponseMessage result;

            try
            {
                result = await _divaService.GetAsync(domainName);
            }
            catch (Exception)
            {
                return NotFound();
            }
                     
            if (!result.IsSuccessStatusCode)
            {
                return NotFound();
            }

            return Ok(result.Content.ReadAsStringAsync());
        }

        [HttpPut("{domainName:regex(^[[a-z0-9-_]]{{3,64}}\\.i2p$)}/{b32String:regex(^[[a-z0-9]]{{52}})}", Name = $"{nameof(PutDomainName)}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(502)]
        public async Task<ActionResult> PutDomainName([FromRoute] string domainName, [FromRoute] string b32String)
        {
            HttpResponseMessage result;

            try
            {
                result = await _divaService.PostAsync(domainName, b32String, 0);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status502BadGateway);
            }
                     
            if (!result.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            return Ok(result.Content.ReadAsStringAsync());
        }
    }
}
