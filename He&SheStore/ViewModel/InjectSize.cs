#nullable disable
using He_SheStore.Areas.Identity.Data;
using He_SheStore.Models;

namespace He_SheStore.ViewModel
{
    public class InjectSize
    {
        private readonly ApplicationDbContext _context;
        public InjectSize(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<ProductSize>> productSizes(int productId)
        {
            List<ProductSize> getSize = new List<ProductSize>();
            getSize = _context.ProductSizes.Where(x => x.ProductId == productId).ToList();
            if (getSize != null)
            {
                return getSize;
            }
            return getSize;
        }
    }
}
