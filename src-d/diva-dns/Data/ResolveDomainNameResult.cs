using System.Text.Json.Serialization;

namespace diva_dns.Data
{
    public class ResolveDomainNameResult
    {
        [JsonPropertyName("value")]
        public string B32Address { get; set; } = "";
    }
}
