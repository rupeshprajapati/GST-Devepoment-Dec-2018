���   `� �� �                     ֪0   m                   PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Screen                                                                                              WINDOWS _1TZ0R4C2A 886465766      /  F      ]                          �      �                       WINDOWS _1TZ0R4C2B1312727043�      �  �      �      �  ~                  t}                           WINDOWS _1TK14D1YR 914512506h}      \}  O}  ;}  �|                                                           WINDOWS _1TZ0R4C2A1292932873�|      �|  �|  �|  �{      �i  �L                                               WINDOWS _1TL0MMD1W1285186528mi      Xi  Gi  3i  �h      Qh  �K                                               WINDOWS _21X0RQZ3N 914516061?h      -h   h  h  �g              �]  |]                                       COMMENT RESERVED                                2]                                                            �                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      dataenvironment      dataenvironment      Dataenvironment      YTop = 0
Left = 0
Width = 0
Height = 0
DataSource = .NULL.
Name = "Dataenvironment"
      1      2      form      form      frmUBIReport     �DataSession = 2
Top = 0
Left = 0
Height = 115
Width = 716
ShowWindow = 1
ScrollBars = 2
DoCreate = .T.
ShowTips = .T.
BorderStyle = 2
Caption = "Udyog MIS Reports"
MaxButton = .F.
MinButton = .F.
KeyPreview = .T.
WindowState = 0
left1 = 0
top1 = 0
left2 = 0
top2 = 0
qiqry = 
nhandle = 0
connstatus = 0
tagval = .F.
cmdobj = .F.
rptid = .F.
repttype = .F.
qry_id = 
para_nm = .F.
Name = "frmUBIReport"
     GPROCEDURE cmdclick
**Lparameters lparaid,lpnm



IF TYPE('mvu_nenc')='U'
	mvu_nenc=0
endif
IF mvu_nenc=1
	_m1Key_user=Thisform.sqlconobj.newdecry(strconv(mvu_user,16),"Ud*yog+1993Uid")
	_m1Key_pass=Thisform.sqlconobj.newdecry(STRCONV(mvu_pass,16),"Ud*yog+1993Pwd")
ELSE
	_m1Key_user=Thisform.sqlconobj.dec(thisform.sqlconobj.ondecrypt(mvu_user))
	_m1Key_pass=Thisform.sqlconobj.dec(thisform.sqlconobj.ondecrypt(mvu_pass))
ENDIF



constr = 'Driver={SQL server};server='+mvu_server+';database='+Alltrim(company.dbname)+';uid='+_m1Key_user+';pwd='+_m1Key_pass+';'
If !Empty(Thisform.qry_id)	
	nConnStatus = Thisform.connStatus
Endif
Thisform.connStatus =Sqlstringconnect(constr)
If Thisform.connStatus<= 0
	=Messagebox("Connection Error...!!!",0+64,vumess)
	Return .F.
Endif

Thisform.Panel.Panels(2).Text = 'Status : Please wait, Processing....!!!'
If Empty(Thisform.qry_id)	
	lpnm=This.tagval
Else
	lpnm=Thisform.para_nm
Endif


nRetval=SQLExec(Thisform.connStatus,'Select a.Sqlquery,b.searchflds from UBIQueryMast a INNER JOIN UBIParaMast b ON(a.QueryId=b.ParaQuery) where b.ParaName=?lpnm','_qryInfo')		
If nRetval <=0
	Return .F.
Else
	If Reccount('_qryInfo')=0
		=Messagebox("Help Query not found....!!!",64,vumess)
		Return .F.
	Endif
Endif

Select _qryInfo
QString= Upper(Alltrim(Sqlquery ))			

pnm=lpnm

nRetval=SQLDisconnect(Thisform.connStatus)
If nRetval<=0
	Return .F.
Endif
If !Empty(Thisform.qry_id)		
	Thisform.connStatus=nConnStatus
Endif

Thisform.nHandle=0
nRetval=Thisform.sqlconobj.dataconn("EXE",company.dbname,QString,"_help","thisform.nHandle",Thisform.DataSessionId)
If nRetval<=0
	Return .F.
Endif

nRetval=Thisform.sqlconobj.sqlconnclose('thisform.nHandle')
If nRetval<0
	=Messagebox("SQL disconnect error"+Chr(13)+Proper(Message()),48,vumess)
Endif

If !Empty(Thisform.qry_id)	
	Return .F.
Endif


Local ln,exword1,exword2,exword3,exword4,cparam
exword1=""
exword2=""
exword3=""
exword4=""
cparam=""

cparam=Upper(Alltrim(_qryInfo.searchflds))
ln=At('{',cparam)
If ln>0
	exword1 = Substr(cparam,At('{',cparam)+1,Iif(At('#',cparam)>0,At('#',cparam),At('}',cparam))-At('{',cparam)-1)
	exword2 = Iif(At('#',cparam)>0,Substr(cparam,At('#',cparam)+1,Iif(At('#',cparam,2)>0,At('#',cparam,2),At('}',cparam))-At('#',cparam)-1),exword1)
	exword3 = Iif(At('#',cparam,2)>0,Substr(cparam,At('#',cparam,2)+1,Iif(At('#',cparam,3)>0,At('#',cparam,3),At('}',cparam))-At('#',cparam,2)-1),"")
	exword4 = Iif(At('#',cparam,3)>0,Substr(cparam,At('#',cparam,3)+1,At('}',cparam)-At('#',cparam,3)-1),"")
Endif
If Empty(exword1)
	exword1  = Substr(QString,At("SELECT ",QString)+Len("SELECT "),At(" ",QString,2)-(At("SELECT ",QString)+Len("SELECT ")))
	exword2  = exword1
	If Upper(Alltrim(exword1)) = 'DISTINCT'
		exword1  = Substr(QString,At("SELECT DISTINCT ",QString)+Len("SELECT DISTINCT "),At(" ",QString,3)-(At("SELECT DISTINCT ",QString)+Len("SELECT DISTINCT ")))
		exword2 = exword1
	Endif
	If At('AS',QString)>0
		Select _help
		exword1= Alltrim(Field(1))
		exword2 = exword1
	Endif
Endif

If Reccount("_help") # 0
	Local ltxtobj,lresu,lcap
	Store '' To lcap
	If !Empty(This.Tag)
		lcap = 'thisform.'+Alltrim(This.Tag)+'.caption'
		lcap = &lcap
	Endif
	ltxtobj = "thisform."+Strtran(This.cmdobj,"cmd","txt")+".value"
	If Empty(exword4)
		exword5 = exword1
		Do While !Empty(exword5)
			exword6 = Iif(At(',',exword5)>0,Substr(exword5,1,At(',',exword5)-1),exword5)
			exword6 = Iif(At('.',exword6)>0,Substr(exword6,At('.',exword6)+1),exword6)
			exword5 = Iif(At(',',exword5)>0,Substr(exword5,At(',',exword5)+1),'')
			exword4 = exword4+Iif(!Empty(exword4),',','')+exword6+':'+exword6
		Enddo
	Endif
	lresu   = uegetpop('_Help','Select '+Alltrim(lcap)+'...',exword1,exword2,&ltxtobj,'','','',.F.,[],[],exword4,exword3)

	If Type('lresu')!='L'
		&ltxtobj = lresu
	Endif
Else
	=Messagebox("No Records found",64,vumess)
	Return .F.
Endif

Thisform.Panel.Panels(2).Text = 'Status :'




ENDPROC
PROCEDURE genparameters
Lparameters lparaid

Local _lPara,_lwidth

If !Empty(lparaid)
	msqlStr = 'select Sqlquery From UBIQueryMast Where ReportId= ?lparaid'
	nretval=SQLExec(Thisform.connstatus,msqlStr,'MX5')
	If nretval < 0
		Return .F.
	Endif
	lcsqlquery=MX5.SQLQuery
	_lnparacount=Occurs("{@",Alltrim(lcsqlquery))
	lcquery=""
	If _lnparacount >0
		lcquery=" Where ParaName In ("
		i=1
		Do While i<=_lnparacount
			pid = Strextract(lcsqlquery,"{@","}",i)
			lcquery=lcquery+"'"+pid +"',"
			nretval=SQLExec(Thisform.connstatus,'select * from UBIParaMast Where ParaName= ?pid','MX4')
			If nretval < 0
				Return .F.
			Endif
			i=i+1
		Enddo
		lcquery=left(lcquery,Len(lcquery)-1)
		lcquery=lcquery+")"
	Endif
	Use In MX5

	msqlStr = 'Select ParaName,ParaOrder from UBIParaMast '+lcquery+ ' Order by ParaOrder '
	nretval=SQLExec(Thisform.connstatus,msqlStr,'MX3')
	If nretval < 0
		Return .F.
	Endif
	ocaption = ''
	Select mx3
	For n2 = 1 To Reccount()
		pid = mx3.ParaName
		nretval=SQLExec(Thisform.connstatus,'select * from UBIParaMast where ParaName= ?pid','MX4')
		If nretval < 0
			Return .F.
		Endif
		pcaption = Alltrim(mx4.ParaCap)
		ptype = mx4.ParaType
		pnm = Alltrim(mx4.ParaName)
		isq = Iif(!Empty(mx4.paraQuery) And !Isnull(mx4.paraQuery),.T.,.F.)
		qryid = mx4.paraQuery
		If !(Substr(pcaption,Rat(' ',pcaption),Len(pcaption)) $ ocaption)
			_lPara = .T.
			Do Case
			Case Inlist(Lower(Alltrim(ptype)),"datetime","date")
				lpnm = "lbl" + pnm
				Thisform.AddObject(lpnm,"Label")
				sx = "thisform." + lpnm + ".visible = .t."
				&sx
				sx = "thisform." + lpnm + ".left = 12"
				&sx
				sx = "thisform." + lpnm + ".top = " + Alltrim(Str(Thisform.top1))
				&sx
				sx = "thisform." + lpnm + ".caption = [" + pcaption + " :]"
				&sx
				sx = "thisform." + lpnm + ".fontsize = 8"
				&sx
				sx = "thisform."+  lpnm + ".backstyle = 0"
				&sx
				tpnm = "dt" + pnm
				Thisform.AddObject(tpnm,"dpk")
				sx = "thisform." + tpnm + ".visible = .t."
				&sx
				sx = "thisform." + tpnm + ".left = 115"
				&sx
				sx = "thisform." + tpnm + ".top = " + Alltrim(Str(Thisform.top1-1))
				&sx
				sx = "thisform." + tpnm + ".width = 90"
				&sx
				sx = "thisform." + tpnm + ".fontsize = 8"
				&sx
				If Inlist(Upper(pnm),'FDATE')
					sx = "thisform." + tpnm + ".Value = CTOD('"+Dtoc(company.sta_dt)+"')"
					&sx
				Endif
				If Inlist(Upper(pnm),'TDATE')
					sx = "thisform." + tpnm + ".Value = CTOD('"+Dtoc(company.end_dt)+"')"
					&sx
				Endif
			Otherwise
				lpnm = "lbl" + pnm
				Thisform.AddObject(lpnm,"Label")
				sx = "thisform." + lpnm + ".visible = .t."
				&sx
				sx = "thisform." + lpnm + ".left = 12"
				&sx
				sx = "thisform." + lpnm + ".top = " + Alltrim(Str(Thisform.top1))
				&sx
				sx = "thisform." + lpnm + ".caption = [" + pcaption + " :]"
				&sx
				sx = "thisform." + lpnm + ".fontsize = 8"
				&sx
				sx = "thisform."+  lpnm + ".backstyle = 0"
				&sx
				tpnm = "txt" + pnm
				Thisform.AddObject(tpnm,"TextBox")
				sx = "thisform." + tpnm + ".visible = .t."
				&sx
				sx = "thisform." + tpnm + ".left = 115"
				&sx
				sx = "thisform." + tpnm + ".top = " + Alltrim(Str(Thisform.top1-1))
				&sx
				sx = "thisform." + tpnm + ".width = 200"
				&sx
				sx = "thisform." + tpnm + ".fontsize = 8"
				&sx
				If Inlist(Lower(ptype),"decimal","numeric")
					sx = "thisform." + tpnm + ".value = 0"
					&sx
				Endif
				If isq = .T.
					nretval=SQLExec(Thisform.connstatus,'Select SQLQuery from UBIQueryMast where Queryid=?qryid ','MX5')
					If nretval < 0
						Return .F.
					Endif
					Select MX5
					If Reccount() > 0
						repqr = Alltrim(SQLQuery)
					Endif
					Thisform.qiqry = Alltrim(repqr)
					cmdnm = "cmd" + pnm
					If 'COMMANDCLICK' $ Upper(Alltrim(Set("Classlib")))
					Else
						Set Classlib To commandclick.vcx Additive
					Endif


					Thisform.qry_id = qryid
					Thisform.para_nm=pnm
					Thisform.cmdclick()
					Thisform.qry_id = ""
					If Inlist(Upper(pnm),'FPARTY','FITEM','FSERIES','FCATE') Or !(Substr(pcaption,Rat(' ',pcaption),Len(pcaption)) $ ocaption)
						Select _help
						Index On 1 Tag CURAAA
						Go Top

					Endif
					If Inlist(Upper(pnm),'TPARTY','TITEM','TSERIES','TCATE') Or (Substr(pcaption,Rat(' ',pcaption),Len(pcaption)) $ ocaption)
						Select _help
						Index On 1 Tag CURBBB Desc
						Go Top
					Endif

					cparam=Upper(Alltrim(mx4.searchflds))
					ln=At('{',cparam)
					If ln>0
						exword1 = Substr(cparam,At('{',cparam)+1,Iif(At('#',cparam)>0,At('#',cparam),At('}',cparam))-At('{',cparam)-1)
						exword2 = Iif(At('#',cparam)>0,Substr(cparam,At('#',cparam)+1,Iif(At('#',cparam,2)>0,At('#',cparam,2),At('}',cparam))-At('#',cparam)-1),exword1)
						sx = "thisform." + tpnm + ".value = '"+Evaluate(Field(exword2,'_help'))+"'"
					Else

						sx = "thisform." + tpnm + ".value = '"+Evaluate(Field(1,'_help'))+"'"
					Endif
					&sx


					Thisform.AddObject(cmdnm,"CMDTEST")
					sx = "THISFORM." + cmdnm + ".tag ='"+Alltrim(pnm)+"'"
					&sx
					sx = "THISFORM." + cmdnm + ".VISIBLE = .T."
					&sx
					sx = "THISFORM." + cmdnm + ".CAPTION = ''"
					&sx
					sx = "THISFORM." + cmdnm + ".LEFT = (thisform." + tpnm + ".left + thisform." + tpnm + ".width + 5)"
					&sx
					sx = "THISFORM." + cmdnm + ".TOP = " + Alltrim(Str(Thisform.top1))
					&sx
					sx = "THISFORM." + cmdnm + ".HEIGHT = 22"
					&sx
					sx = "THISFORM." + cmdnm + ".WIDTH = 30"
					&sx
					sx = "THISFORM." + cmdnm + ".picture  ='"+apath+"bmp\loc-on.gif'"
					&sx
					sx = "THISFORM." + cmdnm + ".disabledpicture  ='"+apath+"bmp\loc-off.gif'"
					&sx
					sx = "=BINDEVENT(thisform." + cmdnm + ",'Click',THISFORM,'CMDCLICK',1)"
					&sx
				Endif
			Endcase
			Thisform.top2 = Thisform.top1
			Thisform.top1 = Thisform.top1 + 25
			If Thisform.Height <= Thisform.top1
				Thisform.Height = Thisform.top1 + 25
			Endif

		Else
			pid = mx3.ParaName
			nretval=SQLExec(Thisform.connstatus,'select * from UBIParaMast where ParaName = ?pid','MX4')
			If nretval <=0
				Return .F.
			Endif
			pcaption = Alltrim(mx4.ParaCap)
			ptype = mx4.ParaType
			pnm = Alltrim(mx4.ParaName)
			isq = Iif(!Empty(mx4.paraQuery) And !Isnull(mx4.paraQuery),.T.,.F.)
			qryid = Alltrim(mx4.paraQuery)
			_lwidth=.T.

			Do Case
			Case Inlist(Lower(ptype),"datetime","date")
				lpnm = "lbl" + pnm
				Thisform.AddObject(lpnm,"Label")
				sx = "thisform." + lpnm + ".visible = .t."
				&sx
				sx = "thisform." + lpnm + ".left = 380"
				&sx
				sx = "thisform." + lpnm + ".top = " + Alltrim(Str(Thisform.top2))
				&sx
				sx = "thisform." + lpnm + ".caption = [" + pcaption + " :]"
				&sx
				sx = "thisform." + lpnm + ".fontsize = 8"
				&sx
				sx = "thisform."+  lpnm + ".backstyle = 0"
				&sx
				tpnm = "dt" + pnm
				Thisform.AddObject(tpnm,"dpk")
				sx = "thisform." + tpnm + ".visible = .t."
				&sx
				sx = "thisform." + tpnm + ".left = 480"
				&sx
				sx = "thisform." + tpnm + ".top = " + Alltrim(Str(Thisform.top2-1))
				&sx
				sx = "thisform." + tpnm + ".width = 90"
				&sx
				sx = "thisform." + tpnm + ".fontsize = 8"
				&sx
				If Inlist(Upper(pnm),'FDATE')
					sx = "thisform." + tpnm + ".Value = CTOD('"+Dtoc(company.sta_dt)+"')"
					&sx
				Endif
				If Inlist(Upper(pnm),'TDATE')
					sx = "thisform." + tpnm + ".Value = CTOD('"+Dtoc(company.end_dt)+"')"
					&sx
				Endif
			Otherwise
				lpnm = "lbl" + pnm
				Thisform.AddObject(lpnm,"Label")
				sx = "thisform." + lpnm + ".visible = .t."
				&sx
				sx = "thisform." + lpnm + ".left = 380"
				&sx
				sx = "thisform." + lpnm + ".top = " + Alltrim(Str(Thisform.top2))
				&sx
				sx = "thisform." + lpnm + ".caption = [" + pcaption + " :]"
				&sx
				sx = "thisform." + lpnm + ".backstyle = 0"
				&sx
				sx = "thisform." + lpnm + ".fontsize = 8"
				&sx
				sx = "thisform."+  lpnm + ".backstyle = 0"
				&sx
				tpnm = "txt" + pnm
				Thisform.AddObject(tpnm,"TextBox")
				sx = "thisform." + tpnm + ".visible = .t."
				&sx
				sx = "thisform." + tpnm + ".left = 480"
				&sx
				sx = "thisform." + tpnm + ".top = " + Alltrim(Str(Thisform.top2-1))
				&sx
				sx = "thisform." + tpnm + ".width = 200"
				&sx
				sx = "thisform." + tpnm + ".fontsize = 8"
				&sx
				If Inlist(Lower(ptype),"decimal","numeric")
					sx = "thisform." + tpnm + ".value = 0"
					&sx
				Endif
				If isq = .T.
					nretval=SQLExec(Thisform.connstatus,'Select SQLQuery from UBIQueryMast where Queryid=?qryid ','MX5')
					If nretval <=0
						Return .F.
					Endif
					Select MX5
					If Reccount() > 0
						repqr = Alltrim(SQLQuery)
					Endif
					Thisform.qiqry = Alltrim(repqr)
					cmdnm = "cmd" + pnm
					If 'COMMANDCLICK' $ Upper(Alltrim(Set("Classlib")))
					Else
						Set Classlib To commandclick.vcx Additive
					Endif

						Thisform.qry_id = qryid
						Thisform.para_nm=pnm
						Thisform.cmdclick()
						Thisform.qry_id = ""
						If Inlist(Upper(pnm),'FPARTY','FITEM','FSERIES','FCATE') Or !(Substr(pcaption,Rat(' ',pcaption),Len(pcaption)) $ ocaption)
							Select _help
							Index On 1 Tag CURAAA
							Go Top
						Endif
						If Inlist(Upper(pnm),'TPARTY','TITEM','TSERIES','TCATE') Or (Substr(pcaption,Rat(' ',pcaption),Len(pcaption)) $ ocaption)
							Select _help
							Index On 1 Tag CURBBB Desc
							Go Top
						Endif

						cparam=Upper(Alltrim(mx4.searchflds))
						ln=At('{',cparam)
						If ln>0
							exword1 = Substr(cparam,At('{',cparam)+1,Iif(At('#',cparam)>0,At('#',cparam),At('}',cparam))-At('{',cparam)-1)
							exword2 = Iif(At('#',cparam)>0,Substr(cparam,At('#',cparam)+1,Iif(At('#',cparam,2)>0,At('#',cparam,2),At('}',cparam))-At('#',cparam)-1),exword1)
							sx = "thisform." + tpnm + ".value = '"+Evaluate(Field(exword2,'_help'))+"'"
						Else
							sx = "thisform." + tpnm + ".value = '"+Evaluate(Field(1,'_help'))+"'"
						Endif
						&sx



					Thisform.AddObject(cmdnm,"CMDTEST")
					sx = "THISFORM." + cmdnm + ".VISIBLE = .T."
					&sx
					sx = "THISFORM." + cmdnm + ".tag ='"+Alltrim(pnm)+"'"
					&sx
					sx = "THISFORM." + cmdnm + ".CAPTION = ''"
					&sx
					sx = "THISFORM." + cmdnm + ".LEFT = (thisform." + tpnm + ".left + thisform." + tpnm + ".width + 5)"
					&sx
					sx = "THISFORM." + cmdnm + ".TOP = " + Alltrim(Str(Thisform.top2))
					&sx
					sx = "THISFORM." + cmdnm + ".HEIGHT = 22"
					&sx
					sx = "THISFORM." + cmdnm + ".WIDTH = 30"
					&sx
					sx = "THISFORM." + cmdnm + ".picture  ='"+apath+"bmp\loc-on.gif'"
					&sx
					sx = "THISFORM." + cmdnm + ".disabledpicture  ='"+apath+"bmp\loc-off.gif'"
					&sx
					sx = "=BINDEVENT(thisform." + cmdnm +",'Click',THISFORM,'CMDCLICK',1)"
					&sx
				Endif
			Endcase

		Endif

		Select mx3
		ocaption = pcaption
		If !Eof()
			Skip
		Endif
	Endfor

	If _lPara = .F.
		Thisform.Width = 600
		Thisform.Panel.Width  = Thisform.Width
		Thisform.cmdShowReport.Left = 415
		Thisform.cmdCancel.Left = Thisform.cmdShowReport.Left + Thisform.cmdShowReport.Width + 10
	Else
		Thisform.line1.Top = Thisform.top1 + 10
		Thisform.cmdCancel.Top = Thisform.top1 + 10 + 10
		Thisform.cmdShowReport.Top = Thisform.top1 + 10 + 10
	Endif

	Thisform.line1.Left = Thisform.Left + 10
	Thisform.line1.Width = Thisform.Width - 20

	If Thisform.Height <= Thisform.top1 + 10 + 10 + 40
		Thisform.Height = Thisform.top1 + 70
		Thisform.Panel.Align = 0
		Thisform.Panel.Align = 2
	Endif


	If _lwidth=.F.
		Thisform.Width = 550
		Thisform.Panel.Width  = Thisform.Width
		Thisform.line1.Width = Thisform.Width - 20
	Endif

	nretval=SQLDisconnect(Thisform.connstatus)
	If nretval<=0
		Return .F.
	Endif
Endif

ENDPROC
PROCEDURE chk_validation
IF TYPE('Thisform.dtFrmDate')='O' and TYPE('Thisform.dtTodate')='O'
	IF empty(Thisform.dtFrmDate.value)
		MESSAGEBOX("From date can not be empty...!!!")
		Thisform.dtFrmDate.setFocus
		RETURN .f.
	Endif
	IF empty(Thisform.dtToDate.value)
		MESSAGEBOX("To date can not be empty...!!!")
		Thisform.dtToDate.setFocus
		RETURN .f.
	Endif
	IF (Thisform.dtFrmDate.value)> (Thisform.dtToDate.value)
		MESSAGEBOX("From Date can not be greater than to date...!!!")
		thisform.dtToDate.value={  /  /    }
		Thisform.dtToDate.setFocus
		RETURN .f.
	Endif
ENDIF
RETURN .t.
ENDPROC
PROCEDURE Init

lparameters lrptid,lCaption,lrpttype,lrange


PUBLIC lrangeId				
lrangeId=lrange				
thisform.Caption = 'Parameters for '+lCaption
thisform.Icon = icopath
thisform.repttype = lrpttype
thisform.panel.Panels(1).Text = 'Report ID :'+lrptId
thisform.addobject("sqlconobj","sqlconnudobj")

IF TYPE('mvu_nenc')='U'
	mvu_nenc=0
endif
IF mvu_nenc=1
	_user=thisform.sqlconobj.newdecry(strconv(mvu_user,16),"Ud*yog+1993Uid")
	_pass=thisform.sqlconobj.newdecry(STRCONV(mvu_pass,16),"Ud*yog+1993Pwd")
ELSE
	_user=thisform.sqlconobj.dec(thisform.sqlconobj.ondecrypt(mvu_user))
	_pass=thisform.sqlconobj.dec(thisform.sqlconobj.ondecrypt(mvu_pass))
ENDIF



IF 'DATEPICKER' $ UPPER(ALLTRIM(SET('classlib')))
ELSE
	set classlib to datepicker additive
ENDIF

thisform.rptid = lrptId
constr = 'Driver={SQL server};server='+mvu_server+';database='+ALLTRIM(company.dbname)+';uid='+_user+';pwd='+_pass+';'
thisform.connStatus =sqlstringconnect(constr)
if thisform.connStatus<= 0
	=messagebox("Connection Error...!!!",0+64,vumess)
	return .f.
endif

thisform.left1 = 12 && 156
thisform.top1 = 20
thisform.left2 = 300 && 480
thisform.top2 = 20

thisform.BackColor=VAL(company.vcolor)
thisform.genparameters(lrptid)
thisform.AutoCenter=.t.
thisform.AddObject("_stuffObject","_stuff")
thisform._stuffObject._objectPaint()



ENDPROC
PROCEDURE Load
SET TALK OFF
SET score off
SET DELETED ON
SET DATE TO british
SET CENTURY on

ENDPROC
PROCEDURE Activate
If Type('_screen.ActiveForm.cmdshowreport')='O'
	On Key Label CTRL+O _Screen.ActiveForm.cmdshowreport.Click()
Endif

ENDPROC
      ����    �   �                         ;   %   D       Y      S           �  U  
  <�  � U  THISFORM Click,     ��1 q 2                       #       )   �                        ����    �  �                        J�   %   2      E  �   A          �  U  � %�C� mvu_nencb� U��) � T�  �� �� � %��  ���� �. T� �CC� ��� Ud*yog+1993Uid� � � ��. T� �CC� ��� Ud*yog+1993Pwd� � � �� �� �" T� �CC � � � �	 � � � ��" T� �CC � � � �	 � � � �� �a T�
 �� Driver={SQL server};server=� �
 ;database=C� � �� ;uid=� � ;pwd=� �  �� T� � �C�
 ��� %�� � � ����( ��C� Connection Error...!!!�@� �x�� B�-�� �C T� � � ���� ��' Status : Please wait, Processing....!!!�� T� �� � ��O T� ��B select SqlQuery From UBIQueryMast Where  ReportId=?Thisform.rptId �� T� �C� � � � MX3�i�� %�� � ��{� B�-�� � T� �C� Datev�� G(� AMERICAN� F� � -� �� � � T� ��
 [NC_ParaS=�� T� ��
 [NC_ParaN=�� T� ��
 [NC_ParaD=�� T� ��  �� T� ��  �� T�  ��  �� T�! �� �� T�" �� �� T�# �� �� T�$ �� �% �� T�& �C� {@�$ ��� T�' ���� +��' �& ��H� T�( �C�$ � {@� }�' ����J T� �C� � �. select * from UBIParaMast Where ParaName= ?pid� MX4�i�� %�� � ��� B�-�� � H��3�& �CC�) �* @� datetime� date����� T�# ��# ��� T� �� C�) �+ �� ,��+ T�, �� thisform.dtC�) �+ �� .value�� x2 = &x2
+ T�  ��  � [C�) �+ �� =C�, *� ]��( �CC�) �* @� numeric� decimal����� T�" ��" ��� T� �� C�) �+ �� ,��: T�, �� alltrim(STR(thisform.txtC�) �+ �� .value))�� x2 = &x2
) T� �� � [C�) �+ �� =�, � ]�� 2�3� T�! ��! ��� T� �� C�) �+ �� ,��5 T�, �� alltrim(thisform.txtC�) �+ �� .value)�� x2 = &x2
) T� �� � [C�) �+ �� =�, � ]�� � T�' ��' ��� � Set Date To &OldDateStat
 T� �C� � �g�� %�� � ���� B�-�� � %�C� �- 
���� B� � T�. �C� WScript.Shell�N�� H���� �� �/ � PIVOT��� T�0 �� UdMISReports.exe �� � T� �C� CC� �>�=�� T� �C� CC� �>�=�� T� �C� CC� �>�=�� T�1 �� "� � ��& T�2 �� |CC� >� � � � �  6��& T�3 �� |CC� >� � � � �  6��+ T�4 �� |CC�  >� � �  � �  6� "��$ T�5 �C��]� \C��]� ._Sh�� %�C�5 0��5� Erase &_FlName
 � ��C�0 �5 ����� ��C�1 �5 ����� ��C�2 �5 ����� ��C�3 �5 ����� ��C�4 �5 ����� T�6 �C�5 ���� %�CCC� �/ f�� PIVOT���u� T�7 �� �8 �� T�9 �� � �� T�: �� �; �� T�< �C�= �  � <*#*>��� T�> �C� �  � <*#*>��� T�? �C� �@ ��� T�A �CC�B _�  � <*#*>��� T�C �C� Pathv�� T�D �CC�C _�  � <*#*>���" T�E �CCC� �F *_�  � <*#*>���" T�G �CCC� �H *_�  � <*#*>��� T�I ��  ��6 T�J �C� �K �CCC� �@ ��
� � ,� �  6C� �@ ��� ��' ���(�C�J >��G
�! T�I ��I CCC�J �' �\� �� ��, T�I ��I CCC�I ��R� ,� �  � � ,6��% T�6 ��6 �  CC�7 _��  C�9 ���. T�6 ��6 �  C� ��  C� ��  C� ���9 T�6 ��6 �  C�L ��  C�M ��  C�< ��  C�> ���9 T�6 ��6 �  C�N ��  CC�O Z��  C�P ��  �I ��9 T�6 ��6 �  C�A ��  C�D ��  C�E ��  C�G ��� � ��C �6 �. �Q �� ��C�      �?7��$ T� � � ���� �� Status :�� %�C�5 0���� Erase &_FlName
 � UR  MVU_NENC _USER THISFORM	 SQLCONOBJ NEWDECRY MVU_USER _PASS MVU_PASS DEC	 ONDECRYPT CONSTR
 MVU_SERVER COMPANY DBNAME
 CONNSTATUS VUMESS PANEL PANELS TEXT LQUERYID RPTID MSQLSTR NRETVAL OLDDATESTAT AMERICAN MX3 NC_PARAS NC_VALUE NC_PARAN NC_PARAD	 NC_VALUES	 NC_VALUEN	 NC_VALUED
 VNC_COUNTS
 VNC_COUNTN
 VNC_COUNTD
 LCSQLQUERY SQLQUERY _LNPARACOUNT I PID MX4 PARATYPE PARANAME X2 CHK_VALIDATION OWSHELL REPTTYPE _DADOSEXEPATH
 _OTHPARAM1
 _OTHPARAM2
 _OTHPARAM3
 _OTHPARAM4 _FLNAME
 _SHELLEXEC TCCOMPID COMPID TCCOMPDB TCCOMPNM CO_NAME VICOPATH ICOPATH PAPPLCAPTION _PASSROUTE1
 PASSROUTE1 APPPATH APATH CSETPATH SETPATH CSDATE STA_DT CEDATE END_DT	 _PRODCODE PVALUE	 PASSROUTE LRANGEID	 MUSERNAME	 PAPPLNAME PAPPLID	 PAPPLCODE EXEC Click,     ��1 �� A ��� !!A 3A�q A 1��q A 3q A � qqq� � � � � � A� !��q A � a��� ����� �� �Q� �B A �2q A "A A �� ��A ���Saa�C� !A "!!!!���!�1�!!� a�A �R����A 1A� !A 6                       C      )   �                        BArial, 0, 9, 5, 15, 12, 32, 3, 0
Arial, 1, 8, 5, 14, 11, 29, 3, 0
      /OLEObject = E:\U6\UdyogERP\class\mscomctl.ocx
     
 ��ࡱ�                >  ��	                               ����        ��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������R o o t   E n t r y                                               ��������                               �FXK���           O l e O b j e c t D a t a                                            ����                                        8       A c c e s s O b j S i t e D a t a                             &  ������������                                       \        C h a n g e d P r o p s                                         ������������                                                    ��������         �����������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g8�����j ��(6(!C4    J  F  �~��   �     ��� �ͫ       \                          $   8                       9368265E-85FE-11d1-8BE3-0000F8754DA1  ( A : )   L o c a l   D i s k   (   �)                                                              ��������           �e g  g     
   R e p o r t   I d : 
   R e p o r t   I d : �p =)  �	        S t a t u s   :    S t a t u s   : #l �	  �	        I N S �  �	  �	        2 0 : 0 0     ��     R������ � K�Q   �DB Arialrialal�       ITop = 93
Left = 0
Height = 22
Width = 716
Align = 2
Name = "Panel"
      frmUBIReport      Panel      
olecontrol      
olecontrol      .PROCEDURE Click
release thisform

ENDPROC
      �Top = 67
Left = 631
Height = 22
Width = 77
FontBold = .T.
FontSize = 8
Anchor = 12
Caption = "Cancel"
ToolTipText = "Click for Cancel"
Name = "cmdCancel"
      frmUBIReport      	cmdCancel      commandbutton      commandbutton     NPROCEDURE Click

If Type('mvu_nenc')='U'
	mvu_nenc=0
Endif
If mvu_nenc=1
	_user=Thisform.sqlconobj.newdecry(Strconv(mvu_user,16),"Ud*yog+1993Uid")
	_pass=Thisform.sqlconobj.newdecry(Strconv(mvu_pass,16),"Ud*yog+1993Pwd")
Else
	_user=Thisform.sqlconobj.dec(Thisform.sqlconobj.ondecrypt(mvu_user))
	_pass=Thisform.sqlconobj.dec(Thisform.sqlconobj.ondecrypt(mvu_pass))
Endif

constr = 'Driver={SQL server};server='+mvu_server+';database='+Alltrim(company.dbname)+';uid='+_user+';pwd='+_pass+''


Thisform.connStatus =Sqlstringconnect(constr)
If Thisform.connStatus<= 0
	=Messagebox("Connection Error...!!!",0+64,vumess)
	Return .F.
Endif
Thisform.Panel.Panels(2).Text = 'Status : Please wait, Processing....!!!'
lQueryId = Thisform.rptId

msqlStr = 'select SqlQuery From UBIQueryMast Where  ReportId=?Thisform.rptId '
nRetval=SQLExec(Thisform.connStatus,msqlStr,'MX3')
If nRetval < 0
	Return .F.
Endif


OldDateStat = Set("Date")
Set Date To AMERICAN
Select mx3
Locate

Local nc_ParaS,nc_Value
nc_ParaS = "[NC_ParaS="
nc_ParaN = "[NC_ParaN="
nc_ParaD = "[NC_ParaD="

nc_valueS = ''
nc_valueN = ''
nc_valueD = ''


vNC_CountS=0
vNC_CountN=0
vNC_CountD=0

lcsqlquery=mx3.SQLQuery
_lnparacount=Occurs("{@",lcsqlquery)

i=1
Do While i<=_lnparacount
	pid = Strextract(lcsqlquery,"{@","}",i)
	nRetval=SQLExec(Thisform.connStatus,'select * from UBIParaMast Where ParaName= ?pid','MX4')
	If nRetval < 0
		Return .F.
	Endif

	Do Case
	Case Inlist(Lower(mx4.ParaType),"datetime","date")
		vNC_CountD=vNC_CountD+1
		nc_ParaD=nc_ParaD+Alltrim(mx4.ParaName)+","

		x2 = "thisform.dt"+Alltrim(mx4.ParaName)+".value"
		x2 = &x2
		nc_valueD=nc_valueD+"["+Alltrim(mx4.ParaName)+"="+Dtoc(x2)+"]"

	Case Inlist(Lower(mx4.ParaType),"numeric","decimal")
		vNC_CountN=vNC_CountN+1
		nc_ParaN=nc_ParaN+Alltrim(mx4.ParaName)+","

		x2 = "alltrim(STR(thisform.txt"+Alltrim(mx4.ParaName)+".value))"
		x2 = &x2
		nc_valueN=nc_valueN+"["+Alltrim(mx4.ParaName)+"="+x2+"]"
	Otherwise
		vNC_CountS=vNC_CountS+1
		nc_ParaS=nc_ParaS+Alltrim(mx4.ParaName)+","
		x2 = "alltrim(thisform.txt"+Alltrim(mx4.ParaName)+".value)"
		x2 = &x2
		nc_valueS=nc_valueS+"["+Alltrim(mx4.ParaName)+"="+x2+"]"

	Endcase
	i=i+1
Enddo
Set Date To &OldDateStat

nRetval=SQLDisconnect(Thisform.connStatus)
If nRetval<=0
	Return .F.
Endif

If !Thisform.chk_validation()
	Return
Endif

oWSHELL = Createobject("WScript.Shell")

Do Case
Case Thisform.repttype = "PIVOT"
	_DadosExePath  = "UdMISReports.exe "
Endcase

nc_ParaS=Left(nc_ParaS,Len(Alltrim(nc_ParaS))-1)
nc_ParaN=Left(nc_ParaN,Len(Alltrim(nc_ParaN))-1)
nc_ParaD=Left(nc_ParaD,Len(Alltrim(nc_ParaD))-1)


_OthParam1 = '"'+Thisform.rptId
_OthParam2 = "|"+Iif(Len(nc_valueS)<>0,nc_valueS,"")
_OthParam3 = "|"+Iif(Len(nc_valueN)<>0,nc_valueN,"")
_OthParam4 = "|"+Iif(Len(nc_valueD)<>0,nc_valueD,"")+'"'


_FlName       = Sys(2023)+"\"+Sys(2015)+"._Sh"

If File(_FlName)
	Erase &_FlName
Endif

Strtofile(_DadosExePath,_FlName,1)
Strtofile(_OthParam1,_FlName,1)
Strtofile(_OthParam2,_FlName,1)
Strtofile(_OthParam3,_FlName,1)
Strtofile(_OthParam4,_FlName,1)

_ShellExec=Filetostr(_FlName)

If Inlist(Alltrim(Upper(Thisform.repttype)),"PIVOT")
	tcCompId = company.CompId
	tcCompdb = company.dbname
	tcCompNm = company.co_name
	vicopath  =Strtran(icopath,' ','<*#*>')
	pApplCaption=Strtran(vumess,' ','<*#*>')
	_PassRoute1 = Alltrim(company.PassRoute1)
	appPath=Strtran(Transform(apath),' ','<*#*>')
	csetPath=Set("Path")
	SetPath=Strtran(Transform(csetPath),' ','<*#*>')
	csdate =Strtran(Transform(Dtoc(company.sta_dt)),' ','<*#*>')
	cedate =Strtran(Transform(Dtoc(company.end_dt)),' ','<*#*>')
	_prodcode =""
	pvalue = Allt(company.PassRoute)+Iif(!Empty(Alltrim(company.PassRoute1)),",","")+Alltrim(company.PassRoute1)
	For i = 1 To Len(pvalue)
		_prodcode = _prodcode + Chr(Asc(Substr(pvalue,i,1))/2)
	Next i
	_prodcode = _prodcode+Iif(Right(Alltrim(_prodcode),1)=',',"",",")

	_ShellExec = _ShellExec +" "+Alltrim(Transform(tcCompId))+" "+Alltrim(tcCompdb)
	_ShellExec = _ShellExec+" "+Alltrim(mvu_server)+" "+Alltrim(_user)+" "+Alltrim(_pass)
	_ShellExec = _ShellExec+" "+Alltrim(lRangeId)+" "+Alltrim(musername)+" "+Alltrim(vicopath)+" "+Alltrim(pApplCaption)
	_ShellExec = _ShellExec+" "+Alltrim(pApplName)+" "+Alltrim(Str(pApplId))  +" "+Alltrim(pApplCode)+" "+_prodcode
	_ShellExec = _ShellExec+" "+Alltrim(appPath)+" "+Alltrim(SetPath)+" "+Alltrim(csdate)+" "+Alltrim(cedate)
Endif

oWSHELL.Exec(_ShellExec)
=Inkey(1.5)
Thisform.Panel.Panels(2).Text = 'Status :'

If File(_FlName)
	Erase &_FlName
Endif





ENDPROC
      �Top = 67
Left = 537
Height = 22
Width = 88
FontBold = .T.
FontSize = 8
Anchor = 12
Caption = "Show Report"
ToolTipText = "Click for View Report"
Name = "cmdShowReport"
      frmUBIReport      cmdShowReport      commandbutton      commandbutton      NBorderWidth = 2
Height = 1
Left = 0
Top = 60
Width = 705
Name = "Line1"
      frmUBIReport      Line1      line      line      �left1
top1
left2
top2
qiqry
nhandle
connstatus
tagval
cmdobj
rptid
repttype
qry_id
para_nm
*cmdclick 
*genparameters 
*chk_validation 
     <����    �<  �<                        ��   %   �7      C<    8          �  U   %�C� mvu_nencb� U��) � T�  �� �� � %��  ���� �. T� �CC� ��� Ud*yog+1993Uid� � � ��. T� �CC� ��� Ud*yog+1993Pwd� � � �� �� �" T� �CC � � � �	 � � � ��" T� �CC � � � �	 � � � �� �b T�
 �� Driver={SQL server};server=� �
 ;database=C� � �� ;uid=� � ;pwd=� � ;�� %�C� � �
��o� T� �� � �� � T� � �C�
 ��� %�� � � ����( ��C� Connection Error...!!!�@� �x�� B�-�� �C T� � � ���� ��' Status : Please wait, Processing....!!!�� %�C� � ���2� T� �� � �� �J� T� �� � �� �� T� �C� � �| Select a.Sqlquery,b.searchflds from UBIQueryMast a INNER JOIN UBIParaMast b ON(a.QueryId=b.ParaQuery) where b.ParaName=?lpnm� _qryInfo�i�� %�� � ��� B�-�� �^� %�C� _qryInfoN� ��Z�- ��C� Help Query not found....!!!�@� �x�� B�-�� � � F� � T� �CC� �f�� T� �� �� T� �C� � �g�� %�� � ���� B�-�� � %�C� � �
���� T� � �� �� � T� � �� ��E T� �C� EXE� �  � � _help� thisform.nHandle� �  � � � �� %�� � ��J� B�-�� �' T� �C� thisform.nHandle� � �! �� %�� � ����1 ��C� SQL disconnect errorC� CCE��0� �x�� � %�C� � �
���� B�-�� � ��" �# �$ �% �& �' � T�# ��  �� T�$ ��  �� T�% ��  �� T�& ��  �� T�' ��  �� T�' �CC� �( �f�� T�" �C� {�' �� %��" � ����Q T�# �C�' C� {�' �CC� #�' � � C� #�' �	 C� }�' 6C� {�' �\��o T�$ �CC� #�' � �P C�' C� #�' �CC� #�' �� � C� #�' ��	 C� }�' 6C� #�' �\� �# 6��x T�% �CC� #�' �� �V C�' C� #�' ��CC� #�' �� � C� #�' ��	 C� }�' 6C� #�' ��\� �  6��T T�& �CC� #�' �� �2 C�' C� #�' ��C� }�' C� #�' ��\� �  6�� � %�C�# ���V�U T�# �C� C� SELECT � C� SELECT >C�  � �C� SELECT � C� SELECT >\�� T�$ ��# �� %�CC�# �f� DISTINCT���y T�# �C� C� SELECT DISTINCT � C� SELECT DISTINCT >C�  � �C� SELECT DISTINCT � C� SELECT DISTINCT >\�� T�$ ��# �� � %�C� AS� � ��R� F�) � T�# �CC�/��� T�$ ��# �� � � %�C� _helpN� ���
� ��* �+ �, � J��  �(�, � %�C� �- �
����+ T�, ��	 thisform.C� �- �� .caption�� lcap = &lcap
 �5 T�* ��	 thisform.C� �. � cmd� txt�� .value�� %�C�& ���
� T�/ ��# �� +�C�/ �
��
�7 T�0 �CC� ,�/ � � C�/ �C� ,�/ �\� �/ 6��4 T�0 �CC� .�0 � � C�0 C� .�0 �\� �0 6��4 T�/ �CC� ,�/ � � C�/ C� ,�/ �\� �  6��0 T�& ��& CC�& �
� � ,� �  6�0 � :�0 �� � �y lresu   = uegetpop('_Help','Select '+Alltrim(lcap)+'...',exword1,exword2,&ltxtobj,'','','',.F.,[],[],exword4,exword3)
 %�C� lresub� L���
� &ltxtobj = lresu
 � ��
�" ��C� No Records found�@� �x�� B�-�� �$ T� � � ���� �� Status :�� U1  MVU_NENC _M1KEY_USER THISFORM	 SQLCONOBJ NEWDECRY MVU_USER _M1KEY_PASS MVU_PASS DEC	 ONDECRYPT CONSTR
 MVU_SERVER COMPANY DBNAME QRY_ID NCONNSTATUS
 CONNSTATUS VUMESS PANEL PANELS TEXT LPNM THIS TAGVAL PARA_NM NRETVAL _QRYINFO QSTRING SQLQUERY PNM NHANDLE DATACONN DATASESSIONID SQLCONNCLOSE LN EXWORD1 EXWORD2 EXWORD3 EXWORD4 CPARAM
 SEARCHFLDS _HELP LTXTOBJ LRESU LCAP TAG CMDOBJ EXWORD5 EXWORD6;! ��  � �� � � %�C�  �
��4!�G T� ��: select Sqlquery From UBIQueryMast Where ReportId= ?lparaid�� T� �C� � � � MX5�i�� %�� � ��� � B�-�� � T� �� �	 �� T�
 �C� {@C� ���� T� ��  �� %��
 � ���! T� ��  Where ParaName In (�� T� ���� +�� �
 ���� T� �C� � {@� }� ���� T� �� � '� � ',��J T� �C� � �. select * from UBIParaMast Where ParaName= ?pid� MX4�i�� %�� � ���� B�-�� � T� �� ��� � T� �C� C� >�=�� T� �� � )�� � Q� �T T� ��+ Select ParaName,ParaOrder from UBIParaMast � �  Order by ParaOrder �� T� �C� � � � MX3�i�� %�� � ���� B�-�� � T� ��  �� F� � �� ���(�CN��B� T� �� � ��J T� �C� � �. select * from UBIParaMast where ParaName= ?pid� MX4�i�� %�� � ��6� B�-�� � T� �C� � ��� T� �� � �� T� �C� � ���* T� �CC� � �
�
 C� � �
	� a� -6�� T� �� � ��# %�C� C�  � �C� >\� 
��� T� �a�� H�����% �CCC� �@� datetime� date���Y� T� �� lbl� �� ��C � � Label� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
( T� ��	 thisform.� �
 .left = 12�� &sx
0 T� ��	 thisform.� � .top = CC� � Z��� &sx
5 T� ��	 thisform.� � .caption = [� �  :]�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
, T� ��	 thisform.� � .backstyle = 0�� &sx
 T� �� dt� �� ��C � � dpk� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
) T� ��	 thisform.� � .left = 115�� &sx
4 T� ��	 thisform.� � .top = CC� � �Z��� &sx
) T� ��	 thisform.� � .width = 90�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
 %�CC� f� FDATE�����< T� ��	 thisform.� � .Value = CTOD('C�  �! *� ')�� &sx
 � %�CC� f� TDATE���U�< T� ��	 thisform.� � .Value = CTOD('C�  �" *� ')�� &sx
 � 2��� T� �� lbl� �� ��C � � Label� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
( T� ��	 thisform.� �
 .left = 12�� &sx
0 T� ��	 thisform.� � .top = CC� � Z��� &sx
5 T� ��	 thisform.� � .caption = [� �  :]�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
, T� ��	 thisform.� � .backstyle = 0�� &sx
 T� �� txt� �� ��C � � TextBox� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
) T� ��	 thisform.� � .left = 115�� &sx
4 T� ��	 thisform.� � .top = CC� � �Z��� &sx
* T� ��	 thisform.� � .width = 200�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
% %�CC� @� decimal� numeric���L
�( T� ��	 thisform.� �
 .value = 0�� &sx
 � %�� a����S T� �C� � �7 Select SQLQuery from UBIQueryMast where Queryid=?qryid � MX5�i�� %�� � ���
� B�-�� � F� � %�CN� ���
� T�# �C�	 ��� � T� �$ �C�# ��� T�% �� cmd� ��+ %�� COMMANDCLICKCCC� Classlibv�f��I� �k� G~(� commandclick.vcx� � T� �( �� �� T� �) �� �� ��C� �* �� T� �( ��  ��Q %�CC� f� FPARTY� FITEM� FSERIES� FCATE�� C� C�  � �C� >\� 
��� F�+ � & �����, � #)� �P %�CC� f� TPARTY� TITEM� TSERIES� TCATE�� C� C�  � �C� >\� ���� F�+ � & �����- <� #)� � T�. �CC� �/ �f�� T�0 �C� {�. �� %��0 � ����Q T�1 �C�. C� {�. �CC� #�. � � C� #�. �	 C� }�. 6C� {�. �\��o T�2 �CC� #�. � �P C�. C� #�. �CC� #�. �� � C� #�. ��	 C� }�. 6C� #�. �\� �1 6��= T� ��	 thisform.� �
 .value = 'CC�2 � _help/�� '�� ��= T� ��	 thisform.� �
 .value = 'CC�� _help/�� '�� � &sx
 ��C �% � CMDTEST� � ��0 T� ��	 THISFORM.�% � .tag ='C� �� '�� &sx
, T� ��	 THISFORM.�% � .VISIBLE = .T.�� &sx
+ T� ��	 THISFORM.�% � .CAPTION = ''�� &sx
\ T� ��	 THISFORM.�% � .LEFT = (thisform.� � .left + thisform.� � .width + 5)�� &sx
0 T� ��	 THISFORM.�% � .TOP = CC� � Z��� &sx
* T� ��	 THISFORM.�% � .HEIGHT = 22�� &sx
) T� ��	 THISFORM.�% � .WIDTH = 30�� &sx
A T� ��	 THISFORM.�% � .picture  ='�3 � bmp\loc-on.gif'�� &sx
J T� ��	 THISFORM.�% � .disabledpicture  ='�3 � bmp\loc-off.gif'�� &sx
H T� �� =BINDEVENT(thisform.�% � ,'Click',THISFORM,'CMDCLICK',1)�� &sx
 � � T� �4 �� � �� T� � �� � ��� %�� �5 � � ��	� T� �5 �� � ��� � �� T� �� � ��K T� �C� � �/ select * from UBIParaMast where ParaName = ?pid� MX4�i�� %�� � ���� B�-�� � T� �C� � ��� T� �� � �� T� �C� � ���* T� �CC� � �
�
 C� � �
	� a� -6�� T� �C� � ��� T� �a�� H���# �CC� @� datetime� date����� T� �� lbl� �� ��C � � Label� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
) T� ��	 thisform.� � .left = 380�� &sx
0 T� ��	 thisform.� � .top = CC� �4 Z��� &sx
5 T� ��	 thisform.� � .caption = [� �  :]�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
, T� ��	 thisform.� � .backstyle = 0�� &sx
 T� �� dt� �� ��C � � dpk� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
) T� ��	 thisform.� � .left = 480�� &sx
4 T� ��	 thisform.� � .top = CC� �4 �Z��� &sx
) T� ��	 thisform.� � .width = 90�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
 %�CC� f� FDATE���%�< T� ��	 thisform.� � .Value = CTOD('C�  �! *� ')�� &sx
 � %�CC� f� TDATE�����< T� ��	 thisform.� � .Value = CTOD('C�  �" *� ')�� &sx
 � 2�� T� �� lbl� �� ��C � � Label� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
) T� ��	 thisform.� � .left = 380�� &sx
0 T� ��	 thisform.� � .top = CC� �4 Z��� &sx
5 T� ��	 thisform.� � .caption = [� �  :]�� &sx
, T� ��	 thisform.� � .backstyle = 0�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
, T� ��	 thisform.� � .backstyle = 0�� &sx
 T� �� txt� �� ��C � � TextBox� � ��, T� ��	 thisform.� � .visible = .t.�� &sx
) T� ��	 thisform.� � .left = 480�� &sx
4 T� ��	 thisform.� � .top = CC� �4 �Z��� &sx
* T� ��	 thisform.� � .width = 200�� &sx
+ T� ��	 thisform.� � .fontsize = 8�� &sx
% %�CC� @� decimal� numeric�����( T� ��	 thisform.� �
 .value = 0�� &sx
 � %�� a���S T� �C� � �7 Select SQLQuery from UBIQueryMast where Queryid=?qryid � MX5�i�� %�� � ��.� B�-�� � F� � %�CN� ��X� T�# �C�	 ��� � T� �$ �C�# ��� T�% �� cmd� ��+ %�� COMMANDCLICKCCC� Classlibv�f���� ��� G~(� commandclick.vcx� � T� �( �� �� T� �) �� �� ��C� �* �� T� �( ��  ��Q %�CC� f� FPARTY� FITEM� FSERIES� FCATE�� C� C�  � �C� >\� 
��{� F�+ � & �����, � #)� �P %�CC� f� TPARTY� TITEM� TSERIES� TCATE�� C� C�  � �C� >\� ���� F�+ � & �����- <� #)� � T�. �CC� �/ �f�� T�0 �C� {�. �� %��0 � ��#�Q T�1 �C�. C� {�. �CC� #�. � � C� #�. �	 C� }�. 6C� {�. �\��o T�2 �CC� #�. � �P C�. C� #�. �CC� #�. �� � C� #�. ��	 C� }�. 6C� #�. �\� �1 6��= T� ��	 thisform.� �
 .value = 'CC�2 � _help/�� '�� �h�= T� ��	 thisform.� �
 .value = 'CC�� _help/�� '�� � &sx
 ��C �% � CMDTEST� � ��, T� ��	 THISFORM.�% � .VISIBLE = .T.�� &sx
0 T� ��	 THISFORM.�% � .tag ='C� �� '�� &sx
+ T� ��	 THISFORM.�% � .CAPTION = ''�� &sx
\ T� ��	 THISFORM.�% � .LEFT = (thisform.� � .left + thisform.� � .width + 5)�� &sx
0 T� ��	 THISFORM.�% � .TOP = CC� �4 Z��� &sx
* T� ��	 THISFORM.�% � .HEIGHT = 22�� &sx
) T� ��	 THISFORM.�% � .WIDTH = 30�� &sx
A T� ��	 THISFORM.�% � .picture  ='�3 � bmp\loc-on.gif'�� &sx
J T� ��	 THISFORM.�% � .disabledpicture  ='�3 � bmp\loc-off.gif'�� &sx
H T� �� =BINDEVENT(thisform.�% � ,'Click',THISFORM,'CMDCLICK',1)�� &sx
 � � � F� � T� �� �� %�C+
��>� H� � �� %�� -���� T� �6 ��X�� T� �7 �6 �� �6 �� T� �8 �9 �����' T� �: �9 �� �8 �9 � �8 �6 �
�� � � T� �; �< �� � �
�� T� �: �< �� � �
�
�� T� �8 �< �� � �
�
�� � T� �; �9 �� �9 �
�� T� �; �6 �� �6 ���# %�� �5 � � �
�
�(��� � T� �5 �� � �F�� T� �7 �= �� �� T� �7 �= ���� � %�� -��!� T� �6 ��&�� T� �7 �6 �� �6 �� T� �; �6 �� �6 ��� � T� �C� � �g�� %�� � ��0!� B�-�� � � U>  LPARAID _LPARA _LWIDTH MSQLSTR NRETVAL THISFORM
 CONNSTATUS
 LCSQLQUERY MX5 SQLQUERY _LNPARACOUNT LCQUERY I PID OCAPTION MX3 N2 PARANAME PCAPTION MX4 PARACAP PTYPE PARATYPE PNM ISQ	 PARAQUERY QRYID LPNM	 ADDOBJECT SX TOP1 TPNM COMPANY STA_DT END_DT REPQR QIQRY CMDNM COMMANDCLICK VCX QRY_ID PARA_NM CMDCLICK _HELP CURAAA CURBBB CPARAM
 SEARCHFLDS LN EXWORD1 EXWORD2 APATH TOP2 HEIGHT WIDTH PANEL CMDSHOWREPORT LEFT	 CMDCANCEL LINE1 TOP ALIGN�E %�C� Thisform.dtFrmDateb� O� C� Thisform.dtTodateb� O	���� %�C�  � � ���� �, ��C�  From date can not be empty...!!!�x�� ��  � � � B�-�� � %�C�  � � ���� �* ��C� To date can not be empty...!!!�x�� ��  � � � B�-�� � %��  � � �  � � ��|�; ��C�/ From Date can not be greater than to date...!!!�x�� T�  � � ��        �� ��  � � � B�-�� � � B�a�� U  THISFORM	 DTFRMDATE VALUE SETFOCUS DTTODATEi ��  � � � � 7� � T� �� ��# T� � �� Parameters for � �� T� � �� �� T� �	 �� ��+ T� �
 � ���� �� Report ID :�  ��( ��C�	 sqlconobj� sqlconnudobj� � �� %�C� mvu_nencb� U��� � T� �� �� � %�� ���W�. T� �CC� ��� Ud*yog+1993Uid� � � ��. T� �CC� ��� Ud*yog+1993Pwd� � � �� ���" T� �CC � � � � � � � ��" T� �CC � � � � � � � �� �) %��
 DATEPICKERCCC� classlibv�f���� ��� G~(�
 datepicker� � T� � ��  ��b T� �� Driver={SQL server};server=� �
 ;database=C� � �� ;uid=� � ;pwd=� � ;�� T� � �C� ��� %�� � � ����( ��C� Connection Error...!!!�@� �x�� B�-�� � T� � ���� T� �  ���� T� �! ��,�� T� �" ���� T� �# �C� �$ g�� ��C �  � �% �� T� �& �a��% ��C� _stuffObject� _stuff� � �� ��C� �' �( �� U)  LRPTID LCAPTION LRPTTYPE LRANGE LRANGEID THISFORM CAPTION ICON ICOPATH REPTTYPE PANEL PANELS TEXT	 ADDOBJECT MVU_NENC _USER	 SQLCONOBJ NEWDECRY MVU_USER _PASS MVU_PASS DEC	 ONDECRYPT
 DATEPICKER RPTID CONSTR
 MVU_SERVER COMPANY DBNAME
 CONNSTATUS VUMESS LEFT1 TOP1 LEFT2 TOP2	 BACKCOLOR VCOLOR GENPARAMETERS
 AUTOCENTER _STUFFOBJECT _OBJECTPAINT+  G2� G/� G � G(� british� G � U  BRITISHu 4 %�C�  _screen.ActiveForm.cmdshowreportb� O��n �: 12� CTRL+O�( _Screen.ActiveForm.cmdshowreport.Click()� � U  CTRL O cmdclick,     �� genparameters	    �� chk_validation:0    �� Init�1    �� Load�6    �� Activate7    ��1 �� A ��� !!A $1A 1A�q A 2!� A �	q � ��q A A r � 2q A 1A Qq A rA 2q A �� � � � � B1��AA � Q� ��� A �q � A A �� � 1�A Q� � qAAA A ��AA � !q A B6 q � q�q A a� � !���q A A �!A � B�q A � q a�q A !!�1� � QA��q �q q Qq �q �q 1q�q �q Aq �q �q ��q A ��q A � A��q �q q Qq �q �q A��q �q Aq �q �q Q�q A � 1q A q � A !A�� �A � q � Q B q � Q A B1��� �A q �q �q �q �q q �q �q q �q �q A A 1qqqA � �q A !!�!� � 1A��q �q q Qq �q �q 1q�q �q Aq �q �q ��q A ��q A � A��q �q q Qq �q �q �q A��q �q Aq �q �q Q�q A � 1q A q � A !A�� �A � q � Q A q � Q A B1��� �A q ��q q �q �q q �q �q q �q �q A A B r � � A A A � aAq� ���A ��2q11A � a�A 2q A A 3 QQ�� q A Q�� q A ���� q A A q 2 2s � 1���� A ��� !!A �� AA !1A�q A R� Q5 a a a a 3 A�A 2                       �     o   �  .>  �   �  S>  �@    �  �@  �E  !    F  dF  V    �F  �F  ^   )   �<                  0   m                   PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Class                                                                                               WINDOWS _1TL0PYYBW1291615683t      m  �            �  �          ^  k              P               COMMENT RESERVED                        A                                                                  �                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      !Arial, 0, 9, 5, 15, 12, 32, 3, 0
      cmdtest      Pixels      Class      1      commandbutton      cmdtest     ���    �   �                         �   %   �       �      �           �  U  /  T�  � � ��  � �� T�  � � ��  � �� U  THIS PARENT TAGVAL TAG CMDOBJ NAME Click,     ��1 aa4                       P       )   �                         [PROCEDURE Click
this.Parent.tagval=this.Tag
this.Parent.cmdObj=this.name



ENDPROC
      aHeight = 27
Width = 84
Caption = "Command1"
ToolTipText = "Click for Help"
Name = "cmdtest"
      commandbutton0   m                   PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Class                                                                                               WINDOWS _1O61C2TAZ 878941401n      +
  {      �  �  �  (          X  e  �          J               COMMENT RESERVED                        �                                                                   WINDOWS _1NS0MG7JU 911641408U        d      �    �r  �3          ?  L  W%          1               WINDOWS _1NS0MG7JU 879059833      �  �  �  �
  �
                                                       WINDOWS _1O403W86Q 911641408�
      �
  �
  �  8
  �    �                                               WINDOWS _1O603XLBF 863702772�      �  �  �  �  }                                                       COMMENT RESERVED                                                                                            ޷                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      ctl32_progressbar      Pixels      Class      4      control      ctl32_progressbar      TRUE     AutoSize = .T.
FontName = "Tahoma"
FontSize = 8
FontStrikethru = .F.
FontUnderline = .F.
Anchor = 7
BackStyle = 0
Caption = "ctl32_ProgressBar"
Height = 96
Left = 0
Top = 18
Width = 16
ForeColor = 0,0,128
Rotation = 90
Name = "lblControlNameV"
      ctl32_progressbar      lblControlNameV      label      label      TRUE     ���    �   �                         �   %   �       �      �           �  U  E  %��  � � � �� � B� �# T�  � � ��  � � �  � � �� U  THIS PARENT HWND VALUE STEP Timer,     ��1 qA A 22                       |       )   �                         �PROCEDURE Timer
If This.Parent.HWnd = 0 Then
  Return
Endif

This.Parent.Value = This.Parent.Value + This.Parent.Step

ENDPROC
      ctl32_progressbar      ctl32_progressbarlabel      wctl32_name
ctl32_version
ctl32_update^
ctl32_declares^
ctl32_bytestostr^
ctl32_init^
ctl32_bind^
ctl32_unbind^
      Pixels      Class      1      label      ctl32_progressbarlabel     Z_memberdata XML Metadata for customizable properties
buddycontrol Especifies the full name of the ctl32_ProgressBar control to bind this label to. For example: ThisForm.ctl32_ProgressBar1
labelstyle Especifies the Style used to display numbers in label text. N: Number, P: Percent, B: Bytes/KB/MB/GB
labelcaption Especifies the text to display in the label. Any text can be entered, keywords <<Value>> and <<Maximum>> will be replaced by the progressbar respective values.
ctl32_name
ctl32_version
*ctl32_update 
*ctl32_declares 
*ctl32_bytestostr 
*ctl32_init 
*ctl32_bind 
*ctl32_unbind 
     (FontName = "Tahoma"
FontSize = 8
Alignment = 1
BorderStyle = 0
Caption = "ctl32_ProgressBar_Label"
Height = 16
Width = 300
_memberdata = 
buddycontrol = 
labelstyle = N
labelcaption = <<Value>>
ctl32_name = ctl32_ProgressBarLabel
ctl32_version = 1.1
Name = "ctl32_progressbarlabel"
      label      gTop = 0
Left = -25
Height = 23
Width = 23
Enabled = .F.
Interval = 100
Name = "tmrControlTimer"
      tmrControlTimer      timer      timer      TRUE      �FontName = "Tahoma"
FontSize = 8
FontStrikethru = .F.
FontUnderline = .F.
Anchor = 7
BackStyle = 0
Caption = "ctl32_ProgressBar"
Height = 15
Left = 6
Top = 1
Width = 89
ForeColor = 0,0,128
Name = "lblControlNameH"
      ctl32_progressbar      lblControlNameH      label      label      control     ����    �  �                        �q   %   �      �  @   `          �  U  p %�C�  � ��� � B� �# %�C� This.LabelStyleb� C��w �6 R,:��' LabelStyle Property must be Character: Ct�� B� � �� � � � H�� ��� ��  � � N��C�1 T� �CC�  � � .Value�� 999,999,999,999_��3 T� �CC�  � � .Maximum�� 999,999,999,999_��3 T� �CC�  � � .Minimum�� 999,999,999,999_�� ��  � � P����' T� �CC�  � � .Percent�� 999%_�� T� �� 100%�� T� �� 0%�� ��  � � B��!�# T� �CC�  � � .Value��  � ��% T� �CC�  � � .Maximum��  � ��% T� �CC�  � � .Minimum��  � �� 2���1 T� �CC�  � � .Value�� 999,999,999,999_��3 T� �CC�  � � .Maximum�� 999,999,999,999_��3 T� �CC�  � � .Minimum�� 999,999,999,999_�� � T� ��  � ��) T� �C� �	 <<Value>>C� ���
