#nullable disable
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.Models
{
    public class AccountBalance
    {
        public int Id { get; set; } 
        public string UserID { get; set; }
        [Required]
        public string Amount { get; set; }  
    }
}
