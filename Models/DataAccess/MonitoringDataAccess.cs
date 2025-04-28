using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CLIP.Models.DataAccess
{
    public class MonitoringDataAccess : DataAccessBase
    {
        #region MonitoringModule Methods

        public List<MonitoringModule> GetAllMonitoringModules()
        {
            const string sql = @"
                SELECT Id, Category, Type
                FROM MonitoringModules
                ORDER BY Type, Category";

            return ExecuteReader(sql, reader => new MonitoringModule
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Type = reader.GetString(reader.GetOrdinal("Type"))
            });
        }

        public List<MonitoringModule> GetMonitoringModulesByType(string type)
        {
            const string sql = @"
                SELECT Id, Category, Type
                FROM MonitoringModules
                WHERE Type = @Type
                ORDER BY Category";

            var parameters = new Dictionary<string, object>
            {
                { "@Type", type }
            };

            return ExecuteReader(sql, reader => new MonitoringModule
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Type = reader.GetString(reader.GetOrdinal("Type"))
            }, parameters);
        }

        public MonitoringModule GetMonitoringModuleById(int id)
        {
            const string sql = @"
                SELECT Id, Category, Type
                FROM MonitoringModules
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            return ExecuteSingleReader(sql, reader => new MonitoringModule
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Type = reader.GetString(reader.GetOrdinal("Type"))
            }, parameters);
        }

        public int CreateMonitoringModule(MonitoringModule module)
        {
            const string sql = @"
                INSERT INTO MonitoringModules (Category, Type)
                VALUES (@Category, @Type);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@Category", module.Category },
                { "@Type", module.Type }
            };

            return ExecuteScalar<int>(sql, parameters);
        }

        public void UpdateMonitoringModule(MonitoringModule module)
        {
            const string sql = @"
                UPDATE MonitoringModules
                SET Category = @Category, Type = @Type
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", module.Id },
                { "@Category", module.Category },
                { "@Type", module.Type }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteMonitoringModule(int id)
        {
            const string sql = @"
                DELETE FROM MonitoringModules
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            ExecuteNonQuery(sql, parameters);
        }

        #endregion

        #region MonitoringSchedule Methods

        public List<MonitoringSchedule> GetMonitoringSchedulesByModule(int moduleId)
        {
            const string sql = @"
                SELECT 
                    s.Id, s.MonitoringModuleId, s.PlantId, s.AreaId, 
                    s.Frequency, s.ScheduledMonth, s.ScheduledYear, 
                    s.OverallStatus, s.NextDueDate, s.ResponsiblePerson, 
                    s.Comments,
                    p.PlantName,
                    a.AreaName
                FROM MonitoringSchedules s
                INNER JOIN Plants p ON s.PlantId = p.Id
                LEFT JOIN AreaPlants a ON s.AreaId = a.Id
                WHERE s.MonitoringModuleId = @ModuleId
                ORDER BY p.PlantName, s.ScheduledMonth";

            var parameters = new Dictionary<string, object>
            {
                { "@ModuleId", moduleId }
            };

            return ExecuteReader(sql, reader => MapMonitoringSchedule(reader), parameters);
        }

        public MonitoringSchedule GetMonitoringScheduleById(int id)
        {
            const string sql = @"
                SELECT 
                    s.Id, s.MonitoringModuleId, s.PlantId, s.AreaId, 
                    s.Frequency, s.ScheduledMonth, s.ScheduledYear, 
                    s.OverallStatus, s.NextDueDate, s.ResponsiblePerson, 
                    s.Comments,
                    p.PlantName,
                    a.AreaName
                FROM MonitoringSchedules s
                INNER JOIN Plants p ON s.PlantId = p.Id
                LEFT JOIN AreaPlants a ON s.AreaId = a.Id
                WHERE s.Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            return ExecuteSingleReader(sql, reader => MapMonitoringSchedule(reader), parameters);
        }

        public int CreateMonitoringSchedule(MonitoringSchedule schedule)
        {
            const string sql = @"
                INSERT INTO MonitoringSchedules (
                    MonitoringModuleId, PlantId, AreaId, 
                    Frequency, ScheduledMonth, ScheduledYear, 
                    OverallStatus, NextDueDate, ResponsiblePerson, Comments)
                VALUES (
                    @MonitoringModuleId, @PlantId, @AreaId, 
                    @Frequency, @ScheduledMonth, @ScheduledYear, 
                    @OverallStatus, @NextDueDate, @ResponsiblePerson, @Comments);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@MonitoringModuleId", schedule.MonitoringModuleId },
                { "@PlantId", schedule.PlantId },
                { "@AreaId", schedule.AreaId },
                { "@Frequency", schedule.Frequency },
                { "@ScheduledMonth", schedule.ScheduledMonth },
                { "@ScheduledYear", schedule.ScheduledYear },
                { "@OverallStatus", schedule.OverallStatus },
                { "@NextDueDate", schedule.NextDueDate },
                { "@ResponsiblePerson", schedule.ResponsiblePerson },
                { "@Comments", schedule.Comments }
            };

            return ExecuteScalar<int>(sql, parameters);
        }

        public void UpdateMonitoringSchedule(MonitoringSchedule schedule)
        {
            const string sql = @"
                UPDATE MonitoringSchedules
                SET 
                    MonitoringModuleId = @MonitoringModuleId,
                    PlantId = @PlantId,
                    AreaId = @AreaId,
                    Frequency = @Frequency,
                    ScheduledMonth = @ScheduledMonth,
                    ScheduledYear = @ScheduledYear,
                    OverallStatus = @OverallStatus,
                    NextDueDate = @NextDueDate,
                    ResponsiblePerson = @ResponsiblePerson,
                    Comments = @Comments
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", schedule.Id },
                { "@MonitoringModuleId", schedule.MonitoringModuleId },
                { "@PlantId", schedule.PlantId },
                { "@AreaId", schedule.AreaId },
                { "@Frequency", schedule.Frequency },
                { "@ScheduledMonth", schedule.ScheduledMonth },
                { "@ScheduledYear", schedule.ScheduledYear },
                { "@OverallStatus", schedule.OverallStatus },
                { "@NextDueDate", schedule.NextDueDate },
                { "@ResponsiblePerson", schedule.ResponsiblePerson },
                { "@Comments", schedule.Comments }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteMonitoringSchedule(int id)
        {
            const string sql = @"
                DELETE FROM MonitoringSchedules
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            ExecuteNonQuery(sql, parameters);
        }

        private MonitoringSchedule MapMonitoringSchedule(IDataReader reader)
        {
            return new MonitoringSchedule
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MonitoringModuleId = reader.GetInt32(reader.GetOrdinal("MonitoringModuleId")),
                PlantId = reader.GetInt32(reader.GetOrdinal("PlantId")),
                AreaId = reader.IsDBNull(reader.GetOrdinal("AreaId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AreaId")),
                Frequency = reader.GetString(reader.GetOrdinal("Frequency")),
                ScheduledMonth = reader.GetInt32(reader.GetOrdinal("ScheduledMonth")),
                ScheduledYear = reader.GetInt32(reader.GetOrdinal("ScheduledYear")),
                OverallStatus = reader.GetString(reader.GetOrdinal("OverallStatus")),
                NextDueDate = reader.GetDateTime(reader.GetOrdinal("NextDueDate")),
                ResponsiblePerson = reader.GetString(reader.GetOrdinal("ResponsiblePerson")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                // Navigation properties populated from JOIN
                Plant = new Plant { PlantName = reader.GetString(reader.GetOrdinal("PlantName")) },
                Area = reader.IsDBNull(reader.GetOrdinal("AreaName")) ? null : new AreaPlant { AreaName = reader.GetString(reader.GetOrdinal("AreaName")) }
            };
        }

        #endregion

        #region MonitoringStage Methods

        public List<MonitoringStage> GetMonitoringStagesBySchedule(int scheduleId)
        {
            const string sql = @"
                SELECT 
                    Id, MonitoringScheduleId, StageType, Status, 
                    StartDate, CompletionDate, AssignedTo, 
                    Comments, DocumentReference
                FROM MonitoringStages
                WHERE MonitoringScheduleId = @ScheduleId
                ORDER BY StageType";

            var parameters = new Dictionary<string, object>
            {
                { "@ScheduleId", scheduleId }
            };

            return ExecuteReader(sql, reader => MapMonitoringStage(reader), parameters);
        }

        public MonitoringStage GetMonitoringStageById(int id)
        {
            const string sql = @"
                SELECT 
                    Id, MonitoringScheduleId, StageType, Status, 
                    StartDate, CompletionDate, AssignedTo, 
                    Comments, DocumentReference
                FROM MonitoringStages
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            return ExecuteSingleReader(sql, reader => MapMonitoringStage(reader), parameters);
        }

        public int CreateMonitoringStage(MonitoringStage stage)
        {
            const string sql = @"
                INSERT INTO MonitoringStages (
                    MonitoringScheduleId, StageType, Status, 
                    StartDate, CompletionDate, AssignedTo, 
                    Comments, DocumentReference)
                VALUES (
                    @MonitoringScheduleId, @StageType, @Status, 
                    @StartDate, @CompletionDate, @AssignedTo, 
                    @Comments, @DocumentReference);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@MonitoringScheduleId", stage.MonitoringScheduleId },
                { "@StageType", (int)stage.StageType },
                { "@Status", stage.Status },
                { "@StartDate", stage.StartDate },
                { "@CompletionDate", stage.CompletionDate },
                { "@AssignedTo", stage.AssignedTo },
                { "@Comments", stage.Comments },
                { "@DocumentReference", stage.DocumentReference }
            };

            return ExecuteScalar<int>(sql, parameters);
        }

        public void UpdateMonitoringStage(MonitoringStage stage)
        {
            const string sql = @"
                UPDATE MonitoringStages
                SET 
                    MonitoringScheduleId = @MonitoringScheduleId,
                    StageType = @StageType,
                    Status = @Status,
                    StartDate = @StartDate,
                    CompletionDate = @CompletionDate,
                    AssignedTo = @AssignedTo,
                    Comments = @Comments,
                    DocumentReference = @DocumentReference
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", stage.Id },
                { "@MonitoringScheduleId", stage.MonitoringScheduleId },
                { "@StageType", (int)stage.StageType },
                { "@Status", stage.Status },
                { "@StartDate", stage.StartDate },
                { "@CompletionDate", stage.CompletionDate },
                { "@AssignedTo", stage.AssignedTo },
                { "@Comments", stage.Comments },
                { "@DocumentReference", stage.DocumentReference }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteMonitoringStage(int id)
        {
            const string sql = @"
                DELETE FROM MonitoringStages
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            ExecuteNonQuery(sql, parameters);
        }

        private MonitoringStage MapMonitoringStage(IDataReader reader)
        {
            return new MonitoringStage
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MonitoringScheduleId = reader.GetInt32(reader.GetOrdinal("MonitoringScheduleId")),
                StageType = (StageType)reader.GetInt32(reader.GetOrdinal("StageType")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("StartDate")),
                CompletionDate = reader.IsDBNull(reader.GetOrdinal("CompletionDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CompletionDate")),
                AssignedTo = reader.IsDBNull(reader.GetOrdinal("AssignedTo")) ? null : reader.GetString(reader.GetOrdinal("AssignedTo")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                DocumentReference = reader.IsDBNull(reader.GetOrdinal("DocumentReference")) ? null : reader.GetString(reader.GetOrdinal("DocumentReference"))
            };
        }

        #endregion

        #region MonitoringResult Methods

        public MonitoringResult GetMonitoringResultByStageId(int stageId)
        {
            const string sql = @"
                SELECT 
                    Id, MonitoringStageId, ResultValue, PassStatus, 
                    Findings, ActionRequired, CompletedBy, AttachmentPath
                FROM MonitoringResults
                WHERE MonitoringStageId = @StageId";

            var parameters = new Dictionary<string, object>
            {
                { "@StageId", stageId }
            };

            return ExecuteSingleReader(sql, reader => MapMonitoringResult(reader), parameters);
        }

        public int CreateMonitoringResult(MonitoringResult result)
        {
            const string sql = @"
                INSERT INTO MonitoringResults (
                    MonitoringStageId, ResultValue, PassStatus, 
                    Findings, ActionRequired, CompletedBy, AttachmentPath)
                VALUES (
                    @MonitoringStageId, @ResultValue, @PassStatus, 
                    @Findings, @ActionRequired, @CompletedBy, @AttachmentPath);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@MonitoringStageId", result.MonitoringStageId },
                { "@ResultValue", result.ResultValue },
                { "@PassStatus", result.PassStatus },
                { "@Findings", result.Findings },
                { "@ActionRequired", result.ActionRequired },
                { "@CompletedBy", result.CompletedBy },
                { "@AttachmentPath", result.AttachmentPath }
            };

            return ExecuteScalar<int>(sql, parameters);
        }

        public void UpdateMonitoringResult(MonitoringResult result)
        {
            const string sql = @"
                UPDATE MonitoringResults
                SET 
                    ResultValue = @ResultValue,
                    PassStatus = @PassStatus,
                    Findings = @Findings,
                    ActionRequired = @ActionRequired,
                    CompletedBy = @CompletedBy,
                    AttachmentPath = @AttachmentPath
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", result.Id },
                { "@ResultValue", result.ResultValue },
                { "@PassStatus", result.PassStatus },
                { "@Findings", result.Findings },
                { "@ActionRequired", result.ActionRequired },
                { "@CompletedBy", result.CompletedBy },
                { "@AttachmentPath", result.AttachmentPath }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteMonitoringResult(int id)
        {
            const string sql = @"
                DELETE FROM MonitoringResults
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            ExecuteNonQuery(sql, parameters);
        }

        private MonitoringResult MapMonitoringResult(IDataReader reader)
        {
            return new MonitoringResult
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MonitoringStageId = reader.GetInt32(reader.GetOrdinal("MonitoringStageId")),
                ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                PassStatus = reader.GetString(reader.GetOrdinal("PassStatus")),
                Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                ActionRequired = reader.IsDBNull(reader.GetOrdinal("ActionRequired")) ? null : reader.GetString(reader.GetOrdinal("ActionRequired")),
                CompletedBy = reader.GetString(reader.GetOrdinal("CompletedBy")),
                AttachmentPath = reader.IsDBNull(reader.GetOrdinal("AttachmentPath")) ? null : reader.GetString(reader.GetOrdinal("AttachmentPath"))
            };
        }

        #endregion

        #region Monitoring Dashboard Methods

        public class MonitoringStats
        {
            public int TotalSchedules { get; set; }
            public int CompletedSchedules { get; set; }
            public int InProgressSchedules { get; set; }
            public int NotStartedSchedules { get; set; }
            public int OverdueSchedules { get; set; }
            
            public Dictionary<string, StageStats> StageProgress { get; set; }
        }

        public class StageStats
        {
            public int Total { get; set; }
            public int Completed { get; set; }
            public int InProgress { get; set; }
            public int NotStarted { get; set; }
        }

        public MonitoringStats GetMonitoringStatsByModule(int moduleId)
        {
            // Get all schedule IDs for this module
            var scheduleSql = @"
                SELECT Id FROM MonitoringSchedules
                WHERE MonitoringModuleId = @ModuleId";

            var parameters = new Dictionary<string, object>
            {
                { "@ModuleId", moduleId }
            };

            var scheduleIds = ExecuteReader(scheduleSql, reader => reader.GetInt32(0), parameters);
            
            if (scheduleIds.Count == 0)
            {
                return new MonitoringStats
                {
                    TotalSchedules = 0,
                    CompletedSchedules = 0,
                    InProgressSchedules = 0,
                    NotStartedSchedules = 0,
                    OverdueSchedules = 0,
                    StageProgress = new Dictionary<string, StageStats>
                    {
                        { "Quotation", new StageStats() },
                        { "EPR", new StageStats() },
                        { "Work", new StageStats() }
                    }
                };
            }

            // Get schedule stats
            var statsQuery = @"
                SELECT 
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN OverallStatus = 'Completed' THEN 1 ELSE 0 END) AS CompletedCount,
                    SUM(CASE WHEN OverallStatus = 'In Progress' THEN 1 ELSE 0 END) AS InProgressCount,
                    SUM(CASE WHEN OverallStatus = 'Not Started' THEN 1 ELSE 0 END) AS NotStartedCount,
                    SUM(CASE WHEN OverallStatus = 'Overdue' THEN 1 ELSE 0 END) AS OverdueCount
                FROM MonitoringSchedules
                WHERE MonitoringModuleId = @ModuleId";

            var stats = ExecuteSingleReader(statsQuery, reader => new
            {
                Total = reader.GetInt32(reader.GetOrdinal("TotalCount")),
                Completed = reader.GetInt32(reader.GetOrdinal("CompletedCount")),
                InProgress = reader.GetInt32(reader.GetOrdinal("InProgressCount")),
                NotStarted = reader.GetInt32(reader.GetOrdinal("NotStartedCount")),
                Overdue = reader.GetInt32(reader.GetOrdinal("OverdueCount"))
            }, parameters);

            // Get stage stats
            var stageStatsQuery = @"
                SELECT 
                    StageType,
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedCount,
                    SUM(CASE WHEN Status = 'In Progress' THEN 1 ELSE 0 END) AS InProgressCount,
                    SUM(CASE WHEN Status = 'Not Started' THEN 1 ELSE 0 END) AS NotStartedCount
                FROM MonitoringStages
                WHERE MonitoringScheduleId IN (" + string.Join(",", scheduleIds) + @")
                GROUP BY StageType";

            var stageStats = ExecuteReader(stageStatsQuery, reader => new
            {
                StageType = reader.GetInt32(reader.GetOrdinal("StageType")),
                Total = reader.GetInt32(reader.GetOrdinal("TotalCount")),
                Completed = reader.GetInt32(reader.GetOrdinal("CompletedCount")),
                InProgress = reader.GetInt32(reader.GetOrdinal("InProgressCount")),
                NotStarted = reader.GetInt32(reader.GetOrdinal("NotStartedCount"))
            });

            var stageProgress = new Dictionary<string, StageStats>
            {
                { "Quotation", new StageStats() },
                { "EPR", new StageStats() },
                { "Work", new StageStats() }
            };

            foreach (var stat in stageStats)
            {
                string stageKey;
                switch (stat.StageType)
                {
                    case 1: stageKey = "Quotation"; break;
                    case 2: stageKey = "EPR"; break;
                    case 3: stageKey = "Work"; break;
                    default: continue;
                }

                stageProgress[stageKey] = new StageStats
                {
                    Total = stat.Total,
                    Completed = stat.Completed,
                    InProgress = stat.InProgress,
                    NotStarted = stat.NotStarted
                };
            }

            return new MonitoringStats
            {
                TotalSchedules = stats.Total,
                CompletedSchedules = stats.Completed,
                InProgressSchedules = stats.InProgress,
                NotStartedSchedules = stats.NotStarted,
                OverdueSchedules = stats.Overdue,
                StageProgress = stageProgress
            };
        }

        #endregion
    }
} 