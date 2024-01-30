#nullable disable
namespace He_SheStore.Models
{
    public class ProductItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public string size { get; set; }
        private decimal _SubTotal;
        public decimal SubTotal
        {
            get { return _SubTotal; }
            set { _SubTotal = Product.ProductPrice * Quantity; }
        }
    }
}
