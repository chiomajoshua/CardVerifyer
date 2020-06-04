using Newtonsoft.Json;

namespace CardVerifyer.Data
{
    public abstract class Resource
    {
        [JsonProperty(Order = -2)]
        public string Href { get; set; }
    }
}