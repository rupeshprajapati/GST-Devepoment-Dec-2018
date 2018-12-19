Proc tmp_crea
*************
	Para _dbf
	ctr = 1
	Do Whil .T.
		r_temp = Sys(3) + '.' + _dbf
		ctr = ctr + 1
		If File(dix_temp)
			Loop
		Endif
		Exit
	Enddo
	Retu r_temp

Proc enc
********
	Para mcheck
	d=1
	F=Len(mcheck)
	Repl=""
	rep=0
	Do Whil F > 0
		r=Subs(mcheck,d,1)
		Change = Asc(r)+rep
		If Change>255
			Wait Wind Str(Change)
		Endi
		two = Chr(Change)
		Repl=Repl+two
		d=d+01
		rep=rep+1
		F=F-1
Endd
Retu Repl

Proc dec
********
	Para mcheck
	d=1
	F=Len(mcheck)
	Repl=""
	rep=0
	Do Whil F > 0
		r=Subs(mcheck,d,1)
		Change = Asc(r)-rep
		If Change>0
			two = Chr(Change)
		Endi
		Repl=Repl+two
		d=d+01
		F=F-1
		rep=rep+1
Endd
Retu Repl

Proc chkprod
************
	Buffer=[]
****** Added By Sachin N. S. on 23/04/2011 for TKT-2386 ****** Start
	cpasroute = Allt(company.passroute)+Iif(Type('company.passroute1')!='U',Allt(company.passroute1),"")
*!*		If !Empty(Allt(company.passroute))
*!*			For x = 1 To Len(Allt(company.passroute))
*!*				Buffer = Buffer + Chr(Asc(Substr(company.passroute,x,1))/2)
*!*			Next x
*!*		Endif
	If !Empty(Allt(cpasroute))
		For x = 1 To Len(cpasroute)
			Buffer = Buffer + Chr(Asc(Substr(cpasroute,x,1))/2)
		Next x
	Endif
****** Added By Sachin N. S. on 23/04/2011 for TKT-2386 ****** End
	vchkprod=Buffer
	Retur

Procedure modalmenu
*****************
	Local oldpanel
	oldpanel = statdesktop.panels(2).Text
	statdesktop.panels(2).Text = 'Generating Menus....'
	For i = 1 To _Screen.FormCount
		If Left(Upper(Alltrim(_Screen.Forms[i].Name)),5) = 'UDYOG'
			Do gware.mpr With _Screen.Forms[i],.T.
			Exit
		Endif
	Next i
	statdesktop.panels(2).Text = oldpanel
	Return

Function  busymsg
*****************
	Lparameters pmsg,pbusyicon,pbusyform,poldpanel
	If Type('pMsg') = 'L' And Type('pBusyIcon') = 'L' And Type('pBusyform') = 'L'
		If pmsg = .F. And pbusyicon = .F. And pbusyform = .F.
			If Type('pOldpanel') = 'U' Or Type('pOldpanel') = 'L'
				poldpanel = ''
			Endif
			_Screen.ActiveForm.MousePointer = 0
			If !Empty(poldpanel)
				statdesktop.Message	= poldpanel
			Else
				statdesktop.Message	= ''
			Endif
			For i=1 To _Screen.FormCount
				If Alltrim(Upper(_Screen.Forms(i).Name)) = 'THINKPROCESS'
					_Screen.Forms(i).cexit()
					Exit
				Endif
			Endfor
		Endif
	Else
		oldpanel = statdesktop.Message
		oldmousepoint = _Screen.ActiveForm.MousePointer
		_Screen.ActiveForm.MousePointer = 13
		statdesktop.Message = pmsg
		If pbusyform = .T.
			Do Form thinkprocess With pmsg
		Endif
	Endif
	Return

Procedure onencrypt
*****************
	Lpara lcvariable
	lcreturn = ""
	For i=1 To Len(lcvariable)
		lcreturn=lcreturn+Chr(Asc(Substr(lcvariable,i,1))+Asc(Substr(lcvariable,i,1)))
	Endfor
	Return lcreturn

Procedure ondecrypt
*****************
	Lpara lcvariable
	lcreturn = ""
	For i=1 To Len(lcvariable)
		lcreturn=lcreturn+Chr(Asc(Substr(lcvariable,i,1))/2)
	Endfor
	Return lcreturn