����+ T� �C� � <<Maximum>>C� ���
����+ T� �C� � <<Minimum>>C� ���
���� T�  �	 �� ��
 ��  �
 � U  THIS BUDDYCONTROL LCVALUE	 LCMAXIMUM	 LCCAPTION
 LABELSTYLE	 LCMINIMUM CTL32_BYTESTOSTR LABELCAPTION CAPTION REFRESH. + |�� StrFormatByteSizeA� shlwapi���� U  STRFORMATBYTESIZEA SHLWAPI}  ��  � �� � T�� �C�dX�� ��C ��  �� C�� >� �� T�� �C�� ��� T�� �C�� C�� >�=�� B�C�� ��� U  QDW PSZBUF STRFORMATBYTESIZEA1  T�  � ��  ��
 ��  � �
 ��  � �
 ��  � � U  THIS CAPTION CTL32_DECLARES
 CTL32_BIND CTL32_UPDATE�  %�C�  � �
��| � %�C�  � b� U��J �  T�  � ��	 ThisForm.�  � �� �. ��CC�  � �� VALUE�  � CTL32_UPDATE��� � U  THIS BUDDYCONTROLE  %�C�  � �
��> �+ ��CC�  � �� VALUE�  � CTL32_UPDATE�� � U  THIS BUDDYCONTROL 
 ��  � � U  THIS
 CTL32_INIT 
 ��  � � U  THIS CTL32_UNBIND ctl32_update,     �� ctl32_declares    �� ctl32_bytestostrk    ��
 ctl32_init    ��
 ctl32_bind{    �� ctl32_unbind    �� Inits    �� Destroy�    ��1 !A A 2aA A � � R11Rq� R1QQ� 11B ���� 3 �2 q r �2�� 2 � � � 3 1qA �B 3 1�A 2 � 3 � 2                            "   <  �  -   $   �  �  3   ,   �  !  C   1   B  &	  K   8   I	  �	  V   <   �	  �	  [   >   
  
  _    )   �                       
