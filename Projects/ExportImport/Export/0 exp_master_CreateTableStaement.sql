/****** Object:  Table [dbo].[exp_master]    Script Date: 12/13/2011 15:49:10 ******/
IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[exp_master]') AND type in (N'U'))
begin
	CREATE TABLE [dbo].[exp_master](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Category] [varchar](20)  NOT NULL,
		[Description] [varchar](30)  NOT NULL,
		[Code] [varchar](3)  NOT NULL,
		[Tables] [varchar](1000)  NOT NULL,
		[ExpLocDet] [varchar](1000)  NULL,
		[Export] [bit] NULL,
		[SortOrd] int NULL Default 0,
		[ExpDataVol] [varchar](30)  NULL,
		[LExpUserNm] [varchar](30)  NULL,
		[LastExpDate] [datetime] NULL,
		[ExpNote] [varchar](4000)  NULL,
		[Sp_Name] [varchar](60)  NULL DEFAULT ('')
	) ON [PRIMARY]
end 

IF NOT EXISTS(SELECT [Code] FROM exp_master WHERE Code = 'IT')BEGIN 	INSERT INTO exp_master([CATEGORY],[DESCRIPTION],[CODE],[TABLES],[EXPLOCDET],[EXPORT],[EXPDATAVOL],[LEXPUSERNM],[LASTEXPDATE],[EXPNOTE],[SP_NAME]) 	VALUES ('Master','ITEM and Group Master','IT','<<ITEM_GROUP##ExpCode<`#`+IT_GROUP_NAME>>><<IT_MAST##ExpCode<`#`+IT_Name>>>','<<AV1>><<AV2>>',1,'Full','','01/01/1900 12:00:00 AM','','Usp_DataExport_ITEM_Master') END 

IF NOT EXISTS(SELECT [Code] FROM exp_master WHERE Code = 'AG')BEGIN 	INSERT INTO exp_master([CATEGORY],[DESCRIPTION],[CODE],[TABLES],[EXPLOCDET],[EXPORT],[EXPDATAVOL],[LEXPUSERNM],[LASTEXPDATE],[EXPNOTE],[SP_NAME]) 	VALUES ('Master','Account Group Master','AG','<<Ac_Group_Mast##ExpCode<`#`+Ac_Group_Name>>>','<<AV1>><<AV2>>',1,'Full','','01/01/1900 12:00:00 AM','','Usp_DataExport_Ac_Group_Master') END IF NOT EXISTS(SELECT [Code] FROM exp_master WHERE Code = 'AM')BEGIN 	INSERT INTO exp_master([CATEGORY],[DESCRIPTION],[CODE],[TABLES],[EXPLOCDET],[EXPORT],[EXPDATAVOL],[LEXPUSERNM],[LASTEXPDATE],[EXPNOTE],[SP_NAME]) 	VALUES ('Master','Account Master','AM','<<Ac_Mast##ExpCode<`#`+Ac_Name>>><<ShipTo##ExpCode<`#`+MailName+`#`+Location_Id>>>','<<AV1>><<AV2>>',1,'Full','','01/01/1900 12:00:00 AM','','Usp_DataExport_Account_Master') END IF NOT EXISTS(SELECT [Code] FROM exp_master WHERE Code = 'AS')BEGIN 	INSERT INTO exp_master([CATEGORY],[DESCRIPTION],[CODE],[TABLES],[EXPLOCDET],[EXPORT],[EXPDATAVOL],[LEXPUSERNM],[LASTEXPDATE],[EXPNOTE],[SP_NAME]) 	VALUES ('Master','Series Master','AS','<<Series##ExpCode<`#`+Inv_Sr>>>','<<AV1>><<AV2>>',1,'Full','','01/01/1900 12:00:00 AM','','Usp_DataExport_Series_Master') END IF NOT EXISTS(SELECT [Code] FROM exp_master WHERE Code = 'AR')BEGIN 	INSERT INTO exp_master([CATEGORY],[DESCRIPTION],[CODE],[TABLES],[EXPLOCDET],[EXPORT],[EXPDATAVOL],[LEXPUSERNM],[LASTEXPDATE],[EXPNOTE],[SP_NAME]) 	VALUES ('Transaction','Goods Receipt','AR','<<ARMain##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)>>><<ARItem##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)+`#`+ItSerial>>><<Manu_det##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)+`#`+ItSerial>>><<GEN_SRNO##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)+`#`+ItSerial+`#`+CType>>>','<<AV1>><<AV2>>',1,'Updated','',' ','','Usp_DataExport_AR') END IF NOT EXISTS(SELECT [Code] FROM exp_master WHERE Code = 'DC')BEGIN 	INSERT INTO exp_master([CATEGORY],[DESCRIPTION],[CODE],[TABLES],[EXPLOCDET],[EXPORT],[EXPDATAVOL],[LEXPUSERNM],[LASTEXPDATE],[EXPNOTE],[SP_NAME]) 	VALUES ('Transaction','Delivery Challan','DC','<<DCMain##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)>>><<DCItem##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)+`#`+ItSerial>>><<LItemAll##DtdFilt##ExpCode<`#`+Entry_ty+`#`+Cast(Tran_cd as Varchar)+`#`+ItSerial>>>','<<AV1>><<AV2>>',1,'Updated','',' ','','Usp_DataExport_DC') END 
exec add_columns 'IT_MAST','DataExport Varchar(200) default '''' with values'
exec add_columns 'ITEM_GROUP','DataExport Varchar(200) default '''' with values'
exec add_columns 'ARMAIN','DataExport Varchar(200) default '''' with values'
exec add_columns 'ARITEM','DataExport Varchar(200) default '''' with values'
exec add_columns 'MANU_DET','DataExport Varchar(200) default '''' with values'
exec add_columns 'AC_Group_Mast','DataExport Varchar(200) default '''' with values'
exec add_columns 'AC_Mast','DataExport Varchar(200) default '''' with values'
exec add_columns 'ShipTO','DataExport Varchar(200) default '''' with values'
exec add_columns 'LITEMALL','DataExport Varchar(200) default '''' with values'
exec add_columns 'DCMAIN','DataExport Varchar(200) default '''' with values'
exec add_columns 'DCITEM','DataExport Varchar(200) default '''' with values'
exec add_columns 'GEN_SRNO','DataExport Varchar(200) default '''' with values'
exec add_columns 'SERIES','DataExport Varchar(200) default '''' with values'

use vudyog
exec add_columns 'co_mast','IE_LOCCODE Varchar(3) default '''' with values'

