using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using CLIP.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CLIP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Timer _statusUpdateTimer = null;
        private static readonly TimeSpan _updateInterval = TimeSpan.FromHours(24);
        private static readonly object _lockObject = new object();
        private static bool _isUpdating = false;

        protected void Application_Start()
        {
            // Disable database migration check
            Database.SetInitializer<ApplicationDbContext>(null);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Seed sample plants if they don't exist
            SeedSamplePlants();
            
            // Update Certificate statuses immediately at startup
            UpdateCertificateStatuses();
            
            // Set up timer to update certificate statuses daily
            _statusUpdateTimer = new Timer(UpdateCertificateStatusesCallback, null, 
                _updateInterval, _updateInterval);
        }
        
        private void UpdateCertificateStatusesCallback(object state)
        {
            if (_isUpdating) return;
            
            try
            {
                lock(_lockObject)
                {
                    if (_isUpdating) return;
                    _isUpdating = true;
                    
                    UpdateCertificateStatuses();
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }
        
        private void UpdateCertificateStatuses()
        {
            using (var context = new ApplicationDbContext())
            {
                var certificates = context.CertificateOfFitness.ToList();
                bool changesDetected = false;
                
                foreach (var certificate in certificates)
                {
                    string newStatus = CalculateStatus(certificate.ExpiryDate);
                    if (certificate.Status != newStatus)
                    {
                        certificate.Status = newStatus;
                        changesDetected = true;
                    }
                }
                
                if (changesDetected)
                {
                    context.SaveChanges();
                }
            }
        }
        
        private string CalculateStatus(DateTime expiryDate)
        {
            DateTime today = DateTime.Today;
            DateTime expiringSoonDate = expiryDate.AddDays(-60);

            if (today > expiryDate)
                return "Expired";
            else if (today >= expiringSoonDate)
                return "Expiring Soon";
            else
                return "Active";
        }
        
        private void SeedSamplePlants()
        {
            using (var context = new ApplicationDbContext())
            {
                if (!context.Plants.Any())
                {
                    // Sample plants
                    var plants = new List<Plant>
                    {
                        new Plant { PlantName = "Plant 1" },
                        new Plant { PlantName = "Plant 13" },
                        new Plant { PlantName = "Plant 3" },
                        new Plant { PlantName = "Plant 21" },
                        new Plant { PlantName = "Plant 34" },
                        new Plant { PlantName = "Plant 5" },
                        new Plant { PlantName = "Plant 55" }
                    };
                    
                    plants.ForEach(p => context.Plants.Add(p));
                    context.SaveChanges();
                }
            }
        }
    }
}
