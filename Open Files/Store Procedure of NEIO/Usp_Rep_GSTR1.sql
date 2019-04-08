DROP PROCEDURE [Usp_Rep_GSTR1]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [dbo].[Usp_Rep_GSTR1]    Script Date: 09/18/2017 17:04:31 ******/
/*
	Author : Suraj Kumawat
	Date created : 05-06-2017
	Modified By : Priyanka B.
	Modified Date : 03-01-2018
	Modified By : By Suraj Kumawat for bug-31194
	set dateformat dmy EXECUTE Usp_Rep_GSTR1'','','','01/06/2017 ','30/06/2017','','','','',0,0,'','','','','','','','','2017-2018',''
	set dateformat dmy EXECUTE Usp_Rep_GSTR1'','','','01/10/2017 ','30/10/2017','','','','',0,0,'','','','','','','','','2017-2018','sss'
*/
CREATE Procedure [Usp_Rep_GSTR1]
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
As
BEGIN
	Declare @FCON as NVARCHAR(2000),@fld_list NVARCHAR(2000)
	EXECUTE   USP_REP_FILTCON 
		@VTMPAC =@TMPAC,@VTMPIT =@TMPIT,@VSPLCOND =@SPLCOND
		,@VSDATE=@SDATE,@VEDATE=@EDATE
		,@VSAC =@SAC,@VEAC =@EAC
		,@VSIT=@SIT,@VEIT=@EIT
		,@VSAMT=@SAMT,@VEAMT=@EAMT
		,@VSDEPT=@SDEPT,@VEDEPT=@EDEPT
		,@VSCATE =@SCATE,@VECATE =@ECATE
		,@VSWARE =@SWARE,@VEWARE  =@EWARE
		,@VSINV_SR =@SINV_SR,@VEINV_SR =@SINV_SR
		,@VMAINFILE='M',@VITFILE=Null,@VACFILE='i'
		,@VDTFLD ='DATE'
		,@VLYN=Null
		,@VEXPARA=@EXPARA
		,@VFCON =@FCON OUTPUT

	-------Temporary Table Creation ----
	SELECT  Tran_cd =IDENTITY(INT,1,1),PART=0,PARTSR='AAAA',SRNO= SPACE(2),INV_NO=SPACE(40), H.DATE,ORG_INVNO=SPACE(40)
	, H.DATE AS ORG_DATE, D.QTY, d.u_asseamt AS Taxableamt, d.CGST_PER AS RATE, d.CGST_PER, D.CGST_AMT, d.SGST_PER, D.SGST_AMT
	, d.IGST_PER,D.IGST_AMT,D.IGST_AMT as Cess_Amt,D.IGST_AMT as Cessr_Amt	,D.GRO_AMT, IT.IT_NAME
	, cast(IT.IT_DESC as varchar(250)) as IT_DESC , Isservice=SPACE(150), IT.HSNCODE
	,HSN_DESC = IT.SERTY
	,ac_name = cast((CASE WHEN isnull(H.scons_id, 0) > 0 THEN isnull(shipto.Mailname, '') ELSE isnull(ac.ac_name, '') END) as varchar(150))
	, gstin = space(30), location = (CASE WHEN isnull(H.scons_id, 0) > 0 THEN isnull(shipto.state, '') ELSE isnull(ac.state, '') END)
	,SUPP_TYPE = SPACE(100),st_type= SPACE(100),StateCode=space(5),Ecomgstin =space(30),from_srno =space(30),to_srno =space(30)
	,ORG_GSTIN =space(30) ,SBBILLNO=space(50),SBDATE=H.DATE,rptmonth=SPACE(15),rptyear =SPACE(15) ,Amenddate
	into #GSTR1
	FROM  STMAIN H 
	INNER JOIN STITEM D ON (H.ENTRY_TY = D .ENTRY_TY AND H.TRAN_CD = D .TRAN_CD) 
	INNER JOIN IT_MAST IT ON (D .IT_CODE = IT.IT_CODE) 
	LEFT OUTER JOIN shipto ON (shipto.shipto_id = h.scons_id) 
	LEFT OUTER JOIN ac_mast ac ON (h.cons_id = ac.ac_id)  
	WHERE 1=2

/* GSTR_VW DATA STORED IN TEMPORARY TABLE*/
SELECT DISTINCT HSN_CODE, CAST(HSN_DESC AS VARCHAR(250)) AS HSN_DESC INTO #HSN FROM HSN_MASTER

