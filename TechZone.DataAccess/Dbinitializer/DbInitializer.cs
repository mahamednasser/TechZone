using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.DataAccess.Data;
using TechZone.Models.Models;
using TechZone.Utility;

namespace TechZone.DataAccess.Dbinitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;
        
        public DbInitializer( UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,AppDbContext db)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _db = db;
            
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0) 
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { 
            }


            if (!_roleManager.RoleExistsAsync(SD.RoleUserCustomer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.RoleUserCustomer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.RoleEmployee)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName="Admin",
                    Email="admin@gmail.com",
               
                },"Admin123$").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
                _userManager.AddToRoleAsync(user, SD.RoleAdmin).GetAwaiter().GetResult();
            
            
            
            }

            return;

        } 
    }
}
