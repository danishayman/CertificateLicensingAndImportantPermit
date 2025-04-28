-- Create Plants table
CREATE TABLE [dbo].[Plants] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [PlantName] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_dbo.Plants] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Create AreaPlants table
CREATE TABLE [dbo].[AreaPlants] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [AreaName] NVARCHAR(100) NOT NULL,
    [PlantId] INT NOT NULL,
    CONSTRAINT [PK_dbo.AreaPlants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AreaPlants_dbo.Plants_PlantId] FOREIGN KEY ([PlantId]) REFERENCES [dbo].[Plants] ([Id]) ON DELETE CASCADE
);

-- Create index for AreaPlants
CREATE INDEX [IX_PlantId] ON [dbo].[AreaPlants]([PlantId] ASC);

