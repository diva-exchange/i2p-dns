namespace DivaDnsWebApi.Contracts
{
    public interface IDivaService
    {
        Task<HttpResponseMessage> GetAsync(string domainName);
    }
}