Function CheckRegDll
*****************
	Parameters lOle
	Local RegFind
	RegOle = .T.
	oldErrProc = On('error')
	On Error Do DLLErrorProc In vu_udfs
	Declare Long DllRegisterServer In (lOle) Alias chkDll
	Clear Dlls chkDll
	If Type('oldErrProc') = 'C'
		If !Empty(oldErrProc)
			On Error &oldErrProc
		Else
			On Error
		Endif
	Else
		On Error
	Endif
	Return RegOle


Procedure DLLErrorProc
*****************
	lerrorno =Error()
	RegOle = .T.
	Do Case
		Case lerrorno = 1753
			RegOle = .F.
	Endcase
	Return RegOle

Procedure onshutdown
*****************
	On Shutdown
	Clear Events
	Quit
	Return

Procedure pctrlf4
*****************
&&Added by Amrendra for Usqare10 installer Bug-2286 on 22-02-2012 code given by Vasant
	_etdatasessionid = _Screen.ActiveForm.DataSessionId
	Set DataSession To _etdatasessionid
&&Added by Amrendra for Usqare10 installer Bug-2286 on 22-02-2012 code given by Vasant
	Local llRet
	For i = 1 To _Screen.FormCount
		If Allt(Upper(_Screen.Forms[i].BaseClass)) != "TOOLBAR" And Left(Upper(Alltrim(_Screen.Forms[i].Name)),5) != 'UDYOG';
				AND Upper(Alltrim(_Screen.Forms[i].Name)) != 'FRMLOGINUSERS' And Upper(Alltrim(_Screen.Forms[i].Name)) != 'FRMMSGWINDOW'
			statdesktop.Message = [Busy Mode....]
			=beep(600,200)
			statdesktop.Message = [User :]+musername
			=Messagebox("The "+Iif(!Empty(_Screen.Forms[i].Caption),Alltrim(_Screen.Forms[i].Caption),Alltrim(_Screen.Forms[i].Name))+" Form is Open."+Chr(13)+"Close the form and then exit.",0+64,vuMess)
			Return .F.
		Endif
	Next i
&&--->TKT-6529 Rup 29/03/11
	msqlstr="Delete from vudyog..ExtApplLog where calldate+7<getdate()"
	nretval=_Screen.ActiveForm.sqlconobj.dataconn('EXE',company.dbname,msqlstr,"_rExtAppTbl","nHandle1",_Screen.ActiveForm.DataSessionId)
	If nretval<0
		Return .F.
	Endif

	msqlstr="select cApplNm,cApplId,cApplDesc from vudyog..ExtApplLog where pApplCode='"+pApplCode+"'"
	nretval=_Screen.ActiveForm.sqlconobj.dataconn('EXE',company.dbname,msqlstr,"_rExtAppTbl","nHandle1",_Screen.ActiveForm.DataSessionId)
	If nretval<0
		Return .F.
	Endif
	nretval=_Screen.ActiveForm.sqlconobj.sqlconnclose("nHandle1")
	oWMI = Getobject('winmgmts://')
	If Used("_rExtAppTbl")
		Select _rExtAppTbl
		Go Top
		Locate
		Scan
			cQuery = "select * from win32_process where ProcessId="+Alltrim(Str(_rExtAppTbl.cApplId))+" and Name='"+Alltrim(_rExtAppTbl.cApplNm)+"'"
			oResult = oWMI.ExecQuery(cQuery)
			If oResult.Count>0
				statdesktop.Message = [Busy Mode....]
				=beep(600,200)
				statdesktop.Message = [User :]+musername
				=Messagebox("The "+Alltrim(_rExtAppTbl.cApplDesc)+" Application is Open."+Chr(13)+"Close the Application and then exit.",0+64,vuMess)
				exitClick = .F.
				Return .F.
			Endif
		Endscan
	Endif
&&<---TKT-6529 Rup 29/03/11

	llRet=_Screen.ActiveForm.procre(.T.)
	If !llRet
		exitClick = .F.
		Return .F.
	Endif
	Return

Procedure pctrll
*****************
&&Added by Amrendra for Usqare10 installer Bug-2286 on 22-02-2012 code given by Vasant
	_etdatasessionid = _Screen.ActiveForm.DataSessionId
	Set DataSession To _etdatasessionid
