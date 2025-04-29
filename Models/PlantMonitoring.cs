using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLIP.Models
{
    public class PlantMonitoring
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PlantID { get; set; }

        [Required]
        public int MonitoringID { get; set; }

        [StringLength(100)]
        public string Area { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpDate { get; set; }

        // Quotation Phase
        [Display(Name = "Quotation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? QuoteDate { get; set; }

        [Display(Name = "Quotation Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? QuoteSubmitDate { get; set; }

        [Display(Name = "Quotation Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? QuoteCompleteDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Quotation Assigned To")]
        public string QuoteUserAssign { get; set; }

        // Preparation Phase
        [Display(Name = "Preparation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EprDate { get; set; }

        [Display(Name = "Preparation Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EprSubmitDate { get; set; }

        [Display(Name = "Preparation Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EprCompleteDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Preparation Assigned To")]
        public string EprUserAssign { get; set; }

        // Work Execution Phase
        [Display(Name = "Work Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? WorkDate { get; set; }

        [Display(Name = "Work Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? WorkSubmitDate { get; set; }

        [Display(Name = "Work Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? WorkCompleteDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Work Assigned To")]
        public string WorkUserAssign { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        // Navigation properties
        [ForeignKey("PlantID")]
        public virtual Plant Plant { get; set; }

        [ForeignKey("MonitoringID")]
        public virtual Monitoring Monitoring { get; set; }

        // Helper properties for status determination
        [NotMapped]
        public string Status
        {
            get
            {
                if (WorkCompleteDate.HasValue)
                    return "Completed";
                else if (WorkDate.HasValue)
                    return "In Progress";
                else if (EprDate.HasValue)
                    return "In Preparation";
                else if (QuoteDate.HasValue)
                    return "In Quotation";
                else
                    return "Not Started";
            }
        }

        [NotMapped]
        public string StatusCssClass
        {
            get
            {
                switch (Status)
                {
                    case "Completed":
                        return "bg-success";
                    case "In Progress":
                        return "bg-warning";
                    case "In Preparation":
                        return "bg-warning";
                    case "In Quotation":
                        return "bg-secondary";
                    default:
                        return "";
                }
            }
        }
    }
} 