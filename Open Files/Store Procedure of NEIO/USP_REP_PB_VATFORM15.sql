if Exists(select xtype,name from sysobjects where xtype='P' and name='USP_REP_PB_VATFORM15')
begin
	drop procedure USP_REP_PB_VATFORM15
end
go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
   =====================
    Modify By : Suraj Kumawat for bug-25370 on date 09-05-2015 ----
   =====================
*/

----
create PROCEDURE [dbo].[USP_REP_PB_VATFORM15]
@TMPAC NVARCHAR(50),@TMPIT NVARCHAR(50),@SPLCOND VARCHAR(8000),@SDATE  SMALLDATETIME,@EDATE SMALLDATETIME
,@SAC AS VARCHAR(60),@EAC AS VARCHAR(60)
,@SIT AS VARCHAR(60),@EIT AS VARCHAR(60)
,@SAMT FLOAT,@EAMT FLOAT
,@SDEPT AS VARCHAR(60),@EDEPT AS VARCHAR(60)
,@SCATE AS VARCHAR(60),@ECATE AS VARCHAR(60)
,@SWARE AS VARCHAR(60),@EWARE AS VARCHAR(60)
,@SINV_SR AS VARCHAR(60),@EINV_SR AS VARCHAR(60)
,@LYN VARCHAR(20)
,@EXPARA  AS VARCHAR(60)= NULL
AS

BEGIN
DECLARE @FCON AS NVARCHAR(2000)
EXECUTE   USP_REP_FILTCON 
---
@VTMPAC =@TMPAC,@VTMPIT =@TMPIT,@VSPLCOND =@SPLCOND
,@VSDATE=NULL,@VEDATE=@EDATE
,@VSAC =@SAC,@VEAC =@EAC
,@VSIT=@SIT,@VEIT=@EIT
,@VSAMT=@SAMT,@VEAMT=@EAMT
,@VSDEPT=@SDEPT,@VEDEPT=@EDEPT
,@VSCATE =@SCATE,@VECATE =@ECATE
,@VSWARE =@SWARE,@VEWARE  =@EWARE
,@VSINV_SR =@SINV_SR,@VEINV_SR =@SINV_SR
,@VMAINFILE='M',@VITFILE=Null,@VACFILE='AC'
,@VDTFLD ='DATE'
,@VLYN=Null
,@VEXPARA=@EXPARA
,@VFCON =@FCON OUTPUT

DECLARE @SQLCOMMAND NVARCHAR(4000)
DECLARE @RATE NUMERIC(12,2),@AMTA1 NUMERIC(12,2),@AMTB1 NUMERIC(12,2),@AMTC1 NUMERIC(12,2),@AMTD1 NUMERIC(12,2),@AMTE1 NUMERIC(12,2),@AMTF1 NUMERIC(12,2),@AMTG1 NUMERIC(12,2),@AMTH1 NUMERIC(12,2),@AMTI1 NUMERIC(12,2),@AMTJ1 NUMERIC(12,2),@AMTK1 NUMERIC(12,2),@AMTL1 NUMERIC(12,2),@AMTM1 NUMERIC(12,2),@AMTN1 NUMERIC(12,2),@AMTO1 NUMERIC(12,2)
DECLARE @AMTA2 NUMERIC(12,2),@AMTB2 NUMERIC(12,2),@AMTC2 NUMERIC(12,2),@AMTD2 NUMERIC(12,2),@AMTE2 NUMERIC(12,2),@AMTF2 NUMERIC(12,2),@AMTG2 NUMERIC(12,2),@AMTH2 NUMERIC(12,2),@AMTI2 NUMERIC(12,2),@AMTJ2 NUMERIC(12,2),@AMTK2 NUMERIC(12,2),@AMTL2 NUMERIC(12,2),@AMTM2 NUMERIC(12,2),@AMTN2 NUMERIC(12,2),@AMTO2 NUMERIC(12,2),@AMTP1 NUMERIC(12,2)
DECLARE @PER NUMERIC(12,2),@TAXAMT NUMERIC(12,2),@CHAR INT,@LEVEL NUMERIC(12,2),@AMTA3 NUMERIC(12,2),@AMTA4 NUMERIC(12,2)
DECLARE @BAL_AMT1 DECIMAL(18,2),@BAL_TAX1 DECIMAL(18,2)

SELECT DISTINCT AC_NAME=SUBSTRING(AC_NAME1,2,CHARINDEX('"',SUBSTRING(AC_NAME1,2,100))-1) INTO #VATAC_MAST FROM STAX_MAS WHERE AC_NAME1 NOT IN ('"SALES"','"PURCHASES"') AND ISNULL(AC_NAME1,'')<>''
INSERT INTO #VATAC_MAST SELECT DISTINCT AC_NAME=SUBSTRING(AC_NAME1,2,CHARINDEX('"',SUBSTRING(AC_NAME1,2,100))-1) FROM STAX_MAS WHERE AC_NAME1 NOT IN ('"SALES"','"PURCHASES"') AND ISNULL(AC_NAME1,'')<>''
 

Declare @NetEff as numeric (12,2), @NetTax as numeric (12,2)
Declare @NetEffs as numeric (12,2), @NetTaxs as numeric (12,2)
Declare @NetEffp as numeric (12,2), @NetTaxp as numeric (12,2)

