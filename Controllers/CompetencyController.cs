using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using CLIP.Models;

namespace CLIP.Controllers
{
    public class CompetencyController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            // Get list of all competency modules
            var db = new ApplicationDbContext();
            var competencyModules = db.CompetencyModules.ToList();
            
            return View("Competency", competencyModules);
        }

        [Authorize]
        public ActionResult Add()
        {
            return View("AddCompetency");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Add(CompetencyModule model)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();

                // Check for existing module with the same name
                bool nameExists = db.CompetencyModules.Any(c => c.ModuleName == model.ModuleName);
                if (nameExists)
                {
                    ModelState.AddModelError("ModuleName", "A competency module with this name already exists.");
                    return View("AddCompetency", model);
                }

                try
                {
                    db.CompetencyModules.Add(model);
                    db.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Competency module added successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.InnerException?.Message?.Contains("unique constraint") == true ||
                        ex.InnerException?.InnerException?.Message?.Contains("duplicate") == true)
                    {
                        ModelState.AddModelError("ModuleName", "A competency module with this name already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                    }
                }
            }
            
            return View("AddCompetency", model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var db = new ApplicationDbContext();
            var competency = db.CompetencyModules.Find(id);
            
            if (competency == null)
            {
                TempData["ErrorMessage"] = "Competency module not found.";
                return RedirectToAction("Index");
            }
            
            return View("EditCompetency", competency);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CompetencyModule model)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                var competency = db.CompetencyModules.Find(model.Id);
                
                if (competency == null)
                {
                    TempData["ErrorMessage"] = "Competency module not found.";
                    return RedirectToAction("Index");
                }

                // Check for existing module with the same name (excluding current module)
                bool nameExists = db.CompetencyModules
                    .Any(c => c.ModuleName == model.ModuleName && c.Id != model.Id);
                
                if (nameExists)
                {
                    ModelState.AddModelError("ModuleName", "A competency module with this name already exists.");
                    return View("EditCompetency", model);
                }
                
                try
                {
                    // Update the competency properties
                    competency.ModuleName = model.ModuleName;
                    competency.Description = model.Description;
                    competency.ValidityMonths = model.ValidityMonths;
                    competency.IsMandatory = model.IsMandatory;
                    
                    db.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Competency module updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.InnerException?.Message?.Contains("unique constraint") == true ||
                        ex.InnerException?.InnerException?.Message?.Contains("duplicate") == true)
                    {
                        ModelState.AddModelError("ModuleName", "A competency module with this name already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                    }
                }
            }
            
            return View("EditCompetency", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var db = new ApplicationDbContext();
            var competency = db.CompetencyModules.Find(id);
            
            if (competency != null)
            {
                // Check if the competency is in use before deleting
                bool isInUse = db.UserCompetencies.Any(uc => uc.CompetencyModuleId == id);
                
                if (isInUse)
                {
                    TempData["ErrorMessage"] = "This competency cannot be deleted because it is assigned to one or more users.";
                }
                else
                {
                    db.CompetencyModules.Remove(competency);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Competency module deleted successfully.";
                }
            }
            
            return RedirectToAction("Index");
        }
    }
} 