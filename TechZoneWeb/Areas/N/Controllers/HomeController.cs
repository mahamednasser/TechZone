using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            
            HomeCategoryBrandProduct homeCategoryBrandProduct = new() { 
                Products= _unitOfWork.Product.GetAll(IncludeProp: "Category,ProductImages,Brand").ToList(),
                Brands=_unitOfWork.Brand.GetAll().ToList(),
                Categories=_unitOfWork.Category.GetAll().ToList()  

            };
            
            return View(homeCategoryBrandProduct);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new() {

                product = _unitOfWork.Product.GetById(x => x.Id == id, IncludeProp: "Category,ProductImages,Brand"),
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


       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult DisplayCategory(int CategoryId)
        {
            Category category=_unitOfWork.Category.GetById(x=>x.Id==CategoryId);
            ViewData["Title"] = category.name;

            IEnumerable<Product> products = _unitOfWork.Product.GetAll(x => x.CategoryId == CategoryId, IncludeProp: "Category,ProductImages,Brand").ToList();

            return View("DisplayCategory",products);

        }


        public IActionResult DisplayBrand(int BrandId)
        {
            Brand brand = _unitOfWork.Brand.GetById(x => x.Id == BrandId);
            ViewData["Title"] = brand.Name;
            IEnumerable < Product> products = _unitOfWork.Product.GetAll(x => x.BrandId == BrandId, IncludeProp: "Category,ProductImages,Brand").ToList();

            return View("DisplayBrand", products);

        }

        public IActionResult Search(string query)
        {
            ViewData["Title"] = query;

            query = query.ToLower();
            IEnumerable<Product> results = _unitOfWork.Product.GetAll(p => p.name.ToLower().Contains(query), IncludeProp: "Category,ProductImages,Brand").ToList();
            return View("Search",results);
        }

    }
}