----Temporary Cursor1
SELECT BHENT='PT',M.INV_NO,M.Date,A.AC_NAME,A.AMT_TY,STM.TAX_NAME,SET_APP=ISNULL(SET_APP,0),STM.ST_TYPE,M.NET_AMT,M.GRO_AMT,TAXONAMT=M.GRO_AMT+M.TOT_DEDUC+M.TOT_TAX+M.TOT_EXAMT+M.TOT_ADD,PER=STM.LEVEL1,MTAXAMT=M.TAXAMT,TAXAMT=A.AMOUNT,STM.FORM_NM,PARTY_NM=AC1.AC_NAME,AC1.S_TAX,M.U_IMPORM
,ADDRESS=LTRIM(AC1.ADD1)+ ' ' + LTRIM(AC1.ADD2) + ' ' + LTRIM(AC1.ADD3),M.TRAN_CD,VATONAMT=99999999999.99,Dbname=space(20),ItemType=space(1),It_code=999999999999999999-999999999999999999,ItSerial=Space(5)
INTO #FORM_15
FROM PTACDET A 
INNER JOIN PTMAIN M ON (A.ENTRY_TY=M.ENTRY_TY AND A.TRAN_CD=M.TRAN_CD)
INNER JOIN STAX_MAS STM ON (M.TAX_NAME=STM.TAX_NAME)
INNER JOIN AC_MAST AC ON (A.AC_NAME=AC.AC_NAME)
INNER JOIN AC_MAST AC1 ON (M.AC_ID=AC1.AC_ID)
WHERE 1=2 --A.AC_NAME IN ( SELECT AC_NAME FROM #VATAC_MAST)

alter table #FORM_15 add recno int identity

---Temporary Cursor2
SELECT PART=3,PARTSR='AAA',SRNO='AAA',RATE=99.999,AMT1=NET_AMT,AMT2=M.TAXAMT,AMT3=M.TAXAMT,AMT4=NET_AMT,
M.INV_NO,M.DATE,PARTY_NM=AC1.AC_NAME,ADDRESS=Ltrim(AC1.Add1)+' '+Ltrim(AC1.Add2)+' '+Ltrim(AC1.Add3),STM.FORM_NM,AC1.S_TAX
INTO #FORM15
FROM PTACDET A 
INNER JOIN STMAIN M ON (A.ENTRY_TY=M.ENTRY_TY AND A.TRAN_CD=M.TRAN_CD)
INNER JOIN STAX_MAS STM ON (M.TAX_NAME=STM.TAX_NAME)
INNER JOIN AC_MAST AC ON (A.AC_NAME=AC.AC_NAME)
INNER JOIN AC_MAST AC1 ON (M.AC_ID=AC1.AC_ID)
WHERE 1=2

Declare @MultiCo	VarChar(3)
Declare @MCON as NVARCHAR(2000)
IF Exists(Select A.ID From SysObjects A Inner Join SysColumns B On(A.ID = B.ID) Where A.[Name] = 'STMAIN' And B.[Name] = 'DBNAME')
	Begin	------Fetch Records from Multi Co. Data
		 Set @MultiCo = 'YES'
		 EXECUTE USP_REP_MULTI_CO_DATA
		  @TMPAC, @TMPIT, @SPLCOND, @SDATE, @EDATE
		 ,@SAC, @EAC, @SIT, @EIT, @SAMT, @EAMT
		 ,@SDEPT, @EDEPT, @SCATE, @ECATE,@SWARE
		 ,@EWARE, @SINV_SR, @EINV_SR, @LYN, @EXPARA
		 ,@MFCON = @MCON OUTPUT

		--SET @SQLCOMMAND='Select * from '+@MCON
		---EXECUTE SP_EXECUTESQL @SQLCOMMAND
		SET @SQLCOMMAND='Insert InTo #FORM_15 Select * from '+@MCON
		EXECUTE SP_EXECUTESQL @SQLCOMMAND
		---Drop Temp Table 
		SET @SQLCOMMAND='Drop Table '+@MCON
		EXECUTE SP_EXECUTESQL @SQLCOMMAND
	End
else
	Begin ------Fetch Single Co. Data
		 Set @MultiCo = 'NO'
		 EXECUTE USP_REP_SINGLE_CO_DATA_VAT
		  @TMPAC, @TMPIT, @SPLCOND, @SDATE, @EDATE
		 ,@SAC, @EAC, @SIT, @EIT, @SAMT, @EAMT
		 ,@SDEPT, @EDEPT, @SCATE, @ECATE,@SWARE
		 ,@EWARE, @SINV_SR, @EINV_SR, @LYN, @EXPARA
		 ,@MFCON = @MCON OUTPUT
	End

/* 1 SALES DETAILS */
--( a )Gross Sales
 SET @BAL_AMT1 = 0 /* its used for balance */
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(SUM(GRO_AMT),0) FROM VATTBL WHERE BHENT in ('ST')  AND (DATE BETWEEN @SDATE AND @EDATE) AND U_IMPORM NOT IN('Branch Transfer','Consignment Transfer')
 SET @BAL_AMT1 =  @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','A',0,@AMTA1,0,0,0,'')

--Less : Sales within the State by the Exempted Units
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0) FROM VATTBL A  INNER JOIN AC_MAST B ON A.AC_ID =B.AC_ID WHERE A.BHENT ='ST'
 AND A.ST_TYPE IN('LOCAL','') AND (A.DATE BETWEEN @SDATE AND @EDATE) AND B.U_EXEUNIT=1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','B',0,@AMTA1,0,0,0,'')

--Less : Zero rated sales
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0) FROM VATTBL A  INNER JOIN AC_MAST B ON A.AC_ID =B.AC_ID WHERE A.BHENT ='ST'
 AND A.TAX_NAME <> '' AND A.PER= 0 AND (A.DATE BETWEEN @SDATE AND @EDATE) AND B.U_EXEUNIT<>1 AND A.ST_TYPE NOT IN('LOCAL','')
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','C',0,@AMTA1,0,0,0,'')

--Less : Inter- state sales	
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id) WHERE A.BHENT ='ST'  AND A.ST_TYPE = 'OUT OF STATE'
 AND (A.DATE BETWEEN @SDATE AND @EDATE)
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','D',0,@AMTA1,0,0,0,'')

