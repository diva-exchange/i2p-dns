using System.Text.Json.Serialization;

namespace DivaDnsWebApi.Dto
{
    public class TransactionDto
    {
        public TransactionDto(string domainName, string b32String, int number)
        {
            this.Sequence = 1;
            this.Command = "decision";
            this.Ns = "I2PDNS:[domain-name]";
            this.Data = $"{domainName}={b32String}";
            this.Number = number;
        }

        [JsonPropertyName("seq")]
        public int Sequence { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("ns")]
        public string Ns { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("d")]
        public string Data { get; set; }
    }
}