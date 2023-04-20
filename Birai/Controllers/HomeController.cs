using BiliConnect.Services;

using Birai.Data;
using Birai.Models;
using Birai.Models.Home;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using System.Diagnostics;

namespace Birai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQRAuthManager _authManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IQRAuthManager authManager, ApplicationDbContext db)
        {
            _logger = logger;
            _authManager = authManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _authManager.CheckAuthStatusAsync())
            {
                return RedirectToAction(nameof(BLoginController.Login), "BLogin");
            }

            var model = new IndexModel
            {
                VideoInfos = _db.VideoInfos.AsNoTracking().ToList(),
            };

            return View(model);
        }

        public IActionResult Export()
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("videos");

            sheet.Cells.Style.Font.Name = "Source Code Pro";
            sheet.Cells.Style.Font.Size = 10;

            var videos = _db.VideoInfos.AsNoTracking().ToList();

            for (int i = 0; i < videos.Count; ++i)
            {
                var v = videos[i];

                sheet.Cells[i + 1, 1].Value = $"https://bilibili.com/{v.Bvid}";
                sheet.Cells[i + 1, 1].Style.Font.Color.SetColor(0, 17, 85, 204);
                sheet.Cells[i + 1, 1].Style.Font.UnderLine = true;
                sheet.Cells[i + 1, 1].Hyperlink = new Uri($"https://bilibili.com/{v.Bvid}");
                sheet.Cells[i + 1, 2].Value = v.Title;
                sheet.Cells[i + 1, 3].Value = v.TypeName;
                _ = int.TryParse(v.Duration, out int duration);
                sheet.Cells[i + 1, 4].Value = string.Format("{0:00}:{1:00}", duration / 60, duration % 60);
                sheet.Cells[i + 1, 5].Value = v.CopyRight ? "原创" : "搬运";
                sheet.Cells[i + 1, 6].Value = $"{v.SubmiterName}({v.SubmiterId})";
                sheet.Cells[i + 1, 7].Value = v.SubmitReason;
            }

            sheet.Cells.AutoFitColumns();

            var excelData = package.GetAsByteArray();
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            const string filename = "导出.xlsx";

            return File(excelData, contentType, filename);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}