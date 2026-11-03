/****** Object:  Table [dbo].[AdmedikaClaimTransaction]    Script Date: 11/3/2026 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AdmedikaClaimTransaction](
	[AdmedikaClaimTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
	[RegistrationNo] [varchar](40) NULL,
	[TransactionNo] [varchar](40) NULL,
	[PatientID] [varchar](15) NULL,
	[GuarantorID] [varchar](10) NULL,
	[CardNo] [varchar](32) NULL,
	[ClID] [varchar](20) NULL,
	[CovID] [varchar](5) NULL,
	[CovCode] [varchar](10) NULL,
	[PlanCode] [varchar](20) NULL,
	[DiagnosisCodeList] [varchar](500) NULL,
	[ClStatus] [varchar](5) NULL,
	[ClDesc] [varchar](255) NULL,
	[ServiceID] [varchar](50) NULL,
	[PrintOut] [nvarchar](max) NULL,
	[TotAmtIncurred] [decimal](18, 2) NULL,
	[TotAmtApproved] [decimal](18, 2) NULL,
	[TotAmtNotApproved] [decimal](18, 2) NULL,
	[IsClosed] [bit] NOT NULL CONSTRAINT [DF_AdmedikaClaimTransaction_IsClosed] DEFAULT ((0)),
	[CreatedDateTime] [datetime] NULL,
	[CreatedByUserID] [varchar](40) NULL,
	[LastUpdateDateTime] [datetime] NULL,
	[LastUpdateByUserID] [varchar](40) NULL,
 CONSTRAINT [PK_AdmedikaClaimTransaction] PRIMARY KEY CLUSTERED 
(
	[AdmedikaClaimTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_AdmedikaClaimTransaction_ClID]
ON [dbo].[AdmedikaClaimTransaction]([ClID] ASC)
WHERE [ClID] IS NOT NULL
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
