SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ItemImmunization](
	[ItemID] [varchar](10) NOT NULL,
	[ImmunizationID] [varchar](3) NOT NULL,
	[LastUpdateDateTime] [datetime] NULL,
	[LastUpdateByUserID] [varchar](40) NULL,
 CONSTRAINT [PK_ItemImmunization] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[ImmunizationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ItemImmunization] ADD  CONSTRAINT [DF_ItemImmunization_ItemID]  DEFAULT ('') FOR [ItemID]
GO