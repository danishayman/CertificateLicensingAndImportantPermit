-- Create CompetencyModules table
CREATE TABLE [dbo].[CompetencyModules] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ModuleName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [ValidityMonths] INT NOT NULL,
    [IsMandatory] BIT NOT NULL,
    CONSTRAINT [PK_dbo.CompetencyModules] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create UserCompetencies table
CREATE TABLE [dbo].[UserCompetencies] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [CompetencyModuleId] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [CompletionDate] DATE NULL,
    [ExpiryDate] DATE NULL,
    [Remarks] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_dbo.UserCompetencies] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserCompetencies_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_dbo.UserCompetencies_dbo.CompetencyModules_CompetencyModuleId] FOREIGN KEY ([CompetencyModuleId]) 
        REFERENCES [dbo].[CompetencyModules] ([Id])
);

-- Create indexes for UserCompetencies
CREATE INDEX [IX_UserId] ON [dbo].[UserCompetencies]([UserId] ASC);
CREATE INDEX [IX_CompetencyModuleId] ON [dbo].[UserCompetencies]([CompetencyModuleId] ASC);

-- Create UserPlants table
CREATE TABLE [dbo].[UserPlants] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [PlantId] INT NOT NULL,
    CONSTRAINT [PK_dbo.UserPlants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserPlants_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_dbo.UserPlants_dbo.Plants_PlantId] FOREIGN KEY ([PlantId]) 
        REFERENCES [dbo].[Plants] ([Id])
);

-- Create indexes for UserPlants
CREATE INDEX [IX_UserId] ON [dbo].[UserPlants]([UserId] ASC);
CREATE INDEX [IX_PlantId] ON [dbo].[UserPlants]([PlantId] ASC);
