using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class VideoInfoData
    {
        [JsonProperty("bvid", NullValueHandling = NullValueHandling.Ignore)]
        public string Bvid { get; set; } = string.Empty;

        [JsonProperty("aid", NullValueHandling = NullValueHandling.Ignore)]
        public string Avid { get; set; } = string.Empty;

        [JsonProperty("tname", NullValueHandling = NullValueHandling.Ignore)]
        public string TypeName { get; set; } = string.Empty;

        [JsonProperty("copyright", NullValueHandling = NullValueHandling.Ignore)]
        public int CopyRight { get; set; } = 0;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public int DurationSeconds { get; set; } = 0;
    }
}
