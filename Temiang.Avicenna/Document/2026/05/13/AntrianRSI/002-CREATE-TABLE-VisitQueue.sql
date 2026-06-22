CREATE TABLE VisitQueue
(
    VisitQueueNo      VARCHAR(50) PRIMARY KEY,   -- AutoNumber (PK)

    VisitNo           VARCHAR(50) NOT NULL,      -- nomor antrian (A001, B001)
    SRAutoNumber      VARCHAR(100) NOT NULL,     -- VisitTunaiNo, dll

    RegistrationNo    VARCHAR(50) NULL,

    QueueDate         DATE NOT NULL 
                      CONSTRAINT DF_VisitQueue_QueueDate 
                      DEFAULT CAST(GETDATE() AS DATE),

    Status            VARCHAR(20) NOT NULL,
    CurrentStage      VARCHAR(50) NOT NULL,

    -- 🔥 tambahan relasi layanan
    ServiceUnitID     VARCHAR(50) NULL,          -- poli/unit
    ParamedicID       VARCHAR(50) NULL,          -- dokter

	 -- Tambahan grouping/routing antrian
    StageID           VARCHAR(50) NULL,
    CategoryID        VARCHAR(50) NULL,
    QueueKey          VARCHAR(200) NULL,
    QueueLocation     VARCHAR(50) NULL,

    CalledByCounterID VARCHAR(50) NULL,
    CalledTime        DATETIME NULL,

    ServedTime        DATETIME NULL,
    FinishedTime      DATETIME NULL,

    PatientID         VARCHAR(50) NULL,

    CreatedDate       DATETIME NOT NULL 
                      CONSTRAINT DF_VisitQueue_CreatedDate 
                      DEFAULT GETDATE(),

    CreatedBy         VARCHAR(50) NULL,

    QueueSequence     INT NULL,

    Priority          INT NOT NULL 
                      CONSTRAINT DF_VisitQueue_Priority 
                      DEFAULT 100,

    IsManualOverride  BIT 
                      CONSTRAINT DF_VisitQueue_IsManualOverride 
                      DEFAULT 0,

    LastUpdated       DATETIME NULL,
    UpdatedBy         VARCHAR(50) NULL,
	RecallCount       INT NULL
);