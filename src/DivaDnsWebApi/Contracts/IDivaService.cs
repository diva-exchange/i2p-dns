using DivaDnsWebApi.Dtos;

namespace DivaDnsWebApi.Contracts
{
    public interface IDivaService
    {
        Task<GetResultDto> GetAsync(string domainName);
        Task<PutResultDto> PutAsync(string domainName, string b32String);
    }
}
