using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace diva_dns.Data
{
    public class Data
    {
        [JsonPropertyName("seq")]
        public int Sequence { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("ns")]
        public string Namespace { get; set; }

        [JsonPropertyName("d")]
        public string Content { get; set; }

        public static Data Create(string domainName, string b32)
        {
            return new Data()
            {
                Sequence = 1,
                Command = "data",
                Namespace = $"IIPDNS:{domainName}",
                Content = b32
            };
        }

        public bool IsValid()
        {
            Regex nsMatcher = new Regex(@"^([A-Za-z_-]{4,15}:){1,4}[A-Za-z0-9_-]{1,64}$");
            return Sequence >= 1 && Command == "data" && nsMatcher.IsMatch(Namespace) && Content.Length <= 8192;
        }
    }
}
