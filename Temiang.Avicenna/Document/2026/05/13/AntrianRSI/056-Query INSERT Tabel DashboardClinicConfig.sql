CREATE TABLE DashboardClinicConfig
(
    ConfigID               VARCHAR(20)     NOT NULL,
    UserID                 VARCHAR(15)     NOT NULL,
    ConfigName             VARCHAR(100)    NOT NULL,

    AutoRefresh            BIT             NOT NULL
        CONSTRAINT DF_DashboardClinicConfig_AutoRefresh DEFAULT(1),

    RefreshIntervalSec     INT             NOT NULL
        CONSTRAINT DF_DashboardClinicConfig_RefreshInterval DEFAULT(5),

    IsActive               BIT             NOT NULL
        CONSTRAINT DF_DashboardClinicConfig_IsActive DEFAULT(1),

    LastUpdateDateTime     DATETIME        NOT NULL
        CONSTRAINT DF_DashboardClinicConfig_LastUpdate DEFAULT(GETDATE()),

    LastUpdateByUserID     VARCHAR(20)     NULL,

    CONSTRAINT PK_DashboardClinicConfig
        PRIMARY KEY (ConfigID),

    CONSTRAINT FK_DashboardClinicConfig_AppUser
        FOREIGN KEY (UserID)
        REFERENCES AppUser(UserID)
);

