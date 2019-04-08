&&Amrendra for Bug-6496 on 27/09/2012 ---->
_Malias 	= Alias()
_mRecNo	= Recno()
_curvouobj = _Screen.ActiveForm
Set DataSession To _curvouobj.DataSessionId
QcRetstate=.T.


*Birendra : Bug-7465 on 11/12/2012 :Start:
If Not "vutex" $ vchkprod
	If Inlist(.PcvType,'AR','PT','OP','P1')
		lcStr  = [SELECT top 1 * FROM OTHITREF A where a.rentry_ty = ?Main_vw.Entry_Ty AND a.itref_Tran = ?Main_vw.Tran_cd and a.rl_yn = ?Main_vw.l_yn]
		sql_con = _curvouobj.sqlconobj.Dataconn("EXE",company.dbname,lcStr,"tmp_othitref","This.Parent.nHandle", .DataSessionId)
		If sql_con > 0 And Used('tmp_othitref')
			Select tmp_othitref
			If Reccount() > 0
				Messagebox("Entry Passed Against "+tmp_othitref.entry_ty+".",64,vuMess)
				Use In tmp_othitref
				If !Empty(_Malias)
					Select &_Malias
				Endif
				If Betw(_mRecNo,1,Reccount())
					Go _mRecNo
				Endif
				Return .F.
			Endif
		Endif
	Endif
Endif
*Birendra : Bug-7465 on 11/12/2012 :End:

If Type('_curvouobj.PCVTYPE')='C' And ([vuexc] $ vchkprod)
	If oGlblPrdFeat.UdChkProd('qctrl') And (Inlist(.PcvType,'AR','PT','OP') Or Inlist(.Behave,'AR','PT','OP'))
		QcRetstate=.F.
		sql_con = .sqlconobj.Dataconn([EXE],company.dbname,[Select Top 1 Entry_ty From qc_inspection_master where ;
			Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd],[tmptbl_vw],"This.Parent.nHandle",.DataSessionId,.F.)
		lResultInsp = .F.
		If sql_con > 0 And Used('tmptbl_vw')
			Select tmptbl_vw
			If Reccount() = 0
				lResultInsp =.T.
			Endif
		Endif

		sql_con = .sqlconobj.Dataconn([EXE],company.dbname,[Select Top 1 Entry_ty From qchistory where ;
			Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd],[tmptbl_vw],"This.Parent.nHandle",.DataSessionId,.F.)
		lResultCtrl = .F.
		If sql_con > 0 And Used('tmptbl_vw')
			Select tmptbl_vw
			If Reccount() = 0
				lResultCtrl =.T.
			Endif
		Endif

		Do Case
			Case lResultInsp =.F. And lResultCtrl =.F.
				Msg = "QC Entry Passed against this Voucher,"+Chr(13)+;
					"Are you sure you wish to delete this Voucher?"
				If .ShowMessageBox(Msg,4+32+256,vuMess,1)=7
					Return .F.
				Endif
			Case lResultInsp =.F.
				Msg = "QC Entry Passed against this Voucher,"+Chr(13)+;
					"Are you sure you wish to delete this Voucher?"
				If .ShowMessageBox(Msg,4+32+256,vuMess,1)=7
					Return .F.
				Endif
			Case lResultCtrl =.F.
				Msg = "QC Entry Passed against this Voucher,"+Chr(13)+;
					"Are you sure you wish to delete this Voucher?"
				If .ShowMessageBox(Msg,4+32+256,vuMess,1)=7
					Return .F.
				Endif
*** Added for Bug-6965 on 23/10/2012 ---->
			Case lResultInsp =.T. And lResultCtrl =.T.
				If !Empty(_Malias)
					Select &_Malias
				Endif
				If Betw(_mRecNo,1,Reccount())
					Go _mRecNo
				Endif
				QcRetstate=.T.		&& Added by Shrikant S. on 14/04/2018 for Auto updater 13.0.6
*!*				Return .T.			&& Commented by Shrikant S. on 14/04/2018 for Auto updater 13.0.6
*** Added for Bug-6965 on 23/10/2012 <----
		Endcase

		If (lResultCtrl =.F. Or lResultInsp =.F.) And !(lResultInsp =.T. And lResultCtrl =.T.)				&& Added by Shrikant S. on 14/04/2018 for Auto updater 13.0.6
