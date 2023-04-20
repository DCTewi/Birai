using BiliConnect.Data;

using Newtonsoft.Json;

using System.Text.RegularExpressions;

namespace BiliConnect.Services
{
    public class UserInfoService
    {
        private readonly IBilibiliProxy _proxy;

        public UserInfoService(IBilibiliProxy proxy)
        {
            _proxy = proxy;
        }

        public async Task<UserInfoData> GetAsync(string mid)
        {
            var apiurl = Literals.Apis.UserInfo + $"?mid={mid}";
            var resp = await _proxy.GetAsync(apiurl);

            if (resp?.IsSuccessStatusCode ?? false)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<UserInfoData>>(rawJson);

                return jsonResult?.Data ?? new();
            }
            else if (resp?.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var rawContent = await resp.Content.ReadAsStringAsync();
                if (rawContent.StartsWith(@"{""code"":-509,"))
                {
                    var reg = new Regex(@"\{.*?\}");
                    var userInfoAttemptJson = reg.Replace(rawContent, string.Empty, 1);

                    try
                    {
                        var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<UserInfoData>>(userInfoAttemptJson);

                        return jsonResult?.Data ?? new();
                    }
                    catch { }
                }
            }

            return new();
        }
    }
}