--Less : Tax free sales			
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id) WHERE A.BHENT ='ST' 
 AND (A.DATE BETWEEN @SDATE AND @EDATE) AND B.U_EXEUNIT<>1 AND A.TAX_NAME ='' AND A.PER = 0 AND A.ST_TYPE NOT IN('LOCAL','')
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','E',0,@AMTA1,0,0,0,'')

--Less : Sales return, Cash/trade discount					
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(SUM(GRO_AMT),0) FROM VATTBL WHERE BHENT ='SR'  AND (DATE BETWEEN @SDATE AND @EDATE)
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','F',0,@AMTA1,0,0,0,'')

--Less : Tax element included in sales	
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(sum(TAXAMT),0) FROM VATTBL  WHERE  BHENT ='ST' And ST_TYPE IN('LOCAL','') AND (DATE BETWEEN @SDATE AND @EDATE)
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','G',0,@AMTA1,0,0,0,'')

 ---Less : Purchase Value of sale of goods, purchased from exempted units and sold to persons other than taxable persons
 SET @AMTA1= 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','H',0,@AMTA1,0,0,0,'')

---Any other deduction, please specify					
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','I',0,@AMTA1,0,0,0,'')
   /* Net Sales subject to VAT */ 
 SET @AMTA1= 0
 SET @AMTA1 = @BAL_AMT1 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'1','J',0,@AMTA1,0,0,0,'')

/*2 PURCHASE DETAILS */
  SET @BAL_AMT1 = 0
  SET @BAL_TAX1 =0
--Gross Purchases (including capital goods and goods received by stock transfer)
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1=ISNULL(SUM(GRO_AMT),0),@AMTA2=ISNULL(SUM(TAXAMT),0) FROM VATTBL WHERE BHENT in ('PT','EP','P1')  AND (DATE BETWEEN @SDATE AND @EDATE)
 SET @BAL_AMT1 = @AMTA1
 SET @BAL_TAX1 =@AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','A',0,@AMTA1,0,0,0,'')

--Less : Import from outside of India
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0),@AMTA2=ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON ( A.AC_ID =B.Ac_id )  WHERE A.BHENT='P1' and (A.DATE BETWEEN @SDATE AND @EDATE) AND B.U_EXEUNIT<> 1
 SELECT @AMTA1= @AMTA1 + ISNULL(SUM(A.GRO_AMT),0),@AMTA2= @AMTA2 + ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id) WHERE A.BHENT='PT' and (A.DATE BETWEEN @SDATE AND @EDATE) and A.U_IMPORM ='Import from Outside India' AND B.U_EXEUNIT <> 1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','B',0,@AMTA1,0,0,0,'')

--Less : Inter-State purchases/goods received from Branches/principals
 SET @AMTA1=0
 SET @AMTA2=0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0),@AMTA2=ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID=B.Ac_id) WHERE A.BHENT in ('PT','EP') And A.St_type = 'Out of State' AND (A.DATE BETWEEN @SDATE AND @EDATE)
 AND A.U_IMPORM in('Branch Transfer','Consignment Transfer') AND B.U_EXEUNIT <> 1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','C',0,@AMTA1,0,0,0,'')

--Less : Purchases made directly from exempted units
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1=isnull(SUM(A.GRO_AMT),0),@AMTA2=isnull(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id)  WHERE A.BHENT in ('PT','EP')  AND (A.DATE BETWEEN @SDATE AND @EDATE) AND B.U_EXEUNIT=1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','D',0,@AMTA1,0,0,0,'')

--Less : Tax free Purchases
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1=isnull(SUM(A.GRO_AMT),0),@AMTA2=isnull(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id)  WHERE A.BHENT in ('PT','EP')  AND (A.DATE BETWEEN @SDATE AND @EDATE) AND B.U_EXEUNIT=1 AND A.TAX_NAME = '' AND A.PER = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','E',0,@AMTA1,0,0,0,'')

--Less : Purchases from persons other than taxable persons
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1=isnull(SUM(A.GRO_AMT),0),@AMTA2=isnull(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id) WHERE A.BHENT in ('PT','EP')  AND (A.DATE BETWEEN @SDATE AND @EDATE) and A.s_tax = ''  and A.st_type IN('Local','') AND B.U_EXEUNIT <> 1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','F',0,@AMTA1,0,0,0,'')

--Against Form 'H' Purchases
 SET @AMTA1=0
 SET @AMTA2=0
 SELECT @AMTA1=ISNULL(SUM(A.GRO_AMT),0),@AMTA2=ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON (A.AC_ID =B.Ac_id ) WHERE A.BHENT in ('PT','EP','P1')  AND (A.DATE BETWEEN @SDATE AND @EDATE) and A.Form_nm IN('FORM H','H FORM','H') AND B.U_EXEUNIT <>1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','G',0,@AMTA1,0,0,0,'')

--Less : Purchases liable to purchase tax u/s 19(1) and 20 in the hands of the person filing the return
  --- Pending for ca discusstion 
 SET @BAL_AMT1 = @BAL_AMT1 - 0
 SET @BAL_TAX1 =@BAL_TAX1 - 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','H',0,0,0,0,0,'')

--Less : Purchases not eligible for input tax credit under section 13(5)
	--pending for ca discussion 
 SET @AmtA1=0
 SET @AmtA2=0
 --Select @AmtA1=ISNULL(sum(GRO_amt),0) from VATTBL where Bhent in('EP','PT') and (Date Between @Sdate and @Edate) and St_type = 'LOCAL' and Set_App = 0  and len(rtrim(u_imporm)) = 0 and TAX_NAME <>'EXEMPTED' AND S_TAX<>' '
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','I',0,@AMTA1,0,0,0,'')

