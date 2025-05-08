USE [CLIP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[COF_Expiry_Email] AS 
BEGIN 
SET NOCOUNT ON;

-- Get email recipients
DECLARE @emailList varchar(max) = '';
SELECT @emailList = @emailList + EmailAddress + ' ; ' 
FROM [CLIP].[dbo].[EmailRecipients] 
WHERE IsActive = 1;

-- Remove trailing separator
IF LEN(@emailList) > 0
  SET @emailList = LEFT(@emailList, LEN(@emailList) - 2);

-- Find expiring certificates
SELECT 
    COF.Id,
    COF.RegistrationNo,
    P.PlantName,
    COF.MachineName,
    COF.Location,
    COF.Department,
    COF.ExpiryDate,
    DATEDIFF(DAY, GETDATE(), COF.ExpiryDate) AS DaysUntilExpiry,
    COF.Remarks,
    CASE 
        WHEN DATEDIFF(DAY, GETDATE(), COF.ExpiryDate) <= 7 THEN 'critical'
        WHEN DATEDIFF(DAY, GETDATE(), COF.ExpiryDate) <= 30 THEN 'warning'
        ELSE 'info'
    END AS SeverityClass
INTO #ExpiringCertificates
FROM 
    [CLIP].[dbo].[CertificateOfFitness] COF
LEFT JOIN 
    [CLIP].[dbo].[Plants] P ON COF.PlantId = P.Id
WHERE 
    COF.ExpiryDate IS NOT NULL
    AND DATEDIFF(DAY, GETDATE(), COF.ExpiryDate) <= 60
    AND DATEDIFF(DAY, GETDATE(), COF.ExpiryDate) >= 0
ORDER BY 
    DaysUntilExpiry;

-- Count statistics
DECLARE @totalCount int = (SELECT COUNT(*) FROM #ExpiringCertificates);
DECLARE @criticalCount int = (SELECT COUNT(*) FROM #ExpiringCertificates WHERE SeverityClass = 'critical');
DECLARE @warningCount int = (SELECT COUNT(*) FROM #ExpiringCertificates WHERE SeverityClass = 'warning');
DECLARE @infoCount int = (SELECT COUNT(*) FROM #ExpiringCertificates WHERE SeverityClass = 'info');

-- Exit if no expiring certificates
IF @totalCount = 0
BEGIN
    DROP TABLE #ExpiringCertificates;
    RETURN;
END

-- Generate HTML rows more efficiently
DECLARE @certificateRowsHTML nvarchar(max) = '';
SELECT @certificateRowsHTML = @certificateRowsHTML + '
    <tr class="' + SeverityClass + '-row">
        <td>' + RegistrationNo + '</td>
        <td>' + PlantName + '</td>
        <td>' + MachineName + '</td>
        <td>' + ISNULL(Location, '') + '</td>
        <td>' + ISNULL(Department, '') + '</td>
        <td>' + CONVERT(varchar, ExpiryDate, 103) + '</td>
        <td>' + CAST(DaysUntilExpiry AS varchar) + '</td>
        <td>' + ISNULL(Remarks, '') + '</td>
    </tr>'
FROM #ExpiringCertificates
ORDER BY DaysUntilExpiry;

-- Create email content in smaller chunks to avoid truncation
DECLARE @htmlHeader nvarchar(max) = '<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Certificate of Fitness Expiry Notification</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 900px;
            margin: 0 auto;
            padding: 20px;
        }
        .header {
            background-color: #337ab7;
            color: white;
            padding: 15px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }
        .content {
            background-color: #f9f9f9;
            padding: 20px;
            border-left: 1px solid #dddddd;
            border-right: 1px solid #dddddd;
        }
        .footer {
            background-color: #eeeeee;
            padding: 15px;
            text-align: center;
            font-size: 12px;
            color: #777777;
            border-radius: 0 0 5px 5px;
            border: 1px solid #dddddd;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }
        th, td {
            padding: 8px 10px;
            text-align: left;
            border-bottom: 1px solid #dddddd;
        }
        th {
            background-color: #f2f2f2;
        }
        .critical-row {
            background-color: #f2dede;
        }
        .warning-row {
            background-color: #fcf8e3;
        }
        .info-row {
            background-color: #d9edf7;
        }
        .summary-box {
            background-color: #f5f5f5;
            padding: 15px;
            margin: 15px 0;
            border-radius: 5px;
            border: 1px solid #dddddd;
        }
        .summary-item {
            display: inline-block;
            margin-right: 20px;
            padding: 8px 15px;
            border-radius: 4px;
        }
        .critical-count {
            background-color: #f2dede;
            color: #a94442;
            font-weight: bold;
        }
        .warning-count {
            background-color: #fcf8e3;
            color: #8a6d3b;
            font-weight: bold;
        }
        .info-count {
            background-color: #d9edf7;
            color: #31708f;
        }
        .remarks {
            background-color: #f5f5f5;
            padding: 10px;
            border-left: 3px solid #337ab7;
            margin: 15px 0;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Certificate of Fitness Expiry Alert</h1>
            <p>Monthly Expiry Report: ' + CONVERT(varchar, GETDATE(), 103) + '</p>
        </div>
        
        <div class="content">
            
            <p>Dear EHS Team,</p>
            
            <p>This is an <strong>Important Notification</strong> regarding a Certificate of Fitness that will expire within the next 60 days. Please take action to ensure continued compliance with regulatory requirements.</p>
            
            <div class="summary-box">
                <h3>Summary:</h3>
                <div class="summary-item critical-count">Critical (≤7 days): ' + CAST(@criticalCount AS varchar) + '</div>
                <div class="summary-item warning-count">Warning (8-30 days): ' + CAST(@warningCount AS varchar) + '</div>
                <div class="summary-item info-count">Upcoming (31-60 days): ' + CAST(@infoCount AS varchar) + '</div>
                <div class="summary-item">Total: ' + CAST(@totalCount AS varchar) + '</div>
            </div>
            
            <h2>Expiring Certificates of Fitness:</h2>
            
            <table>
                <thead>
                    <tr>
                        <th>Registration No</th>
                        <th>Plant</th>
                        <th>Machine Name</th>
                        <th>Location</th>
                        <th>Department</th>
                        <th>Expiry Date</th>
                        <th>Days Left</th>
                        <th>Remarks</th>
                    </tr>
                </thead>
                <tbody>';

DECLARE @htmlFooter nvarchar(max) = '
                </tbody>
            </table>
            
            <div class="remarks">
                <p><strong>Required Action:</strong></p>
                <p>Please schedule renewal of these Certificates of Fitness before their expiry dates. Expired certificates may result in compliance issues and operational restrictions.</p>
                <p><strong>Color Code:</strong></p>
                <ul>
                    <li><span style="color: #a94442; font-weight: bold;">Red</span> - Critical (7 days or less)</li>
                    <li><span style="color: #8a6d3b; font-weight: bold;">Yellow</span> - Warning (8-30 days)</li>
                    <li><span style="color: #31708f;">Blue</span> - Upcoming (31-60 days)</li>
                </ul>
            </div>
            
        </div>
        
        <div class="footer">
            <p>This is an automated message from the Certificate Licensing And Important Permit (CLIP) system.</p>
            <p>' + CAST(YEAR(GETDATE()) AS varchar) + ' INARI AMERTRON BHD. - Environment, Health and Safety Department (EHS)</p>
        </div>
    </div>
</body>
</html>';

-- Combine HTML parts
DECLARE @htmlContent nvarchar(max) = @htmlHeader + @certificateRowsHTML + @htmlFooter;

-- Insert into email table with error handling
BEGIN TRY
    INSERT INTO [AUTOREPORT].[dbo].TAUTO_EMAIL (
        EMAIL_DESC, 
        TOLIST, 
        CCLIST, 
        EMAIL_TITLE, 
        EMAIL_CONTENT, 
        CREATE_BY, 
        CREATE_DATE, 
        UPDATE_FLAG
    ) 
    VALUES (
        'CLIP Expiring Certificate of Fitness Report', 
        @emailList, 
        '', 
        'Certificate of Fitness Expiry Alert - ' + CAST(@totalCount AS varchar) + ' certificates expiring soon (' + CONVERT(varchar, GETDATE(), 103) + ')', 
        @htmlContent, 
        'INARI PORTAL', 
        GETDATE(), 
        'N'
    );
END TRY
BEGIN CATCH
    -- Log error to a table
    INSERT INTO [CLIP].[dbo].[ErrorLog] (
        ErrorMessage,
        ErrorLine,
        ErrorProcedure,
        ErrorDateTime
    )
    VALUES (
        ERROR_MESSAGE(),
        ERROR_LINE(),
        ERROR_PROCEDURE(),
        GETDATE()
    );
END CATCH

-- Clean up
DROP TABLE #ExpiringCertificates;

END 