$PROCEDURE ctl32_update
If Empty(This.BuddyControl)
  Return
Endif

If Type("This.LabelStyle") <> [C]
  WAIT ([LabelStyle Property must be Character: ] + Program()) WINDOW nowait
  Return
Endif

Local lcValue, lcMaximum, lcCaption
Do Case

Case This.LabelStyle = "N"	&& Value
  lcValue = Transform((Evaluate(This.BuddyControl + ".Value")),"999,999,999,999")
  lcMaximum = Transform((Evaluate(This.BuddyControl + ".Maximum")),"999,999,999,999")
  lcMinimum = Transform((Evaluate(This.BuddyControl + ".Minimum")),"999,999,999,999")

Case This.LabelStyle = "P"	&& Percent
  lcValue = Transform(Evaluate(This.BuddyControl + ".Percent"),"999%")
  lcMaximum = "100%"
  lcMinimum = "0%"

Case This.LabelStyle = "B"	&& Bytes
  lcValue = This.ctl32_bytestostr(Evaluate(This.BuddyControl + ".Value"))
  lcMaximum = This.ctl32_bytestostr(Evaluate(This.BuddyControl + ".Maximum"))
  lcMinimum = This.ctl32_bytestostr(Evaluate(This.BuddyControl + ".Minimum"))

