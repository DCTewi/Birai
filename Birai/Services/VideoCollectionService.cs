using BiliConnect.Data;
using BiliConnect.Services;

using Birai.Data;

using Microsoft.EntityFrameworkCore;

using System.Text.RegularExpressions;

namespace Birai.Services
{
    public class VideoCollectionService : IMessageAnalyzer
    {

        private readonly Regex _vidRegex = new(@"(^[aA][vV][0-9]{1,}|^[bB][vV][0-9a-zA-Z]{1,})");

        private readonly IList<string> _prefix = new List<string>
        {
            "推荐", "/推荐"
        };

        private readonly ILogger<VideoCollectionService> _logger;
        private readonly MessageService _messageService;
        private readonly VideoInfoService _videoInfoService;
        private readonly UserInfoService _userInfoService;
        private readonly IServiceProvider _serviceProvider;

        public VideoCollectionService(ILogger<VideoCollectionService> logger, MessageService messageService, VideoInfoService videoInfoService, UserInfoService userInfoService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _messageService = messageService;
            _videoInfoService = videoInfoService;
            _userInfoService = userInfoService;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> AnalyzeAsync(MessageSession session, string message)
        {
            var trimmedMsg = message.Trim();
            if (_prefix.Any(s => trimmedMsg.StartsWith(s)))
            {
                var pureMsg = trimmedMsg.Substring(_prefix.First(p => trimmedMsg.StartsWith(p)).Length).Trim();
                _logger.LogInformation("视频鉴赏插件获得新消息: {msg}", pureMsg);

                if (!_vidRegex.IsMatch(pureMsg))
                {
                    _logger.LogInformation("视频编号没有在期望的位置上, 返回错误消息");
                    await SendMessageWithValidationAsync(session.TalkerId, "[自动回复] 没有找到视频编号, 请检查消息格式");
                    return true;
                }
                var vid = _vidRegex.Match(pureMsg).Value;
                if (!await _videoInfoService.CheckStatusAsync(vid))
                {
                    await Task.Delay(200);
                    _logger.LogInformation("视频状态异常, 视频编号:{vid}, 返回错误消息", vid);
                    await SendMessageWithValidationAsync(session.TalkerId, $"[自动回复] 视频[{vid}]不存在, 请检查视频编号");
                    return true;
                }

                await Task.Delay(200);
                var infoData = await _videoInfoService.GetAsync(vid);

                if (string.IsNullOrEmpty(infoData?.Bvid))
                {
                    _logger.LogError("获取视频详情失败! 视频编号:{vid}, 装作无事发生", vid);
                    await SendMessageWithValidationAsync(session.TalkerId, $"[自动回复] 添加失败, 请稍后再试");
                    return true;
                }

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (db.WatchedVideo.AsNoTracking().Any(v => v.Bvid == infoData.Bvid))
                {
                    _logger.LogInformation("视频编号:{vid}已经看过", vid);
                    await SendMessageWithValidationAsync(session.TalkerId, $"[自动回复] 视频已经观看过, 请推荐其他视频");
                    return true;
                }

                if (db.VideoInfos.AsNoTracking().Any(v => v.Bvid == infoData.Bvid))
                {
                    _logger.LogInformation("视频编号:{vid}已经在审核列表中", vid);
                    await SendMessageWithValidationAsync(session.TalkerId, $"[自动回复] 视频在审核列表内, 请推荐其他视频");
                    return true;
                }

                await Task.Delay(200);
                var submitterName = (await _userInfoService.GetAsync(session.TalkerId)).Name;

                var reason = _vidRegex.Replace(pureMsg, string.Empty).Trim();

                var info = new VideoInfo
                {
                    Bvid = infoData.Bvid,
                    Title = infoData.Title,
                    TypeName = infoData.TypeName,
                    Duration = infoData.DurationSeconds.ToString(),
                    CopyRight = infoData.CopyRight == 1,
                    SubmiterId = session.TalkerId,
                    SubmiterName = submitterName,
                    SubmitReason = reason,
                };

                await db.AddAsync(info);
                await db.SaveChangesAsync();

                _logger.LogInformation("符合标准, 已添加");
                await SendMessageWithValidationAsync(session.TalkerId, $"[自动回复] 视频成功添加至审核队列, 感谢支持");
                return true;
            }

            return false;
        }
        private async Task SendMessageWithValidationAsync(string talkerId, string msg)
        {
            var result = await _messageService.SendMessageAsync(talkerId, msg);
            if ((result?.StatusCode ?? -1) != 0)
            {
                _logger.LogWarning("消息回复失败!\n返回消息:{msg}", result?.Message ?? string.Empty);
            }
            else
            {
                _logger.LogInformation("消息回复成功\n返回消息:{msg}", result?.Message ?? string.Empty);
            }
        }
    }
}
