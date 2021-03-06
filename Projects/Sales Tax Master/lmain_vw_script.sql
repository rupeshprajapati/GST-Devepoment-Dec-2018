
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER     view [dbo].[lmain_vw] as
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from ARmain
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from BPmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from BRmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from CNmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from CPmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from CRmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from DCmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from DNmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from EPmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from EQmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from ESmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from IImain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from IPmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from IRmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from JVmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from OBmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from OPmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from PCmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from POmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from PTmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from PRmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from SOmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from SQmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from SRmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from SSmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from STmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from TRmain       
union all
select tran_cd,entry_ty,date,doc_no,ac_id,party_nm,cate,dept,inv_no,inv_sr,gro_amt,net_amt,cheq_no,u_pinvno,u_pinvdt,bank_nm,narr,re_all,l_yn,due_dt,[rule],tax_name
from main
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