Otherwise	&& same as "N"
  lcValue = Transform((Evaluate(This.BuddyControl + ".Value")),"999,999,999,999")
  lcMaximum = Transform((Evaluate(This.BuddyControl + ".Maximum")),"999,999,999,999")
  lcMinimum = Transform((Evaluate(This.BuddyControl + ".Minimum")),"999,999,999,999")

Endcase

lcCaption = This.LabelCaption
lcCaption = Strtran(lcCaption ,"<<Value>>",Alltrim(lcValue),1,10,1)
lcCaption = Strtran(lcCaption ,"<<Maximum>>",Alltrim(lcMaximum),1,10,1)
lcCaption = Strtran(lcCaption ,"<<Minimum>>",Alltrim(lcMinimum),1,10,1)

This.Caption = lcCaption
This.Refresh

ENDPROC
PROCEDURE ctl32_declares
DECLARE INTEGER StrFormatByteSizeA IN shlwapi;
	INTEGER qdw,;
	STRING @ pszBuf,;
	INTEGER uiBufSize
ENDPROC
PROCEDURE ctl32_bytestostr
LPARAMETERS qdw

LOCAL pszBuf

m.pszBuf = SPACE(100)

StrFormatByteSizeA(m.qdw, @m.pszBuf, Len(m.pszBuf))

