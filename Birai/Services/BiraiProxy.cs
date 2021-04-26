using Birai.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Birai.Services
{
    public class BiraiProxy
    {
        private readonly ClientProxy clientProxy = new();

        public async Task<bool> CheckForNewMessageAsync()
        {
            var apiurl = "http://api.vc.bilibili.com/session_svr/v1/session_svr/single_unread";

            var response = await clientProxy.GetAsync(apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                var info = JsonConvert.DeserializeObject<UnreadInfo>(rawjson);

                return /*info.Data.FollowUnread + */info.Data.UnfollowUnread != 0;
            }

            return false;
        }

        public async Task<List<MessageSession>> GetLast20MessagesAsync()
        {
            var apiurl = "https://api.vc.bilibili.com/session_svr/v1/session_svr/get_sessions" +
                "?session_type=2" +
                "&group_fold=1" +
                "&unfollow_fold=1" +
                "&sort_rule=2" +
                "&build=0" +
                "&mobi_app=web";

            var response = await clientProxy.GetAsync(apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                var info = JsonConvert.DeserializeObject<MessageListInfo>(rawjson);

                return info.Data.SessionList;
            }

            return null;
        }

        public async Task MarkMessageAsReadAsync(string talkerId, int sessionType, string seqno)
        {
            var apiurl = "https://api.vc.bilibili.com/session_svr/v1/session_svr/update_ack";

            Dictionary<string, string> para = new()
            {
                { "talker_id", talkerId },
                { "session_type", sessionType.ToString() },
                { "ack_seqno", seqno },
                { "build", "0" },
                { "mobi_app", "web" }
            };

            await clientProxy.PostAsync(para, apiurl);
        }

        public async Task<SendMessageResponseInfo> SendMessageAsync(string uid, string msg)
        {
            // NO REPLY!

            return new SendMessageResponseInfo { Code = 0, Message = "当前版本禁止发送回复" };


            var apiurl = "https://api.vc.bilibili.com/web_im/v1/web_im/send_msg";

            var accountInfo = Utils.Utils.GetAccountInfo();

            Dictionary<string, string> para = new()
            {
                { "msg[sender_uid]", accountInfo.Uid },
                { "msg[receiver_id]", uid },
                { "msg[receiver_type]", "1" },
                { "msg[msg_type]", "1" },
                { "msg[msg_status]", "0" },
                { "msg[content]", $"{{\"content\":\"{msg}\"}}" },
                { "msg[timestamp]", Utils.Utils.GetTimeStamp().ToString() },
                { "msg[new_face_version]", "0" },
                { "msg[dev_id]", accountInfo.DevId },
            };


            var response = await clientProxy.PostAsync(para, apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<SendMessageResponseInfo>(rawjson);

                 
            }

            return null;
        }

        public async Task<bool> CheckVideoStatsAsync(string bvid)
        {
            var apiurl = "http://api.bilibili.com/x/web-interface/archive/stat" +
                $"?bvid={bvid}";

            var response = await clientProxy.GetAsync(apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                var stat = JsonConvert.DeserializeObject<VideoStatsInfo>(rawjson);

                return stat.Code == 0;
            }

            return false;
        }

        public async Task<VideoInfo> GetVideoInfoAsync(string bvid)
        {
            var apiurl = "http://api.bilibili.com/x/web-interface/view" +
                $"?bvid={bvid}";

            var response = await clientProxy.GetAsync(apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<VideoInfo>(rawjson);
            }

            return null;
        }

        public async Task<UserInfo> GetUserInfoAsync(string mid)
        {
            var apiurl = "http://api.bilibili.com/x/space/acc/info" +
                $"?mid={mid}";

            var response = await clientProxy.GetAsync(apiurl);

            if (response.IsSuccessStatusCode)
            {
                var rawjson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<UserInfo>(rawjson);
            }

            return null;
        }
    }
}
