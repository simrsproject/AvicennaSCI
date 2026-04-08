--USE [RSI]
--GO

/****** Object:  Table [dbo].[ServiceRoomBridging]    Script Date: 3/3/2026 11:04:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceRoomBridging](
	[RoomID] [varchar](10) NOT NULL,
	[SRBridgingType] [varchar](20) NOT NULL,
	[BridgingID] [varchar](36) NOT NULL,
	[BridgingName] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[LastUpdateDateTime] [datetime] NULL,
	[LastUpdateByUserID] [varchar](15) NULL,
 CONSTRAINT [PK_ServiceRoomBridging] PRIMARY KEY CLUSTERED 
(
	[RoomID] ASC,
	[SRBridgingType] ASC,
	[BridgingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


