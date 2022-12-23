namespace DivaDnsWebApi.Contracts
{
    public interface IDivaService
    {
        Task<HttpResponseMessage> GetAsync(string domainName);
        Task<HttpResponseMessage> PostAsync(string domainName, string b32String, int number);
    }
}
