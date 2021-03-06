IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USP_ENT_DUTY_DEBIT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[USP_ENT_DUTY_DEBIT]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ruepesh Prajapati
-- Create date: 10/06/2009
-- Description:	This Stored procedure is useful in Auto Duty Debit utility project uedutydebit.app.
-- Modified By: 
-- Modify date: 
-- Remark:
-- =============================================
CREATE PROCEDURE        [dbo].[USP_ENT_DUTY_DEBIT]
@input varchar(30),@SDATE  SMALLDATETIME
,@ENTRY_TY VARCHAR(2),@TRAN_CD INT
,@SINV_SR AS VARCHAR(60)
,@SCATE AS VARCHAR(60)
,@SDEPT AS VARCHAR(60)
AS

SET QUOTED_IDENTIFIER OFF
DECLARE @FCON AS NVARCHAR(2000)
DECLARE @SQLCOMMAND NVARCHAR(4000),@VCOND NVARCHAR(1000)
DECLARE @FDATE VARCHAR(15)
SELECT @FDATE=CASE WHEN DBDATE=1 THEN 'DATE' ELSE 'U_CLDT' END FROM MANUFACT

EXECUTE   USP_REP_FILTCON 
@VTMPAC =null,@VTMPIT =null,@VSPLCOND =''
,@VSDATE=@SDATE
,@VEDATE=null
,@VSAC =null,@VEAC =null
,@VSIT=null,@VEIT=null
,@VSAMT=0,@VEAMT=0
,@VSDEPT=@SDEPT,@VEDEPT=''
,@VSCATE =@SCATE,@VECATE =''
,@VSWARE ='',@VEWARE  =''
,@VSINV_SR =@SINV_SR,@VEINV_SR =''
,@VMAINFILE='m',@VITFILE='ac',@VACFILE=' '
,@VDTFLD ='u_cldt'--@FDATE
,@VLYN=''
,@VEXPARA=NULL
,@VFCON =@FCON OUTPUT

select srno,shortnm,ac_name into #er_excise from er_excise where 1=2

IF  CHARINDEX('m.U_CLDT', @FCON)<>0
BEGIN
	SET @FCON=REPLACE(@FCON, 'm.U_CLDT','ac.U_CLDT')
END

if (@input='RG23A')
begin
	print 'RG23A'
	--SET @FCON=rtrim(@FCON)+' AND EX.CRAC=1 AND AC_MAST.AC_NAME LIKE ''%RG23A%''  AND AC_MAST.AC_NAME NOT LIKE ''%ser%'' '
	insert into #er_excise (srno,shortnm,ac_name) select srno,shortnm,ac_name from er_excise where (crac=1) and (ac_name like '%RG23A%')
end
if (@input='RG23C')
begin
	print 'RG23c'
	--SET @FCON=rtrim(@FCON)+' AND EX.CRAC=1 AND AC_MAST.AC_NAME LIKE ''%RG23C%''  AND AC_MAST.AC_NAME NOT LIKE ''%ser%'' '
	insert into #er_excise (srno,shortnm,ac_name) select srno,shortnm,ac_name from er_excise where (crac=1) and (ac_name like '%RG23C%')
end
if (@input='PLA')
begin
	print 'PLA'
	--SET @FCON=rtrim(@FCON)+' AND EX.CURAC=1 AND AC_MAST.AC_NAME LIKE ''%PLA%''  AND AC_MAST.AC_NAME NOT LIKE ''%ser%'' '
	insert into #er_excise (srno,shortnm,ac_name) select srno,shortnm,ac_name from er_excise where (curac=1) and (ac_name like '%PLA%')
end
if (@input='SERVICE TAX')
begin
	print 'SERVICE'
	--SET @FCON=rtrim(@FCON)+' AND AC_MAST.AC_NAME LIKE ''%ser%'' '
	insert into #er_excise (srno,shortnm,ac_name) select srno,shortnm,ac_name from er_excise where (crac=1) and (ac_name like '%SERVICE TAX%')
end

select sel=cast (0 as bit)
,ex.srno,ex.shortnm,ac.ac_id,ac_mast.ac_name
,balamt =AC.AMOUNT,mbalamt =AC.AMOUNT
,adjamt=AC.AMOUNT
into #exdebit
from ex_vw_acdet ac  
inner join ac_mast on (ac.ac_id=ac_mast.ac_id)
inner join er_excise ex on (ac_mast.ac_name=ex.ac_name)   
WHERE 1=2

--select * from #exdebit


print @fcon
print @FDATE
SET @SQLCOMMAND='insert into #exdebit select sel=cast (0 as bit),ex.srno,ex.shortnm'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' ,ac.ac_id,ac_mast.ac_name,balamt =sum(isnull(CASE WHEN AC.AMT_TY=''DR'' THEN AC.AMOUNT ELSE -AC.AMOUNT END,0)),mbalamt =0'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' ,adjamt=cast(0 as decimal(17,2) )'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' from ex_vw_acdet ac '
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' inner join stkl_vw_main m on (m.entry_ty=ac.entry_ty and m.tran_cd=ac.tran_cd)'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' inner join ac_mast on (ac.ac_id=ac_mast.ac_id)  '
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' inner join #er_excise ex on (ac_mast.ac_name=ex.ac_name)  '
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+RTRIM(@FCON)
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+' AND NOT (M.ENTRY_TY='+CHAR(39)+@ENTRY_TY+CHAR(39)+' AND M.TRAN_CD='+LTRIM(CAST(@TRAN_CD AS VARCHAR))+')'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'  group by ex.srno,ex.shortnm,ac.ac_id,ac_mast.ac_name'
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'  HAVING SUM(CASE WHEN AC.AMT_TY='+'''DR'''+' THEN AC.AMOUNT ELSE -AC.AMOUNT END)<>0 '
SET @SQLCOMMAND=RTRIM(@SQLCOMMAND)+' '+'  order by ex.srno'
PRINT @SQLCOMMAND
EXECUTE SP_EXECUTESQL @SQLCOMMAND

update #exdebit set mbalamt=balamt
insert into #exdebit 
select sel=cast (0 as bit),ex.srno,ex.shortnm  
,ac_mast.ac_id,ac_mast.ac_name
,balamt =0,adjamt=0,mbalamt=0 
from #er_excise ex
inner join ac_mast on (ac_mast.ac_name=ex.ac_name)  
WHERE rtrim(ex.ac_name) not in (select distinct rtrim(ac_name) from #exdebit)

select * from #exdebit