SELECT mEntry=case when l.Bcode_nm<>'' then l.Bcode_nm else (case when l.ext_vou=1 then '' else l.entry_ty end) end,
		RATE1= (case when (isnull(A.CGST_PER,0)+isnull(A.SGST_PER,0)) >0  then (isnull(A.CGST_PER,0)+isnull(A.SGST_PER,0))
					when (isnull(A.IGST_PER,0)) > 0 then (isnull(A.IGST_PER,0)) else isnull(A.gstrate,0) end)
		,igst_amt1=ISNULL(a.igst_amt,0),cgst_amt1=ISNULL(a.cgst_amt,0),sGST_AMT1=isnull(a.sGST_AMT,0),CESS_AMT1 =isnull(a.CESS_AMT,0)
		,igsrt_amt1=ISNULL(a.IGSRT_AMT,0),cgsrt_amt1=ISNULL(a.CGSRT_AMT,0),sGSrT_AMT1=isnull(a.SGSRT_AMT,0),CESSR_AMT1 =isnull(a.CessR_amt,0)
		,Ecomgstin = isnull(ac.gstin,'')
		,A.*
		,HSN_DESC=ISNULL((SELECT TOP 1 HSN_DESC FROM #HSN WHERE HSN_CODE = A.HSNCODE),'')
INTO #GSTR1TBL 
FROM GSTR1_VW A
left join ac_mast ac on (A.ecomac_id=ac.ac_id)
inner join lcode l  on (a.entry_ty=l.entry_ty)
WHERE (A.DATE BETWEEN @SDATE AND @EDATE) and A.AMENDDATE=''


-----Amended Detail Temp table 
SELECT mEntry=case when l.Bcode_nm<>'' then l.Bcode_nm else (case when l.ext_vou=1 then '' else l.entry_ty end) end,
		RATE1= (case when (isnull(A.CGST_PER,0)+isnull(A.SGST_PER,0)) >0  then (isnull(A.CGST_PER,0)+isnull(A.SGST_PER,0))
					when (isnull(A.IGST_PER,0)) > 0 then (isnull(A.IGST_PER,0)) else isnull(A.gstrate,0) end)
		,igst_amt1=ISNULL(a.igst_amt,0),cgst_amt1=ISNULL(a.cgst_amt,0),sGST_AMT1=isnull(a.sGST_AMT,0),CESS_AMT1 =isnull(a.CESS_AMT,0)
		,igsrt_amt1=ISNULL(a.IGSRT_AMT,0),cgsrt_amt1=ISNULL(a.CGSRT_AMT,0),sGSrT_AMT1=isnull(a.SGSRT_AMT,0),CESSR_AMT1 =isnull(a.CessR_amt,0)
		,Ecomgstin = isnull(ac.gstin,'')
		,A.*
		,HSN_DESC=ISNULL((SELECT TOP 1 HSN_DESC FROM #HSN WHERE HSN_CODE = A.HSNCODE),'')
INTO #GSTR1AMD
FROM GSTR1_VW A
left join ac_mast ac on (A.ecomac_id=ac.ac_id)
inner join lcode l  on (a.entry_ty=l.entry_ty)
WHERE (A.AMENDDATE BETWEEN @SDATE AND @EDATE) AND A.HSNCODE <> ''

Declare @amt1 decimal(18,2),@amt2 decimal(18,2),@amt3 decimal(18,2),@amt4 decimal(18,2),@from_srno varchar(30),@to_srno varchar(30)

/* 4. Taxable outward supplies made to registered persons (including UIN-holders) other than supplies covered by Table 6 */ 
/*4A. Supplies other than those (i) attracting reverse charge and (ii) supplies made through e-commerce operator*/
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
SELECT 4 AS PART ,'4A' AS PARTSR,'A' AS SRNO,gstin,inv_no,date,POS
--,net_amt  --Commented by Priyanka B on 08032019 for Bug-31793
,gro_amt  --Modified by Priyanka B on 08032019 for Bug-31793
,rate1,sum(taxableamt)taxableamt
,sum(CGST_AMT1)CGST_AMT ,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt ,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE 
from #GSTR1TBL
where (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type <> 'Out of country' 
and supp_type IN ('Registered','Compounding','E-Commerce') 
and gstin <>'' AND LineRule = 'Taxable' AND HSNCODE <> '' and Ecomgstin = ''
and (SGSRT_AMT1 + CGSRT_AMT1  + IGSRT_AMT1 + cessr_amt1) = 0  and (SGST_AMT1 + CGST_AMT1  + IGST_AMT1 + cess_amt1) > 0
group by gstin,inv_no,date,POS,rate1
--,net_amt  --Commented by Priyanka B on 08032019 for Bug-31793
,gro_amt  --Modified by Priyanka B on 08032019 for Bug-31793
Order by Date,Inv_no,Rate1
  
IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '4A' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,location,inv_no,date,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,cess_amt)
	VALUES(4,'4A','A','','','','',0,0,0,0,0,0,0,0,'','','','',0)
END

/*4B. Supplies attracting tax on reverse charge basis*/
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
SELECT 4 AS PART ,'4B' AS PARTSR,'A' AS SRNO,gstin,inv_no,date,POS,net_amt,rate1,sum(taxableamt)taxableamt
,sum(CGST_AMT1)CGST_AMT,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt ,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE 
FROM #GSTR1TBL
WHERE (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type <> 'Out of country' 
and supp_type IN('Registered','Compounding','E-Commerce')
and gstin <>'' 
AND LineRule = 'Taxable' AND HSNCODE <> '' and Ecomgstin = '' AND (SGSRT_AMT1 + CGSRT_AMT1 + IGSRT_AMT1 + cessr_amt1) > 0
group by gstin,inv_no,date,POS,rate1,net_amt 
Order by Date,Inv_no,Rate1

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '4B' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(4,'4B','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

/*4C. Supplies made through e-commerce operator attracting TCS (operator wise, rate wise) */
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Ecomgstin)
select 4 AS PART ,'4C' AS PARTSR,'A' AS SRNO,gstin,inv_no,date,POS,net_amt,rate1,sum(taxableamt)taxableamt
,sum(CGST_AMT1)CGST_AMT ,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt ,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE
,Ecomgstin 
from #GSTR1TBL
where (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type <> 'Out of country' and Ecomgstin <> '' AND LineRule = 'Taxable' 
AND HSNCODE <> ''  and gstin not in('Unregistered') and (SGSRT_AMT1 + CGSRT_AMT1  + IGSRT_AMT1 + cessr_amt1) = 0  
and (SGST_AMT1 + CGST_AMT1 + IGST_AMT1 + cess_amt1) > 0
group by Ecomgstin,gstin,inv_no,date,net_amt,rate1,POS 
Order by Date,Inv_no,Rate1

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '4C' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(4,'4C','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

/* 5. Taxable outward inter-State supplies to un-registered persons where the invoice value is more than Rs 2.5 lakh */
--- 5A. Outward supplies (other than supplies made through e-commerce operator, rate wise)
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
SELECT 5 AS PART ,'5A' AS PARTSR,'A' AS SRNO,gstin,inv_no,date,POS,net_amt,rate1,sum(taxableamt)taxableamt
,sum(CGST_AMT1)CGST_AMT,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt ,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE 
FROM #GSTR1TBL
WHERE (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type = 'InterState' and supp_type = 'Unregistered' and gstin ='Unregistered' AND LineRule = 'Taxable' 
AND HSNCODE <> '' and Ecomgstin = '' and net_amt >250000 and (SGST_AMT + CGST_AMT1  + IGST_AMT1 + cess_amt1) > 0
group by gstin,inv_no,date,POS,net_amt,rate1  
Order by Date,Inv_no,Rate1

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '5A' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(5,'5A','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

--- 5B. Supplies made through e-commerce operator attracting TCS (operator wise, rate wise) GSTIN of e-commerce operator
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Ecomgstin)
SELECT 5 AS PART ,'5B' AS PARTSR,'A' AS SRNO,'' as Ecomgstin,inv_no,date,POS,net_amt,rate1,sum(taxableamt)taxableamt
,sum(CGST_AMT1)CGST_AMT,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt ,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE
,Ecomgstin 
from #GSTR1TBL
where (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type = 'InterState' and Ecomgstin <> '' AND LineRule = 'Taxable' AND HSNCODE <> '' 
and net_amt >250000 and (SGST_AMT1 + CGST_AMT1  + IGST_AMT1 + cess_amt1) > 0
group by Ecomgstin,inv_no,DATE,net_amt,POS,rate1 
Order by Date,Inv_no,Rate1
        
IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '5B' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(5,'5B','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

/* 	6.Zero rated supplies and Deemed Exports */
  ---6A. Exports
SELECT 6 AS PART ,'6A' AS PARTSR,'A' AS SRNO,Consgstin,inv_no,date,POS,net_amt,RATE1
,sum(taxableamt)taxableamt,sum(CGST_AMT1)CGST_AMT,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt 
,CGST_PER,SGST_PER,IGST_PER
,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,sbbillno,sbdate 
into #Section6A
from #GSTR1TBL
where (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type = 'Out of Country' and Ecomgstin = ''
group by Consgstin,inv_no,date,POS,rate1,sbbillno,sbdate,net_amt,EXPOTYPE
,CGST_PER,SGST_PER,IGST_PER
Order by Date,Inv_no,Rate1

Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt  
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,sbbillno,sbdate)

select PART ,PARTSR,SRNO,CONSGSTIN,inv_no,date,POS,net_amt,rate1,sum(taxableamt),CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,rate1 as igst_per,IGST_AMT,cess_amt
,AC_NAME,SUPP_TYPE,ST_TYPE,sbbillno,sbdate 
from #Section6A
group by PART ,PARTSR,SRNO,CONSGSTIN,inv_no,date,POS,net_amt,rate1,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt ,AC_NAME,SUPP_TYPE
,ST_TYPE,sbbillno,sbdate
,CGST_PER,SGST_PER,IGST_PER
Order by Date,Inv_no,Rate1

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '6A' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(6,'6A','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

---6B. Supplies made to SEZ unit or SEZ Developer
select 6 AS PART ,'6B' AS PARTSR,'A' AS SRNO
,(CASE WHEN isnull(ltrim(rtrim(CONSGSTIN)),'') = '' THEN (case when isnull(ltrim(rtrim(GSTIN)),'') <> '' then ltrim(rtrim(GSTIN)) 
else '' end) ELSE ltrim(rtrim(CONSGSTIN)) END) AS Consgstin,inv_no,date,POS,net_amt,RATE1
,sum(taxableamt)taxableamt,sum(CGST_AMT1)CGST_AMT,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt 
,CGST_PER,SGST_PER,IGST_PER,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,sbbillno,sbdate
into #Section6B
from #GSTR1TBL
where (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type iN('INTERSTATE','INTRASTATE') and supp_type IN('SEZ') and Ecomgstin = ''
group by Consgstin,inv_no,date,POS,rate1,sbbillno,sbdate,net_amt,GSTIN,EXPOTYPE,CGST_PER,SGST_PER,IGST_PER
Order by Date,Inv_no,Rate1


Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt	
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,sbbillno,sbdate)

select PART ,PARTSR,SRNO,CONSGSTIN,inv_no,date,POS,net_amt,rate1,sum(taxableamt),CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,rate1 as igst_per,IGST_AMT,cess_amt
,AC_NAME,SUPP_TYPE,ST_TYPE,sbbillno,sbdate
from #Section6B
group by PART ,PARTSR,SRNO,CONSGSTIN,inv_no,date,POS,net_amt,rate1,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt ,AC_NAME,SUPP_TYPE
,ST_TYPE,sbbillno,sbdate,CGST_PER,SGST_PER,IGST_PER
Order by Date,Inv_no,Rate1
    
IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '6B' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(6,'6B','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END


----6C. Deemed exports
select 6 AS PART ,'6C' AS PARTSR,'A' AS SRNO
,(CASE WHEN isnull(ltrim(rtrim(CONSGSTIN)),'') = '' THEN (case when isnull(ltrim(rtrim(GSTIN)),'') <> '' then ltrim(rtrim(GSTIN)) 
else '' end) ELSE ltrim(rtrim(CONSGSTIN)) END) AS Consgstin,inv_no,date,POS,net_amt,RATE1
,sum(taxableamt)taxableamt,sum(CGST_AMT1)CGST_AMT,sum(SGST_AMT1)SGST_AMT,sum(IGST_AMT1)IGST_AMT,sum(cess_amt1)cess_amt 
,CGST_PER,SGST_PER,IGST_PER,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,sbbillno,sbdate 
into #Section6C
from #GSTR1TBL
where (mEntry in ('ST','SB') and entry_ty<>'UB') and st_type iN('INTERSTATE','INTRASTATE') and supp_type  in('Export','EOU','IMPORT') and Ecomgstin = ''
group by Consgstin,inv_no,date,POS,rate1,sbbillno,sbdate,net_amt,GSTIN,EXPOTYPE,CGST_PER,SGST_PER,IGST_PER
Order by Date,Inv_no,Rate1

Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,sbbillno,sbdate)

select PART ,PARTSR,SRNO,CONSGSTIN,inv_no,date,POS,net_amt,rate1,sum(taxableamt),CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,rate1 as  IGST_PER,IGST_AMT,cess_amt
,AC_NAME,SUPP_TYPE,ST_TYPE,sbbillno,sbdate 
from #Section6C
group by PART ,PARTSR,SRNO,CONSGSTIN,inv_no,date,POS,net_amt,rate1,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt ,AC_NAME,SUPP_TYPE
,ST_TYPE,sbbillno,sbdate,CGST_PER,SGST_PER,IGST_PER
Order by Date,Inv_no,Rate1


IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '6C' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(6,'6C','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

/*7. Taxable supplies (Net of debit notes and credit notes) to Unregistered persons other than the supplies covered in Table 5 */
  ---7A. Intra-State supplies
  ---7A (1). Consolidated rate wise outward supplies [including supplies made through e-commerce operator attracting TCS]
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
Select * 
from (
		select 7 AS PART ,'7AA' AS PARTSR,'A' AS SRNO,'' as gstin,'' as inv_no,'' as date,'' as location,0 as GRO_AMT,rate1
		,taxableamt =sum((case when mEntry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mEntry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mEntry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mEntry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mEntry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1TBL  
		where (mEntry in ('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Intrastate' and supp_type ='Unregistered' 
		AND (CGST_AMT1+SGST_AMT1+IGST_AMT1+cess_amt1) > 0
		group by rate1
	)aa
where (CGST_AMT+SGST_AMT+IGST_AMT+cess_amt) > 0


IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '7AA' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(7,'7AA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

 ----7A (2). Out of supplies mentioned at 7A(1), value of supplies made through e-Commerce Operators attracting TCS (operator wise, rate wise)
 ---GSTIN of e-commerce operator
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,ecomgstin)
Select * 
from (
		select 7 AS PART ,'7AB' AS PARTSR,'B' AS SRNO,'' as gstin1,'' as inv_no,'' as date,'' as location,0.00 as GRO_AMT,rate1
		,taxableamt = sum((case when mEntry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mEntry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mEntry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mEntry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mEntry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE,Ecomgstin
		from #GSTR1TBL 
		where (mEntry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type = 'Intrastate' and gstin = 'Unregistered'	and Ecomgstin <> ''
		AND (CGST_AMT1+SGST_AMT1+IGST_AMT1+cess_amt1) > 0
		group by Ecomgstin,rate1 
	)aa	
where (CGST_AMT+SGST_AMT+IGST_AMT+cess_amt) > 0

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '7AB' AND SRNO ='B')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(7,'7AB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

----7B. Inter-State Supplies where invoice value is upto Rs 2.5 Lakh [Rate wise]
-----7B (1). Place of Supply (Name of State)
SELECT * 
INTO #tempTbl1 
FROM (
		select A.entry_ty,A.Tran_cd,A.Itserial,A.rentry_ty,A.Itref_tran,A.Ritserial,B.net_amt,B.INV_NO 
		from SRITREF A inner JOIN #GSTR1TBL B ON (A.rentry_ty =B.ENTRY_TY AND A.Itref_tran =B.Tran_cd  AND A.Ritserial =B.ITSERIAL)
		UNION ALL
		select A.entry_ty,A.Tran_cd,A.Itserial,A.rentry_ty,A.Itref_tran,A.Ritserial,B.net_amt,B.INV_NO
		from OTHITREF A inner JOIN #GSTR1TBL B ON (A.rentry_ty =B.ENTRY_TY AND A.Itref_tran =B.Tran_cd  AND A.Ritserial =B.ITSERIAL) 
	)A 
	
select * INTO #TEMP1 
FROM (
		SELECT  *
		from #GSTR1TBL  
		where  mEntry in('CN','DN','SR')
		and (Entry_ty + QUOTENAME(Tran_cd)) in(select (Entry_ty + QUOTENAME(Tran_cd)) from #tempTbl1 where net_amt <=250000)
		UNION ALL 
		select *
		from #GSTR1TBL  
		where  (mEntry in('SB','ST') and entry_ty<>'UB') AND net_amt <=250000 
	) AA
	
	
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
Select * 
from (
		select 7 AS PART ,'7BA' AS PARTSR,'A' AS SRNO,'' as gstin, '' as inv_no, '' as date,POS,0.00 as GRO_AMT,rate1
		,taxableamt = sum((case when mentry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mentry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE
		from #TEMP1
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Interstate' and Ecomgstin = ''
		and supp_type ='Unregistered' AND (CGST_AMT1+SGST_AMT1+IGST_AMT1+cess_amt1) > 0 
		group by POS,rate1 
	)aa	
where (CGST_AMT+SGST_AMT+IGST_AMT+cess_amt) > 0
order by POS,rate1

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '7BA' AND SRNO ='A')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(7,'7BA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

----7B (2). Out of the supplies mentioned in 7B (1), the supplies made through e-Commerce Operators (operator wise,rate wise)
-----GSTIN of e-commerce operator
Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,ecomgstin)
Select * 
from (
		select 7 AS PART ,'7BB' AS PARTSR,'B' AS SRNO,'' as gstin, '' as inv_no, '' as date,'' as location,0.00 as GRO_AMT,rate1
		,taxableamt = sum((case when mentry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mentry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,Ecomgstin 
		from #TEMP1
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Interstate' and Ecomgstin <> ''
		and gstin = 'Unregistered' AND HSNCODE <> '' AND (CGST_AMT1+SGST_AMT1+IGST_AMT1+cess_amt1) > 0 
		group by Ecomgstin,rate1  
	)aa	
where (CGST_AMT+SGST_AMT+IGST_AMT+cess_amt) > 0
order by Ecomgstin,rate1

IF NOT EXISTS(SELECT PART FROM #GSTR1 WHERE PARTSR = '7BB' AND SRNO ='B')
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(7,'7BB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

----8. Nil rated, exempted and non GST outward supplies
select gstin,Entry_ty,Supp_type ,St_type ,linerule,NilAmt =(case when lineRule ='Nil Rated'  then GRO_AMT else 0.00 end)
,ExemAmt =(case when lineRule ='Exempted'  then GRO_AMT else 0.00 end),NonGstAmt =(case when (HSNCODE ='' or LineRule ='Non GST')   then GRO_AMT else 0.00 end) 
,mentry
into #Gstr1Tbl1
from #GSTR1TBL
where (lineRule in('Nil Rated','Exempted','Non GST') or HSNCODE = '' )
and st_type in('INTERSTATE','INTRASTATE') 
AND (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') AND Supp_type NOT IN('EXPORT','SEZ','IMPORT','EOU')

---8A. Inter-State supplies to registered persons
set @AMT1 = 0.00 
set @AMT2 = 0.00
set @AMT3 = 0.00
SELECT @AMT1 =sum((case when mentry in('ST','SB','DN') THEN +(NilAmt) ELSE - (NilAmt) END )),
@AMT2 =sum((case when mentry in('ST','SB','DN') THEN +(ExemAmt) ELSE - (ExemAmt) END )) ,
@AMT3 =sum((case when mentry in('ST','SB','DN') THEN +(NonGstAmt) ELSE - (NonGstAmt) END )) 
FROM #Gstr1Tbl1 
WHERE st_type = 'Interstate' and gstin <>'Unregistered'

Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
VALUES(8,'8','A','','','','','','','','',0,0,0,@AMT1,0,@AMT2,0,@AMT3,'8A. Inter-State supplies to registered persons','','','')

-----8B. Intra- State supplies to registered persons
set @AMT1 = 0.00 
set @AMT2 = 0.00
set @AMT3 = 0.00
SELECT @AMT1 =sum((case when mentry in('ST','SB','DN') THEN +(NilAmt) ELSE - (NilAmt) END )),
@AMT2 =sum((case when mentry in('ST','SB','DN') THEN +(ExemAmt) ELSE - (ExemAmt) END )) ,
@AMT3 =sum((case when mentry in('ST','SB','DN') THEN +(NonGstAmt) ELSE - (NonGstAmt) END )) 
FROM #Gstr1Tbl1 WHERE st_type = 'Intrastate' and gstin <>'Unregistered'

Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
VALUES(8,'8','B','','','','','','','','',0,0,0,@AMT1,0,@AMT2,0,@AMT3,'8B. Intra- State supplies to registered persons','','','')

-----8C. Inter-State supplies to Unregistered persons
set @AMT1 = 0.00 
set @AMT2 = 0.00
set @AMT3 = 0.00
SELECT @AMT1 =sum((case when mentry in('ST','SB','DN') THEN +(NilAmt) ELSE - (NilAmt) END )),
@AMT2 =sum((case when mentry in('ST','SB','DN') THEN +(ExemAmt) ELSE - (ExemAmt) END )) ,
@AMT3 =sum((case when mentry in('ST','SB','DN') THEN +(NonGstAmt) ELSE - (NonGstAmt) END )) 
FROM #Gstr1Tbl1 
WHERE st_type = 'Interstate' and gstin ='Unregistered'

Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
VALUES(8,'8','C','','','','','','','','',0,0,0,@AMT1,0,@AMT2,0,@AMT3,'8C. Inter-State supplies to Unregistered persons','','','')

-----8D. Intra-State supplies to Unregistered persons
set @AMT1 = 0.00 
set @AMT2 = 0.00
set @AMT3 = 0.00
SELECT @AMT1 =sum((case when mentry in('ST','SB','DN') THEN +(NilAmt) ELSE - (NilAmt) END )),
@AMT2 =sum((case when mentry in('ST','SB','DN') THEN +(ExemAmt) ELSE - (ExemAmt) END )) ,
@AMT3 =sum((case when mentry in('ST','SB','DN') THEN +(NonGstAmt) ELSE - (NonGstAmt) END )) 
FROM #Gstr1Tbl1 
WHERE st_type = 'Intrastate' and gstin ='Unregistered'

Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
	,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
VALUES(8,'8','D','','','','','','','','',0,0,0,@AMT1,0,@AMT2,0,@AMT3,'8D. Intra-State supplies to Unregistered persons','','','')

----9. Amendments to taxable outward supply details furnished in returns for earlier tax periods in Table 4, 5 and 6 [including debit notes, credit notes, refund vouchers issued during current period and amendments thereof]
---9A. If the invoice/Shipping bill details furnished earlier were incorrect
select * into #gstr19AA 
from (
		----4A
		select * from  #gstr1amd where (mentry in('ST','SB') and entry_ty<>'UB') and st_type <> 'Out of country' 
		and supp_type IN('Registered','Compounding','E-Commerce')
		and gstin <>'' AND LineRule = 'Taxable' AND HSNCODE <> '' and Ecomgstin = ''
		and (SGSRT_AMT1 + CGSRT_AMT1  + IGSRT_AMT1 + cessr_amt1) = 0  and (SGST_AMT1 + CGST_AMT1  + IGST_AMT1 + cess_amt1) > 0
		----4B
		UNION ALL
		select * from #gstr1amd where (mentry in('ST','SB') and entry_ty<>'UB') and st_type <> 'Out of country' 
		and supp_type IN('Registered','Compounding','E-Commerce')
		and gstin <>'' AND LineRule = 'Taxable' AND HSNCODE <> '' and Ecomgstin = ''
		and (SGSRT_AMT1 + CGSRT_AMT1  + IGSRT_AMT1 + cessr_amt1) > 0 
		---4C
		UNION ALL
		SELECT * from #gstr1amd 
		where (mentry in('ST','SB') and entry_ty<>'UB') and st_type <> 'Out of country' and Ecomgstin <> '' AND LineRule = 'Taxable' 
		AND HSNCODE <> ''  and gstin not in('Unregistered') and (SGSRT_AMT1 + CGSRT_AMT1  + IGSRT_AMT1 + cessr_amt1) = 0  
		and (SGST_AMT1 + CGST_AMT1  + IGST_AMT1 + cess_amt1) > 0
		 
		---5A
		UNION ALL
		SELECT * from #gstr1amd where (mentry in('ST','SB') and entry_ty<>'UB') and st_type = 'InterState' and supp_type = 'Unregistered' 
		and gstin ='Unregistered' AND LineRule = 'Taxable' AND HSNCODE <> '' and Ecomgstin = ''	and net_amt >250000
		---5B
		UNION ALL 
		SELECT * from #gstr1amd where (mentry in('ST','SB') and entry_ty<>'UB') and st_type = 'InterState' and Ecomgstin <> ''
		AND LineRule = 'Taxable' AND HSNCODE <> ''  and net_amt >250000
		---6A
		UNION ALL 
		SELECT * FROM #gstr1amd where (mentry in('ST','SB') and entry_ty<>'UB') and st_type = 'Out of Country' AND HSNCODE <> '' AND LineRule = 'Taxable'
		UNION ALL 	
		SELECT *  FROM #gstr1amd where (mentry in('ST','SB') and entry_ty<>'UB') and st_type iN('INTERSTATE','INTRASTATE') 
		and supp_type in('Export','EOU','IMPORT','SEZ')  AND HSNCODE <> '' AND LineRule = 'Taxable'
	)aa
order by DATE,INV_NO

Insert Into #GSTR1(PART,PARTSR,SRNO,ORG_GSTIN,ORG_INVNO,ORG_DATE,gstin,inv_no,date,SBBILLNO,SBDATE,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,Cess_Amt,location,AC_NAME,SUPP_TYPE,ST_TYPE)
select 9 AS PART,'9AA' AS PARTSR,'A' AS SRNO,Consgstin AS org_gstin,ORG_INVNO,ORG_DATE,gstin,INV_NO,AMENDDATE
,SBBILLNO,SBDATE,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT
,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
from #gstr19AA  
where mentry in('ST','SB') and entry_ty<>'UB'
GROUP BY ORG_INVNO,ORG_DATE,Consgstin,INV_NO,AMENDDATE,SBBILLNO,SBDATE,pos,rate1 ,net_amt,gstin
Order by AMENDDATE,INV_NO

IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 9 AND PARTSR = '9AA' AND SRNO = 'A')	
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(9,'9AA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END

---9B. Debit Notes/Credit Notes/Refund voucher [original]

Insert Into #GSTR1(PART,PARTSR,SRNO,ORG_GSTIN,ORG_INVNO,ORG_DATE,gstin,inv_no,date,SBBILLNO,SBDATE,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,Cess_Amt,location,AC_NAME,SUPP_TYPE,ST_TYPE)
select * 
from (
		select 9 AS PART,'9AB' AS PARTSR,'B' AS SRNO
		,(CASE WHEN isnull(ltrim(rtrim(CONSGSTIN)),'') = '' THEN (case when isnull(ltrim(rtrim(GSTIN)),'') <> '' then ltrim(rtrim(GSTIN)) 
			else '' end) ELSE ltrim(rtrim(CONSGSTIN)) END) AS ORG_GSTIN
		,ORG_INVNO,ORG_DATE,gstin,INV_NO,DATE,SBBILLNO,SBDATE,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT
		,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1TBL
		where entry_ty ='RV' AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,Consgstin,INV_NO,DATE,SBBILLNO,SBDATE,pos,rate1 ,net_amt,gstin
		
		UNION ALL 
		
		SELECT 9 AS PART,'9AB' AS PARTSR,'B' AS SRNO
		,(CASE WHEN isnull(ltrim(rtrim(CONSGSTIN)),'') = '' THEN (case when isnull(ltrim(rtrim(GSTIN)),'') <> '' then ltrim(rtrim(GSTIN))
			else '' end) ELSE ltrim(rtrim(CONSGSTIN)) END) AS ORG_GSTIN
		,ORG_INVNO,ORG_DATE,gstin,INV_NO,DATE,SBBILLNO,SBDATE,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT
		,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1TBL 
		where mentry in('CN','DN','SR') and ST_TYPE = 'OUT OF COUNTRY' AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,Consgstin,INV_NO,DATE,SBBILLNO,SBDATE,pos,rate1,net_amt,gstin
		
		UNION ALL 
		
		SELECT 9 AS PART,'9AB' AS PARTSR,'B' AS SRNO
		,(CASE WHEN isnull(ltrim(rtrim(CONSGSTIN)),'') = '' THEN (case when isnull(ltrim(rtrim(GSTIN)),'') <> '' then ltrim(rtrim(GSTIN))
			else '' end) ELSE ltrim(rtrim(CONSGSTIN)) END) AS ORG_GSTIN
		,ORG_INVNO,ORG_DATE,gstin,INV_NO,DATE,SBBILLNO,SBDATE,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT
		,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1TBL 
		where mentry in('CN','DN','SR') and ST_TYPE <> 'OUT OF COUNTRY' AND SUPP_TYPE <> 'Unregistered' 
		AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,Consgstin,INV_NO,DATE,SBBILLNO,SBDATE,pos,rate1,net_amt,gstin
		
		UNION ALL 
		
		SELECT 9 AS PART,'9AB' AS PARTSR,'B' AS SRNO
		,(CASE WHEN isnull(ltrim(rtrim(CONSGSTIN)),'') = '' THEN (case when isnull(ltrim(rtrim(GSTIN)),'') <> '' then ltrim(rtrim(GSTIN))
			else '' end) ELSE ltrim(rtrim(CONSGSTIN)) END) AS ORG_GSTIN
		,ORG_INVNO,ORG_DATE,gstin,INV_NO,DATE,SBBILLNO,SBDATE,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT
		,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1TBL  where mentry in('CN','DN','SR') and ST_TYPE = 'INTERSTATE' AND SUPP_TYPE = 'Unregistered' 
		and (Entry_ty + QUOTENAME(Tran_cd)) in(select (Entry_ty + QUOTENAME(Tran_cd)) from #tempTbl1 where net_amt > 250000)
		AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,Consgstin,INV_NO,DATE,SBBILLNO,SBDATE,pos,rate1 ,net_amt,gstin
	)a
Order by DATE,INV_NO

IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART ='9' AND PARTSR = '9AB' AND SRNO = 'B' )
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(9,'9AB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END
	
---9C.Debit Notes/Credit Notes/Refund voucher [amendments thereof]

SELECT * 
INTO #tempTbl2 
FROM (
		select A.entry_ty,A.Tran_cd,A.Itserial,A.rentry_ty,A.Itref_tran,A.Ritserial,B.net_amt,B.INV_NO 
		from SRITREF A INNER JOIN #GSTR1AMD B ON (A.rentry_ty =B.ENTRY_TY AND A.Itref_tran =B.Tran_cd  AND A.Ritserial =B.ITSERIAL)
		UNION ALL
		select A.entry_ty,A.Tran_cd,A.Itserial,A.rentry_ty,A.Itref_tran,A.Ritserial,B.net_amt,B.INV_NO
		from OTHITREF A INNER JOIN #GSTR1AMD B ON (A.rentry_ty =B.ENTRY_TY AND A.Itref_tran =B.Tran_cd  AND A.Ritserial =B.ITSERIAL )
	)A
	

Insert Into #GSTR1(PART,PARTSR,SRNO,ORG_GSTIN,ORG_INVNO,ORG_DATE,gstin,inv_no,date,SBBILLNO,SBDATE,gro_amt,rate,taxableamt
	,CGST_AMT,SGST_AMT,IGST_AMT,Cess_Amt,location,AC_NAME,SUPP_TYPE,ST_TYPE)
SELECT * 
FROM (
		select 9 AS PART,'9AC' AS PARTSR,'C' AS SRNO,Consgstin AS org_gstin,ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE
		,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT
		,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1AMD  
		where entry_ty ='RV' AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE,pos,rate1,net_amt ,Consgstin
		
		UNION ALL

		SELECT 9 AS PART,'9AC' AS PARTSR,'C' AS SRNO,Consgstin AS org_gstin,ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE
		,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT
		,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1AMD  
		where mentry in('CN','DN','SR') and ST_TYPE = 'OUT OF COUNTRY' AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE
		,SBBILLNO,SBDATE,pos,rate1,net_amt ,Consgstin
		
		UNION ALL 
		
		select 9 AS PART,'9AC' AS PARTSR,'C' AS SRNO,Consgstin AS org_gstin,ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE
		,net_amt,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT
		,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1AMD  
		where mentry in('CN','DN','SR') and ST_TYPE <>'OUT OF COUNTRY' AND SUPP_TYPE <> 'Unregistered' AND HSNCODE <> '' 
		AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE,pos,rate1,net_amt ,Consgstin
		
		UNION ALL 
		
		select 9 AS PART,'9AC' AS PARTSR,'C' AS SRNO,Consgstin AS org_gstin,ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE
		,net_amt ,rate1,SUM(taxableamt)taxableamt,SUM(CGST_AMT1)CGST_AMT,SUM(SGST_AMT1)SGST_AMT,SUM(IGST_AMT1)IGST_AMT
		,SUM(cess_amt1)cess_amt,pos,'' as AC_NAME,'' as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1AMD  
		where mentry in('CN','DN','SR') and ST_TYPE ='INTERSTATE' AND SUPP_TYPE = 'Unregistered' 
		and (Entry_ty + QUOTENAME(Tran_cd)) in(select (Entry_ty + QUOTENAME(Tran_cd)) from #tempTbl2 where net_amt > 250000)
		AND HSNCODE <> '' AND LineRule = 'Taxable'
		GROUP BY ORG_INVNO,ORG_DATE,GSTIN,INV_NO,AMENDDATE,SBBILLNO,SBDATE,pos,rate1,Consgstin,net_amt
	)A
Order by AMENDDATE,INV_NO

IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART ='9' AND PARTSR = '9AC' AND SRNO = 'C' )
BEGIN
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(9,'9AC','C','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
END
	
-----10. Amendments to taxable outward supplies to Unregistered persons furnished in returns for earlier tax periods in Table 7
---10A. Intra-State Supplies [including supplies made through e-commerce operator attracting TCS] [Rate wise]

    Declare @rate1 decimal(10,2),@taxableamt1 decimal(18,2),@cgst_amt1 decimal(18,2),@sgst_amt1 decimal(18,2),@igst_amt1 decimal(18,2),@cess_amt1 decimal(18,2),@st_type varchar(20)
    ,@supp_type varchar(20),@COUNT INT
    
    Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(10,'10','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','') 
	
	select distinct supp_type=datename(mm,isnull(date,''))
	INTO #MONTHCOUNT
	from #GSTR1AMD 
	where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and supp_type ='Unregistered'  AND LineRule = 'Taxable'
	group by rate1,datename(mm,isnull(date,''))
	order by datename(mm,isnull(date,''))
		
	SELECT @COUNT = COUNT(*) FROM #MONTHCOUNT
	
	IF @COUNT = 0
	BEGIN
		IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10AA' AND SRNO ='A')
		BEGIN
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10AA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','') 
		END

		if not exists(select srno from #GSTR1 where part = 10 and partsr = '10AB' and srno='B')
		begin
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10AB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
		end

		IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10BA' AND SRNO='A')
		BEGIN
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10BA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
		END
		 
		IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10BB' AND SRNO='B')
		BEGIN
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10BB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
		END	
	END
		
	Declare curMonth cursor for 
		select supp_type=datename(mm,isnull(date,'')),ac_name=Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,''))
		from #GSTR1AMD 
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and supp_type ='Unregistered'  AND LineRule = 'Taxable'
		GROUP BY Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,'')),datename(mm,isnull(date,''))
		order by Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,''))
    OPEN curMonth
	FETCH NEXT FROM curMonth INTO @supp_type,@st_type
	WHILE @@FETCH_STATUS=0
	BEGIN
	IF @st_type<>''
	BEGIN
		print @st_type
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
			,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
		select 10 AS PART ,'10AA' AS PARTSR,'A' AS SRNO,'' as gstin,'' as inv_no,'' as date,'' as location,0.00 as Net_amt,rate1
		,taxableamt =sum((case when mentry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(sGST_AMT1) ELSE - (sGST_AMT1) END ))
		,IGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(iGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mentry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,'')) as AC_NAME
		,datename(mm,isnull(date,'')) as SUPP_TYPE,''as ST_TYPE
		from #GSTR1AMD 
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Intrastate' and supp_type ='Unregistered'  
		AND LineRule = 'Taxable' and Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,''))=@st_type
		group by rate1,Left(convert(varchar(50),date,112),6),datename(mm,isnull(date,''))
		order by rate1,Left(convert(varchar(50),date,112),6)
		
		IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10AA' AND SRNO ='A' and ac_name=@st_type)
		BEGIN
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10AA','A','','','','','','','','',0,0,0,0,0,0,0,0,@st_type,@supp_type,'','') 
		END
		
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
			,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,ecomgstin)
		select 10 AS PART ,'10AB' AS PARTSR,'B' AS SRNO,'' as gstin1,'' as inv_no,'' as date,'' as location
		,GRO_AMT=sum((case when mentry in('ST','SB','DN') THEN +(GRO_AMT)ELSE -(GRO_AMT) END )),rate1
		,taxableamt = sum((case when mentry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mentry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,'')) as AC_NAME,datename(mm,isnull(date,'')) as SUPP_TYPE
		,''as ST_TYPE ,Ecomgstin
		from #GSTR1AMD 
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Intrastate' and gsTin ='Unregistered'
		AND LineRule = 'Taxable' and Ecomgstin <> ''
		and (CGST_AMT1+SGST_AMT1+IGST_AMT1+cess_amt1) > 0
		and Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,''))=@st_type
		group by rate1,	Ecomgstin ,Left(convert(varchar(50),date,112),6),datename(mm,isnull(date,''))
		order by Ecomgstin,rate1,Left(convert(varchar(50),date,112),6)
		
		if not exists(select srno from #GSTR1 where part = 10 and partsr = '10AB' and srno='B' and ac_name=@st_type)
		begin
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10AB','B','','','','','','','','',0,0,0,0,0,0,0,0,@st_type,@supp_type,'','')
		end
				
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
			,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE)
		select 10 AS PART ,'10BA' AS PARTSR,'A' AS SRNO,'' as gstin, '' as inv_no, '' as date,pos,0.00 AS GRO_AMT,rate1
		,taxableamt = sum((case when mentry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mentry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,'')) as AC_NAME
		,datename(mm,isnull(date,'')) as SUPP_TYPE,'' as ST_TYPE
		from #GSTR1AMD 
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Interstate' and supp_type ='Unregistered' 
		and net_amt <=250000 and (CGST_AMT1+SGST_AMT1+IGST_AMT1+cess_amt1) > 0
		and Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,''))=@st_type
		group by rate1,	pos ,Left(convert(varchar(50),date,112),6),datename(mm,isnull(date,''))
		order by rate1,Left(convert(varchar(50),date,112),6)
		
		IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10BA' AND SRNO='A' and ac_name=@st_type)
		BEGIN
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10BA','A','','','','','','','','',0,0,0,0,0,0,0,0,@st_type,@supp_type,'','')
		END
		
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
			,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,ecomgstin)
		select 10 AS PART ,'10BB' AS PARTSR,'B' AS SRNO,'' as gstin, '' as inv_no, '' as date,'' as location,0.00 as net_amt,rate1
		,taxableamt = sum((case when mentry in('ST','SB','DN') THEN +(taxableamt)ELSE - (taxableamt) END ))
		,CGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(CGST_AMT1) ELSE - (CGST_AMT1) END ))
		,SGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(SGST_AMT1) ELSE - (SGST_AMT1) END ))
		,IGST_AMT = sum((case when mentry in('ST','SB','DN') THEN +(IGST_AMT1) ELSE - (IGST_AMT1) END ))
		,cess_amt = sum((case when mentry in('ST','SB','DN') THEN +(cess_amt1) ELSE - (cess_amt1) END ))
		,Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,'')) as AC_NAME
		,datename(mm,isnull(date,'')) as SUPP_TYPE,''as ST_TYPE,Ecomgstin
		from #GSTR1AMD 
		where (mentry in('ST','SB','CN','DN','SR') and entry_ty<>'UB') and st_type ='Interstate' and Ecomgstin <> ''
		AND LineRule = 'Taxable' and gstin = 'Unregistered'	AND net_amt <= 250000 
		and Left(convert(varchar(50),date,112),6)+'-'+datename(mm,isnull(date,''))=@st_type
		group by rate1,	Ecomgstin ,Left(convert(varchar(50),date,112),6),datename(mm,isnull(date,''))
		order by Ecomgstin,rate1,Left(convert(varchar(50),date,112),6)
		 
		IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10BB' AND SRNO='B' and ac_name=@st_type)
		BEGIN
			Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
				,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
			VALUES(10,'10BB','B','','','','','','','','',0,0,0,0,0,0,0,0,@st_type,@supp_type,'','')
		END	
	END
	ELSE
	BEGIN
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10AA' AND SRNO ='A')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(10,'10AA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','') 
	END

	if not exists(select srno from #GSTR1 where part = 10 and partsr = '10AB' and srno='B')
	begin
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(10,'10AB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	end


	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10BA' AND SRNO='A')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(10,'10BA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	END
	 
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 10 AND PARTSR = '10BB' AND SRNO='B')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(10,'10BB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	END	
	END
	FETCH NEXT FROM curMonth INTO @supp_type,@st_type
	END
	close curMonth
	Deallocate curMonth

-----11. Consolidated Statement of Advances Received/Advance adjusted in the current tax period/ Amendments of information furnished in earlier tax period
--I Information for the current tax period

	SELECT * 
	into #BANK_TMP 
	FROM (
			SELECT DISTINCT TRAN_CD,ENTRY_TY,POS,pos_std_cd,DATE 
			FROM #GSTR1TBL 
			WHERE (mentry IN('CR','BR','ST','SB') and entry_ty<>'UB')
		)bb
		
	select A.*,B.POS,B.POS_STD_CD,C.DATE 
	INTO #TaxAlloc_tmp 
	from TaxAllocation A INNER JOIN #BANK_TMP B ON (A.Itref_tran =B.TRAN_CD AND A.REntry_ty =B.Entry_ty)
	INNER JOIN #BANK_TMP C ON (A.Entry_ty =C.Entry_ty AND A.Tran_cd =C.Tran_cd) 
	WHERE C.DATE BETWEEN @SDATE AND @EDATE
	
	SELECT * 
	INTO #Tax_tmp 
	FROM (
			select PKEY = '+', rate1,pos ,Taxableamt,CGST_AMT1,SGST_AMT1,IGST_AMT1,Cess_amt1 
			from #GSTR1TBL 
			where mentry in('BR','CR') and (CGST_AMT1 + SGST_AMT1+IGST_AMT1) > 0
			union all 
			SELECT PKEY = '-',TaxRate,POS,Taxable,CGST_AMT,SGST_AMT,IGST_AMT,COMPCESS 
			FROM #TaxAlloc_tmp
		)AA
		
-----11A. Advance amount received in the tax period for which invoice has not been issued (tax amount to be added to output tax liability)
-----11A (1). Intra-State supplies (Rate Wise)
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Amenddate)
	Select * 
	From (
			select 11 AS PART ,'11AA' AS PARTSR,'A' AS SRNO,'' as gstin, '' as inv_no, '' as date ,POS,0.00 AS net_amt,rate1
			,taxableamt = sum(case when pkey ='+' then +Taxableamt else -Taxableamt end )
			,CGST_AMT = sum(case when pkey ='+' then +CGST_AMT1 else -CGST_AMT1 end )
			,SGST_AMT = sum(case when pkey ='+' then +SGST_AMT1 else -SGST_AMT1 end )
			,IGST_AMT = 0 
			,cess_amt = sum(case when pkey ='+' then +CESS_AMT1 else -CESS_AMT1 end )
			,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,'' as Amenddate
			from #Tax_tmp 
			where (isnull(SGST_AMT1,0) + isnull(CGST_AMT1,0)) > 0  
			group by rate1,POS
		)aab
	where (isnull(SGST_AMT,0) + isnull(CGST_AMT,0)) <> 0   
	order by rate1,POS
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 11 AND PARTSR ='11AA' AND SRNO = 'A')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(11,'11AA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	END
		
---- 11A (2). Inter-State Supplies (Rate Wise)
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Amenddate)
	select * 
	from ( 
			select 11 AS PART ,'11AB' AS PARTSR,'B' AS SRNO,'' as gstin, '' as inv_no, '' as date 
			,POS,0.00 AS net_amt ,rate1
			,taxableamt = sum(case when pkey ='+' then +Taxableamt else -Taxableamt end )
			,CGST_AMT = 0 
			,SGST_AMT = 0 
			,IGST_AMT = sum(case when pkey ='+' then +IGST_AMT1 else -IGST_AMT1 end )
			,cess_amt = sum(case when pkey ='+' then +CESS_AMT1 else -CESS_AMT1 end )
			,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,'' as Amenddate
			from #Tax_tmp where (isnull(IGST_AMT1,0)) > 0  
			group by rate1,POS
		)aab 
	where (isnull(IGST_AMT,0)) <> 0   order by rate1,POS
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 11 AND PARTSR ='11AB' AND SRNO = 'B')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(11,'11AB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	End
	
	-----11B. Advance amount received in earlier tax period and adjusted against the supplies being shown in this tax period in Table Nos. 4, 5, 6 and 7

	SELECT A.*,b.pos,b.pos_std_cd ,b.date into #TaxTemp2 
	FROM TaxAllocation A 
	inner join  #BANK_TMP b ON (A.Entry_ty =B.Entry_ty AND A.Tran_cd =b.Tran_cd)  
	where (a.REntry_ty+QUOTENAME(a.Itref_tran) not IN(select (Entry_ty + quotename(Tran_cd))  from #BANK_TMP 
														where Entry_ty in('BR','CR') 
														and DATE between @SDATE and @EDATE)) 
	and (b.date between @sdate and @edate) and a.REntry_ty  in('BR','CR')

	---11B (1). Intra-State Supplies (Rate Wise)
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Amenddate)
	select 11 AS PART ,'11BA' AS PARTSR,'A' AS SRNO,'' as gstin, '' as inv_no, '' as date ,POS,0.00 AS net_amt ,TaxRate 
	,taxableamt = sum(taxable)
	,CGST_AMT = sum(CGST_AMT),SGST_AMT = sum(SGST_AMT),IGST_AMT = sum(IGST_AMT),cess_amt = sum(COMPCESS)
	,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,'' as Amenddate
	from #TaxTemp2
	where (isnull(SGST_AMT,0) + isnull(CGST_AMT,0)) > 0
	group by TaxRate,POS 
	order by TaxRate,POS
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 11 AND PARTSR ='11BA' AND SRNO = 'A')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(11,'11BA','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	End
			
	---11B (2). Inter-State Supplies (Rate Wise)
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Amenddate)
	select 11 AS PART ,'11BB' AS PARTSR,'B' AS SRNO,'' as gstin, '' as inv_no, '' as date ,POS,0.00 AS net_amt ,TaxRate 
	,taxableamt = sum(taxable)
	,CGST_AMT = sum(CGST_AMT),SGST_AMT = sum(SGST_AMT),IGST_AMT = sum(IGST_AMT),cess_amt = sum(COMPCESS)
	,'' as AC_NAME,''as SUPP_TYPE,'' as ST_TYPE,'' as Amenddate
	from #TaxTemp2 
	where (isnull(IGST_AMT,0)) > 0
	group by TaxRate,POS 
	order by TaxRate,POS
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 11 AND PARTSR ='11BB' AND SRNO = 'B')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(11,'11BB','B','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	END
			
--------II Amendment of information furnished in Table No. 11[1] in GSTR-1 statement for earlier tax periods [Furnish revised information]
	SELECT *  
	INTO #TEMP2 
	FROM (
			SELECT TRAN_CD,ENTRY_TY, gststate as  POS, GSTSCODE as pos_std_cd,DATE,AmendDate 
			FROM STMAIN 
			UNION ALL 
			SELECT TRAN_CD,ENTRY_TY,gststate,GSTSCODE ,DATE,AmendDate 
			FROM SBMAIN 
		)AA
		
	SELECT DISTINCT TRAN_CD,ENTRY_TY,POS,pos_std_cd,DATE,AmendDate 
	INTO #BANK_AMD_TMP 
	FROM #GSTR1AMD 
	WHERE (mentry IN('BR','CR','ST','SB') and entry_ty<>'UB')
		
	SELECT * 
	INTO #TaxAlloc_tmp1 
	FROM (
			select distinct a.*,B.DATE AS B,b.AmendDate,C.date,C.POS,C.pos_std_cd 
			from TaxAllocation a 
			inner join #GSTR1AMD b on (a.Itref_tran =b.Tran_cd and a.rEntry_ty = b.Entry_ty)
			INNER JOIN #TEMP2 C  on ( C.Entry_ty =A.entry_ty AND C.Tran_cd =A.Tran_cd 
			and (quotename(month(c.date))+QUOTENAME(YEAR(c.date)))= (quotename(month(B.date))+QUOTENAME(YEAR(B.date))))
		)AA
	
	SELECT * 
	INTO #Tax_AMD_tmp 
	FROM (
			select PKEY = '+', rate1,pos ,Taxableamt,CGST_AMT1,SGST_AMT1,IGST_AMT1,Cess_amt1,DATE,AmendDate  
			from #GSTR1AMD 
			where mentry in('BR','CR') and (CGST_AMT1 + SGST_AMT1+IGST_AMT1) > 0
			union all 
			SELECT PKEY = '-',TaxRate,POS,Taxable,CGST_AMT,SGST_AMT,IGST_AMT,COMPCESS,DATE,AmendDate 
			FROM #TaxAlloc_tmp1 
		)AA

-----11A. Advance amount received in the tax period for which invoice has not been issued (tax amount to be added to output tax liability)
-----11A (1). Intra-State supplies (Rate Wise)
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Amenddate)
	Select * 
	From (
			select 11 AS PART ,'11CA' AS PARTSR,'' AS SRNO,'' as gstin, '' as inv_no, '' as date,POS,0.00 AS net_amt ,rate1
			,taxableamt = sum(case when pkey ='+' then +Taxableamt else -Taxableamt end )
			,CGST_AMT = sum(case when pkey ='+' then +CGST_AMT1 else -CGST_AMT1 end )
			,SGST_AMT = sum(case when pkey ='+' then +SGST_AMT1 else -SGST_AMT1 end )
			,IGST_AMT = 0
			,cess_amt = sum(case when pkey ='+' then +Cess_amt1 else -Cess_amt1 end ),'' as AC_NAME,datename(mm,isnull(date,''))as AmdMth 
			,'AA' as ST_TYPE,'' as Amenddate
			from #Tax_AMD_tmp 
			where (isnull(SGST_AMT1,0) + isnull(CGST_AMT1,0)) > 0 
			group by rate1,POS,datename(mm,isnull(date,'')) 
		)aab
	where (isnull(SGST_AMT,0) + isnull(CGST_AMT,0)) > 0 
	order by rate1,pos,datename(mm,isnull(date,''))
	
---- 11A (2). Inter-State Supplies (Rate Wise)
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,inv_no,date,location,gro_amt,rate,taxableamt
		,CGST_AMT,SGST_AMT,IGST_AMT,cess_amt,AC_NAME,SUPP_TYPE,ST_TYPE,Amenddate)
	select * 
	from (
			select 11 AS PART ,'11CA' AS PARTSR,'' AS SRNO,'' as gstin, '' as inv_no, '' as date,pos,0.00 AS net_amt ,rate1
			,taxableamt = sum(case when pkey ='+' then +Taxableamt else -Taxableamt end )
			,CGST_AMT = 0 
			,SGST_AMT = 0
			,IGST_AMT = sum(case when pkey ='+' then +IGST_AMT1 else -IGST_AMT1 end )
			,cess_amt = sum(case when pkey ='+' then +Cess_amt1 else -Cess_amt1 end )
			,'' as AC_NAME,datename(mm,isnull(date,''))as AmdMth 
			,'AB' as ST_TYPE,'' as Amenddate
			from #Tax_AMD_tmp 
			where (isnull(IGST_AMT1,0)) > 0 
			group by rate1,pos,datename(mm,isnull(date,''))
		)aab 
	where (isnull(IGST_AMT,0)) <> 0   
	order by rate1,pos,datename(mm,isnull(date,''))

-----11B. Advance amount received in earlier tax period and adjusted against the supplies being shown in this tax period in Table Nos. 4, 5, 6 and 7
---11B (1). Intra-State Supplies (Rate Wise)

	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 11 AND PARTSR ='11CA' AND SRNO = '')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(11,'11CA','','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	END
	
-----12. HSN-wise summary of outward supplies
 	select A.*,isnull(B.SERVTCODE,'') as SERVTCODE,B.SERTY INTO #GSTRT1TBL_HSN  from #GSTR1TBL A INNER JOIN IT_MAST B  ON (A.IT_CODE=B.IT_CODE)
	where  (A.mentry In('ST','SR','CN','DN','SB')  and entry_ty<>'UB')
    UPDATE #GSTRT1TBL_HSN SET SERVTCODE=ISNULL((select top 1 code from sertax_mast  where serty = #GSTRT1TBL_HSN.Serty),'')  where isnull(#GSTRT1TBL_HSN.SERVTCODE,'') = '' AND Isservice = 'services'
    ---------------------
	Insert into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,HSN_DESC,date
	,gro_amt,taxableamt,CGST_AMT,SGST_AMT,IGST_AMT,Cess_Amt,qty,AC_NAME,SUPP_TYPE,ST_TYPE,location)
		SELECT * FROM (
		  select 12 AS PART ,'12' AS PARTSR,'A' AS SRNO,'' as gstin,HSNCODE,HSN_DESC ,'' as date
         --,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +GRO_AMT ELSE -GRO_AMT END)GRO_AMT  --Commented by Priyanka B on 08032019 for Bug-31793
		 ,(CASE WHEN mentry IN('ST','DN','SB') THEN +NET_AMT ELSE -NET_AMT END)NET_AMT  --Modified by Priyanka B on 08032019 for Bug-31793
         ,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +Taxableamt ELSE -Taxableamt END)Taxableamt
         ,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +(ISNULL(CGST_AMT,0)) ELSE -(ISNULL(CGST_AMT,0)) END)CGST_AMT
		 ,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +(ISNULL(SGST_AMT,0)) ELSE -(ISNULL(SGST_AMT,0)) END)SGST_AMT
		 ,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +(ISNULL(IGST_AMT,0)) ELSE -(ISNULL(IGST_AMT,0)) END)IGST_AMT
		 ,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +(ISNULL(CESS_AMT,0)) ELSE -(ISNULL(CESS_AMT,0)) END)CESS_AMT
		 ,SUM(CASE WHEN mentry IN('ST','DN','SB') THEN +QTY ELSE -QTY END) AS QTY
		 ,'' as AC_NAME,''as SUPP_TYPE,uqc  ,'' as location
		FROM #GSTRT1TBL_HSN
		WHERE (mentry In('ST','SR','CN','DN','SB') and entry_ty<>'UB')
		and HSNCODE <>'' 
		group by HSNCODE,uqc,HSN_DESC
		,NET_AMT,mentry  --Added by Priyanka B on 08032019 for Bug-31793
		)AA order by HSNCODE,uqc

	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = 12 AND PARTSR ='12' AND SRNO = 'A')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,SGST_PER,SGST_AMT,CGST_PER,CGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(12,'12','A','','','','','','','','',0,0,0,0,0,0,0,0,'','','','')
	end
	
