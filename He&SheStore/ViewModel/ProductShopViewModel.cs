//#nullable disable
//using He_SheStore.Models;

//namespace He_SheStore.ViewModel
//{
//    public class ProductShopViewModel
//    {
//        public IEnumerable<Product> Product { get; set; }
//        public IEnumerable<Category> Category { get; set; }


//        //for product detail
//        public Product DetailProduct { get; set; }
//        public Category DetailCategory { get; set; }
//        public ProductSize ProductSize { get; set; }
//    }
//}
//#nullable disable
//using He_SheStore.Models;

//namespace He_SheStore.ViewModel
//{
//    public class ProductShopViewModel
//    {
//        public IEnumerable<Product> Product { get; set; }
//        public IEnumerable<Category> Category { get; set; }


//        //for product detail
//        public Product DetailProduct { get; set; }
//        public Category DetailCategory { get; set; }
//        public ProductSize ProductSize { get; set; }

//    }
//}
#nullable disable
using He_SheStore.Models;

namespace He_SheStore.ViewModel
{
    public class ProductShopViewModel
    {
        public IEnumerable<Product> Product { get; set; }
        public IEnumerable<Category> Category { get; set; }

        // For product detail
        public Product DetailProduct { get; set; }
        public IEnumerable<ProductSize> ProductSizes { get; set; }
        public IEnumerable<ProductReview> ProductReviews { get; set; }
        public ProductReview NewReview { get; set; } // Added property for a new review

        // Constructor
        public ProductShopViewModel()
        {
            ProductSizes = new List<ProductSize>();
            ProductReviews = new List<ProductReview>();
            NewReview = new ProductReview();
        }
    }
}
