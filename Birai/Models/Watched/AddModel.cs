using System.ComponentModel.DataAnnotations;

namespace Birai.Models.Watched
{
    public class AddModel
    {
        [Required]
        public string Watches { get; set; } = string.Empty;
    }
}