&&Added by Amrendra for Usqare10 installer Bug-2286 on 22-02-2012 code given by Vasant
	Local llRet
	On Key Label Alt+l
	For i = 1 To _Screen.FormCount
		If Allt(Upper(_Screen.Forms[i].BaseClass)) != "TOOLBAR" And Left(Upper(Alltrim(_Screen.Forms[i].Name)),5) != 'UDYOG';
				AND Upper(Alltrim(_Screen.Forms[i].Name)) != 'FRMLOGINUSERS' And Upper(Alltrim(_Screen.Forms[i].Name)) != 'FRMMSGWINDOW'
			statdesktop.Message = [Busy Mode....]
			=beep(600,200)
			statdesktop.Message = [User :]+musername
			=Messagebox("The "+Iif(!Empty(_Screen.Forms[i].Caption),Alltrim(_Screen.Forms[i].Caption),Alltrim(_Screen.Forms[i].Name))+" Form is Open."+Chr(13)+"Close the form and then exit.",0+64,vuMess)
			Return .F.
		Endif
	Next i
&&--->TKT-6529 Rup 29/03/11
	msqlstr="Delete from vudyog..ExtApplLog where calldate+7<getdate()"
	nretval=_Screen.ActiveForm.sqlconobj.dataconn('EXE',company.dbname,msqlstr,"_rExtAppTbl","nHandle1",_Screen.ActiveForm.DataSessionId)
	If nretval<0
		Return .F.
	Endif

	msqlstr="select cApplNm,cApplId,cApplDesc from vudyog..ExtApplLog where pApplCode='"+pApplCode+"'"
	nretval=_Screen.ActiveForm.sqlconobj.dataconn('EXE',company.dbname,msqlstr,"_rExtAppTbl","nHandle1",_Screen.ActiveForm.DataSessionId)
	If nretval<0
		Return .F.
	Endif

	nretval=_Screen.ActiveForm.sqlconobj.sqlconnclose("nHandle1")
	oWMI = Getobject('winmgmts://')
	If Used("_rExtAppTbl")
		Select _rExtAppTbl
		Go Top
		Locate
		Scan
			cQuery = "select * from win32_process where ProcessId="+Alltrim(Str(_rExtAppTbl.cApplId))+" and Name='"+Alltrim(_rExtAppTbl.cApplNm)+"'"
			oResult = oWMI.ExecQuery(cQuery)
			If oResult.Count>0
				=Messagebox("The "+Alltrim(_rExtAppTbl.cApplDesc)+" Application is Open."+Chr(13)+"Close the Application and then exit.",0+64,vuMess)
				Return .F.
			Endif
		Endscan
	Endif
&&<---TKT-6529 Rup 29/03/11

	llRet=_Screen.ActiveForm.procre(.F.)
	If !llRet
		exitClick = .F.
		Return .F.
	Endif

******* Added By Sachin N. S. on 29/08/2009 ******* Start
	AppSessionId = 1
******* Added By Sachin N. S. on 29/08/2009 ******* End

	Return

Procedure decoder
*****************
	Parameters thispass,passflag
	Store "" To finecode,mycoder
	For i = 1 To Len(thispass)
		If !passflag
			mycoder = Chr(Asc(Substr(thispass,i,1))-4) &&+7-11)
		Else
			mycoder = Chr(Asc(Substr(thispass,i,1))+4) &&-7+11)
		Endif (!passflag)
		finecode = finecode+mycoder
	Next (i)
	Return finecode

