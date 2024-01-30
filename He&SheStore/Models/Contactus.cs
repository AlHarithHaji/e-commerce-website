#nullable disable
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.Models
{
    public class Contactus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }
        [Required]
        [Display(Name = "Your Message")]
        public string CustomerMessage { get; set; }
        [Required]
        [Display(Name = "Your Message Subject")]
        public string MessageSubject { get; set; }
        public string MessageDate { get; set; }
        public string MessagesStatus { get; set; }
    }
}