----13. Documents issued during the tax period
---1.Invoices for outward supply
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'A' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,1 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Invoices for outward supply('+ CASE WHEN a.ENTRY_TY = 'EI' THEN 'Export Sales' ELSE 'Sales' END +')' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM STMAIN a
	inner join lcode l on (a.entry_ty=l.entry_ty)
	WHERE (case when l.Bcode_nm<>'' then l.Bcode_nm else (case when l.ext_vou=1 then '' else l.entry_ty end) end) IN ('ST') and a.entry_ty<>'UB'
	and ( date between @sdate and @edate ) 
	GROUP BY INV_SR,a.ENTRY_TY
	
---For Services
---1.Invoices for outward supply
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'A' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,1 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Invoices for outward supply (Service Invoice)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
    FROM SBMAIN 
    WHERE ENTRY_TY IN('S1') and ( date between @sdate and @edate ) 
    GROUP BY INV_SR
    
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'A')
	BEGIN
		 Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','A','','','','','','','','',0,0,1,0,0,0,0,0,'Invoices for outward supply','','','','','')
	END 
	
-----2 Invoices for inward supply from Unregistered person		
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'B' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,2 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Invoices for inward supply from Unregistered person' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM STMAIN 
	WHERE ENTRY_TY IN('UB') and ( date between @sdate and @edate ) 
	GROUP BY INV_SR
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'B')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','B','','','','','','','','',0,0,2,0 ,0,0,0,0,'Invoices for inward supply from Unregistered person','','','','','')
	End

