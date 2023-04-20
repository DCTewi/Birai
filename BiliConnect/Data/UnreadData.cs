using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class UnreadData
    {
        [JsonProperty("unfollow_unread", NullValueHandling = NullValueHandling.Ignore)]
        public int UnfollowUnread { get; set; } = 0;

        [JsonProperty("follow_unread", NullValueHandling = NullValueHandling.Ignore)]
        public int FollowUnread { get; set; } = 0;
    }
}
