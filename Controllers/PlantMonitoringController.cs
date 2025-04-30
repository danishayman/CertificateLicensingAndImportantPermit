using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIP.Models;
using System.Globalization;
using System.IO;
using Microsoft.AspNet.Identity;

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

            // Filter by user's plants if user is not admin
            if (!User.IsInRole("Admin"))
            {
                var userId = User.Identity.GetUserId();
                var userPlantIds = db.UserPlants
                    .Where(up => up.UserId == userId)
                    .Select(up => up.PlantId)
                    .ToList();
                
                query = query.Where(p => userPlantIds.Contains(p.PlantID));
            }
            
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
            // If user is admin, get all plants, otherwise get only the user's plants
            if (User.IsInRole("Admin"))
            {
                ViewBag.Plants = db.Plants.OrderBy(p => p.PlantName).ToList();
            }
            else
            {
                var userId = User.Identity.GetUserId();
                ViewBag.Plants = db.UserPlants
                    .Where(up => up.UserId == userId)
                    .Select(up => up.Plant)
                    .OrderBy(p => p.PlantName)
                    .ToList();
            }
            
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

            // Get plants based on user role
            var plants = new List<Plant>();
            if (User.IsInRole("Admin"))
            {
                plants = db.Plants.OrderBy(p => p.PlantName).ToList();
            }
            else
            {
                var userId = User.Identity.GetUserId();
                plants = db.UserPlants
                    .Where(up => up.UserId == userId)
                    .Select(up => up.Plant)
                    .OrderBy(p => p.PlantName)
                    .ToList();
            }

            // Get plant monitoring records based on user role
            var plantMonitoringsQuery = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = User.Identity.GetUserId();
                var userPlantIds = db.UserPlants
                    .Where(up => up.UserId == userId)
                    .Select(up => up.PlantId)
                    .ToList();
                
                plantMonitoringsQuery = plantMonitoringsQuery.Where(p => userPlantIds.Contains(p.PlantID));
            }

            var plantMonitorings = plantMonitoringsQuery.ToList();

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
            ViewBag.IsAdmin = User.IsInRole("Admin");

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

            // Check if user has access to this plant monitoring record
            if (!User.IsInRole("Admin"))
            {
                var userId = User.Identity.GetUserId();
                var userHasAccessToPlant = db.UserPlants
                    .Any(up => up.UserId == userId && up.PlantId == plantMonitoring.PlantID);
                
                if (!userHasAccessToPlant)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(plantMonitoring);
        }

        // GET: PlantMonitoring/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p => p.PlantName), "Id", "PlantName");
            ViewBag.MonitoringID = new SelectList(db.Monitorings.OrderBy(m => m.MonitoringName), "MonitoringID", "MonitoringName");
            return View();
        }

        // POST: PlantMonitoring/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

            // Check if user has access to this plant monitoring record
            if (!User.IsInRole("Admin"))
            {
                var userId = User.Identity.GetUserId();
                var userHasAccessToPlant = db.UserPlants
                    .Any(up => up.UserId == userId && up.PlantId == plantMonitoring.PlantID);
                
                if (!userHasAccessToPlant)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(plantMonitoring);
        }

        // POST: PlantMonitoring/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, PlantMonitoring model, HttpPostedFileBase quoteDocument, HttpPostedFileBase eprDocument, HttpPostedFileBase workDocument)
        {
            if (ModelState.IsValid)
            {
                var plantMonitoring = db.PlantMonitorings.Find(id);
                if (plantMonitoring == null)
                {
                    return HttpNotFound();
                }

                // Check if user has access to this plant monitoring record
                if (!User.IsInRole("Admin"))
                {
                    var userId = User.Identity.GetUserId();
                    var userHasAccessToPlant = db.UserPlants
                        .Any(up => up.UserId == userId && up.PlantId == plantMonitoring.PlantID);
                    
                    if (!userHasAccessToPlant)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }
                }

                // Update properties
                plantMonitoring.ExpDate = model.ExpDate;
                plantMonitoring.Remarks = model.Remarks;
                
                // Quotation Phase
                plantMonitoring.QuoteDate = model.QuoteDate;
                plantMonitoring.QuoteSubmitDate = model.QuoteSubmitDate;
                plantMonitoring.QuoteCompleteDate = model.QuoteCompleteDate;
                plantMonitoring.QuoteUserAssign = model.QuoteUserAssign;
                
                // Preparation Phase
                plantMonitoring.EprDate = model.EprDate;
                plantMonitoring.EprSubmitDate = model.EprSubmitDate;
                plantMonitoring.EprCompleteDate = model.EprCompleteDate;
                plantMonitoring.EprUserAssign = model.EprUserAssign;
                
                // Work Execution Phase
                plantMonitoring.WorkDate = model.WorkDate;
                plantMonitoring.WorkSubmitDate = model.WorkSubmitDate;
                plantMonitoring.WorkCompleteDate = model.WorkCompleteDate;
                plantMonitoring.WorkUserAssign = model.WorkUserAssign;

                // Handle file uploads
                if (quoteDocument != null && quoteDocument.ContentLength > 0)
                {
                    // Check file size (20MB limit)
                    if (quoteDocument.ContentLength > 20 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "The quote document exceeds the maximum file size of 20MB.");
                        var quoteDocumentValidationEntity = db.PlantMonitorings
                            .Include(p => p.Plant)
                            .Include(p => p.Monitoring)
                            .FirstOrDefault(p => p.Id == id);
                        
                        if (quoteDocumentValidationEntity != null)
                        {
                            model.Plant = quoteDocumentValidationEntity.Plant;
                            model.Monitoring = quoteDocumentValidationEntity.Monitoring;
                        }
                        ViewBag.IsAdmin = User.IsInRole("Admin");
                        return View(model);
                    }

                    string fileName = Path.GetFileName(quoteDocument.FileName);
                    string uniqueFileName = $"Quote_{id}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{fileName}";
                    string path = Path.Combine(Server.MapPath("~/uploads/Monitoring"), uniqueFileName);
                    
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    quoteDocument.SaveAs(path);
                    plantMonitoring.QuoteDoc = $"~/uploads/Monitoring/{uniqueFileName}";
                }

                if (eprDocument != null && eprDocument.ContentLength > 0)
                {
                    // Check file size (20MB limit)
                    if (eprDocument.ContentLength > 20 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "The EPR document exceeds the maximum file size of 20MB.");
                        var eprDocumentValidationEntity = db.PlantMonitorings
                            .Include(p => p.Plant)
                            .Include(p => p.Monitoring)
                            .FirstOrDefault(p => p.Id == id);
                        
                        if (eprDocumentValidationEntity != null)
                        {
                            model.Plant = eprDocumentValidationEntity.Plant;
                            model.Monitoring = eprDocumentValidationEntity.Monitoring;
                        }
                        ViewBag.IsAdmin = User.IsInRole("Admin");
                        return View(model);
                    }

                    string fileName = Path.GetFileName(eprDocument.FileName);
                    string uniqueFileName = $"EPR_{id}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{fileName}";
                    string path = Path.Combine(Server.MapPath("~/uploads/Monitoring"), uniqueFileName);
                    
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    eprDocument.SaveAs(path);
                    plantMonitoring.EprDoc = $"~/uploads/Monitoring/{uniqueFileName}";
                }

                if (workDocument != null && workDocument.ContentLength > 0)
                {
                    // Check file size (20MB limit)
                    if (workDocument.ContentLength > 20 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "The work document exceeds the maximum file size of 20MB.");
                        var workDocumentValidationEntity = db.PlantMonitorings
                            .Include(p => p.Plant)
                            .Include(p => p.Monitoring)
                            .FirstOrDefault(p => p.Id == id);
                        
                        if (workDocumentValidationEntity != null)
                        {
                            model.Plant = workDocumentValidationEntity.Plant;
                            model.Monitoring = workDocumentValidationEntity.Monitoring;
                        }
                        ViewBag.IsAdmin = User.IsInRole("Admin");
                        return View(model);
                    }

                    string fileName = Path.GetFileName(workDocument.FileName);
                    string uniqueFileName = $"Work_{id}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{fileName}";
                    string path = Path.Combine(Server.MapPath("~/uploads/Monitoring"), uniqueFileName);
                    
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    workDocument.SaveAs(path);
                    plantMonitoring.WorkDoc = $"~/uploads/Monitoring/{uniqueFileName}";
                }

                db.Entry(plantMonitoring).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = plantMonitoring.Id });
            }

            // If we got this far, something failed, redisplay form
            var plantMonitoringFromDb = db.PlantMonitorings
                .Include(p => p.Plant)
                .Include(p => p.Monitoring)
                .FirstOrDefault(p => p.Id == id);

            if (plantMonitoringFromDb == null)
            {
                return HttpNotFound();
            }

            // Copy document paths back from DB entity to model to preserve them
            model.QuoteDoc = plantMonitoringFromDb.QuoteDoc;
            model.EprDoc = plantMonitoringFromDb.EprDoc;
            model.WorkDoc = plantMonitoringFromDb.WorkDoc;
            
            // Set navigation properties
            model.Plant = plantMonitoringFromDb.Plant;
            model.Monitoring = plantMonitoringFromDb.Monitoring;

            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(model);
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