using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLIP.Models
{
    public class CompetencyModule
    {
        public CompetencyModule()
        {
            UserCompetencies = new HashSet<UserCompetency>();
        }

        public int Id { get; set; }
        
        [Required]
        [Index(IsUnique = true)]
        public string ModuleName { get; set; }
        
        public string Description { get; set; }
        
        public int? ValidityMonths { get; set; }
        
        public bool IsMandatory { get; set; }

        // Navigation property
        public virtual ICollection<UserCompetency> UserCompetencies { get; set; }
    }
} 