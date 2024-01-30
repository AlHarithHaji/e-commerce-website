#nullable disable
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
    }
}
