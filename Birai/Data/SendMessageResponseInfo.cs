using Newtonsoft.Json;

namespace Birai.Data
{
    public class SendMessageResponseInfo
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
