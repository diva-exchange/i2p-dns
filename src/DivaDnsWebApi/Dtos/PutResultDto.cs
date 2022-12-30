using Newtonsoft.Json;

namespace DivaDnsWebApi.Dtos
{
    public class PutResultDto
    {
        public PutResultDto(string identity)
        {
            Identity = identity;
        }

        [JsonProperty(PropertyName = "ident")]
        public string Identity { get; set; }
    }
}