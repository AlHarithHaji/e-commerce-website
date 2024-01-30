#nullable disable
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display (Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }
        [Required]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; }
        [Required]
        [Display(Name = "Adress")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "City ")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string userId { get; set; }
    }
}
