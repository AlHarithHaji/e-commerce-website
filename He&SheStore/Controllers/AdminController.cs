using He_SheStore.Areas.Identity.Data;
using He_SheStore.Models;
using He_SheStore.ViewModel;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;


namespace He_SheStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminController(ApplicationDbContext applicationDb, IWebHostEnvironment hostEnvironment)
        {
            _context = applicationDb;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            ViewBag.PendingOrder = _context.OrderDetails.Where(x => x.OrderStatus == "Pending").Count();
            ViewBag.PatchOrder = _context.OrderDetails.Where(x => x.OrderStatus == "Patch").Count();
            ViewBag.DeliverOrder = _context.OrderDetails.Where(x => x.OrderStatus == "Deliver").Count();
            ViewBag.CancelOrder = _context.OrderDetails.Where(x => x.OrderStatus == "cancelled").Count();
            ViewBag.Message = _context.Contacts.Where(x => x.MessagesStatus == "UnRead").Count();

            return View();
        }
        public IActionResult CategoryIndex()
        {
            return View();
        }
        public IActionResult CategoryIndexData()
        {
            var getCategory = _context.Categories.ToList();
            return new JsonResult(getCategory);
        }
        public IActionResult CategoryCreate()
        {
            return PartialView("_AddCategory");
        }
        [HttpPost]

        public IActionResult CategoryCreate([FromForm] Category category)
        {

            _context.Categories.Add(category);
            int check = _context.SaveChanges();
            if (check == 1)
            {
                return new JsonResult("Category is Created successfully");
            }

            else
            {
                return new JsonResult("Error! Category is not  Created");
            }
        }

        public IActionResult CategoryEdit(int? id)
        {
            var getCategory = _context.Categories.Find(id);
            if (getCategory == null)
            {
                return new JsonResult("Category is not Found in Database");
            }
            return PartialView("_AddCategory", getCategory);
        }
        [HttpPost]
        public IActionResult CategoryEdit([FromForm] Category category)
        {
            _context.Categories.Update(category);
            int check = _context.SaveChanges();
            if (check == 1)
            {
                return new JsonResult("Category is Updated successfully");
            }

            else
            {
                return new JsonResult("Error! Category is not Updated");
            }

        }    
        //working on the product
        public IActionResult ProductIndex()
        {
            var getProduct = _context.Products.Include(x => x.Category).ToList();
            return View(getProduct);
        }
        public IActionResult ProductCreate()
        {
            var companyData = (from user in _context.Categories select user).ToList();

            if (companyData == null || companyData.Count == 0)
            {

                ViewBag.Itemss = null;
            }
            else
            {

                ViewBag.Itemss = new SelectList(_context.Categories, "Id", "CategoryName");
            }

            return View();
        }
        [HttpPost]

        public IActionResult ProductCreate(ProductViewModel productview)
        {
            if(productview.ProductSize == null)
            {
                ModelState.AddModelError(string.Empty, "Please select an size.");
                return View(productview);
            }
            if (productview != null)
            {
                Product product = new Product();
                product.ProductDescription = productview.ProductDescription;
                product.ProductName = productview.ProductName;
                product.CategoryId = productview.CategoryId;
                product.ProductPrice = productview.ProductPrice;
                product.ProductQuantity = productview.available;
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(productview.ProductFile.FileName);
                string extension = Path.GetExtension(productview.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                product.ProductPicture = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "ProductImage", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    productview.ProductFile.CopyTo(fileStream);
                }
                _context.Products.Add(product);
                _context.SaveChanges();
                for (int i = 0; i < productview.ProductSize.Count(); i++)
                {
                    ProductSize productSize = new ProductSize();
                    productSize.Name = productview.ProductSize[i].ToString();
                    productSize.ProductId = product.Id;
                    _context.ProductSizes.Add(productSize);
                    _context.SaveChanges();

                }
                return RedirectToAction("ProductIndex");
            }
            else
            {
                return View(productview);
            }

        }
        public IActionResult ProductEdit(int? id)
        {
            ViewBag.Itemss = new SelectList(_context.Categories, "Id", "CategoryName");
            var getProduct = _context.Products.Find(id);

            if (getProduct != null)
            {
                ViewBag.Itemss = new SelectList(_context.Categories, "Id", "CategoryName");

                ProductViewModel product = new ProductViewModel
                {
                    CategoryId = getProduct.CategoryId,
                    ProductPicture = getProduct.ProductPicture,
                    Id = getProduct.Id,
                    ProductPrice = getProduct.ProductPrice,
                    Category = getProduct.Category,
                    ProductDescription = getProduct.ProductDescription,
                    ProductName = getProduct.ProductName,
                    available=getProduct.ProductQuantity,
                    PurchasePrice= getProduct.PurchasePrice,

                };
                return View(product);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult ProductEdit(ProductViewModel productview)
        {
            if (productview.ProductFile != null)
            {

                Product product = new Product
                {
                    ProductName = productview.ProductName,
                    CategoryId = productview.CategoryId,
                    Id = productview.Id,
                    ProductDescription = productview.ProductDescription,
                    ProductPrice = productview.ProductPrice,
                    ProductPicture = productview.ProductPicture,
                    ProductQuantity=productview.available,
                };

                var getProductSize = _context.ProductSizes.Where(x => x.ProductId == productview.Id).ToList();


                for (int i = 0; i < getProductSize.Count(); i++)
                {
                    var deletSize = getProductSize[i];
                    _context.ProductSizes.Remove(deletSize);
                    _context.SaveChanges();
                }

                for (int i = 0; i < productview.ProductSize.Count(); i++)
                {
                    ProductSize productSize = new ProductSize();
                    productSize.Name = productview.ProductSize[i].ToString();
                    productSize.ProductId = product.Id;
                    _context.ProductSizes.Add(productSize);
                    _context.SaveChanges();

                }

                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(productview.ProductFile.FileName);
                string extension = Path.GetExtension(productview.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                product.ProductPicture = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "ProductImage", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    productview.ProductFile.CopyTo(fileStream);
                }

                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("ProductIndex");
            }
            else
            {

                Product product = new Product
                {
                    ProductName = productview.ProductName,
                    CategoryId = productview.CategoryId,
                    Id = productview.Id,
                    ProductDescription = productview.ProductDescription,
                    ProductPrice = productview.ProductPrice,
                    ProductPicture = productview.ProductPicture,
                    ProductQuantity = productview.available,

                };
                _context.Products.Update(product);
                _context.SaveChanges();

                var getProductSize = _context.ProductSizes.Where(x => x.ProductId == productview.Id).ToList();


                for (int i = 0; i < getProductSize.Count(); i++)
                {
                    var deletSize = getProductSize[i];
                    _context.ProductSizes.Remove(deletSize);
                    _context.SaveChanges();
                }

                for (int i = 0; i < productview.ProductSize.Count(); i++)
                {
                    ProductSize productSize = new ProductSize();
                    productSize.Name = productview.ProductSize[i].ToString();
                    productSize.ProductId = product.Id;
                    _context.ProductSizes.Add(productSize);
                    _context.SaveChanges();

                }
                return RedirectToAction("ProductIndex");
            }

        }

        public IActionResult ProductDelete(int? id)
        {
            var getProduct = _context.Products.Find(id);

            var ProductDetail = _context.ProductSizes.Where(x => x.ProductId == id).ToList();
            if (getProduct == null)
            {

                return NotFound();
            }

            for (int i = 0; i < ProductDetail.Count(); i++)
            {
                var delete = ProductDetail[i];
                _context.ProductSizes.Remove(delete);
                _context.SaveChanges();
            }

            _context.Products.Remove(getProduct);
            int check = _context.SaveChanges();
            if (check == 1)
            {
                return new JsonResult("Product is Delete successfully");
            }

            else
            {
                return new JsonResult("Error! Product is not Delete");
            }
        }

        //working on customer support
        public IActionResult ContactUs()
        {
            var getContact = _context.Contacts.Where(x => x.MessagesStatus == "UnRead").ToList();
            return View(getContact);
        }

        public IActionResult ContactUsMessagesDetail(int? id)
        {
            ViewBag.Message = (from msg in _context.Contacts where msg.Id == id select msg.CustomerMessage).FirstOrDefault();

            return PartialView("_ViewMessage");
        }
        public IActionResult ReadMessages(int? id)
        {
            var ChnageMessagesStatus = _context.Contacts.Find(id);

            if (ChnageMessagesStatus != null)
            {
                ChnageMessagesStatus.MessagesStatus = "Read";
                _context.Contacts.Update(ChnageMessagesStatus);
                int check = _context.SaveChanges();
                if (check == 1)
                {
                    return new JsonResult("Messages Messages is Updated successfully");
                }

                else
                {
                    return new JsonResult("Error! Contact Message is not Updated");
                }

            }
            return new JsonResult("Error! Contact Message is not Updated");
        }
        public IActionResult ReadContactMessage()
        {

            var getContact = _context.Contacts.Where(x => x.MessagesStatus == "Read").ToList();
            return View(getContact);
        }

        public IActionResult AllContactMessageCheck(int? id)
        {
            ViewBag.Message = (from msg in _context.Contacts where msg.Id == id select msg.CustomerMessage).FirstOrDefault();

            return PartialView("_ViewReadMessage");
        }
        public IActionResult UserList()
        {
            var obj = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       where roles.Name == "User"
                       select new UserViewModel
                       {
                           FirstName=user.FirstName,
                           LastName=user.LastName,
                           Email=user.Email,
                           MobileNumber=user.PhoneNumber,                         
                           UserRole=roles.Name,
                           userId=user.Id
                       }).ToList();
            return View(obj);
        }
        public IActionResult GuestUserList()
        {
            var obj = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       where roles.Name == "GuestUser"
                       select new UserViewModel
                       {
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           Email = user.Email,
                           MobileNumber = user.PhoneNumber,
                           UserRole = roles.Name,
                       }).ToList();
            return View(obj);
        }
        [HttpPost]
        public IActionResult DeleteUser(string id)
        {
            var getData = _context.Users.Find(id);
            if (getData != null)
            {
                _context.Users.Remove(getData);
                int check = _context.SaveChanges();
                if (check == 1)
                {
                    return new JsonResult("User is Delete successfully");
                }

                else
                {
                    return new JsonResult("Error! User is not Delete");
                }
            }
            return new JsonResult("Error! User is not Delete");
        }
        public IActionResult PendingOrderList()
        {

            var getOrder = (from order in _context.Orders
                            join od in _context.OrderDetails on order.Id equals od.OrderId
                            where od.OrderStatus == "Pending"
                            group order by new {order.OrderType, order.OrderDate, order.OrderNumber, order.Id, order.FirstName, order.LastName, order.Email, order.Mobile, order.Country, order.Address, order.City } into groupedOrders
                            where groupedOrders.Any()
                            select new OrderViewModel
                            {
                                OrderNumber=groupedOrders.Key.OrderNumber.ToString(),
                                OrderDate=groupedOrders.Key.OrderDate.ToString(),
                                FirstName = groupedOrders.Key.FirstName,
                                LastName = groupedOrders.Key.LastName,
                                CustomerEmail = groupedOrders.Key.Email,
                                MobileNumber = groupedOrders.Key.Mobile,
                                Country = groupedOrders.Key.Country,
                                Address = groupedOrders.Key.Address,
                                City = groupedOrders.Key.City,
                                OrderCount = groupedOrders.Count(),
                                PaymentMethod=groupedOrders.Key.OrderType,
                                orderid=groupedOrders.Key.Id,
                                OrderStatus="Pending"
                            }).ToList();

            return View(getOrder);
        }
        public IActionResult PatchOrderList()
        {

            var getOrder = (from order in _context.Orders
                            join od in _context.OrderDetails on order.Id equals od.OrderId
                            where od.OrderStatus == "Patch"
                            group order by new { order.OrderType, order.OrderDate, order.OrderNumber, order.Id, order.FirstName, order.LastName, order.Email, order.Mobile, order.Country, order.Address, order.City } into groupedOrders
                            where groupedOrders.Any()
                            select new OrderViewModel
                            {
                                OrderNumber = groupedOrders.Key.OrderNumber.ToString(),
                                OrderDate = groupedOrders.Key.OrderDate.ToString(),
                                FirstName = groupedOrders.Key.FirstName,
                                LastName = groupedOrders.Key.LastName,
                                CustomerEmail = groupedOrders.Key.Email,
                                MobileNumber = groupedOrders.Key.Mobile,
                                Country = groupedOrders.Key.Country,
                                Address = groupedOrders.Key.Address,
                                City = groupedOrders.Key.City,
                                OrderCount = groupedOrders.Count(),
                                PaymentMethod = groupedOrders.Key.OrderType,
                                orderid = groupedOrders.Key.Id,
                                OrderStatus = "Patch"
                            }).ToList();

            return View(getOrder);
        }
        public IActionResult DeliverOrderList()
        {

            var getOrder = (from order in _context.Orders
                            join od in _context.OrderDetails on order.Id equals od.OrderId
                            where od.OrderStatus == "Deliver"
                            group order by new { order.OrderType, order.OrderDate, order.OrderNumber, order.Id, order.FirstName, order.LastName, order.Email, order.Mobile, order.Country, order.Address, order.City } into groupedOrders
                            where groupedOrders.Any()
                            select new OrderViewModel
                            {
                                OrderNumber = groupedOrders.Key.OrderNumber.ToString(),
                                OrderDate = groupedOrders.Key.OrderDate.ToString(),
                                FirstName = groupedOrders.Key.FirstName,
                                LastName = groupedOrders.Key.LastName,
                                CustomerEmail = groupedOrders.Key.Email,
                                MobileNumber = groupedOrders.Key.Mobile,
                                Country = groupedOrders.Key.Country,
                                Address = groupedOrders.Key.Address,
                                City = groupedOrders.Key.City,
                                OrderCount = groupedOrders.Count(),
                                PaymentMethod = groupedOrders.Key.OrderType,
                                orderid = groupedOrders.Key.Id,
                                OrderStatus = "Deliver"
                            }).ToList();

            return View(getOrder);
        }
        public IActionResult CancelOrderList()
        {         
            var getOrder = (from order in _context.Orders
                            join od in _context.OrderDetails on order.Id equals od.OrderId
                            where od.OrderStatus == "cancelled"
                            select new OrderViewModel
                            {
                                FirstName = order.FirstName,
                                LastName = order.LastName,
                                OrderDate = order.OrderDate.ToString(),
                                CustomerEmail = order.Email,
                                MobileNumber = order.Mobile,
                                Country = order.Country,
                                Address = order.Address,
                                City = order.City,
                                PaymentMethod = order.OrderType,
                                orderid = order.Id,
                                OrderStatus = od.OrderStatus
                            }).ToList();
            return View(getOrder);
        }

        public IActionResult CheckOrder(int? id,string status)
        {
            var getOrderDetail = (from order in _context.Orders
                                  join od in _context.OrderDetails on
                                  order.Id equals od.OrderId
                                  join pro in _context.Products on od.productId equals pro.Id
                                  where order.Id == id && od.OrderStatus == status
                                  select new OrderDetailViewModel
                                  {
                                      FirstName = order.FirstName,
                                      LastName = order.LastName,
                                      Address = order.Address,
                                      City = order.City,
                                      Country = order.Country,
                                      Email = order.Email,
                                      Mobile = order.Mobile,
                                      OrderDate = order.OrderDate.ToString(),
                                      OrderStatus = od.OrderStatus,
                                      PostalCode = order.PostalCode,
                                      ProductName = pro.ProductName,
                                      ProductPicture = pro.ProductPicture,
                                      ProductPrice = pro.ProductPrice,
                                      ProductSize = od.ProductSize,
                                      Quantity = od.Quantity,
                                      OrderNote=order.Address,
                                  }).ToList();

            return View(getOrderDetail);
        }
        public IActionResult CheckPayment(int?id)
        {
            var obj = _context.paymentDetails.Where(x => x.OrderId == id).FirstOrDefault();
            return PartialView("_ViewPayment",obj);
        }
        public IActionResult ClearPendingOrder(int? id)
        {
            var obj = _context.OrderDetails.Where(x => x.OrderId == id).ToList();
            if (obj != null)
            {              
                foreach (var orderDetail in obj)
                {
                    if (orderDetail.OrderStatus == "Pending")
                    {                       
                        orderDetail.OrderStatus = "Patch";
                        _context.OrderDetails.Update(orderDetail);
                    }
                }
                    _context.SaveChanges();
                
                    return new JsonResult("Order is Patch successfully");
            }
            return new JsonResult("Error! System is not working");
        }

        public IActionResult ClearPacthOrder(int? id)
        {
            var obj = _context.OrderDetails.Where(x => x.OrderId == id).ToList();
            if (obj != null)
            {
                foreach (var orderDetail in obj)
                {
                    if (orderDetail.OrderStatus == "Patch")
                    {
                        orderDetail.OrderStatus = "Deliver";
                        _context.OrderDetails.Update(orderDetail);
                    }
                }
                _context.SaveChanges();

                return new JsonResult("Order is Patch successfully");
            }
            return new JsonResult("Error! System is not working");
        }
        public IActionResult ProfitLossReport()
        {
            var getOrder = (from order in _context.Orders
                            join od in _context.OrderDetails on order.Id equals od.OrderId
                            join pro in _context.Products on od.productId equals pro.Id
                            where od.OrderStatus != "cancelled"
                            group new { order, od,pro } by new { order.OrderType, order.OrderDate, order.OrderNumber, order.Id, order.FirstName, order.LastName, order.Email, order.Mobile, order.Country, order.Address, order.City } into groupedOrders
                            where groupedOrders.Any()
                            select new OrderViewModel
                            {
                                OrderNumber = groupedOrders.Key.OrderNumber.ToString(),
                                OrderDate = groupedOrders.Key.OrderDate.ToString(),
                                FirstName = groupedOrders.Key.FirstName,
                                LastName = groupedOrders.Key.LastName,
                                CustomerEmail = groupedOrders.Key.Email,
                                MobileNumber = groupedOrders.Key.Mobile,
                                Country = groupedOrders.Key.Country,
                                Address = groupedOrders.Key.Address,
                                City = groupedOrders.Key.City,
                                OrderCount = groupedOrders.Count(),
                                PaymentMethod = groupedOrders.Key.OrderType,
                                orderid = groupedOrders.Key.Id,
                                ProductSale = groupedOrders.Sum(g => g.od.SalePrice * g.od.Quantity),
                                PurchasePrice=groupedOrders.Sum(x=>x.pro.PurchasePrice * x.od.Quantity),
                                
                            }).ToList();
            return View(getOrder);
        }

        public IActionResult DailySale()
        {
            var dailySales = (from order in _context.Orders
                              join od in _context.OrderDetails on order.Id equals od.OrderId
                              where od.OrderStatus != "cancelled"
                              group new { order, od } by order.OrderDate.Date into groupedOrders
                              select new
                              {
                                  OrderDate = groupedOrders.Key,
                                  TotalSale = groupedOrders.Sum(g => g.od.Quantity * g.od.SalePrice)
                              }).ToList();

            var chartData = new
            {
                labels = dailySales.Select(ds => ds.OrderDate.ToString("dddd")),
                datasets = new List<object>
            {
            new
            {
                label = "Daily Sales",
                data = dailySales.Select(ds => ds.TotalSale),
                fill = false,
                borderColor = "rgb(75, 192, 192)",
                tension = 0.1
            }
            }
            };
            return Ok(chartData);
        }

        public IActionResult testimonial()
        {
            var obj = _context.testimonials.Where(x => x.Status == "Unread").ToList();
            return View(obj);
        }
        public IActionResult testimonialread()
        {
            var obj = _context.testimonials.Where(x => x.Status == "read").ToList();
            return View(obj);
        }
        public IActionResult ChangeStatus(int?id)
        {
            var obj = _context.testimonials.Find(id);
            if(obj!=null)
            {
                obj.Status = "Read";
                _context.testimonials.Update(obj);
                _context.SaveChanges();
                return new JsonResult("Status change successfully");

            }
            return new JsonResult("Error! Status is not change");

        }

    }
}