m.pszBuf = ALLTRIM(m.pszBuf)

* Remove chr(0)
m.pszBuf = Left(m.pszBuf,Len(m.pszBuf)-1)

RETURN ALLTRIM(m.pszBuf)
ENDPROC
PROCEDURE ctl32_init
This.Caption = ""

This.ctl32_Declares
This.ctl32_Bind
This.ctl32_Update

ENDPROC
PROCEDURE ctl32_bind
If Not Empty(This.BuddyControl) Then
  If Type(This.BuddyControl) = [U] Then
    This.BuddyControl = [ThisForm.] + This.BuddyControl
  Endif

  Bindevent(Evaluate(This.BuddyControl),"VALUE",This,"CTL32_UPDATE",1)

Endif

ENDPROC
PROCEDURE ctl32_unbind
If Not Empty(This.BuddyControl) Then
  Unbindevent(Evaluate(This.BuddyControl),"VALUE",This,"CTL32_UPDATE")
Endif
ENDPROC
PROCEDURE Init
This.ctl32_Init

ENDPROC
PROCEDURE Destroy
This.ctl32_Unbind

ENDPROC
     Actl32_hwnd^
ctl32_dwexstyle^
ctl32_lpclassname^
ctl32_dwstyle^
ctl32_parenthwnd^
ctl32_hinstance^
ctl32_creating^
ctl32_name
ctl32_hmenu^
ctl32_lpparam^
ctl32_lpwindowname^
ctl32_oldstep^
ctl32_version
ctl32_hwnds^
ctl32_left^
ctl32_top^
ctl32_width^
ctl32_height^
builderx
ctl32_resize^
step_assign^
minimum_assign^
maximum_assign^
marquee_assign^
visible_assign^
ctl32_create^
ctl32_destroy^
ctl32_declaredlls^
ctl32_bindevents^
ctl32_unbindevents^
marqueespeed_assign^
hwnd_access^
value_access^
value_assign^
percent_access^
smooth_assign^
backcolor_assign^
barcolor_assign^
play_assign^
scrolling_assign^
percent_assign^
max_assign^
min_assign^
hwnd_assign^
orientation_assign^
vertical_assign^
themes_assign^
ctl32_themes^
flat_assign^
bordercolor_assign^
instatusbar_assign^
StatusBarText^
Picture^
BackStyle^
Click^
ControlCount^
Controls^
DblClick^
ColorSource^
Drag^
DragDrop^
DragIcon^
DragMode^
DragOver^
GotFocus^
LostFocus^
MiddleClick^
MouseDown^
MouseEnter^
MouseIcon^
MouseLeave^
MouseMove^
MousePointer^
MouseUp^
MouseWheel^
OLECompleteDrag^
OLEDrag^
OLEDragDrop^
OLEDragMode^
OLEDragOver^
OLEDragPicture^
OLEDropEffects^
OLEDropHasData^
OLEDropMode^
OLEGiveFeedback^
OLESetData^
OLEStartDrag^
Objects^
RightClick^
Style^
BorderWidth^
ForeColor^
AddProperty^
ActiveControl^
Draw^
Enabled^
HelpContextID^
Move^
Moved^
Refresh^
ResetToDefault^
Resize^
SaveAsClass^
SetFocus^
ShowWhatsThis^
SpecialEffect^
TabStop^
ToolTipText^
WhatsThisHelpID^
WriteExpression^
WriteMethod^
     Nctl32_hwnd CreateWindowEx return value.
ctl32_dwexstyle CreateWindowEx parameter.
ctl32_lpclassname CreateWindowEx parameter.
ctl32_dwstyle CreateWindowEx parameter.
ctl32_parenthwnd CreateWindowEx parameter.
ctl32_hinstance CreateWindowEx parameter.
ctl32_creating
minimum Specifies the lower limit of the value property. Must be a positive or negative number smaller than Maximum
maximum Specifies the upper limit of the value property. Must be a positive or negative number larger than minimum.
vertical Specifies if the progressbar is vertical or horizontal.
_memberdata XML Metadata for customizable properties
step Determines the value to use in the stepit method. Can be a positive or negative value.
marquee Especifies if the marquee style is active. When set to true, the Smooth property is set to false to avoid wrong display of bars when using XP with no themes.
ctl32_name Name of the control class
marqueespeed Specifies the speed of the marquee bar, in milliseconds.
hwnd Specifies the Window handle of the Control.
value Specifies the current value of the control.
percent Specifies the percent of the value property relative to the total of maximum - minimum. 
repeat Specifies if the controls rolls over to minimum when value reaches maximum. Use it with Play to display a self updating progressbar.
smooth Specifies if the progressbar is shown using segments, or using a continuous bar.
parenthwnd Especifies the handle of the parent window of the control.
ctl32_hmenu CreateWindowEx parameter.
ctl32_lpparam CreateWindowEx parameter.
ctl32_lpwindowname CreateWindowEx parameter.
barcolor Specifies the color of the progress bar. A value of -1 resets color to system default. Backcolor specifies the color of the background, a value of -1 resets color to system default.
play When True, fires the StepIt method every 100 milliseconds. To set the speed, change the value of the step property.
max For compatibility only. Use Maximum property instead.
min For compatibility only. Use Minimum property instead.
scrolling For compatibility only. Use Smooth property instead.
orientation For compatibility only. Use Vertical  property instead. 0: Horizontal, 1:Vertical
ctl32_oldstep Saves old Step value when the StepIt method is called with a parameter.
sizeadjust Adjusts Width/Height of Horizontal/Vertical ProgressBar so that bars show even and complete at the end/top. Use only with Themes applied in Windows XP.
themes Determines if Themes are used for the control. (Windows XP).
ctl32_version
ctl32_hwnds Static window hwnd
flat Especifies if the flat style is active.
ctl32_left
ctl32_top
ctl32_width
ctl32_height
builderx
instatusbar
ctl32_flat
ctl32_xp
*ctl32_resize Bound to Form.Resize
*step_assign 
*minimum_assign 
*maximum_assign 
*marquee_assign 
*visible_assign 
*ctl32_create 
*ctl32_destroy 
*ctl32_declaredlls DLL declarations.
*ctl32_bindevents Binds events.
*ctl32_unbindevents 
*marqueespeed_assign 
*stepit Increments the value of the control by the amount specified in step. If a numeric parameter is passed, that value is used instead of the value set in the step property.
*hwnd_access 
*value_access 
*value_assign 
*percent_access 
*smooth_assign 
*backcolor_assign 
*barcolor_assign 
*play_assign 
*scrolling_assign 
*percent_assign 
*max_assign 
*min_assign 
*hwnd_assign 
*reset Resets the Value property to the Minimum value.
*orientation_assign 
*vertical_assign 
*themes_assign 
*ctl32_themes Bound to Form.Themes
*flat_assign 
*bordercolor_assign 
*instatusbar_assign 
*repeat_assign 
*width_assign 
*height_assign 
*u_strtolong 
     >����    �>  �>                        �X(   %   �3      "<  e  X7          �  U  0  %��  � a� �  � � ��$ � B� � ���  ��)� %��� a��� �4 ��C�� � �� ��� ��� ���	 ��� �� T��
 ������ T�� ������ T�� ��� ��� T�� ���	 ��� �� T��
 ��� �� T�� ��� �� T�� ��� �� T�� ���	 �� �$ ��C�� � ��
 �� �� �� �� �� �� U  THIS CTL32_CREATING
 CTL32_HWND
 CTL32_FLAT SETWINDOWPOS CTL32_HWNDS LEFT TOP WIDTH HEIGHT
 CTL32_LEFT	 CTL32_TOP CTL32_WIDTH CTL32_HEIGHT�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� ��C� � �� � � � �� U  VNEWVAL THIS STEP SENDMESSAGEN
 CTL32_HWND�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� T� � ���  �� %�� � � � ��� � T� � �� � �� �  ��C� � �� � � � � �� U  VNEWVAL THIS MINIMUM MIN VALUE SENDMESSAGEN
 CTL32_HWND MAXIMUM�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� T� � ���  �� %�� � � � ��� � T� � �� � �� �  ��C� � �� � � � � �� U  VNEWVAL THIS MAXIMUM MAX VALUE SENDMESSAGEN
 CTL32_HWND MINIMUM ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� %�� � a��� � T� � �-�� � %�� � � ��� ��C� � �� ��C� � �� � U  VNEWVAL THIS MARQUEE PLAY
 CTL32_HWND CTL32_DESTROY CTL32_CREATEN ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� %�� � � ��� � B� � %�� � a��� ��C� � �� �� ��C� � �� �� �G� ��C� � � � �� ��C� � � � �� � U  VNEWVAL THIS VISIBLE
 CTL32_HWND SHOWWINDOWX CTL32_HWNDS� ���  ���� %��� �� � B� � T�� �a�� T�� ��� �� %��� a��� � %��� -�	 C� � ��� � T�� �a�� T�� �-�� � %��� -��� � T�� �a�� � � %��� a���� T�� �� �� T��	 �� static�� T��
 ��  �� T�� �C�
   @�	   ��� T�� �� �� T�� �C�� ���� �� T�� �� ��O T�� �C�� ��	 ��
 �� �� ��� ��� ��� ��� �� �� �� � �� %��� � ����F ��C� Error Creating Common Control � static�  Window��� �x�� � � �� � T�� �� �� T��	 �� msctls_progress32�� T��
 ��  �� T�� �C�
   @�	   ��� %��� a���� T�� ������ T�� ������ T�� ��� ��� T�� ��� ��� T�� ��� �� ��� T�� ��� �� T�� ��� �� T�� ��� �� T�� ��� �� T�� ��� �� � %��� a��#� T�� �C�� ���� � %��� a��L� T�� �C�� ���� � %��� a�	 ��  � ���� T�� �C�� ���� � T�� �� �� T�� �C�� ���� �� T�� �� ��A T��! �C�� ��	 ��
 �� �� �� �� ��  �� �� �� �� � �� %���! � ��L�A ��C� Error Creating Common Control ��	 �  Window��� �x�� � %��� ��� ��" � T�" ��  � �� %��# � -���� T�" �-�� � %�C��
]� 0���� T�" �-�� � %�C� � ���� T�" �-�� � %��" ���� ��C�  �% ���$ �� �� ��C�  �% ��  �$ �� � � T��& ���' �� T��( ���) �� T��* ���* �� T��+ ���+ �� T��, ���, �� T��- ���- �� T��. ���. �� T��/ ���/ �� T��0 ���0 �� T�� �-�� �� U1  THIS CTL32_CREATING
 CTL32_FLAT FLAT INSTATUSBAR CTL32_XP ISTHEMEACTIVE THEMES CTL32_DWEXSTYLE CTL32_LPCLASSNAME CTL32_LPWINDOWNAME CTL32_DWSTYLE CTL32_HMENU CTL32_HINSTANCE GETWINDOWLONG CTL32_PARENTHWND CTL32_LPPARAM CTL32_HWNDS CREATEWINDOWEX LEFT TOP WIDTH HEIGHT
 CTL32_NAME LNPARENTHWND
 CTL32_LEFT	 CTL32_TOP CTL32_WIDTH CTL32_HEIGHT MARQUEE SMOOTH VERTICAL ORIENTATION
 CTL32_HWND
 LUSETHEMES THISFORM SETWINDOWTHEME HWND MIN MINIMUM MAX MAXIMUM STEP VALUE MARQUEESPEED PLAY	 BACKCOLOR BARCOLOR VISIBLEC  ��C� � �  �� ��C� � �  �� T� � �� �� T� � �� �� U  DESTROYWINDOW THIS
 CTL32_HWND CTL32_HWNDS  ��  ���� � ��C��  ���� T�� �C��  ����4 %�C��  � CallWindowProc��� ���� ��� �) |�� CallWindowProc� user32������ �: %�C��  � ChildWindowFromPoint��� ���� ��� �+ |�� ChildWindowFromPoint� user32���� �4 %�C��  � CreateWindowEx��� ���� ��i�7 |�� CreateWindowEx� user32������������� �3 %�C��  � DestroyWindow��� ���� ����  |�� DestroyWindow� user32�� �3 %�C��  � GetClientRect��� ���� ���# |�� GetClientRect� user32��� �1 %�C��  � GetSysColor��� ���� ��m� |�� GetSysColor� user32�� �3 %�C��  � GetWindowLong��� ���� ����" |�� GetWindowLong� user32��� �* %�CC�JgCC�Jg�d�
