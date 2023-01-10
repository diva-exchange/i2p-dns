using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace diva_dns.Data
{
    public class Transaction
    {
        [JsonPropertyName("seq")]
        public int Sequence { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("ns")]
        public string Namespace { get; set; }

        [JsonPropertyName("h")]
        public int BlockChainHeight { get; set; }

        [JsonPropertyName("d")]
        public string Data { get; set; }

        public static Transaction Create(string domainName, int blockChainHeight, string b32)
        {
            return new Transaction()
            {
                Sequence = 1,
                Command = "decision",
                Namespace = $"I2PNS:{domainName}",
                BlockChainHeight = blockChainHeight + 25,
                Data = $"domain-name={b32}"
            };
        }
    }
}
