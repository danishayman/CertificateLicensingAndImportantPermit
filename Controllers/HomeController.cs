using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLIP.Models;

namespace CLIP.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Welcome()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize]
        public ActionResult Competency()
        {
            // Get list of all competency modules
            var db = new ApplicationDbContext();
            var competencyModules = db.CompetencyModules.ToList();
            
            return View(competencyModules);
        }

        [Authorize]
        public ActionResult AddCompetency()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompetency(CompetencyModule model)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                db.CompetencyModules.Add(model);
                db.SaveChanges();
                
                return RedirectToAction("Competency");
            }
            
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCompetency(int id)
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
            
            return RedirectToAction("Competency");
        }
    }
}