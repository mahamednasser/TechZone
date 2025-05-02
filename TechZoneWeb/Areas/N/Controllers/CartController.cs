using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.Models.Models;
using TechZone.Models.ViewModels;
using TechZone.Utility;

namespace TechZoneWeb.Areas.N.Controllers
{
    [Area("N")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        private shoppingCartViewModel shoppingCartViewModel { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           ClaimsIdentity claimsIdentity=(ClaimsIdentity)User.Identity;
            string userid=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userid != null && claimsIdentity != null)
            {
                shoppingCartViewModel = new()
                {
                    ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userid,IncludeProp:"product"),
                    orderHeader = new OrderHeader()
                };
                foreach (var item in shoppingCartViewModel.ShoppingCarts) 
                {
                    
                    if (item.product.DiscounPrice == null)
                    {
                        item.price = item.product.Price;
                    }
                    else
                    {
                        item.price = (double)item.product.DiscounPrice;
                    }
                    shoppingCartViewModel.orderHeader.OrderTotal += (double)(item.price * item.count);
                    
                
                }

            }
                return View(shoppingCartViewModel);
        }

        public IActionResult Plus(int cartId)
        {
            
                ShoppingCart cartFrmDb = _unitOfWork.ShoppingCart.GetById(x => x.Id == cartId);
                cartFrmDb.count += 1;
                _unitOfWork.ShoppingCart.Update(cartFrmDb);
                _unitOfWork.save();
            return RedirectToAction("index");

        }
        public IActionResult Minus(int cartId)
        {

            ShoppingCart cartFrmDb = _unitOfWork.ShoppingCart.GetById(x => x.Id == cartId,tracked:true);
            if (cartFrmDb.count <= 1)
            { 
                _unitOfWork.ShoppingCart.Remove(cartFrmDb);
                HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cartFrmDb.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(cartFrmDb);

            }
            else
            {
                cartFrmDb.count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFrmDb);

            }
            _unitOfWork.save();
            return RedirectToAction("index");

        }

        public IActionResult Remove(int cartId)
        {

            ShoppingCart cartFrmDb = _unitOfWork.ShoppingCart.GetById(x => x.Id == cartId,tracked:true);

            HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cartFrmDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(cartFrmDb);

            _unitOfWork.save();
            return RedirectToAction("index");

        }

        public IActionResult Summary() 
        {

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            string userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userid != null && claimsIdentity != null)
            {
                shoppingCartViewModel = new()
                {
                    ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userid, IncludeProp: "product"),
                   
                    //orderHeader = new OrderHeader()
                };

                shoppingCartViewModel.orderHeader = _unitOfWork.OrderHeader.GetById(x => x.OrderStatus == SD.status_Pending && x.ApplicationId == userid);
                if (shoppingCartViewModel.orderHeader == null) 
                {
                    shoppingCartViewModel.orderHeader=new OrderHeader();
                    shoppingCartViewModel.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetById(x => x.Id == userid);
                    shoppingCartViewModel.orderHeader.Name = shoppingCartViewModel.orderHeader.ApplicationUser.name;
                    shoppingCartViewModel.orderHeader.StreetAddress = shoppingCartViewModel.orderHeader.ApplicationUser.StreetAddress;
                    shoppingCartViewModel.orderHeader.City = shoppingCartViewModel.orderHeader.ApplicationUser.City;
                    shoppingCartViewModel.orderHeader.PhoneNumber = shoppingCartViewModel.orderHeader.ApplicationUser.PhoneNumber; 
                    
                }

                foreach (var item in shoppingCartViewModel.ShoppingCarts)
                {

                    if (item.product.DiscounPrice == null)
                    {
                        item.price = item.product.Price;
                    }
                    else
                    {
                        item.price = (double)item.product.DiscounPrice;
                    }
                    shoppingCartViewModel.orderHeader.OrderTotal += (double)(item.price * item.count);


                }

            }
            return View(shoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost( shoppingCartViewModel shoppingCart)
        {

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            string userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userid != null && claimsIdentity != null)
            {
                //shoppingCartViewModel = new()
                //{
                //    ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userid, IncludeProp: "product"),
                //    orderHeader = new OrderHeader()
                //};

                shoppingCartViewModel = new()
                {
                    ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userid, IncludeProp: "product").ToList(),
                    orderHeader = shoppingCart.orderHeader
                };
                if (shoppingCart.orderHeader.Id == 0) 
                { 
                    shoppingCartViewModel.orderHeader.OrderDate = DateTime.Now;
                    shoppingCartViewModel.orderHeader.ApplicationId = userid;
                    shoppingCartViewModel.orderHeader.OrderStatus = SD.status_Pending;
                    shoppingCartViewModel.orderHeader.PaymentStatus = SD.Payment_status_Pending;
                    foreach (var item in shoppingCartViewModel.ShoppingCarts)
                    {

                        if (item.product.DiscounPrice == null)
                        {
                            item.price = item.product.Price;
                        }
                        else
                        {
                            item.price = (double)item.product.DiscounPrice;
                        }
                        shoppingCartViewModel.orderHeader.OrderTotal += (double)(item.price * item.count);


                    }
                    
                    _unitOfWork.OrderHeader.Add(shoppingCartViewModel.orderHeader);
                    _unitOfWork.save();

                }
                else
                {
                    foreach (var item in shoppingCartViewModel.ShoppingCarts)
                    {

                        if (item.product.DiscounPrice == null)
                        {
                            item.price = item.product.Price;
                        }
                        else
                        {
                            item.price = (double)item.product.DiscounPrice;
                        }
                        shoppingCartViewModel.orderHeader.OrderTotal += (double)(item.price * item.count);


                    }
                }









                    foreach (var item in shoppingCartViewModel.ShoppingCarts)
                    {
                        OrderDetail orderDetail = new()
                        {
                            OrderHeaderId = shoppingCartViewModel.orderHeader.Id,
                            productId = item.ProductId,
                            count = item.count,
                            price = (double)item.price
                        };
                        _unitOfWork.OrderDetail.Add(orderDetail);
                        _unitOfWork.save();

                    }
                   var domain="https://localhost:7224/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {

                            SuccessUrl = domain + $"N/Cart/OrderConfirmation?id={shoppingCartViewModel.orderHeader.Id} ",
                            CancelUrl = domain + $"N/Cart/Index",
                            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                            
                    Mode = "payment",
                };
                    foreach(var item in shoppingCartViewModel.ShoppingCarts)
                    {
                        var sessionLineItem = new SessionLineItemOptions()
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount=(long)(item.price * 100),
                                Currency="usd",
                                ProductData=new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name=item.product.name,
                                }

                            },
                            Quantity=item.count
                        
                        
                        };
                        options.LineItems.Add(sessionLineItem);

                    }
                        var service = new SessionService();
                        Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripPaymentId(shoppingCartViewModel.orderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction("OrderConfirmation" ,new {id = shoppingCartViewModel.orderHeader.Id});
        }

        public IActionResult OrderConfirmation( int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetById(x => x.Id == id, IncludeProp: "ApplicationUser");
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripPaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.status_Approved, SD.Payment_status_Approved);
                _unitOfWork.save();
            }

            HttpContext.Session.Clear();

            List<ShoppingCart> shoppingList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingList);
            _unitOfWork.save();

            return View("orderConfirm",id);
        }



    }
}
