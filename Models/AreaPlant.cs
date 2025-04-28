using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLIP.Models
{
    public class AreaPlant
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AreaName { get; set; }
        
        public int PlantId { get; set; }
        
        // Navigation property
        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }
    }
} 