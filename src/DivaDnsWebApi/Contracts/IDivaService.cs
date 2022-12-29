namespace DivaDnsWebApi.Contracts
{
    public interface IDivaService
    {
        Task<HttpResponseMessage> GetAsync(string domainName);
        Task<HttpResponseMessage> PutAsync(string domainName, string b32String);
    }
}
