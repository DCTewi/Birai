using BiliConnect.Data;

using Microsoft.Extensions.Logging;

namespace BiliConnect.Services
{
    public class BilibiliProxy : IBilibiliProxy
    {
        private readonly ILogger<BilibiliProxy> _logger;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IQRAuthManager _qrAuthManager;

        public string DevId()
        {
            if (string.IsNullOrEmpty(_devId))
            {
                const string baseStr = "0123456789ABCDEF";
                var devid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".ToCharArray();
                var random = new Random();

                for (int i = 0; i < devid.Length; ++i)
                {
                    if (devid[i] == '-' || devid[i] == '4')
                    {
                        continue;
                    }
                    if (devid[i] == 'x')
                    {
                        devid[i] = baseStr[random.Next(baseStr.Length)];
                    }
                    else
                    {
                        devid[i] = baseStr[3 & random.Next(baseStr.Length) | 8];
                    }
                }
                _logger.LogInformation("Generate new devId: {DevId}", _devId);
                return _devId = devid?.ToString() ?? string.Empty;
            }
            return _devId;
        }
        private string _devId = string.Empty;

        public BilibiliProxy(IHttpClientFactory httpFactory,
            IQRAuthManager qrAuthManager,
            ILogger<BilibiliProxy> logger)
        {
            _logger = logger;
            _httpFactory = httpFactory;
            _qrAuthManager = qrAuthManager;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var content = new HttpRequestMessage(HttpMethod.Get, url);
            content.Headers.Add("cookie", await GetCookiesAsync());
            content.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.48");
            content.Headers.TryAddWithoutValidation("referer", "https://www.bilibili.com");
            content.Headers.TryAddWithoutValidation("origin", "https://www.bilibili.com");

            using var http = _httpFactory.CreateClient("bili");
            var resp = await http.SendAsync(content);

            var rawResult = await resp.Content.ReadAsStringAsync();
            if (rawResult.StartsWith(@"{""code"":-509,"))
            {
                resp.StatusCode = System.Net.HttpStatusCode.TooManyRequests;
                _logger.LogWarning("触发B站风控!\n消息内容:{msg}", rawResult);
            }

            return resp;
        }

        public async Task<HttpResponseMessage> PostAsync(IDictionary<string, string> parameters, string url)
        {
            var token = await GetTokenAsync();
            parameters.Add(Literals.Parameters.CSRF, token);
            parameters.Add(Literals.Parameters.CSRFToken, token);

            var content = new FormUrlEncodedContent(parameters);
            content.Headers.Add("cookie", await GetCookiesAsync());
            content.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.48");
            content.Headers.TryAddWithoutValidation("Referer", "https://www.bilibili.com");
            content.Headers.TryAddWithoutValidation("Origin", "https://www.bilibili.com");

            using var http = _httpFactory.CreateClient("bili");
            var resp = await http.PostAsync(url, content);

            var rawResult = await resp.Content.ReadAsStringAsync();
            if (rawResult.StartsWith(@"{""code"":-509,"))
            {
                resp.StatusCode = System.Net.HttpStatusCode.TooManyRequests;
                _logger.LogWarning("触发B站风控!\n消息内容:{msg}", rawResult);
            }

            return resp;
        }

        private async Task<bool> CheckCookieStatusAsync()
        {
            if (!await _qrAuthManager.CheckAuthStatusAsync())
            {
                _logger.LogWarning("Bilibili service is not logged or cookie expired.");
                return false;
            }

            if (!_qrAuthManager.Cookies().ContainsKey(Literals.Parameters.AuthToken))
            {
                _logger.LogWarning("Read cookies failed.");
                return false;
            }

            return true;
        }

        private async Task<string> GetTokenAsync()
        {
            if (await CheckCookieStatusAsync())
            {
                return _qrAuthManager.Cookies()[Literals.Parameters.AuthToken];
            }

            return string.Empty;
        }

        private async Task<string> GetCookiesAsync()
        {
            if (await CheckCookieStatusAsync())
            {
                return _qrAuthManager.CookieTexts();
            }

            return string.Empty;
        }
    }
}
