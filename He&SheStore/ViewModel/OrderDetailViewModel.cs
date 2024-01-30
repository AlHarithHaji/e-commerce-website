#nullable disable
namespace He_SheStore.ViewModel
{
    public class OrderDetailViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string OrderNote { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }

        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }

        public string ProductSize { get; set; }

        public string ProductName { get; set; }

        public string ProductPicture { get; set; }

        public decimal ProductPrice { get; set; }


    }
}
