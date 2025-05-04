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
            List<Product> ProductList=_unitOfWork.Product.GetAll(IncludeProp:"Category,Brand").ToList();
            return View("AllProducts",ProductList);
        }

        public IActionResult Edit(int id) 
        { 
            if(id != 0  )
            {
               Product ProductFrmDb= _unitOfWork.Product.GetById(x=>x.Id==id,IncludeProp:"ProductImages");
                if (ProductFrmDb != null)
                {
                    CreateProductViewModel obj= new CreateProductViewModel();
                    obj.Id = ProductFrmDb.Id;
                    obj.Price = ProductFrmDb.Price;
                    obj.Description = ProductFrmDb.Description;
                    obj.DiscounPrice = ProductFrmDb.DiscounPrice;
                    obj.CategoryId = ProductFrmDb.CategoryId;
                    obj.name=ProductFrmDb.name;
                    obj.BrandId = ProductFrmDb.BrandId;
                    obj.ProductImages= ProductFrmDb.ProductImages;
                    obj.ProductCount=ProductFrmDb.ProductCount;
                    obj.CategoryList=_unitOfWork.Category.GetAll().ToList();
                    obj.BrandList=_unitOfWork.Brand.GetAll().ToList();
                    return View("Details", obj);
                }
                return NotFound();
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult SaveEdit(CreateProductViewModel objfrmRequest,List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                if (objfrmRequest.Id != 0 && objfrmRequest.Id !=null )
                {
                    string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                    if (files != null)
                    {
                        foreach (IFormFile file in files) { 
                        
                            string filename=Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string productPath = @"Images\Products\product-" + objfrmRequest.Id;

                            string FinalPath =Path.Combine(wwwrootpath,productPath);

                            if (!Directory.Exists(FinalPath)) 
                            {
                                Directory.CreateDirectory(FinalPath);
                            }

                            using (var filestream = new FileStream(Path.Combine(FinalPath, filename), FileMode.Create))
                            {
                                file.CopyTo(filestream);

                            }
                            ProductImage productImage = new()
                            {
                                ImageUrl = @"\" + productPath + @"\" + filename,
                                ProductId = (int)objfrmRequest.Id
                            };
                            if (objfrmRequest.ProductImages == null) { 
                                objfrmRequest.ProductImages = new List<ProductImage>();
                            
                            }
                            objfrmRequest.ProductImages.Add(productImage);
                        }
                        Product product = _unitOfWork.Product.GetById(x => x.Id == objfrmRequest.Id);
                        product.name = objfrmRequest.name;
                        product.ProductImages = objfrmRequest.ProductImages;
                        product.CategoryId = objfrmRequest.CategoryId;
                        product.Price = objfrmRequest.Price;
                        product.ProductCount = objfrmRequest.ProductCount;
                        product.Description = objfrmRequest.Description;
                        product.DiscounPrice = objfrmRequest.DiscounPrice;
                        product.BrandId = objfrmRequest.BrandId;
  
                        _unitOfWork.Product.Update(product);
                        _unitOfWork.save();
                       

                    }
                   
                    TempData["success"] = "Updated Successfully";
                    return RedirectToAction("All");
                }
                TempData["error"] = "failed To update Product";
                objfrmRequest.CategoryList=_unitOfWork.Category.GetAll().ToList() ;
                objfrmRequest.BrandList = _unitOfWork.Brand.GetAll().ToList();
                return View("Details",objfrmRequest);
            }
            TempData["error"] = "failed To update Product";

            objfrmRequest.CategoryList = _unitOfWork.Category.GetAll().ToList();
            objfrmRequest.BrandList= _unitOfWork.Brand.GetAll().ToList();
            return View("Details", objfrmRequest);
        }

        public IActionResult Remove(int? id)
        {
            if (id != null && id != 0)
            {
                Product obj = _unitOfWork.Product.GetById(x => x.Id == id);
                if (obj != null)
                {
                    string productPath = @"Images\Products\product-" + id;

                    string FinalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

                    if (Directory.Exists(FinalPath))
                    {
                        string[] files = Directory.GetFiles(FinalPath);
                        foreach (string file in files)
                        {
                            System.IO.File.Delete(file);
                        }
                        Directory.Delete(FinalPath);
                    }


                    _unitOfWork.Product.Remove(obj);
                    _unitOfWork.save();
                    TempData["success"] = "The Product Deleted Successfully";
                    return RedirectToAction("All");
                }
                
                return NotFound();
            }
            return NotFound();
        }

        public IActionResult create()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            List<Brand> brands=_unitOfWork.Brand.GetAll().ToList();
            CreateProductViewModel productViewModel = new CreateProductViewModel();
            productViewModel.CategoryList = categories;
            productViewModel.BrandList = brands;

            return View("create",productViewModel);
        }
        [HttpPost]
        public IActionResult SaveCreate(CreateProductViewModel objfrmRequest ,List<IFormFile>? files) 
        {
            if (ModelState.IsValid) 
            {
                    Product obj = new Product();
                if (objfrmRequest.Id == 0 || objfrmRequest.Id==null)
                {
                    obj.name = objfrmRequest.name;
                    obj.Description = objfrmRequest.Description;
                    obj.Price = objfrmRequest.Price;
                    obj.ProductCount = objfrmRequest.ProductCount;
                    obj.DiscounPrice = objfrmRequest.DiscounPrice;
                    obj.CategoryId = objfrmRequest.CategoryId;
                    obj.BrandId = objfrmRequest.BrandId;

                    _unitOfWork.Product.Add(obj);
                    _unitOfWork.save();



                    if (files != null)
                    {
                        string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                        foreach (IFormFile file in files)
                        {

                            string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string productPath = @"Images\Products\product-" + obj.Id;

                            string FinalPath = Path.Combine(wwwrootpath, productPath);

                            if (!Directory.Exists(FinalPath))
                            {
                                Directory.CreateDirectory(FinalPath);
                            }

                            using (var filestream = new FileStream(Path.Combine(FinalPath, filename), FileMode.Create))
                            {
                                file.CopyTo(filestream);

                            }
                            ProductImage productImage = new()
                            {
                                ImageUrl = @"\" + productPath + @"\" + filename,
                                ProductId = obj.Id
                            };
                            if (obj.ProductImages == null)
                            {
                                obj.ProductImages = new List<ProductImage>();

                            }
                            obj.ProductImages.Add(productImage);
                        }

                        _unitOfWork.Product.Update(obj);
                        _unitOfWork.save();
                        TempData["success"] = " Product created successfully";

                    }



                    return RedirectToAction("All");
                }
            }
            objfrmRequest.CategoryList= _unitOfWork.Category.GetAll().ToList();
            objfrmRequest.BrandList= _unitOfWork.Brand.GetAll().ToList();
            TempData["error"] = "failed To Create Product";
            return RedirectToAction("create",objfrmRequest);
        }


        public IActionResult removeImage(int ImageId)
        {
            ProductImage image =_unitOfWork.ProductImage.GetById(x=>x.Id==ImageId);
            var productId=image.ProductId;
            if(image != null)
            {
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    var oldPath=Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                }
                    _unitOfWork.ProductImage.Remove(image);
                    _unitOfWork.save();
                TempData["success"] = "The Image Is Deleted Successfully";
            }


            return RedirectToAction("Edit", new { id = productId });
        }

    }
}