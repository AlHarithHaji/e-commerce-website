#nullable disable
using Microsoft.Build.Framework;

namespace He_SheStore.Models
{
    public class testimonial
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LsdtName { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]

        public string ReviewAbout { get; set; }

        public string Status { get; set; }  

    }
}
