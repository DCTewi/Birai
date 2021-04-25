using Newtonsoft.Json;

namespace Birai.Data
{
    public class AccountInfo
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("dev_id")]
        public string DevId { get; set; }
    }
}
