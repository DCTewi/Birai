using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class GenerateJsonResult
    {
        [JsonProperty("code")]
        public int StatusCode { get; set; } = -1;

        [JsonProperty("message")]
        public string Message { get; set; } = "Error: empty response.";
    }

    public class GenerateJsonResult<T> where T : class
    {
        [JsonProperty("code")]
        public int StatusCode { get; set; } = -1;

        [JsonProperty("message")]
        public string Message { get; set; } = "Error: empty response.";

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; set; } = null;
    }
}
