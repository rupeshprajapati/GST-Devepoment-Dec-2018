SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CURR_MAST]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CURR_MAST](
	[currencyid] [int] IDENTITY(1,1) NOT NULL,
	[currencycd] [varchar](10) NOT NULL,
	[currdesc] [varchar](50) NULL,
	[symbol] [varchar](5) NULL,
	[user_name] [varchar](30) NOT NULL,
	[sysdate] [varchar](30) NOT NULL,
	[compid] [int] NULL,
 CONSTRAINT [PK_CURR_MAST] PRIMARY KEY CLUSTERED 
(
	[currencyid] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CURR_RATE]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CURR_RATE](
	[currencyid] [int] NOT NULL,
	[currdate] [datetime] NOT NULL,
	[impcurrrate] [numeric](18, 4) NOT NULL,
	[expcurrrate] [numeric](18, 4) NOT NULL,
	[remark] [varchar](10) not null,
	[user_name] [varchar](30) NOT NULL,
	[sysdate] [varchar](30) NOT NULL,
	[compid] [int] NULL,
 CONSTRAINT [PK_CURR_RATE] PRIMARY KEY CLUSTERED 
(
	[currencyid] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CURR_RATE_CURR_MAST]') AND parent_object_id = OBJECT_ID(N'[dbo].[CURR_RATE]'))
ALTER TABLE [dbo].[CURR_RATE]  WITH CHECK ADD  CONSTRAINT [FK_CURR_RATE_CURR_MAST] FOREIGN KEY([currencyid])
REFERENCES [dbo].[CURR_MAST] ([currencyid])