-----3 Revised Invoice
	SELECT DISTINCT CONS_AC_NAME,AMENDDATE,INV_NO,ENTRY_TY INTO #REVINVNO FROM #GSTR1AMD
	
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)	
	SELECT DISTINCT 13 AS PART,'13' AS PARTSR,'C' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,3 AS CGST_PER
	,CGST_AMT =(sum((Case when cons_ac_name !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when cons_ac_name ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when cons_ac_name !='' then 1 else 0 end)) - sum((Case when cons_ac_name ='CANCELLED.' then 1 else 0 end)))
	,'Revised Invoice ('+ltrim(rtrim(upper(b.code_nm)))+')' as ac_name
	,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM #REVINVNO a
	left join lcode b on (a.entry_ty=b.entry_ty)
	WHERE (amenddate between @sdate and @edate ) 
	GROUP BY a.entry_ty,b.code_nm--,inv_sr
	Order by 'Revised Invoice ('+ltrim(rtrim(upper(b.code_nm)))+')'
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'C')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','C','','','','','','','','',0,0,3,0 ,0,0,0,0,'Revised Invoice','','','',@from_srno,@to_srno)
	End
	
-----4 Debit Note
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'D' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,4 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Debit Note (DEBIT NOTE SUPPLY OF GOODS)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM DNMAIN 
	WHERE ENTRY_TY IN('GD') and ( date between @sdate and @edate )  AND AGAINSTGS ='SALES'  
	GROUP BY INV_SR
	
