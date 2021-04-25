using Birai.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Birai.Services
{
    public class Bot
    {
        private readonly Regex BVRegex = new(@"^[bB][vV][0-9a-zA-Z]{1,}$");

        private readonly BiraiProxy biraiProxy = new();

        private List<MessageSession> currentSession;

        public async Task TickAsync()
        {
            // Console.WriteLine("Heartbeat");

            await CheckForNewRequests();
        }

        private async Task SaveNewItemAsync(string uid, string bvid, string desciption)
        {
            var vinfo = await biraiProxy.GetVideoInfoAsync(bvid);
            var uinfo = await biraiProxy.GetUserInfoAsync(uid);

            var newline = $"https://bilibili.com/{bvid}," +
                $"{vinfo.Data.Title.Replace(',', ' ')}," +
                $"{vinfo.Data.TypeName}," +
                $"{vinfo.Data.DurationSeconds / 60}:{vinfo.Data.DurationSeconds % 60:00}," +
                (vinfo.Data.CopyRight == 1 ? "原创," : "搬运,") +
                $"{uinfo.Data.Name}({uid})," +
                $"{desciption.Replace(',', ' ')}," +
                $"{Environment.NewLine}";

            await File.AppendAllTextAsync("app/videos.csv", newline);
        }

        private async Task CheckForNewRequests()
        {
            if (await biraiProxy.CheckForNewMessageAsync())
            {
                Console.WriteLine("|||>\n|||> 收到新消息 ");

                currentSession = await biraiProxy.GetLast20MessagesAsync();

                if (currentSession != null)
                {
                    foreach (var session in currentSession)
                    {
                        if (session.UnreadCount > 0)
                        {
                            if (session.Body.MessageType == 1)
                            {
                                var message = JsonConvert.DeserializeObject<ContentWrapper>(session.Body.WrappedContent).Raw;

                                Console.WriteLine($"||> 发送者: {session.TalkerId}");
                                Console.WriteLine($"||> 消息内容: {message}");

                                if (message.StartsWith("/推荐"))
                                {
                                    var messageSlice = message.Split(' ');

                                    if (messageSlice.Length >= 3)
                                    {
                                        var bvid = messageSlice[1].Trim();
                                        var descs = new string[messageSlice.Length - 2];

                                        Array.Copy(messageSlice, 2, descs, 0, descs.Length);
                                        for (int i = 0; i < descs.Length; i++)
                                        {
                                            descs[i] = descs[i].Trim();
                                        }

                                        var desc = string.Join('\0', descs);

                                        if (BVRegex.IsMatch(bvid) && await biraiProxy.CheckVideoStatsAsync(bvid))
                                        {
                                            if (Utils.Utils.CheckIfSeen(bvid))
                                            {
                                                Console.WriteLine($"|> 视频已看过");
                                                var result = await biraiProxy.SendMessageAsync(session.TalkerId, "[自动回复] 该视频已经被鉴赏过");
                                                if (result == null)
                                                {
                                                    Console.WriteLine("|> 回复发生未知错误");
                                                }
                                                else Console.WriteLine(result.Code == 0 ? $"|> 回复已发送 {result.Message}" : $"|> 回复发送失败{result.Message}");

                                                Thread.Sleep(200);
                                            }
                                            else
                                            {
                                                Console.WriteLine($"|> 符合标准, 尝试添加");
                                                var result = await biraiProxy.SendMessageAsync(session.TalkerId, "[自动回复] 视频成功添加至审核队列, 感谢支持");
                                                if (result == null)
                                                {
                                                    Console.WriteLine("|> 回复发生未知错误");
                                                }
                                                else Console.WriteLine(result.Code == 0 ? $"|> 回复已发送 {result.Message}" : $"|> 回复发送失败 {result.Message}");

                                                await SaveNewItemAsync(session.TalkerId, bvid, desc);
                                                Thread.Sleep(200);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"|> 视频不存在");
                                            var result = await biraiProxy.SendMessageAsync(session.TalkerId, "[自动回复] 视频不存在, 请检查BV号");
                                            if (result == null)
                                            {
                                                Console.WriteLine("|> 回复发生未知错误");
                                            }
                                            else Console.WriteLine(result.Code == 0 ? $"|> 回复已发送 {result.Message}" : $"|> 回复发送失败 {result.Message}");

                                            Thread.Sleep(200);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"|> 格式不正确");
                                        var result = await biraiProxy.SendMessageAsync(session.TalkerId, "[自动回复] 格式不正确, 正确格式为: /推荐 BV号 推荐理由");
                                        if (result == null)
                                        {
                                            Console.WriteLine("|> 回复发生未知错误");
                                        }
                                        else Console.WriteLine(result.Code == 0 ? $"|> 回复已发送{result.Message}" : $"|> 回复发送失败{result.Message}");

                                        Thread.Sleep(200);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("|> 系统消息, 无视. ");
                            }

                            await biraiProxy.MarkMessageAsReadAsync(session.TalkerId, session.SessionType, session.MaxSeqNo);
                        }
                    }
                }
            }
        }
    }
}
