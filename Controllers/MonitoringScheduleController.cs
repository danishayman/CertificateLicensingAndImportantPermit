using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLIP.Models;
using CLIP.Models.DataAccess;
using CLIP.ViewModels;

namespace CLIP.Controllers
{
    [Authorize]
    public class MonitoringScheduleController : Controller
    {
        private readonly MonitoringDataAccess _dataAccess = new MonitoringDataAccess();

        // GET: MonitoringSchedule/Category/5
        public ActionResult Category(int id)
        {
            var module = _dataAccess.GetMonitoringModuleById(id);
            if (module == null)
            {
                return HttpNotFound();
            }

            var schedules = _dataAccess.GetMonitoringSchedulesByModule(id);
            var stats = _dataAccess.GetMonitoringStatsByModule(id);

            var viewModel = new MonitoringCategoryViewModel
            {
                MonitoringModule = module,
                Schedules = schedules,
                TotalSchedules = stats.TotalSchedules,
                CompletedSchedules = stats.CompletedSchedules,
                InProgressSchedules = stats.InProgressSchedules,
                NotStartedSchedules = stats.NotStartedSchedules,
                OverdueSchedules = stats.OverdueSchedules,
                StageProgress = stats.StageProgress
            };

            return View(viewModel);
        }

        // GET: MonitoringSchedule/Details/5
        public ActionResult Details(int id)
        {
            var schedule = _dataAccess.GetMonitoringScheduleById(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }

            var stages = _dataAccess.GetMonitoringStagesBySchedule(id);
            
            // Load results for each stage
            foreach (var stage in stages.Where(s => s.StageType == StageType.WorkExecution))
            {
                stage.MonitoringResult = _dataAccess.GetMonitoringResultByStageId(stage.Id);
            }

            var viewModel = new MonitoringScheduleDetailsViewModel
            {
                Schedule = schedule,
                Stages = stages
            };

            return View(viewModel);
        }

        // GET: MonitoringSchedule/Create
        public ActionResult Create(int moduleId)
        {
            var module = _dataAccess.GetMonitoringModuleById(moduleId);
            if (module == null)
            {
                return HttpNotFound();
            }

            var plants = GetPlantList();
            var areas = new List<SelectListItem>(); // Will be populated via AJAX

            var viewModel = new MonitoringScheduleEditViewModel
            {
                MonitoringModuleId = moduleId,
                ModuleName = module.Category,
                PlantList = plants,
                AreaList = areas,
                ScheduledMonth = DateTime.Now.Month,
                ScheduledYear = DateTime.Now.Year,
                NextDueDate = DateTime.Now.AddMonths(1),
                OverallStatus = "Not Started",
                FrequencyOptions = GetFrequencyOptions()
            };

            return View(viewModel);
        }

        // POST: MonitoringSchedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MonitoringScheduleEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var schedule = new MonitoringSchedule
                    {
                        MonitoringModuleId = viewModel.MonitoringModuleId,
                        PlantId = viewModel.PlantId,
                        AreaId = viewModel.AreaId,
                        Frequency = viewModel.Frequency,
                        ScheduledMonth = viewModel.ScheduledMonth,
                        ScheduledYear = viewModel.ScheduledYear,
                        OverallStatus = viewModel.OverallStatus,
                        NextDueDate = viewModel.NextDueDate,
                        ResponsiblePerson = viewModel.ResponsiblePerson,
                        Comments = viewModel.Comments
                    };

                    int scheduleId = _dataAccess.CreateMonitoringSchedule(schedule);

                    // Create the three stages
                    CreateDefaultStages(scheduleId);

