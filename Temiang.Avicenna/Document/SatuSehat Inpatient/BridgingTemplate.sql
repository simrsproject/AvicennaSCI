/****** Object:  Table [dbo].[BridgingTemplate]    Script Date: 14/11/2025 09:18:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BridgingTemplate](
	[TemplateID] [int] NOT NULL,
	[TemplateName] [varchar](100) NOT NULL,
 CONSTRAINT [PK_BridgingTemplate] PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [BridgingTemplate]([TemplateID],[TemplateName])
VALUES(100,N'Satu Sehat Rawat Inap');

