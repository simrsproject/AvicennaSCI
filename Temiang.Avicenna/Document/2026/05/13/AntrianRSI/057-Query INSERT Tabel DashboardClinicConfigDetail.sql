CREATE TABLE DashboardClinicConfigDetail
(
    ConfigDetailID     BIGINT IDENTITY(1,1),

    ConfigID           VARCHAR(20)     NOT NULL,

    ServiceUnitID      VARCHAR(10)     NOT NULL,

    StageID            VARCHAR(50)     NOT NULL,

    ParamedicID        VARCHAR(10)     NOT NULL,

    KamarCode          VARCHAR(20)     NOT NULL,

    CONSTRAINT PK_DashboardClinicConfigDetail
        PRIMARY KEY (ConfigDetailID),

    CONSTRAINT FK_DCCD_Config
        FOREIGN KEY (ConfigID)
        REFERENCES DashboardClinicConfig(ConfigID),

    CONSTRAINT FK_DCCD_ServiceUnit
        FOREIGN KEY (ServiceUnitID)
        REFERENCES ServiceUnit(ServiceUnitID),

    CONSTRAINT FK_DCCD_Stage
        FOREIGN KEY (StageID)
        REFERENCES QueueStage(StageID),

    CONSTRAINT FK_DCCD_Paramedic
        FOREIGN KEY (ParamedicID)
        REFERENCES Paramedic(ParamedicID)
);