ףp=
@
��O�3 %�C��  � IsThemeActive��� ���� ��K�$ |�� IsThemeActive� uxtheme.Dll� � �1 %�C��  � PostMessage��� ���� ����$ |�� PostMessage� user32����� �2 %�C��  � RedrawWindow��� ���� ���& |�� RedrawWindow� user32����� �2 %�C��  � SendMessageN��� ���� ��n�4 |�� SendMessage� user32Q� SendMessageN����� �3 %�C��  � SetWindowLong��� ���� ����$ |�� SetWindowLong� user32���� �2 %�C��  � SetWindowPos��� ���� ��*�+ |�� SetWindowPos� user32�������� �* %�CC�JgCC�Jg�d�
ףp=
@
����4 %�C��  � SetWindowTheme��� ���� ����& |�� SetWindowTheme� UxTheme���� � �1 %�C��  � ShowWindowX��� ���� ���. |��
 ShowWindow� user32Q� ShowWindowX��� � U  LADLLS LNLEN CALLWINDOWPROC USER32 CHILDWINDOWFROMPOINT CREATEWINDOWEX DESTROYWINDOW GETCLIENTRECT GETSYSCOLOR GETWINDOWLONG ISTHEMEACTIVE UXTHEME DLL POSTMESSAGE REDRAWWINDOW SENDMESSAGE SENDMESSAGEN SETWINDOWLONG SETWINDOWPOS SETWINDOWTHEME
 SHOWWINDOW SHOWWINDOWX� * ��C�  � RESIZE�  � CTL32_RESIZE���' ��C�  � TOP�  � CTL32_RESIZE���( ��C�  � LEFT�  � CTL32_RESIZE���* ��C� � THEMES�  � CTL32_THEMES��� U  THIS THISFORM�  %��  � � �� � B� �' ��C�  � RESIZE�  � CTL32_RESIZE��$ ��C�  � TOP�  � CTL32_RESIZE��% ��C�  � LEFT�  � CTL32_RESIZE��' ��C� � THEMES�  � CTL32_THEMES�� U  THIS
 CTL32_HWND THISFORM�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� ��C� � �
�� � � �� U  VNEWVAL THIS MARQUEESPEED SENDMESSAGEN
 CTL32_HWND@ ��  � �� � %�C� m.lnValb� N��; � T��  �� � �� �) %�� � -� � � ��  � � 	��l � B� �) %�� � -� � � ��  � � 	��� � B� � %���  � � ��� � T� � �� � �� T� � ���  �� �� � T� � �� �� � ��C� �
 �� � �	 �� %�� � � ��9� T� � �� � �� � U  LNVAL	 LNOLDSTEP THIS STEP REPEAT VALUE MAXIMUM MINIMUM CTL32_OLDSTEP SENDMESSAGEN
 CTL32_HWND  B��  � �� U  THIS
 CTL32_HWNDe  ��  � %�� � a��+ � T��  �� � �� �S �  T��  �C� � �� � � �� � B���  �� U  NVALUE THIS CTL32_CREATING VALUE SENDMESSAGEN
 CTL32_HWNDU ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � %�� � -��� � %���  � � ��� � B� � %���  � � ��� � B� � �� %���  � � ��� � T��  �� � �� � %���  � � ��� T��  �� � �� � � T� � ���  �� %�� � � ��N� ��C� � � ��  � � �� � U	  VNEWVAL THIS REPEAT MAXIMUM MINIMUM VALUE HWND SENDMESSAGEN
 CTL32_HWND. + B�C�d�  � �  � C�  � �  � 8�� U  THIS VALUE MINIMUM MAXIMUM�  ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� %�� � � ��� � ��C� � �� ��C� � �� � U  VNEWVAL THIS SMOOTH
 CTL32_HWND CTL32_DESTROY CTL32_CREATE�  ��  � %�C�	 m.vNewValb� N��W �3 ��C�' Parameter for BackColor must be Numeric�x�� � %���  ���� ��� � T��  ������ � %���  ������ � T��  ��
      ��A�� � T� � ���  �� ��C� � � � � � � �� B� U  VNEWVAL THIS	 BACKCOLOR SENDMESSAGEN
 CTL32_HWND�  ��  � %�C�	 m.vNewValb� N��V �2 ��C�& Parameter for BarColor must be Numeric�x�� � %���  ���� ��� � T��  ������ � %���  ������ � T��  �C�� �� � T� � ���  �� ��C� � �	� � � � �� B� U  VNEWVAL GETSYSCOLOR THIS BARCOLOR SENDMESSAGEN
 CTL32_HWND, ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � %���  a�	 � � a	��� � B� � T� � ���  �� %�� � a��� T� � �� � �� � T� � � �� � �� U  VNEWVAL THIS MARQUEE PLAY VALUE MINIMUM TMRCONTROLTIMER ENABLED�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� %�� � � ��� � T� � �-�� �� � T� � �a�� � U  VNEWVAL THIS SROLLING	 SCROLLING SMOOTH  ��  � B� U  VNEWVAL�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� T� � ���  �� U  VNEWVAL THIS MAX MAXIMUM�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� T� � ���  �� U  VNEWVAL THIS MIN MINIMUM  ��  � B� U  VNEWVAL  T�  � ��  � �� U  THIS VALUE MINIMUM�  ��  � %�C�	 m.vNewValb� N��U �- ��C� Parameter must be Numeric: Ct��x�� B� � T� � ���  �� %�� � � ��� � T� � �-�� �� � T� � �a�� � U  VNEWVAL THIS ORIENTATION VERTICAL8 ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� %�� � a��� � T� � ���� �� � T� � �� �� � %�� � � ��1� ��C� � �� ��C� � �� � U  VNEWVAL THIS VERTICAL ORIENTATION
 CTL32_HWND CTL32_DESTROY CTL32_CREATE ��  � %�� � -�� � B� � %�C�	 m.vNewValb� N��w � %���  � ��^ � T��  �-�� �s � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� %�� � � ��� � B� � ��C� � �� ��C� � �� U  VNEWVAL THIS CTL32_XP THEMES HWND CTL32_DESTROY CTL32_CREATE  T�  � �� � �� U  THIS THEMES THISFORM�  ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� %�� � � ��� � ��C� � �� ��C� � �� � U  VNEWVAL THIS FLAT
 CTL32_HWND CTL32_DESTROY CTL32_CREATEL  ��  � %���  �����3 � T��  �C� � � �^�� � T� � ���  �� U  VNEWVAL THIS BORDERCOLOR  ��  � T� � ���  �� U  VNEWVAL THIS INSTATUSBAR�  ��  � %�C�	 m.vNewValb� N��] � %���  � ��D � T��  �-�� �Y � T��  �a�� � � %�C�	 m.vNewValb� L��� �- ��C� Parameter must be Logical: Ct��x�� B� � T� � ���  �� U  VNEWVAL THIS REPEAT�  ��  � T� � ���  �� ��� �� � %��� a��{ � %��� � � �� -��w �% T�� �C�� ��� T���� � � �� U  VNEWVAL THIS WIDTH
 SIZEADJUST ORIENTATION VERTICAL�  ��  � T� � ���  �� ��� �� � %��� a��{ � %��� �� �� a��w �% T�� �C�� ��� T���� � � �� U  VNEWVAL THIS HEIGHT
 SIZEADJUST ORIENTATION VERTICAL/  4�  � T�� �C��  � 4RS���� B��� �� U 	 TCLONGSTR LNRETVAL� 4�  � T� � � ��  �� T� � � ��  �� T� � �� �� ��� ����) %�CC�JgCC�Jg�d�
ףp=
@�� � T�� �-�� �� � T�� �a�� � %�C� ThisFormb� O���c ��C�T USAGE: _Screen.Newobject("oProgressBar","ctl32_progressbar","ctl32_progressbar.vcx")��x�� B� � %��� a� C�t� 	��=� B� � ��C�� �� %�C�t� ��m� T��	 ���  �� ��� %��
 � ����� T��	 ��
 � �� ��� T� �CC�  �Q�� ��C�
 � � � �� T�� �CC� ��\�� �� T�� �CC� ��\�� �� T�� �CC� �	�\�� �� T�� �CC� ��\�� ��	 ��� �	 ��� �	 ��� �	 ��� � T�� �C�� �� �8�� T�� �C�� �� �8��! T��	 �C�
 �  ��  �� � �� � �% %��� � � Form�	 ��	 � 	���� B� �  %��
 � �9� � �� a��� T�� �-�� � %��� a���� %��� �� �� a��q�% T�� �C�� ��� T���� ���% T�� �C�� ��� T���� � � ��C��  �� ��C��! �� �� U"  TNPARENTHWND THIS LBLCONTROLNAMEH CAPTION LBLCONTROLNAMEV	 BACKSTYLE CTL32_XP INSTATUSBAR CTL32_DECLAREDLLS CTL32_PARENTHWND THISFORM
 SHOWWINDOW HWND LPRECT GETCLIENTRECT LNLEFT U_STRTOLONG LNTOP LNRIGHT LNBOTTOM LNPOINTY LNPOINTX CHILDWINDOWFROMPOINT PARENT	 BASECLASS NAME VISIBLE
 SIZEADJUST ORIENTATION VERTICAL HEIGHT WIDTH CTL32_BINDEVENTS CTL32_CREATE  ��C�  � �� U  THIS CTL32_DESTROY ctl32_resize,     �� step_assign�    �� minimum_assign�    �� maximum_assign�    �� marquee_assign�    �� visible_assignN    �� ctl32_create�    �� ctl32_destroy�    �� ctl32_declaredlls8    �� ctl32_bindevents|    �� ctl32_unbindevents6    �� marqueespeed_assign    �� stepit�    �� hwnd_access~    �� value_access�    �� value_assignK    �� percent_access�    �� smooth_assignE    �� backcolor_assign�    �� barcolor_assign�     �� play_assign�!    �� scrolling_assignF#    �� percent_assign $    ��
 max_assign;$    ��
 min_assign�$    �� hwnd_assign}%    �� reset�%    �� orientation_assign�%    �� vertical_assign�&    �� themes_assign'(    �� ctl32_themes�)    �� flat_assign�)    �� bordercolor_assign�*    �� instatusbar_assigna+    �� repeat_assign�+    �� width_assign�,    �� height_assignC-    �� u_strtolong.    �� InitO.    �� Destroy�3    ��1 A A � F� � 11� � � � � A GB 4 t ��A A "�3 t ��A A "!s1A 3 t ��A A "!s1A 5 q �1� � � A A ��A A ""� A C� � A 3 v �1� � � A A ��A A "BA A "11� 11A 4 p� � A A � � �� � A � A A � A� �� q� �#aA B s � �� �� � 11� � � � � A QA QA �QA � r� #A � q "� A R� A "� A � !� AA B � � � � � � � � � � B 3 3 � QB�A ��A B}A 2A 23A �A 2#A �1AA A EA "eA "EA 2DA "�A �AdA A �A 3 �q��5 BA A rAQq6 s ��A A "�5 t r �!A �B A �B A c1!� A �C1A : � 3 t #!� A � 2 t ��A A "bA A bA A � b!A b!A B "C�A 7 �5 q �1� � � A A ��A A "C� � A 4 z �1A bA BqA "�C 3 z �!A bA B1A "�B 3 q �1� � � A A ��A A �A A ""1A b3 q ��A A "B� � � A 3 q A 2 q ��A A "!3 q ��A A "!2 q A 2 14 q ��A A "B� � � A 4 q �1� � � A A ��A A ""� A C� � A 3 q "A A �1� � � A A ��A A "BA A � � 4 12 t �1� � � A A ��A A "B� � A 3 q B�A "3 q "3 q �1� � � A A ��A A "3 q "� �QA A A 3 q "� �QA A A 3 r �� 3 w 21� �� � � A �1A A �A A � � A� CA����� � � � ��A A RA A � A �Q� QA A � � B 4 � 4                       n        �  �  ,      �  �  =   (   �  �  T   4   	  �
  m   I     �  �   a   �  "  �   �   )"  �"  �  �   �"  �.  �  �   /  �/    �   �/  �0      )1  Q2  ,    n2  66  =  #  X6  �6  l  %  �6  8  q  -  .8  �:  �  F  �:  %;  �  H  I;  �<  �  Z  =  d?  �  h  �?  �A  �  v  �A  �C    �  �C  �D  +  �  �D  E  <  �  3E  �E  @  �  F  �F  L  �  �F  �F  W  �  G  0G  [  �  YG  XH  `  �  ~H  J  r  �  �J  sL  �  �  �L  �L  �  �  �L  �N  �  �  �N  +O  �    TO  �O  �    �O  �P  �    !Q   R  �    DR  FS     '  hS  �S    +  T  %[    c  C[  ][  h   )   �>                       [hPROCEDURE ctl32_resize
* If we are in the Control Init Stage, or
* we do not have a handle to the Control yet, just return:
If This.ctl32_Creating = .T. Or This.Ctl32_hWnd = 0 Then
  Return
Endif

* Else, resize the Control Window to its container size:

#Define SWP_NOZORDER			0x4

With This

  If .ctl32_Flat = .T. Then
    SetWindowPos(.Ctl32_hWnds, 0,;
      .Left + 1, ;
      .Top + 1, ;
      .Width - 2, ;
      .Height - 2, ;
      SWP_NOZORDER)
      
    .ctl32_Left = -2
    .ctl32_Top = -2
    .ctl32_Width = .Width + 2
    .ctl32_Height = .Height + 2
  Else
    .ctl32_Left = .Left
    .ctl32_Top = .Top
    .ctl32_Width = .Width
    .ctl32_Height = .Height
  Endif .ctl32_Flat = .T.

  SetWindowPos(.Ctl32_hWnd, 0,;
    .ctl32_Left, ;
    .ctl32_Top, ;
    .ctl32_Width, ;
    .ctl32_Height, ;
    SWP_NOZORDER)

Endwith


ENDPROC
PROCEDURE step_assign
#Define WM_USER					0x400
#Define PBM_SETSTEP				(WM_USER+4)

LPARAMETERS vNewVal

If type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

THIS.Step = m.vNewVal

* Set Step Value
SendMessageN(This.ctl32_hwnd, PBM_SETSTEP , THIS.Step, 0)

ENDPROC
PROCEDURE minimum_assign
#Define WM_USER					0x400
#Define PBM_SETRANGE32			(WM_USER+6)

Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.Minimum = m.vNewVal
This.Min = m.vNewVal

* If actual Value is less than new Minimum, set value to new Minimum
If This.Value < This.Minimum Then
  This.Value =  This.Minimum
Endif

* Set Minimum and Maximum values:
SendMessageN(This.ctl32_hwnd, PBM_SETRANGE32, This.Minimum, This.maximum)

ENDPROC
PROCEDURE maximum_assign
#Define WM_USER					0x400
#Define PBM_SETRANGE32			(WM_USER+6)

Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.Maximum = m.vNewVal
This.Max = m.vNewVal

* If actual Value is greater than new Maximum, set value to new Maximum
If This.Value > This.Maximum Then
  This.Value =  This.Maximum
Endif

* Set Minimum and Maximum values:
SendMessageN(This.ctl32_hwnd, PBM_SETRANGE32, This.Minimum, This.Maximum)



ENDPROC
PROCEDURE marquee_assign
Lparameters vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
ENDIF

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

This.Marquee = m.vNewVal

If This.Marquee = .T. Then
  This.Play = .F.
Endif

* Marquee change needs to recreate Control
If This.ctl32_hwnd <> 0 Then
  This.ctl32_Destroy()
  This.ctl32_Create()
Endif

ENDPROC
PROCEDURE visible_assign
#Define SW_HIDE					0
#Define SW_SHOW					5
#Define SW_SHOWNA				8
#Define SW_SHOWDEFAULT			10

Lparameters vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
ENDIF

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

This.Visible = m.vNewVal

If This.ctl32_HWnd = 0 Then
  Return
ENDIF

If This.Visible  = .T. Then
  ShowWindowX(This.Ctl32_HWnds, SW_SHOWNA)
  ShowWindowX(This.Ctl32_HWnd, SW_SHOWNA)
Else
  ShowWindowX(This.Ctl32_HWnds, SW_HIDE)
  ShowWindowX(This.Ctl32_HWnd, SW_HIDE)
Endif


ENDPROC
PROCEDURE ctl32_create
#Define WS_EX_CLIENTEDGE		0x200
#Define WS_EX_WINDOWEDGE		0x100
#Define WS_EX_OVERLAPPEDWINDOW	Bitor(WS_EX_WINDOWEDGE, WS_EX_CLIENTEDGE)
#Define WS_EX_STATICEDGE	0x20000

#Define WS_CHILD				0x40000000
#Define WS_VISIBLE				0x10000000
#Define WS_CLIPSIBLINGS			0x4000000
#Define WS_BORDER				0x800000

