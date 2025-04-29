using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIP.Models;
using System.Globalization;

namespace CLIP.Controllers
{
    [Authorize]
    public class PlantMonitoringController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PlantMonitoring
        public ActionResult Index(string category = null, int? plantId = null, string status = null)
        {
            // Get all plant monitoring items with plant and monitoring details
            var query = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Monitoring.MonitoringCategory == category);
                ViewBag.SelectedCategory = category;
            }

            if (plantId.HasValue)
            {
                query = query.Where(p => p.PlantID == plantId.Value);
                ViewBag.SelectedPlantId = plantId.Value;
            }

            if (!string.IsNullOrEmpty(status))
            {
                // Filter by status
                if (status == "Completed")
                {
                    query = query.Where(p => p.WorkCompleteDate != null);
                }
                else if (status == "In Progress")
                {
                    query = query.Where(p => p.WorkDate != null && p.WorkCompleteDate == null);
                }
                else if (status == "In Preparation")
                {
                    query = query.Where(p => p.EprDate != null && p.WorkDate == null);
                }
                else if (status == "In Quotation")
                {
                    query = query.Where(p => p.QuoteDate != null && p.EprDate == null);
                }
                else if (status == "Not Started")
                {
                    query = query.Where(p => p.QuoteDate == null);
                }
                else if (status == "Expiring Soon")
                {
                    var thirtyDaysFromNow = DateTime.Now.AddDays(30);
                    query = query.Where(p => p.ExpDate != null && p.ExpDate <= thirtyDaysFromNow && p.ExpDate > DateTime.Now);
                }
                else if (status == "Expired")
                {
                    query = query.Where(p => p.ExpDate != null && p.ExpDate < DateTime.Now);
                }
                
                ViewBag.SelectedStatus = status;
            }

            // Load plants and monitoring categories for filtering
            ViewBag.Plants = db.Plants.OrderBy(p => p.PlantName).ToList();
            ViewBag.Categories = db.Monitorings
                .Select(m => m.MonitoringCategory)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewBag.StatusList = new List<string>
            {
                "All",
                "Completed",
                "In Progress",
                "In Preparation",
                "In Quotation",
                "Not Started",
                "Expiring Soon",
                "Expired"
            };

            // Group by Month for schedule view
            var currentYear = DateTime.Now.Year;
            var result = query.ToList();

            return View(result);
        }

        // GET: PlantMonitoring/Schedule
        public ActionResult Schedule()
        {
            // Get all monitoring types
            var monitoringTypes = db.Monitorings
                .OrderBy(m => m.MonitoringCategory)
                .ThenBy(m => m.MonitoringName)
                .ToList();

            // Get all plants
            var plants = db.Plants.OrderBy(p => p.PlantName).ToList();

            // Get all plant monitoring records
            var plantMonitorings = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .ToList();

            // Create a dictionary to group by monitoring type and plant
            var currentYear = DateTime.Now.Year;
            var data = new Dictionary<int, Dictionary<int, List<PlantMonitoringViewModel>>>();

            foreach (var pm in plantMonitorings)
            {
                if (!data.ContainsKey(pm.MonitoringID))
                {
                    data[pm.MonitoringID] = new Dictionary<int, List<PlantMonitoringViewModel>>();
                }

                if (!data[pm.MonitoringID].ContainsKey(pm.PlantID))
                {
                    data[pm.MonitoringID][pm.PlantID] = new List<PlantMonitoringViewModel>();
                }

                var viewModel = new PlantMonitoringViewModel
                {
                    Id = pm.Id,
                    Area = pm.Area,
                    Status = pm.Status,
                    ExpDate = pm.ExpDate,
                    QuoteDate = pm.QuoteDate,
                    WorkDate = pm.WorkDate,
                    WorkCompleteDate = pm.WorkCompleteDate
                };

                data[pm.MonitoringID][pm.PlantID].Add(viewModel);
            }

            ViewBag.MonitoringTypes = monitoringTypes;
            ViewBag.Plants = plants;
            ViewBag.Data = data;
            ViewBag.CurrentYear = currentYear;
            ViewBag.MonthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            return View();
        }

        // GET: PlantMonitoring/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantMonitoring plantMonitoring = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .FirstOrDefault(p => p.Id == id);

            if (plantMonitoring == null)
            {
                return HttpNotFound();
            }
            return View(plantMonitoring);
        }

        // GET: PlantMonitoring/Create
        public ActionResult Create()
        {
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p => p.PlantName), "Id", "PlantName");
            ViewBag.MonitoringID = new SelectList(db.Monitorings.OrderBy(m => m.MonitoringName), "MonitoringID", "MonitoringName");
            return View();
        }

        // POST: PlantMonitoring/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlantMonitoring plantMonitoring)
        {
            if (ModelState.IsValid)
            {
                db.PlantMonitorings.Add(plantMonitoring);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p => p.PlantName), "Id", "PlantName", plantMonitoring.PlantID);
            ViewBag.MonitoringID = new SelectList(db.Monitorings.OrderBy(m => m.MonitoringName), "MonitoringID", "MonitoringName", plantMonitoring.MonitoringID);
            return View(plantMonitoring);
        }

        // GET: PlantMonitoring/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantMonitoring plantMonitoring = db.PlantMonitorings.Find(id);
            if (plantMonitoring == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p => p.PlantName), "Id", "PlantName", plantMonitoring.PlantID);
            ViewBag.MonitoringID = new SelectList(db.Monitorings.OrderBy(m => m.MonitoringName), "MonitoringID", "MonitoringName", plantMonitoring.MonitoringID);
            return View(plantMonitoring);
        }

        // POST: PlantMonitoring/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlantMonitoring plantMonitoring)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plantMonitoring).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p => p.PlantName), "Id", "PlantName", plantMonitoring.PlantID);
            ViewBag.MonitoringID = new SelectList(db.Monitorings.OrderBy(m => m.MonitoringName), "MonitoringID", "MonitoringName", plantMonitoring.MonitoringID);
            return View(plantMonitoring);
        }

        // GET: PlantMonitoring/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantMonitoring plantMonitoring = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .FirstOrDefault(p => p.Id == id);
                
            if (plantMonitoring == null)
            {
                return HttpNotFound();
            }
            return View(plantMonitoring);
        }

        // POST: PlantMonitoring/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlantMonitoring plantMonitoring = db.PlantMonitorings.Find(id);
            db.PlantMonitorings.Remove(plantMonitoring);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: PlantMonitoring/UpdateStatus/5
        public ActionResult UpdateStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantMonitoring plantMonitoring = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .FirstOrDefault(p => p.Id == id);
                
            if (plantMonitoring == null)
            {
                return HttpNotFound();
            }
            return View(plantMonitoring);
        }

        // POST: PlantMonitoring/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, PlantMonitoring model)
        {
            var plantMonitoring = db.PlantMonitorings.Find(id);
            if (plantMonitoring == null)
            {
                return HttpNotFound();
            }

            // Update status fields
            plantMonitoring.QuoteDate = model.QuoteDate;
            plantMonitoring.QuoteSubmitDate = model.QuoteSubmitDate;
            plantMonitoring.QuoteCompleteDate = model.QuoteCompleteDate;
            plantMonitoring.QuoteUserAssign = model.QuoteUserAssign;
            
            plantMonitoring.EprDate = model.EprDate;
            plantMonitoring.EprSubmitDate = model.EprSubmitDate;
            plantMonitoring.EprCompleteDate = model.EprCompleteDate;
            plantMonitoring.EprUserAssign = model.EprUserAssign;
            
            plantMonitoring.WorkDate = model.WorkDate;
            plantMonitoring.WorkSubmitDate = model.WorkSubmitDate;
            plantMonitoring.WorkCompleteDate = model.WorkCompleteDate;
            plantMonitoring.WorkUserAssign = model.WorkUserAssign;
            
            plantMonitoring.ExpDate = model.ExpDate;
            plantMonitoring.Remarks = model.Remarks;

            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
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

    // View Model for Schedule view
    public class PlantMonitoringViewModel
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public DateTime? ExpDate { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? WorkDate { get; set; }
        public DateTime? WorkCompleteDate { get; set; }
    }
} 