---FOR Services
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'D' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,4 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Debit Note (DEBIT NOTE SUPPLY OF SERVICES)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM DNMAIN 
	WHERE ENTRY_TY IN('D6') and ( date between @sdate and @edate )  AND AGAINSTGS ='SERVICE INVOICE'  
	GROUP BY INV_SR
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'D')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','D','','','','','','','','',0,0,4,0,0,0,0,0,'Debit Note','','','','','')
	end
	
-----5 Credit Note
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'E' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,5 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Credit Note(CREDIT NOTE SUPPLY OF GOODS)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM CNMAIN 
	WHERE ENTRY_TY IN('GC') and ( date between @sdate and @edate )  AND AGAINSTGS ='SALES' 
	GROUP BY INV_SR
	
	-----For Services
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'E' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,5 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Credit Note(CREDIT NOTE SUPPLY OF SERVICES)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM CNMAIN
	WHERE ENTRY_TY IN('C6') and ( date between @sdate and @edate ) AND AGAINSTGS ='SERVICE INVOICE' 
	GROUP BY INV_SR
	
	-----For Sales Return
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'E' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,5 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Credit Note(Sales Return)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM SRMAIN
	WHERE ENTRY_TY IN('SR') and ( date between @sdate and @edate )
	GROUP BY INV_SR
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'E')
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','E','','','','','','','','',0,0,5,0,0,0,0,0,'Credit Note','','','','','')
	end 
	
