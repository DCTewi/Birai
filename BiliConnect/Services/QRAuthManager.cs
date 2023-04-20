using BiliConnect.Data;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System.Text;

namespace BiliConnect.Services
{
    public class QRAuthManager : IQRAuthManager
    {
        private readonly ILogger<QRAuthManager> _logger;
        private readonly IHttpClientFactory _httpFactory;
        private readonly BilibiliConnectOptions _options;

        private readonly Dictionary<string, string> _cookies = new();
        private string _cookiesRaw = string.Empty;

        public QRAuthManager(IHttpClientFactory httpFactory, IOptions<BilibiliConnectOptions> options, ILogger<QRAuthManager> logger)
        {
            _logger = logger;
            _httpFactory = httpFactory;
            _options = options.Value;
        }

        public Dictionary<string, string> Cookies() => _cookies;

        public string CookieTexts() => _cookiesRaw;

        public async Task<bool> CheckAuthStatusAsync()
        {
            if (string.IsNullOrEmpty(_cookiesRaw))
            {
                await ReadCookieCacheAsync();
            }

            if (!_cookies.ContainsKey(Literals.Parameters.AuthToken))
            {
                _logger.LogWarning("Cookies invalid: {Cookies}", _cookiesRaw);
                return false;
            }

            using var httpClient = _httpFactory.CreateClient("bili");
            var content = new HttpRequestMessage(HttpMethod.Get, Literals.Apis.CheckAuthStatus);
            content.Headers.Add(Literals.Parameters.Cookie, _cookiesRaw);

            var resp = await httpClient.SendAsync(content);
            if (resp.IsSuccessStatusCode)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();
                var jsonresult = JsonConvert.DeserializeObject<GenerateJsonResult<AuthStatusData>>(rawJson);

                if (jsonresult?.StatusCode == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<QRCodeStatus> FetchCookiesAsync(QRCodeData code)
        {
            using var httpClient = _httpFactory.CreateClient("bili");
            var apiurl = $"{Literals.Apis.PollQRCodeCookie}" +
                $"?qrcode_key={code.Key}";
            var content = new HttpRequestMessage(HttpMethod.Get, apiurl);

            var resp = await httpClient.SendAsync(content);
            if (resp.IsSuccessStatusCode)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();
                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<PollQRCodeData>>(rawJson);

                if (jsonResult?.Data != null && jsonResult.StatusCode == 0)
                {
                    if (jsonResult.Data.Code == (int)QRCodeStatus.Success)
                    {
                        if (resp.Headers.Contains("Set-Cookie"))
                        {
                            var cookieStrs = resp.Headers.GetValues("Set-Cookie");
                            if (cookieStrs != null)
                            {
                                var cookieStr = string.Join(";", cookieStrs
                                    .Select(s => s.Contains(';') ? s.Split(';')[0] : s));

                                if (!File.Exists(_options.CookieSavePath))
                                {
                                    File.Create(_options.CookieSavePath).Close();
                                }
                                await File.WriteAllTextAsync(_options.CookieSavePath, cookieStr.ToString(), Encoding.UTF8);
                                await ReadCookieCacheAsync();
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Cookie update failed: what?");
                        }
                    }
                    _logger.LogInformation("Cookie update success.");
                    return (QRCodeStatus)jsonResult.Data.Code;
                }
                else
                {
                    _logger.LogWarning("Cookie update failed: api returns failed: {Status}.", jsonResult?.Data?.Code);
                }
            }
            else
            {
                _logger.LogWarning("Cookie update failed: network error.");
            }

            return QRCodeStatus.Expired;
        }

        public async Task<QRCodeData?> GetNewQRCodeAsync()
        {
            using var httpClient = _httpFactory.CreateClient("bili");
            var content = new HttpRequestMessage(HttpMethod.Get, Literals.Apis.GenerateQRCode);

            var resp = await httpClient.SendAsync(content);
            if (resp.IsSuccessStatusCode)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();
                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<QRCodeData>>(rawJson);

                if (jsonResult != null && jsonResult.StatusCode == 0)
                {
                    _logger.LogInformation("Fetched new QRCode.");
                    return jsonResult.Data;
                }
            }

            _logger.LogInformation("Fetched new QRCode failed: network error.");
            return null;
        }

        private async Task ReadCookieCacheAsync()
        {
            var path = _options.CookieSavePath;

            if (!File.Exists(path))
            {
                return;
            }

            var cookieRawText = await File.ReadAllTextAsync(path, Encoding.UTF8);

            if (!string.IsNullOrEmpty(cookieRawText))
            {
                _cookiesRaw = cookieRawText.Trim();
                _cookiesRaw.Split(';').ToList()
                    .ForEach(s =>
                    {
                        var kp = s.Trim().Split('=');
                        _cookies.Add(kp[0].TrimEnd(), kp[1].TrimStart());
                    });
            }
        }
    }
}
