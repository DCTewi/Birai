using Newtonsoft.Json;
using System.Collections.Generic;

namespace Birai.Data
{
    public class MessageListInfo
    {
        [JsonProperty("data")]
        public MessageListData Data { get; set; }
    }

    public class MessageListData
    {
        [JsonProperty("session_list")]
        public List<MessageSession> SessionList { get; set; }

        [JsonProperty("has_more")]
        public int HasMore { get; set; }
    }

    public class MessageSession
    {
        [JsonProperty("talker_id")]
        public string TalkerId { get; set; }

        [JsonProperty("session_type")]
        public int SessionType { get; set; }

        [JsonProperty("max_seqno")]
        public string MaxSeqNo { get; set; }

        [JsonProperty("ack_ts")]
        public string AckTs { get; set; }

        [JsonProperty("session_ts")]
        public string SessionTs { get; set; }

        [JsonProperty("unread_count")]
        public int UnreadCount { get; set; }

        [JsonProperty("last_msg")]
        public MessageBody Body { get; set; }
    }

    public class MessageBody
    {
        [JsonProperty("sender_uid")]
        public string SenderUid { get; set; }

        [JsonProperty("receiver_type")]
        public int ReceiverType { get; set; }

        [JsonProperty("receiver_id")]
        public string ReceiverId { get; set; }

        [JsonProperty("content")]
        public string WrappedContent { get; set; }

        [JsonProperty("msg_seqno")]
        public string SequenceNo { get; set; }

        [JsonProperty("msg_type")]
        public int MessageType { get; set; }

        [JsonProperty("msg_key")]
        public string MessageKey { get; set; }
    }

    public class ContentWrapper
    {
        [JsonProperty("content")]
        public string Raw { get; set; }
    }
}