                    return RedirectToAction("Category", new { id = viewModel.MonitoringModuleId });
                }
                catch
                {
                    // Log error
                    ModelState.AddModelError("", "An error occurred while creating the schedule. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay form
            viewModel.PlantList = GetPlantList();
            viewModel.AreaList = GetAreaList(viewModel.PlantId);
            viewModel.FrequencyOptions = GetFrequencyOptions();
            return View(viewModel);
        }

        // GET: MonitoringSchedule/Edit/5
        public ActionResult Edit(int id)
        {
            var schedule = _dataAccess.GetMonitoringScheduleById(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }

            var module = _dataAccess.GetMonitoringModuleById(schedule.MonitoringModuleId);

            var viewModel = new MonitoringScheduleEditViewModel
            {
                Id = schedule.Id,
                MonitoringModuleId = schedule.MonitoringModuleId,
                ModuleName = module.Category,
                PlantId = schedule.PlantId,
                AreaId = schedule.AreaId,
                Frequency = schedule.Frequency,
                ScheduledMonth = schedule.ScheduledMonth,
                ScheduledYear = schedule.ScheduledYear,
                OverallStatus = schedule.OverallStatus,
                NextDueDate = schedule.NextDueDate,
                ResponsiblePerson = schedule.ResponsiblePerson,
                Comments = schedule.Comments,
                PlantList = GetPlantList(),
                AreaList = GetAreaList(schedule.PlantId),
                FrequencyOptions = GetFrequencyOptions()
            };

            return View(viewModel);
        }

        // POST: MonitoringSchedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MonitoringScheduleEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var schedule = new MonitoringSchedule
                    {
                        Id = viewModel.Id,
                        MonitoringModuleId = viewModel.MonitoringModuleId,
                        PlantId = viewModel.PlantId,
                        AreaId = viewModel.AreaId,
                        Frequency = viewModel.Frequency,
                        ScheduledMonth = viewModel.ScheduledMonth,
                        ScheduledYear = viewModel.ScheduledYear,
                        OverallStatus = viewModel.OverallStatus,
                        NextDueDate = viewModel.NextDueDate,
                        ResponsiblePerson = viewModel.ResponsiblePerson,
                        Comments = viewModel.Comments
                    };

                    _dataAccess.UpdateMonitoringSchedule(schedule);

                    return RedirectToAction("Category", new { id = viewModel.MonitoringModuleId });
                }
                catch
                {
                    // Log error
                    ModelState.AddModelError("", "An error occurred while updating the schedule. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay form
            viewModel.PlantList = GetPlantList();
            viewModel.AreaList = GetAreaList(viewModel.PlantId);
            viewModel.FrequencyOptions = GetFrequencyOptions();
            return View(viewModel);
        }

        // GET: MonitoringSchedule/Delete/5
        public ActionResult Delete(int id)
        {
            var schedule = _dataAccess.GetMonitoringScheduleById(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }

            return View(schedule);
        }

        // POST: MonitoringSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var schedule = _dataAccess.GetMonitoringScheduleById(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }

            try
            {
                _dataAccess.DeleteMonitoringSchedule(id);
                return RedirectToAction("Category", new { id = schedule.MonitoringModuleId });
            }
            catch
            {
                // Log error
                ModelState.AddModelError("", "An error occurred while deleting the schedule. Please try again.");
                return View(schedule);
            }
        }

        // Helper methods
        private List<SelectListItem> GetPlantList()
        {
            // In a real implementation, this would fetch from the database
            var plantDataAccess = new PlantDataAccess();
            var plants = plantDataAccess.GetAllPlants();
            
            return plants.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.PlantName
            }).ToList();
        }

        private List<SelectListItem> GetAreaList(int plantId)
        {
            // In a real implementation, this would fetch from the database
            var areaDataAccess = new AreaDataAccess();
            var areas = areaDataAccess.GetAreasByPlant(plantId);
            
            return areas.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.AreaName
            }).ToList();
        }

        private List<SelectListItem> GetFrequencyOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Yearly", Text = "Yearly" },
                new SelectListItem { Value = "Half-Yearly", Text = "Half-Yearly" },
                new SelectListItem { Value = "Quarterly", Text = "Quarterly" },
                new SelectListItem { Value = "Monthly", Text = "Monthly" }
            };
        }

        private void CreateDefaultStages(int scheduleId)
        {
            // Create Quotation Request stage
            var quotationStage = new MonitoringStage
            {
                MonitoringScheduleId = scheduleId,
                StageType = StageType.QuotationRequest,
                Status = "Not Started"
            };
            _dataAccess.CreateMonitoringStage(quotationStage);

            // Create EPR stage
            var eprStage = new MonitoringStage
            {
                MonitoringScheduleId = scheduleId,
                StageType = StageType.EPR,
                Status = "Not Started"
            };
            _dataAccess.CreateMonitoringStage(eprStage);

            // Create Work Execution stage
            var workStage = new MonitoringStage
            {
                MonitoringScheduleId = scheduleId,
                StageType = StageType.WorkExecution,
                Status = "Not Started"
            };
            _dataAccess.CreateMonitoringStage(workStage);
        }
    }
} 