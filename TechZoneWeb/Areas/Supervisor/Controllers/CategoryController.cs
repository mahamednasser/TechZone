using Microsoft.AspNetCore.Mvc;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.DataAccess.Data;
using TechZone.Models;
using TechZone.Models.Models;
using Microsoft.AspNetCore.Authorization;
using TechZone.Utility;

namespace TechZoneWeb.Areas.Supervisor.Controllers
{
    [Area("Supervisor")]
    [Authorize(Roles=SD.RoleAdmin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        public IActionResult SaveEdit(Category objfrmRequest)
        {
            if (ModelState.IsValid)
            {
                if (objfrmRequest.Id != 0)
                {
                    _unitOfWork.Category.Update(objfrmRequest);
                    _unitOfWork.save();
                    TempData["success"] = "Updated Successfully";
                    return RedirectToAction("All");
                }
                return NotFound();
            }
            
                return NotFound();
        }

        public IActionResult Remove(int? id)
        {
            if (id != null && id != 0)
            {
                Category obj = _unitOfWork.Category.GetById(x => x.Id == id);
                if (obj != null)
                {
                    _unitOfWork.Category.Remove(obj);
                    _unitOfWork.save();
                    TempData["error"] = "The Element Deleted";
                    return RedirectToAction("All");
                }
                return NotFound();
            }
            return NotFound();
        }

        public IActionResult create()
        {
            return View("create");
        }
        [HttpPost]
        public IActionResult SaveCreate(Category objFrmRequest) 
        {
            if (ModelState.IsValid) 
            {
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