*!*			If lResultCtrl =.F. Or lResultInsp =.F.			&& Commented by Shrikant S. on 14/04/2018 for Auto updater 13.0.6
&& Added by Shrikant S. on 29/09/2014 	for Bug-23879		&& Start
			lnInsp_id=0
			sql_con = .sqlconobj.Dataconn([EXE],company.dbname,[Select Top 1 Insp_id From qc_inspection_master where ;
			Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd],[tmptbl_vw],"This.Parent.nHandle",.DataSessionId,.F.)
			If Reccount('tmptbl_vw')>0
				Select tmptbl_vw
				Locate
				lnInsp_id=tmptbl_vw.insp_id
			Endif
			If lnInsp_id>0
				mSqlStr = _curvouobj.sqlconobj.GenDelete("qc_inspection_item","Insp_Id=?lnInsp_id")
				sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[],;
					"This.Parent.nHandle",_curvouobj.DataSessionId,.T.)

				mSqlStr = _curvouobj.sqlconobj.GenDelete("QC_INSPECTION_PARAMETER","Insp_Id=?lnInsp_id")
				sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[],;
					"This.Parent.nHandle",_curvouobj.DataSessionId,.T.)

***** Added by Sachin N. S. on 18/09/2018 for Bug-31756 -- Start
				mSqlStr = _curvouobj.sqlconobj.GenDelete("QcSampleValue","Insp_Id=?lnInsp_id")
				sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[],;
					"This.Parent.nHandle",_curvouobj.DataSessionId,.T.)
***** Added by Sachin N. S. on 18/09/2018 for Bug-31756 -- End
			Endif
&& Added by Shrikant S. on 29/09/2014 	for Bug-23879		&& End
			mSqlStr = _curvouobj.sqlconobj.GenDelete("qc_inspection_master","Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd")
			sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[],;
				"This.Parent.nHandle",_curvouobj.DataSessionId,.T.)
			mSqlStr = _curvouobj.sqlconobj.GenDelete("qchistory","Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd")
			sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[],;
				"This.Parent.nHandle",_curvouobj.DataSessionId,.T.)
			QcRetstate=.T.
		Endif
	Endif
Endif


&& Added By Shrikant S. on 29/12/2012 for Bug-2267 		&&Start 	&&vasant030412
If Type('_curvouobj.PcvType') = 'C'
	_mstkresrvtn = .F.
	_mstkresrvtn = oGlblPrdFeat.UdChkProd('stkresrvtn')
	If _mstkresrvtn = .T.
		If (lcode_vw.entry_ty ='SO' Or lcode_vw.bcode_nm ='SO')
			_mQty = 0
*!*				mSqlStr="Select Top 1 AllocQty From StkResrvSum Where Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd"	 && Commented By Shrikant S. on 08/12/2012 for Bug-2267
			mSqlStr="Select Top 1 a.AllocQty From StkResrvSum a  Inner Join StkResrvDet b on (a.Entry_ty=b.rEntry_ty and a.Tran_cd=b.Rtran_cd) "+;
				" Where a.Entry_ty = ?Main_vw.Entry_ty And a.Tran_cd = ?Main_vw.Tran_cd and b.allocqty>0 " 		&& Added By Shrikant S. on 08/12/2012 for Bug-2267
			sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[chktbl_vw],"This.Parent.nHandle",_curvouobj.DataSessionId,.F.)
			If sql_con > 0 And Used('chktbl_vw')
				Select chktbl_vw
				_mQty = Iif(Isnull(chktbl_vw.AllocQty)=.F.,chktbl_vw.AllocQty,0)
				Use In chktbl_vw
			Endif
			Select item_vw
			If _mQty > 0
				.ShowMessageBox("Stock Reservation done"+Chr(13)+"Entry Cannot be Deleted",0+32,vuMess)
				If !Empty(_Malias)
					Select &_Malias
				Endif
				If Betw(_mRecNo,1,Reccount())
					Go _mRecNo
				Endif
				Return .F.
			Endif
		Endif
		If (Inlist(lcode_vw.entry_ty,'WK','PO') Or Inlist(lcode_vw.bcode_nm,'WK','PO') Or lcode_vw.inv_stk = '+')
			_mQty = 0
			mSqlStr="Select SUM(AllocQty) as AllocQty From StkResrvDet Where Entry_ty = ?Main_vw.Entry_ty And Tran_cd = ?Main_vw.Tran_cd"
			sql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,mSqlStr,[chktbl_vw],"This.Parent.nHandle",_curvouobj.DataSessionId,.F.)
			If sql_con > 0 And Used('chktbl_vw')
				Select chktbl_vw
				_mQty = Iif(Isnull(chktbl_vw.AllocQty)=.F.,chktbl_vw.AllocQty,0)
				Use In chktbl_vw
			Endif
			Select item_vw
			If _mQty > 0
				.ShowMessageBox("Stock Reservation done"+Chr(13)+"Entry Cannot be Deleted",0+32,vuMess)
				If !Empty(_Malias)
					Select &_Malias
				Endif
				If Betw(_mRecNo,1,Reccount())
					Go _mRecNo
				Endif
				Return .F.
			Endif
		Endif
	Endif
