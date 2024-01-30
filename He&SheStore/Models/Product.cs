#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace He_SheStore.Models
{
    public class Product
    {
        [Key]
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

        [Display(Name = "Product Picture")]
        public string ProductPicture { get; set; }
        [NotMapped]
        public IFormFile ProductFile { get; set; }
        [Required]
        [Display(Name = "Select Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

         public Category Category { get; set; }
         public int ProductQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
