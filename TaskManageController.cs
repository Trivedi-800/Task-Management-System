using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_TMS.Models;
using System.Security.Claims;

namespace TMS.Controllers
{
  
    public class TaskManageController : Controller
    {
        private ApplicationDbContext _db;
        private SignInManager<IdentityUserNew> signInManager { get; set; }
        private UserManager<IdentityUserNew> userManager { get; set; }
        public TaskManageController(SignInManager<IdentityUserNew> _signInManager, UserManager<IdentityUserNew> _userManager, ApplicationDbContext db)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            _db = db;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var objCategoryList = _db.tasks.Include(e=>e.AssignedTo).ToList();

            return View(objCategoryList);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(TaskModel obj)
        {

            if (ModelState.IsValid)
            {
                _db.tasks.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Data Saved Successfullyy!";
                return RedirectToAction("Index");

            }
            return View();
        }
        
        public IActionResult Edit(int? id)
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
      
        [HttpPost]
        public IActionResult Edit(TaskModel obj)
        {

            if (ModelState.IsValid)
            {
                _db.tasks.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Data Updated Successfullyy!";
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var t = _db.tasks.FirstOrDefault(x => x.Id == id);
            if (t == null)
            {
                return NotFound();
            }
            _db.tasks.Remove(t);
            _db.SaveChanges();
            TempData["delete"] = "Data Deleted Successfullyy!";
            return RedirectToAction("Index");



        }
        [Authorize(Roles = "Admin")]
        public IActionResult AssignTask(int id)
        {
            var detail = _db.tasks.Find(id);
            return View(detail);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>AssignTask(TaskModel obj)
        {
            var detail = _db.tasks.Find(obj.Id);
            var Assigned = Request.Form["AssignedTo"];
            IdentityUserNew user = new IdentityUserNew();
            if (detail != null)
            {
                if (!string.IsNullOrEmpty(Assigned)) ;
                {
                    user = await userManager.FindByIdAsync(Assigned);
                }
                detail.AssignedDate = obj.AssignedDate;
                detail.AssignedTo = user;
                _db.tasks.Update(detail);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        /* public IActionResult GetStatus()
         {
             var statuses = _db.tasks.Select(t => t.Status).Distinct().ToList();
             return Json(statuses);
         }
         public IActionResult GetPriority()
         {
             var priority = _db. tasks.Select(t => t.Priority).Distinct().ToList();
             return Json(priority);
         }*/
        [Authorize(Roles = "Admin")]
        public IActionResult GetAssignees()
        {
            //var Assigness = userManager.GetUsersInRoleAsync("User");
            var Assign = _db.identityUsers.ToList();
            return Json(Assign);
        }
        
    }
}