Endif
&& Added By Shrikant S. on 29/12/2012 for Bug-2267 		&&End 	&&vasant030412

&& Added by Sachin N. S. on 30/10/2017 for Bug-30782 -- Start
*!*	If oGlblPrdFeat.UdChkProd('vugst')     &&Commented by Priyanka B on 19032019 for Bug-32067
If oGlblPrdFeat.UdChkProd('vugst') Or oGlblPrdFeat.UdChkProd('vuisd') OR oGlblPrdFeat.UdChkProd('isdkgen')  &&Modified by Priyanka B on 19032019 for Bug-32067
	cdAmendDt = Iif(Type('Main_vw.AmendDate')='T','Main_vw.AmendDate',Iif(Type('Lmc_vw.AmendDate')='T','Lmc_vw.AmendDate',Iif(Type('MainAdd_vw.AmendDate')='T','MainAdd_vw.AmendDate','')))
	If !Empty(cdAmendDt)
		If !Empty(Evaluate(cdAmendDt)) And Evaluate(cdAmendDt)!=Ctod('01/01/1900')
			=Messagebox("This record is already Amended cannot delete...!!!",0+16,vuMess)
			Return .F.
		Endif
	Endif
Endif
&& Added by Sachin N. S. on 30/10/2017 for Bug-30782 -- End

&&Added by Priyanka B on 25082018 for Bug-31756 Start
_curscrobj=_curvouobj
If (_curscrobj.addmode = .F. And _curscrobj.editmode = .F. And Inlist(main_vw.entry_ty,"BP","RV","CO","GS"))
	vChqErrMsg = ""
	If !Empty(main_vw.cheq_no)
		msql_str1="Select top 1 * From (Select cd.Chq_No,BCode_Nm=isnull(l.BCode_Nm,''),Inv_No=isnull(m.Inv_No,''),Status,Iss_Entry_Ty,Iss_Tran_cd From Cheque_Details cd"
		msql_str2=" inner join ChequeBookMaster cb on (cb.Book_Id=cd.Book_Id) Left join Lcode l on (l.Entry_Ty=cd.Iss_Entry_Ty ) Left Join BpMain m on (cd.Iss_Tran_Cd=m.Tran_cd)"
		msql_str3=" Where cb.Bank_Nm='"+Alltrim(main_vw.Bank_nm)+"' and cd.Chq_No='"+Alltrim(main_vw.cheq_no)+"' and Iss_Entry_Ty='"+main_vw.entry_ty+"' and Iss_Tran_cd="+Alltrim(Str(main_vw.tran_cd))
		msql_str3= msql_str3 + " Union Select cd.Chq_No,BCode_Nm=isnull(l.BCode_Nm,''),Inv_No=isnull(m.Inv_No,''),Status,Iss_Entry_Ty,Iss_Tran_cd From Cheque_Details cd"+;
			" inner join ChequeBookMaster cb on (cb.Book_Id=cd.Book_Id) Left join Lcode l on (l.Entry_Ty=cd.Iss_Entry_Ty ) Left Join BrMain m on (cd.Iss_Tran_Cd=m.Tran_cd) "+;
			" Where cb.Bank_Nm='"+Alltrim(main_vw.Bank_nm)+"' and cd.Chq_No='"+Alltrim(main_vw.cheq_no)+"' and Iss_Entry_Ty='"+main_vw.entry_ty+;
			"' and Iss_Tran_cd="+Alltrim(Str(main_vw.tran_cd))+")aa where inv_no<>'' "
		etsql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,msql_str1+msql_str2+msql_str3,"_delchkChqNo","_curvouobj.nHandle",_curvouobj.DataSessionId,.T.)

		If etsql_con > 0 And Used("_delchkChqNo")
			Select _delchkChqNo
			If Reccount("_delchkChqNo")>0
				Select _delchkChqNo
				If Alltrim(Upper(main_vw.party_nm)) = "CANCELLED."
					vChqErrMsg=	"Cheque No. " + Alltrim(_delchkChqNo.Chq_No) + " is Dishonor/Cancelled!!"
				Endif
				If (_delchkChqNo.Status="Reconciled" And (_delchkChqNo.Iss_Entry_Ty=main_vw.entry_ty And _delchkChqNo.Iss_Tran_cd =main_vw.tran_cd))
					If !Empty(vChqErrMsg)
						vChqErrMsg=	vChqErrMsg + Chr(13) + "And also Reconciled!!"
					Else
						vChqErrMsg=	"Cheque No. " + Alltrim(_delchkChqNo.Chq_No) + " is Reconciled!!"
					Endif
				Endif
				If !Empty(vChqErrMsg)
					vChqErrMsg = vChqErrMsg + Chr(13)+"Cannot delete this entry!!"
				Endif

				Use In _delchkChqNo
				If !Empty(vChqErrMsg)
					=Messagebox(vChqErrMsg,16,vuMess)
					Return .F.
				Else
					msql_str1= "Update cd set Status='Available',Iss_Entry_Ty='',Iss_Tran_cd='0' from Cheque_Details cd"
					msql_str2= " Left Join Lcode l on (l.Entry_Ty=cd.Iss_Entry_Ty ) Left Join "+ Alltrim(lcode_vw.bcode_nm)+"Main m on (cd.Iss_Tran_Cd=m.Tran_cd)"
					msql_str3= " Where cd.Bank_Nm='"+Alltrim(main_vw.Bank_nm)+"' and Iss_Entry_Ty='"+main_vw.entry_ty+"' and Iss_Tran_cd="+Alltrim(Str(main_vw.tran_cd))+" and cd.Status<>'Reconciled'"
					etsql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,msql_str1+msql_str2+msql_str3,[_delChqNo],"_curvouobj.nHandle",_curvouobj.DataSessionId,.T.)

					If etsql_con > 0 And Used('_delChqNo')
						Use In _delChqNo
					Endif
				Endif
			Endif
		Endif
	Endif
