using Microsoft.AspNetCore.Mvc;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.DataAccess.Data;
using TechZone.Models;
using TechZone.Models.Models;
using TechZone.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TechZone.Utility;

namespace TechZoneWeb.Areas.Supervisor.Controllers
{
    [Area("Supervisor")]
    [Authorize(Roles =SD.RoleAdmin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController( IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult All()
        {
            List<Product> ProductList=_unitOfWork.Product.GetAll(IncludeProp:"Category").ToList();
            return View("AllProducts",ProductList);
        }

        public IActionResult Edit(int id) 
        { 
            if(id != 0  )
            {
               Product ProductFrmDb= _unitOfWork.Product.GetById(x=>x.Id==id);
                if (ProductFrmDb != null)
                {
                    CreateProductViewModel obj= new CreateProductViewModel();
                    obj.Id = ProductFrmDb.Id;
                    obj.Price = ProductFrmDb.Price;
                    obj.Description = ProductFrmDb.Description;
                    obj.DiscounPrice = ProductFrmDb.DiscounPrice;
                    obj.CategoryId = ProductFrmDb.CategoryId;
                    obj.name=ProductFrmDb.name;
                    obj.ImageUrl = ProductFrmDb.ImageUrl;
                    obj.CategoryList=_unitOfWork.Category.GetAll().ToList();
                    return View("Details", obj);
                }
                return NotFound();
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult SaveEdit(CreateProductViewModel objfrmRequest,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (objfrmRequest.Id != 0 && objfrmRequest.Id !=null )
                {
                    string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                    if (file != null)
                    {
                        string filename=Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                       string productPath=Path.Combine(wwwrootpath,@"Images/Products");

                        if (!string.IsNullOrEmpty( objfrmRequest.ImageUrl)) 
                        { 
                            string oldPath=Path.Combine(wwwrootpath,objfrmRequest.ImageUrl.Trim('\\'));
                            if (System.IO.File.Exists(oldPath)) 
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        
                        }

                        using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create)) 
                        {
                            file.CopyTo(filestream);
                        
                        }
                        objfrmRequest.ImageUrl = @"\Images\Products\" + filename;
                    }
                    Product product=_unitOfWork.Product.GetById(x=>x.Id == objfrmRequest.Id);
                    product.name = objfrmRequest.name;
                    product.ImageUrl = objfrmRequest.ImageUrl;
                    product.CategoryId = objfrmRequest.CategoryId;
                    product.Price = objfrmRequest.Price;
                    product.Description = objfrmRequest.Description;
                    product.DiscounPrice = objfrmRequest.DiscounPrice;
  
                    _unitOfWork.Product.Update(product);
                    _unitOfWork.save();
                    TempData["success"] = "Updated Successfully";
                    return RedirectToAction("All");
                }
                objfrmRequest.CategoryList=_unitOfWork.Category.GetAll().ToList() ;
                return View("Details",objfrmRequest);
            }

            objfrmRequest.CategoryList = _unitOfWork.Category.GetAll().ToList();
            return View("Details", objfrmRequest);
        }

        public IActionResult Remove(int? id)
        {
            if (id != null && id != 0)
            {
                Product obj = _unitOfWork.Product.GetById(x => x.Id == id);
                if (obj != null)
                {
                    _unitOfWork.Product.Remove(obj);
                    _unitOfWork.save();
                    TempData["success"] = "The Element Deleted";
                    return RedirectToAction("All");
                }
                return NotFound();
            }
            return NotFound();
        }

        public IActionResult create()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            CreateProductViewModel productViewModel = new CreateProductViewModel();
            productViewModel.CategoryList = categories;

            return View("create",productViewModel);
        }
        [HttpPost]
        public IActionResult SaveCreate(CreateProductViewModel objFrmRequest ,IFormFile? file) 
        {
            if (ModelState.IsValid) 
            {
                string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwrootpath, @"Images/Products");

                    using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    objFrmRequest.ImageUrl =@"\Images\Products\" + filename;
                }
                //mapping 
                Product obj = new Product();
                obj.name=objFrmRequest.name;
                obj.Description=objFrmRequest.Description;
                obj.Price=objFrmRequest.Price;
                obj.DiscounPrice=objFrmRequest.DiscounPrice;
                obj.ImageUrl=objFrmRequest.ImageUrl;
                obj.CategoryId=objFrmRequest.CategoryId;

                _unitOfWork.Product.Add(obj);
                _unitOfWork.save();
                TempData["success"] = "created successfully";
                return RedirectToAction("All");
            }
            objFrmRequest.CategoryList= _unitOfWork.Category.GetAll().ToList();
            TempData["error"] = "failed To Create Product";
            return RedirectToAction("create",objFrmRequest);
        }

    }
}