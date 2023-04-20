namespace BiliConnect
{
    public class BilibiliConnectOptions
    {
        public BilibiliConnectOptions() { }

        public const string Bilibili = "Bilibili";

        public bool UseProxy { get; set; } = false;

        public string ProxyUrl { get; set; } = string.Empty;

        public string CookieSavePath { get; set; } = "./cookies.txt";
    }
}
