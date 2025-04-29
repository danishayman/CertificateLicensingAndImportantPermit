using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CLIP.Models;
using Microsoft.AspNet.Identity;

namespace CLIP.Controllers
{
    [Authorize]
    public class CertificateOfFitnessController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CertificateOfFitness
        public ActionResult Index()
        {
            var certificates = db.CertificateOfFitness.Include(c => c.Plant);
            return View(certificates.ToList());
        }

        // GET: CertificateOfFitness/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertificateOfFitness certificateOfFitness = db.CertificateOfFitness.Include(c => c.Plant).FirstOrDefault(c => c.Id == id);
            if (certificateOfFitness == null)
            {
                return HttpNotFound();
            }
            return View(certificateOfFitness);
        }

        // GET: CertificateOfFitness/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.PlantId = new SelectList(db.Plants, "Id", "PlantName");
            return View();
        }

        // POST: CertificateOfFitness/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,PlantId,RegistrationNo,ExpiryDate")] CertificateOfFitness certificateOfFitness)
        {
            if (ModelState.IsValid)
            {
                db.CertificateOfFitness.Add(certificateOfFitness);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PlantId = new SelectList(db.Plants, "Id", "PlantName", certificateOfFitness.PlantId);
            return View(certificateOfFitness);
        }

        // GET: CertificateOfFitness/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertificateOfFitness certificateOfFitness = db.CertificateOfFitness.Find(id);
            if (certificateOfFitness == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlantId = new SelectList(db.Plants, "Id", "PlantName", certificateOfFitness.PlantId);
            return View(certificateOfFitness);
        }

        // POST: CertificateOfFitness/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,PlantId,RegistrationNo,ExpiryDate")] CertificateOfFitness certificateOfFitness)
        {
            if (ModelState.IsValid)
            {
                db.Entry(certificateOfFitness).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlantId = new SelectList(db.Plants, "Id", "PlantName", certificateOfFitness.PlantId);
            return View(certificateOfFitness);
        }

        // GET: CertificateOfFitness/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertificateOfFitness certificateOfFitness = db.CertificateOfFitness.Include(c => c.Plant).FirstOrDefault(c => c.Id == id);
            if (certificateOfFitness == null)
            {
                return HttpNotFound();
            }
            return View(certificateOfFitness);
        }

        // POST: CertificateOfFitness/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CertificateOfFitness certificateOfFitness = db.CertificateOfFitness.Find(id);
            db.CertificateOfFitness.Remove(certificateOfFitness);
            db.SaveChanges();
            return RedirectToAction("Index");
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