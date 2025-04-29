using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLIP.Models
{
    public class CertificateOfFitness
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Plant")]
        public int PlantId { get; set; }

        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNo { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Machine Name")]
        public string MachineName { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }
    }
} 