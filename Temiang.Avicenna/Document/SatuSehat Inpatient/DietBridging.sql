/****** Object:  Table [dbo].[DietBridging]    Script Date: 12/09/2025 12:38:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DietBridging](
	[DietId] [varchar](20) NOT NULL,
	[SRBridgingType] [varchar](20) NOT NULL,
	[BridgingID] [varchar](15) NOT NULL,
	[BridgingName] [varchar](100) NULL,
	[IsActive] [bit] NULL,
	[LastUpdateDateTime] [datetime] NULL,
	[LastUpdateByUserID] [varchar](40) NULL,
 CONSTRAINT [PK_DietBridging] PRIMARY KEY CLUSTERED 
(
	[DietId] ASC,
	[SRBridgingType] ASC,
	[BridgingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


