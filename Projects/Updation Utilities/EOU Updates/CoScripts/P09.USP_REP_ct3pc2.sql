
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[USP_REP_ct3/pc] 
@TMPAC NVARCHAR(50),@TMPIT NVARCHAR(50),@SPLCOND VARCHAR(8000),@SDATE  SMALLDATETIME,@EDATE SMALLDATETIME
,@SAC AS VARCHAR(60),@EAC AS VARCHAR(60)
,@SIT AS VARCHAR(60),@EIT AS VARCHAR(60)
,@SAMT FLOAT,@EAMT FLOAT
,@SDEPT AS VARCHAR(60),@EDEPT AS VARCHAR(60)
,@SCATE AS VARCHAR(60),@ECATE AS VARCHAR(60)
,@SWARE AS VARCHAR(60),@EWARE AS VARCHAR(60)
,@SINV_SR AS VARCHAR(60),@EINV_SR AS VARCHAR(60)
,@LYN VARCHAR(20)
,@EXPARA  AS VARCHAR(60)= null
AS
SET QUOTED_IDENTIFIER OFF

Declare @FCON as NVARCHAR(2000),@VSAMT DECIMAL(14,2),@VEAMT DECIMAL(14,2)
EXECUTE   USP_REP_FILTCON 
@VTMPAC =NULL,@VTMPIT =NULL,@VSPLCOND =@SPLCOND
,@VSDATE=@SDATE
 ,@VEDATE=@EDATE
,@VSAC =NULL,@VEAC =NULL
,@VSIT=NULL,@VEIT=null
,@VSAMT=@SAMT,@VEAMT=@EAMT
,@VSDEPT=@SDEPT,@VEDEPT=@EDEPT
,@VSCATE =@SCATE,@VECATE =@ECATE
,@VSWARE =@SWARE,@VEWARE  =@EWARE
,@VSINV_SR =@SINV_SR,@VEINV_SR =@SINV_SR
,@VMAINFILE='OBMAIN',@VITFILE='OBITEM',@VACFILE=' '
,@VDTFLD ='DATE'
,@VLYN =NULL
,@VEXPARA=@EXPARA
,@VFCON =@FCON OUTPUT

DECLARE @SQLCOMMAND NVARCHAR(4000), @VCOND NVARCHAR(2000)
SET @SQLCOMMAND = 'SELECT U_EXPBNO,U_CLDT,U_EXBAMT,U_EXBVLDT FROM OBMAIN'
SET @SQLCOMMAND = @SQLCOMMAND+RTRIM(@FCON)
SET @SQLCOMMAND = @SQLCOMMAND+'AND PARTY_NM=''BALANCE WITH B17-BOND''AND OBMAIN.U_EXPBNO<>'''
---SELECT @SQLCOMMAND

EXECUTE SP_EXECUTESQL @SQLCOMMAND
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

