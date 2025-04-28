using System;

namespace CLIP.Models
{
    public enum StageType
    {
        QuotationRequest = 1,
        EPR = 2,
        WorkExecution = 3
    }

    public class MonitoringStage
    {
        public int Id { get; set; }
        public int MonitoringScheduleId { get; set; }
        public StageType StageType { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string AssignedTo { get; set; }
        public string Comments { get; set; }
        public string DocumentReference { get; set; }

        // Navigation properties
        public MonitoringSchedule MonitoringSchedule { get; set; }
        public MonitoringResult MonitoringResult { get; set; }
    }
} 