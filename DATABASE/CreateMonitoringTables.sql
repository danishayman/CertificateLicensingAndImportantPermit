-- STEP 1: Create MonitoringSchedules table
CREATE TABLE [dbo].[MonitoringSchedules] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [MonitoringModuleId] INT NOT NULL,
    [PlantId] INT NOT NULL,
    [AreaId] INT NULL,
    [Frequency] NVARCHAR(50) NOT NULL,
    [ScheduledMonth] INT NOT NULL,
    [ScheduledYear] INT NOT NULL,
    [OverallStatus] NVARCHAR(50) NOT NULL,
    [NextDueDate] DATE NOT NULL,
    [ResponsiblePerson] NVARCHAR(100) NOT NULL,
    [Comments] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_dbo.MonitoringSchedules] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MonitoringSchedules_dbo.MonitoringModules_MonitoringModuleId] 
        FOREIGN KEY ([MonitoringModuleId]) 
        REFERENCES [dbo].[MonitoringModules] ([Id]) 
        ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MonitoringSchedules_dbo.Plants_PlantId] 
        FOREIGN KEY ([PlantId]) 
        REFERENCES [dbo].[Plants] ([Id]) 
        ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MonitoringSchedules_dbo.AreaPlants_AreaId] 
        FOREIGN KEY ([AreaId]) 
        REFERENCES [dbo].[AreaPlants] ([Id])
);

-- Create indexes for MonitoringSchedules
CREATE INDEX [IX_MonitoringModuleId] ON [dbo].[MonitoringSchedules]([MonitoringModuleId] ASC);
CREATE INDEX [IX_PlantId] ON [dbo].[MonitoringSchedules]([PlantId] ASC);
CREATE INDEX [IX_AreaId] ON [dbo].[MonitoringSchedules]([AreaId] ASC);

-- STEP 2: Create MonitoringStages table
CREATE TABLE [dbo].[MonitoringStages] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [MonitoringScheduleId] INT NOT NULL,
    [StageType] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [StartDate] DATE NULL,
    [CompletionDate] DATE NULL,
    [AssignedTo] NVARCHAR(100) NULL,
    [Comments] NVARCHAR(MAX) NULL,
    [DocumentReference] NVARCHAR(100) NULL,
    CONSTRAINT [PK_dbo.MonitoringStages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MonitoringStages_dbo.MonitoringSchedules_MonitoringScheduleId] 
        FOREIGN KEY ([MonitoringScheduleId]) 
        REFERENCES [dbo].[MonitoringSchedules] ([Id]) 
        ON DELETE CASCADE
);

-- Create index for MonitoringStages
CREATE INDEX [IX_MonitoringScheduleId] ON [dbo].[MonitoringStages]([MonitoringScheduleId] ASC);

-- STEP 3: Create MonitoringResults table
CREATE TABLE [dbo].[MonitoringResults] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [MonitoringStageId] INT NOT NULL,
    [ResultValue] NVARCHAR(MAX) NULL,
    [PassStatus] NVARCHAR(20) NOT NULL,
    [Findings] NVARCHAR(MAX) NULL,
    [ActionRequired] NVARCHAR(MAX) NULL,
    [CompletedBy] NVARCHAR(100) NOT NULL,
    [AttachmentPath] NVARCHAR(255) NULL,
    CONSTRAINT [PK_dbo.MonitoringResults] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MonitoringResults_dbo.MonitoringStages_MonitoringStageId] 
        FOREIGN KEY ([MonitoringStageId]) 
        REFERENCES [dbo].[MonitoringStages] ([Id])
);

-- Create index for MonitoringResults
CREATE INDEX [IX_MonitoringStageId] ON [dbo].[MonitoringResults]([MonitoringStageId] ASC); 