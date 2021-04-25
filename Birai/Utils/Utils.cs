using Birai.Data;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Birai.Utils
{
    public static class Utils
    {
        public static long GetTimeStamp() => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

        public static AccountInfo GetAccountInfo()
        {
            var rawjson = File.ReadAllText("app/account.json");

            return JsonConvert.DeserializeObject<AccountInfo>(rawjson);
        }

        public static bool CheckIfSeen(string bvid)
        {
            var raw = File.ReadAllText("app/history.txt");

            var list = raw.Split('\n');
            foreach (var item in list)
            {
                if (item.Trim() == bvid)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