--Less : Purchases return and cash /trade discount
 SET @AMTA1= 0
 SET @AMTA2= 0
 SELECT @AMTA1=ISNULL(SUM(GRO_AMT),0),@AMTA2=ISNULL(SUM(TAXAMT),0) FROM VATTBL WHERE BHENT in ('PR')  AND (DATE BETWEEN @SDATE AND @EDATE)
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 -  @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','J',0,@AMTA1,0,0,0,'')

--Any Other Deduction
	--pnding for ca discussion
 SET @AMTA1=0
 SET @AMTA2=0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 -  @AMTA2
 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','K',0,@AMTA1,0,0,0,'')
 
  --Net : purchases eligible for input tax credit [a-(b+c+d+e+f+g+h+i+j+k)]
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'2','L',0,@BAL_AMT1,@BAL_TAX1,0,0,'')

--- Part 3
 --Total Output Tax
 SET @BAL_AMT1 = 0
 --VAT on net taxable sales during the return period
 SET @AMTA1 = 0 
 --SELECT @AMTA2=AMT1 FROM #FORM15 WHERE Partsr = '1' and srno = 'G'
SELECT @AMTA1 =ISNULL(SUM(TAXAMT),0) FROM	VATTBL WHERE BHENT='ST'  AND ST_TYPE IN('LOCAL','') AND (DATE BETWEEN @SDATE AND @EDATE)
SET @BAL_AMT1 = @AMTA1
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'3','A',0,@AMTA1,0,0,0,'')

 --Add : Purchase Tax  on turnover as per Col. 2(f)
 SET @AMTA1=0
 --Select @AMTA1=sum(AMT1) from #FORM15 where Partsr = '2' and srno = 'H'
 SELECT @AMTA1 =ISNULL(SUM(TAXAMT),0) FROM	VATTBL WHERE BHENT='PT'  AND S_TAX=' ' AND ST_TYPE IN('LOCAL','') AND (DATE BETWEEN @SDATE AND @EDATE) 
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'3','B',0,@AMTA1,0,0,0,'')
 --Add/Less : Output tax for prior period adjustment.
 SET @BAL_AMT1 = @BAL_AMT1 - 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'3','C',0,0,0,0,0,'')
 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'3','D',0,@BAL_AMT1,0,0,0,'')

--- Part 4
SET @BAL_AMT1 = 0
 --(a)ITC brought forward from previous return period
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','A',0,0,0,0,0,'')
 --(b)Add : Installment of ITC on stock held on appointed day
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','B',0,0,0,0,0,'')
 --(c)Add : TDS against Tax Deduction Certificate
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','C',0,0,0,0,0,'')
 --(d)Add : ITC on purchases made during the period as per col.2(l)
 SET @AMTA1 = 0
 SELECT @AMTA1=ISNULL(sum(AMT2),0) FROM #FORM15  WHERE  PART=1 AND PARTSR='2' AND SRNO='l'
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','D',0,@AMTA1,0,0,0,'')
 --(e)Add : ITC, debited earlier, on goods received back after job work u/s 13(3)
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','E',0,@AMTA1,0,0,0,'')
 --(f)Less           Add/Less : Prior period net adjustment to input tax
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','F',0,@AMTA1,0,0,0,'')
 --(g)Add : Any other, please specify
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','G',0,@AMTA1,0,0,0,'')
 --(h)Total input tax credit available
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','H',0,@BAL_AMT1,0,0,0,'')
--(i)Less : Appointment of ITC on manufacturing tax free goods
 SET @BAL_AMT1 = 0
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','I',0,@AMTA1,0,0,0,'')
 --(j)Less : Appointment of ITC from branch transfer
 SET @AMTA1 = 1111
 select @AMTA1 = isnull(SUM(taxamt),0) from VATTBL where BHENT in('PT','EP') AND U_IMPORM in('Branch Transfer')
 AND (DATE BETWEEN @SDATE AND @EDATE)
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','J',0,@AMTA1,0,0,0,'')
--(k)Less : Appointment of ITC u/s 13(4)
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','K',0,@AMTA1,0,0,0,'')
 --(l)Less : ITC on goods send for job work u/s 13(3)
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM)VALUES (1,'4','L',0,@AMTA1,0,0,0,'')
--(m)Less : Reversal of ITC

 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','M',0,@AMTA1,0,0,0,'')
--(n)Less : Any other, please specify
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','N',0,@AMTA1,0,0,0,'')
 --(o)Total (i+j+k+l+m+n)
 SET @AMTA1 = 0
 SET @AMTA1 = @BAL_AMT1 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','O',0,@AMTA1,0,0,0,'')
 SET @AMTA1 = 0
 select @AMTA1 =ISNULL(SUM(case when srno='H' THEN AMT1 ELSE -AMT1 END),0) FROM #FORM15 WHERE SRNO IN('H','O') AND PART=1 AND PARTSR='4'
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'4','P',0,@AMTA1,0,0,0,'')

