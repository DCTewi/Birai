using BiliConnect.Data;

namespace Birai.Services
{
    public interface IMessageAnalyzer
    {
        Task<bool> AnalyzeAsync(MessageSession session, string message);
    }
}
