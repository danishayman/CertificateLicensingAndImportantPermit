using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIP.Models;

namespace CLIP.Controllers
{
    [Authorize]
    public class MonitoringController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Monitoring
        public ActionResult Index()
        {
            return View(db.Monitorings.ToList());
        }

        // GET: Monitoring/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitoring monitoring = db.Monitorings.Find(id);
            if (monitoring == null)
            {
                return HttpNotFound();
            }
            return View(monitoring);
        }

        // GET: Monitoring/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Monitoring/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "MonitoringID,MonitoringName,MonitoringCategory,MonitoringFreq")] Monitoring monitoring)
        {
            // Validate custom frequency
            if (monitoring.MonitoringFreq < 1 || monitoring.MonitoringFreq > 120)
            {
                ModelState.AddModelError("MonitoringFreq", "Frequency must be between 1 and 120 months.");
            }

            if (ModelState.IsValid)
            {
                db.Monitorings.Add(monitoring);
                db.SaveChanges();
                
                TempData["SuccessMessage"] = "Monitoring type created successfully.";
                return RedirectToAction("Index");
            }

            PopulateDropDownLists();
            return View(monitoring);
        }

        // GET: Monitoring/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitoring monitoring = db.Monitorings.Find(id);
            if (monitoring == null)
            {
                return HttpNotFound();
            }
            
            PopulateDropDownLists();
            return View(monitoring);
        }

        // POST: Monitoring/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "MonitoringID,MonitoringName,MonitoringCategory,MonitoringFreq")] Monitoring monitoring)
        {
            // Validate custom frequency
            if (monitoring.MonitoringFreq < 1 || monitoring.MonitoringFreq > 120)
            {
                ModelState.AddModelError("MonitoringFreq", "Frequency must be between 1 and 120 months.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(monitoring).State = EntityState.Modified;
                db.SaveChanges();
                
                TempData["SuccessMessage"] = "Monitoring type updated successfully.";
                return RedirectToAction("Index");
            }
            
            PopulateDropDownLists();
            return View(monitoring);
        }

        // GET: Monitoring/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitoring monitoring = db.Monitorings.Find(id);
            if (monitoring == null)
            {
                return HttpNotFound();
            }
            return View(monitoring);
        }

        // POST: Monitoring/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Monitoring monitoring = db.Monitorings.Find(id);
            
            // Check if this monitoring type is in use
            bool isInUse = db.PlantMonitorings.Any(pm => pm.MonitoringID == id);
            
            if (isInUse)
            {
                TempData["ErrorMessage"] = "This monitoring type cannot be deleted because it is in use.";
                return RedirectToAction("Index");
            }
            
            db.Monitorings.Remove(monitoring);
            db.SaveChanges();
            
            TempData["SuccessMessage"] = "Monitoring type deleted successfully.";
            return RedirectToAction("Index");
        }

        private void PopulateDropDownLists()
        {
            // Categories
            var categoryList = new List<SelectListItem>
            {
                new SelectListItem { Text = "-- Select Category --", Value = "" },
                new SelectListItem { Text = "Environment", Value = "Environment" },
                new SelectListItem { Text = "Health & Safety", Value = "Health & Safety" }
            };
            ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text");
            
            // Frequencies in months - key presets with custom option
            var frequencyList = new List<SelectListItem>
            {
                new SelectListItem { Text = "-- Select Frequency --", Value = "" },
                new SelectListItem { Text = "Monthly (1)", Value = "1" },
                new SelectListItem { Text = "Quarterly (3)", Value = "3" },
                new SelectListItem { Text = "Half-Yearly (6)", Value = "6" },
                new SelectListItem { Text = "Yearly (12)", Value = "12" },
                new SelectListItem { Text = "Every 2 Years (24)", Value = "24" },
                new SelectListItem { Text = "Every 3 Years (36)", Value = "36" },
                new SelectListItem { Text = "Custom...", Value = "custom" }
            };
            ViewBag.FrequencyList = new SelectList(frequencyList, "Value", "Text");
            
            // Whether we're editing or creating a new record
            ViewBag.IsEdit = ControllerContext.RouteData.Values["action"].ToString() == "Edit";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 