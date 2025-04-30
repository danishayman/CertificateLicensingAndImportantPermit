using System;
using System.Collections.Generic;
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
        public ActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Monitoring/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MonitoringID,MonitoringName,MonitoringCategory,MonitoringFreq")] Monitoring monitoring)
        {
            if (ModelState.IsValid)
            {
                db.Monitorings.Add(monitoring);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateDropDownLists();
            return View(monitoring);
        }

        // GET: Monitoring/Edit/5
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
        public ActionResult Edit([Bind(Include = "MonitoringID,MonitoringName,MonitoringCategory,MonitoringFreq")] Monitoring monitoring)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitoring).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            PopulateDropDownLists();
            return View(monitoring);
        }

        // GET: Monitoring/Delete/5
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
            
            // Frequencies
            var frequencyList = new List<SelectListItem>
            {
                new SelectListItem { Text = "-- Select Frequency --", Value = "" },
                new SelectListItem { Text = "Yearly", Value = "Yearly" },
                new SelectListItem { Text = "Every 3 Years", Value = "Every 3 Years" },
                new SelectListItem { Text = "Half-Yearly", Value = "Half-Yearly" },
                new SelectListItem { Text = "Quarterly", Value = "Quarterly" },
                new SelectListItem { Text = "Monthly", Value = "Monthly" }
            };
            ViewBag.FrequencyList = new SelectList(frequencyList, "Value", "Text");
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