-----6 Receipt voucher
-----Bank Receipt
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'F' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,6 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Receipt voucher (BANK RECEIPT)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM BRMAIN 
	WHERE ENTRY_TY IN('BR') and ( date between @sdate and @edate ) 
	GROUP BY inv_sr 
	
----Cash Receipt 
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'F' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,6 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Receipt voucher (CASH RECEIPT)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no)
	,to_srno = max(inv_no)
	FROM CRMAIN 
	WHERE ENTRY_TY IN('CR') and ( date between @sdate and @edate ) 
	GROUP BY inv_sr
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'F')	
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','F','','','','','','','','',0,0,6,0,0,0,0,0,'Receipt voucher','','','','','')
	END
	
-----7 Payment Voucher
-----Bank Payment
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'G' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,7 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Payment Voucher(BANK PAYMENT)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no),to_srno = max(inv_no)
	FROM BPMAIN 
	WHERE ENTRY_TY IN('BP') and ( date between @sdate and @edate ) 
	GROUP BY inv_sr
	
-----Cash Payment
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'G' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,7 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Payment Voucher(CASH PAYMENT)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no),to_srno = max(inv_no)
	FROM CPMAIN 
	WHERE ENTRY_TY IN('CP') and ( date between @sdate and @edate ) 
	GROUP BY inv_sr
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'G')	 
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','G','','','','','','','','',0,0,7,0,0,0,0,0,'Payment Voucher','','','','','')
	END