--- Part 5
set @BAL_AMT1 = 0
SET @BAL_TAX1 =0
 ---(A)Total Purchase made during the return period [as per col.2(d)]
 set @AMTA1 =0
 set @AMTA2 =0
 SELECT @AMTA1=isnull(SUM(a.gro_AMT),0),@AMTA2 =ISNULL(SUM(A.TAXAMT),0) FROM vattbl a inner join AC_MAST b on (a.AC_ID =b.Ac_id)
 WHERE a.BHENT in ('PT','EP')  AND (a.DATE BETWEEN @SDATE AND @EDATE) and b.U_EXEUNIT = 1
 SET @BAL_AMT1 = @BAL_AMT1 + @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 + @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','A',0,@AMTA1,0,0,0,'')
 PRINT 'TAXAMT'
 PRINT @BAL_TAX1

 --(B)Less : Goods return and cash / trade discount 
 set @AMTA1= 0
 set @AMTA2= 0
 SELECT @AMTA1=isnull(SUM(a.gro_AMT),0),@AMTA2=isnull(SUM(a.TAXAMT),0) FROM vattbl a inner join AC_MAST b on ( a.AC_ID =b.Ac_id)
 WHERE a.BHENT ='PR'  AND (a.DATE BETWEEN @SDATE AND @EDATE) and b.U_EXEUNIT = 1
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','B',0,@AMTA1,0,0,0,'')
 PRINT 'TAXAMT'
 PRINT @BAL_TAX1

 --(C)Less : Goods used in the manufacture of tax free goods
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','C',0,@AMTA1,0,0,0,'')
 --(D)Goods Exported out of India
	--pending for ca discussion 
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 ---
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','D',0,@AMTA1,0,0,0,'')
 --(E)Goods Consignment/Branch Transfer
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1 =ISNULL(SUM(A.GRO_AMT),0),@AMTA2 =ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON A.AC_ID =B.Ac_id 
 WHERE A. BHENT = 'PT' AND (A.Date BETWEEN @SDATE AND @EDATE )  AND B.U_EXEUNIT=1 AND A.U_IMPORM IN('Branch Transfer','Consignment Transfer')
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','E',0,@AMTA1,0,0,0,'')
 
 --(F)Less : Capital goods
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SELECT @AMTA1 =ISNULL(SUM(A.GRO_AMT),0),@AMTA2 =ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON A.AC_ID =B.Ac_id 
 WHERE A. BHENT = 'PT' AND (A.Date BETWEEN @SDATE AND @EDATE )  AND B.U_EXEUNIT=1 AND A.itemtype ='C' 
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','F',0,@AMTA1,0,0,0,'')
 
 --(G)Less : Sales made to persons other than taxable persons
 SET @AMTA1 = 0
 SET @AMTA2= 0
 SELECT @AMTA1 =ISNULL(SUM(A.GRO_AMT),0),@AMTA2 =ISNULL(SUM(A.TAXAMT),0) FROM VATTBL A INNER JOIN AC_MAST B ON A.AC_ID =B.Ac_id 
 WHERE A. BHENT = 'ST' AND (A.Date BETWEEN @SDATE AND @EDATE )  AND B.U_EXEUNIT=1 AND A.S_TAX ='' 
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','G',0,@AMTA1,0,0,0,'')
 PRINT 'TAXAMT'
 PRINT @BAL_TAX1

 --(H)Goods Not Eligible for Input Tax Credit
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','H',0,@AMTA1,0,0,0,'')

 --(I)Less : Any other goods on which notional input tax credit is not available
 SET @AMTA1 = 0
 SET @AMTA2 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 SET @BAL_TAX1 =@BAL_TAX1 - @AMTA2
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','I',0,0,0,0,0,'')
 --TOTAL 
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5','J',0,@BAL_AMT1,@BAL_TAX1,0,0,'')
 
-- Part 5A
--(a)Brought forward from previous return period
 SET @BAL_AMT1 = 0
 SET @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5A','A',0,@AMTA1,0,0,0,'')
 -- (B)Add notional ITC on purchases from exempted units as per col.5(j)
 SET @AMTA1 = 0
 SELECT @AMTA1 =ISNULL(SUM(AMT2),0) FROM #FORM15 WHERE PART=1 AND PARTSR='5'  AND SRNO='J'
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'5A','B',0,@AMTA1,0,0,0,'')

--- Part 6 TAX PAYABLE/EXCESS INPUT TAX CREDIT
 SET @BAL_AMT1 = 0
