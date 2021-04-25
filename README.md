# Birai4Otome
A bilibili bot

你需要手动创建以下文件并设置为复制到输出目录。

1. app/cookies.txt

直接放置进Header的cookie字符串。

2. app/account.json

```json
{
  "uid": "你的UID",
  "dev_id": "发送私信的表单需要的DEV_ID，是一串UUID"
}
```

3. app/videos.csv

用于接收推荐视频结果，留空。

4. app/history.txt

播放历史，每行一个BV号。
