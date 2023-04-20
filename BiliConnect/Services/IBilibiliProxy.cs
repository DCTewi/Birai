namespace BiliConnect.Services
{
    public interface IBilibiliProxy
    {
        string DevId();

        Task<HttpResponseMessage> GetAsync(string url);

        Task<HttpResponseMessage> PostAsync(IDictionary<string, string> parameters, string url);
    }
}
