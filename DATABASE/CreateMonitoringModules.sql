-- Create MonitoringModules table
CREATE TABLE [dbo].[MonitoringModules] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [Type] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_dbo.MonitoringModules] PRIMARY KEY CLUSTERED ([Id] ASC)
);

