using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birai.Data
{
    public class VideoInfo
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public VideoInfoData Data { get; set; }
    }

    public class VideoInfoData
    {
        [JsonProperty("tname")]
        public string TypeName { get; set; }

        [JsonProperty("copyright")]
        public int CopyRight { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public int DurationSeconds { get; set; }
    }
}
