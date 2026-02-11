/************************************************************
 * Code formatted by SoftTree SQL Assistant © v11.3.277
 * Time: 11/02/2026 10.25.48
 ************************************************************/

/************************************************************    
 * Code formatted by SoftTree SQL Assistant © v11.3.277    
 * Time: 22/11/2025 18.18.54    
 ************************************************************/    
    
CREATE OR ALTER PROCEDURE spxml_RL3_5V2025(@RlTxReportNo VARCHAR(20))
AS
	SET NOCOUNT ON 
	
	
	--DECLARE @RlTxReportNo VARCHAR(20) = 'RL/250903-0004';            
	DECLARE @city                         VARCHAR(30),
	        @HealthcareName               VARCHAR(MAX),
	        @ProvincesCode                VARCHAR(30),
	        @HospitalCode                 VARCHAR(30),
	        @PeriodYear                   INT,
	        @PeriodMonthStart             INT,
	        @PeriodMonthEnd               INT,
	        @JumlahHariBukaPoliklinik     INT,
	        @JumlahPoliklinikdiRS         INT,
	        @TotKunjunganPriaDalam        INT,
	        @TotKunjunganPriaLuar         INT,
	        @TotKunjunganWanitaLuar       INT,
	        @TotKunjunganWanitaDalam      INT    
	
	
	
	
	
	SELECT @city = h.City,
	       @HealthcareName     = h.HealthcareName,
	       @HospitalCode       = h.HospitalCode,
	       @ProvincesCode      = h.ProvincesCode
	FROM   Healthcare AS h WITH(NOLOCK)    
	
	SELECT @TotKunjunganPriaDalam = rtrv.JumlahLaki,
	       @TotKunjunganPriaLuar        = rtrv.JumlahLaki2,
	       @TotKunjunganWanitaDalam     = rtrv.JumlahPerempuan,
	       @TotKunjunganWanitaLuar      = rtrv.JumlahPerempuan2
	FROM   RlTxReport3_5V2025 AS rtrv
	WHERE  rtrv.RlMasterReportItemID = '206'
	       AND rtrv.RlTxReportNo = @RlTxReportNo    
	
	
	SELECT @PeriodYear = rtrv.PeriodYear,
	       @PeriodMonthStart     = rtrv.PeriodMonthStart,
	       @PeriodMonthEnd       = rtrv.PeriodMonthEnd
	FROM   RlTxReportV2025 AS rtrv WITH(NOLOCK)
	WHERE  rtrv.RlTxReportNo = @RlTxReportNo    
	
	DECLARE @ServiceUnit TABLE (ServiceUnitID VARCHAR(255))    
	
	INSERT INTO @ServiceUnit
	  (
	    ServiceUnitID
	  )
	SELECT LTRIM(RTRIM(x.XmlCol.value('.', 'varchar(50)')))
	FROM   (
	           SELECT CAST(
	                      '<i>' +
	                      REPLACE(
	                          REPLACE(string_agg(rmriv.ParameterValue, ','), ',,', ','),
	                          ',',
	                          '</i><i>'
	                      ) 
	                      + '</i>' AS XML
	                  ) AS XMLDATA
	           FROM   RlMasterReportItemV2025 rmriv
	           WHERE  rmriv.RlMasterReportID = '5'
	       ) AS r
	       CROSS APPLY r.XmlData.nodes('/i') AS x(XmlCol); 
	WITH CTE_1 AS (
	    SELECT DISTINCT ps.ServiceUnitID,
	           ps.ScheduleDate,
	           MONTH(ps.ScheduleDate)  AS Bulan
	    FROM   ParamedicScheduleDate   AS ps
	    WHERE  ps.PeriodYear = @PeriodYear
	           AND MONTH(ps.ScheduleDate) BETWEEN @PeriodMonthStart AND @PeriodMonthEnd
	           AND ps.ServiceUnitID IN (SELECT *
	                                    FROM   @ServiceUnit AS su
	                                    WHERE  su.ServiceUnitID <> '')
	)    
	
	SELECT @JumlahHariBukaPoliklinik = COUNT(*)
	FROM   CTE_1 AS c
	GROUP BY
	       c.Bulan    
	
	SELECT @JumlahPoliklinikdiRS = COUNT(*)
	FROM   @ServiceUnit AS su;;    
	
	
	SELECT 'RL 3.5 Rekapitulasi Kunjungan' ReportName,
	       CASE 
	            WHEN b.PeriodMonthStart = b.PeriodMonthEnd THEN 'Periode : ' + asri.Note + ' ' + b.PeriodYear
	            ELSE 'Periode : ' + asri.Note + ' s/d ' + asri2.ItemName + ' ' + b.PeriodYear
	       END                         AS Periode,
	       asri.Note                   AS Bulan,
	       @HealthcareName             AS HealthcareName,
	       @HospitalCode               AS HospitalCode,
	       @ProvincesCode              AS ProvincesCode,
	       @City                       AS City,
	       b.PeriodYear,
	       c.RlMasterReportItemCode    AS 'No',
	       c.RlMasterReportItemName    AS 'JenisKegiatan',
	       a.JumlahLaki,
	       a.JumlahPerempuan,
	       a.JumlahLaki2,
	       a.JumlahPerempuan2,
	       a.Jumlah,
	       @JumlahHariBukaPoliklinik   AS JumlahHariBukaPoliklinik,
	       @JumlahPoliklinikdiRS       AS JumlahPoliklinikdiRS,
	       @JumlahHariBukaPoliklinik / @JumlahPoliklinikdiRS AS RataRataHariPoliklinikBuka,
	       @TotKunjunganPriaDalam      AS TotKunjunganPriaDalam,
	       @TotKunjunganWanitaDalam    AS TotKunjunganWanitaDalam,
	       @TotKunjunganPriaLuar       AS TotKunjunganPriaLuar,
	       @TotKunjunganWanitaLuar     AS TotKunjunganWanitaLuar,
	       @TotKunjunganPriaDalam / (@JumlahHariBukaPoliklinik / @JumlahPoliklinikdiRS) AS RataA,
	       @TotKunjunganWanitaDalam / (@JumlahHariBukaPoliklinik / @JumlahPoliklinikdiRS) AS RataB,
	       @TotKunjunganPriaLuar / (@JumlahHariBukaPoliklinik / @JumlahPoliklinikdiRS) AS RataC,
	       @TotKunjunganWanitaLuar / (@JumlahHariBukaPoliklinik / @JumlahPoliklinikdiRS) AS RataD
	FROM   RlTxReport3_5V2025          AS a WITH(NOLOCK)
	       INNER JOIN RlTxReportV2025  AS b WITH(NOLOCK)
	            ON  b.RlTxReportNo = a.RlTxReportNo
	       INNER JOIN RlMasterReportItemV2025 AS c WITH(NOLOCK)
	            ON  c.RlMasterReportItemID = a.RlMasterReportItemID
	       INNER JOIN AppStandardReferenceItem asri WITH(NOLOCK)
	            ON  asri.StandardReferenceID = 'MonthID'
	            AND asri.ReferenceID = b.PeriodMonthStart
	       INNER JOIN AppStandardReferenceItem asri2 WITH(NOLOCK)
	            ON  asri2.StandardReferenceID = 'MonthID'
	            AND asri2.ReferenceID = b.PeriodMonthEnd
	WHERE  a.RlTxReportNo = @RlTxReportNo
	ORDER BY
	       c.RlMasterReportItemNo             
                  
                  
                  