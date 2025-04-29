using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CLIP.Models;
using CLIP.Models.DataAccess;

namespace CLIP.ViewModels
{
    public class MonitoringCategoryViewModel
    {
        public MonitoringModule MonitoringModule { get; set; }
        public List<MonitoringSchedule> Schedules { get; set; }
        
        // Dashboard statistics
        public int TotalSchedules { get; set; }
        public int CompletedSchedules { get; set; }
        public int InProgressSchedules { get; set; }
        public int NotStartedSchedules { get; set; }
        public int OverdueSchedules { get; set; }
        
        public Dictionary<string, MonitoringDataAccess.StageStats> StageProgress { get; set; }
    }

    public class MonitoringScheduleDetailsViewModel
    {
        public MonitoringSchedule Schedule { get; set; }
        public List<MonitoringStage> Stages { get; set; }
    }

    public class MonitoringScheduleEditViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public int MonitoringModuleId { get; set; }
        
        public string ModuleName { get; set; }
        
        [Required]
        [Display(Name = "Plant")]
        public int PlantId { get; set; }
        
        [Display(Name = "Area")]
        public int? AreaId { get; set; }
        
        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; }
        
        [Required]
        [Display(Name = "Month")]
        [Range(1, 12)]
        public int ScheduledMonth { get; set; }
        
        [Required]
        [Display(Name = "Year")]
        [Range(2020, 2050)]
        public int ScheduledYear { get; set; }
        
        [Required]
        [Display(Name = "Status")]
        public string OverallStatus { get; set; }
        
        [Required]
        [Display(Name = "Next Due Date")]
        [DataType(DataType.Date)]
        public DateTime NextDueDate { get; set; }
        
        [Required]
        [Display(Name = "Responsible Person")]
        public string ResponsiblePerson { get; set; }
        
        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        
        // Dropdown lists for UI
        public List<SelectListItem> PlantList { get; set; }
        public List<SelectListItem> AreaList { get; set; }
        public List<SelectListItem> FrequencyOptions { get; set; }
    }

    public class MonitoringStageEditViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public int MonitoringScheduleId { get; set; }
        
        [Required]
        [Display(Name = "Stage Type")]
        public StageType StageType { get; set; }
        
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }
        
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = "Completion Date")]
        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }
        
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        
        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        
        [Display(Name = "Document Reference")]
        public string DocumentReference { get; set; }
        
        // For UI display
        public string ScheduleName { get; set; }
        public List<SelectListItem> StatusOptions { get; set; }
    }

    public class MonitoringResultEditViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public int MonitoringStageId { get; set; }
        
        [Display(Name = "Result Value")]
        public string ResultValue { get; set; }
        
        [Required]
        [Display(Name = "Pass Status")]
        public string PassStatus { get; set; }
        
        [Display(Name = "Findings")]
        [DataType(DataType.MultilineText)]
        public string Findings { get; set; }
        
        [Display(Name = "Action Required")]
        [DataType(DataType.MultilineText)]
        public string ActionRequired { get; set; }
        
        [Required]
        [Display(Name = "Completed By")]
        public string CompletedBy { get; set; }
        
        [Display(Name = "Attachment")]
        public string AttachmentPath { get; set; }
        
        // For UI display
        public string StageName { get; set; }
        public List<SelectListItem> PassStatusOptions { get; set; }
    }

    public class MonitoringScheduleViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Required]
        [Display(Name = "Plant")]
        public int PlantId { get; set; }
        public string PlantName { get; set; }

        [Required]
        [Display(Name = "Area")]
        public int AreaId { get; set; }
        public string AreaName { get; set; }

        [Required]
        [Display(Name = "Scheduled Date")]
        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; }

        [Display(Name = "Completed Date")]
        [DataType(DataType.Date)]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; }

        public SelectList PlantSelectList { get; set; }
        public SelectList AreaSelectList { get; set; }
        public SelectList UserSelectList { get; set; }
    }

    public class MonitoringResultViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Schedule")]
        public int ScheduleId { get; set; }

        [Required]
        [Display(Name = "Monitoring Date")]
        [DataType(DataType.Date)]
        public DateTime MonitoringDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Findings")]
        [DataType(DataType.MultilineText)]
        public string Findings { get; set; }

        [Display(Name = "Actions")]
        [DataType(DataType.MultilineText)]
        public string Actions { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Responsible Person")]
        public string ResponsiblePersonId { get; set; }
        public string ResponsiblePersonName { get; set; }

        [Display(Name = "Attachments")]
        public List<MonitoringAttachmentViewModel> Attachments { get; set; }

        public SelectList UserSelectList { get; set; }
        public MonitoringScheduleViewModel Schedule { get; set; }
    }

    public class MonitoringAttachmentViewModel
    {
        public int Id { get; set; }
        public int ResultId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; }
    }

    public class MonitoringDashboardViewModel
    {
        public int TotalScheduled { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalOverdue { get; set; }
        public int TotalPending { get; set; }
        
        public List<MonitoringScheduleViewModel> UpcomingSchedules { get; set; }
        public List<MonitoringResultViewModel> RecentResults { get; set; }
        
        public Dictionary<string, int> StatusDistribution { get; set; }
        public Dictionary<string, int> AreaDistribution { get; set; }
        public Dictionary<string, int> PlantDistribution { get; set; }
    }
} 