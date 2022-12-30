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

        public async Task<GetResultDto> GetAsync(string domainName)
        {
            var result = await _httpClient.GetAsync($"state/IIPDNS:{convertToCompatibility(domainName)}");

            result.EnsureSuccessStatusCode();

            var jsonResult = await result.Content.ReadAsStringAsync();

            var getResultDto = JsonConvert.DeserializeObject<GetResultDto>(jsonResult);

            if (getResultDto == null)
            {
                throw new Exception();
            }

            return getResultDto;
        }

        public async Task<PutResultDto> PutAsync(string domainName, string b32String)
        {
            var command = new CommandDto(convertToCompatibility(domainName), b32String);
            var transaction = new List<CommandDto>(){command};

            var jsonData = JsonConvert.SerializeObject(transaction);
            var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var result = await _httpClient.PutAsync("transaction/", contentData);

            result.EnsureSuccessStatusCode();

            var jsonResult = await result.Content.ReadAsStringAsync();

            var putResultDto = JsonConvert.DeserializeObject<PutResultDto>(jsonResult) ?? new PutResultDto(string.Empty);

            return putResultDto;
        }

        private string convertToCompatibility(string ns)
        {
            // FIXME: Workaround to match json schema of data command in divachain 0.34
            return ns.Replace(".i2p", ":i2p_");
        }

        private string convertFromCompatibility(string ns)
        {
            // FIXME: Workaround to restore correct domain name from data command in divachain 0.34
            return ns.Replace(":i2p_", ".i2p");
        }
    }
}
