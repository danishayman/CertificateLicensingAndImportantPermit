using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLIP.Models
{
    public class UserPoint
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public int CompetencyModuleId { get; set; }
        
        public int? PointsEarned { get; set; }
        
        public int? PointsRemaining { get; set; }
        
        [Column(TypeName = "Date")]
        public DateTime? LastEvaluatedDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        [ForeignKey("CompetencyModuleId")]
        public virtual CompetencyModule CompetencyModule { get; set; }
    }
} 