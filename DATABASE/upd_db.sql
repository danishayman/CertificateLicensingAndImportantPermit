CREATE TABLE PlantMonitoring (
    Id INT IDENTITY PRIMARY KEY,
    PlantID INT NOT NULL,
    MonitoringID INT NOT NULL,
    Area NVARCHAR(100),
    ExpDate DATE,
    QuoteDate DATE,
    QuoteSubmitDate DATE,
    QuoteCompleteDate DATE,
    QuoteUserAssign NVARCHAR(100),
    EprDate DATE,
    EprSubmitDate DATE,
    EprCompleteDate DATE,
    EprUserAssign NVARCHAR(100),
    WorkDate DATE,
    WorkSubmitDate DATE,
    WorkCompleteDate DATE,
    WorkUserAssign NVARCHAR(100),
    Remarks NVARCHAR(MAX),
    
    CONSTRAINT FK_PlantMonitoring_Plant FOREIGN KEY (PlantID) REFERENCES Plants(Id),
    CONSTRAINT FK_PlantMonitoring_Monitoring FOREIGN KEY (MonitoringID) REFERENCES Monitoring(MonitoringID)
);


CREATE TABLE Monitoring (
    MonitoringID INT PRIMARY KEY IDENTITY,
    MonitoringName NVARCHAR(100) NOT NULL,
    MonitoringCategory NVARCHAR(100),
    MonitoringFreq NVARCHAR(50)
);


ALTER TABLE PlantMonitoring
ADD 
    QuoteDoc NVARCHAR(50) NULL,
    ePRDoc NVARCHAR(50) NULL,
    WorkDoc NVARCHAR(50) NULL;
