using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using CLIP.Models;

namespace CLIP
{
    public class MvcApplication : System.Web.HttpApplication
    {
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
