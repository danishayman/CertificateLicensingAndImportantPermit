-- Master SQL script that executes all scripts in the correct order
-- to set up the database without using migrations

-- Start with identity tables (AspNetUsers, AspNetRoles, etc.)
:r initialize.sql
GO

-- Add EmpID to AspNetUsers
:r add_Emp_ID.sql
GO

-- Create Plants and AreaPlants tables
:r CreatePlants.sql
GO

-- Create CompetencyModules, UserCompetencies and UserPlants tables
:r CreateCompetencyTables.sql
GO

-- Create MonitoringModules table
:r CreateMonitoringModules.sql
GO

-- Create all monitoring-related tables
:r CreateMonitoringTables.sql
GO

PRINT 'All database objects have been created successfully.' 