using System.ComponentModel.DataAnnotations;

namespace Birai.Models.BLogin
{
    public class LoginModel
    {
        [Required]
        public string QRCodeUrl { get; set; } = string.Empty;

        [Required]
        public string QRCodeKey { get; set; } = string.Empty;
    }
}
