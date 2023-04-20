using Newtonsoft.Json;

namespace BiliConnect.Data
{
    public class MessageSessionData
    {
        [JsonProperty("session_list", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageSession> SessionList { get; set; } = new();

        [JsonProperty("has_more", NullValueHandling = NullValueHandling.Ignore)]
        public int HasMore { get; set; } = 0;
    }

    public class MessageSession
    {
        [JsonProperty("talker_id")]
        public string TalkerId { get; set; } = string.Empty;

        [JsonProperty("session_type")]
        public int SessionType { get; set; }

        [JsonProperty("session_ts")]
        public string SessionTimestamp { get; set; } = string.Empty;

        [JsonProperty("max_seqno")]
        public string MaxSeqNo { get; set; } = string.Empty;

        [JsonProperty("unread_count")]
        public int UnreadCount { get; set; }

        [JsonProperty("last_msg")]
        public MessageBody Body { get; set; } = new();
    }

    public class MessageBody
    {
        [JsonProperty("sender_uid")]
        public string SenderUid { get; set; } = string.Empty;

        [JsonProperty("content")]
        public string WrappedContent { get; set; } = string.Empty;

        [JsonProperty("msg_type")]
        public int MessageType { get; set; }

        [JsonProperty("msg_seqno")]
        public string SequenceNo { get; set; } = string.Empty;
    }

    public class ContentWrapper
    {
        [JsonProperty("content")]
        public string MessageContent { get; set; } = string.Empty;
    }
}
