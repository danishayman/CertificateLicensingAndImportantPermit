using System;
using System.Collections.Generic;

namespace CLIP.Models
{
    public class MonitoringSchedule
    {
        public MonitoringSchedule()
        {
            MonitoringStages = new List<MonitoringStage>();
        }

        public int Id { get; set; }
        public int MonitoringModuleId { get; set; }
        public int PlantId { get; set; }
        public int? AreaId { get; set; }
        public string Frequency { get; set; }
        public int ScheduledMonth { get; set; }
        public int ScheduledYear { get; set; }
        public string OverallStatus { get; set; }
        public DateTime NextDueDate { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Comments { get; set; }

        // Navigation properties
        public MonitoringModule MonitoringModule { get; set; }
        public Plant Plant { get; set; }
        public AreaPlant Area { get; set; }
        public List<MonitoringStage> MonitoringStages { get; set; }
    }
} 