#Define WM_NCPAINT				0x85

#Define GWL_HINSTANCE			-6
#Define GWL_EXSTYLE				-20
#Define GWL_STYLE				-16

#Define PBS_SMOOTH				0x1			&& Comctl32.dll Version 4.7 or later
#Define PBS_VERTICAL			0x4			&& Comctl32.dll Version 4.7 or later
#Define PBS_MARQUEE				0x8			&& Comctl32.dll version 6

#Define WM_USER					0x400
#Define CCM_FIRST				0x2000
#Define CCM_SETBKCOLOR			(CCM_FIRST + 1)

#Define PBM_DELTAPOS			(WM_USER+3)
#Define PBM_GETPOS				(WM_USER+8)
#Define PBM_GETRANGE			(WM_USER+7)
#Define PBM_SETBARCOLOR			(WM_USER+9)
#Define PBM_SETBKCOLOR			CCM_SETBKCOLOR
#Define PBM_SETPOS				(WM_USER+2)
#Define PBM_SETRANGE			(WM_USER+1)
#Define PBM_SETRANGE32			(WM_USER+6)
#Define PBM_SETSTEP				(WM_USER+4)
#Define PBM_STEPIT				(WM_USER+5)
#Define PBM_SETMARQUEE  		(WM_USER+10)

#Define SW_HIDE					0
#Define SW_SHOW					5
#Define SW_SHOWNA				8

* START Version 1.2
#Define HWND_TOP				0
#Define SWP_NOMOVE				0x2
#Define SWP_NOSIZE				0x1
* END Version 1.2

#Define SW_SHOWDEFAULT			10

#Define COLOR_HIGHLIGHT         13
#Define COLOR_BTNFACE           15

#Define PS_SOLID				0
#Define COLOR_WINDOW            5
#Define COLOR_BTNFACE           15

With This

	If .ctl32_Creating Then
		Return
	Endif

	* We enter Initialization Stage... (checked by ctl32_Resize)
	.ctl32_Creating = .T.

	* If Win98 or Themes off, set flat for statusbar
	.ctl32_Flat = .Flat
	If .InStatusBar = .T. Then
		If .ctl32_XP = .F. Or isThemeActive() = 0 Then
			.ctl32_Flat = .T.
			.Themes = .F.
		Endif
		If .Themes = .F. Then
			.ctl32_Flat = .T.
		Endif
	Endif

	* Create Static window to hold progressbar if needed
	If .ctl32_Flat = .T. Then
		*Define parameters for static createwindowex:
		.ctl32_dwExStyle = 0
		.ctl32_lpClassName = [static]
		.ctl32_lpWindowName = ""
		.ctl32_dwStyle = Bitor(WS_CHILD, WS_CLIPSIBLINGS)

		.ctl32_hMenu = 0
		.ctl32_hInstance = GetWindowLong(.ctl32_ParentHWnd, GWL_HINSTANCE)
		.ctl32_lpParam = 0

		.ctl32_hwnds = CreateWindowEx( ;
			.ctl32_dwExStyle, ;
			.ctl32_lpClassName, ;
			.ctl32_lpWindowName, ;
			.ctl32_dwStyle, ;
			.Left + 1, .Top + 1, .Width - 2, .Height - 2,;
			.ctl32_ParentHWnd,;
			.ctl32_hMenu, ;
			.ctl32_hInstance, ;
			.ctl32_lpParam)

		* If the handle to the Control is 0 then we have a problem!
		If .ctl32_hwnds = 0
			Messagebox([Error Creating Common Control ] + [static] + [ Window], 0+16, .ctl32_name)
		Endif

	Endif

	* Define parameters for progressbar createwindowex:
	Local lnParentHWnd

	.ctl32_dwExStyle = 0
	.ctl32_lpClassName = [msctls_progress32]
	.ctl32_lpWindowName = ""
	.ctl32_dwStyle = Bitor(WS_CHILD, WS_CLIPSIBLINGS)

	If .ctl32_Flat = .T. Then
		.ctl32_Left = -2
		.ctl32_Top = -2
		.ctl32_Width = .Width + 2
		.ctl32_Height = .Height + 2
		m.lnParentHWnd = .ctl32_hwnds
	Else
		.ctl32_Left = .Left
		.ctl32_Top = .Top
		.ctl32_Width = .Width
		.ctl32_Height = .Height
		m.lnParentHWnd = .ctl32_ParentHWnd
	Endif .ctl32_Flat = .T.

	* Setup Control specific Styles:
	* Marquee
	If .Marquee = .T. Then
		.ctl32_dwStyle = Bitor(.ctl32_dwStyle, PBS_MARQUEE)
	Endif

	* Smooth
	If .Smooth = .T.
		.ctl32_dwStyle = Bitor(.ctl32_dwStyle, PBS_SMOOTH)
	Endif

	* Orientation
	If .Vertical = .T. Or .Orientation <> 0 Then
		.ctl32_dwStyle = Bitor(.ctl32_dwStyle, PBS_VERTICAL)
	Endif

	.ctl32_hMenu = 0

	.ctl32_hInstance = GetWindowLong(.ctl32_ParentHWnd, GWL_HINSTANCE)

	.ctl32_lpParam = 0

	.ctl32_hwnd = CreateWindowEx( ;
		.ctl32_dwExStyle, ;
		.ctl32_lpClassName, ;
		.ctl32_lpWindowName, ;
		.ctl32_dwStyle, ;
		.ctl32_Left, .ctl32_Top, .ctl32_Width, .ctl32_Height, ;
		m.lnParentHWnd,;
		.ctl32_hMenu, ;
		.ctl32_hInstance, ;
		.ctl32_lpParam)

	* If the handle to the Control is 0 then we have a problem!
	If .ctl32_hwnd = 0
		Messagebox([Error Creating Common Control ] + .ctl32_lpClassName + [ Window], 0+16, .ctl32_name)
	Endif

	* Set Theme
	If .ctl32_XP Then
		Local lUseThemes

		lUseThemes = This.Themes

		If Thisform.Themes = .F. Then
			lUseThemes = .F.
		Endif

		If Sys(2700) = "0" Then
			lUseThemes = .F.
		Endif

		If isThemeActive() = 0 Then
			lUseThemes = .F.
		Endif

		If lUseThemes Then
			SetWindowTheme(This.HWnd, Null, Null)
		Else
			SetWindowTheme(This.HWnd, Null, "")
		Endif

	Endif

	* Set Control Minimum and Maximum values:
	.Min = .Minimum
	.Max = .Maximum

	* Set Control Step Value
	.Step = .Step

	* Set Control Value to the Container Value property
	.Value = .Value

	* Set MarqueeSpeed Value
	.MarqueeSpeed = .MarqueeSpeed

	* Set Play state
	.Play = .Play

	* Set Colors
	.BackColor = .BackColor
	.BarColor = .BarColor

	* Set Visible state
	.Visible = .Visible

	* We finish Initialization State
	.ctl32_Creating = .F.

Endwith

ENDPROC
PROCEDURE ctl32_destroy

* Release Control:
DestroyWindow(This.Ctl32_HWnd)
DestroyWindow(This.Ctl32_HWnds)

This.Ctl32_HWnd = 0
This.Ctl32_HWnds = 0

ENDPROC
PROCEDURE ctl32_declaredlls
Local laDLLs[1], lnLen

Adlls( laDLLs )
m.lnLen = Alen( laDLLs, 1 )

