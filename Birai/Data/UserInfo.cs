using Newtonsoft.Json;

namespace Birai.Data
{
    public class UserInfo
    {
        [JsonProperty("data")]
        public UserInfoData Data { get; set; }
    }

    public class UserInfoData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
