USE [Vudyog]
GO
/****** Object:  Table [dbo].[co_mast]    Script Date: 02/17/2009 11:32:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'area' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [area] [varchar](12) NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'Zone' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [zone] [varchar](12) NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'City' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [city] [varchar](22) NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'Zip' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [zip] [varchar](7) NULL
End
	
If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'State' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [state] [varchar](30) NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'Country' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [country] [varchar](30) NULL
End	

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'mCur' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [mcur_] [bit] NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'Com_Type' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [com_type] [varchar](1) NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'coFormDt' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [coFormDt] [smalldatetime] NULL
End

If not exists (select * from syscolumns where id = object_id('co_mast') and [name] = 'Punchln' )
Begin
	Alter TABLE [dbo].[co_mast] Add Column [PunchLn] [varChar](100) NULL
End
