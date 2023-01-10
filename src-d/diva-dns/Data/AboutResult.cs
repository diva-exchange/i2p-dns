using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace diva_dns.Data
{
    public class AboutResult
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("license")]
        public string License { get; set; }

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }
}
