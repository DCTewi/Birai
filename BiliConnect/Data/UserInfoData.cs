using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class UserInfoData
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = string.Empty;
    }
}
