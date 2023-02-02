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
        [ProducesResponseType(typeof(GetResultDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(502)]
        public async Task<ActionResult<GetResultDto>> GetDomainName([FromRoute] string domainName)
        {
            GetResultDto result;

            try
            {
                result = await _divaService.GetAsync(domainName);
            }
            catch (HttpRequestException hrEx)
            {
                if (hrEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                return StatusCode(StatusCodes.Status502BadGateway);
            }            
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status502BadGateway);
            }

            return Ok(result);
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
