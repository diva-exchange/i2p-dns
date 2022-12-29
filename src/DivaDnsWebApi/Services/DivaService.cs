using DivaDnsWebApi.Contracts;
using DivaDnsWebApi.Dto;
using Newtonsoft.Json;
using System.Text;

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
            return await _httpClient.GetAsync($"state/I2PDNS:{domainName}");
        }

        public async Task<HttpResponseMessage> PutAsync(string domainName, string b32String)
        {
            var command = new CommandDto(domainName, b32String);
            var transaction = new List<CommandDto>(){command};

            var jsonData = JsonConvert.SerializeObject(transaction);
            var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

            return await _httpClient.PutAsync("transaction/", contentData);
        }
    }
}
