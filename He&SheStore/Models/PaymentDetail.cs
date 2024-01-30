#nullable disable
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.Models
{
    public class PaymentDetail
    {
        [Key]
        public int Id { get; set; }
        public string cardNumber { get; set; }
        public string ExpiryDate { get; set; }  
        public string CVV { get; set; }
        public int OrderId { get; set; }
       
        public string CardName { get; set; }
    }
}