-----8 Refund voucher
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'H' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,8 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Refund voucher' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=MIN(inv_no),to_srno = max(inv_no)
	FROM BPMAIN 
	WHERE ENTRY_TY IN('RV') and ( date between @sdate and @edate ) 
	GROUP BY inv_sr
	
	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'H')	 
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
		VALUES(13,'13','H','','','','','','','','',0,0,8,0,0,0,0,0,'Refund voucher','','','','','')
	END
	
-----9 Delivery Challan for job work
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'I' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,9 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER  
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Delivery Challan for job work (Annexure IV)' AS AC_NAME,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode,from_srno=entry_ty+'/'+MIN(inv_no),to_srno = entry_ty+'/'+max(inv_no)
	FROM IIMAIN 
	WHERE ENTRY_TY IN('LI') and ( date between @sdate and @edate ) 
	GROUP BY inv_sr,entry_ty

	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'I')	 
	BEGIN
		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(13,'13','I','','','','','','','','',0,0,9,0,0,0,0,0,'Delivery Challan for job work','','','')
	END

-----10 Delivery Challan for supply on approval
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(13,'13','J','','','','','','','','',0,0,10,0,0,0,0,0,'Delivery Challan for supply on approval','','','')
-----11 Delivery Challan in case of liquid gas
	Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
	VALUES(13,'13','K','','','','','','','','',0,0,11,0,0,0,0,0,'Delivery Challan in case of liquid gas','','','')
-----12 Delivery Challan in cases other than by way of supply (excluding at S no. 9 to 11)
   Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'L' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,12 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Delivery Challan in cases other than by way of supply (excluding at S no. 9 to 11)(Annexure V)' AS AC_NAME 
	,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode
	,from_srno=entry_ty+'/'+MIN(inv_no),to_srno = entry_ty+'/'+max(inv_no) 
	FROM IIMAIN 
	WHERE ENTRY_TY IN('IL') and ( date between @sdate and @edate )
	GROUP BY inv_sr	,entry_ty  
	
   Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
		,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode,from_srno,to_srno)
	SELECT 13 AS PART,'13' AS PARTSR,'L' AS SRNO,'' AS gstin,'' AS HSNCODE,'' AS isService,'' AS location,'' AS inv_no,'' AS date
	,'' AS ORG_INVNO,'' AS ORG_DATE,0 AS gro_amt,0 AS taxableamt,12 AS CGST_PER
	,CGST_AMT =(sum((Case when PARTY_NM !='' then 1 else 0 end))),0 AS SGST_PER
	,SGST_AMT =(sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end))),0 AS IGST_PER
	,IGST_AMT = (sum((Case when PARTY_NM !='' then 1 else 0 end)) - sum((Case when PARTY_NM ='CANCELLED.' then 1 else 0 end)))
	,'Delivery Challan in cases other than by way of supply (excluding at S no. 9 to 11)(Branch Transfer)' AS AC_NAME 
	,'' AS SUPP_TYPE,'' AS ST_TYPE,'' AS StateCode
	,from_srno=+MIN(inv_no),to_srno = +max(inv_no) 
	FROM DCMAIN 
	WHERE ENTRY_TY IN('DC') and ( date between @sdate and @edate ) and ISNULL(MTRNTYPE,'')='Branch Transfer'  
	GROUP BY inv_sr	,entry_ty  

	IF NOT EXISTS(SELECT SRNO FROM #GSTR1 WHERE PART = '13' AND PARTSR = '13' AND SRNO = 'L')	 
	BEGIN
 		Insert Into #GSTR1(PART,PARTSR,SRNO,gstin,HSNCODE,isService,location,inv_no,date,ORG_INVNO,ORG_DATE,gro_amt,taxableamt
			,CGST_PER,CGST_AMT,SGST_PER,SGST_AMT,IGST_PER,IGST_AMT,AC_NAME,SUPP_TYPE,ST_TYPE,StateCode)
		VALUES(13,'13','L','','','','','','','','',0,0,12,0,0,0,0,0,'Delivery Challan in cases other than by way of supply (excluding at S no. 9 to 11)','','','')
	end	
	
	UPDATE #GSTR1 
	SET rptmonth =datename(mm,@SDATE),rptyear =year(@SDATE) 
	,GSTIN =(CASE WHEN ISNULL(gstin,'')='UNREGISTERED' THEN '' ELSE isnull(gstin,'') END )
	,ORG_GSTIN =(CASE WHEN ISNULL(ORG_GSTIN,'')='UNREGISTERED' THEN '' ELSE isnull(ORG_GSTIN,'') END )
	,Ecomgstin =(CASE WHEN ISNULL(Ecomgstin,'')='UNREGISTERED' THEN '' ELSE ISNULL(Ecomgstin,'')  END )
	
IF ISNULL(@EXPARA,'') = ''
BEGIN
	SELECT * FROM #GSTR1
	order by PART,case when PART=10 then ac_name else '' end,partsr ,tran_cd
