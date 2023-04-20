using BiliConnect.Data;

using Newtonsoft.Json;

namespace BiliConnect.Services
{
    public class MessageService
    {
        private readonly IBilibiliProxy _proxy;
        private readonly AccountService _accountService;

        public MessageService(IBilibiliProxy proxy, AccountService accountService)
        {
            _accountService = accountService;
            _proxy = proxy;
        }

        public static long GetTimeStamp() => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

        public async Task<UnreadData> CheckNewMessageAsync()
        {
            var apiurl = Literals.Apis.GetMessageUnread;

            var response = await _proxy.GetAsync(apiurl);

            if (response?.IsSuccessStatusCode ?? false)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                var info = JsonConvert.DeserializeObject<GenerateJsonResult<UnreadData>>(rawjson);

                return info?.Data ?? new();
            }

            return new();
        }

        public async Task<MessageSessionData> GetAsync(bool unfollow, string endTs = "")
        {
            var apiurl = Literals.Apis.GetMessageSessions +
                $"?session_type={(unfollow ? 2 : 1)}" +
                "&unfollow_fold=1" +
                "&group_fold=1" +
                "&sort_rule=2" +
                "&mobi_app=web";
            if (!string.IsNullOrEmpty(endTs))
            {
                apiurl += $"&end_ts={endTs}";
            }

            var resp = await _proxy.GetAsync(apiurl);

            if (resp?.IsSuccessStatusCode ?? false)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<MessageSessionData>>(rawJson);

                if (jsonResult != null)
                {
                    return jsonResult?.Data ?? new();
                }
            }

            return new();
        }

        public async Task MarkMessageAsReadAsync(MessageSession msgSes)
        {
            var apiurl = Literals.Apis.MarkMessageAsRead;

            Dictionary<string, string> para = new()
            {
                { "talker_id", msgSes.TalkerId },
                { "session_type", msgSes.SessionType.ToString() },
                { "ack_seqno", msgSes.MaxSeqNo },
                { "build", "0" },
                { "mobi_app", "web" }
            };

            await _proxy.PostAsync(para, apiurl);
        }

        public async Task<GenerateJsonResult> SendMessageAsync(string uid, string msg)
        {
            var apiurl = "https://api.vc.bilibili.com/web_im/v1/web_im/send_msg";

            var accountInfo = await _accountService.GetAsync();

            Dictionary<string, string> para = new()
            {
                { "msg[sender_uid]", accountInfo.Mid },
                { "msg[receiver_id]", uid },
                { "msg[receiver_type]", "1" },
                { "msg[msg_type]", "1" },
                { "msg[msg_status]", "0" },
                { "msg[content]", $"{{\"content\":\"{msg}\"}}" },
                { "msg[timestamp]", GetTimeStamp().ToString() },
                { "msg[new_face_version]", "0" },
                { "msg[dev_id]", _proxy.DevId() },
            };


            var response = await _proxy.PostAsync(para, apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult>(rawjson);

                return jsonResult ?? new();
            }

            return new();
        }
    }
}
