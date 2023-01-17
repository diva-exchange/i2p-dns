using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace diva_dns.Data
{
    public class TransactionV34
    {
        [JsonPropertyName("seq")]
        public int Sequence { get; set; } = 1;

        [JsonPropertyName("command")]
        public string Command { get; set; } = "";

        [JsonPropertyName("ns")]
        public string Namespace { get; set; } = "";

        [JsonPropertyName("d")]
        public string Content { get; set; } = "";

        public TransactionV34() { }

        public static TransactionV34 PutDomainName(string domainName, string b32Address)
        {
            return new TransactionV34()
            {
                Sequence = 1,
                Command = "data",
                Namespace = domainName,
                Content = b32Address
            };
        }
    }
}
