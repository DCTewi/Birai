using Newtonsoft.Json;
using System.Collections.Generic;

namespace Birai.Data
{
    public class MemberListInfo
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public MemberListData Data { get; set; }
    }

    public class MemberListData
    {
        [JsonProperty("info")]
        public MemberStatsInfo Info { get; set; }

        [JsonProperty("list")]
        public List<MemberInfo> CurrentPage { get; set; }
    }

    public class MemberStatsInfo
    {
        [JsonProperty("num")]
        public int Number { get; set; }

        [JsonProperty("page")]
        public int TotalPage { get; set; }

        [JsonProperty("now")]
        public int CurrentPage { get; set; }
    }

    public class MemberInfo
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("guard_level")]
        public int MemberLevel { get; set; }
    }
}
