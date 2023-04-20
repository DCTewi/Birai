using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class PollQRCodeData
    {
        [JsonProperty("code")]
        public int Code { get; set; } = (int)QRCodeStatus.Expired;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