--(a)Total output [as per 3 (d)]
 set @AMTA1 = 0
 SELECT @AMTA1=isnull(SUM(AMT1),0) FROM #FORM15 WHERE Partsr = '3' and part=1 and srno='D'
 SET @BAL_AMT1 = @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','A',0,@AMTA1,0,0,0,'')
 --(b)Less: Monthly tax paid (as per 2nd proviso to rule 36)
 set @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','B',0,@AMTA1,0,0,0,'')
 --(i) 1st month of the quarter
  set @AMTA1 = 0
  SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','BA',0,@AMTA1,0,0,0,'')
 --(ii) 2nd month of the quarter
 set @AMTA1 = 0
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','BB',0,@AMTA1,0,0,0,'')

 ---Less NET ITC AS PER 4 (O)
 SET @AMTA1=0
 SELECT @AMTA1=ISNULL(SUM(AMT1),0) FROM #FORM15 WHERE Partsr = '4' and srno = 'O'
 SET @BAL_AMT1 = @BAL_AMT1 - @AMTA1
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','C',0,@AMTA1,0,0,0,'')

 --Difference (a-b-c) (if output tax is more than input tax, balance is adjusted out on Notional ITC, if any otherwise amount is to be deposited)
 SET @AMTA1 =  0
 SET @AMTA1 = @BAL_AMT1 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','D',0,Case when @AMTA1 > 0 then @AMTA1 else 0 end ,0,0,0,'')
 --Excess ITC If Any
  INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','E',0,0,0,0,0,'')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','EA',0,Case when @AMTA1 < 0 then -1*@AMTA1 else 0 end,0,0,0,'')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','EB',0,Case when @AMTA1 > 0 then @AMTA1 else 0 end,0,0,0,'')

 --Less CST Liable For The Period
 SET @AMTA1= 0
 SELECT @AMTA1=ISNULL(SUM(case when bhent in('ST') THEN + TAXAMT ELSE - TAXAMT END),0) FROM VATTBL  WHERE ST_TYPE='OUT OF STATE' AND BHENT in('ST','SR') AND (DATE BETWEEN @SDATE AND @EDATE)
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','F',0,case when @AMTA1 > 0 then @AMTA1 else 0 end,0,0,0,'')
 

 --Difference 6 (E-F)
 SET @AMTA1 = 0
 Select @AMTA1=ISNULL(sum(AMT1),0) from #form15 where partsr = '6' and srno in('E','EA','EB')
 SET @AMTA2= 0
 SELECT @AMTA2=ISNULL(SUM(case when bhent in('ST') THEN + TAXAMT ELSE - TAXAMT END),0) FROM VATTBL  WHERE ST_TYPE='OUT OF STATE' AND BHENT in('ST','SR') AND (DATE BETWEEN @SDATE AND @EDATE)
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','G',0,(CASE WHEN @AMTA1 <0 THEN +@AMTA1 ELSE @AMTA1 END)-@AMTA2 ,0,0,0,'')

 --Excess ITC if Any
 Select @AMTA1=AMT1 from #form15 where partsr = '6' and srno = 'G'
 SET @AMTA1=CASE WHEN @AMTA1 IS NULL THEN 0 ELSE @AMTA1 END
  INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','H',0,0,0,0,0,'')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','HA',0,Case when @AMTA1 < 0 then -1*@AMTA1 else 0 end,0,0,0,'')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','HB',0,Case when @AMTA1 > 0 then @AMTA1 else 0 end,0,0,0,'')

 --Less Actual ITC As Per 6 (H) To Claim
 SET @AMTA1= 0
 Select @AMTA1=ISNULL(SUM(AMT1),0) from #form15 where partsr = '6' and srno IN('H','HA','HB')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','I',0,@AMTA1,0,0,0,'')

 --Balance ITC Carried Forward
 Select @AMTA1=AMT1 from #form15 where partsr = '6' and srno = 'I'
 SET @AMTA1=CASE WHEN @AMTA1 IS NULL THEN 0 ELSE @AMTA1 END
  INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','J',0,@AMTA1,0,0,0,'')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','JA',0,0,0,0,0,'')
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','JB',0,0,0,0,0,'')

 --Net Tax Payable 
 SELECT @AMTA1=AMT1 FROM #FORM15 WHERE Partsr = '6' and srno = 'D'
 SET @AMTA1=CASE WHEN @AMTA1 IS NULL THEN 0 ELSE @AMTA1 END
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','K',0,@AMTA1,0,0,0,'')
 --(i) Tax payable as per VAT-2					
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','L',0,0,0,0,0,'')
  --(i) Tax payable as per VAT-2a					
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','LA',0, 0 ,0,0,0,'')
 --(i) Tax payable as per VAT-2b
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'6','LB',0,0 ,0,0,0,'')
 
--- Part 7
Declare @TAXONAMT as numeric(12,2),@TAXAMT1 as numeric(12,2),@ITEMAMT as numeric(12,2),@INV_NO as varchar(10),@DATE as smalldatetime,@PARTY_NM as varchar(50),@ADDRESS as varchar(100),@ITEM as varchar(50),@FORM_NM as varchar(30),@S_TAX as varchar(30),@QTY as numeric(18,4)
SELECT @TAXONAMT=0,@TAXAMT =0,@ITEMAMT =0,@INV_NO ='',@DATE ='',@PARTY_NM ='',@ADDRESS ='',@ITEM ='',@FORM_NM='',@S_TAX ='',@QTY=0

SET @CHAR=65

SET @PER = 0
declare Cur_VatPay cursor  for
select A.vatonamt,A.Gro_amt,A.taxamt,A.INV_NO,qty=b.u_chalno,Date=b.u_chaldt,Party_nm=b.bank_nm,Address='',A.Form_nm,S_tax=C.BSRCODE
from vattbl  A
Inner join Bpmain B on (A.Bhent = B.Entry_ty and A.Tran_cd = B.Tran_cd)
LEFT OUTER JOIN AC_MAST C ON (B.BANK_NM=C.AC_NAME)
where A.BHENT = 'BP' And(B.Date Between @sdate and @edate) and b.Party_nm='VAT PAYABLE'
open Cur_VatPay
FETCH NEXT FROM Cur_VatPay INTO @TAXONAMT,@ITEMAMT,@TAXAMT,@QTY,@INV_NO,@DATE,@PARTY_NM,@ADDRESS,@FORM_NM,@S_TAX
	WHILE (@@FETCH_STATUS=0)
	BEGIN

	SET @PER=CASE WHEN @PER IS NULL THEN 0 ELSE @PER END
	SET @TAXONAMT=CASE WHEN @TAXONAMT IS NULL THEN 0 ELSE @TAXONAMT END
	SET @TAXAMT=CASE WHEN @TAXAMT IS NULL THEN 0 ELSE @TAXAMT END
	SET @ITEMAMT=CASE WHEN @ITEMAMT IS NULL THEN 0 ELSE @ITEMAMT END
	SET @QTY=CASE WHEN @QTY IS NULL THEN 0 ELSE @QTY END
	SET @PARTY_NM=CASE WHEN @PARTY_NM IS NULL THEN '' ELSE @PARTY_NM END
	SET @INV_NO=CASE WHEN @INV_NO IS NULL THEN '' ELSE @INV_NO END
	SET @DATE=CASE WHEN @DATE IS NULL THEN '' ELSE @DATE END
	SET @ADDRESS=CASE WHEN @ADDRESS IS NULL THEN '' ELSE @ADDRESS END
	SET @ITEM=CASE WHEN @ITEM IS NULL THEN '' ELSE @ITEM END
	SET @S_TAX=CASE WHEN @S_TAX IS NULL THEN '' ELSE @S_TAX END
	SET @FORM_NM=CASE WHEN @FORM_NM IS NULL THEN '' ELSE @FORM_NM END
	INSERT INTO #FORM15  (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,INV_NO,DATE,PARTY_NM,ADDRESS,FORM_NM,S_TAX) VALUES (1,'7',CHAR(@CHAR),@PER,@TAXONAMT,@TAXAMT,@ITEMAMT,@QTY,@INV_NO,@DATE,@PARTY_NM,@ADDRESS,@FORM_NM,@S_TAX)
	SET @CHAR=@CHAR+1
	FETCH NEXT FROM CUR_VatPay INTO @TAXONAMT,@TAXAMT,@ITEMAMT,@QTY,@INV_NO,@DATE,@PARTY_NM,@ADDRESS,@FORM_NM,@S_TAX--,@ITEM,@QTY
