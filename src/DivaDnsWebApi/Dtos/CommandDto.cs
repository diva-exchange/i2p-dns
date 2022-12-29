using Newtonsoft.Json;

namespace DivaDnsWebApi.Dto
{
    public class CommandDto
    {
        public CommandDto(string domainName, string b32String)
        {
            Sequence = 1;
            Command = "data";
            Ns = $"I2PDNS:{domainName}";
            Data = b32String;
        }

        [JsonProperty(PropertyName = "seq")]
        public int Sequence { get; set; }

        [JsonProperty(PropertyName = "command")]
        public string Command { get; set; }

        [JsonProperty(PropertyName = "ns")]
        public string Ns { get; set; }

        [JsonProperty(PropertyName = "d")]
        public string Data { get; set; }
    }
}