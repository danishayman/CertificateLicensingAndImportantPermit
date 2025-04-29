using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLIP.Models
{
    public class Monitoring
    {
        public Monitoring()
        {
            PlantMonitorings = new HashSet<PlantMonitoring>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MonitoringID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Monitoring Name")]
        public string MonitoringName { get; set; }

        [StringLength(100)]
        [Display(Name = "Category")]
        public string MonitoringCategory { get; set; }

        [StringLength(50)]
        [Display(Name = "Frequency")]
        public string MonitoringFreq { get; set; }

        // Navigation property
        public virtual ICollection<PlantMonitoring> PlantMonitorings { get; set; }
    }
} 