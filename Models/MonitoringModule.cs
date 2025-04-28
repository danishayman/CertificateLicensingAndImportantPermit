using System.ComponentModel.DataAnnotations;

namespace CLIP.Models
{
    public class MonitoringModule
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Type { get; set; }
    }
} 