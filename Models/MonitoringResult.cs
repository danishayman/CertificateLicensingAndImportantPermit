using System;

namespace CLIP.Models
{
    public class MonitoringResult
    {
        public int Id { get; set; }
        public int MonitoringStageId { get; set; }
        public string ResultValue { get; set; } // Actual measurement/value from monitoring
        public string PassStatus { get; set; } // Pass, Fail, NA
        public string Findings { get; set; }
        public string ActionRequired { get; set; }
        public string CompletedBy { get; set; } // User ID or name
        public string AttachmentPath { get; set; } // Path to report/certificate file

        // Navigation property
        public MonitoringStage MonitoringStage { get; set; }
    }
} 