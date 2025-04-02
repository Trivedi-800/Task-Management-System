using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

using Project_TMS.Models;
using System.Net.Mail;
using System.Threading;

namespace Project_TMS.Controllers
{
    public class RoleManagerController : Controller
    {
            private RoleManager<IdentityRole> roleManager;
            private UserManager<IdentityUserNew> userManager;


            public RoleManagerController(RoleManager<IdentityRole> _roleManager, UserManager<IdentityUserNew> _userManager)
            {
                roleManager = _roleManager;
                userManager = _userManager;
            }
          
            public async Task<IActionResult> GetAllRoles()
            {
                var roles = await roleManager.Roles.ToListAsync();
                return View(roles);
            }
            
            public IActionResult AddRole()
            {
                return View();
            }
       
            [HttpPost]
            public async Task<IActionResult> AddRole(string Name)
            {

                if (!string.IsNullOrEmpty(Name))
                {
                    var r = await roleManager.FindByNameAsync(Name);
                    if (r == null)
                    {
                        var res = new IdentityRole() { Name = Name };
                        var result = await roleManager.CreateAsync(res);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("GetAllRoles");
                        }
                        else
                        {
                            foreach (var i in result.Errors)
                            {
                                ModelState.AddModelError("", i.Description);
                            }
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Role", "Enter valid Role Name");
                    return View();
                }
                return View();
            }
           /* public async Task<IActionResult> DisplayAllUser( )
            {
                var result = await userManager.Users.ToListAsync();
                return View(result);
            }*/
            public async Task<IActionResult> AddRemoveRole(string id)
            {
                ViewBag.msg = TempData["msg"];
                var user = await userManager.FindByIdAsync(id);
            ViewBag.UserName = user.FirstName;
                var roles = roleManager.Roles.ToList();

                List<UserRoleVM> users = new List<UserRoleVM>();

                foreach (var role in roles)
                {
                    UserRoleVM obj = new UserRoleVM();
                    obj.Id = role.Id;
                    obj.Name = role.Name;
                    obj.UserId = id;

                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        obj.IsAssigned = true;
                    }
                    else
                    {
                        obj.IsAssigned = false;
                    }
                    users.Add(obj);
                }
                return View(users);
            }
            [HttpPost]
            public async Task<IActionResult> AddRemoveRole(List<UserRoleVM> UserRoles)
            {
                var user = await userManager.FindByIdAsync(UserRoles[0].UserId);
                string msg = string.Empty;


                foreach (var role in UserRoles)
                {
                    if (role.IsAssigned && await userManager.IsInRoleAsync(user, role.Name) == false)
                    {
                        var result = await userManager.AddToRoleAsync(user, role.Name);
                        if (result.Succeeded)
                        {
                            msg = "Role saved successfully";
                        }
                    }

                    else if (role.IsAssigned == false && await userManager.IsInRoleAsync(user, role.Name) == true)
                    {
                        var result = await userManager.RemoveFromRoleAsync(user, role.Name);
                        if (result.Succeeded)
                        {
                            msg = "Role saved successfully";
                        }
                    }
                    TempData["msg"] = msg;
                }
                return RedirectToAction("AddRemoveRole");
            }


        }
    }