END
ELSE 
BEGIN 
	---- Below code for Csv file Geration...  
		---Sectiona 4A
		SELECT * INTO #GSTR_CSV FROM #GSTR1 ORDER BY PART,PARTSR,SRNO,date
		SELECT * FROM ( 
		Select 4 as Section,
		'Section' + '|' +  
		'GSTIN/UIN' + '|' +  
		'Invoice No.' + '|' +
		'Invoice Date' + '|' +
		'Invoice Value' + '|' +
		'Rate' + '|' +
		'Taxable value' + '|' +
		'Integrated Tax' + '|' +
		'Central Tax' + '|' +
		'State/UT Tax' + '|' +
		'Cess' + '|' +
		'Place of Supply (Name of State)'   as ColumnDetails
	    UNION ALL 
	    SELECT 4 as Section ,
	    '4A' + '|'  + 
	    RTRIM(LTRIM(isnull(GSTIN,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)< =1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_amt,0) AS VARCHAR) + '|'  + 
		CAST(isnull(cess_amt,0) AS VARCHAR) + '|'  +
		RTRIM(LTRIM(isnull(Location,''))) 
	    FROM  #GSTR_CSV WHERE PARTSR = '4A' AND SRNO = 'A'
	    UNION ALL 
		---Section 4b
	    SELECT 4 as Section ,
	    '4B' + '|'  + 
	    RTRIM(LTRIM(isnull(GSTIN,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)< =1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(cess_amt,0) AS VARCHAR) + '|' +  
		RTRIM(LTRIM(isnull(Location,''))) 
	    FROM  #GSTR_CSV WHERE PARTSR = '4B' AND SRNO = 'A'
		---4C
	    UNION ALL 
	    SELECT 4 as Section ,
	    '4C' + '|'  + 
	    RTRIM(LTRIM(isnull(GSTIN,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)<=1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(cess_amt,0) AS VARCHAR) + '|'   +
		RTRIM(LTRIM(isnull(Location,''))) 
	    FROM  #GSTR_CSV WHERE PARTSR = '4C' AND SRNO = 'A'
	
		---Section 5
		UNION ALL 
		Select 5 as Section,
		'Section' + '|' +  
		'GSTIN/UIN' + '|' +  
		'Place of Supply (Name of State)' + '|' +  
		'Invoice No.' + '|' +
		'Invoice Date' + '|' +
		'Invoice Value' + '|' +
		'Rate' + '|' +
		'Taxable value' + '|' +
		'Integrated Tax' + '|' +
		'Cess'   as ColumnDetails
	    UNION ALL 
	    SELECT 5 as Section ,
	    '5A' +'|'  + 
		'' +'|'  + 
	    RTRIM(LTRIM(isnull(Location,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)<=1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  + 
		CAST(isnull(cess_amt,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '5A' AND SRNO = 'A' 
	    UNION ALL 
	    SELECT 5 as Section ,
	    '5B' + '|' +  
	    RTRIM(LTRIM(isnull(ecomgstin,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(Location,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)<=1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  + 
		CAST(isnull(cess_amt,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '5B' AND SRNO = 'A' 
		UNION ALL 
		----6A
		Select 6 as Section,
		'Section' + '|' +  
		'GSTIN of recipient' + '|' +  
		'Invoice No.' + '|' +
		'Invoice Date' + '|' +
		'Invoice Value' + '|' +
		'Shipping Bill No. of export' + '|' +
		'Shipping Bill Date. of export' + '|' +
		'Integrated Rate' + '|' + 
		'Integrated Taxable value' + '|' +
		'Integrated Tax Amt' + '|' +
		'Central Rate' + '|' +
		'Central Taxable value' + '|' +
		'Central Tax Amt' + '|' +
		'State/UT Rate' + '|' +
		'State/UT Taxable value' + '|' +
		'State/UT Tax Amt' + '|' +
		'Comp. Cess' 
		as ColumnDetails
	    UNION ALL 
	    SELECT 6 as Section ,
	    '6A' + '|' +  
		'' +'|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)<=1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    RTRIM(LTRIM(isnull(SBBILLNO,''))) + '|'  + 
	    CASE WHEN YEAR(isnull(SBDATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,isnull(SBDATE,''),103) END  + '|'  + 
	    CAST(isnull(IGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(CGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CESS_AMT,0) AS VARCHAR)
	    FROM  #GSTR_CSV WHERE PARTSR = '6A' AND SRNO = 'A' 
	    UNION ALL 
	    SELECT 6 as Section ,
	    '6B' + '|' +  
		RTRIM(LTRIM(isnull(gstin,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)<=1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    RTRIM(LTRIM(isnull(SBBILLNO,''))) + '|'  + 
	    CASE WHEN YEAR(isnull(SBDATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,isnull(SBDATE,''),103) END  + '|'  + 
	    CAST(isnull(IGST_PER,0) AS VARCHAR) + '|'  +
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(CGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CESS_AMT,0) AS VARCHAR)
	    FROM  #GSTR_CSV WHERE PARTSR = '6B' AND SRNO = 'A' 
	    UNION ALL 
	    SELECT 6 as Section ,
	    '6C' + '|' +  
		RTRIM(LTRIM(isnull(gstin,''))) + '|'  + 
	    RTRIM(LTRIM(isnull(inv_no,''))) + '|'  + 
	    CASE WHEN YEAR(DATE)<=1900 THEN '' ELSE CONVERT(VARCHAR,DATE,103) END  + '|'  + 
	    CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  + 
	    RTRIM(LTRIM(isnull(SBBILLNO,''))) + '|'  + 
	    CASE WHEN YEAR(isnull(SBDATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,isnull(SBDATE,''),103) END  + '|'  + 
	    CAST(isnull(IGST_PER,0) AS VARCHAR) + '|'  +
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_PER,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(SGST_amt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(CESS_AMT,0) AS VARCHAR)
	    FROM  #GSTR_CSV WHERE PARTSR = '6C' AND SRNO = 'A' 
		--- 7a
		Union all
		Select 7 as Section,
		'Section' + '|' +  
		'GSTIN/POS' + '|' +  
		'Rate of Tax' + '|' +
		'Total Taxable Value' + '|' +
		'Integrated Amount' + '|' +
		'Central Tax Amount' + '|' +
		'State Tax/UT Tax Amount' + '|' +
		'Cess Amount'  as ColumnDetails
		UNION ALL 
	    SELECT 7 as Section ,
	    '7A(1)' + '|' + 
		'' +'|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(Cess_Amt,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '7AA' AND SRNO = 'A' 
		UNION ALL 
	    SELECT 7 as Section ,
	    '7A(2)' + '|' +  
		CAST(isnull(Ecomgstin,'') AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(Cess_Amt,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '7AB' AND SRNO = 'B' 
		--- 7b
		UNION ALL 
	    SELECT 7 as Section ,
	    '7B(1)' + + '|'  + 
		isnull(RTRIM(location),'')  + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(Cess_Amt,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '7BA' AND SRNO = 'A' 
		UNION ALL 
	    SELECT 7 as Section ,
	    '7B(2)' + + '|'  +
		CAST(isnull(Ecomgstin,'') AS VARCHAR) + '|'  + 
	    CAST(isnull(RATE,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(taxableamt,0) AS VARCHAR) + '|'  + 
	    CAST(isnull(IGST_amt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(Cess_Amt,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '7BB' AND SRNO = 'B' 
		--- 8
		Union all
		Select 8 as Section,
		'Description' + '|' +  
		'Nil Rated Supplies' + '|' + 
		'Exempted(Other than Nil rated/non-GST supply)' + '|' +  
		'Non-GST supplies'   as ColumnDetails
		UNION ALL 
	    SELECT 8 as Section ,
		isnull(RTRIM(ac_name),'')  + '|'  + 
	    CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR = '8'  and SRNO in('A','B','C','D')
		--- 9
		Union all
		Select 9 as Section,
		'Section' + + '|'  +
		'Original GSTIN' + '|' +  
		'Original Inv. No.' + '|' +  
		'Original Inv. Date' + '|' +  
		'GSTIN' + '|' + 
		'Invoice No.' + '|' + 
		'Invoice Date.' + '|' + 
		'Shipping bill No.' + '|' + 
		'Shipping bill Date.' + '|' + 
		'Value' + '|' + 
		'Rate' + '|' + 
		'Taxable Value' + '|' + 
		'Integrated Tax' + '|' + 
		'Central Tax' + '|' + 
		'State/UT Tax' + '|' + 
		'Cess' + '|' + 
		'Place of supply'   as ColumnDetails
		
		UNION ALL 
	    SELECT 9 as Section ,
	    '9A' + + '|'  +
		isnull(RTRIM(ORG_GSTIN),'')  + '|'  + 
		isnull(RTRIM(ORG_INVNO),'')  + '|'  + 
		CASE WHEN YEAR(isnull(ORG_DATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,isnull(ORG_DATE,''),103) END  + '|'  + 
		isnull(RTRIM(GSTIN),'')  + '|'  + 
		isnull(RTRIM(inv_no),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(DATE,''))<=1900 THEN '' ELSE  CONVERT(VARCHAR,ISNULL(DATE,'') ,103) END + '|'  + 
		isnull(RTRIM(SBBILLNO),'')  + '|'  + 
		CASE WHEN YEAR(isnull(SBDATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,isnull(SBDATE,''),103) END  + '|'  + 
		CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  
	    FROM  #GSTR_CSV WHERE PARTSR IN('9AA')
		UNION ALL 
	    SELECT 9 as Section ,
	    '9B' + + '|'  +
		isnull(RTRIM(ORG_GSTIN),'')  + '|'  + 
		isnull(RTRIM(ORG_INVNO),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(ORG_DATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,ISNULL(ORG_DATE,''),103) END  + '|'  + 
		isnull(RTRIM(GSTIN),'')  + '|'  + 
		isnull(RTRIM(inv_no),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(DATE,''))<=1900 THEN '' ELSE  CONVERT(VARCHAR,ISNULL(DATE,'') ,103) END + '|'  + 
		
		isnull(RTRIM(SBBILLNO),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(SBDATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,ISNULL(SBDATE,''),103) END  + '|'  + 
		CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  
	    FROM  #GSTR_CSV WHERE PARTSR IN('9AB')
		UNION ALL 
	    SELECT 9 as Section ,
	    '9C' + + '|'  +
		isnull(RTRIM(ORG_GSTIN),'')  + '|'  + 
		isnull(RTRIM(ORG_INVNO),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(ORG_DATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,ISNULL(ORG_DATE,''),103) END  + '|'  + 
		isnull(RTRIM(GSTIN),'')  + '|'  + 
		isnull(RTRIM(inv_no),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(DATE,''))<=1900 THEN '' ELSE  CONVERT(VARCHAR,ISNULL(DATE,'') ,103) END + '|'  + 
		isnull(RTRIM(SBBILLNO),'')  + '|'  + 
		CASE WHEN YEAR(ISNULL(SBDATE,''))<=1900 THEN '' ELSE CONVERT(VARCHAR,ISNULL(SBDATE,''),103) END  + '|'  + 
		CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  
	    FROM  #GSTR_CSV WHERE PARTSR IN('9AC' )
		---Section 10
		Union all
		Select 10 as Section,
		'Section' + + '|'  +
		'Month' + + '|'  +  
		'GSTIN/POS' + '|' +  
		'Rate of Tax' + '|' +  
		'Total Taxable Value' + '|' + 
		'Integrated Tax' + '|' + 
		'Central Tax' + '|' + 
		'State/UT Tax' + '|' + 
		'Cess'  as ColumnDetails
		UNION ALL 
	    SELECT '10' as Section ,
	    '10A'  + '|'  + 
	    rtrim(ltrim(isnull(supp_type,'')))  + '|'  + 
		'' + '|'  + 
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('10AA')
		UNION ALL 
	    SELECT 10 as Section ,
	    '10A(1)'  + '|'  + 
	    rtrim(ltrim(isnull(supp_type,'')))  + '|'  + 
		isnull(RTRIM(Ecomgstin),'')  + '|'    + 
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('10AB')
		UNION ALL 
	    SELECT 10 as Section ,
	    '10B'  + '|'  + 
	    rtrim(ltrim(isnull(supp_type,'')))  + '|'  + 
		isnull(RTRIM(location),'')  + '|'    + 
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('10BA')
		UNION ALL 
	    SELECT 10 as Section ,
	    '10B(1)'  + '|'  + 
	    rtrim(ltrim(isnull(supp_type,'')))  + '|'  + 
		isnull(RTRIM(Ecomgstin),'')  + '|'    + 
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('10BB')
		--- 11
		Union all
		Select 11 as Section,
		'Section' + '|' + 
		'Rate' + '|' + 
		'Gross Advance Received/adjusted' + '|' + 
		'Place of supply(Name of State)' + '|' + 
		'Integrated Tax' + '|' + 
		'Central Tax' + '|' + 
		'State/UT Tax' + '|' + 
		'Cess'   as ColumnDetails
		UNION ALL 
	    SELECT '11' as Section ,
	    '11A(1)' + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  + '|'   +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('11AA')

		UNION ALL 
	    SELECT 11 as Section ,
	    '11A(2)' + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  + '|'   +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('11AB')

		UNION ALL 
	    SELECT 11 as Section ,
	    '11B(1)' + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  + '|'   +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('11BA')

		UNION ALL 
	    SELECT 11 as Section ,
	    '11B(2)' + '|'  +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  + '|'   +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('11BB')
	    		
		UNION ALL 
		Select 11.2 as Section,
		'Month' + '|' + 
		'Rate' + '|' + 
		'Gross Advance Received/adjusted' + '|' + 
		'Place of supply(Name of State)' + '|' + 
		'Integrated Tax' + '|' + 
		'Central Tax' + '|' + 
		'State/UT Tax' + '|' + 
		'Cess' +'|'+ 
		'11A(1)' +'|'+ 
		'11A(2)' +'|'+ 
		'11B(1)' +'|'+ 
		'11B(2)' 
		 as ColumnDetails
		UNION ALL 
	    SELECT 11.2 as Section ,
	    ltrim(RTRIM(isnull(SUPP_TYPE,'')))  + '|'   +
		CAST(isnull(RATE,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
		isnull(RTRIM(Location),'')  + '|'   +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) + '|'  +
	    (case when st_type = 'AA' THEN 'Yes' else '' end) +'|'+
	    (case when st_type = 'AB' THEN 'Yes' else '' end) +'|'+
	    (case when st_type = 'BA' THEN 'Yes' else '' end) +'|'+ 
	    (case when st_type = 'BB' THEN 'Yes' else '' end)  
	    FROM  #GSTR_CSV WHERE PARTSR IN('11CA')
	    
	    
		--- 12
		Union all
		Select 12 as Section,
		'HSN' + '|' + 
		'Description(Optional if HSN is provided' + '|' + 
		'UQC' + '|' + 
		'Total Quantity' + '|' + 
		'Total Value' + '|' + 
		'Total Taxable Value' + '|' + 
		'Integrated Tax' + '|' + 
		'Central Tax' + '|' + 
		'State/UT Tax' + '|' + 
		'Cess'   as ColumnDetails
		UNION ALL 
	    SELECT 12 as Section ,
		rtrim(ltrim(isnull(HSNCODE,''))) + '|'  +
		'' + '|'  +
		ltrim(rtrim(isnull(st_TYPE,''))) + '|'  +
		CAST(isnull(qty,0) AS VARCHAR) + '|'  +
		CAST(isnull(gro_amt,0) AS VARCHAR) + '|'  +
		CAST(isnull(Taxableamt,0) AS VARCHAR) + '|'  +
	    CAST(isnull(IGST_AMT,0) AS VARCHAR) + '|'  +
		CAST(isnull(CGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(SGST_AMT,0) AS VARCHAR) + '|'  +
	    CAST(isnull(cess_AMT,0) AS VARCHAR) 
	    FROM  #GSTR_CSV WHERE PARTSR IN('12')
		--- 13
		Union all
		Select 13 as Section,
		'Sr. No.' + '|' + 
		'Nature of document' + '|' + 
		'Sr. No. From' + '|' + 
		'To' + '|' + 
		'Total number' + '|' + 
		'Cancelled' + '|' + 
		'Net issued'  as ColumnDetails
		UNION ALL 
	    SELECT 13 as Section ,
		CAST(isnull(cast(CGST_PER as integer) ,0) AS VARCHAR) + '|'  +
		isnull(rtrim(ac_name),'') + '|'  +
		isnull(rtrim(from_srno),'') + '|'  +
		isnull(rtrim(to_srno),'') + '|'  +
		CAST(isnull(cast(CGST_AMT as integer) ,0) AS VARCHAR) + '|'  +
		CAST(isnull(cast(sGST_AMT as integer) ,0) AS VARCHAR) + '|'  +
		CAST(isnull(cast(iGST_AMT as integer) ,0) AS VARCHAR)
	    FROM  #GSTR_CSV WHERE PARTSR IN('13') )AA ORDER BY Section
	End 
END
GO
