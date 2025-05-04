using Microsoft.AspNetCore.Mvc;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.DataAccess.Data;
using TechZone.Models;
using TechZone.Models.Models;
using Microsoft.AspNetCore.Authorization;
using TechZone.Utility;
using Microsoft.AspNetCore.Hosting;
using TechZone.Models.ViewModels;

namespace TechZoneWeb.Areas.Supervisor.Controllers
{
    [Area("Supervisor")]
    [Authorize(Roles=SD.RoleAdmin)]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandController( IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult All()
        {
            List<Brand> BrandList=_unitOfWork.Brand.GetAll().ToList();
            return View("AllBrands",BrandList);
        }

        public IActionResult Edit(int? id) 
        { 
            if(id != 0 && id != null )
            {
               Brand? BrandFrmDb= _unitOfWork.Brand.GetById(x=>x.Id==id);
                if (BrandFrmDb != null)
                {
                    return View("Details", BrandFrmDb);
                }
                return NotFound();
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult SaveEdit(Brand objfrmRequest,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (objfrmRequest.Id != 0 )
                {
                    string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                    if (file != null)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine(wwwrootpath, @"Images/Brands");

                        if (!string.IsNullOrEmpty(objfrmRequest.ImageUrl))
                        {
                            string oldPath = Path.Combine(wwwrootpath, objfrmRequest.ImageUrl.Trim('\\'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }

                        }

                        using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                        {
                            file.CopyTo(filestream);

                        }
                        objfrmRequest.ImageUrl = @"\Images\Brands\" + filename;
                    }

                    _unitOfWork.Brand.Update(objfrmRequest);
                    _unitOfWork.save();
                    TempData["success"] = "Updated Successfully";
                    return RedirectToAction("All");
                }
                return NotFound();
            }
            
                return NotFound();

        }

        //public IActionResult Remove(int? id)
        //{
        //    if (id != null && id != 0)
        //    {
        //        Brand obj = _unitOfWork.Brand.GetById(x => x.Id == id);
        //        string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();

        //        if (!string.IsNullOrEmpty(obj.ImageUrl))
        //        {
        //            string imagePath = Path.Combine(wwwrootpath, obj.ImageUrl.TrimStart('\\'));

        //            if (System.IO.File.Exists(imagePath))
        //            {
        //                System.IO.File.Delete(imagePath);
        //            }
        //        }

        //        if (obj != null)
        //        {
        //            _unitOfWork.Brand.Remove(obj);
        //            _unitOfWork.save();
        //            TempData["error"] = "The Element Deleted";
        //            return RedirectToAction("All");
        //        }
        //        return NotFound();
        //    }
        //    return NotFound();
        //}

        public IActionResult create()
        {
            return View("create");
        }
        [HttpPost]
        public IActionResult SaveCreate(Brand objFrmRequest,IFormFile? file) 
        {

            if (ModelState.IsValid) 
            {
               string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwrootpath, @"Images/Brands");
                    using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    objFrmRequest.ImageUrl= @"\Images\Brands\" + filename;

                }
                    _unitOfWork.Brand.Add(objFrmRequest);
                _unitOfWork.save();
                TempData["success"] = "created successfully";
                return RedirectToAction("All");
            }
            TempData["error"] = "failed To Create Category";
            return RedirectToAction("All");





               
               
          

        }

    }
}