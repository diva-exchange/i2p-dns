using DivaDnsWebApi.Contracts;

namespace DivaDnsWebApi.Services
{

    public class DivaService : IDivaService
    {
        private readonly HttpClient _httpClient;

        public DivaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAsync(string domainName)
        {
            return await _httpClient.GetAsync($"state/decision:I2PDNS:{domainName}");
        }
    }
}
