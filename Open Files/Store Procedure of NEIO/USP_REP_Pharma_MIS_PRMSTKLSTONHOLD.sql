IF EXISTS(SELECT * FROM SYSOBJECTS WHERE XTYPE='P' AND [NAME] = 'USP_REP_Pharma_MIS_PRMSTKLSTONHOLD')
BEGIN
 DROP PROCEDURE USP_REP_Pharma_MIS_PRMSTKLSTONHOLD
END
Go

/****** Object:  StoredProcedure [dbo].[USP_REP_Pharma_MIS_PRMSTKLSTONHOLD]    Script Date: 12/04/2018 17:16:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[USP_REP_Pharma_MIS_PRMSTKLSTONHOLD]
@SDATE  SMALLDATETIME,@EDATE SMALLDATETIME
,@SIT AS VARCHAR(60),@EIT AS VARCHAR(60)
AS
SET QUOTED_IDENTIFIER OFF
Declare @FCON as NVARCHAR(2000),@VSAMT DECIMAL(14,4),@VEAMT DECIMAL(14,4)
EXECUTE   USP_REP_FILTCON 
@VTMPAC ='',@VTMPIT ='',@VSPLCOND =''
,@VSDATE=NULL
,@VEDATE=@EDATE
,@VSAC =null,@VEAC =null
,@VSIT=@SIT,@VEIT=@EIT
,@VSAMT=0.00,@VEAMT=0.00
,@VSDEPT='',@VEDEPT=''
,@VSCATE ='',@VECATE =''
,@VSWARE ='',@VEWARE  =''
,@VSINV_SR ='',@VEINV_SR =''
,@VMAINFILE='STKL_VW_MAIN',@VITFILE='STKL_VW_ITEM',@VACFILE=' '
,@VDTFLD ='DATE'
,@VLYN =NULL
,@VEXPARA=NULL
,@VFCON =@FCON OUTPUT


DECLARE @SQLCOMMAND NVARCHAR(4000),@VCOND NVARCHAR(4000)

SELECT  STKL_VW_ITEM.ENTRY_TY,BEH='  ',STKL_VW_ITEM.DATE,STKL_VW_ITEM.AC_ID,STKL_VW_ITEM.IT_CODE,STKL_VW_ITEM.QTY,STKL_VW_ITEM.U_LQTY,STKL_VW_ITEM.ITSERIAL,STKL_VW_ITEM.PMKEY,IT_MAST.ITGRID,IT_MAST.[GROUP],IT_MAST.IT_NAME,IT_MAST.CHAPNO,IT_MAST.TYPE,IT_MAST.RATEUNIT,STKL_VW_ITEM.BATCHNO,STKL_VW_ITEM.MFGDT,STKL_VW_ITEM.EXPDT 
,STKL_VW_MAIN.TRAN_CD					--Added by Shrikant S. on 19/09/2018 for Installer ENT 2.0.0
,psm.Schedule,it_mast.prRackNo,psm.salt   --Added By Prajakta B. on 04/12/2018 for Bug 32062
INTO #TITEM FROM STKL_VW_ITEM 
INNER JOIN STKL_VW_MAIN  ON (STKL_VW_ITEM.TRAN_CD=STKL_VW_MAIN.TRAN_CD )
INNER JOIN IT_MAST  ON (IT_MAST.IT_CODE=STKL_VW_ITEM.IT_CODE)
INNER JOIN AC_MAST  ON (AC_MAST.AC_ID=STKL_VW_MAIN.AC_ID)
INNER JOIN LCODE  ON (STKL_VW_ITEM.ENTRY_TY=LCODE.ENTRY_TY)
Left Join pRetSalt_Master psm on psm.Salt=it_mast.prsalt --Added By Prajakta B. on 04/12/2018 for Bug 32062
WHERE 1=2



SET @SQLCOMMAND='INSERT INTO  #TITEM SELECT  STKL_VW_ITEM.ENTRY_TY,BEH=(CASE WHEN LCODE.EXT_VOU=1 THEN LCODE.BCODE_NM ELSE LCODE.ENTRY_TY END),STKL_VW_ITEM.DATE,STKL_VW_ITEM.AC_ID,STKL_VW_ITEM.IT_CODE,STKL_VW_ITEM.QTY,STKL_VW_ITEM.U_LQTY,STKL_VW_ITEM.ITSERIAL,STKL_VW_ITEM.PMKEY,IT_MAST.ITGRID,IT_MAST.[GROUP],IT_MAST.IT_NAME,IT_MAST.CHAPNO,IT_MAST.TYPE,IT_MAST.RATEUNIT,STKL_VW_ITEM.BATCHNO,STKL_VW_ITEM.MFGDT,STKL_VW_ITEM.EXPDT'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'  ,STKL_VW_MAIN.TRAN_CD'		
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' ,psm.Schedule,it_mast.prRackNo,psm.salt'  --Added By Prajakta B. on 04/12/2018 for Bug 32062
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'  FROM STKL_VW_ITEM '
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' INNER JOIN STKL_VW_MAIN  ON (STKL_VW_ITEM.TRAN_CD=STKL_VW_MAIN.TRAN_CD AND STKL_VW_ITEM.ENTRY_TY=STKL_VW_MAIN.ENTRY_TY)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' INNER JOIN IT_MAST  ON (IT_MAST.IT_CODE=STKL_VW_ITEM.IT_CODE)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' INNER JOIN AC_MAST  ON (AC_MAST.AC_ID=STKL_VW_MAIN.AC_ID)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' INNER JOIN LCODE  ON (STKL_VW_ITEM.ENTRY_TY=LCODE.ENTRY_TY)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' INNER JOIN PRETBATCHDETAILS B ON (STKL_VW_ITEM.BATCHNO=B.BATCHNO AND STKL_VW_ITEM.IT_CODE=B.IT_CODE)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' Left Join pRetSalt_Master psm on psm.Salt=it_mast.prsalt' --Added By Prajakta B. on 04/12/2018 for Bug 32062
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+RTRIM(@FCON)
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' AND STKL_VW_ITEM.BATCHNO<>'+CHAR(39)+SPACE(1)+CHAR(39)+' AND STKL_VW_ITEM.DC_NO=SPACE(1)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' AND B.BATCHONHOLD=1'
PRINT @SQLCOMMAND
EXECUTE SP_EXECUTESQL @SQLCOMMAND


SELECT DESCRIPTION=A.IT_NAME,A.RATEUNIT,A.BATCHNO,A.MFGDT,A.EXPDT
,OPBAL =SUM(CASE WHEN (A.BEH ='OS' OR A.DATE<@SDATE) THEN (CASE WHEN A.PMKEY='+' THEN A.QTY ELSE -A.QTY END) ELSE 0 END)
,RQTY_PT =SUM(CASE WHEN A.BEH IN ('PT','AR') AND A.PMKEY='+' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,RQTY_OP=SUM(CASE WHEN A.BEH ='OP' AND A.PMKEY='+' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,RQTY_IR=SUM(CASE WHEN A.BEH ='IR' AND A.PMKEY='+' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,RQTY_SR=SUM(CASE WHEN A.BEH ='SR' AND A.PMKEY='+' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,RQTY_OT=SUM(CASE WHEN A.BEH NOT IN ('OS','PT','OP','IR','SR','AR') AND A.PMKEY='+' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,IQTY_ST =SUM(CASE WHEN A.BEH IN ('ST','DC') AND A.PMKEY='-' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,IQTY_IP   =SUM(CASE WHEN A.BEH ='IP' AND A.PMKEY='-' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,IQTY_II    =SUM(CASE WHEN A.BEH ='II' AND A.PMKEY='-' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,IQTY_PR =SUM(CASE WHEN A.BEH ='PR' AND A.PMKEY='-' AND A.DATE>=@SDATE  THEN A.QTY ELSE 0 END)
,IQTY_OT=SUM(CASE WHEN A.BEH NOT IN ('ST','IP','II','PR','DC') AND A.PMKEY='-' AND A.DATE>=@SDATE THEN A.QTY ELSE 0 END)
,CLBAL =SUM(CASE WHEN  A.PMKEY='+' THEN A.QTY ELSE -A.QTY END)
,A.prRackNo,A.Schedule,A.salt
FROM #TITEM A
GROUP BY A.IT_NAME,A.RATEUNIT,A.BATCHNO,A.MFGDT,A.EXPDT,A.prRackNo,A.Schedule,A.salt
ORDER BY A.IT_NAME,A.RATEUNIT,A.BATCHNO


DROP TABLE #TITEM

