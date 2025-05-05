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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ExpDate { get; set; }

        // Quotation Phase
        [Display(Name = "Quotation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? QuoteDate { get; set; }

        [Display(Name = "Quotation Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? QuoteSubmitDate { get; set; }

        [Display(Name = "Quotation Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? QuoteCompleteDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Quotation Assigned To")]
        public string QuoteUserAssign { get; set; }

        [StringLength(500)]
        [Display(Name = "Quotation Document")]
        public string QuoteDoc { get; set; }

        // Preparation Phase
        [Display(Name = "Preparation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? EprDate { get; set; }

        [Display(Name = "Preparation Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? EprSubmitDate { get; set; }

        [Display(Name = "Preparation Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? EprCompleteDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Preparation Assigned To")]
        public string EprUserAssign { get; set; }

        [StringLength(500)]
        [Display(Name = "ePR Document")]
        public string EprDoc { get; set; }

        // Work Execution Phase
        [Display(Name = "Work Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? WorkDate { get; set; }

        [Display(Name = "Work Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? WorkSubmitDate { get; set; }

        [Display(Name = "Work Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? WorkCompleteDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Work Assigned To")]
        public string WorkUserAssign { get; set; }

        [StringLength(500)]
        [Display(Name = "Work Document")]
        public string WorkDoc { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [StringLength(50)]
        [Display(Name = "Process Status")]
        public string ProcStatus { get; set; }

        [StringLength(50)]
        [Display(Name = "Expiration Status")]
        public string ExpStatus { get; set; }

        // Navigation properties
        [ForeignKey("PlantID")]
        public virtual Plant Plant { get; set; }

        [ForeignKey("MonitoringID")]
        public virtual Monitoring Monitoring { get; set; }

        // Helper method to calculate process status
        public void CalculateProcStatus()
        {
            if (WorkCompleteDate.HasValue)
                ProcStatus = "Completed";
            else if (WorkDate.HasValue)
                ProcStatus = "Work In Progress";
            else if (EprDate.HasValue)
                ProcStatus = "ePR Raised";
            else if (QuoteDate.HasValue)
                ProcStatus = "Quotation Requested";
            else
                ProcStatus = "Not Started";
        }

        // Helper method to calculate expiration status
        public void CalculateExpStatus()
        {
            if (!ExpDate.HasValue)
                ExpStatus = "No Expiry";
            else if (ExpDate < DateTime.Now)
                ExpStatus = "Expired";
            else if (ExpDate < DateTime.Now.AddDays(30))
                ExpStatus = "Expiring Soon";
            else
                ExpStatus = "Valid";
        }

        // Helper method to calculate both statuses
        public void CalculateStatuses()
        {
            CalculateProcStatus();
            CalculateExpStatus();
        }

        [NotMapped]
        public string ProcStatusCssClass
        {
            get
            {
                switch (ProcStatus)
                {
                    case "Completed":
                        return "bg-success";
                    case "Work In Progress":
                        return "bg-warning";
                    case "ePR Raised":
                        return "bg-warning";
                    case "Quotation Requested":
                        return "bg-secondary";
                    default:
                        return "";
                }
            }
        }

        [NotMapped]
        public string ExpStatusCssClass
        {
            get
            {
                switch (ExpStatus)
                {
                    case "Expired":
                        return "bg-danger";
                    case "Expiring Soon":
                        return "bg-warning";
                    case "Valid":
                        return "bg-success";
                    case "No Expiry":
                        return "bg-secondary";
                    default:
                        return "";
                }
            }
        }
    }
} 