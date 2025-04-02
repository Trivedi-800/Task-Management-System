using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_TMS.Models;
using System.Data;

namespace Project_TMS.Controllers
{   public class AccountController : Controller
    {
        private ApplicationDbContext db ;
        private SignInManager<IdentityUserNew> signInManager { get; set; }
        private UserManager<IdentityUserNew> userManager { get; set; }
        public AccountController(SignInManager<IdentityUserNew> _signInManager, UserManager<IdentityUserNew> _userManager, ApplicationDbContext _db)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            db = _db;
        }
        public IActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login(UserSignIn user)
        {
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    var result = await userManager.FindByEmailAsync(user.Email);
                    if (result != null)
                    {
                        Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.PasswordSignInAsync(result, user.Password, false, true);
                        if (signInResult.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (signInResult.IsLockedOut)
                        {
                            ModelState.AddModelError("", "Login attempt exceeded max limit");
                            return View();
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please enter valid Email/Password");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Please enter valid Email and Password");
                        return View();
                    }

                }

                else
                {
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Please enter Email and Password");
                return View();
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserSignUp user)
        {
            if (user == null)
            {
                ModelState.AddModelError("", "User details can not empty!");
                return View();
            }

            else if (ModelState.IsValid)
            {
                var res = await userManager.FindByEmailAsync(user.Email);
                if (res == null)
                {
                    var userData = new IdentityUserNew()
                    {
                        UserName = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PasswordHash = user.Password,
                        PhoneNumber = user.Mobile,
                        Role = user.Role


                    };
                    var result = await userManager.CreateAsync(userData, user.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User details can not empty!");
                    return View();
                }
            }

            else
            {
                return View();
            }

            

        }
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult ShowUsers()
        {
            var details = db.identityUsers.ToList();
            return View(details);

        }
        public IActionResult EditUsers(string? id)
        {
            if (id == null || id == "0")
            {
                return NotFound();
            }
            var User = db.identityUsers.FirstOrDefault(c => c.Id == id);
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }
        [HttpPost]
        public IActionResult EditUsers(IdentityUserNew obj)
        {

            if (ModelState.IsValid)
            {
                this.db.identityUsers.Update(obj);
                db.SaveChanges();
                TempData["success"] = "Data Updated Successfullyy!";
                return RedirectToAction("ShowUsers");
            }
            return View();
        }

    }

}
    