*All uecon.exe has been changed to uecon.uxe &&Changes done by vasant on 28/01/2012 as per Bug 1760 - The Application is Opeing Default New Company Master page even when Companies are already Created.
&&vasant16/11/2010	Changes done for VU 10 (Standard/Professional/Enterprise)
Procedure NewENCRY
*vfpencryption.fll has been renamed to uecon.exe
	Lparameters _NewEncryValue,_NewEncryPass
	Local lcVFPEncryptionFile,_FileValidErr,_EncRetValue
	_EncRetValue = '*1*'
	_FileValidErr = ''
	_muecon = "uecon.uxe"
	Try
		If !(Upper(_muecon) $ Upper(Set("Library"))) &&Changes has been done as per TKT-6470 (Multilanguage support - Tested with English & Japanese Language) on 24/02/2011
			lcVFPEncryptionFile = Filetostr(_muecon)
			Set Library To &_muecon AddI
			If Len(lcVFPEncryptionFile) = 122880 ;
					and Strconv(Hash(lcVFPEncryptionFile,5),15) == '20A7D4AF02E62A362CE44FCBAB6EB5FE';
					and Strconv(Hash(lcVFPEncryptionFile,4),15) == 'DBD1EC320842ED0DB1A4DAE26F0513E4DF1F9D1C65FB93D896E553940ACFA408479EDFBE78FCEA9901915384E5FBC1C50CCD00709C0CD870CA8A4C8BC40676EF'
			Else
				_FileValidErr = 'Error'
			Endif
		Endif &&Changes has been done as per TKT-6470 (Multilanguage support - Tested with English & Japanese Language) on 24/02/2011
		If Empty(_FileValidErr)
			_EncRetValue = Encrypt(_NewEncryValue,_NewEncryPass,1024)
*Release Library uecon.exe	&&Changes has been done as per TKT-6470 (Multilanguage support - Tested with English & Japanese Language) on 24/02/2011
		Endif
	Catch To _FileValidErr
		_EncRetValue = '*1*'
	Endtry
	Return _EncRetValue

Procedure NewDECRY
*vfpencryption.fll has been renamed to uecon.exe
	Lparameters _NewDecryValue,_NewDecryPass
	Local lcVFPEncryptionFile,_FileValidErr,_DecRetValue
	_DecRetValue = '+1+'
	_FileValidErr = ''
	_muecon = "uecon.uxe"
	Try
		If !(Upper(_muecon) $ Upper(Set("Library"))) &&Changes has been done as per TKT-6470 (Multilanguage support - Tested with English & Japanese Language) on 24/02/2011
			lcVFPEncryptionFile = Filetostr(_muecon)
			Set Library To &_muecon AddI
			If Len(lcVFPEncryptionFile) = 122880 ;
					and Strconv(Hash(lcVFPEncryptionFile,5),15) == '20A7D4AF02E62A362CE44FCBAB6EB5FE';
					and Strconv(Hash(lcVFPEncryptionFile,4),15) == 'DBD1EC320842ED0DB1A4DAE26F0513E4DF1F9D1C65FB93D896E553940ACFA408479EDFBE78FCEA9901915384E5FBC1C50CCD00709C0CD870CA8A4C8BC40676EF'
			Else
				_FileValidErr = 'Error'
			Endif
		Endif &&Changes has been done as per TKT-6470 (Multilanguage support - Tested with English & Japanese Language) on 24/02/2011
		If Empty(_FileValidErr)
			_DecRetValue = DECRYPT(_NewDecryValue,_NewDecryPass,1024)
*Release Library uecon.exe		&&Changes has been done as per TKT-6470 (Multilanguage support - Tested with English & Japanese Language) on 24/02/2011
		Endif
	Catch To _FileValidErr
		_DecRetValue = '+2+'
	Endtry
	Return _DecRetValue

Procedure AppVerChk
	Lparameters _AppName,_AppVerName,_CurrAppl
	Local _AppRetVal,_FileValidErr
	_AppRetVal = 'NO' &&.f.
	_FileValidErr = ''
	If Type('_AppName') = 'C' And Type('_AppVerName') = 'C'
		_OldAppVerChkAlias = Alias()
		Try
			Select 0
			Select Version From AppVerInfo Where Alltrim(Upper(AppName)) == Alltrim(Upper(_AppName)) Into Cursor _TmpAppVerInfo_vw
			If Used('_TmpAppVerInfo_vw')
				Select _TmpAppVerInfo_vw
				If Reccount() > 0 And Alltrim(Upper(_TmpAppVerInfo_vw.Version)) == Alltrim(Upper(_AppVerName))
*			_AppRetVal = .t. &&Commented By Amrendra for TKT - 8121 on 19/05/2011
*			Added By Amrendra for TKT - 8121 on 19/05/2011 Start
					_AppRetVal = 'YES'
				Else
					cMsgStr="A Version Conflict found!"
					cMsgStr=cMsgStr+Chr(13)+Chr(13)+"Module Name  : " + _CurrAppl
					cMsgStr=cMsgStr+Chr(13)+"Required Version is : " + _TmpAppVerInfo_vw.Version
					cMsgStr=cMsgStr+Chr(13)+"Current Version is : " + _AppVerName
					Messagebox(cMsgStr,64,vuMess)
					_AppRetVal = 'NO'
