using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using CLIP.Models;

namespace CLIP.Controllers
{
    [Authorize]
    public class UserCompetencyController : Controller
    {
        // GET: UserCompetency
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            
            // Get all competency modules
            var competencyModules = db.CompetencyModules
                .Include(cm => cm.UserCompetencies.Select(uc => uc.User))
                .ToList();
            
            return View(competencyModules);
        }

        // Legacy action - redirects to main view
        public ActionResult UserView()
        {
            return RedirectToAction("Index");
        }

        // GET: UserCompetency/Assign
        public ActionResult Assign()
        {
            var db = new ApplicationDbContext();
            
            // Get all users and competency modules for dropdowns
            ViewBag.Users = db.Users.ToList();
            ViewBag.CompetencyModules = db.CompetencyModules.ToList();
            
            return View();
        }

        // POST: UserCompetency/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(UserCompetency model, string[] Building)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                
                // Check if this competency is already assigned to the user
                bool exists = db.UserCompetencies.Any(uc => 
                    uc.UserId == model.UserId && 
                    uc.CompetencyModuleId == model.CompetencyModuleId);
                
                if (exists)
                {
                    TempData["ErrorMessage"] = "This competency is already assigned to the selected user.";
                    
                    // Reload ViewBag data for dropdowns
                    ViewBag.Users = db.Users.ToList();
                    ViewBag.CompetencyModules = db.CompetencyModules.ToList();
                    
                    return View(model);
                }
                
                // Status is now set from the form dropdown
                // model.Status = "Not Started";
                
                // Calculate expiry date if completion date is provided
                if (model.CompletionDate.HasValue)
                {
                    // Get validity months from the competency module
                    var competencyModule = db.CompetencyModules.Find(model.CompetencyModuleId);
                    if (competencyModule != null)
                    {
                        if (competencyModule.ValidityMonths.HasValue)
                        {
                            model.ExpiryDate = model.CompletionDate.Value.AddMonths(competencyModule.ValidityMonths.Value);
                        }
                    }
                }
                
                // Process selected buildings
                if (Building != null && Building.Length > 0)
                {
                    model.Building = string.Join(",", Building);
                }
                
                db.UserCompetencies.Add(model);
                db.SaveChanges();
                
                TempData["SuccessMessage"] = "Competency assigned to user successfully.";
                return RedirectToAction("Index");
            }
            
            // If we got this far, something failed; reload form
            var context = new ApplicationDbContext();
            ViewBag.Users = context.Users.ToList();
            ViewBag.CompetencyModules = context.CompetencyModules.ToList();
            
            return View(model);
        }

        // GET: UserCompetency/Edit/5
        public ActionResult Edit(int id)
        {
            var db = new ApplicationDbContext();
            var userCompetency = db.UserCompetencies
                .Include(uc => uc.User)
                .Include(uc => uc.CompetencyModule)
                .FirstOrDefault(uc => uc.Id == id);
                
            if (userCompetency == null)
            {
                TempData["ErrorMessage"] = "User competency not found.";
                return RedirectToAction("Index");
            }
            
            // Prepare status dropdown items with new options
            ViewBag.Statuses = new List<string> 
            {
                "Pending",
                "Certified"
            };
            
            // Set up the selected buildings if any
            if (!string.IsNullOrEmpty(userCompetency.Building))
            {
                ViewBag.SelectedBuildings = userCompetency.Building.Split(',');
            }
            else
            {
                ViewBag.SelectedBuildings = new string[] { };
            }
            
            return View(userCompetency);
        }

        // POST: UserCompetency/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserCompetency model, string[] Building)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                var userCompetency = db.UserCompetencies.Find(model.Id);
                
                if (userCompetency == null)
                {
                    TempData["ErrorMessage"] = "User competency not found.";
                    return RedirectToAction("Index");
                }
                
                // Update properties
                userCompetency.Status = model.Status;
                userCompetency.CompletionDate = model.CompletionDate;
                userCompetency.Remarks = model.Remarks;
                
                // Process selected buildings
                if (Building != null && Building.Length > 0)
                {
                    userCompetency.Building = string.Join(",", Building);
                }
                else
                {
                    userCompetency.Building = null;
                }
                
                // If status is changed to "Certified" and no completion date is set, set it to today
                if (model.Status == "Certified" && !userCompetency.CompletionDate.HasValue)
                {
                    userCompetency.CompletionDate = DateTime.Today;
                    
                    // Calculate expiry date based on the competency module's validity
                    var competencyModule = db.CompetencyModules.Find(userCompetency.CompetencyModuleId);
                    if (competencyModule != null)
                    {
                        if (competencyModule.ValidityMonths.HasValue)
                        {
                            userCompetency.ExpiryDate = userCompetency.CompletionDate.Value.AddMonths(competencyModule.ValidityMonths.Value);
                        }
                    }
                }
                
                db.SaveChanges();
                
                TempData["SuccessMessage"] = "User competency updated successfully.";
                return RedirectToAction("Index");
            }
            
            // If we got this far, something failed; reload validation
            var context = new ApplicationDbContext();
            
            // Prepare status dropdown items with new options
            ViewBag.Statuses = new List<string> 
            {
                "Pending",
                "Certified"
            };
            
            // Set up the selected buildings if any
            if (Building != null && Building.Length > 0)
            {
                ViewBag.SelectedBuildings = Building;
            }
            else
            {
                ViewBag.SelectedBuildings = new string[] { };
            }
            
            return View(model);
        }

        // POST: UserCompetency/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var db = new ApplicationDbContext();
            var userCompetency = db.UserCompetencies.Find(id);
            
            if (userCompetency != null)
            {
                db.UserCompetencies.Remove(userCompetency);
                db.SaveChanges();
                TempData["SuccessMessage"] = "User competency removed successfully.";
            }
            
            return RedirectToAction("Index");
        }

        // GET: UserCompetency/UserCompetencies/userId
        public ActionResult UserCompetencies(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index");
            }
            
            var db = new ApplicationDbContext();
            var user = db.Users.Find(userId);
            
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }
            
            var userCompetencies = db.UserCompetencies
                .Include(uc => uc.CompetencyModule)
                .Where(uc => uc.UserId == userId)
                .ToList();
                
            ViewBag.User = user;
            
            return View(userCompetencies);
        }

        // GET: UserCompetency/MyCompetencies
        public ActionResult MyCompetencies()
        {
            string userId = User.Identity.GetUserId();
            var db = new ApplicationDbContext();
            
            var userCompetencies = db.UserCompetencies
                .Include(uc => uc.CompetencyModule)
                .Where(uc => uc.UserId == userId)
                .ToList();
                
            return View(userCompetencies);
        }
    }
} 