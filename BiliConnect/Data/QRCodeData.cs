using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class QRCodeData
    {
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("qrcode_key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; } = string.Empty;
    }
}
