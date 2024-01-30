#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace He_SheStore.Models
{
    public class ProductSize
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]

        public Product Product { get; set; }
    }
}