END
CLOSE CUR_VatPay
DEALLOCATE CUR_VatPay
if not exists(select top 1 * from #FORM15 where PART=1 and partsr='7')
begin
	INSERT INTO #FORM15  (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,INV_NO,DATE,PARTY_NM,ADDRESS,FORM_NM,S_TAX) VALUES (1,'7',CHAR(@CHAR),@PER,@TAXONAMT,@TAXAMT,@ITEMAMT,@QTY,@INV_NO,@DATE,@PARTY_NM,@ADDRESS,@FORM_NM,'')
end


--- Part 8 ---FOR UNITS AVAILING EXEMPTION OR DEFERMENTS
  --Entitlement of certificate no. and date 
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','A',0,0,0,0,0,'')
 --Date of expiry of exemption/deferment
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','B',0,0,0,0,0,'')
 --Total amount of exemption/deferment allowed
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','C',0,0,0,0,0,'')
 --(i) Exemption/deferment available at the begining of the return period (including Under CST Act)
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','CA',0,0,0,0,0,'')
 --(ii) Exemption/deferment availed during the return period
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','CB',0,0,0,0,0,'')
 --(iii) Balance at the end of the return period
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','CC',0,0,0,0,0,'')
 --Admissible amount of refund of tax paid purchases
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','D',0,0,0,0,0,'')
 --Goods sent on consignment/stock transfer basis
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'8','E',0,0,0,0,0,'')

---Part 9
 ---MISC Informations
 --Value of Branch Transfer/Consignment Transfer made during the return period
 SET @AMTA1 = 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'9','A',0,0,0,0,0,'')
 
 --Value of commission Sales made by Kacha Arhtiya during return period
 SET @AMTA1 = 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'9','B',0,0,0,0,0,'')
 
 --Payments made to contractor(s) sub-contractor(s)
 SET @AMTA1 = 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'9','C',0,0,0,0,0,'')
 
 --Proof of payment of TDS
 SET @AMTA1 = 0
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'9','D',0,0,0,0,0,'')
 --Value of Capital Goods Purchase from taxable persons
 SET @AMTA1 = 0
 SELECT @AMTA1 =ISNULL(SUM(GRO_AMT),0) FROM VATTBL WHERE BHENT='PT' AND ITEMTYPE='C' AND S_TAX <> '' AND (DATE BETWEEN @SDATE AND @EDATE)
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'9','E',0,@AMTA1,0,0,0,'')

-- Part 10 1.BREAK UP OF TAXABLE SALES AND PURCHASES IN PUNJAB(EXCLUDING  CAPITAL GOODS)

 SELECT @AMTA1=0,@AMTB1=0,@AMTC1=0,@AMTD1=0,@AMTE1=0,@AMTF1=0,@AMTG1=0,@AMTH1=0,@AMTI1=0,@AMTJ1=0,@AMTK1=0,@AMTL1=0,@AMTM1=0,@AMTN1=0,@AMTO1=0,@AMTP1=0 
 SET @CHAR=65
 DECLARE  CUR_FORM221 CURSOR FOR 
 select distinct level1 from stax_mas --where ST_TYPE='LOCAL'--CHARINDEX('VAT',TAX_NAME)>0
 OPEN CUR_FORM221
 FETCH NEXT FROM CUR_FORM221 INTO @PER
 WHILE (@@FETCH_STATUS=0)
 BEGIN
	SELECT @AMTA1=isnull(SUM(VATONAMT),0),@AMTB1=isnull(SUM(TAXAMT),0) FROM VATTBL WHERE ST_TYPE IN('LOCAL','') AND BHENT='ST' AND TAX_NAME LIKE'%VAT%'  AND PER=@PER AND (DATE BETWEEN @SDATE AND @EDATE) AND itemtype ='I'
	SELECT @AMTC1=isnull(SUM(VATONAMT),0),@AMTD1=isnull(SUM(TAXAMT),0) FROM VATTBL WHERE ST_TYPE IN('LOCAL','') AND BHENT='SR'  AND TAX_NAME LIKE'%VAT%'  AND PER=@PER AND (DATE BETWEEN @SDATE AND @EDATE)AND itemtype ='I'
	--Purchases
	SELECT @AMTG1=isnull(SUM(VATONAMT),0),@AMTH1=isnull(SUM(TAXAMT),0) FROM VATTBL WHERE ST_TYPE IN('LOCAL','') AND BHENT IN('PT','EP') AND PER=@PER AND (DATE BETWEEN @SDATE AND @EDATE)  AND TAX_NAME LIKE'%VAT%'  AND itemtype ='I'
	SELECT @AMTI1=isnull(SUM(VATONAMT),0),@AMTJ1=isnull(SUM(TAXAMT),0) FROM VATTBL WHERE ST_TYPE IN('LOCAL','') AND BHENT='PR' AND PER=@PER AND (DATE BETWEEN @SDATE AND @EDATE)  AND TAX_NAME LIKE'%VAT%' AND itemtype ='I'
