using DivaDnsWebApi.Contracts;
using DivaDnsWebApi.Dtos;
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
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(502)]
        public async Task<ActionResult<string>> GetDomainName([FromRoute] string domainName)
        {
            HttpResponseMessage result;

            try
            {
                result = await _divaService.GetAsync(domainName);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status502BadGateway);
            }

            if (result.IsSuccessStatusCode)
            {
                return Ok(result.Content.ReadAsStringAsync());
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            
            return StatusCode(StatusCodes.Status502BadGateway);
        }

        [HttpPut("{domainName:regex(^[[a-z0-9-_]]{{3,64}}\\.i2p$)}/{b32String:regex(^[[a-z0-9]]{{52}})}", Name = $"{nameof(PutDomainName)}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(PutResultDto), 200)]
        [ProducesResponseType(502)]
        public async Task<ActionResult<PutResultDto>> PutDomainName([FromRoute] string domainName, [FromRoute] string b32String)
        {
            PutResultDto result;

            try
            {
                result = await _divaService.PutAsync(domainName, b32String);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status502BadGateway);
            }

            return Ok(result);
        }
    }
}
