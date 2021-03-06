/****** Object:  StoredProcedure [dbo].[USP_REP_118REC(IND)]    Script Date: 02/05/2010 09:58:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





---<<    USP_REP_118REC(INDIGENIOUS) This Stored Procedure useful to generate ANNE-118(RECEIPT)  Report   >>
CREATE PROCEDURE [dbo].[USP_REP_118REC(IND)] 
@TMPAC NVARCHAR(50),@TMPIT NVARCHAR(50),@SPLCOND VARCHAR(8000),@SDATE  SMALLDATETIME,@EDATE SMALLDATETIME
,@SAC AS VARCHAR(60),@EAC AS VARCHAR(60)
,@SIT AS VARCHAR(60),@EIT AS VARCHAR(60)
,@SAMT FLOAT,@EAMT FLOAT
,@SDEPT AS VARCHAR(60),@EDEPT AS VARCHAR(60)
,@SCATE AS VARCHAR(60),@ECATE AS VARCHAR(60)
,@SWARE AS VARCHAR(60),@EWARE AS VARCHAR(60)
,@SINV_SR AS VARCHAR(60),@EINV_SR AS VARCHAR(60)
,@LYN VARCHAR(20)
,@RULE  AS VARCHAR(25)
,@EXPARA  AS VARCHAR(60)= null

AS

SET QUOTED_IDENTIFIER OFF

Declare @FCON as NVARCHAR(2000),@VSAMT DECIMAL(14,2),@VEAMT DECIMAL(14,2)
EXECUTE   USP_REP_FILTCON 
@VTMPAC =@TMPAC,@VTMPIT =@TMPIT,@VSPLCOND =@SPLCOND
,@VSDATE=@SDATE
,@VEDATE=@EDATE
,@VSAC =@SAC,@VEAC =@EAC
,@VSIT=@SIT,@VEIT=@EIT
,@VSAMT=@SAMT,@VEAMT=@EAMT
,@VSDEPT=@SDEPT,@VEDEPT=@EDEPT
,@VSCATE =@SCATE,@VECATE =@ECATE
,@VSWARE =@SWARE,@VEWARE  =@EWARE
,@VSINV_SR =@SINV_SR,@VEINV_SR =@SINV_SR
,@VMAINFILE='PTMAIN',@VITFILE='PTITEM',@VACFILE='PTACDET'--,@VRULE ='INDIGENIOUS         '
,@VDTFLD ='DATE'
,@VLYN =NULL
,@VEXPARA=@EXPARA
,@VFCON =@FCON OUTPUT

DECLARE @SQLCOMMAND NVARCHAR(4000), @VCOND NVARCHAR(2000)


 SET @SQLCOMMAND='SELECT PTITEM.TRAN_CD,PTITEM.ENTRY_TY,PTITEM.INV_NO,PTITEM.DATE,IT_MAST.IT_NAME,PTITEM.QTY,PTITEM.ITEM_NO,PTITREF.RINV_NO,PTMAIN.U_BENO,PTMAIN.U_BEDATE,PTMAIN.U_PINVNO,PTMAIN.U_PINVDT,PTITEM.U_EXPMARK,PTITEM.U_CIFAMT,PTITEM.EXAMT,PTITEM.U_EXAMT,PTITEM.U_ACAMT1,PTITEM.U_HACAMT1,PTITEM.U_CESSAMT,PTITEM.U_HCESSAMT,PTITEM.U_IMPDUTY,IT_MAST.P_UNIT,PTMAIN.[RULE],ARITREF.RINV_NO,PTITEM.U_ASSEAMT FROM PTITEM '
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'INNER JOIN PTMAIN ON(PTITEM.ENTRY_TY=PTMAIN.ENTRY_TY AND PTITEM.TRAN_CD=PTMAIN.TRAN_CD)'
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'INNER JOIN IT_MAST ON (PTITEM.ITEM=IT_MAST.IT_NAME)'
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'LEFT JOIN PTITREF ON ( PTITEM.TRAN_CD=PTITREF.TRAN_CD) ' 
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'LEFT JOIN ARITEM ON (ARITEM.ENTRY_TY=PTITREF.RENTRY_TY AND ARITEM.DATE=PTITREF.DATE AND ARITEM.INV_NO=PTITREF.RINV_NO)'    
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'LEFT JOIN ARITREF ON (ARITEM.TRAN_CD=ARITREF.TRAN_CD )'
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+RTRIM(@FCON)
 SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'ORDER BY PTITEM.DATE,PTITEM.ITEM'

PRINT @SQLCOMMAND

EXECUTE SP_EXECUTESQL @SQLCOMMAND

IF @@ERROR = 0
BEGIN
PRINT 'USP_REP_118REC(IND) Successfully Created..!'
END


