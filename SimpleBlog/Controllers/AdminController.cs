using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Entities;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Dashboard()
        {
            // get list of users with thier roles
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();
            var userRoles = new List<UserRoleViewModel>();
            foreach (var user in users)
            {
                var userRole = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList()
                };
                userRoles.Add(userRole);
            }
            return View(userRoles);
        }

        public IActionResult ManageUserRoles(string id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = _roleManager.Roles.ToList();
            var userRoles = _userManager.GetRolesAsync(user).Result.ToList();
            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                Roles = roles.Select(r => new RoleViewModel
                {
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name)
                }).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult ManageUserRoles(ManageUserRolesViewModel model)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = _roleManager.Roles.ToList();
            var userRoles = _userManager.GetRolesAsync(user).Result.ToList();
            foreach (var role in roles)
            {
                if (model.Roles.Any(r => r.RoleName == role.Name && r.IsSelected))
                {
                    if (!userRoles.Contains(role.Name))
                    {
                        _userManager.AddToRoleAsync(user, role.Name).Wait();
                    }
                }
                else
                {
                    if (userRoles.Contains(role.Name))
                    {
                        _userManager.RemoveFromRoleAsync(user, role.Name).Wait();
                    }
                }
            }
            return RedirectToAction("Dashboard");
        }
    }
}
