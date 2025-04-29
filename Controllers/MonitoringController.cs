using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLIP.Models;
using CLIP.Models.DataAccess;

namespace CLIP.Controllers
{
    [Authorize]
    public class MonitoringController : Controller
    {
        private readonly MonitoringDataAccess _dataAccess = new MonitoringDataAccess();

        // GET: Monitoring
        public ActionResult Index()
        {
            var modules = _dataAccess.GetAllMonitoringModules();
            return View(modules);
        }

        // GET: Monitoring/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var monitoringModule = _dataAccess.GetMonitoringModuleById(id.Value);
            if (monitoringModule == null)
            {
                return HttpNotFound();
            }
            
            return View(monitoringModule);
        }

        // GET: Monitoring/Create
        public ActionResult Create()
        {
            // Create a dropdown list for monitoring types
            ViewBag.Types = new List<SelectListItem>
            {
                new SelectListItem { Value = "Periodic", Text = "Periodic" },
                new SelectListItem { Value = "Continuous", Text = "Continuous" }
            };
            return View();
        }

        // POST: Monitoring/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Category,Type")] MonitoringModule monitoringModule)
        {
            if (ModelState.IsValid)
            {
                int newId = _dataAccess.CreateMonitoringModule(monitoringModule);
                monitoringModule.Id = newId;
                return RedirectToAction("Index");
            }

            // Recreate dropdown list if validation fails
            ViewBag.Types = new List<SelectListItem>
            {
                new SelectListItem { Value = "Periodic", Text = "Periodic" },
                new SelectListItem { Value = "Continuous", Text = "Continuous" }
            };
            return View(monitoringModule);
        }

        // GET: Monitoring/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var monitoringModule = _dataAccess.GetMonitoringModuleById(id.Value);
            if (monitoringModule == null)
            {
                return HttpNotFound();
            }

            // Create a dropdown list for monitoring types
            ViewBag.Types = new List<SelectListItem>
            {
                new SelectListItem { Value = "Periodic", Text = "Periodic" },
                new SelectListItem { Value = "Continuous", Text = "Continuous" }
            };
            return View(monitoringModule);
        }

        // POST: Monitoring/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Category,Type")] MonitoringModule monitoringModule)
        {
            if (ModelState.IsValid)
            {
                _dataAccess.UpdateMonitoringModule(monitoringModule);
                return RedirectToAction("Index");
            }

            // Recreate dropdown list if validation fails
            ViewBag.Types = new List<SelectListItem>
            {
                new SelectListItem { Value = "Periodic", Text = "Periodic" },
                new SelectListItem { Value = "Continuous", Text = "Continuous" }
            };
            return View(monitoringModule);
        }

        // GET: Monitoring/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var monitoringModule = _dataAccess.GetMonitoringModuleById(id.Value);
            if (monitoringModule == null)
            {
                return HttpNotFound();
            }
            
            return View(monitoringModule);
        }

        // POST: Monitoring/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _dataAccess.DeleteMonitoringModule(id);
            return RedirectToAction("Index");
        }
    }
} 