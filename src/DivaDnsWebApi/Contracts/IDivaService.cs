namespace DivaDnsWebApi.Contracts
{
    public interface IDivaService
    {
        Task<string> GetAsync(string domainName);
    }
}
