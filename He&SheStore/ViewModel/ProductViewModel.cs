#nullable disable
using He_SheStore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Product Price")]
        public decimal ProductPrice { get; set; }
        [Required]
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }
        [Required]
        [Display(Name = "Product Picture")]
        public string ProductPicture { get; set; }
        [NotMapped]
        public IFormFile ProductFile { get; set; }
        [Required]
        [Display(Name = "Select Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        public Category Category { get; set; }
        public string[] ProductSize { get; set; }
        [Required]
        [Display(Name = "Product quantity")]
        public int available { get; set; }
        [Required]
        [Display(Name = "Purchase Price")]

        public decimal PurchasePrice { get; set; }
    }
}
