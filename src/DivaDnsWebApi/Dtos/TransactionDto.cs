using Newtonsoft.Json;

namespace DivaDnsWebApi.Dto
{
    public class TransactionDto
    {
        public TransactionDto(string domainName, string b32String, int number)
        {
            Sequence = 1;
            Command = "decision";
            Ns = "I2PDNS:[domain-name]";
            Data = $"{domainName}={b32String}";
            Number = number;
        }

        [JsonProperty(PropertyName = "seq")]
        public int Sequence { get; set; }

        [JsonProperty(PropertyName = "command")]
        public string Command { get; set; }

        [JsonProperty(PropertyName = "ns")]
        public string Ns { get; set; }

        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }

        [JsonProperty(PropertyName = "d")]
        public string Data { get; set; }
    }
}