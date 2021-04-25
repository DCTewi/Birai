using Newtonsoft.Json;

namespace Birai.Data
{
    public class UnreadInfo
    {
        [JsonProperty("data")]
        public UnreadData Data { get; set; }
    }

    public class UnreadData
    {
        [JsonProperty("unfollow_unread")]
        public int UnfollowUnread { get; set; }

        [JsonProperty("follow_unread")]
        public int FollowUnread { get; set; }
    }
}