--	SELECT @AMTK1=SUM(VATONAMT),@AMTL2=SUM(TAXAMT) FROM VATTBL WHERE ST_TYPE IN('LOCAL','') AND BHENT='EP' AND PER=@PER AND (DATE BETWEEN @SDATE AND @EDATE)  AND TAX_NAME LIKE'%VAT%' AND itemtype ='I'
  --Sales Invoices
  --SET @AMTA1=ISNULL(@AMTA1,0)
  --SET @AMTB1=ISNULL(@AMTB1,0)
  --Return Invoices
  --SET @AMTC1=ISNULL(@AMTC1,0)
  --SET @AMTD1=ISNULL(@AMTD1,0)
  --Net Effect
  Set @NetEFFS = @AMTA1-@AMTC1
  Set @NetTAXS = (@AMTB1)-(@AMTD1)
  --Purchase Invoice
  --SET @AMTG1=ISNULL(@AMTG1,0)
  --SET @AMTH1=ISNULL(@AMTH1,0)
  --Return Invoice
  --SET @AMTI1=ISNULL(@AMTI1,0)
  --SET @AMTJ1=ISNULL(@AMTJ1,0)
  --Expense Purchase Invoice
  --SET @AMTK1=ISNULL(@AMTK1,0)
  --SET @AMTL2=ISNULL(@AMTL2,0)

  --Net Effect
  Set @NetEFFP= @AMTG1 - @AMTI1
  Set @NetTAXP = @AMTH1 - @AMTJ1
if @nettaxS <> 0 OR @NetEFFP <> 0
begin
		  INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'10','A',@PER,@NETEFFS,@NETTAXS,@NETEFFP,@NETTAXP,'')
		  SET @AMTM1=@AMTM1+@NETEFFS --TOTAL TAXABLE AMOUNT
		  SET @AMTN1=@AMTN1+@NETTAXS --TOTAL TAX

		  SET @AMTO1=@AMTO1+@NETEFFP --TOTAL TAXABLE AMOUNT
		  SET @AMTP1=@AMTP1+@NETTAXP --TOTAL TAX
		  SET @CHAR=@CHAR+1
	  end

  FETCH NEXT FROM CUR_FORM221 INTO @PER
 END
 CLOSE CUR_FORM221
 DEALLOCATE CUR_FORM221
IF NOT EXISTS(SELECT TOP 1 SRNO FROM #FORM15 WHERE PART=1 AND PARTSR='10' AND SRNO='A')
BEGIN
	INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'10','',0,0,0,0,0,'')
END
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'10','B',0,0,0,0,0,'')
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'10','C',0,0,0,0,0,'')

 --- 2.BREAK UP OF GOODS SOLD UNDER WORKS CONTRACT
	--Pending for CA discussion  
 INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'11','A',0,0,0,0,0,'')

--Part-12 3.BREAK UP OF ZERO RATED SALES 
set @AMTA1 = 0
SELECT @AMTA1=ISNULL(SUM(Gro_AMT),0) FROM vattbl  WHERE (st_type = 'Out of Country') AND U_IMPORM ='Export Out of India' AND BHENT='ST' AND (DATE BETWEEN @SDATE AND @EDATE)
set @AMTA2 = 0
SELECT @AMTA2=ISNULL(SUM(Gro_AMT),0) FROM vattbl  WHERE (st_type = 'Out of Country') AND U_IMPORM ='Export Out of India' AND BHENT='SR' AND (DATE BETWEEN @SDATE AND @EDATE)

INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'12','A',0,@AMTA1,@AMTA2,0,(@AMTA1-@AMTA2),'')
set @AMTA1=0
SELECT @AMTA1=isnull(SUM(gro_AMT),0) FROM VATTBL  WHERE (TAX_NAME in('Form H','H FORM','H') ) AND BHENT='ST' AND (DATE BETWEEN @SDATE AND @EDATE)
set @AMTA2 = 0
SELECT @AMTA2=isnull(SUM(gro_AMT),0) FROM VATTBL  WHERE (TAX_NAME in('Form H','H FORM','H') ) AND BHENT='SR' AND (DATE BETWEEN @SDATE AND @EDATE)
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'12','B',0,@AMTA1,@AMTA2,0,(@AMTA1-@AMTA2),'')

--Part-13 4.PRIOR PERIOD ADJUSTMENTS 
   --Pending for CA discussion 
SELECT @AMTA1=0,@AMTA2=0,@AMTA3=0,@AMTA4=0
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'13','A',0,@AMTA1,@AMTA2,@AMTA3,@AMTA4,'')
SELECT @AMTA1=0,@AMTA2=0,@AMTA3=0,@AMTA4=0
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'13','B',0,@AMTA1,@AMTA2,@AMTA3,@AMTA4,'')
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'13','C',0,0,0,0,0,'')
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'13','D',0,0,0,0,0,'')

--Part-14
     ---5.Any other adjustment( please specify)
INSERT INTO #FORM15 (PART,PARTSR,SRNO,RATE,AMT1,AMT2,AMT3,AMT4,PARTY_NM) VALUES (1,'14','',0,0,0,0,0,'')

Update #FORM15 set  PART = isnull(Part,0) , Partsr = isnull(PARTSR,''), SRNO = isnull(SRNO,''),
		             RATE = isnull(RATE,0), AMT1 = isnull(AMT1,0), AMT2 = isnull(AMT2,0), 
					 AMT3 = isnull(AMT3,0), AMT4 = isnull(AMT4,0), INV_NO = isnull(INV_NO,''), DATE = isnull(Date,''), 
					 PARTY_NM = isnull(Party_nm,''), ADDRESS = isnull(Address,''),
					 FORM_NM = isnull(form_nm,''), S_TAX = isnull(S_tax,'')
SELECT * FROM #FORM15 order by cast(substring(partsr,1,case when (isnumeric(substring(partsr,1,2))=1) then 2 else 1 end) as int)
END