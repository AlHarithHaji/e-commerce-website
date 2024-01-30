#nullable disable
using He_SheStore.Models;

namespace He_SheStore.ViewModel
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> FirstProduct { get; set; }
        public IEnumerable<Product> SecondProduct { get; set; }
        public IEnumerable<Product> LastProduct { get; set; }
        public IEnumerable<testimonial> testimonial { get; set; }

    }
}
