using DivaDnsWebApi.Contracts;
using DivaDnsWebApi.Dto;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

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

        public async Task<HttpResponseMessage> PostAsync(string domainName, string b32String, int number)
        {
            var transaction = new TransactionDto(domainName, b32String, number);

            var jsonData = JsonConvert.SerializeObject(transaction);
            var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync("transaction/", contentData);
        }
    }
}
