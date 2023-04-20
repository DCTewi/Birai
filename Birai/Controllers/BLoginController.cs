using BiliConnect.Data;
using BiliConnect.Services;

using Birai.Models.BLogin;

using Microsoft.AspNetCore.Mvc;

namespace Birai.Controllers
{
    public class BLoginController : Controller
    {
        private readonly IQRAuthManager _authManager;

        public BLoginController(IQRAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var qrcode = await _authManager.GetNewQRCodeAsync();

            var model = new LoginModel
            {
                QRCodeKey = qrcode?.Key ?? string.Empty,
                QRCodeUrl = qrcode?.Url ?? string.Empty,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var qrcode = new QRCodeData
                {
                    Key = model.QRCodeKey,
                    Url = model.QRCodeUrl,
                };
                var result = await _authManager.FetchCookiesAsync(qrcode);

                ViewData["Result"] = result;

                switch (result)
                {
                    case QRCodeStatus.Unconfirmed:
                    case QRCodeStatus.Unscaned:
                        return View(model);

                    case QRCodeStatus.Success:
                        return RedirectToAction(nameof(HomeController.Index), "Home");

                    case QRCodeStatus.Expired:
                        ModelState.Clear();
                        return await Login();
                }
            }

            return RedirectToAction(nameof(BLoginController.Login), "BLogin");
        }
    }
}
