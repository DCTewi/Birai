namespace Birai.Data
{
    public class VideoInfo
    {
        public int Id { get; set; }

        public string Bvid { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string TypeName { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;

        public bool CopyRight { get; set; } = false;

        public string SubmiterId { get; set; } = string.Empty;

        public string SubmiterName { get; set; } = string.Empty;

        public string SubmitReason { get; set; } = string.Empty;
    }
}
