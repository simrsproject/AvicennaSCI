/****** Object:  Table [dbo].[SatuSehatILPPreparation]    Script Date: 11/09/2025 09:38:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SatuSehatILPPreparation](
	[RegistrationNo] [varchar](20) NOT NULL,
	[TemplateID] [int] NOT NULL,
	[TestNo] [varchar](10) NOT NULL,
	[Sequence] [int] NOT NULL,
	[AnswerValue] [varchar](100) NOT NULL,
	[AnswerText] [varchar](4000) NOT NULL,
	[PostData] [varchar](max) NOT NULL,
	[IsSent] [bit] NOT NULL,
	[IsError] [bit] NOT NULL,
	[RespondData] [varchar](max) NOT NULL,
	[SentDateTime] [datetime] NULL,
	[CreateByUserID] [varchar](20) NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[LastUpdateByUserID] [varchar](20) NOT NULL,
	[LastUpdateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_SatuSehatILPPreparation] PRIMARY KEY CLUSTERED 
(
	[RegistrationNo] ASC,
	[TemplateID] ASC,
	[TestNo] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


