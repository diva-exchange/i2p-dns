using DivaDnsWebApi.Contracts;
using DivaDnsWebApi.Dtos;
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

        public async Task<PutResultDto> PutAsync(string domainName, string b32String)
        {
            var command = new CommandDto(domainName, b32String);
            var transaction = new List<CommandDto>(){command};

            var jsonData = JsonConvert.SerializeObject(transaction);
            var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var result = await _httpClient.PutAsync("transaction/", contentData);

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            var jsonResult = await result.Content.ReadAsStringAsync();

            var putResultDto = JsonConvert.DeserializeObject<PutResultDto>(jsonResult);

            if (putResultDto == null)
            {
                throw new Exception();
            }

            return putResultDto;
        }
    }
}
