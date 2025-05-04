using Microsoft.AspNetCore.Mvc;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.DataAccess.Data;
using TechZone.Models;
using TechZone.Models.Models;
using Microsoft.AspNetCore.Authorization;
using TechZone.Utility;
using Microsoft.AspNetCore.Hosting;

namespace TechZoneWeb.Areas.Supervisor.Controllers
{
    [Area("Supervisor")]
    [Authorize(Roles=SD.RoleAdmin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CategoryController( IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult All()
        {
            List<Category> CategoryList=_unitOfWork.Category.GetAll().ToList();
            return View("AllCategories",CategoryList);
        }

        public IActionResult Edit(int? id) 
        { 
            if(id != 0 && id != null )
            {
               Category? CategoryFrmDb= _unitOfWork.Category.GetById(x=>x.Id==id);
                if (CategoryFrmDb != null)
                {
                    return View("Details", CategoryFrmDb);
                }
                return NotFound();
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult SaveEdit(Category objfrmRequest,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (objfrmRequest.Id != 0)
                {


                    string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                    if (file != null)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine(wwwrootpath, @"Images/Category");

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
                        objfrmRequest.ImageUrl = @"\Images\Category\" + filename;
                    }


                    _unitOfWork.Category.Update(objfrmRequest);
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
        //        Category obj = _unitOfWork.Category.GetById(x => x.Id == id);
        //        if (obj != null)
        //        {
        //            _unitOfWork.Category.Remove(obj);
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
        public IActionResult SaveCreate(Category objFrmRequest,IFormFile? file) 
        {
            if (ModelState.IsValid) 
            {

                string wwwrootpath = _webHostEnvironment.WebRootPath.ToString();
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwrootpath, @"Images/Category");
                    using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    objFrmRequest.ImageUrl = @"\Images\Category\" + filename;

                }


                _unitOfWork.Category.Add(objFrmRequest);
                _unitOfWork.save();
                TempData["success"] = "created successfully";
                return RedirectToAction("All");
            }
            TempData["error"] = "failed To Create Category";
            return RedirectToAction("All");
        }

    }
}