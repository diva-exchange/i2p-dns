using Newtonsoft.Json;

namespace DivaDnsWebApi.Dtos
{
    public class GetResultDto
    {
        public GetResultDto(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}