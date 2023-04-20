using BiliConnect.Services;

using Microsoft.Extensions.DependencyInjection;

using System.Net;

namespace BiliConnect
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBilibiliConnect(this IServiceCollection services, Action<BilibiliConnectOptions> option)
        {
            var options = new BilibiliConnectOptions();
            option(options);

            if (options.UseProxy)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri(options.ProxyUrl),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                };

                services.AddHttpClient("bili")
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                    {
                        Proxy = proxy,
                    });
            }
            else
            {
                services.AddHttpClient("bili");
            }

            services.AddSingleton<IQRAuthManager, QRAuthManager>();
            services.AddSingleton<IBilibiliProxy, BilibiliProxy>();

            services.AddSingleton<AccountService>();
            services.AddSingleton<UserInfoService>();
            services.AddSingleton<VideoInfoService>();

            services.AddSingleton<MessageService>();

            services.Configure(option);
        }
    }
}
