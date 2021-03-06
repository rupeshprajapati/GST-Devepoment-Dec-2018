use vudyog
/* Add STCoAdditional Table Start */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[STCoAdditional]') AND type in (N'U'))
begin
	DROP TABLE [dbo].[STCoAdditional]
end

go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[STCoAdditional](
	[Fbcode] [varchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FbName] [varchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FBAdd] [varchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SeradjPer] [decimal](6, 2) NULL,
	[SerCode] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Serty] [varchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SREGN] [varchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TypeORG] [varchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Serapl] [bit] NULL,
	[premisecd] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[range] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[rcd] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[division] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[dcd] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[collrate] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ccd] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
select[range] from [STCoAdditional]
if (@@rowcount=0)
begin
	insert into [STCoAdditional] ([Fbcode],[FbName],[FBAdd],[SeradjPer],[SerCode],[Serty],[SREGN],[TypeORG],[Serapl],[premisecd],[range],[rcd],[division],[dcd],[collrate],[ccd]) values ('','','',0,'','','','',1,'','','','','','','')
end
/* Add STCoAdditional Table end */
SET ANSI_PADDING OFF
select * from [STCoAdditional]

 