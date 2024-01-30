using He_SheStore.Areas.Identity.Data;
using He_SheStore.DTO;
using He_SheStore.EmailSender;
using He_SheStore.Models;
using He_SheStore.SessionHelper;
using He_SheStore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;

namespace He_SheStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IMailSender _sender;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context,IMailSender sender,UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _sender = sender;
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();



            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;

            var obj1 = _context.Products
                  .OrderByDescending(p => p.Id)
                  .Take(4)
                  .ToList();
            var obj2 = _context.Products.Take(4).ToList();

            var totalProducts = _context.Products?.Count();
            var middleStartIndex = Math.Max((totalProducts.Value - 4) / 2, 0); // Ensure it's non-negative
            var middleFourProducts = _context.Products
                .Skip(middleStartIndex)
                .Take(4)
                .ToList();

            var tes = _context.testimonials.Where(x => x.Status == "Read").ToList();
            ProductListViewModel productListView = new ProductListViewModel
            {
                FirstProduct = obj1,
                SecondProduct=obj2,
                LastProduct=middleFourProducts,
                testimonial=tes
               
            };
            return View(productListView);
        }
        public IActionResult Shop(int?id , string? productname)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;

            if (productname!=null)
            {
                var objProduct = _context.Products.Where(x=>x.ProductName==productname).ToList();
                var objCategory = _context.Categories.ToList();
                ProductShopViewModel productShopView = new ProductShopViewModel
                {
                    Product = objProduct,
                    Category = objCategory
                };
                return View(productShopView);

            }
            if(id!=null)
            {
                var objCategoryProduct = (from pro in _context.Products 
                                          join cat in _context.Categories 
                                          on pro.CategoryId 
                                          equals cat.Id where cat.Id==id
                                          select pro).ToList();
                var objcat=_context.Categories.ToList();
                ProductShopViewModel productShopView = new ProductShopViewModel
                {
                    Product = objCategoryProduct,
                    Category = objcat
                };
                return View(productShopView);
            }
            else
            {
                var objProduct = _context.Products.ToList();
                var objCategory = _context.Categories.ToList();
                ProductShopViewModel productShopView = new ProductShopViewModel
                {
                    Product = objProduct,
                    Category = objCategory
                };
                return View(productShopView);
            }
           
        }
        [Authorize(Roles = "User")]
        public IActionResult Cart()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            if (cart == null)
            {
                cart = new List<ProductItem>();
            }

            return View(cart);
        }
        [Authorize(Roles ="User")]
        public IActionResult ProductDetail(int? id)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;

            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;

            if (id != null)
            {
                ViewBag.size = _context.ProductSizes.Where(x => x.ProductId == id).ToList();
                ProductShopViewModel viewModel = new ProductShopViewModel
                {
                    DetailProduct = _context.Products.Include(x => x.Category).FirstOrDefault(x => x.Id == id),
                    ProductSizes = _context.ProductSizes.Where(x => x.ProductId == id).ToList(),
                    ProductReviews = _context.ProductReviews.Where(x => x.ProductId == id).ToList(),
                };

                return View(viewModel);
            }

            return RedirectToAction("Shop");
        }

        [HttpPost]
        public IActionResult AddReview(int productId, int rating, string comment)
        {
            // Get the user ID of the current user (you may need to modify this based on your authentication setup)
            string userId = _userManager.GetUserId(User);

            // Create a new ProductReview instance
            var newReview = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };

            // Save the review to the database
            _context.ProductReviews.Add(newReview);
            _context.SaveChanges();

            // Redirect back to the product detail page
            return RedirectToAction("ProductDetail", new { id = productId });
        }



        public IActionResult Buy(int id, string sizeId)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var book = _context.Products.Find(id);

            List<ProductItem> cart = new List<ProductItem>();

            cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            if (cart == null)
            {
                cart = new List<ProductItem>();
                cart.Add(new ProductItem { Product = book, Quantity = 1, size = sizeId });
            }
            else
            {
                int index = cart.FindIndex(k => k.Product.Id == id && k.size == sizeId);

                if (index != -1) //if item already in the cart
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new ProductItem { Product = book, Quantity = 1, size = sizeId });
                }
            }
            for (int i = 0; i < cart.Count; i++)
            {
                var b = _context.Products.Single(x => x.Id == cart[i].Product.Id);
            }
            HttpContext.Session.Set<List<ProductItem>>("cart", cart);
            return RedirectToAction("Cart");
        }
        public IActionResult Add(int id, string sizeId)
        {
          
            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int index = cart.FindIndex(w => w.Product.Id == id && w.size == sizeId);
            cart[index].Quantity++;
            HttpContext.Session.Set<List<ProductItem>>("cart", cart);
            return RedirectToAction("Cart");
        }
        public IActionResult minus(int id, string sizeId)
        {
            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            var index = cart.FindIndex(m => m.Product.Id == id && m.size == sizeId);

            if (cart[index].Quantity == 1)
            {
                cart.RemoveAt(index);
            }
            else
            {
                cart[index].Quantity--;
            }

            HttpContext.Session.Set<List<ProductItem>>("cart", cart);
            return RedirectToAction("Cart");
        }
        public IActionResult Remove(int id)
        {
            _context.Products.Find(id);
            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            var index = cart.FindIndex(m => m.Product.Id == id);
            cart.RemoveAt(index);
            HttpContext.Session.Set<List<ProductItem>>("cart", cart);
            return RedirectToAction("Cart");
        }
        [Authorize(Roles = "User")]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            decimal total = cart?.Sum(s => s.Quantity * s.Product.ProductPrice) ?? 0;

            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            ViewBag.Total = total;
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();


            return View();
        }
        [HttpPost]
        public IActionResult CheckOut(OrderDTO order)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();
            if (order.OrderType== "Cash")
            {
                if(order.FirstName==null)
                {
                    return Json(new { success = false, errors = "First Name is required" });

                }
                else if(order.LastName== null)
                {
                    return Json(new { success = false, errors = "Last Name is required" });
                }
                else if (order.Mobile == null)
                {
                    return Json(new { success = false, errors = "Mobile number is required" });
                }
                else if (order.City == null)
                {
                    return Json(new { success = false, errors = "City is required" });
                }
                else if (order.Country == null)
                {
                    return Json(new { success = false, errors = "Country is required" });
                }
                else if (order.Address == null)
                {
                    return Json(new { success = false, errors = "Adress is required" });
                }
                else if (order.PostalCode == null)
                {
                    return Json(new { success = false, errors = "Zip Code is required" });
                }
                else
                {
                    var carts = HttpContext.Session.Get<List<ProductItem>>("cart");
                    decimal total = carts?.Sum(s => s.Quantity * s.Product.ProductPrice) ?? 0;

                    var obj = _context.accountBalances.Where(x => x.UserID == userId).FirstOrDefault();

                    decimal RemainingAmount = Convert.ToDecimal(obj.Amount) - total;

                    obj.Amount = RemainingAmount.ToString();
                    _context.accountBalances.Update(obj);
                    _context.SaveChanges();



                    int ordernumber = GenerateRandomNumber();
                    var UserId = _context.Users.Where(x => x.Email == order.Email).Select(x => x.Id).FirstOrDefault();
                    Order order1 = new Order
                    {
                        OrderDate = System.DateTime.Now,
                        OrderNumber = ordernumber,
                        OrderType = "Cash",
                        Address = order.Address,
                        FirstName = order.FirstName,
                        LastName = order.LastName,
                        City = order.City,
                        PostalCode = order.PostalCode,
                        Country = order.Country,
                        Mobile = order.Mobile,
                        Email = order.Email,
                        userId=UserId,
                    };
                    _context.Orders.Add(order1);
                    _context.SaveChanges();

                    //for orderdeatil
                    var cart = HttpContext.Session.Get<List<ProductItem>>("cart");

                    for (int i = 0; i < cart.Count; i++)
                    {
                        var od = new OrderDetail
                        {
                            Quantity = cart[i].Quantity,
                            SalePrice = cart[i].Product.ProductPrice,
                            OrderId = order1.Id,
                            ProductSize = cart[i].size,
                            productId = cart[i].Product.Id,
                            OrderStatus = "Pending",
                        };
                        _context.OrderDetails.Add(od);
                        _context.SaveChanges();
                        var product = _context.Products.Find(cart[i].Product.Id);
                        if (product != null)
                        {
                            product.ProductQuantity -= cart[i].Quantity;
                            _context.SaveChanges(); 
                        }
                    }
                    HttpContext.Session.Remove("cart");
                    string trackingUrl = $"{Request.Scheme}://{Request.Host}/Home/OrderTracking";

                    var message = new Message(order.Email, "He&She Store",
                       $" Hello Dear {order.FirstName}! <br /><br/> We are thrilled to inform you that your order has been successfully received! We would like to express our gratitude for choosing He&She Store for your purchase. We are committed to providing you with top-notch service and delivering a delightful shopping experience.<br/></br/> Our team is now diligently working on processing your order and ensuring that it is prepared with the utmost care. We will make sure that your items are packaged securely and dispatched as soon as possible. <br/><br/> Thank you once again for your trust in us. We look forward to serving you and exceeding your expectations. <br/>You can track the delivery status using this <b> {ordernumber} </b> tracking number after one business day.<br/> This link for tracking order <a href='{HtmlEncoder.Default.Encode(trackingUrl)}'>clicking here</a>.  <br/><br/> He&She Store", "");
                    _sender.MessageSend(message);
                    return Json(new { success = true, errors = "Your Order is place Please check your Email" });

                }

            }
            else
            {
                if(order.CardHolderName==null)
                {
                    return Json(new { success = false, errors = "Card holder Name is required" });
                }
                else if(order.CardCvvCode==null)
                {
                    return Json(new { success = false, errors = "Card CVV Code is required" });
                }
                else if (order.ExpiryDate == null)
                {
                    return Json(new { success = false, errors = "Card Expiry Date is required" });
                }
                else if(order.CardHolderNumber == null)
                {
                    return Json(new { success = false, errors = "Card  Number is required" });
                }
                else
                {
                    int ordernumber = GenerateRandomNumber();
                    var UserId = _context.Users.Where(x => x.Email == order.Email).Select(x => x.Id).FirstOrDefault();
                    Order order1 = new Order
                    {
                        OrderDate = System.DateTime.Now,
                        OrderNumber = ordernumber,
                        OrderType = "Credit",
                        Address = order.Address,
                        FirstName = order.FirstName,
                        LastName = order.LastName,
                        City = order.City,
                        PostalCode = order.PostalCode,
                        Country = order.Country,
                        Mobile = order.Mobile,
                        Email = order.Email,
                        userId=UserId,
                    };
                    var carts = HttpContext.Session.Get<List<ProductItem>>("cart");
                    decimal total = carts?.Sum(s => s.Quantity * s.Product.ProductPrice) ?? 0;
                    var obj = _context.accountBalances.Where(x => x.UserID == userId).FirstOrDefault();
                    decimal RemainingAmount = Convert.ToDecimal(obj.Amount) - total;
                    obj.Amount = RemainingAmount.ToString();
                    _context.Orders.Add(order1);
                    _context.SaveChanges();

                    //for orderdeatil
                    var cart = HttpContext.Session.Get<List<ProductItem>>("cart");

                    for (int i = 0; i < cart.Count; i++)
                    {
                        var od = new OrderDetail
                        {
                            Quantity = cart[i].Quantity,
                            SalePrice = cart[i].Product.ProductPrice,
                            OrderId = order1.Id,
                            ProductSize = cart[i].size,
                            productId = cart[i].Product.Id,
                            OrderStatus = "Pending",
                        };
                        _context.OrderDetails.Add(od);
                        _context.SaveChanges();

                        var product = _context.Products.Find(cart[i].Product.Id);
                        if (product != null)
                        {
                            product.ProductQuantity -= cart[i].Quantity;
                            _context.SaveChanges();
                        }
                    }

                    //for save in payment method
                    PaymentDetail paymentDetail = new PaymentDetail
                    {
                        cardNumber = order.CardHolderNumber.ToString(),
                        CVV = order.CardCvvCode,
                        ExpiryDate = order.ExpiryDate,
                        CardName=order.CardHolderName,
                        OrderId = order1.Id,
                    };
                    _context.paymentDetails.Add(paymentDetail);
                    _context.SaveChanges();
                    HttpContext.Session.Remove("cart");
                    string trackingUrl = $"{Request.Scheme}://{Request.Host}/Home/OrderTracking";
                    var message = new Message(order.Email, "He&She Store",
                       $" Hello Dear {order.FirstName}! <br /><br/> We are thrilled to inform you that your order has been successfully received! We would like to express our gratitude for choosing He&She Store for your purchase. We are committed to providing you with top-notch service and delivering a delightful shopping experience.<br/></br/> Our team is now diligently working on processing your order and ensuring that it is prepared with the utmost care. We will make sure that your items are packaged securely and dispatched as soon as possible. <br/><br/> Thank you once again for your trust in us. We look forward to serving you and exceeding your expectations. <br/>You can track the delivery status using this <b> {ordernumber} </b> tracking number after one business day.<br/> This link for tracking order <a href='{HtmlEncoder.Default.Encode(trackingUrl)}'>clicking here</a>.  <br/><br/> He&She Store", "");
                    _sender.MessageSend(message);
                    return Json(new { success = true, errors = "Your Order is place Please check your Email" });
                }
            }           
        }
        public async Task<IActionResult> Logout()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult About()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            return View();
        }
        public IActionResult Contactus()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            //for total amount show on the checkout
            decimal total = cart?.Sum(s => s.Quantity * s.Product.ProductPrice) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            ViewBag.Total = total;

            return View();
        }
        [HttpPost]
        public IActionResult ContactUs(Contactus contactus)
        {

            contactus.MessageDate = System.DateTime.Now.ToString();
            contactus.MessagesStatus = "UnRead";
            _context.Contacts.Add(contactus);
            int check = _context.SaveChanges();

            var message = new Message(contactus.CustomerEmail, "He&he Store",
               $" Hello Dear Customer! <br /> Certainly! The message you received from our Customer Support team acknowledges that your inquiry has been received and that they will respond to you within 2 working days. This timeframe allows our team to thoroughly review and address your query in a timely manner. <br />During this time, our team will work diligently to provide you with a comprehensive and accurate response. We strive to deliver excellent customer service and ensure that your concerns or questions are addressed in a timely manner. <br/> We appreciate your patience and understanding as we work to assist you with your web application inquiry.<br/> <br/> Thank you for your continued support of He&She Store!  ", "");
            _sender.MessageSend(message);
            if (check == 1)
            {
                return new JsonResult("Your Message is send to Company!!");
            }
            else
            {
                return new JsonResult(" Error ! System Error Please try again later!!");
            }
        }
        public IActionResult ThankYou()
        {
            return View();
        }
        public IActionResult UserDashborad()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            return View();
        }
        [Authorize(Roles = "User")]
        public IActionResult Profile()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var obj = (from user in _context.Users
                       where user.Id == userId
                       select new UserViewModel
                       {
                            userId = user.Id,
                            FirstName=user.FirstName,
                            LastName=user.LastName,
                            Email=user.Email,
                            MobileNumber=user.PhoneNumber,
                       }).FirstOrDefault();
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModel model)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(model.userId);       
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            await _signInManager.RefreshSignInAsync(user);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.MobileNumber;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("UserDashborad");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "not Update the user information");
                return View(model);
            }
        }
        [Authorize(Roles = "User")]
        public IActionResult OrderHistory()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;

            string userid = _userManager.GetUserId(User);
            var obj = (from od in _context.OrderDetails
                       join order in _context.Orders on od.OrderId equals order.Id
                       join pro in _context.Products on od.productId equals pro.Id
                       join cat in _context.Categories on pro.CategoryId equals cat.Id
                       where order.userId == userid && od.OrderStatus != "cancelled"
                       select new OrderViewModel
                       {
                           Address = order.Address,
                           CategoryName = cat.CategoryName,
                           FirstName = order.FirstName,
                           LastName = order.LastName,
                           MobileNumber = order.Mobile,
                           orderid = order.Id,
                           productName = pro.ProductName,
                           productPicture = pro.ProductPicture,
                           ProductSize = od.ProductSize,
                           productQuantity = od.Quantity,
                           OrderNumber = order.OrderNumber.ToString(),
                           OrderDate = order.OrderDate.ToString(),
                           Total = (od.Quantity * od.SalePrice + 10),
                           ProductPrice = od.SalePrice.ToString(),
                           OrderStatus=od.OrderStatus,
                       }).ToList();
            return View(obj);
        }
        [Authorize(Roles = "User")]
        public IActionResult OrderTracking()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            string userid = _userManager.GetUserId(User);
            return View();
        }
        [HttpPost]
        public IActionResult OrderTrackingView(string OrderNumber)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            var cart = HttpContext.Session.Get<List<ProductItem>>("cart");
            int cartCount = cart?.Sum(s => s.Quantity) ?? 0;
            ViewBag.CartCount = cartCount != 0 ? cartCount : (int?)null;
            var obj = (from od in _context.OrderDetails
                       join order in _context.Orders on od.OrderId equals order.Id
                       join pro in _context.Products on od.productId equals pro.Id
                       join cat in _context.Categories on pro.CategoryId equals cat.Id
                       where order.OrderNumber ==Convert.ToInt32(OrderNumber) && od.OrderStatus!= "cancelled"
                       select new OrderViewModel
                       {
                           Address = order.Address,
                           CategoryName = cat.CategoryName,
                           FirstName = order.FirstName,
                           LastName = order.LastName,
                           MobileNumber = order.Mobile,
                           orderid = order.Id,
                           productName = pro.ProductName,
                           productPicture = pro.ProductPicture,
                           ProductSize = od.ProductSize,
                           productQuantity = od.Quantity,
                           OrderNumber = order.OrderNumber.ToString(),
                           OrderDate = order.OrderDate.ToString(),
                           Total = (od.Quantity * od.SalePrice + 10),
                           ProductPrice = od.SalePrice.ToString(),
                           OrderStatus=od.OrderStatus,
                       }).ToList();
            ViewBag.OrderStatus = obj.FirstOrDefault()?.OrderStatus;
            return PartialView("_OrderTracking",obj);
        }
        public IActionResult UpdateStatus(int? id, string size)
        {

            var obj = _context.OrderDetails.Where(x => x.OrderId == id && x.ProductSize == size).FirstOrDefault();
            if (obj != null)
            {
                obj.OrderStatus = "cancelled";
                _context.OrderDetails.Update(obj);
                _context.SaveChanges();
                return RedirectToAction("OrderHistory");
            }
            return View();
        }
        private int GenerateRandomNumber()
        {
            Random random = new Random();
            int randomNumber = random.Next(100000000, 999999999);
            return randomNumber;
        }

        public IActionResult AccountIndex()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            string userid = _userManager.GetUserId(User);
            var obj = _context.accountBalances.Where(x=>x.UserID==userid).ToList();
            return View(obj);
        }

        public IActionResult AddBalanceAmount()
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            return View();
        }
        [HttpPost]

        public IActionResult AddBalanceAmount(AccountBalance accountBalance)
        {
            if(accountBalance!=null)
            {
                string userid = _userManager.GetUserId(User);
                accountBalance.UserID = userid;

                _context.accountBalances.Add(accountBalance);
                _context.SaveChanges();
                return RedirectToAction("AccountIndex");

            }
            return View();
        }

        public IActionResult UpdateAmount(int? id)
        {
            string userId = _userManager.GetUserId(User);
            ViewBag.AccountBalance = _context.accountBalances.Where(x => x.UserID == userId).Select(x => x.Amount).FirstOrDefault();

            if (id != null)
            {
                var obj = _context.accountBalances.Find(id);
                return View(obj);

            }
            return View();
        }
        [HttpPost]
        public IActionResult UpdateAmount(AccountBalance accountBalance)
        {
            if (accountBalance != null)
            {
                _context.accountBalances.Update(accountBalance);
                _context.SaveChanges();
                return RedirectToAction("AccountIndex");

            }
            return View();
        }


        public IActionResult testimonialwrite()
        {
            return View();
        }

        [HttpPost]
        public IActionResult testimonialwrite(testimonial testimonial)
        {
            if(testimonial != null)
            {
                testimonial.Status = "Unread";
                _context.testimonials.Add(testimonial);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}