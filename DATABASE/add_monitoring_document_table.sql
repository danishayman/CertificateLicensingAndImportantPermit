-- Update column sizes for document paths in PlantMonitoring table
ALTER TABLE dbo.PlantMonitoring ALTER COLUMN QuoteDoc NVARCHAR(500);
ALTER TABLE dbo.PlantMonitoring ALTER COLUMN EprDoc NVARCHAR(500);
ALTER TABLE dbo.PlantMonitoring ALTER COLUMN WorkDoc NVARCHAR(500); 