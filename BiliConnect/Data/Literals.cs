namespace BiliConnect.Data
{
    public static class Literals
    {
        public static class Parameters
        {
            public const string Cookie = "cookie";

            public const string AuthToken = "bili_jct";

            public const string CSRF = "csrf";

            public const string CSRFToken = "csrf_token";
        }

        public static class Apis
        {
            public const string CheckAuthStatus = "https://api.bilibili.com/x/web-interface/nav";

            public const string GenerateQRCode = "https://passport.bilibili.com/x/passport-login/web/qrcode/generate";

            public const string PollQRCodeCookie = "https://passport.bilibili.com/x/passport-login/web/qrcode/poll";

            public const string UserInfo = "http://api.bilibili.com/x/space/acc/info";

            public const string VideoInfo = "http://api.bilibili.com/x/web-interface/view";

            public const string VideoStatus = "http://api.bilibili.com/x/web-interface/archive/stat";

            public const string GetMessageUnread = "http://api.vc.bilibili.com/session_svr/v1/session_svr/single_unread";

            public const string GetMessageSessions = "https://api.vc.bilibili.com/session_svr/v1/session_svr/get_sessions";

            public const string MarkMessageAsRead = "https://api.vc.bilibili.com/session_svr/v1/session_svr/update_ack";
        }
    }
}
