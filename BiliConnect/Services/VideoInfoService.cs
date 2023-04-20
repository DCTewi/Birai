using BiliConnect.Data;

using Newtonsoft.Json;

namespace BiliConnect.Services
{
    public class VideoInfoService
    {
        private readonly IBilibiliProxy _proxy;

        public VideoInfoService(IBilibiliProxy proxy)
        {
            _proxy = proxy;
        }

        public async Task<bool> CheckStatusAsync(string avNumberOrBVNumber)
        {
            bool isBV = avNumberOrBVNumber.StartsWith("BV");

            var apiurl = Literals.Apis.VideoInfo + $"?{(isBV ? "bvid" : "aid")}={avNumberOrBVNumber[(isBV ? 0 : 2)..]}";
            var resp = await _proxy.GetAsync(apiurl);

            if (resp?.IsSuccessStatusCode ?? false)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<object>>(rawJson);

                return jsonResult != null && jsonResult.StatusCode == 0;
            }

            return false;
        }

        public async Task<VideoInfoData> GetAsync(string avNumberOrBVNumber)
        {
            bool isBV = avNumberOrBVNumber.StartsWith("BV");

            var apiurl = Literals.Apis.VideoInfo + $"?{(isBV ? "bvid" : "aid")}={avNumberOrBVNumber[(isBV ? 0 : 2)..]}";
            var resp = await _proxy.GetAsync(apiurl);

            if (resp?.IsSuccessStatusCode ?? false)
            {
                var rawJson = await resp.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<GenerateJsonResult<VideoInfoData>>(rawJson);

                return jsonResult?.Data ?? new();
            }

            return new();
        }
    }
}