If Ascan( laDLLs, "CallWindowProc", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer CallWindowProc In user32 ;
    INTEGER lpPrevWndFunc,;
    INTEGER HWnd,;
    INTEGER msg,;
    INTEGER wParam,;
    INTEGER Lparam
ENDIF

If Ascan( laDLLs, "ChildWindowFromPoint", 1, m.lnLen , 1, 15 ) = 0
	Declare Integer ChildWindowFromPoint In user32 ;
		INTEGER hWndParent,;
		INTEGER px,;
		INTEGER py
Endif

If Ascan( laDLLs, "CreateWindowEx", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer CreateWindowEx In user32 ;
    INTEGER dwExStyle,;
    STRING lpClassName,;
    STRING lpWindowName,;
    INTEGER dwStyle,;
    INTEGER x,;
    INTEGER Y,;
    INTEGER nWidth,;
    INTEGER nHeight,;
    INTEGER hWndParent,;
    INTEGER hMenu,;
    INTEGER hInstance,;
    INTEGER lpParam
Endif

If Ascan( laDLLs, "DestroyWindow", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer DestroyWindow In user32 ;
    INTEGER HWnd
Endif

If Ascan( laDLLs, "GetClientRect", 1, m.lnLen , 1, 15 ) = 0
	Declare Integer GetClientRect In user32 ;
		INTEGER HWnd,;
		STRING @ lpRect
Endif

If Ascan( laDLLs, "GetSysColor", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer GetSysColor In user32 ;
    INTEGER nIndex
Endif

If Ascan( laDLLs, "GetWindowLong", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer GetWindowLong In user32 ;
    INTEGER HWnd, ;
    INTEGER nIndex
Endif

If Not Val(Os(3)) + Val(Os(4))/100 < 5.01 Then
	If Ascan( laDLLs, "IsThemeActive", 1, m.lnLen , 1, 15 ) = 0
		Declare Integer IsThemeActive In uxtheme.Dll
	Endif
ENDIF

If Ascan( laDLLs, "PostMessage", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer PostMessage In user32 ;
    INTEGER HWnd,;
    INTEGER Msg,;
    INTEGER wParam,;
    INTEGER Lparam
Endif

If Ascan( laDLLs, "RedrawWindow", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer RedrawWindow In user32 ;
    INTEGER HWnd,;
    STRING @ lprcUpdate,;
    INTEGER hrgnUpdate,;
    INTEGER fuRedraw
Endif

If Ascan( laDLLs, "SendMessageN", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer SendMessage In user32 as SendMessageN;
    INTEGER HWnd,;
    INTEGER Msg,;
    INTEGER wParam,;
    INTEGER Lparam
Endif

If Ascan( laDLLs, "SetWindowLong", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer SetWindowLong In user32 ;
    INTEGER HWnd,;
    INTEGER nIndex,;
    INTEGER dwNewLong
Endif

If Ascan( laDLLs, "SetWindowPos", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer SetWindowPos In user32 ;
    INTEGER HWnd,;
    INTEGER hWndInsertAfter,;
    INTEGER x,;
    INTEGER Y,;
    INTEGER cx,;
    INTEGER cy,;
    INTEGER wFlags
Endif

If NOT Val(Os(3)) + Val(Os(4))/100 < 5.01 Then
  If Ascan( laDLLs, "SetWindowTheme", 1, m.lnLen , 1, 15 ) = 0
    Declare Integer SetWindowTheme In UxTheme ;
      INTEGER HWnd,;
      String pszSubAppName,;
      String pszSubIdList
  Endif
Endif

If Ascan( laDLLs, "ShowWindowX", 1, m.lnLen , 1, 15 ) = 0
  Declare Integer ShowWindow In user32 As ShowWindowX ;
    INTEGER HWnd,;
    INTEGER nCmdShow
Endif

ENDPROC
PROCEDURE ctl32_bindevents

Bindevent(This, [RESIZE], This, [CTL32_RESIZE],1)
Bindevent(This, [TOP], This, [CTL32_RESIZE],1)
Bindevent(This, [LEFT], This, [CTL32_RESIZE],1)
Bindevent(Thisform,[THEMES],This,[CTL32_THEMES],1)



ENDPROC
PROCEDURE ctl32_unbindevents

If This.ctl32_HWnd = 0 Then
  Return
Endif

Unbindevent(This, [RESIZE], This, [CTL32_RESIZE])
Unbindevent(This, [TOP], This, [CTL32_RESIZE])
Unbindevent(This, [LEFT], This, [CTL32_RESIZE])
Unbindevent(Thisform,[THEMES],This,[CTL32_THEMES])




ENDPROC
PROCEDURE marqueespeed_assign
#Define PBS_MARQUEE				0x8			&& Comctl32.dll version 6

Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.MarqueeSpeed = m.vNewVal

SendMessageN(This.Ctl32_HWnd, PBM_SETMARQUEE, 1, This.MarqueeSpeed)



ENDPROC
PROCEDURE stepit
#Define WM_USER					0x400
#Define PBM_STEPIT				(WM_USER+5)

Lparameters lnVal

Local lnOldStep

* If no numeric parameter, use actual step value:
If Type("m.lnVal") <> "N"
  m.lnVal = This.Step
Endif

If This.Repeat = .F. And This.Value + m.lnVal > This.Maximum Then
*  This.Value = This.Maximum
  Return
Endif

If This.Repeat = .F. And This.Value + m.lnVal < This.Minimum Then
*  This.Value = This.Minimum
  Return
Endif

* If parameter is different from actual step value:
If m.lnVal <> This.Step Then
  This.ctl32_OldStep = This.Step
  This.Step = m.lnVal
Else
  This.ctl32_OldStep = 0
Endif

* Send StepIt message:
SendMessageN(This.ctl32_hwnd, PBM_STEPIT, 0, 0)

*Reset Step Value if old value saved:
If This.ctl32_OldStep <> 0 Then
  This.Step = This.ctl32_OldStep
Endif

* Update Container Value Property with the position property of Control,
* forcing Access and Assign Events to fire:
*This.Value = This.Value




ENDPROC
PROCEDURE hwnd_access
* Returns the HWnd of the Control
RETURN This.Ctl32_HWnd

ENDPROC
PROCEDURE value_access
#Define WM_USER					0x400
#Define PBM_GETPOS				(WM_USER+8)

Local nValue

* If setting up Control, use Value of Container, not Value of Control
If This.ctl32_Creating = .T. Then
  m.nValue = This.Value
Else
  * Ask Control for Value to return:
  m.nValue = SendMessageN(This.ctl32_hwnd, PBM_GETPOS, 0, 0)
Endif

Return m.nValue
ENDPROC
PROCEDURE value_assign
#Define WM_USER					0x400
#Define PBM_SETPOS				(WM_USER+2)

Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

If This.Repeat = .F.

  If m.vNewVal > This.Maximum Then
    Return
  Endif

  If m.vNewVal < This.Minimum Then
    Return
  Endif

Else

  If m.vNewVal > This.Maximum Then
    m.vNewVal = This.Minimum
  Endif

  If m.vNewVal < This.Minimum Then
    m.vNewVal = This.Maximum
  Endif

Endif

This.Value = m.vNewVal


If This.HWnd # 0 Then
  SendMessageN(This.ctl32_hwnd, PBM_SETPOS, m.vNewVal, 0)
Endif





ENDPROC
PROCEDURE percent_access
Return INT(100 * (This.Value - This.Minimum) / (ABS(This.Maximum - This.Minimum)))



ENDPROC
PROCEDURE smooth_assign
Lparameters vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
ENDIF

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

This.Smooth = m.vNewVal

* Smooth change needs to recreate Control
If This.ctl32_hwnd <> 0 Then
  This.ctl32_destroy()
  This.ctl32_Create()
Endif


ENDPROC
PROCEDURE backcolor_assign
#Define WM_USER					0x400
#Define CCM_FIRST				0x2000
#Define CCM_SETBKCOLOR			(CCM_FIRST + 1)
#DEFINE CLR_DEFAULT				0xFF000000
#Define PBM_SETBARCOLOR			(WM_USER+9)
#Define PBM_SETBKCOLOR			CCM_SETBKCOLOR

#Define COLOR_BTNFACE           15

Lparameters vNewVal

If Type("m.vNewVal") <> [N]
	Messagebox([Parameter for BackColor must be Numeric])
Endif

If m.vNewVal > 16777215 Then
	m.vNewVal = -1
Endif

If m.vNewVal = -1 Then
m.vNewVal = clr_default
Endif

This.BackColor= m.vNewVal


SendMessageN(This.Ctl32_HWnd, PBM_SETBKCOLOR, 0, This.BackColor)


Return

ENDPROC
PROCEDURE barcolor_assign
#Define WM_USER					0x400
#Define CCM_FIRST				0x2000
#Define CCM_SETBKCOLOR			(CCM_FIRST + 1)

#Define PBM_SETBARCOLOR			(WM_USER+9)
#Define PBM_SETBKCOLOR			CCM_SETBKCOLOR

#Define COLOR_HIGHLIGHT         13

Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter for BarColor must be Numeric])
Endif

If m.vNewVal > 16777215 Then
  m.vNewVal = -1
Endif

If m.vNewVal = -1 Then
  m.vNewVal = GetSysColor(COLOR_HIGHLIGHT)
Endif

This.BarColor= m.vNewVal

SendMessageN(This.Ctl32_HWnd, PBM_SETBARCOLOR, 0, This.BarColor)

Return

ENDPROC
PROCEDURE play_assign
Lparameters vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
Endif

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

If m.vNewVal = .T. And This.Marquee = .T. Then
  Return
Endif

This.Play = m.vNewVal

If This.Play = .T. Then
  This.Value = This.Minimum
Endif

This.tmrControlTimer.Enabled = This.Play

ENDPROC
PROCEDURE scrolling_assign
Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.Srolling = m.vNewVal

If This.Scrolling = 0 Then
  This.Smooth = .F.
Else
  This.Smooth = .T.
Endif

ENDPROC
PROCEDURE percent_assign
LPARAMETERS vNewVal
RETURN
ENDPROC
PROCEDURE max_assign
Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.Max = m.vNewVal
This.Maximum = m.vNewVal

ENDPROC
PROCEDURE min_assign
LPARAMETERS vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.Min = m.vNewVal
This.Minimum = m.vNewVal
ENDPROC
PROCEDURE hwnd_assign
LPARAMETERS vNewVal
RETURN
ENDPROC
PROCEDURE reset
This.Value = This.Minimum


ENDPROC
PROCEDURE orientation_assign
Lparameters vNewVal

If Type("m.vNewVal") <> [N]
  Messagebox([Parameter must be Numeric: ] + Program(), 16)
  Return
Endif

This.Orientation = m.vNewVal

If This.Orientation = 0 Then
  This.Vertical = .F.
Else
  This.Vertical = .T.
Endif


ENDPROC
PROCEDURE vertical_assign
Lparameters vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
ENDIF

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

This.Vertical = m.vNewVal

If This.Vertical = .T. Then
  This.Orientation = 1
Else
  This.Orientation = 0
Endif

* Vertical change needs to recreate Control
If This.ctl32_hwnd <> 0 Then
  This.ctl32_destroy()
  This.ctl32_Create()
Endif

ENDPROC
PROCEDURE themes_assign
Lparameters vNewVal

If This.ctl32_XP = .F.
	Return
Endif

If Type("m.vNewVal") = [N] Then
	If m.vNewVal = 0 Then
		m.vNewVal = .F.
	Else
		m.vNewVal = .T.
	Endif
Endif

If Type("m.vNewVal") <> [L] Then
	Messagebox([Parameter must be Logical: ] + Program(), 16)
	Return
Endif

This.Themes = m.vNewVal

If This.HWnd = 0 Then
	Return
Endif

* Window is recreated, or artifacts remain in border:
This.ctl32_Destroy()
This.ctl32_Create()


ENDPROC
PROCEDURE ctl32_themes
This.Themes = ThisForm.Themes
ENDPROC
PROCEDURE flat_assign
#Define COLOR_WINDOW            5
#Define COLOR_BTNFACE           15

Lparameters vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
ENDIF

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

This.Flat = m.vNewVal

If This.ctl32_hwnd <> 0 Then
  This.ctl32_destroy()
  This.ctl32_Create()
Endif

ENDPROC
PROCEDURE bordercolor_assign
LPARAMETERS vNewVal

If m.vNewVal = -1 Then
  m.vNewVal = RGB(0,0,0)
Endif

THIS.BorderColor = m.vNewVal

ENDPROC
PROCEDURE instatusbar_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
THIS.InStatusBar = m.vNewVal

ENDPROC
PROCEDURE repeat_assign
LPARAMETERS vNewVal

If Type("m.vNewVal") = [N] Then
  If m.vNewVal = 0 Then
    m.vNewVal = .F.
  Else
    m.vNewVal = .T.
  Endif
ENDIF

If Type("m.vNewVal") <> [L] Then
  Messagebox([Parameter must be Logical: ] + Program(), 16)
  Return
Endif

THIS.Repeat = m.vNewVal

ENDPROC
PROCEDURE width_assign
Lparameters vNewVal
*To do: Modify this routine for the Assign method
This.Width = m.vNewVal

With This
	If .SizeAdjust = .T. Then
		If .Orientation = 0 Or .Vertical = .F. Then
			.Width = Round((.Width - 5)/8,0) * 8 + 5
		Endif
	Endif
Endwith

ENDPROC
PROCEDURE height_assign
Lparameters vNewVal
*To do: Modify this routine for the Assign method
This.Height = m.vNewVal

With This
	If .SizeAdjust = .T. Then
		If .Orientation = 1 Or .Vertical = .T. Then
			.Height = Round((.Height - 8)/8,0) * 8 + 5
		Endif
	Endif
Endwith

ENDPROC
PROCEDURE u_strtolong
* This function converts a String to a Long
Parameters tcLongStr

m.lnRetval = CToBin(m.tcLongStr,[4RS])

Return m.lnRetval

ENDPROC
PROCEDURE Init
*	Ctl32_ProgressBar
*	Control creado por Carlos Alloatti - calloatti@gmail.com
*	Utiliza funciones API de Windows
*	Probado con Windows XP, 98 y VFP 9
*	Versi�n  1.00 - 2005-12-01

Parameters tnparenthwnd

This.lblControlNameH.Caption = ""
This.lblControlNameV.Caption = ""
This.BackStyle = 0

With This

	If Val(Os(3)) + Val(Os(4))/100 < 5.01
		.ctl32_XP = .F.
	Else
		.ctl32_XP = .T.
	Endif

	If Type([ThisForm]) <> [O] Then
		Messagebox([USAGE: _Screen.Newobject("oProgressBar","ctl32_progressbar","ctl32_progressbar.vcx")],16)
		Return
	Endif

	If .InStatusBar = .T. And Pcount() = 0 Then
		Return
	Endif

	.ctl32_declaredlls()
	
	If Pcount() > 0 Then
		.ctl32_Parenthwnd = m.tnparenthwnd
	Else
		If Thisform.ShowWindow < 2 Then
			.ctl32_Parenthwnd = Thisform.HWnd
		Else
* find hwnd of "screen" of top level form:

			lpRect = Replicate(Chr(0), 16)
			GetClientRect(ThisForm.HWnd, @lpRect)

			m.lnLeft   = .u_StrToLong(Substr(lpRect,  1,4))
			m.lnTop    = .u_StrToLong(Substr(lpRect,  5,4))
			m.lnRight  = .u_StrToLong(Substr(lpRect,  9,4))
			m.lnBottom = .u_StrToLong(Substr(lpRect, 13,4))
			m.lnLeft
			m.lnTop
			m.lnRight
			m.lnBottom

			m.lnPointy = Int((m.lnBottom - m.lnTop) / 2)
			m.lnPointx = Int((m.lnRight - m.lnLeft) / 2)
			.ctl32_Parenthwnd= ChildWindowFromPoint(ThisForm.HWnd, m.lnPointx, m.lnPointy)
		Endif
	Endif

	If .Parent.BaseClass <> "Form" And .ctl32_Parenthwnd = 0 Then
		Return
	Endif

	If Thisform.Name = _Screen.Name  Or .InStatusBar = .T. Then
		.Visible = .F.
	Endif

	If .SizeAdjust = .T. Then
		If .Orientation = 1 Or .Vertical = .T. Then
			.Height = Round((.Height - 8)/8,0) * 8 + 5
		Else
			.Width = Round((.Width - 5)/8,0) * 8 + 5
		Endif
	Endif

	.ctl32_BindEvents()
	.ctl32_Create()

Endwith


ENDPROC
PROCEDURE Destroy
This.Ctl32_Destroy()



ENDPROC
     �Width = 301
Height = 18
ForeColor = 0,0,0
ctl32_hwnd = 0
ctl32_dwexstyle = 0
ctl32_dwstyle = 0
ctl32_parenthwnd = 0
ctl32_hinstance = 0
minimum = 0
maximum = 100
_memberdata =     3003<VFPData><memberdata name="vertical" type="property" display="Vertical" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Vertical = NOT gObj(1).Vertical&#xA;&#xA;lnWidth = gObj(1).Height&#xA;lnHeight = gObj(1).Width&#xA;&#xA;gObj(1).Width = lnWidth&#xA;gObj(1).Height = lnHeight&#xA;" favorites="True"/><memberdata name="builderx" type="property" display="BuilderX" script="do home() + &quot;wizards\ctl32_progressbar_builder.app&quot;"/><memberdata name="flat" type="property" display="Flat" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Flat = NOT gObj(1).Flat" favorites="True"/><memberdata name="marquee" type="property" display="Marquee" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Marquee = NOT gObj(1).Marquee" favorites="True"/><memberdata name="marqueespeed" type="property" display="MarqueeSpeed" favorites="True"/><memberdata name="maximum" type="property" display="Maximum" favorites="True"/><memberdata name="minimum" type="property" display="Minimum" favorites="True"/><memberdata name="orientation" type="property" display="Orientation"/><memberdata name="parenthwnd" type="property" display="ParenthWnd"/><memberdata name="percent" type="property" display="Percent"/><memberdata name="play" type="property" display="Play" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Play = NOT gObj(1).Play" favorites="True"/><memberdata name="scrolling" type="property" display="Scrolling"/><memberdata name="sizeadjust" type="property" display="SizeAdjust" favorites="True" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).SizeAdjust = NOT gObj(1).SizeAdjust"/><memberdata name="smooth" type="property" display="Smooth" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Smooth = NOT gObj(1).Smooth"/><memberdata name="stepit" type="method" display="StepIt"/><memberdata name="builderx" type="property" display="Builderx"/><memberdata name="hwnd" type="property" display="Hwnd"/><memberdata name="step" type="property" display="Step"/><memberdata name="value" type="property" display="Value"/><memberdata name="repeat" type="property" display="Repeat" favorites="True" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Repeat = NOT gObj(1).Repeat"/><memberdata name="reset" type="method" display="Reset"/><memberdata name="max" type="property" display="Max"/><memberdata name="min" type="property" display="Min"/><memberdata name="themes" type="property" display="Themes" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).Themes = NOT gObj(1).Themes" favorites="True"/><memberdata name="barcolor" type="property" display="BarColor" script="gnobject = Aselobj(gObj)&#xA;&#xA;lnColor = Getcolor(gObj(1).Barcolor)&#xA;&#xA;If lnColor = -1 Then&#xA;  Return&#xA;Endif&#xA;&#xA;gObj(1).Barcolor = lnColor&#xA;" favorites="True"/><memberdata name="backcolor" type="property" favorites="True"/><memberdata name="instatusbar" type="property" display="InStatusBar" script="gnobject = ASELOBJ(gObj)&#xA;&#xA;gObj(1).InStatusBar = NOT gObj(1).InStatusBar "/></VFPData>
step = 1
ctl32_name = ctl32_progressbar
marqueespeed = 100
hwnd = 0
value =     0.00000
percent = 0
parenthwnd = 0
ctl32_hmenu = 0
ctl32_lpparam = 0
ctl32_lpwindowname = ProgressBar
barcolor = -1
max = 0
min = 0
scrolling = 0
orientation = 0
ctl32_oldstep = 0
themes = .T.
ctl32_version = 2.0
ctl32_hwnds = 0
ctl32_left = 0
ctl32_top = 0
ctl32_width = 0
ctl32_height = 0
builderx = (home() + "wizards\ctl32_progressbar.app")
instatusbar = .F.
ctl32_flat = .F.
ctl32_xp = .F.
Name = "ctl32_progressbar"
    %           �  �   �  �}�L�   �Y ��  � � �� � � � T� ��  �� T� �� NO�� T� �� 10.0.0.0�� ��{ �( T� �C� DADOSREPORT � CC�]��� �� �(� �� � T� �� NO�� �� %�C�
 _VerRetValb� L��B�# T� �� Version Error occured!��N T� �� C� �  Kindly update latest version of C� ProductTitle� � �� ��C� �@�	 �x�� B�-�� � %�� � NO��`� B�-�� � %�C� mx2���~� Q�
 � � %�C� mx3����� Q� � � T� �� ��, %�� SQLCONNECTIONCCC� Classlibv�f���� ��4 Set Classlib To sqlconnection In &xapps Additive
 � �� �/ T� �C� sqlconnudobj� sqlconnection� ���� %�C� mvu_nencb� U��x� T� �� �� � %�� �����+ T� �CC� ��� Ud*yog+1993Uid� � ��+ T� �CC� ��� Ud*yog+1993Pwd� � �� �#� T� �CC � � � � � �� T� �CC � � � � � �� �b T� �� Driver={SQL server};server=� �
 ;database=C� � �� ;uid=� � ;pwd=� � ;��& T� �� "�  � |� |� |� "�� T� �C� ��� %�� � ��9� T� �C� �g�� %�� � ���� B�-�� �6 ��C� Connection Failed !!!C� C� CE�@�	 �x�� B�-�� � �� � �L T� �C� �3 Select * From UBIReportMast Where ReportId= ?lrepid� MX2�i�� %�� � ��� T� �C� �g�� %�� � ���� B�-�� �= ��C� Error in UBIReportMast tableC� C� CE�@�	 �x�� B�-�� ��� %�C� mx2N� ���� T� �C� �g�� %�� � ��[� B�-�� �1 ��C� Report ID :C�  ��
  not found�@�	 �x�� B�-�� ��� T� �C�
 � ��� T� �� PIVOT�� � � %�CCC� f�� PIVOT����� T�  �� �! �� T�" �� � �� T�# �� �$ �� T�% �C�& �  � <*#*>��� T�' �C�	 �  � <*#*>��� T�( �C� �) ��� T�* �CC�+ _�  � <*#*>��� T�, �C� Pathv�� T�- �CC�, _�  � <*#*>���" T�. �CCC� �/ *_�  � <*#*>���" T�0 �CCC� �1 *_�  � <*#*>��� T�2 ��  ��6 T�3 �C� �4 �CCC� �) ��
� � ,� �  6C� �) ��� ��5 ���(�C�3 >��l�! T�2 ��2 CCC�3 �5 �\� �� ��, T�2 ��2 CCC�2 ��R� ,� �  � � ,6��% T� �� �  CC�  _��  C�" ���. T� �� �  C� ��  C� ��  C� ���9 T� �� �  C� ��  C�6 ��  C�% ��  C�' ���9 T� �� �  C�7 ��  CC�8 Z��  C�9 ��  �2 ��9 T� �� �  C�* ��  C�- ��  C�. ��  C�0 ��� �e T�: ��X select Top 1 QueryId From UBIQueryMast Where ReportId= ?lrepid and SQLQuery Like '%{@%' �� T� �C� �: � MX3�i�� %�� � ���	� T� �C� �g�� %�� � ��U	� B�-�� �< ��C� Error in UBIQueryMast tableC� C� CE�@�	 �x�� B�-�� �N� F� � %�C� mx3N� ��� H��	�
� �� � PIVOT��
�  %�C� UdMISReports.exe 0���
� T� �C� �g�� %�� � ��,
� B�-�� �" T�; �� UdMISReports.exe � �� T�< �C� WScript.Shell�N�� ��C �; �< �= �� ��C�      �?7�� �� T� �C� �g�� %�� � ���
� B�-�� �8 ��C�& Udyog MIS Reports Tool not found...!!!�@�	 �x�� B�-�� � � �J� Q�
 � Q� �$ � frmUBIReport��  � � � � � � B� U?  LREPID LRANGE _VERVALIDERR
 _VERRETVAL _CURRVERVAL	 APPVERCHK CMSGSTR	 GLOBALOBJ GETPROPERTYVAL VUMESS MX2 MX3 NCONN	 SQLCONOBJ XAPPS MVU_NENC _USER NEWDECRY MVU_USER _PASS MVU_PASS DEC	 ONDECRYPT CONSTR
 MVU_SERVER COMPANY DBNAME CON_STR NRETVAL LRPTTYPE LRPTHEAD REPORTNM TCCOMPID COMPID TCCOMPDB TCCOMPNM CO_NAME VICOPATH ICOPATH PAPPLCAPTION _PASSROUTE1
 PASSROUTE1 APPPATH APATH CSETPATH SETPATH CSDATE STA_DT CEDATE END_DT	 _PRODCODE PVALUE	 PASSROUTE I	 MUSERNAME	 PAPPLNAME PAPPLID	 PAPPLCODE MSQLSTR	 LCCOMMAND OWSHELL EXEC FRMUBIREPORT� � � � Q� �� � A �1�!q A 1q A #� A "� A � �� AA q ��� A ��� ��A #bq A aq A � �q A �q � aq A q � !!A A ���!�1�!!� a�A �R����A T�q A �q � q a� aq A !�1� q A �q A A � � � AA A C 1                 .\ frmubireport.scx frmubireport.sct commandclick.vcx commandclick.vct e:\u3\vudyogsdk\class\ ctl32_progressbar.vcx ctl32_progressbar.vct main.prg c:\users\shrikant\appdata\local\temp\ main.fxp 	)   �                 �  ��                 ��  ��      %           ��  {�      6           {�  ��  G   ^           ��  �� G   t            �� �� �   �           