*			Added By Amrendra for TKT - 8121 on 19/05/2011 End
				Endif
			Endif
		Catch To _FileValidErr
			_AppRetVal = 'NO'
			cMsgStr="Version Error occured!"
			Messagebox(cMsgStr,64,vuMess)
		Endtry

		If Used('AppVerInfo')
			Use In AppVerInfo
		Endif
		If Used('_TmpAppVerInfo_vw')
			Use In _TmpAppVerInfo_vw
		Endif
		If !Empty(_OldAppVerChkAlias)
			Select (_OldAppVerChkAlias)
		Endif
	Endif
	Return _AppRetVal

&&---> :The below procedure Fetch Current Version No on if we give AppFileName: Amrendra for TKT 8543
Procedure AppVerGet
	Lparameters _AppName
	Local _AppRetVal,_FileValidErr
	_AppRetVal = 'No Version'
	_FileValidErr = ''
	If Type('_AppName') = 'C'
		_OldAppVerChkAlias = Alias()
		Try
			Select 0
			Select Version From AppVerInfo Where Alltrim(Upper(FileName)) == Alltrim(Upper(_AppName)) Into Cursor _TmpAppVInfo_vw
			If Used('_TmpAppVInfo_vw')
				Select _TmpAppVInfo_vw
				If Reccount() > 0
					_AppRetVal = _TmpAppVInfo_vw.Version
				Else
					_AppRetVal = 'No Version'
				Endif
			Endif
		Catch To _FileValidErr
			_AppRetVal = 'No Version'
			cMsgStr="Version Error occured!"
			Messagebox(cMsgStr,64,vuMess)
		Endtry

		If Used('AppVerInfo')
			Use In AppVerInfo
		Endif
		If Used('_TmpAppVInfo_vw')
			Use In _TmpAppVInfo_vw
		Endif
		If !Empty(_OldAppVerChkAlias)
			Select (_OldAppVerChkAlias)
		Endif
	Endif
	Return _AppRetVal
&&<--- :The below procedure Fetch Current Version No on if we give AppFileName: Amrendra for TKT 8543

Procedure ReadInfFile

	Select 0
&&Changes done by Vasant on 12/10/12 as per Bug 5400 - (The Application is not checking the .inf file like it is checking the Register.me)
*Create Cursor custinfo_vw (clientnm Varchar(100),macid Varchar(50),prodnm Varchar(50),prodcd Varchar(250),zip Varchar(100),featureid Memo,vatstates Memo, AddProdCd Memo)	&&Changes done by Vasant on 02/02/12 as per Bug 1932 - The application is not checking the inf file properly when all the possible combination are selected while Creating a company.
	Create Cursor custinfo_vw (clientnm Varchar(100),macid Varchar(50),prodnm Varchar(50),MainProdCd Varchar(50),prodcd Varchar(250),zip Varchar(100),featureid Memo,vatstates Memo, AddProdCd Memo)
