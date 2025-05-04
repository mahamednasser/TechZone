using Microsoft.AspNetCore.Mvc;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.DataAccess.Data;
using TechZone.Models;
using TechZone.Models.Models;
using TechZone.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TechZone.Utility;
using Microsoft.AspNetCore.Identity;

namespace TechZoneWeb.Areas.Supervisor.Controllers
{
    [Area("Supervisor")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult All()
        {
            List<ApplicationUser> UserList = _unitOfWork.ApplicationUser.GetAll().ToList();
           

            foreach (var user in UserList)
            {
                user.role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            }
            return View("AllUser", UserList);
        }
        public IActionResult LockUnlock(string id)
        {
            ApplicationUser user = _unitOfWork.ApplicationUser.GetById(x =>x.Id == id);

            if (user != null)
            {

                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
                {
                    //user is lock and need to unlock
                    user.LockoutEnd = DateTime.Now;
                    TempData["success"] = "user is unlocked";

                }
                else
                {
                    //need to lock the user
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                    TempData["success"] = "user is locked";

                }
                _unitOfWork.ApplicationUser.Update(user);
                _unitOfWork.save();
                List<ApplicationUser> UserList = _unitOfWork.ApplicationUser.GetAll().ToList();
                return View("AllUser",UserList);
            }

            return NoContent();

        }


        public IActionResult Remove(string id)
        {
            ApplicationUser user = _unitOfWork.ApplicationUser.GetById(x => x.Id == id);
            _unitOfWork.ApplicationUser.Remove(user);
            _unitOfWork.save();
            return RedirectToAction("All");
        }

       

    }
}