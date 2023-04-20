using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class AuthStatusData
    {
        [JsonProperty("isLogin", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsLogin { get; set; } = false;

        [JsonProperty("mid", NullValueHandling = NullValueHandling.Ignore)]
        public string Mid { get; set; } = string.Empty;

        [JsonProperty("uname", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; } = string.Empty;
    }
}