&&Changes done by Vasant on 12/10/12 as per Bug 5400 - (The Application is not checking the .inf file like it is checking the Register.me)

	Select 0
	Create Cursor custinfofile_vw (clientdet m,dec_cldet m)

	custinfosrno = 1
	Do While custinfosrno <= 2
		custinfofilenm = ''
		Do Case
			Case custinfosrno = 1
				custinfofilenm = apath
			Case custinfosrno = 2
				custinfofilenm = apath+'Database\'
		Endcase
		vufile	= Sys(2000,custinfofilenm+"*info.inf")
		nFile	= Adir(arrFile,custinfofilenm+vufile)
		_lens = 1
		For _lens = 1 To nFile
			Select custinfofile_vw
			Append Blank In custinfofile_vw
			Replace clientdet With Filetostr(custinfofilenm+arrFile(_lens,1)) In custinfofile_vw
		Endfor
		custinfosrno = custinfosrno + 1
	Enddo

	Select custinfofile_vw
	Scan

		nFile	 = Len(custinfofile_vw.clientdet)
		_partynm = Alltrim(Substr(custinfofile_vw.clientdet,1,50))
		Replace dec_cldet With _partynm In custinfofile_vw

		_lens = 1
		For _lens = 51 To nFile Step 50
			_mname = NewDECRY(Substr(custinfofile_vw.clientdet,_lens,50),_partynm)
			Replace dec_cldet With custinfofile_vw.dec_cldet +  _mname In custinfofile_vw
		Endfor

		_partyz = custinfofile_vw.dec_cldet
		Do While .T.
			_party1 = At('<<~0s>>',_partyz)
			_party2 = At('<<e0~>>',_partyz)
			If _party1 > 0 And _party2 > 0
				_partya = Substr(_partyz,_party1,_party2-_party1)
				_macid  = ''
				Select custinfo_vw
				Append Blank In custinfo_vw
				Do While .T.
					_party3 = At('<<~1s>>',_partya)
					_party4 = At('<<e1~>>',_partya)
					If _party3 > 0 And _party4 > 0
						_partyd = Substr(_partya,_party3,_party4-_party3)
						Do Case
							Case Substr(_partyd,8,2) = 'CN'
								Replace clientnm With dec(NewDECRY(Substr(_partyd,11),_partynm)) In custinfo_vw
							Case Substr(_partyd,8,2) = 'MI'
								Replace macid With dec(NewDECRY(Substr(_partyd,11),_partynm)) In custinfo_vw
								_macid = Alltrim(custinfo_vw.macid)
							Case Substr(_partyd,8,2) = 'PV'
								Replace prodnm With dec(NewDECRY(Substr(_partyd,11),_macid)) In custinfo_vw
&&Changes done by Vasant on 12/10/12 as per Bug 5400 - (The Application is not checking the .inf file like it is checking the Register.me)
							Case Substr(_partyd,8,2) = 'MP'
								Replace MainProdCd With NewDECRY(Substr(_partyd,11),_macid) In custinfo_vw
&&Changes done by Vasant on 12/10/12 as per Bug 5400 - (The Application is not checking the .inf file like it is checking the Register.me)
							Case Substr(_partyd,8,2) = 'PC'
*!*							REPLACE prodcd WITH NewDECRY(SUBSTR(_partyd,11),_macid) IN custinfo_vw
****** Changed By Sachin N. S. on 21/04/2011 for TKT-6920 and TKT-2386 ****** Start
								_cPrdCode1=''
								_cPrdCode2=''
								_cPrdCode3=''
								_cPrdCode = Alltrim(NewDECRY(Substr(_partyd,11),_macid))+','
								Do While .T.
									_cPrdCode1 = Left(_cPrdCode,At(',',_cPrdCode)-1)

*!*								If Inlist(_cPrdCode1,"vuent" ,"vupro","vuexc","vuexp","vuinv","vuord","vubil","vutex","vuser","vuisd","vumcu","vutds")
									If Inlist(_cPrdCode1,"vuent" ,"vupro","vuexc","vuexp","vuinv","vuord","vubil","vutex","vuser","vuisd","vumcu","vutds","vugst")		&& Changed by Sachin N. S. on 30/09/2016 for GST
										_cPrdCode2 = _cPrdCode2 + Iif(Empty(_cPrdCode2),"",",") + _cPrdCode1
									Else
										_cPrdCode3 = _cPrdCode3 + Iif(Empty(_cPrdCode3),"",",") + _cPrdCode1
									Endif
&&Changes has been done by vasant on 28/01/2013 as per Bug-6094 ("Kindly Subscribe" Message coming when opening some of the Registered Modules.)
*_cPrdCode = Strtran(_cPrdCode,_cPrdCode1+',','')
									_cPrdCode = Subs(_cPrdCode,At(',',_cPrdCode)+1)
&&Changes has been done by vasant on 28/01/2013 as per Bug-6094 ("Kindly Subscribe" Message coming when opening some of the Registered Modules.)

									If Empty(_cPrdCode)
										Exit
									Endif
								Enddo
								Replace prodcd With _cPrdCode2, AddProdCd With _cPrdCode3 In custinfo_vw

