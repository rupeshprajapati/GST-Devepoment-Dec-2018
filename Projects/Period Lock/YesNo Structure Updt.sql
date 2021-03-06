

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

If Exists (SELECT Top 1 ENTRY_TY FROM [YESNO])
	Begin
		IF NOT Exists (SELECT COLUMN_NAME FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CATE') )
			Begin
				Alter Table [YESNO] ADD [CATE] VARCHAR(20)
				UPDATE [YESNO] SET [CATE]=''
			End

		IF NOT Exists (SELECT COLUMN_NAME FROM SYSCOLUMNS WHERE ID = OBJECT_ID('COMPID') )
			Begin
				Alter Table [YESNO] ADD [COMPID] INT
				UPDATE [YESNO] SET [COMPID]=0
			End

		IF NOT Exists (SELECT COLUMN_NAME FROM SYSCOLUMNS WHERE ID = OBJECT_ID('USER_NAME') )
			Begin
				Alter Table [YESNO] ADD [USER_NAME] VARCHAR(30)
				UPDATE [YESNO] SET [USER_NAME]=''
			End

		IF NOT Exists (SELECT COLUMN_NAME FROM SYSCOLUMNS WHERE ID = OBJECT_ID('SYSDATE') )
			Begin
				Alter Table [YESNO] ADD [SYSDATE] VARCHAR(20)
				UPDATE [YESNO] SET [SYSDATE]=''
			End
	End
Else
	Begin

		DROP TABLE [YESNO]
		GO 
		CREATE TABLE [dbo].[YESNO](
			[entry_ty] [varchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
			[inv_sr] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
			[dept] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
			[cate] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			[lock_from] [datetime] NOT NULL,
			[lock_to] [datetime] NOT NULL,
			[allow] [bit] NOT NULL,
			[CompId] [int] NULL,
			[user_name] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			[sysdate] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
		) ON [PRIMARY]

	End
GO
SET ANSI_PADDING OFF