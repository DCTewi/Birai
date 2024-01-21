using BiliConnect.Data;
using BiliConnect.Services;

using Newtonsoft.Json;

namespace Birai.Services
{
    public class BotService : BackgroundService
    {

        private readonly ILogger<BotService> _logger;
        private readonly IQRAuthManager _qrAuth;
        private readonly MessageService _messageService;
        private readonly UserInfoService _userinfoService;
        private readonly IEnumerable<IMessageAnalyzer> _messageAnalyzers;

        private int _heartbeatCount = 0;

        public BotService(ILogger<BotService> logger, IQRAuthManager qrAuth, MessageService messageService, UserInfoService userinfoService, IEnumerable<IMessageAnalyzer> messageAnalyzers)
        {
            _logger = logger;
            _qrAuth = qrAuth;
            _messageService = messageService;
            _userinfoService = userinfoService;
            _messageAnalyzers = messageAnalyzers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bot已启动");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (await _qrAuth.CheckAuthStatusAsync())
                {
                    if (++_heartbeatCount >= 50)
                    {
                        _heartbeatCount = 0;
                        _logger.LogInformation("Heartbeat");
                    }

                    await TickAsync();

                    await Task.Delay(200, stoppingToken);
                }
                else
                {
                    _logger.LogWarning("用户未登录");

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task TickAsync()
        {
            var unreadData = await _messageService.CheckNewMessageAsync();
            await Task.Delay(200);
            if (unreadData.UnfollowUnread > 0)
            {
                _logger.LogInformation("检查到未读粉丝消息");

                await ExcuteUnreadMessagesAsync(unfollow: true);
            }

            // followUnread is unchanged, no need to update unread data
            await Task.Delay(200);
            if (unreadData.FollowUnread > 0)
            {
                _logger.LogInformation("检查到未读关注消息");

                await ExcuteUnreadMessagesAsync(unfollow: false);
            }
        }

        private async Task ExcuteUnreadMessagesAsync(bool unfollow)
        {
            var unreadData = await _messageService.CheckNewMessageAsync();
            await Task.Delay(200);

            var sessions = await _messageService.GetAsync(unfollow);
            await Task.Delay(200);

            var currentEndTs = string.Empty;
            do
            {
                sessions = await _messageService.GetAsync(unfollow, currentEndTs);
                await Task.Delay(200);

                foreach (var session in sessions.SessionList)
                {
                    if (session.UnreadCount > 0)
                    {
                        // text
                        if (session.Body.MessageType == 1)
                        {
                            var message = JsonConvert.DeserializeObject<ContentWrapper>(session.Body.WrappedContent)?.MessageContent;
                            if (string.IsNullOrEmpty(message))
                            {
                                _logger.LogWarning("解析文字消息失败! \n消息内容:{msg}", session.Body.WrappedContent);
                            }
                            else
                            {
                                var talker = await _userinfoService.GetAsync(session.TalkerId) ?? new();
                                await Task.Delay(200);

                                _logger.LogInformation("消息内容:\n发送者:{id}\n发送者昵称:{nick}\n消息内容:{msg}",
                                    session.TalkerId, talker.Name, message);

                                foreach (var analyzer in _messageAnalyzers)
                                {
                                    if (analyzer != null)
                                    {
                                        if (await analyzer.AnalyzeAsync(session, message ?? string.Empty))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        // others
                        else
                        {
                            _logger.LogInformation("收到非文字消息\n消息类型:{type}\n内容:{msg}",
                                session.Body.MessageType, session.Body.WrappedContent);
                        }

                        await _messageService.MarkMessageAsReadAsync(session);
                        await Task.Delay(200);
                    }
                }

                unreadData = await _messageService.CheckNewMessageAsync();
                await Task.Delay(200);
                currentEndTs = sessions.SessionList.Last().SessionTimestamp;
            }
            while ((unfollow ? unreadData.UnfollowUnread : unreadData.FollowUnread) > 0 && sessions.HasMore == 1);

            if ((unfollow ? unreadData.UnfollowUnread : unreadData.FollowUnread) > 0)
            {
                _logger.LogError("存在异常未读消息! 是折叠的未关注消息吗:{unfollow}", unfollow);
            }
        }
    }
}
