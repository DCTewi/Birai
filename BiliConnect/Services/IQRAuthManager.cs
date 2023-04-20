using BiliConnect.Data;

namespace BiliConnect.Services
{
    public interface IQRAuthManager
    {
        string CookieTexts();

        Dictionary<string, string> Cookies();

        Task<bool> CheckAuthStatusAsync();

        Task<QRCodeData?> GetNewQRCodeAsync();

        Task<QRCodeStatus> FetchCookiesAsync(QRCodeData code);
    }
}
