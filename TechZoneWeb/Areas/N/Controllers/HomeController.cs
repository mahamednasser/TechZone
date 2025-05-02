using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.Models.Models;
using TechZone.Models.ViewModels;
using TechZone.Utility;

namespace TechZoneWeb.Areas.N.Controllers
{
    [Area("N")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //obj.Id = null;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(userId != null)
            {
                HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId.Value).Count());

            }

            TempData["success"] = "Hello In TechZone";
            IEnumerable<Product> products = _unitOfWork.Product.GetAll();
            return View(products);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new() {

                product = _unitOfWork.Product.GetById(x => x.Id == id, IncludeProp: "Category"),
                count = 1,
                ProductId = id


            };
            if (cart.product != null)
            {
                return View(cart);
            }
            return View("Index");
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(ShoppingCart obj)
        {
            //obj.Id = null;
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
           string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null && claimsIdentity!=null) 
            {
                obj.ApplicationUserId = userId;
               

                ShoppingCart shoppingFrmDb=_unitOfWork.ShoppingCart.GetById(x=>x.ApplicationUserId==userId && x.ProductId==obj.ProductId);
                if (shoppingFrmDb != null) 
                {
                    shoppingFrmDb.count += obj.count;
                    _unitOfWork.ShoppingCart.Update(shoppingFrmDb);
                    _unitOfWork.save();

                }
                else
                {
                    _unitOfWork.ShoppingCart.Add(obj);
                    _unitOfWork.save();
                    HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());   
                }
            }

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