****** Changed By Sachin N. S. on 21/04/2011 for TKT-6920 and TKT-2386 ****** End
							Case Substr(_partyd,8,2) = 'ZP'
								Replace zip With NewDECRY(Substr(_partyd,11),_macid) In custinfo_vw
							Case Substr(_partyd,8,2) = 'FI'
								Replace featureid With NewDECRY(Substr(_partyd,11),_partynm+_macid) In custinfo_vw
							Case Substr(_partyd,8,2) = 'VS'
								Replace vatstates With NewDECRY(Substr(_partyd,11),_macid) In custinfo_vw
						Endcase

						_partya = Substr(_partya,_party4+7)
					Else
						_partya = ''
					Endif

					If Empty(_partya)
						Exit
					Endif
				Enddo
				_partyz = Substr(_partyz,_party2+7)
			Else
				_partyz = ''
			Endif

			If Empty(_partyz)
				Exit
			Endif
		Enddo

		Select custinfofile_vw
	Endscan

	If Used('custinfofile_vw')
		Use In custinfofile_vw
	Endif
	Select custinfo_vw
	Return .T.
&&vasant16/11/2010	Changes done for VU 10 (Standard/Professional/Enterprise)

&&Changes done by Vasant on 30/04/2013 as per Bug 7303(Barcode Printing Details).
Function Func_EAN13(tcString,tlCheckD)
	Local lcLat, lcMed, lcRet, lcJuego, ;
		lcIni, lcResto, lcCod, lnI, ;
		lnCheckSum, lnAux, laJuego(10), lnPri
	lcRet = Alltrim(tcString)
	If Len(lcRet) # 12
		Return ''
	Endif
	lnCheckSum = 0
	For lnI = 1 To 12
		If Mod(lnI,2) = 0
			lnCheckSum = lnCheckSum + Val(Subs(lcRet,lnI,1)) * 3
		Else
			lnCheckSum = lnCheckSum + Val(Subs(lcRet,lnI,1)) * 1
		Endif
	Endfor
	lnAux = Mod(lnCheckSum,10)
	lcRet = lcRet + Alltrim(Str(Iif(lnAux = 0,0,10-lnAux)))
	If tlCheckD
		Return lcRet
	Endif
	lnPri = Val(Left(lcRet,1))
	laJuego(1) = 'AAAAAACCCCCC'
	laJuego(2) = 'AABABBCCCCCC'
	laJuego(3) = 'AABBABCCCCCC'
	laJuego(4) = 'AABBBACCCCCC'
	laJuego(5) = 'ABAABBCCCCCC'
	laJuego(6) = 'ABBAABCCCCCC'
	laJuego(7) = 'ABBBAACCCCCC'
	laJuego(8) = 'ABABABCCCCCC'
	laJuego(9) = 'ABABBACCCCCC'
	laJuego(10) = 'ABBABACCCCCC'
	lcIni = Chr(lnPri + 35)
	lcLat = Chr(33)
	lcMed = Chr(45)
	lcResto = Subs(lcRet,2,12)
	For lnI = 1 To 12
		lcJuego = Subs(laJuego(lnPri + 1),lnI,1)
		Do Case
			Case lcJuego = 'A'
				lcResto = Stuff(lcResto,lnI,1,Chr(Val(Subs(lcResto,lnI,1)) + 48))
			Case lcJuego = 'B'
				lcResto = Stuff(lcResto,lnI,1,Chr(Val(Subs(lcResto,lnI,1)) + 65))
			Case lcJuego = 'C'
				lcResto = Stuff(lcResto,lnI,1,Chr(Val(Subs(lcResto,lnI,1)) + 97))
		Endcase
	Endfor
	lcCod = lcIni + lcLat + Subs(lcResto,1,6) + lcMed + ;
		SUBS(lcResto,7,6) + lcLat
	Return lcCod
Endfunc

Function Func_CODE128		&&CODE128B
	Lparameters m.cadena
	If Pcount() = 0
		Return ""
	Endif
	If Type("m.cadena") = "N"
		m.cadena = Transform(m.cadena)
	Endif
	If Type("m.cadena") != "C"
		Return ""
	Endif
	m.cadena = Alltrim(m.cadena)
	Local m.vuelta, m.suma, m.caracterinicial, m.co, m.letra, m.valorascii, ;
		m.checksum, m.caracterfinal
	m.suma = 104
	m.caracterinicial = Chr(124)
	m.vuelta = m.caracterinicial
	m.caracterfinal = Chr(126)
	For m.co = 1 To Len(m.cadena)
		m.letra = Substr(m.cadena, m.co, 1)
		m.valorascii = Asc(m.letra)
		m.valorascii = m.valorascii - Iif(m.valorascii < 123, 32, 70)
		m.suma = m.suma + (m.valorascii * m.co)
		m.vuelta = m.vuelta + Iif(m.letra = " ", Chr(174), m.letra)
	Next m.co
	m.checksum = Max(Mod(m.suma, 103), 0)
	m.checksum = m.checksum + Iif(m.checksum > 90, 70, Iif(m.checksum = 0, 174, 32))
	m.checksum = Chr(m.checksum)
	m.vuelta = m.vuelta + m.checksum + m.caracterfinal
	Return m.vuelta
