using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_TMS.Models;

namespace Project_TMS.Controllers
{
    public class UserController : Controller
    {
        public ApplicationDbContext _db;
        private SignInManager<IdentityUserNew> signInManager { get; set; }
        private UserManager<IdentityUserNew> userManager { get; set; }
        public UserController(SignInManager<IdentityUserNew> _signInManager, UserManager<IdentityUserNew> _userManager, ApplicationDbContext db)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            _db = db;
        }

        public IActionResult GetTasks()
        {
            var details =_db.tasks.ToList();
            return View(details);
        }

        public IActionResult UpdateTaskStatus(int? id)
        {
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var task = _db.tasks.FirstOrDefault(c => c.Id == id);
                if (task == null)
                {
                    return NotFound();
                }
                return View(task);
            }
            return View();
        }
       
        

        [HttpPost]
        public IActionResult UpdateTaskStatus(TaskModel obj)
        {
            var x = _db.tasks.FirstOrDefault(c => c.Id == obj.Id);
            x.Status = obj.Status;
                  
                _db.tasks.Update(x);
                _db.SaveChanges();
                TempData["success"] = "Data Updated Successfullyy!";
                return RedirectToAction("GetTasks");
            
            return View();
        }
    }
}
    