Endif
&&Added by Priyanka B on 25082018 for Bug-31756 End

&&Added by Priyanka B on 12122018 for Bug-31930 Start
_curscrobj=_curvouobj
*!*	If (_curscrobj.addmode = .F. And _curscrobj.editmode = .F. And Inlist(main_vw.entry_ty,"HS"))
If (_curscrobj.addmode = .F. And _curscrobj.editmode = .F. And Inlist(main_vw.entry_ty,"HS","PS"))		&& Changed by Sachin N. S. on 22/01/2019 for Bug-32215
	msql_str=""
	msql_str= "Select ISNULL(POSOUTTRAN,0) as POSOUTTRAN from dcmain where entry_ty=?main_vw.entry_ty and tran_cd=?main_vw.tran_cd"
	etsql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,msql_str,[_delPpos],"_curvouobj.nHandle",_curvouobj.DataSessionId,.T.)
	Select _delPpos
	If Reccount()>0
		If _delPpos.posouttran > 0
			Messagebox("Cash out entry is passed against this transaction."+Chr(13)+"Cannot delete!!",0+16,vuMess)
			Return .F.
		Endif
	Endif

	If Used('_delPpos')
		Use In _delPpos
	Endif
Endif
&&Added by Priyanka B on 12122018 for Bug-31930 End


&&Added by Priyanka B on 28012019 for Bug-32210 Start
_curscrobj=_curvouobj
If (_curscrobj.addmode = .F. And _curscrobj.editmode = .F. And Inlist(main_vw.entry_ty,"Q1","S2"))
	msql_str=""
	msql_str= "Select rtrim(code_nm) as tran_name From Lcode Where entry_ty=(Select top 1 rentry_ty From Autotranref Where entry_ty=?main_vw.entry_ty and tran_cd=?main_vw.tran_cd)"
	etsql_con = _curvouobj.sqlconobj.Dataconn([EXE],company.dbname,msql_str,[_delautotran],"_curvouobj.nHandle",_curvouobj.DataSessionId,.T.)
	Select _delautotran
	If Reccount()>0
		Messagebox(Alltrim(_delautotran.tran_name) + " entry is passed against this transaction."+Chr(13)+"Cannot delete!!",0+16,vuMess)
		Return .F.
	Endif

	If Used('_delautotran')
		Use In _delautotran
	Endif
Endif

If ! _curscrobj.addmode And ! _curscrobj.editmode And Inlist(main_vw.entry_ty,"PQ","PO")
	etsql_str="Delete From AutoTranRef Where REntry_ty=?Main_vw.Entry_ty and RTran_cd=?Main_vw.Tran_cd "
	etsql_con  = _curscrobj.sqlconobj.Dataconn([EXE],company.dbname,etsql_str,"","_curscrobj.nHandle",_curscrobj.DataSessionId,.T.)
Endif
&&Added by Priyanka B on 28012019 for Bug-32210 End

If !Empty(_Malias)
	Select &_Malias
Endif
If Betw(_mRecNo,1,Reccount())
	Go _mRecNo
Endif
Return	QcRetstate

&&Amrendra for Bug-6496 on 27/09/2012 <----
