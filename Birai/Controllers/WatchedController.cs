using Birai.Data;
using Birai.Models.Watched;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Text.RegularExpressions;

namespace Birai.Controllers
{
    public class WatchedController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WatchedController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new IndexModel
            {
                Watches = _db.WatchedVideo.AsNoTracking().ToList(),
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddModel model)
        {
            if (ModelState.IsValid)
            {
                var reg = new Regex(@"^[bB][vV][0-9a-zA-Z]{1,}$");

                var watchesRaw = model
                    .Watches
                    .Trim()
                    .Split('\n')
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s) && reg.IsMatch(s.Trim()))
                    .Select(s => s.Trim());

                foreach (var w in watchesRaw)
                {
                    await _db.WatchedVideo.AddAsync(new WatchedVideo
                    {
                        Bvid = w
                    });
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