Endfunc
&&Changes done by Vasant on 30/04/2013 as per Bug 7303(Barcode Printing Details).

&&Changes has been done by Vasant on 17/10/2013 as per Bug 19622 (NITTO DENKO INDIA PRIVATE LIMITED (Maneswar) -Project ID).
Function Func_PDF417		&&PDF417
	Lparameters m.cadena
*m.cadena = '12345'
	m.vuelta = ''
	Try
		If Type('_BarCodeDll') <> 'O'
			Public _BarCodeDll
		Endif
		_BarCodeDll = Createobject("UdBarCode.ClsUdBarCode")
		If Type('_BarCodeDll') = 'O'
			m.vuelta = _BarCodeDll.CodePdf417(m.cadena,-1,CompanySetting.Bc_Col,0)
		Endif
	Catch To errObj
		=Messagebox(Alltrim(errObj.Message),0+64,vuMess)
	Endtry
	Return m.vuelta
Endfunc
&&Changes has been done by Vasant on 17/10/2013 as per Bug 19622 (NITTO DENKO INDIA PRIVATE LIMITED (Maneswar) -Project ID).


&& Added by Shrikant S. on 12/02/2018 for Bug-31185			&& Start
Function Func_QRCode		&& QR Code
	Lparameters m.cadena
*m.cadena = '12345'
	m.vuelta = ''
	Try
		If Type('_BarCodeDll') <> 'O'
			Public _BarCodeDll
		Endif
		_BarCodeDll = Createobject("Udyog.QRCode.Codec.QRCodeEncoder")

		If Type('_BarCodeDll') = 'O'
*!*		ENCODE_MODE ={ALPHA_NUMERIC=1,NUMERIC=1,BYTE=3}
*!*		ERROR_CORRECTION ={L=1, M=2, Q=3, H=4}
*!*		QRCodeVersion can be 7 to 40
*!*			QRCodeScale= start from 1
			QRCodeSize=4
			If Type('CompanySetting.Bc_Size')<>'U'
				QRCodeSize=Iif(CompanySetting.Bc_Size >0,CompanySetting.Bc_Size,QRCodeSize)
			Endif
			_BarCodeDll.QRCodeEncodeMode=2			&& Added by Shrikant S. on 01/12/2018 for Bug-32047
*!*				_BarCodeDll.QRCodeEncodeMode=3		&& Commented by Shrikant S. on 01/12/2018 for Bug-32047

			_BarCodeDll.QRCodeScale=QRCodeSize
			_BarCodeDll.QRCodeVersion = 7

			&& Added by Shrikant S. on 03/12/2018	for Bug-32047		&& Start	
			If Type('CompanySetting.BCVer')<>'U'
				_BarCodeDll.QRCodeVersion=Iif(CompanySetting.BCVer >0,CompanySetting.BCVer,_BarCodeDll.QRCodeVersion)
			ENDIF
			&& Added by Shrikant S. on 03/12/2018	for Bug-32047		&& End
						
*!*			 _BarCodeDll.QRCodeErrorCorrect=2  &&Commented by Shrikant S. on 31/10/2018
			_BarCodeDll.QRCodeErrorCorrect=1  &&Modified by Shrikant S. on 31/10/2018
			lcfileName= _BarCodeDll.UdyogEncodeBarcode(m.cadena)
*!*			m.vuelta ='0x'+TRANSFORM(STRCONV(FILETOSTR(lcfileName),15))
*!*			IF FILE(lcfileName)
*!*				DELETE FILE (lcfileName)
*!*			endif
			m.vuelta =lcfileName
		Endif
	Catch To errObj
		=Messagebox(Alltrim(errObj.Message),0+64,vuMess)
	Endtry
	Return m.vuelta
&& Added by Shrikant S. on 12/02/2018 for Bug-31185			&& End
