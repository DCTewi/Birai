using BiliConnect.Data;

using Newtonsoft.Json;

namespace BiliConnect.Services
{
    public class AccountService
    {
        private readonly IBilibiliProxy _proxy;

        public AccountService(IBilibiliProxy proxy)
        {
            _proxy = proxy;
        }

        public async Task<AuthStatusData> GetAsync()
        {
            var apiurl = Literals.Apis.CheckAuthStatus;
            var resp = await _proxy.GetAsync(apiurl);

            if (resp?.IsSuccessStatusCode ?? false)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<AuthStatusData>>(rawJson);

                return jsonResult?.Data ?? new();
            }

            return new();
        }
    }
}
