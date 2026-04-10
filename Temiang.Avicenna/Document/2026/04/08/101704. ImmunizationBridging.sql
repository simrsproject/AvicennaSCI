SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ImmunizationBridging](
	[ImmunizationID] [varchar](3) NOT NULL,
	[SRBridgingType] [varchar](20) NOT NULL,
	[BridgingID] [varchar](20) NOT NULL,
	[BridgingName] [varchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[LastUpdateDateTime] [datetime] NULL,
	[LastUpdateByUserID] [varchar](40) NULL,
 CONSTRAINT [PK_ImmunizationBridging] PRIMARY KEY CLUSTERED 
(
	[ImmunizationID] ASC,
	[SRBridgingType] ASC,
	[BridgingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
