���    �L �K                     ��    %             
   �   EbK    ��  ��  �' %�� \DATEPICKER.CC� Classvf
��B � G~(�
 datepicker� �% %��
 \VOUCLASS.CC� Classvf
��} � G~(� VOUCLASS� � � frmcntrmast��  � U  TNRANGE
 DATEPICKER VOUCLASS FRMCNTRMASTq  qAA Q!A r1                 0   m                   PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Screen                                                                                              WINDOWS _26N0Q1979 926115298      /  F      ]                          �      �                       WINDOWS _26N0Q197A1258776429�  �          ,        �:                  9:                           WINDOWS _26N0RE50I1211069047,:      :  :  :  �9                                                           WINDOWS _26N0Q19791211263186�9      �9  w9  d9  �8                                                           WINDOWS _26N0Q197A1211263186�8      �8  �8  {8  �7      7  �,                                               WINDOWS _26N0Q197912112631867      7  �6  �6  76                                                           WINDOWS _26N0RE50J1211263186*6      6  6  �5  V5                                                           WINDOWS _26N0RE50L1211273389A5      ,5  5  
5  �4      �0  �(                                               WINDOWS _26Z150B821211263186�0      }0  o0  \0  �/                                                           WINDOWS _4JJ0RQMJO1211273558�/      �/  r/  _/  �.                                                           WINDOWS _4JJ0UPNLM1211263186�.      �.  �.  �.  �-                                                           COMMENT RESERVED                                �-                                                            W�                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      dataenvironment      dataenvironment      Dataenvironment      YTop = 0
Left = 0
Width = 0
Height = 0
DataSource = .NULL.
Name = "Dataenvironment"
      1      2      basefrm      &e:\u3\udyoggstsdk\class\standardui.vcx      form      FRMCNTRMAST      �DataSession = 2
Height = 170
Width = 340
DoCreate = .T.
BorderStyle = 1
Caption = "Counter Master"
ControlBox = .T.
MaxButton = .F.
ncurrid = .F.
stretchflg = .F.
lcmdcurrclk = .F.
curonmouse = .F.
Name = "FRMCNTRMAST"
     $|PROCEDURE getlastrecord
*!* added by Ruchit for countermast 
LOCAL cquery
IF !USED('cntr_mast_vw')
	cquery= "SELECT TOP 1 * FROM countermast ORDER BY CNTRID DESC"
	mret=THISFORM.sqlconobj.dataconn("EXE",company.dbname, cquery,"cntr_mast_vw","this.parent.nHandle",THISFORM.DATASESSIONID)
	IF mret > 0
		mret=THISFORM.sqlconobj.sqlconnclose("this.parent.nHandle")
		IF mret <= 0
			RETURN .F.
		ENDIF
	ENDIF
ELSE
	IF !EMPTY(THISFORM.ncurrid)
		cquery= "select * from countermast where cntrid = ?THISFORM.ncurrid"
		mret=THISFORM.sqlconobj.dataconn("EXE",company.dbname,cquery,"countermast_1","this.parent.nHandle",THISFORM.DATASESSIONID)
		IF mret > 0
			mret=THISFORM.sqlconobj.sqlconnclose("this.parent.nHandle")
			IF mret <= 0
				RETURN .F.
			ENDIF
			SELECT cntr_mast_vw
			IF !THISFORM.addmode AND !THISFORM.editmode
				DELETE ALL
			ENDIF
			APPEND FROM DBF("countermast_1")
		ENDIF
	ENDIF
ENDIF
SELECT cntr_mast_vw
GO BOTT
*!* added by Ruchit for countermast
ENDPROC
PROCEDURE createcontrolsource
*!* added by ruchit for countermast
ThisForm.txt_cntrcode.ControlSource = 'cntr_mast_vw.CNTRCODE'
ThisForm.txt_cntrdesc.ControlSource = 'cntr_mast_vw.cntrdesc'
ThisForm.txt_cntrrmrk.ControlSource = 'cntr_mast_vw.cntrrmrk'
*!* added by ruchit for countermast
ENDPROC
PROCEDURE modify
*!* added by Ruchit for countermast
THISFORM.addmode 	= .F.
THISFORM.editmode 	= .T.
THISFORM.act_deact(.T.)
THISFORM.REFRESH()
*!* added by Ruchit for countermast
ENDPROC
PROCEDURE Activate
DODEFAULT()
IF THISFORM.stretchflg=.F.
	THISFORM.stretchflg=.T.
	tbrdesktop.btnbtm.CLICK()
	THISFORM.createcontrolsource()
ENDIF 
IF !THISFORM.addmode AND !THISFORM.editmode
	=barstat(.T.,.T.,.F.,.F.,.F.,.F.,THISFORM.addbutton,THISFORM.editbutton,THISFORM.deletebutton,.F.,.F.,.F.,.T.,.T.)
ELSE
	=barstat(.F.,.F.,.F.,.F.,.F.,.F.,.T.,.T.,.F.,.F.,.F.,.F.,.T.,.T.)
ENDIF
ENDPROC
PROCEDURE Init
LPARA tnrange
lrange=tnrange
THISFORM.stretchflg=.F.

*!*	WITH THISFORM
*!*		.mainalias = "Curr_Mast_Vw"
*!*		.maintbl   = "Curr_Mast"
*!*		.mainfld   = "CurrencyId"
*!*		.addmode   = .F.
*!*		.editmode  = .F.
*!*		.createstdobjects()
*!*		.sqlconobj.assignedrights(tnrange,THISFORM.DATASESSIONID)
*!*	ENDWITH

*!*	SET NULLDISPLAY TO "" && very important

*!*	IF TYPE('TbrTools')='O'
*!*		tbrtools.VISIBLE=.F.
*!*	ENDIF
*!*	THISFORM.act_deact(.F.)
*!* ruchit
WITH Thisform
	.mainalias = "cntr_mast_vw"
	.maintbl  = "COUNTERMAST"
	.mainfld  = "CNTRID"
	.addmode = .F.
	.editmode = .F. 
	.createstdobjects()
	.sqlconobj.assignedrights(tnrange,THISFORM.DATASESSIONID)
ENDWITH

SET NULLDISPLAY TO "" && very important

IF TYPE('TbrTools')='O'
	tbrtools.VISIBLE=.F.
ENDIF
THISFORM.act_deact(.F.)
*!* ruchit
ENDPROC
PROCEDURE Refresh
*!* added by ruchit for countermast
IF !THISFORM.addmode AND !THISFORM.editmode
	IF USED('cntr_mast_vw')
		IF !THISFORM.lcmdcurrclk
*!*				THISFORM.ncurrid = cntr_mast_vw.cntrcode
			thisform.ncurrid = cntr_mast_vw.cntrid
		ENDIF
		THISFORM.lcmdcurrclk=.F.
		THISFORM.getlastrecord()
	ENDIF
ENDIF
tbrdesktop.btncopy.Enabled=.F.
tbrdesktop.btnview.Enabled=.F.
tbrdesktop.btnprin.Enabled=.F.
tbrdesktop.btnfind.Enabled=.F.
tbrdesktop.btnloc.Enabled=.F.
DODEFAULT()
*!* added by ruchit for countermast
ENDPROC
PROCEDURE act_deact
LPARAMETERS lact

*!* added by ruchit for countermast
WITH THISFORM
	.cntrpop.PICTURE 		 = apath+"Bmp\loc-on.Gif"
	.cntrpop.DISABLEDPICTURE = apath+"Bmp\loc-Off.Gif"
	.txt_cntrcode.ENABLED 		 = lact
	.txt_cntrdesc.ENABLED 		 = lact
	.txt_cntrrmrk.ENABLED 		 = lact
	.cntrpop.ENABLED 		 	 = !lact
	.label4.VISIBLE 			 = lact
ENDWITH


IF THISFORM.editmode = .F.
	THISFORM.txt_cntrcode.ENABLED = lact
ELSE
	THISFORM.txt_cntrcode.ENABLED  = .F.
ENDIF
*!* added by ruchit for countermast
ENDPROC
PROCEDURE addnew
*!* added by ruchit for countermast
THISFORM.addmode = .T.
THISFORM.editmode = .F.
THISFORM.act_deact(.T.)
SELECT cntr_mast_vw
DELETE ALL IN cntr_mast_vw
lccurrid = cntrid
APPEND BLANK
=TABLEUPDATE(.T.)
THISFORM.REFRESH()
*!* added by ruchit for countermast
ENDPROC
PROCEDURE cancel
*!* added by ruchit for countermast
=barstat(.T.,.T.,.T.,.T.,.F.,.F.,THISFORM.addbutton,THISFORM.editbutton,THISFORM.deletebutton,.F.,.F.,.F.,.T.,.T.)
THISFORM.addmode=.F.
THISFORM.editmode=.F.
SELECT cntr_mast_vw
=TABLEREVERT(.T.)

WITH THISFORM
	tbrdesktop.btnbtm.CLICK()
	.addmode=.F.
	.editmode=.F.
ENDWITH
THISFORM.getlastrecord()
THISFORM.REFRESH()
*!* added by ruchit for countermast


ENDPROC
PROCEDURE delete
*!* added by ruchit for countermast
SET STEP ON
yesno = MESSAGEBOX("Do you want to delete this Counter Record ?",4+32+256,vumess)
IF yesno <> 6
	RETURN .F.
ENDIF
SELECT cntr_mast_vw
mcurrid = cntr_mast_vw.cntrid
&&check in poscashin
cchkstr = "select top 1 cntrcode from poscashin where cntrcode = '"+thisform.txt_cntrcode.Value+"'" &&TRANSFORM(mcurrid)
nret=THISFORM.sqlconobj.dataconn("EXE",company.dbname,cchkstr,"temp","thisform.nhandle",THISFORM.DATASESSIONID,.T.)
SELECT temp
IF RECCOUNT() > 0
	MESSAGEBOX("Already in use, cannot delete counter code",0+16+256,vumess)
	RETURN .F.
ENDIF 

&&check in poscashin
cdelestr = [Delete From countermast Where cntrid = ]+Transform(mcurrid)

&& Delete Information [Start]
nret=THISFORM.sqlconobj.dataconn("EXE",company.dbname,cdelestr,"","thisform.nhandle",THISFORM.DATASESSIONID,.T.)
IF nret < 0
	MESSAGEBOX("Error occured while deleting details Information!!",16,vumess)
	RETURN .F.
ENDIF
&& Delete Information [End]

IF nret > 0
	cm=THISFORM.sqlconobj._sqlcommit("thisform.nhandle")
	IF cm < 0
		RETURN .F.
	ELSE
		MESSAGEBOX("Entry Deleted",64,vumess,1)
	ENDIF
	mret=THISFORM.sqlconobj.sqlconnclose("thisform.nhandle")
ENDIF

WITH THISFORM
	.act_deact(.F.)
	.deletebutton = .T.
	.printbutton = .T.
	.addmode = .F.
	.editmode = .F.
*!*		=Barstat(.F.,.F.,.F.,.F.,.F.,.F.,.AddButton,.EditButton,.DeleteButton,.T.,.T.,.F.,.T.,.T.)
	tbrdesktop.btnbtm.CLICK()
	.REFRESH()
ENDWITH
*!* added by ruchit for countermast
ENDPROC
PROCEDURE saveit
*!* added by Ruchit for countermast

SELECT cntr_mast_vw
=TABLEUPDATE(.T.)
GO TOP

SELECT cntr_mast_vw
IF EMPTY(cntr_mast_vw.CNTRCODE)
	=MESSAGEBOX("Counter Code cannot be Empty.",16,vumess)
	THISFORM.txt_CNTRCODE.SETFOCUS()
	RETURN .F.
ENDIF


IF THISFORM.addmode 	&& Checking Duplicate Records [Start]
	lcstr = [Select CNTRCODE From CounterMast Where CNTRCODE = ?cntr_mast_vw.cntrcode ]
	mret=THISFORM.sqlconobj.dataconn("EXE",company.dbname,lcstr,"_DupIt","Thisform.nhandle",THISFORM.DATASESSIONID)
	IF mret > 0
		SELECT _dupit
		IF RECCOUNT("_DupIt") > 0
			MESSAGEBOX("Counter Code has already been entered.",16,vumess)
			THISFORM.txt_CNTRCODE.SETFOCUS()
			RETURN .F.
		ENDIF
		mret=THISFORM.sqlconobj.sqlconnclose("thisform.nhandle")
		IF mret <= 0
			RETURN .F.
		ENDIF
	ENDIF
ENDIF			&& Checking Duplicate Records [End]
*!* ruchit
*/*/*/*/*/*/*/*/*/*/*/********** Checking Validations for CounterMast **********/*/*/*/*/*/*/*/*/*/*/* End
*!* ruchit
SELECT cntr_mast_vw
UPDATE cntr_mast_vw SET compid = company.compid WHERE EMPTY(compid)
=TABLEUPDATE(.T.)
WITH THISFORM
	lnhshandle = 0
	DO CASE
			CASE THISFORM.addmode

			lcinsstr = .sqlconobj.geninsert("countermast","'Code'","","cntr_mast_vw",THISFORM.platform,"")
			lnhshandle = .sqlconobj.dataconn("EXE",company.dbname,lcinsstr,"","thisform.nhandle",THISFORM.DATASESSIONID,.T.)
			IF lnhshandle < 0
				rb=THISFORM.sqlconobj._sqlrollback("thisform.nhandle")
				IF rb < 0
					MESSAGEBOX("RollBack Error!!",16,vumess)
					RETURN .F.
				ENDIF
				MESSAGEBOX("Error Occured While Saving!!",16,vumess)
				RETURN .F.
			ELSE
				mlncode=THISFORM.sqlconobj.dataconn("EXE",company.dbname,"SELECT TOP 1 * FROM countermast ORDER BY CNTRID DESC","Code","thisform.nHandle",THISFORM.DATASESSIONID)
				THISFORM.ncurrid=CODE.CNTRID
			ENDIF

		CASE THISFORM.editmode					&& in Editmode

			********** Header Line Update
			lcinsstr = .sqlconobj.genupdate("countermast","'cntrid','cntrcode','compid'","","cntr_mast_vw",THISFORM.platform,[cntrcode = ]+"'"+TRANSFORM(cntr_mast_vw.cntrcode)+"'","")
			lnhshandle = .sqlconobj.dataconn("EXE",company.dbname,lcinsstr,"","thisform.nhandle",THISFORM.DATASESSIONID,.T.)
			IF lnhshandle < 0
				rb=THISFORM.sqlconobj._sqlrollback("thisform.nhandle")
				IF rb < 0
					MESSAGEBOX("RollBack Error!!",16,vumess)
					RETURN .F.
				ENDIF
				MESSAGEBOX("Error Occured While Saving!!",16,vumess)
				RETURN .F.
			ELSE
				mlncode=THISFORM.sqlconobj.dataconn("EXE",company.dbname,"SELECT TOP 1 * FROM countermast ORDER BY CNTRID DESC","Code","thisform.nHandle",THISFORM.DATASESSIONID)
				THISFORM.ncurrid=CODE.CNTRID
			ENDIF

	ENDCASE
ENDWITH

*!*	Thisform.txtfitem.SetFocus()

IF lnhshandle > 0
	cm=THISFORM.sqlconobj._sqlcommit("thisform.nhandle")
	IF cm < 0
		RETURN .F.
	ELSE
		MESSAGEBOX("Entry Saved",64,vumess,1)
	ENDIF
	mret=THISFORM.sqlconobj.sqlconnclose("thisform.nhandle")
ENDIF

WITH THISFORM
	.act_deact(.F.)
	.deletebutton = .T.
	.printbutton = .T.
	.addmode = .F.
	.editmode = .F.
	tbrdesktop.btnbtm.CLICK()
ENDWITH
THISFORM.REFRESH()
*!* added by Ruchit for countermast
ENDPROC
     ����    �  �                        q   %   a      �     p          �  U  ^ ��  � � � � � � T� ��  ��i T� ��\ Select CNTRCODE as Counter_Code, cntrdesc as [Desc],CNTRID  From countermast order by CNTRID��O T� �C� EXE�	 �
  � � Cntr_countrmast� THISFORM.nHandle� � � � � �� %�� � ��	� ��CCE�� �x�� B�-�� �) T� �� SELECT : Counter Master Code��� T�  �C� Cntr_countrmast � � Counter_Code+desc� CNTRID�  �  �  �  -�  � Counter_Code,Desc,CNTRID�* Counter_Code:Counter Code,Desc:Description� CNTRID� �� F� � T� � ��  ��' T� �C� thisform.nHandle� � � �� %�C� Cntr_countrmast���<� Q� � � T� � �a�� ��C� � �� U  RETITEM CSQLSTR NRETVAL MRET LCCATION	 LCIT_NAME THISFORM	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID VUMESS UEGETPOP CNTR_MAST_VW NCURRID SQLCONNCLOSE CNTR_COUNTRMAST LCMDCURRCLK REFRESH Click,     ��1 �� ��q A �!
s r�� A � � 2                       �      )   �                       %���                              ��   %   �       �      �           �  U  V  F�  � %�C�  � ���O �/ ��C� Counter Code Cannot be Empty.�� �x�� B�-�� � U  CNTR_MAST_VW CNTRCODE VUMESS Valid,     ��1 q !�q A 2                       �       )                           !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      �FontSize = 8
Height = 23
Left = 113
MaxLength = 0
SpecialEffect = 1
TabIndex = 2
Top = 39
Width = 218
BorderColor = 128,128,128
Name = "txt_cntrdesc"
      FRMCNTRMAST      txt_cntrdesc      textbox      textbox      rFontSize = 8
Height = 98
Left = 113
MaxLength = 0
TabIndex = 3
Top = 66
Width = 218
Name = "txt_cntrrmrk"
      FRMCNTRMAST      txt_cntrrmrk      editbox      editbox      �AutoSize = .T.
FontBold = .T.
FontSize = 15
BackStyle = 0
Caption = "*"
Height = 27
Left = 101
Top = 13
Width = 10
TabIndex = 8
ForeColor = 255,0,0
Name = "Label4"
      FRMCNTRMAST      Label4      label      label     �PROCEDURE Click
Local RetItem,CSqlstr,nretval,mret,Lccation,lcIt_Name

*!* added by Ruchit for countermast
	lcIt_Name = []
	CSqlstr = 'Select CNTRCODE as Counter_Code, cntrdesc as [Desc],CNTRID  From countermast order by CNTRID'
	nretval=Thisform.sqlconobj.dataconn("EXE",company.dbname,CSqlstr,"Cntr_countrmast","THISFORM.nHandle",Thisform.DataSessionId)
	If nretval<0
		=Messagebox(Message(),0+16,vumess)
		Return .F.
	Endif
	Lccation = "SELECT : Counter Master Code"
	RetItem=uegetpop([Cntr_countrmast],Lccation,[Counter_Code+desc],[CNTRID],[],[],[],[],.F.,[],[Counter_Code,Desc,CNTRID],[Counter_Code:Counter Code,Desc:Description],[CNTRID])

	*!*	If Vartype(RetItem) = "O"
		Select cntr_mast_vw
		Thisform.ncurrid = RetItem
	*!*	Endif
	mret=Thisform.sqlconobj.sqlconnclose("thisform.nHandle")
	If Used("Cntr_countrmast")
		Use In Cntr_countrmast
	Endif
	*!*	Thisform.getlastrecord()
	*!*	Endif
	ThisForm.lcmdcurrclk=.t.
	Thisform.Refresh()
*!* added by Ruchit for countermast
ENDPROC
      kTop = 12
Left = 302
Height = 23
Width = 23
FontSize = 8
Caption = ""
TabIndex = 7
Name = "cntrpop"
      FRMCNTRMAST      cntrpop      commandbutton      commandbutton      �AutoSize = .T.
FontBold = .F.
FontSize = 8
BackStyle = 0
Caption = "Remark"
Height = 16
Left = 14
Top = 69
Width = 38
TabIndex = 6
Name = "Label3"
      FRMCNTRMAST      Label3      label      label      �AutoSize = .T.
FontBold = .F.
FontSize = 8
BackStyle = 0
Caption = "Description"
Height = 16
Left = 14
Top = 42
Width = 56
TabIndex = 5
Name = "Label2"
      FRMCNTRMAST      Label2      label      label      �PROCEDURE Valid
Select cntr_mast_vw
If Empty(cntr_mast_vw.cntrcode)
	=Messagebox("Counter Code Cannot be Empty.",16,vuMess)
	Return .F.
Endif

ENDPROC
      �FontSize = 8
Format = "!@"
Height = 23
Left = 113
MaxLength = 5
SpecialEffect = 1
TabIndex = 1
Top = 12
Width = 177
BorderColor = 128,128,128
Name = "txt_cntrcode"
      FRMCNTRMAST      txt_cntrcode      textbox      textbox      �AutoSize = .T.
FontBold = .F.
FontSize = 8
BackStyle = 0
Caption = "Code"
Height = 16
Left = 14
Top = 15
Width = 27
TabIndex = 4
Name = "Label1"
      FRMCNTRMAST      Label1      label      label      \Top = 1
Left = 1
Height = 168
Width = 338
BackStyle = 0
SpecialEffect = 0
Name = "w"
      FRMCNTRMAST      w      shape      shape      Vncurrid
stretchflg
lcmdcurrclk
curonmouse
*getlastrecord 
*createcontrolsource 
     ^���    E  E                        U�   %   �      \  �   n          �  U  w ��  � %�C� cntr_mast_vw�
���A T�  ��4 SELECT TOP 1 * FROM countermast ORDER BY CNTRID DESC��O T� �C� EXE� �  �  � cntr_mast_vw� this.parent.nHandle� � � � � �� %�� � ��
�* T� �C� this.parent.nHandle� � � �� %�� � ��� B�-�� � � �d� %�C� �	 �
��`�G T�  ��: select * from countermast where cntrid = ?THISFORM.ncurrid��P T� �C� EXE� �  �  � countermast_1� this.parent.nHandle� � � � � �� %�� � ��\�* T� �C� this.parent.nHandle� � � �� %�� � ��� B�-�� � F�
 � %�� � 
� � � 
	��?� � � �C� countermast_1&�� � � � F�
 � #6� U  CQUERY MRET THISFORM	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID SQLCONNCLOSE NCURRID CNTR_MAST_VW ADDMODE EDITMODE ALL{ ( T�  � � �� cntr_mast_vw.CNTRCODE��( T�  � � �� cntr_mast_vw.cntrdesc��( T�  � � �� cntr_mast_vw.cntrrmrk�� U  THISFORM TXT_CNTRCODE CONTROLSOURCE TXT_CNTRDESC TXT_CNTRRMRK:  T�  � �-�� T�  � �a�� ��Ca�  � �� ��C�  � �� U  THISFORM ADDMODE EDITMODE	 ACT_DEACT REFRESH� 	 ��C��� %��  � -��F � T�  � �a�� ��C� � � �� ��C�  � �� � %��  � 
� �  � 
	��� �' ��Caa----�  �	 �  �
 �  � ---aa� �� �� � ��C------aa----aa� �� � U  THISFORM
 STRETCHFLG
 TBRDESKTOP BTNBTM CLICK CREATECONTROLSOURCE ADDMODE EDITMODE BARSTAT	 ADDBUTTON
 EDITBUTTON DELETEBUTTON�  ��  � T� ��  �� T� � �-�� ��� ��� � T�� �� cntr_mast_vw�� T�� �� COUNTERMAST�� T�� �� CNTRID�� T�� �-�� T�� �-�� ��C��	 �� ��C �  � � ��
 � �� �� G�(��  �� %�C� TbrToolsb� O��� � T� � �-�� � ��C-� � �� U  TNRANGE LRANGE THISFORM
 STRETCHFLG	 MAINALIAS MAINTBL MAINFLD ADDMODE EDITMODE CREATESTDOBJECTS	 SQLCONOBJ ASSIGNEDRIGHTS DATASESSIONID TBRTOOLS VISIBLE	 ACT_DEACT�  %��  � 
� �  � 
	��~ � %�C� cntr_mast_vw���z � %��  � 
��[ � T�  � �� � �� � T�  � �-�� ��C�  � �� � � T� �	 �
 �-�� T� � �
 �-�� T� � �
 �-�� T� � �
 �-�� T� � �
 �-��	 ��C��� U  THISFORM ADDMODE EDITMODE LCMDCURRCLK NCURRID CNTR_MAST_VW CNTRID GETLASTRECORD
 TBRDESKTOP BTNCOPY ENABLED BTNVIEW BTNPRIN BTNFIND BTNLOC�  ��  � ��� ��� �# T�� � �� � Bmp\loc-on.Gif��$ T�� � �� � Bmp\loc-Off.Gif�� T�� � ��  �� T�� � ��  �� T��	 � ��  �� T�� � ��  
�� T��
 � ��  �� �� %�� � -��� � T� � � ��  �� �� � T� � � �-�� � U  LACT THISFORM CNTRPOP PICTURE APATH DISABLEDPICTURE TXT_CNTRCODE ENABLED TXT_CNTRDESC TXT_CNTRRMRK LABEL4 VISIBLE EDITMODEf  T�  � �a�� T�  � �-�� ��Ca�  � �� F� �	 � � T� �� �� �
 ��Ca��� ��C�  � �� U	  THISFORM ADDMODE EDITMODE	 ACT_DEACT CNTR_MAST_VW ALL LCCURRID CNTRID REFRESH� ' ��Caaaa--� � � � � � ---aa�  �� T� � �-�� T� � �-�� F� �
 ��Ca��� ��� ��� � ��C� �	 �
 �� T�� �-�� T�� �-�� �� ��C� � �� ��C� � �� U  BARSTAT THISFORM	 ADDBUTTON
 EDITBUTTON DELETEBUTTON ADDMODE EDITMODE CNTR_MAST_VW
 TBRDESKTOP BTNBTM CLICK GETLASTRECORD REFRESHa G1 �B T�  �C�+ Do you want to delete this Counter Record ?�$� �x�� %��  ���` � B�-�� � F� � T� �� � ��S T� ��7 select top 1 cntrcode from poscashin where cntrcode = '� � � � '��E T�	 �C� EXE� �  � � temp� thisform.nhandle� � a� �
 � �� F� � %�CN� ��n�= ��C�* Already in use, cannot delete counter code�� �x�� B�-�� �: T� ��' Delete From countermast Where cntrid = C� _��A T�	 �C� EXE� �  � �  � thisform.nhandle� � a� �
 � �� %��	 � ��I�D ��C�2 Error occured while deleting details Information!!�� �x�� B�-�� � %��	 � ����' T� �C� thisform.nhandle� �
 � �� %�� � ���� B�-�� ���" ��C� Entry Deleted�@� ��x�� �' T� �C� thisform.nhandle� �
 � �� � ��� ��Z� ��C-�� �� T�� �a�� T�� �a�� T�� �-�� T�� �-�� ��C� � � �� ��C�� �� �� U  YESNO VUMESS CNTR_MAST_VW MCURRID CNTRID CCHKSTR THISFORM TXT_CNTRCODE VALUE NRET	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID TEMP CDELESTR CM
 _SQLCOMMIT MRET SQLCONNCLOSE	 ACT_DEACT DELETEBUTTON PRINTBUTTON ADDMODE EDITMODE
 TBRDESKTOP BTNBTM CLICK REFRESHO F�  �
 ��Ca��� #)� F�  � %�C�  � ���u �/ ��C� Counter Code cannot be Empty.�� �x�� ��C� � � �� B�-�� � %�� � ����V T� ��I Select CNTRCODE From CounterMast Where CNTRCODE = ?cntr_mast_vw.cntrcode ��F T� �C� EXE� �  � � _DupIt� Thisform.nhandle� � � �	 �
 �� %�� � ���� F� � %�C� _DupItN� ����8 ��C�& Counter Code has already been entered.�� �x�� ��C� � � �� B�-�� �' T� �C� thisform.nhandle� �	 � �� %�� � ���� B�-�� � � � F�  �( p� cntr_mast_vw�� �� � ���C� ���
 ��Ca��� ��� ��7� T� �� �� H�T�3� �� � ��(�D T� �C� countermast� 'Code'�  � cntr_mast_vw� � �  ��	 � ��? T� �C� EXE� �  � �  � thisform.nhandle� � a��	 �
 �� %�� � ����' T� �C� thisform.nhandle� �	 � �� %�� � ��Y�" ��C� RollBack Error!!�� �x�� B�-�� �. ��C� Error Occured While Saving!!�� �x�� B�-�� �$�w T� �C� EXE� � �4 SELECT TOP 1 * FROM countermast ORDER BY CNTRID DESC� Code� thisform.nHandle� � � �	 �
 �� T� � �� � �� � �� � ��3�{ T� �C� countermast� 'cntrid','cntrcode','compid'�  � cntr_mast_vw� � � cntrcode = � 'C�  � _� '�  ��	 � ��? T� �C� EXE� �  � �  � thisform.nhandle� � a��	 �
 �� %�� � ����' T� �C� thisform.nhandle� �	 � �� %�� � ��d�" ��C� RollBack Error!!�� �x�� B�-�� �. ��C� Error Occured While Saving!!�� �x�� B�-�� �/�w T� �C� EXE� � �4 SELECT TOP 1 * FROM countermast ORDER BY CNTRID DESC� Code� thisform.nHandle� � � �	 �
 �� T� � �� � �� � � �� %�� � ����' T� �C� thisform.nhandle� �	 � �� %�� � ���� B�-�� ���  ��C� Entry Saved�@� ��x�� �' T� �C� thisform.nhandle� �	 � �� � ��� ��;� ��C-�� �� T��  �a�� T��! �a�� T�� �-�� T�� �-�� ��C�" �# �$ �� �� ��C� �% �� U&  CNTR_MAST_VW CNTRCODE VUMESS THISFORM TXT_CNTRCODE SETFOCUS ADDMODE LCSTR MRET	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID _DUPIT SQLCONNCLOSE COMPID
 LNHSHANDLE LCINSSTR	 GENINSERT PLATFORM RB _SQLROLLBACK MLNCODE NCURRID CODE CNTRID EDITMODE	 GENUPDATE CM
 _SQLCOMMIT	 ACT_DEACT DELETEBUTTON PRINTBUTTON
 TBRDESKTOP BTNBTM CLICK REFRESH getlastrecord,     �� createcontrolsource1    �� modify�    �� Activateb    �� Init�    �� RefreshU    ��	 act_deact�    �� addnew[
    �� cancel    �� deleteU    �� saveit�    ��1 r ���q A A � 1q�q A q �Q A �A A A q Q 3 ���3 � � � � 3 � !� � A �q� �A 2 q � �  � ��A� � � �A � �� A � 3 ��2A � � A A � 3 q � 1A!A #1� A 3 � � � q � � Q � � 3 r� � q � � � � A � � 5 b !q A q 2Qq �q A �Aq A qq � !A qA � � � � � � � A 3 s � Q r !�q A aaq ��q A qq A A A t �� � � � B�q!q A �q � q1A ��q!q A �q � q1A B A qq � A qA � � � � � � A � 2                       �            "   #   2  �  )   (   �  r  1   4   �  �
  >   G   �
  �  f   W     
  z   g   '  2  �   q   O  �  �   ~     �  �   �     q$  �    )   E                  0	   m                   PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Class                                                                                               WINDOWS _20E0O8ZGW 911629295u      �  �      �      �  eK          _  l  �          Q               COMMENT RESERVED                        A                                                                 WINDOWS _20E0O8ZGW 913667855`      {  l            �  �N          J  W  �          <               COMMENT RESERVED                        -                                                                 WINDOWS _20T0ZFDV9 914658114�      x  
      �      �  �T          �  �  �          �  �           WINDOWS _20T0ZFE2M 914658114�      �  �  {  �                                                           WINDOWS _20T0ZFE4D 914658114�  �  x  ^  O                                                             COMMENT RESERVED                        �      �                                                            Y�                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      !Arial, 0, 9, 5, 15, 12, 32, 3, 0
      standfrm      Pixels      Class      1      form      standfrm      Ynhandle SQL - Connection handle
ofrmfrom Main form property object
*createstdobjects 
      �DataSession = 2
Height = 250
Width = 375
ShowWindow = 1
DoCreate = .T.
AutoCenter = .T.
BorderStyle = 3
Caption = ""
ControlBox = .T.
Closable = .F.
MaxButton = .F.
MinButton = .F.
WindowType = 1
nhandle = 0
ofrmfrom = 
Name = "standfrm"
      form      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      basefrm      Pixels      Class      1      form      basefrm      form      !Arial, 0, 9, 5, 15, 12, 32, 3, 0
      frmpbar      Pixels      Progress bar class      Class      3      form      frmpbar     .Top = 26
Left = 13
BackColor = 255,255,255
BorderColor = 0,128,255
value = 0
smooth = .T.
barcolor = 12937777
play = .F.
sizeadjust = .T.
Name = "Ctl32_progressbar1"
lblControlNameH.Name = "lblControlNameH"
tmrControlTimer.Name = "tmrControlTimer"
lblControlNameV.Name = "lblControlNameV"
      frmpbar      Ctl32_progressbar1      control      ctl32_progressbar.vcx      ctl32_progressbar      �AutoSize = .T.
FontBold = .T.
FontSize = 8
BackStyle = 0
Caption = "Refreshing data......."
Height = 16
Left = 14
Top = 8
Width = 110
ForeColor = 255,0,0
Name = "Lblinfo"
      frmpbar      Lblinfo      label      label      oshowprogress
lblcation
*initproc 
*progressbarexec 
*cleaprogressbar 
*showpbar 
*incpbar 
*setcation 
     ����    �  �                        ��	   %   �      �  $   �          �  U  5  G]� ��C� Please wait...!�  � �� ��C�  � �� U  THIS	 SETCATION SHOWPBAR�  4�  � %��  �d��% � ��C� � �� �� � %�� � -��� � T� � � �� � � �  �� �� ���(��d������y � �� ��C� � �� � � U  MVALUE THIS CLEAPROGRESSBAR SHOWPROGRESS CTL32_PROGRESSBAR1 VALUE A REFRESH  T�  � �a�� ��C�  � �� U  THIS SHOWPROGRESS RELEASE  T�  � �a�� U  THIS VISIBLEO ! ��  Q� INTEGER� Q� INTEGER� �� ��  �(�� ��H � ��C�� � �� �� U  FNUM SNUM I THIS PROGRESSBAREXEC  ��  � T� � � ��  �� U  LABLCAPTION THIS LBLINFO CAPTION)  T�  � ��  � �� T�  � ��  � �� U  THIS MINWIDTH WIDTH	 MINHEIGHT HEIGHT  U  	  G] � U   initproc,     �� progressbarexec�     �� cleaprogressbarm    �� showpbar�    �� incpbar�    ��	 setcationI    �� Load�    �� Init�    �� Release�    ��1 a �� 3 q � � !��A � A A 3 � � 3 � 3 qA 3 q 13 114 4 a 1                       X         ~   l        �  �        �  �          r        �  �  %      �     *   !   ;  =  0   "   [  h  4    )   �                       sPROCEDURE initproc
SET CURSOR OFF
THIS.setcation([Please wait...!])
THIS.showpbar()

ENDPROC
PROCEDURE progressbarexec
PARAMETERS MValue
IF MValue > 100
	THIS.CleaProgressBar()
ELSE
	IF THIS.showProgress = .F.
		THIS.ctl32_progressbar1.VALUE = THIS.ctl32_progressbar1.VALUE + MValue
		FOR a=1 TO 100 STEP 1
		ENDFOR
		THIS.REFRESH()
	ENDIF
ENDIF

ENDPROC
PROCEDURE cleaprogressbar
THIS.showProgress = .T.
THIS.RELEASE()

ENDPROC
PROCEDURE showpbar
THIS.VISIBLE = .T.

ENDPROC
PROCEDURE incpbar
LPARAMETERS Fnum AS INTEGER, Snum AS INTEGER
FOR i = Fnum TO Snum
	THIS.ProgressBarExec(1)
ENDFOR

ENDPROC
PROCEDURE setcation
LPARA Lablcaption
THIS.Lblinfo.CAPTION = Lablcaption

ENDPROC
PROCEDURE Load
This.MinWidth = This.Width
This.MinHeight = This.Height


ENDPROC
PROCEDURE Init


ENDPROC
PROCEDURE Release
SET CURSOR ON
ENDPROC
      form     PROCEDURE createstdobjects
*:*****************************************************************************
*:        Methods: Createstdobjects
*:         System: UDYOG ERP
*:         Author: RND Team.
*:  Last modified: 15-Feb-2007
*:			AIM  : Create UDYOG ERP Standard object and UI
*:*****************************************************************************
WITH THISFORM
	IF TYPE("Thisform.ofrmfrom") = "O"
		.BACKCOLOR = .ofrmfrom.BACKCOLOR
		IF .ofrmfrom.EDITMODE .OR. .ofrmfrom.ADDMODE
			.SETALL("enabled",.T.,"textbox")
			.SETALL("enabled",.T.,"checkbox")
			.SETALL("enabled",.T.,"combobox")
		ELSE
			.SETALL("enabled",.F.,"textbox")
			.SETALL("enabled",.F.,"checkbox")
			.SETALL("enabled",.F.,"combobox")
		ENDIF
		SET DATASESSION TO .ofrmfrom.DATASESSIONID
	ENDIF
	.ADDOBJECT("sqlconobj","sqlconnudobj")
	.ADDOBJECT("_stuffObject","_stuff")
	._stuffObject._objectPaint()
	.ICON = icopath
ENDWITH

ENDPROC
PROCEDURE Activate
=barstat(.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.)

ENDPROC
     )nhandle SQL - Connection handle
addmode TRUE is Addmode
editmode TRUE is Edit mode
co_dtbase Active company database
maincond Main condition for SQL-String
mainfld Main fields [Primary fields]
maintbl Maintbl is <SERVER TABLE> 
mainalias Mainalias is <VFP Cursor>
addbutton
editbutton
deletebutton
istoolbar Is use standard tool bar
platform
printbutton
previewbutton
ldefaenv
*createstdobjects Create standard UDYOG ERP object.
*addnew Add new entry method
*modify Modify/Alter records method
*delete Delete Record Method
*saveit Save / Update Record Method
*cancel Cancel records method
*loc 
*find Find records Method
*copy 
*printing Printing Method
*stdactivate Udyog standard activate method
*stdqunload Udyog standard Query unload
*act_deact 
*defaenv Default environment
     �Height = 59
Width = 328
ShowWindow = 2
ShowInTaskBar = .F.
DoCreate = .T.
AutoCenter = .T.
BorderStyle = 1
Caption = ""
ControlBox = .F.
Closable = .F.
HalfHeightCaption = .F.
MaxButton = .F.
MinButton = .F.
Movable = .T.
Visible = .F.
ClipControls = .F.
DrawWidth = 0
TitleBar = 0
WindowType = 0
AlwaysOnTop = .T.
BackColor = 235,235,235
ContinuousScroll = .F.
Themes = .F.
BindControls = .F.
showprogress = .F.
lblcation = .F.
Name = "frmpbar"
     R���    9  9                        uZ   %   �      �     �          �  U  � ���  ����% %�C� Thisform.ofrmfromb� O��?� T�� ��� � �� %��� � � �� � ��� �  ��C� enableda� textbox�� ��! ��C� enableda� checkbox�� ��! ��C� enableda� combobox�� �� �,�  ��C� enabled-� textbox�� ��! ��C� enabled-� checkbox�� ��! ��C� enabled-� combobox�� �� � G�(��� � �� �& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� �	 �� T��
 �� �� �� U  THISFORM	 BACKCOLOR OFRMFROM EDITMODE ADDMODE SETALL DATASESSIONID	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT ICON ICOPATH  ��C--------------�  �� U  BARSTAT createstdobjects,     �� Activate^    ��1 � Q!�� A � A a1� � A 3 �2                       �        �        )   9                       �DataSession = 2
Height = 250
Width = 375
ShowWindow = 1
DoCreate = .T.
ShowTips = .T.
AutoCenter = .T.
BorderStyle = 3
Caption = ""
ControlBox = .T.
FontSize = 8
WindowType = 0
nhandle = 0
addmode = .F.
editmode = .F.
co_dtbase = 
maincond = .F.
mainfld = 
maintbl = 
mainalias = 
addbutton = .F.
editbutton = .F.
deletebutton = .F.
istoolbar = .T.
platform = .F.
printbutton = .F.
previewbutton = .F.
ldefaenv = .T.
Name = "basefrm"
     	hPROCEDURE createstdobjects
*:*****************************************************************************
*:        Methods: Createstdobjects
*:         System: UDYOG ERP
*:         Author: RND Team.
*:  Last modified: 15-Feb-2007
*:			AIM  : Create UDYOG ERP Standard object and UI
*:*****************************************************************************
WITH THISFORM
	IF TYPE("Company") = "O"
		.BACKCOLOR = VAL(Company.vcolor)
		.co_dtbase = Company.DBname
		.platform = mvu_backend
		.ICON = icopath
	ENDIF
	.defaenv()
	.ADDOBJECT("sqlconobj","sqlconnudobj")
	.ADDOBJECT("_stuffObject","_stuff")
	._stuffObject._objectPaint()
ENDWITH

ENDPROC
PROCEDURE printing
LPARAMETERS tcviewprint as Character 

ENDPROC
PROCEDURE stdactivate
*:*****************************************************************************
*:        Methods: Stdactivate
*:         System: UDYOG ERP
*:         Author: RND Team.
*:  Last modified: 15-Feb-2007
*:			AIM  : Create UDYOG ERP Standard Tool bar control method
*:*****************************************************************************
WITH THISFORM
	IF TYPE("tbrDesktop") = "O" AND .Istoolbar
		IF ! .addmode AND ! .editmode
			=barstat(.T.,.T.,.T.,.T.,.F.,.F.,.AddButton,.EditButton,.DeleteButton,.F.,.F.,.F.,.T.,.T.)
		ELSE
			=barstat(.F.,.F.,.F.,.F.,.F.,.F.,.T.,.T.,.F.,.F.,.F.,.F.,.T.,.T.)
		ENDIF
		tbrDesktop.REFRESH()
	ENDIF
	.REFRESH()
ENDWITH

ENDPROC
PROCEDURE stdqunload
*:*****************************************************************************
*:        Methods: Stdqunload
*:         System: UDYOG ERP
*:         Author: RND Team.
*:  Last modified: 15-Feb-2007
*:			AIM  : Standard Query unload Method when Istoolbar is TRUE
*:*****************************************************************************
IF TYPE("tbrDesktop") = "O" AND THISFORM.Istoolbar
	IF tbrDesktop.RESTORE()
		NODEFA
		RETU .F.
	ENDIF
	IF ! tbrDesktop.FLAG
		NODEFA
		RETU .F.
	ENDIF
	=barstat(.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.,.F.)
ENDIF

ENDPROC
PROCEDURE defaenv
IF THIS.lDefaenv
	SET DELETED ON
	SET DATE British
	SET CENTURY ON
	SET TALK OFF
	SET SAFETY OFF
	SET STATUS OFF
	SET NULL ON
	SET NULLDISPLAY TO ''
	SET STRICTDATE TO 0
ENDIF


ENDPROC
PROCEDURE Activate
THISFORM.stdactivate()
ENDPROC
PROCEDURE QueryUnload
THISFORM.Stdqunload()

ENDPROC
     3���                              ��   %   t      q  8             �  U  �  ���  ��� � %�C� Companyb� O��h � T�� �C� � g�� T�� �� � �� T�� �� �� T�� ��	 �� � ��C��
 ��& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� � �� �� U  THISFORM	 BACKCOLOR COMPANY VCOLOR	 CO_DTBASE DBNAME PLATFORM MVU_BACKEND ICON ICOPATH DEFAENV	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT  ��  Q�	 CHARACTER� U  TCVIEWPRINT�  ���  ��� �& %�C�
 tbrDesktopb� O� �� 	��� � %��� 
� �� 
	��l �! ��Caaaa--�� �� �� ---aa� �� �� � ��C------aa----aa� �� � ��C� �	 �� � ��C��	 �� �� U
  THISFORM	 ISTOOLBAR ADDMODE EDITMODE BARSTAT	 ADDBUTTON
 EDITBUTTON DELETEBUTTON
 TBRDESKTOP REFRESH� ( %�C�
 tbrDesktopb� O� �  � 	��� � %�C� � ��D � �� B�-�� � %�� � 
��d � �� B�-�� � ��C--------------� �� � U  THISFORM	 ISTOOLBAR
 TBRDESKTOP RESTORE FLAG BARSTAT`  %��  � ��Y � G � G� British� G � G2� G.� G0� Gw � G�(��  �� G�(�� �� � U  THIS LDEFAENV BRITISH  ��C�  � �� U  THISFORM STDACTIVATE  ��C�  � �� U  THISFORM
 STDQUNLOAD createstdobjects,     �� printing�    �� stdactivate�    ��
 stdqunload�    �� defaenv�    �� Activate    �� QueryUnloadJ    ��1 � �1� � A � a1� A 3 A3 � a�� �A � A � A 3 �A q A A q A �A 3 a � a a a a a � � A 4 � 2 � 2                       �        �  �          �        �    0   (   2  �  E   4   	  $	  T   6   F	  ]	  W    )                          R���    9  9                        uZ   %   �      �     �          �  U  � ���  ����% %�C� Thisform.ofrmfromb� O��?� T�� ��� � �� %��� � � �� � ��� �  ��C� enableda� textbox�� ��! ��C� enableda� checkbox�� ��! ��C� enableda� combobox�� �� �,�  ��C� enabled-� textbox�� ��! ��C� enabled-� checkbox�� ��! ��C� enabled-� combobox�� �� � G�(��� � �� �& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� �	 �� T��
 �� �� �� U  THISFORM	 BACKCOLOR OFRMFROM EDITMODE ADDMODE SETALL DATASESSIONID	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT ICON ICOPATH  ��C--------------�  �� U  BARSTAT createstdobjects,     �� Activate^    ��1 � Q!�� A � A a1� � A 3 �2                       �        �        )   9                       3���                              ��   %   t      q  8             �  U  �  ���  ��� � %�C� Companyb� O��h � T�� �C� � g�� T�� �� � �� T�� �� �� T�� ��	 �� � ��C��
 ��& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� � �� �� U  THISFORM	 BACKCOLOR COMPANY VCOLOR	 CO_DTBASE DBNAME PLATFORM MVU_BACKEND ICON ICOPATH DEFAENV	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT  ��  Q�	 CHARACTER� U  TCVIEWPRINT�  ���  ��� �& %�C�
 tbrDesktopb� O� �� 	��� � %��� 
� �� 
	��l �! ��Caaaa--�� �� �� ---aa� �� �� � ��C------aa----aa� �� � ��C� �	 �� � ��C��	 �� �� U
  THISFORM	 ISTOOLBAR ADDMODE EDITMODE BARSTAT	 ADDBUTTON
 EDITBUTTON DELETEBUTTON
 TBRDESKTOP REFRESH� ( %�C�
 tbrDesktopb� O� �  � 	��� � %�C� � ��D � �� B�-�� � %�� � 
��d � �� B�-�� � ��C--------------� �� � U  THISFORM	 ISTOOLBAR
 TBRDESKTOP RESTORE FLAG BARSTAT`  %��  � ��Y � G � G� British� G � G2� G.� G0� Gw � G�(��  �� G�(�� �� � U  THIS LDEFAENV BRITISH  ��C�  � �� U  THISFORM STDACTIVATE  ��C�  � �� U  THISFORM
 STDQUNLOAD createstdobjects,     �� printing�    �� stdactivate�    ��
 stdqunload�    �� defaenv�    �� Activate    �� QueryUnloadJ    ��1 � �1� � A � a1� A 3 A3 � a�� �A � A � A 3 �A q A A q A �A 3 a � a a a a a � � A 4 � 2 � 2                       �        �  �          �        �    0   (   2  �  E   4   	  $	  T   6   F	  ]	  W    )                          ����    �  �                        ��	   %   �      �  $   �          �  U  5  G]� ��C� Please wait...!�  � �� ��C�  � �� U  THIS	 SETCATION SHOWPBAR�  4�  � %��  �d��% � ��C� � �� �� � %�� � -��� � T� � � �� � � �  �� �� ���(��d������y � �� ��C� � �� � � U  MVALUE THIS CLEAPROGRESSBAR SHOWPROGRESS CTL32_PROGRESSBAR1 VALUE A REFRESH  T�  � �a�� ��C�  � �� U  THIS SHOWPROGRESS RELEASE  T�  � �a�� U  THIS VISIBLEO ! ��  Q� INTEGER� Q� INTEGER� �� ��  �(�� ��H � ��C�� � �� �� U  FNUM SNUM I THIS PROGRESSBAREXEC  ��  � T� � � ��  �� U  LABLCAPTION THIS LBLINFO CAPTION)  T�  � ��  � �� T�  � ��  � �� U  THIS MINWIDTH WIDTH	 MINHEIGHT HEIGHT  U  	  G] � U   initproc,     �� progressbarexec�     �� cleaprogressbarm    �� showpbar�    �� incpbar�    ��	 setcationI    �� Load�    �� Init�    �� Release�    ��1 a �� 3 q � � !��A � A A 3 � � 3 � 3 qA 3 q 13 114 4 a 1                       X         ~   l        �  �        �  �          r        �  �  %      �     *   !   ;  =  0   "   [  h  4    )   �                       R���    9  9                        uZ   %   �      �     �          �  U  � ���  ����% %�C� Thisform.ofrmfromb� O��?� T�� ��� � �� %��� � � �� � ��� �  ��C� enableda� textbox�� ��! ��C� enableda� checkbox�� ��! ��C� enableda� combobox�� �� �,�  ��C� enabled-� textbox�� ��! ��C� enabled-� checkbox�� ��! ��C� enabled-� combobox�� �� � G�(��� � �� �& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� �	 �� T��
 �� �� �� U  THISFORM	 BACKCOLOR OFRMFROM EDITMODE ADDMODE SETALL DATASESSIONID	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT ICON ICOPATH  ��C--------------�  �� U  BARSTAT createstdobjects,     �� Activate^    ��1 � Q!�� A � A a1� � A 3 �2                       �        �        )   9                       3���                              ��   %   t      q  8             �  U  �  ���  ��� � %�C� Companyb� O��h � T�� �C� � g�� T�� �� � �� T�� �� �� T�� ��	 �� � ��C��
 ��& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� � �� �� U  THISFORM	 BACKCOLOR COMPANY VCOLOR	 CO_DTBASE DBNAME PLATFORM MVU_BACKEND ICON ICOPATH DEFAENV	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT  ��  Q�	 CHARACTER� U  TCVIEWPRINT�  ���  ��� �& %�C�
 tbrDesktopb� O� �� 	��� � %��� 
� �� 
	��l �! ��Caaaa--�� �� �� ---aa� �� �� � ��C------aa----aa� �� � ��C� �	 �� � ��C��	 �� �� U
  THISFORM	 ISTOOLBAR ADDMODE EDITMODE BARSTAT	 ADDBUTTON
 EDITBUTTON DELETEBUTTON
 TBRDESKTOP REFRESH� ( %�C�
 tbrDesktopb� O� �  � 	��� � %�C� � ��D � �� B�-�� � %�� � 
��d � �� B�-�� � ��C--------------� �� � U  THISFORM	 ISTOOLBAR
 TBRDESKTOP RESTORE FLAG BARSTAT`  %��  � ��Y � G � G� British� G � G2� G.� G0� Gw � G�(��  �� G�(�� �� � U  THIS LDEFAENV BRITISH  ��C�  � �� U  THISFORM STDACTIVATE  ��C�  � �� U  THISFORM
 STDQUNLOAD createstdobjects,     �� printing�    �� stdactivate�    ��
 stdqunload�    �� defaenv�    �� Activate    �� QueryUnloadJ    ��1 � �1� � A � a1� A 3 A3 � a�� �A � A � A 3 �A q A A q A �A 3 a � a a a a a � � A 4 � 2 � 2                       �        �  �          �        �    0   (   2  �  E   4   	  $	  T   6   F	  ]	  W    )                          ����    �  �                        ��	   %   �      �  $   �          �  U  5  G]� ��C� Please wait...!�  � �� ��C�  � �� U  THIS	 SETCATION SHOWPBAR�  4�  � %��  �d��% � ��C� � �� �� � %�� � -��� � T� � � �� � � �  �� �� ���(��d������y � �� ��C� � �� � � U  MVALUE THIS CLEAPROGRESSBAR SHOWPROGRESS CTL32_PROGRESSBAR1 VALUE A REFRESH  T�  � �a�� ��C�  � �� U  THIS SHOWPROGRESS RELEASE  T�  � �a�� U  THIS VISIBLEO ! ��  Q� INTEGER� Q� INTEGER� �� ��  �(�� ��H � ��C�� � �� �� U  FNUM SNUM I THIS PROGRESSBAREXEC  ��  � T� � � ��  �� U  LABLCAPTION THIS LBLINFO CAPTION)  T�  � ��  � �� T�  � ��  � �� U  THIS MINWIDTH WIDTH	 MINHEIGHT HEIGHT  U  	  G] � U   initproc,     �� progressbarexec�     �� cleaprogressbarm    �� showpbar�    �� incpbar�    ��	 setcationI    �� Load�    �� Init�    �� Release�    ��1 a �� 3 q � � !��A � A A 3 � � 3 � 3 qA 3 q 13 114 4 a 1                       X         ~   l        �  �        �  �          r        �  �  %      �     *   !   ;  =  0   "   [  h  4    )   �                       R���    9  9                        uZ   %   �      �     �          �  U  � ���  ����% %�C� Thisform.ofrmfromb� O��?� T�� ��� � �� %��� � � �� � ��� �  ��C� enableda� textbox�� ��! ��C� enableda� checkbox�� ��! ��C� enableda� combobox�� �� �,�  ��C� enabled-� textbox�� ��! ��C� enabled-� checkbox�� ��! ��C� enabled-� combobox�� �� � G�(��� � �� �& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� �	 �� T��
 �� �� �� U  THISFORM	 BACKCOLOR OFRMFROM EDITMODE ADDMODE SETALL DATASESSIONID	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT ICON ICOPATH  ��C--------------�  �� U  BARSTAT createstdobjects,     �� Activate^    ��1 � Q!�� A � A a1� � A 3 �2                       �        �        )   9                       3���                              ��   %   t      q  8             �  U  �  ���  ��� � %�C� Companyb� O��h � T�� �C� � g�� T�� �� � �� T�� �� �� T�� ��	 �� � ��C��
 ��& ��C�	 sqlconobj� sqlconnudobj�� ��# ��C� _stuffObject� _stuff�� �� ��C�� � �� �� U  THISFORM	 BACKCOLOR COMPANY VCOLOR	 CO_DTBASE DBNAME PLATFORM MVU_BACKEND ICON ICOPATH DEFAENV	 ADDOBJECT _STUFFOBJECT _OBJECTPAINT  ��  Q�	 CHARACTER� U  TCVIEWPRINT�  ���  ��� �& %�C�
 tbrDesktopb� O� �� 	��� � %��� 
� �� 
	��l �! ��Caaaa--�� �� �� ---aa� �� �� � ��C------aa----aa� �� � ��C� �	 �� � ��C��	 �� �� U
  THISFORM	 ISTOOLBAR ADDMODE EDITMODE BARSTAT	 ADDBUTTON
 EDITBUTTON DELETEBUTTON
 TBRDESKTOP REFRESH� ( %�C�
 tbrDesktopb� O� �  � 	��� � %�C� � ��D � �� B�-�� � %�� � 
��d � �� B�-�� � ��C--------------� �� � U  THISFORM	 ISTOOLBAR
 TBRDESKTOP RESTORE FLAG BARSTAT`  %��  � ��Y � G � G� British� G � G2� G.� G0� Gw � G�(��  �� G�(�� �� � U  THIS LDEFAENV BRITISH  ��C�  � �� U  THISFORM STDACTIVATE  ��C�  � �� U  THISFORM
 STDQUNLOAD createstdobjects,     �� printing�    �� stdactivate�    ��
 stdqunload�    �� defaenv�    �� Activate    �� QueryUnloadJ    ��1 � �1� � A � a1� A 3 A3 � a�� �A � A � A 3 �A q A A q A �A 3 a � a a a a a � � A 4 � 2 � 2                       �        �  �          �        �    0   (   2  �  E   4   	  $	  T   6   F	  ]	  W    )                          ����    �  �                        ��	   %   �      �  $   �          �  U  5  G]� ��C� Please wait...!�  � �� ��C�  � �� U  THIS	 SETCATION SHOWPBAR�  4�  � %��  �d��% � ��C� � �� �� � %�� � -��� � T� � � �� � � �  �� �� ���(��d������y � �� ��C� � �� � � U  MVALUE THIS CLEAPROGRESSBAR SHOWPROGRESS CTL32_PROGRESSBAR1 VALUE A REFRESH  T�  � �a�� ��C�  � �� U  THIS SHOWPROGRESS RELEASE  T�  � �a�� U  THIS VISIBLEO ! ��  Q� INTEGER� Q� INTEGER� �� ��  �(�� ��H � ��C�� � �� �� U  FNUM SNUM I THIS PROGRESSBAREXEC  ��  � T� � � ��  �� U  LABLCAPTION THIS LBLINFO CAPTION)  T�  � ��  � �� T�  � ��  � �� U  THIS MINWIDTH WIDTH	 MINHEIGHT HEIGHT  U  	  G] � U   initproc,     �� progressbarexec�     �� cleaprogressbarm    �� showpbar�    �� incpbar�    ��	 setcationI    �� Load�    �� Init�    �� Release�    ��1 a �� 3 q � � !��A � A A 3 � � 3 � 3 qA 3 q 13 114 4 a 1                       X         ~   l        �  �        �  �          r        �  �  %      �     *   !   ;  =  0   "   [  h  4    )   �                  0   m                 d  PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Class                                                                                               WINDOWS _RH20ODKH7 829973796v      �  �      �  M  <"  c4         `  m  �            +           COMMENT RESERVED                        B                                                                 WINDOWS _18U0KAIDG 8982065622  !    �      �      ��  _�            )              �  �           COMMENT RESERVED                        �      s                                                           WINDOWS _1BQ0V2Q491065453749m      3  }      �      ��  �          W  d  �                       COMMENT RESERVED                               �                                                           WINDOWS _RH20ODKH71065453828J      d  Y      ��  x  ��  �          4  A  ��          �  �           COMMENT RESERVED                        m      C                                                           WINDOWS _18M0PPFE51098812603�      �  �      )�      ��  ��          �  �  	          �  �           WINDOWS _1U1135DF31098812603�      �  �  �  m      x�  �  p  7                                       COMMENT RESERVED                        p      G                                                           k�                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      "Tahoma, 0, 8, 5, 13, 11, 21, 2, 0
      tpk      �currentdatetime
daystart
dayend
monthstart
monthend
yearstart
yearend
hourend
hours24format
hourstart
minuteend
minutestart
secondend
secondstart
setyearsellength
setstartendpositions
      Pixels      -Textbox only from DatePicX classlib, modified      Class      1      textbox      tpk     currentdatetime DateTime on field entry
lallowblankdate Allow blank dates
datetype Date Type 1-{mm/dd/yy},{mm-dd-yy},{mm.dd.yy}, 2-{dd/mm/yy},{dd-mm-yy},{dd.mm.yy}, 3-(yy/mm/dd),(yy-mm-dd),(yy.mm.dd)
datepartinitfocus Date part highlighted when control receives focus: 1- Month (default), 2-Day, 3-Year
yearsellength Selected length of the year ( 2 or 4 )
daystart Day Start position
dayend Day end position
monthstart Month start position
monthend Month end position
yearstart Year start position
yearend Year end position
lupdowndisabled Flag indicating whether the Up / Down arrow keys are disabled ( +/- functionality only )
lexitontabonly Exit the control only on (Shift+)Tab
calendar_form_ref referinta la formul cu calendar
cbbutton_name
_memberdata XML Metadata for customizable properties
ampmstart
hourend
hours24format
hourstart
minuteend
minutestart
secondend
secondstart
blankdatetimestring
*datepartinitfocus_assign 
*setyearsellength Sets the year selected length
*setstartendpositions Sets the Day, Month and Year Start and End positions
*lupdowndisabled_assign 
*lexitontabonly_assign 
*dropdown Show the calendar form
*width_assign 
*height_assign 
*left_assign 
*top_assign 
*visible_assign 
*enabled_assign 
*readonly_assign 
     $FontName = "Tahoma"
FontSize = 8
Alignment = 3
Value = (DATETIME())
Height = 22
Width = 115
NullDisplay = "(undefined)"
currentdatetime = 
lallowblankdate = .T.
datetype = 2
datepartinitfocus = 2
yearsellength = 4
daystart = 0
dayend = 1
monthstart = 3
monthend = 4
yearstart = 6
yearend = 9
lexitontabonly = .T.
calendar_form_ref = .NULL.
cbbutton_name = 
_memberdata = <?xml version="1.0" encoding="Windows-1252" standalone="yes"?><VFPData><memberdata name="cbbutton_name" type="property" display="cbButton_name" favorites="True"/><memberdata name="comboformref" type="property" display="comboFormRef"/></VFPData>
ampmstart = 20
hourend = 12
hourstart = 11
minuteend = 15
minutestart = 14
secondend = 18
secondstart = 17
blankdatetimestring = 19000101000000
Name = "tpk"
      textbox      !Arial, 0, 9, 5, 15, 12, 32, 3, 0
      cbbutton      Pixels      1small combo-button only, I only needed the button      Class      1      combobox      cbbutton      Z_memberdata XML Metadata for customizable properties
odatetextbox
*setpositionandsize 
     <Height = 22
Width = 21
_memberdata = <?xml version="1.0" encoding="Windows-1252" standalone="yes"?><VFPData><memberdata name="odatetextbox" type="property" display="oDateTextbox"/><memberdata name="setpositionandsize" type="method" display="setPositionAndSize"/></VFPData>
odatetextbox = null
Name = "cbbutton"
      combobox      "Tahoma, 0, 8, 5, 13, 11, 23, 2, 0
      dpk      qcurrentdate
daystart
dayend
monthstart
monthend
yearstart
yearend
setyearsellength
setstartendpositions
      Pixels      -Textbox only from DatePicX classlib, modified      Class      1      textbox      dpk      textbox      !Arial, 0, 9, 5, 15, 12, 32, 3, 0
      calendar_form_desktop      Pixels      Mcalendar form with Desktop = .T., instantiated when parent is top level form.      Class      1      calendar_form      !Arial, 0, 9, 5, 15, 12, 32, 3, 0
      calendar_form      Pixels      &the "always on top" form with calendar      Class      2      form      calendar_form      calendar_form_desktop      form      datepicker.vcx      .OLEObject = C:\WINDOWS\system32\mscomct2.ocx
      GTop = -1
Left = -1
Height = 100
Width = 100
Name = "oleMonthView"
      calendar_form      oleMonthView      
olecontrol      
olecontrol      �textboxref reference to textbox
oldvalue old value, for Escape/Deactivate
toformref
totextboxref
dpkname
mainfrm
*gettoplevelform get top level form reference (or _screen if doesn't exist)
      �Desktop = .T.
DoCreate = .T.
WindowType = 1
AlwaysOnTop = .F.
Name = "calendar_form_desktop"
oleMonthView.Top = 0
oleMonthView.Left = 0
oleMonthView.Height = 144
oleMonthView.Width = 148
oleMonthView.Name = "oleMonthView"
      form     ����    �  �                        �}   %   g        ,   �          �  U  �  ��  � H� �� �$ �C� � ��� D�	 C� � ���_ �' T� � � � �C� � � � � �	 $��$ �C� � ��� T�	 C� � ���� �: T� � � � �C� � � � � �	 C� � �C� � ���� � �� � �
 � U  DATECLICKED THISFORM OLDVALUE THIS PARENT
 TEXTBOXREF VALUE YEAR MONTH DAY RELEASE�  ��  � H� �� �$ �C� � ��� D�	 C� � ���_ �' T� � � � �C� � � � � �	 $��$ �C� � ��� T�	 C� � ���� �: T� � � � �C� � � � � �	 C� � �C� � ���� � �� � �
 � U  DATEDBLCLICKED THISFORM OLDVALUE THIS PARENT
 TEXTBOXREF VALUE YEAR MONTH DAY RELEASE�  ��  � � � H� �� �$ �C� � ��� D�	 C� � ���g �' T� � � � �C� �	 � �
 � � $��$ �C� � ��� T�	 C� � ���� �: T� � � � �C� �	 � �
 � � C� � �C� � ���� � U 	 STARTDATE ENDDATE CANCEL THISFORM OLDVALUE THIS PARENT
 TEXTBOXREF VALUE YEAR MONTH DAY  ��  � � U  KEYCODE SHIFT! ��  � H� �� ��  ���� � H�2 �� �$ �C� � ��� D�	 C� � ���} �' T� � � � �C� � � � � �	 $��$ �C� � ��� T�	 C� � ���� �: T� � � � �C� � � � � �	 C� � �C� � ���� �
 �� �
 � ��  ���� T� � � �� � ��
 �� �
 � � U  NKEYCODE THISFORM OLDVALUE THIS PARENT
 TEXTBOXREF VALUE YEAR MONTH DAY RELEASE	 DateClick,     �� DateDblClick^    ��	 SelChange�    �� KeyDown�    �� KeyPress�    ��1 r � AqA�A � 4 r � AqA�A � 3 � � AqA�A 2 � 3 r � � AqA�A � a� A 2                       �     
   �  �        �  p        �  �  )      �    .    )   �                       ����    �  �                        �   %   �      �  9   W          �  U  Y  ��  � ��� ��R � T�� ��  � �� T�� ��  � �  � ��� T�� ��  � �� �� U 	 TOTEXTBOX THIS TOP LEFT WIDTH HEIGHTQ  ��  � � � �) %�C� Thisform.ObjClickMoveb� L��J � T� � �-�� � U  NBUTTON NSHIFT NXCOORD NYCOORD THISFORM OBJCLICKMOVE  ��  � T� � ��  � �� U  OTEXTBOX THIS ODATETEXTBOX NAME� " T�  �C� this.parent.� � ���" %�C� oTextBox.valueb� D��� �2 %�CC�  � H���
� CC�  � i�l��
��� � T�  � �C�  #�� � �/ %�C� thisform.curonmouseb� L� C�a	��� � T� � �a�� � B�C��� U  OTEXTBOX THIS ODATETEXTBOX VALUE THISFORM
 CURONMOUSE�" T�  �C� this.parent.� � ��� ��C�  � ��c T� �CC� oTextBox.parent.whenexprb� C� � oTextBox.parent.whenexpr� � oTextBox.Whenexpr6�� IF !EMPTY(&zTextBox)�H� IF EVALUATE(&zTextBox)�� � %��  � a�	 �  � -	��� � ��C�  � �� � �D�A ��CC� oTextBox.parent.whenexprb� C� C�  � � � C�  � 6�� � �� %��  � a�	 �  � -	��{� ��C�  � �� � � U	  OTEXTBOX THIS ODATETEXTBOX SETFOCUS ZTEXTBOX ENABLED READONLY DROPDOWN PARENT< ' %�C� thisform.curonmouseb� L��5 � T�  � �-�� � U  THISFORM
 CURONMOUSE< ' %�C� thisform.curonmouseb� L��5 � T�  � �-�� � U  THISFORM
 CURONMOUSEQ  ��  � � � �) %�C� Thisform.ObjClickMoveb� L��J � T� � �a�� � U  NBUTTON NSHIFT NXCOORD NYCOORD THISFORM OBJCLICKMOVE setpositionandsize,     ��
 MouseLeave�     �� InitD    �� When�    �� DropDown�    �� GotFocus�    ��	 LostFocus�    ��	 MouseMove1    ��1 q � �A 3 1�� A 3 q 23 !!!!A A �� A � 3 !� 2���� A � A � �� A A 9 r� A 3 r� A 4 1�� A 3                       �         �   \  
      w  �        �  G        f  8  %   ,   W  �  ?   0   �  1  F   4   Q  �  N    )   �                       ],PROCEDURE datepartinitfocus_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method

IF INLIST( m.vNewVal, 1, 2, 3 )	&& Month, Day, Year
	THIS.DatePartInitFocus = m.vNewVal
ENDIF
ENDPROC
PROCEDURE setyearsellength
WITH THIS
	DO CASE
		CASE .Century = 0	&& Off
			.YearSelLength = 2

		CASE .Century = 1	&& On
			.YearSelLength = 4

		CASE .Century = 2	&& Default
			IF SET( "CENTURY" ) = "ON"
				.YearSelLength = 4
			ELSE
				.YearSelLength = 2
			ENDIF
	ENDCASE
ENDWITH

ENDPROC
PROCEDURE setstartendpositions
*	We set the positions for testing when moving
*	between date parts, etc.
WITH THIS
	DO CASE
		CASE .DateType = 1			&& MDY 01/34/6789
			.DayStart = 3
			.DayEnd = 4
			.MonthStart = 0
			.MonthEnd = 1
			.YearStart = 6
			.YearEnd = 7			&& Century Off	(01/34/67)

			IF .YearSelLength = 4	&& Century On	(01/34/6789)
				.YearEnd = .YearEnd + 2
			ENDIF

		CASE .DateType = 2			&& DMY 01/34/6789
			.DayStart = 0
			.DayEnd = 1
			.MonthStart = 3
			.MonthEnd = 4
			.YearStart = 6
			.YearEnd = 7			&& Century Off	(01/34/67)

			IF .YearSelLength = 4	&& Century On	(01/34/6789)
				.YearEnd = .YearEnd + 2
			ENDIF

		OTHERWISE					&& YMD 0123/56/89
			.YearStart = 0

			.YearEnd = 1			&& Century Off	(01/34/67)
			.MonthStart = 3
			.MonthEnd = 4
			.DayStart = 6
			.DayEnd = 7

			IF .YearSelLength = 4	&& Century On	(0123/56/89)
				.YearEnd = .YearEnd + 2
				.MonthStart = .MonthStart + 2
				.MonthEnd = .MonthEnd + 2
				.DayStart = .DayStart + 2
				.DayEnd = .DayEnd + 2
			ENDIF
	ENDCASE

*	Time data
	.HourStart = 9		&& Century Off
	.MinuteStart = 12
	.SecondStart = 15
	.AMPMStart = 18
	.Hours24Format = .F.

	IF .YearSelLength = 4	&& Century On (01/34/6789)
		.HourStart = .HourStart + 2
		.MinuteStart = .MinuteStart + 2
		.SecondStart = .SecondStart + 2
		.AMPMStart = .AMPMStart + 2
	ENDIF

	.HourEnd = .HourStart + 1
	.MinuteEnd = .MinuteStart + 1
	.SecondEnd = .SecondStart + 1

	IF ( .Hours = 24 OR ( .Hours = 0 AND SET( "HOURS" ) = 24 ) )
		.AMPMStart = .SecondStart	&& 24 hour Military time
		.Hours24Format = .T.
	ENDIF

ENDWITH

ENDPROC
PROCEDURE lupdowndisabled_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF VARTYPE( vNewVal ) = 'L'
	THIS.lUpDownDisabled = m.vNewVal
ENDIF

ENDPROC
PROCEDURE lexitontabonly_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF VARTYPE( m.vNewVal ) = 'L'
	THIS.lExitOnTabOnly = m.vNewVal
ENDIF

ENDPROC
PROCEDURE dropdown
IF thisform.ShowWindow = 2
	this.calendar_form_ref = CREATEOBJECT('calendar_Form_desktop',this,thisform)
ELSE 
	this.calendar_form_ref = CREATEOBJECT('calendar_Form',this,thisform)
ENDIF 
this.calendar_form_ref.AlwaysOnTop = .T.
this.calendar_form_ref.Show()

ENDPROC
PROCEDURE width_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF vNewVal <> this.Width
	this.Width = vNewVal 
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE height_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF vNewVal <> this.Height 
	this.Height = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE left_assign
LPARAMETERS vNewVal
IF vNewVal <> this.left
	this.left = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE top_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF vNewVal <> this.Top
	this.top = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE visible_assign
LPARAMETERS vNewVal
IF vNewVal <> this.Visible
	this.Visible = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.Visible = vNewVal
	
ENDIF 

ENDPROC
PROCEDURE enabled_assign
LPARAMETERS vNewVal
IF vNewVal <> this.Enabled
	this.Enabled = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.Enabled = vNewVal and this.ReadOnly = .f.  
ENDIF 

ENDPROC
PROCEDURE readonly_assign
LPARAMETERS vNewVal
IF vNewVal <> this.ReadOnly
	this.ReadOnly = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.Enabled = NOT vNewVal AND NOT this.Enabled = .f. 
ENDIF 

ENDPROC
PROCEDURE Refresh
IF VARTYPE(this.Value) = 'D'
	IF VAL(TTOC(this.value,1)) = this.blankDateTimeString 
		this.ForeColor = this.BackColor 
	ELSE
		this.ForeColor = this.Parent.ForeColor
	ENDIF 
ENDIF

ENDPROC
PROCEDURE Click
THIS.SetFocus()
ENDPROC
PROCEDURE InteractiveChange
WITH THIS
	IF INLIST( .DateType, 1, 2 )	&& MDY, DMY
		DO CASE
			CASE .SelStart = 3
				.SelLength = 2

			CASE .SelStart = 6
				.SelLength = .YearSelLength

			CASE INLIST( .SelStart, .HourStart, .MinuteStart, .SecondStart )
				.SelLength = 2
		ENDCASE
	ELSE							&& YMD
		IF INLIST( .SelStart, .MonthStart, .DayStart .HourStart, .MinuteStart, .SecondStart )	&& 5, 8
			.SelLength = 2
		ENDIF
	ENDIF
	IF VARTYPE(this.Value) = 'D'
		IF VAL(TTOC(this.value,1)) = this.blankDateTimeString 
			.ForeColor = .BackColor 
		ELSE
			.ForeColor = .Parent.ForeColor
		ENDIF 
	ENDIF	
ENDWITH

ENDPROC
PROCEDURE KeyPress
LPARAMETERS nKeyCode, nShiftAltCtrl

#DEFINE	SECONDS_PER_MINUTE	60
#DEFINE	SECONDS_PER_HOUR	3600
#DEFINE	SECONDS_PER_DAY		86400

WITH THIS
	DO CASE
		CASE nKeyCode = 32					&& Space Bar ( DATE() )
			NODEFAULT
			IF .ReadOnly
				RETURN
			ENDIF
			.Value = DATE()
			.REFRESH
			.SETFOCUS()
		
		CASE nKeyCode = 1					&& Home ( Last Date entered )
			NODEFAULT
			IF .ReadOnly
				RETURN
			ENDIF
			.Value = .CurrentDateTime
			.REFRESH
			.SETFOCUS()
		
		CASE nKeyCode = 4		&& Right Arrow
			IF .ReadOnly
				NODEFAULT
				RETURN
			ENDIF
			IF .DateType = 3	&& YMD 0123/56/89 12:45:78 01, 01/34/67 12:45:78 01
								&&	   YYYY/MM/DD HH:MM:SS PM, YY/MM/DD HH:MM:SS PM
				DO CASE
					CASE .SelStart < .MonthStart	&& 5
						NODEFAULT
						.SelStart = .MonthStart		&& Moving to second date part ( Month )
						.SelLength = 2

					CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& 3, 8
						NODEFAULT
						.SelStart = .DayStart		&& Moving to third date part ( Day )
						.SelLength = 2
					
					CASE .SelStart > .MonthEnd AND .SelStart < .HourStart
						NODEFAULT
						.SelStart = .HourStart		&& Moving to Hour time part
						.SelLength = 2

					CASE .SelStart > .DayEnd AND .SelStart < .MinuteStart
						NODEFAULT
						.SelStart = .MinuteStart	&& Moving to Minute time part
						.SelLength = 2

					CASE .SelStart > .HourEnd AND .SelStart < .SecondStart
						NODEFAULT
						.SelStart = .SecondStart	&& Moving to Second time part
						.SelLength = 2

					CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart
						NODEFAULT
						.SelStart = .AMPMStart		&& Moving to AMPM time part
						.SelLength = 2

					CASE .SelStart >= .AMPMStart
						NODEFAULT
						IF NOT .lExitOnTabOnly
							KEYBOARD '{TAB}'		&& Exiting the control
						ENDIF
					
					OTHERWISE
						NODEFAULT
				ENDCASE
			ELSE				&& MDY/DMY 01/34/6789 12:45:78 01, 01/34/67 12:45:78 01
								&&	   	   MM/DD/YYYY HH:MM:SS PM, MM/DD/YY HH:MM:SS PM
				DO CASE			&& Month and Day SelLengths are the same so we use
					CASE .SelStart < 3	&& Literals for performance here when we can
						NODEFAULT
						.SelStart = 3	&& Moving to second date part ( Day or Month )
						.SelLength = 2

					CASE .SelStart > 1 AND .SelStart < 6
						NODEFAULT
						.SelStart = 6	&& Moving to third date part ( Year )
						.SelLength = .YearSelLength

					CASE .SelStart >= .YearStart AND .SelStart < .HourStart
						NODEFAULT
						.SelStart = .HourStart		&& Moving to Hour time part
						.SelLength = 2

					CASE .SelStart > .YearEnd AND .SelStart < .MinuteStart
						NODEFAULT
						.SelStart = .MinuteStart	&& Moving to Minute time part
						.SelLength = 2

					CASE .SelStart > .HourEnd AND .SelStart < .SecondStart
						NODEFAULT
						.SelStart = .SecondStart	&& Moving to Second time part
						.SelLength = 2

					CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart
						NODEFAULT
						.SelStart = .AMPMStart		&& Moving to AMPM time part
						.SelLength = 2
				
					CASE .SelStart >= .AMPMStart
						NODEFAULT
						IF NOT .lExitOnTabOnly
							KEYBOARD '{TAB}'		&& Exiting the control
						ENDIF
					
					OTHERWISE
						NODEFAULT
				ENDCASE
			ENDIF

		CASE nKeyCode = 19					&& Left Arrow
			IF .ReadOnly
				NODEFAULT
				RETURN
			ENDIF
			IF .DateType = 3	&& YMD 0123/56/89 12:45:78 01, 01/34/67 12:45:78 01
								&&	   YYYY/MM/DD HH:MM:SS PM, YY/MM/DD HH:MM:SS PM
				DO CASE
					CASE .Hours24Format AND .SelStart >= .SecondStart	&& Moving to Minute time part
						NODEFAULT
						.SelStart = .MinuteStart	&& Moving to Minute time part
						.SelLength = 2

					CASE .SelStart > .SecondEnd
						NODEFAULT
						.SelStart = .SecondStart	&& Moving to Seconds time part
						.SelLength = 2

					CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart
						NODEFAULT
						.SelStart = .MinuteStart	&& Moving to Minute time part
						.SelLength = 2

					CASE .SelStart > .HourEnd AND .SelStart < .SecondStart
						NODEFAULT
						.SelStart = .HourStart	&& Moving to Hour time part
						.SelLength = 2

					CASE .SelStart > .DayEnd AND .SelStart < .MinuteStart
						NODEFAULT
						.SelStart = .DayStart	&& Moving to third date part ( Day )
						.SelLength = 2

					CASE .SelStart > .MonthEnd AND .SelStart < .HourStart	&& 7
						NODEFAULT
						.SelStart = .MonthStart	&& Moving to second date part ( Month )
						.SelLength = 2

					CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& 3, 8
						NODEFAULT
						.SelStart = 0	&& Moving to first date part ( Year )
						.SelLength = .YearSelLength
					
					CASE .SelStart < .MonthStart
						NODEFAULT
						IF NOT .lExitOnTabOnly
							KEYBOARD '{BACKTAB}'	&& Exiting the control
						ENDIF
					
					OTHERWISE
						NODEFAULT
				ENDCASE
			ELSE				&& MDY/DMY 01/34/6789 12:45:78 01, 01/34/67 12:45:78 01
								&&	   	   MM/DD/YYYY HH:MM:SS PM, MM/DD/YY HH:MM:SS PM
				DO CASE			&& Month and Day SelLengths are the same so we use literal when we can
					CASE .Hours24Format AND .SelStart >= .SecondStart	&& Moving to Minute time part
						NODEFAULT
						.SelStart = .MinuteStart	&& Moving to Minute time part
						.SelLength = 2

					CASE .SelStart > .SecondEnd
						NODEFAULT
						.SelStart = .SecondStart	&& Moving to Seconds time part
						.SelLength = 2

					CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart
						NODEFAULT
						.SelStart = .MinuteStart	&& Moving to Minute time part
						.SelLength = 2

					CASE .SelStart > .HourEnd AND .SelStart < .SecondStart
						NODEFAULT
						.SelStart = .HourStart	&& Moving to Hour time part
						.SelLength = 2

					CASE .SelStart > .YearEnd AND .SelStart < .MinuteStart
						NODEFAULT
						.SelStart = .YearStart	&& Moving to third date part ( Year )
						.SelLength = .YearSelLength

					CASE .SelStart > 4 AND .SelStart < .HourStart
						NODEFAULT
						.SelStart = 3	&& Moving to second date part ( Day or Month )
						.SelLength = 2

					CASE .SelStart > 1 AND .SelStart < 6
						NODEFAULT
						.SelStart = 0	&& Moving to first date part ( Month or Day )
						.SelLength = 2
					
					CASE .SelStart < 3
						NODEFAULT
						IF NOT .lExitOnTabOnly
							KEYBOARD '{BACKTAB}'	&& Exiting the control
						ENDIF
					
					OTHERWISE
						NODEFAULT
				ENDCASE
			ENDIF

		CASE INLIST( nKeyCode, 5, 43, 61 )	&& Up Arrow, '+'('=') ( Increment )
			NODEFAULT
			IF ( ( .lUpDownDisabled AND nKeyCode = 5 ) OR .ReadOnly )	&& Up Arrow disabled
				RETURN
			ENDIF

			DO CASE
				CASE .DateType = 1	&& MDY 01/34/6789 12:45:78 01, 01/34/67 12:45:78 01
									&&	   MM/DD/YYYY HH:MM:SS PM, MM/DD/YY HH:MM:SS PM
					DO CASE			&& Use literals whenever possible for performance
						CASE .SelStart < 3						&& Month
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, 1 ) )
							.REFRESH
							.SelStart = 0
							.SelLength = 2

						CASE .SelStart > 1 AND .SelStart < 6	&& Day
							.Value = .Value + SECONDS_PER_DAY
							.REFRESH
							.SelStart = 3
							.SelLength = 2

						CASE .SelStart > 4 AND .SelStart < .HourStart	&& Year
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, 12 ) )
							.REFRESH
							.SelStart = 6
							.SelLength = .YearSelLength	&& 4

						CASE .SelStart > .YearEnd AND .SelStart < .MinuteStart	&& Hour
							.Value = .Value + SECONDS_PER_HOUR
							.REFRESH
							.SelStart = .HourStart
							.SelLength = 2

						CASE .SelStart > .HourEnd AND .SelStart < .SecondStart	&& Minute
							.Value = .Value + SECONDS_PER_MINUTE
							.REFRESH
							.SelStart = .MinuteStart
							.SelLength = 2

						CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart	&& Second
							.Value = .Value + 1
							.REFRESH
							.SelStart = .SecondStart
							.SelLength = 2

						CASE .SelStart >= .AMPMStart			&& AM/PM or Seconds
							.Value = .Value + IIF( .Hours24Format, 1, SECONDS_PER_DAY / 2 )
							.REFRESH
							.SelStart = .AMPMStart
							.SelLength = 2

						OTHERWISE
							NODEFAULT
					ENDCASE

				CASE .DateType = 2	&& DMY 01/34/6789 12:45:78 01, 01/34/67 12:45:78 01
									&&	   DD/MM/YYYY HH:MM:SS PM, DD/MM/YY HH:MM:SS PM
					DO CASE
						CASE .SelStart < 3						&& Day
							.Value = .Value + SECONDS_PER_DAY
							.REFRESH
							.SelStart = 0
							.SelLength = 2

						CASE .SelStart > 1 AND .SelStart < 6	&& Month
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, 1 ) )
							.REFRESH
							.SelStart = 3
							.SelLength = 2

						CASE .SelStart > 4 AND .SelStart < .HourStart	&& Year
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, 12 ) )
							.REFRESH
							.SelStart = 6
							.SelLength = .YearSelLength	&& 4

						CASE .SelStart > .YearEnd AND .SelStart < .MinuteStart	&& Hour
							.Value = .Value + SECONDS_PER_HOUR
							.REFRESH
							.SelStart = .HourStart
							.SelLength = 2

						CASE .SelStart > .HourEnd AND .SelStart < .SecondStart	&& Minute
							.Value = .Value + SECONDS_PER_MINUTE
							.REFRESH
							.SelStart = .MinuteStart
							.SelLength = 2

						CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart	&& Second
							.Value = .Value + 1
							.REFRESH
							.SelStart = .SecondStart
							.SelLength = 2

						CASE .SelStart >= .AMPMStart			&& AM/PM or Seconds
							.Value = .Value + IIF( .Hours24Format, 1, SECONDS_PER_DAY / 2 )
							.REFRESH
							.SelStart = .AMPMStart
							.SelLength = 2

						OTHERWISE
							NODEFAULT
					ENDCASE

				OTHERWISE			&& YMD 0123/56/89 12:45:78 01, 01/34/67 12:45:78 01
									&&	   YYYY/MM/DD HH:MM:SS PM, YY/MM/DD HH:MM:SS PM
					DO CASE
						CASE .SelStart < .MonthStart	&& 5	&& Year
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, 12 ) )
							.REFRESH
							.SelStart = 0
							.SelLength = .YearSelLength	&& 4

						CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& Month ( 3, 8 )
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, 1 ) )
							.REFRESH
							.SelStart = .MonthStart	&& 5
							.SelLength = 2

						CASE .SelStart >= .MonthEnd AND .SelStart < .HourStart	&& Day
							.Value = .Value + SECONDS_PER_DAY
							.REFRESH
							.SelStart = .DayStart	&& 8
							.SelLength = 2

						CASE .SelStart > .DayEnd AND .SelStart < .MinuteStart	&& Hour
							.Value = .Value + SECONDS_PER_HOUR
							.REFRESH
							.SelStart = .HourStart
							.SelLength = 2

						CASE .SelStart > .HourEnd AND .SelStart < .SecondStart	&& Minute
							.Value = .Value + SECONDS_PER_MINUTE
							.REFRESH
							.SelStart = .MinuteStart
							.SelLength = 2

						CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart	&& Second
							.Value = .Value + 1
							.REFRESH
							.SelStart = .SecondStart
							.SelLength = 2

						CASE .SelStart >= .AMPMStart			&& AM/PM or Seconds
							.Value = .Value + IIF( .Hours24Format, 1, SECONDS_PER_DAY / 2 )
							.REFRESH
							.SelStart = .AMPMStart
							.SelLength = 2

						OTHERWISE
							NODEFAULT
					ENDCASE
				ENDCASE

		CASE INLIST( nKeyCode, 24, 45 )	&& Down Arrow, '-' ( Decrement )
			NODEFAULT
			IF ( ( .lUpDownDisabled AND nKeyCode = 24 ) OR .ReadOnly )	&& Down Arrow disabled
				RETURN
			ENDIF

			DO CASE
				CASE .DateType = 1	&& MDY 01/34/6789 12:45:78 01, 01/34/67 12:45:78 01
									&&	   MM/DD/YYYY HH:MM:SS PM, MM/DD/YY HH:MM:SS PM
					DO CASE			&& Use literals whenever possible for performance
						CASE .SelStart < 3						&& Month
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, -1 ) )
							.REFRESH
							.SelStart = 0
							.SelLength = 2

						CASE .SelStart > 1 AND .SelStart < 6	&& Day
							.Value = .Value - SECONDS_PER_DAY
							.REFRESH
							.SelStart = 3
							.SelLength = 2

						CASE .SelStart > .DayEnd AND .SelStart < .HourStart	&& Year
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, -12 ) )
							.REFRESH
							.SelStart = 6
							.SelLength = .YearSelLength

						CASE .SelStart > .YearEnd AND .SelStart < .MinuteStart	&& Hour
							.Value = .Value - SECONDS_PER_HOUR
							.REFRESH
							.SelStart = .HourStart
							.SelLength = 2

						CASE .SelStart > .HourEnd AND .SelStart < .SecondStart	&& Minute
							.Value = .Value - SECONDS_PER_MINUTE
							.REFRESH
							.SelStart = .MinuteStart
							.SelLength = 2

						CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart	&& Second
							.Value = .Value - 1
							.REFRESH
							.SelStart = .SecondStart
							.SelLength = 2

						CASE .SelStart >= .AMPMStart			&& AM/PM or Seconds
							.Value = .Value - IIF( .Hours24Format, 1, SECONDS_PER_DAY / 2 )
							.REFRESH
							.SelStart = .AMPMStart
							.SelLength = 2

						OTHERWISE
							NODEFAULT
					ENDCASE

				CASE .DateType = 2	&& DMY 01/34/6789 12:45:78 01, 01/34/67 12:45:78 01
									&&	   DD/MM/YYYY HH:MM:SS PM, DD/MM/YY HH:MM:SS PM
					DO CASE
						CASE .SelStart < 3						&& Day
							.Value = .Value - SECONDS_PER_DAY
							.REFRESH
							.SelStart = 0
							.SelLength = 2

						CASE .SelStart > 1 AND .SelStart < 6	&& Month
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, -1 ) )
							.REFRESH
							.SelStart = 3
							.SelLength = 2

						CASE .SelStart > .MonthEnd AND .SelStart < .HourStart	&& Year
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, -12 ) )
							.REFRESH
							.SelStart = 6
							.SelLength = .YearSelLength

						CASE .SelStart > .YearEnd AND .SelStart < .MinuteStart	&& Hour
							.Value = .Value - SECONDS_PER_HOUR
							.REFRESH
							.SelStart = .HourStart
							.SelLength = 2

						CASE .SelStart > .HourEnd AND .SelStart < .SecondStart	&& Minute
							.Value = .Value - SECONDS_PER_MINUTE
							.REFRESH
							.SelStart = .MinuteStart
							.SelLength = 2

						CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart	&& Second
							.Value = .Value - 1
							.REFRESH
							.SelStart = .SecondStart
							.SelLength = 2

						CASE .SelStart >= .AMPMStart			&& AM/PM or Seconds
							.Value = .Value - IIF( .Hours24Format, 1, SECONDS_PER_DAY / 2 )
							.REFRESH
							.SelStart = .AMPMStart
							.SelLength = 2

						OTHERWISE
							NODEFAULT
					ENDCASE

				OTHERWISE			&& YMD 0123/56/89 12:45:78 01, 01/34/67 12:45:78 01
									&&	   YYYY/MM/DD HH:MM:SS PM, YY/MM/DD HH:MM:SS PM
					DO CASE
						CASE .SelStart < .MonthStart	&& 5	&& Year
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, -12 ) )
							.REFRESH
							.SelStart = 0
							.SelLength = .YearSelLength	&& 4

						CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& Month ( 3, 8 )
							.Value = ( .Value - DTOT( TTOD( .Value ) ) ) + ;
										DTOT( GOMONTH( .Value, -1 ) )
							.REFRESH
							.SelStart = .MonthStart	&& 5
							.SelLength = 2

						CASE .SelStart > .MonthEnd AND .SelStart < .HourStart	&& Day
							.Value = .Value - SECONDS_PER_DAY
							.REFRESH
							.SelStart = .DayStart	&& 8
							.SelLength = 2

						CASE .SelStart > .DayEnd AND .SelStart < .MinuteStart	&& Hour
							.Value = .Value + SECONDS_PER_HOUR
							.REFRESH
							.SelStart = .HourStart
							.SelLength = 2

						CASE .SelStart > .HourEnd AND .SelStart < .SecondStart	&& Minute
							.Value = .Value + SECONDS_PER_MINUTE
							.REFRESH
							.SelStart = .MinuteStart
							.SelLength = 2

						CASE .SelStart > .MinuteEnd AND .SelStart < .AMPMStart	&& Second
							.Value = .Value + 1
							.REFRESH
							.SelStart = .SecondStart
							.SelLength = 2

						CASE .SelStart >= .AMPMStart			&& AM/PM or Seconds
							.Value = .Value - IIF( .Hours24Format, 1, SECONDS_PER_DAY / 2 )
							.REFRESH
							.SelStart = .AMPMStart
							.SelLength = 2

						OTHERWISE		&& AM/PM
							NODEFAULT
					ENDCASE
			ENDCASE

		CASE nKeyCode = 7								&& Del
			IF ( .ReadOnly OR NOT .lAllowBlankDate )
				NODEFAULT
				RETURN
			ENDIF
			.Value = {  /  /  }

		CASE ( ( nKeyCode > 47 AND nKeyCode < 60 ) OR INLIST( nKeyCode, 65, 97, 80, 112 ) )
			IF .ReadOnly	&& Pass Digits, 'A', 'a', 'P' and 'p' if editable
				NODEFAULT
				RETURN
			ENDIF

		CASE INLIST( nKeyCode, 9, 15 )					&& Pass TAB, Shift+TAB

		CASE ( nKeyCode = 13 )							&& Pass the Enter key

		CASE ( nKeyCode = 127 )							&& Pass the Backspace key

		CASE ( nKeyCode = 27 )							&& Pass the Escape key

		CASE nKeyCode = 145 OR nKeycode = -3 OR nKeyCode = 160
			this.DropDown()
		OTHERWISE										&& Inhibit all other key strokes
			NODEFAULT
	ENDCASE
ENDWITH

ENDPROC
PROCEDURE GotFocus
LOCAL lnStart, lnLength

WITH THIS
	.SetYearSelLength()		&& Set these property values each time we enter
	.SetStartEndPositions()	&& the control in case of programmatic changes
	.CurrentDateTime = .Value

	DO CASE
		CASE .DatePartInitFocus = 2	&& Day
			lnLength = 2
			lnStart = .DayStart

		CASE .DatePartInitFocus = 3	&& Year
			lnLength = .YearSelLength
			lnStart = .YearStart

		OTHERWISE					&& Month
			lnLength = 2
			lnStart = .MonthStart
	ENDCASE

	.SelStart = lnStart
	.SelLength = lnLength
ENDWITH

ENDPROC
PROCEDURE Valid
IF EMPTY( THIS.VALUE )
	IF NOT THIS.lAllowBlankDate
		RETURN 0
	ENDIF
ELSE
	IF YEAR( THIS.VALUE ) = 0
		RETURN 0
	ENDIF
ENDIF

ENDPROC
PROCEDURE Init
WITH THIS
	.SetYearSelLength()
	.SetStartEndPositions()
	.cbButton_name = SYS(2015)
	IF NOT INLIST( .Parent.BaseClass, 'Toolbar','Column')
		.Parent.NewObject( 	.cbButton_name ,'cbButton', '','',this)
		loBtn = EVALUATE( '.parent.'+.cbButton_name)
		loBtn.SetPositionAndSize(this)
		loBtn.Visible = this.Visible 
	ENDIF 
ENDWITH

ENDPROC
     
 ��ࡱ�                >  ��	                               ����        ��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������R o o t   E n t r y                                               ��������                               �w�S���           O l e O b j e c t D a t a                                            ����                                        �        A c c e s s O b j S i t e D a t a                             &  ������������                                       \        C h a n g e d P r o p s                                         ������������                                       D             ����   ����      ����������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������jE.#Ç���  �uM�!C4   (  L  �_�   $   �  �            \                          $   8                       651A8940-87C5-11d1-8BE3-0000F8754DA1�          (	        �        O    �   TitleBackColor 	   I
     y    TitleForeColor 	   I
   ���      �.�  p�.�p  ��. �  p�� p  &  �֛w�֛w�֛w�֛w'           �           �       �             �ͫ       �9    ��� �� ���   y ��  �� R������ � K�Q   ��$ Tahoma֛w�֛w�֛w�֛wp֛w�  �      �       'PROCEDURE DateClick
*** ActiveX Control Event ***
LPARAMETERS dateclicked
DO CASE 
CASE VARTYPE(thisform.oldvalue) = 'D' OR ISNULL(thisform.oldvalue)
	this.Parent.textboxref.value = DATE(this.year,this.month,this.day)
CASE VARTYPE(thisform.oldvalue) = 'T' OR ISNULL(thisform.oldvalue)
	this.Parent.textboxref.value = DATETIME(this.year,this.month,this.day ;
				,HOUR(thisform.oldvalue), MINUTE(thisform.oldvalue))
ENDCASE 

this.Parent.Release 


ENDPROC
PROCEDURE DateDblClick
*** ActiveX Control Event ***
LPARAMETERS datedblclicked
DO CASE 
CASE VARTYPE(thisform.oldvalue) = 'D' OR ISNULL(thisform.oldvalue)
	this.Parent.textboxref.value = DATE(this.year,this.month,this.day)
CASE VARTYPE(thisform.oldvalue) = 'T' OR ISNULL(thisform.oldvalue)
	this.Parent.textboxref.value = DATETIME(this.year,this.month,this.day ;
				,HOUR(thisform.oldvalue), MINUTE(thisform.oldvalue))
ENDCASE 

this.Parent.Release 

ENDPROC
PROCEDURE SelChange
*** ActiveX Control Event ***
LPARAMETERS startdate, enddate, cancel
DO CASE 
CASE VARTYPE(thisform.oldvalue) = 'D' OR ISNULL(thisform.oldvalue)
	this.Parent.textboxref.value = DATE(this.year,this.month,this.day)
CASE VARTYPE(thisform.oldvalue) = 'T' OR ISNULL(thisform.oldvalue)
	this.Parent.textboxref.value = DATETIME(this.year,this.month,this.day ;
				,HOUR(thisform.oldvalue), MINUTE(thisform.oldvalue))
ENDCASE
ENDPROC
PROCEDURE KeyDown
*** ActiveX Control Event ***
LPARAMETERS keycode, shift

ENDPROC
PROCEDURE KeyPress
*** ActiveX Control Event ***
LPARAMETERS nKeyCode

DO case
case nKeyCode = 13
	DO CASE 
	CASE VARTYPE(thisform.oldvalue) = 'D' OR ISNULL(thisform.oldvalue)
		this.Parent.textboxref.value = DATE(this.year,this.month,this.day)
	CASE VARTYPE(thisform.oldvalue) = 'T' OR ISNULL(thisform.oldvalue)
		this.Parent.textboxref.value = DATETIME(this.year,this.month,this.day ;
				,HOUR(thisform.oldvalue), MINUTE(thisform.oldvalue))
	ENDCASE
 	thisform.Release 
CASE nKeyCode = 27
	thisform.textboxref.Value = thisform.oldvalue 
	thisform.Release
ENDCASE

ENDPROC
     	c���    J	  J	                        ��   %   �      �  V             �  U  W9 ��C� _SCREEN.ActiveFormb� O� C�9�  � � ����� H�F �G�D ��9� � �. C� _SCREEN.ActiveFormb� O� �9�  � � 	��� � T� ��9��7 �C� _SCREEN.ActiveFormb� O� �9�  � �	��� � T� ��9�  �� 2�G� �� �9� �� %�� � ���� T� �� �� !� � �� %�C� ��� O��C� T� ��9�� � �	 B�� �� U 
 ACTIVEFORM
 SHOWWINDOW	 FORMCOUNT	 LOTOPFORM LOFORM FORMSS %�C�  � �
��L�/ %�C� Thisform.Mainfrm.Curonmouseb� L��H� T�  � � �-��$ T� �� Thisform.Mainfrm�  � �� with &_toTextboxRef�&� %�C�� �
��"� �� � � T� �C�� ���� T� �C�� ����
 F�� �� %�C� b� T���0 replace &tfldname with .value in &taliasname
 � � �� &_toTextboxRef..setfocus()
 � � U	  THISFORM	 TOFORMREF MAINFRM
 CURONMOUSE _TOTEXTBOXREF DPKNAME UCONTROLSOURCE TFLDNAME
 TALIASNAME 
 ��  � � U  THIS RELEASE� ��  � � � T� � �� �� T� � ��  �� T� �  ��  �� T� � �� �� %�C�	 ToformRefb� O�� � T� � �� � �� �  %�C� toTextboxRefb� O��� � T� �  ��  � �� �( %�C� toformRef.curonmouseb� L��� � T� � �a�� � %�C�  �� C�  ��� O��� B� � �� � T� �C�	 �
 �� ���	 ���� T�� ��  �� T�� ��� � ��" %�C�� � �
� C�� � �
	���� T�	 � � �C�� � i�� T�	 � � �C�� � H�� T�	 � � �C�� � %�� �� T�	 � � �CC$i�� T�	 � � �CC$H�� T�	 � � �CC$%�� �2 T�� ��� � C�� ��]C��%C�	�%� � ��# T�� �C�� ��]C��%� � ��0 %��� �� C��%C��%�{�G�z�?����# T�� ��� C��%�{�G�z�?�� � �� U  TOTEXTBOXREF	 TOFORMREF _DPKNAME THISFORM MAINFRM DPKNAME NAME
 CURONMOUSE OTOPLEVELFORM THIS GETTOPLEVELFORM
 TEXTBOXREF OLDVALUE VALUE OLEMONTHVIEW YEAR MONTH DAY TOP HEIGHT LEFT WIDTHF  ��  � T� � � �� � �� T� � � �� � �� ��C� � � �� U  NSTYLE THIS OLEMONTHVIEW WIDTH HEIGHT SETFOCUS gettoplevelform,     �� Destroy�    ��
 Deactivate�    �� Init�    �� Show<    ��1 �� C� s� � D� A A A R� A B � 3 2�A�� � AA A A �A @A 4 � 2 � �1A 1A �� A �A A q � � !"���� BAAB $11A PA 2 q aa2                       z        �  ?
  )   (   `
  l
  U   *   �
  >  X   Q   Y  �  �    )   J	                       E���    ,  ,                        +J   %   <      �  !  A          �  U  :  ��  � %�C��  ������3 � T� � ���  �� � U  VNEWVAL THIS DATEPARTINITFOCUS�  ���  ��� � H� �� � ��� � ��: � T�� ���� ��� ���Z � T�� ���� ��� ���� � %�C� CENTURYv� ON��� � T�� ���� �� � T�� ���� � � �� U  THIS CENTURY YEARSELLENGTH ���  ��� H� �	� ��� ���� � T�� ���� T�� ���� T�� �� �� T�� ���� T�� ���� T�� ���� %��� ���� � T�� ��� ��� � ��� ���8� T�� �� �� T�� ���� T�� ���� T�� ���� T�� ���� T�� ���� %��� ���4� T�� ��� ��� � 2�	� T�� �� �� T�� ���� T�� ���� T�� ���� T�� ���� T�� ���� %��� ���� T�� ��� ��� T�� ��� ��� T�� ��� ��� T�� ��� ��� T�� ��� ��� � � �� U	  THIS DATETYPE DAYSTART DAYEND
 MONTHSTART MONTHEND	 YEARSTART YEAREND YEARSELLENGTH5  ��  � %�C�  ��� L��. � T� � ���  �� � U  VNEWVAL THIS LUPDOWNDISABLED7  ��  � %�C��  ��� L��0 � T� � ���  �� � U  VNEWVAL THIS LEXITONTABONLY� " T�  �CCC��� ]fC� � f�  ��� %�� � ���j �4 T� � �C� calendar_Form_desktop �  �  �  �N�� �� �, T� � �C� calendar_Form �  �  �  �N�� � T� � � �a�� ��C� � � �� U  _DPKNAME THIS THISFORM NAME
 SHOWWINDOW CALENDAR_FORM_REF ALWAYSONTOP SHOWe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS WIDTH
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS HEIGHT
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS LEFT
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS TOP
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEd  ��  � %��  � � ��] � T� � ��  ��" T� �C� this.parent.� � ��� T� � ��  �� � U  VNEWVAL THIS VISIBLE
 OBUTTONREF CBBUTTON_NAMEr  ��  � %��  � � ��k � T� � ��  ��" T� �C� this.parent.� � ��� T� � ��  
�
 � � -
	�� � U  VNEWVAL THIS READONLY
 OBUTTONREF CBBUTTON_NAME ENABLEDp  ��  � %��  � � ��i � T� � ��  ��" T� �C� this.parent.� � ��� T� � ��  �	 � � -	�� � U  VNEWVAL THIS ENABLED
 OBUTTONREF CBBUTTON_NAME READONLY1 ���  ��*� %�C�� �
��&� �� � � T� �C�� ��� %�C� ���� �s ��C�# Please enter alias name along with C� �% field name in ucontrolsource property�0� Technical Mess.�x�� B�-�� � %�C� b� T��"� convDt = dtoc(&tConts)
. T�  � �CC� �
 01/01/1900� �  � � 6#�� � � �� U  THIS UCONTROLSOURCE TCONTS CONVDT VALUE/  %�C�  � �
��( � ��C�  � �@� �x�� � U  THIS CERRMSG VUMESSU ���  ��� T�� �� ��( %��� C�
 01/01/1900#� C�� ��� � %�C�� �
��� � �� � � T� �C�� ���� T� �C�� ����
 F�� �� %�C� b� T��� �) IF &tfldname = CTOD('01/01/1900')�� �, replace &tfldname with {} in &taliasname
 � � � � ��! %�C� This.Whenexprb� C��N� %�C�  � �
��J� B�C�  � ��� � � U  THIS SELSTART VALUE UCONTROLSOURCE TFLDNAME
 TALIASNAME WHENEXPR  U    U  �  ��  � �) %�C� Thisform.ObjClickMoveb� L��B � T� � �-�� � ��� ��� � %�C�� �
��� � �� � � T� �C�� ���� T� �C�� ����
 F�� �� %�C� b� T��� �4 replace &tfldname with this.value in &taliasname
 � � �� U  NKEYCODE NSHIFTALTCTRL THISFORM OBJCLICKMOVE THIS UCONTROLSOURCE TFLDNAME
 TALIASNAMED  ��  � � ��� ��= � ��C�� �� ��C�� �� T�� ��� �� �� U  LNSTART LNLENGTH THIS SETYEARSELLENGTH SETSTARTENDPOSITIONS CURRENTDATE VALUET T�  �C�� T� �CC+
� CO� � 6��) %�C� Thisform.ObjClickMoveb� U��l � %�� � a��h � B�a�� � � %�C� � ���� � %�� � 
��� � B�-�� � �� � %�C� � i� ��� � B�-�� � � %�C� � �
����2 %�CC� � H���
� CC� � i�l��
���� %�CC� � H���
��_�4 T� � ��$ Month should be between Jan. to Dec.�� � %�CC� � i�l��
����3 T� � ��# Year should be between 1900 to 2078�� � T� � �C�  #�� B�-�� � � %�C� � �
���� T�	 �C� � ��� %�C� _curvalb� L���� %��	 -���� %�C� � �
��`� T�
 �CC� � Λ�� ��C�
 �@� �x�� �
 F��  �� �� %�� � ����	 #�� �� � B�� � �� � � � ��� ��M� %�C�� �
��I� �� � � T� �C�� ���� T� �C�� ����
 F�� �� %�C� b� T��E�4 Replace &tFldName With This.Value In &tAliasName
 � � �� U  _CURTBL _CURREC THISFORM OBJCLICKMOVE THIS VALUE LALLOWBLANKDATE CERRMSG	 VALIDEXPR _CURVAL _CURMES VUMESS UCONTROLSOURCE TFLDNAME
 TALIASNAME	+ ��  � � � � � � � � �	 �� %�C� nTopb� L� C� nLeftb� L� C� cSourceb� L� C� cWhenb� L� C� cValidb� L� C� cDefaultb� L� C� cErrorb� L� C� flsourceb� L� C� lTextBoxb� L���# T�
 � �CC� �
� C� �� �  6��# T�
 � �CC� �
� C� �� �  6��# T�
 � �CC� �
� C� �� �  6�� T�
 � �� �� T�
 � �� �� T�
 � ��  �� T�
 � ��      �?��* %�C� � f� D� C� � f� N���� T�
 � ���� ��� T�
 � ���� � T�
 � ��
�� T�
 � �� �� T�
 � ���� � ���
 ��� ��C�� �� ��C�� �� T� �C�
 � �� dpkcmb�� T�� �� ��' %�C�� � � Toolbar� Column�
����' ��C�� � cbButton�  �   �
 �� � �� T�  �C� .parent.�� ��� ��C �
 �  �! �� T�  �" ��
 �" �� � �� U#  NTOP NLEFT TXTWIDTH CSOURCE CWHEN CVALID CDEFAULT CERROR FLSOURCE LTEXTBOX THIS WHENEXPR	 VALIDEXPR CERRMSG VALUE LEFT TOP HEIGHT LOTHER DATA_TY WIDTH	 MAXLENGTH CONTROLSOURCE FONTSIZE SETYEARSELLENGTH SETSTARTENDPOSITIONS MCBBTNNM NAME CBBUTTON_NAME PARENT	 BASECLASS	 NEWOBJECT LOBTN SETPOSITIONANDSIZE VISIBLE  ���  �� � ��� � �� U  THIS CONVCONTROLSOURCE datepartinitfocus_assign,     �� setyearsellength�     �� setstartendpositionsi    �� lupdowndisabled_assign�    �� lexitontabonly_assign5    �� dropdown�    �� width_assign�    �� height_assignY    �� left_assign    ��
 top_assign�    �� visible_assigna    �� readonly_assign�    �� enabled_assign�	    �� convcontrolsource`
    �� ErrorMessage�    �� When    �� Click�    �� InteractiveChange�    �� KeyPress�    �� GotFocus    �� Valid�    �� Init�    �� Refresh    ��1 q �!A 3 � � !� "� "�� � � A A A 3 � � !� � � � � � "1A "� � � � � � "1A � � � � � � � "11111A A A 3 q R!A 3 q r!A 3 "AA� �A 3 q B!A 3 q B!A 3 q A!A 3 q B!A 3 q A!A 3 q A!�A 3 q A!�A 3 � � � 1q A A��A A A 3 1QA 3 � � �� � A��A A A A A 1� A A 4 3 �1 � �� A 0� � � AAA A A 3 � � � � � 0A 3 � ��!q A A "q A � aq A A 1!�AA �1A !q A A 1"�� 1A!A � A � A � A A A � � � AAA A A 3 ��111��� A A � � � �� sq�1A A 3 � � A 2                    $   �         �   	  
      4  S     <   �    N   A   ;  �  V   F   �  n	  ^   O   �	  v
  j   V   �
  �  t   ]   �  R  ~   d   s  S  �   k   x  (  �   r   N    �   y   C    �   �   3  �  �   �   �  /  �   �   J  L  �   �   h  �  �   �   �  c  �   �   �  %;  �   �   D;  �=  5  �   �=  C  R  �   &C  =H  �    [H  �H  �   )   ,                       ����    �  �                        �)   %         `  !             �  U  " ��  � � � T� � ��  �� T� �  ��  �� T� � �� �� %�C�	 ToformRefb� O��o � T� � �� � �� �  %�C� toTextboxRefb� O��� � T� �  ��  � �� �( %�C� toformRef.curonmouseb� L��� � T� � �a�� � %�C�  �� C�  ��� O��� B� � �� � T� �C� �	 �� ��� ��� T��
 ��  �� T�� ���
 � ��" %�C��
 � �
� C��
 � �
	���� T� � � �C��
 � i�� T� � � �C��
 � H�� T� � � �C��
 � %�� � G1 �2 T�� ���
 � C��
 ��]C��%C�	�%� � ��# T�� �C��
 ��]C��%� � �� �� U  TOTEXTBOXREF	 TOFORMREF _DPKNAME THISFORM DPKNAME NAME
 CURONMOUSE OTOPLEVELFORM THIS GETTOPLEVELFORM
 TEXTBOXREF OLDVALUE VALUE OLEMONTHVIEW YEAR MONTH DAY TOP HEIGHT LEFT Init,     ��1 � �1A 1A �� A �A A q � � !"���A a $1PA 2                       )      )   �                       ]Top = 0
Left = 0
Height = 144
Width = 164
Desktop = .F.
ShowWindow = 1
ShowInTaskBar = .F.
DoCreate = .T.
BorderStyle = 1
Caption = "ComboForm"
Closable = .F.
KeyPreview = .T.
TitleBar = 0
WindowType = 1
AlwaysOnTop = .T.
textboxref = .NULL.
toformref = .F.
totextboxref = .F.
dpkname = .F.
mainfrm = .F.
Name = "calendar_form"
     FontName = "Tahoma"
FontSize = 8
Alignment = 3
Value = (DATE())
Height = 22
Width = 64
NullDisplay = ""
currentdate = 
lallowblankdate = .T.
datetype = 2
datepartinitfocus = 2
yearsellength = 4
daystart = 0
dayend = 1
monthstart = 3
monthend = 4
yearstart = 6
yearend = 9
lexitontabonly = .T.
calendar_form_ref = .NULL.
cbbutton_name = 
_memberdata =      286<?xml version="1.0" standalone="yes"?>
<VFPData><memberdata name="cbbutton_name" type="property" display="cbButton_name" favorites="True"/><memberdata name="comboformref" type="property" display="comboFormRef"/><memberdata name="cerrmsg" type="property" display="cErrMsg"/></VFPData>

blankdatestring = 19000101
ucontrolsource = .F.
whenexpr = 
validexpr = 
cerrmsg = 
Name = "dpk"
     4PROCEDURE Init
LPARAMETERS toTextboxRef, toformRef,_dpkname
*vasant
Thisform.Toformref    = ''
Thisform.ToTextboxRef = ''
Thisform.Dpkname      = _dpkname
IF TYPE('ToformRef') = 'O'
	Thisform.Toformref  = ToformRef.name
Endif	
IF TYPE('toTextboxRef') = 'O'
	Thisform.toTextboxRef = toTextboxRef.name
Endif	
IF TYPE('toformRef.curonmouse') = 'L'
	toformRef.curonmouse = .t.
Endif	
*vasant
IF ISNULL(toTextboxRef) OR VARTYPE(toTextboxRef) <> 'O'
	RETURN
ENDIF 
LOCAL oTopLevelForm
oTopLevelForm = this.getTopLevelForm() &&get host form reference for setting position
WITH this
	.textboxRef = toTextboxRef
	.oldValue = .textboxRef.Value 	&&just in case for Esc or Deactivate

	IF NOT EMPTY(.textboxRef.value) AND NOT ISNULL(.textboxRef.value)
		this.oleMonthView.Year = YEAR(.textboxRef.Value)		
		this.oleMonthView.month = MONTH(.textboxRef.Value)
		this.oleMonthView.day = DAY(.textboxRef.Value)
	ENDIF 
SET STEP ON
*	this.top=200
*	this.Left=515
	.Top = .textboxRef.Height + OBJTOCLIENT(.textboxRef ,1)  ;
		+ SYSMETRIC(4) + SYSMETRIC(9) + toFormRef.top
	.Left = OBJTOCLIENT(.textboxRef ,2) + SYSMETRIC(3) + toFormRef.left 

*!*	SET STEP ON 
*check to see id "drop up" os horizontal position shifting needed	
*!*		DO CASE 
*!*		CASE toFormRef.ShowWindow = 0 OR toFormRef.ShowWindow = 1
*!*			IF .Left + .Width > oTopLevelForm.Width 	
*!*				.Left = oTopLevelForm.Width - .Width - 4
*!*			ENDIF
*!*		
*!*			IF .Top + .Height  > oTopLevelForm.Height
*!*				.Top = .Top - .textboxRef.Height - .Height - 2
*!*			ENDIF 
*!*		CASE  toFormRef.ShowWindow = 2
*!*			IF .Left + .Width > SYSMETRIC(1)
*!*				.Left = SYSMETRIC(1) - .Width - 4
*!*			ENDIF
*!*		
*!*			IF .Top + .Height  > SYSMETRIC(2)
*!*				.Top = .Top - .textboxRef.Height - .Height - 2
*!*			ENDIF 
*!*		ENDC
ENDWITH 

ENDPROC
     �PROCEDURE setpositionandsize
LPARAMETERS toTextbox
WITH this
	.Top = toTextbox.Top
	.Left = toTextbox.Left + toTextbox.Width - 1
	.Height = toTextbox.Height
ENDWITH 

ENDPROC
PROCEDURE MouseLeave
LPARAMETERS nButton, nShift, nXCoord, nYCoord
&&Vasant
IF TYPE('Thisform.ObjClickMove') = 'L'
	Thisform.ObjClickMove = .f.
ENDIF
&&Vasant
ENDPROC
PROCEDURE Init
LPARAMETERS oTextBox
*only name, to avoid dangling reference on Destroy
this.oDateTextbox = oTextbox.Name

ENDPROC
PROCEDURE When
oTextBox = EVALUATE('this.parent.' + this.oDateTextbox)
IF TYPE('oTextBox.value') = 'D'
	IF !Between(Month(oTextBox.Value),1,12) OR !BETWEEN(Year(oTextBox.Value),1900,2078)
		oTextBox.Value = CTOD('')
	Endif
ENDIF
IF TYPE('thisform.curonmouse') = 'L' AND MDOWN() = .t.
	thisform.curonmouse = .t.
Endif	
RETURN MDOWN()

ENDPROC
PROCEDURE DropDown
oTextBox = EVALUATE('this.parent.' + this.oDateTextbox)
oTextBox.Setfocus()
*Birendra: Bug-434 on 01/12/2011:Start:
zTextBox=IIF(TYPE('oTextBox.parent.whenexpr')='C','oTextBox.parent.whenexpr','oTextBox.Whenexpr')
	IF !EMPTY(&zTextBox)
	   IF EVALUATE(&zTextBox)
			IF oTextBox.Enabled = .T. AND oTextBox.ReadOnly = .F.
				oTextbox.DropDown()
			ENDIF 
		ELSE
		IIF(TYPE('oTextBox.parent.whenexpr')='C',oTextBox.parent.setfocus(),oTextBox.setfocus())
	   ENDIF 
	ELSE
		IF oTextBox.Enabled = .T. AND oTextBox.ReadOnly = .F.
			oTextbox.DropDown()
		ENDIF 
	ENDIF 
*Birendra: Bug-434 on 01/12/2011:End:

*!*	IF oTextBox.Enabled = .T. AND oTextBox.ReadOnly = .F.
*!*		oTextbox.DropDown()
*!*	ENDIF 


ENDPROC
PROCEDURE GotFocus
&&Vasant
IF TYPE('thisform.curonmouse') = 'L'
	thisform.curonmouse = .f.
Endif	
&&Vasant
ENDPROC
PROCEDURE LostFocus
&&Vasant
IF TYPE('thisform.curonmouse') = 'L'
	thisform.curonmouse = .f.
Endif	
&&Vasant

ENDPROC
PROCEDURE MouseMove
LPARAMETERS nButton, nShift, nXCoord, nYCoord
&&Vasant
IF TYPE('Thisform.ObjClickMove') = 'L'
	Thisform.ObjClickMove = .t.
Endif	
&&Vasant

ENDPROC
     �currentdate Date on field entry
lallowblankdate Allow blank dates
datetype Date Type 1-{mm/dd/yy},{mm-dd-yy},{mm.dd.yy}, 2-{dd/mm/yy},{dd-mm-yy},{dd.mm.yy}, 3-(yy/mm/dd),(yy-mm-dd),(yy.mm.dd)
datepartinitfocus Date part highlighted when control receives focus: 1- Month (default), 2-Day, 3-Year
yearsellength Selected length of the year ( 2 or 4 )
daystart Day Start position
dayend Day end position
monthstart Month start position
monthend Month end position
yearstart Year start position
yearend Year end position
lupdowndisabled Flag indicating whether the Up / Down arrow keys are disabled ( +/- functionality only )
lexitontabonly Exit the control only on (Shift+)Tab
calendar_form_ref referinta la formul cu calendar
cbbutton_name
_memberdata XML Metadata for customizable properties
blankdatestring
ucontrolsource
whenexpr
validexpr
cerrmsg
*datepartinitfocus_assign 
*setyearsellength Sets the year selected length
*setstartendpositions Sets the Day, Month and Year Start and End positions
*lupdowndisabled_assign 
*lexitontabonly_assign 
*dropdown afiseaza form calendr
*width_assign 
*height_assign 
*left_assign 
*top_assign 
*visible_assign 
*readonly_assign 
*enabled_assign 
*convcontrolsource 
     �PROCEDURE gettoplevelform
*from Foundation Classes "_ui.vcx"

ASSERT TYPE("_SCREEN.ActiveForm") # "O"  OR ;
       INLIST(_SCREEN.ActiveForm.ShowWindow, 0,1,2)

DO CASE
CASE _SCREEN.FormCount = 0 OR ;
     (TYPE("_SCREEN.ActiveForm") = "O" AND ;
     _SCREEN.ActiveForm.ShowWindow = 0 )     && ShowWindow In Screen
		     
     loTopForm = _SCREEN

CASE (TYPE("_SCREEN.ActiveForm") = "O" AND ;
      _SCREEN.ActiveForm.ShowWindow = 2 )    && ShowWindow As Top Form

     loTopForm = _SCREEN.ActiveForm
		     
OTHERWISE 
		                                       
     FOR EACH loForm IN _SCREEN.Forms  && note: these may be toolbars
                                       && if undocked, but that's okay --
                                       && they are only ShowWIndow 0 or 1.

        IF loForm.ShowWindow = 2 && the first one in the collection will
                                && be "active top form"
           loTopForm = loForm
           EXIT
        ENDIF
     ENDFOR
		     
     IF VARTYPE(loTopForm) # "O"
        loTopForm = _SCREEN
     ENDIF
		          
ENDCASE

RETURN loTopForm     

ENDPROC
PROCEDURE Destroy
*vasant
IF !EMPTY(Thisform.Toformref)
	&&Changes done by vasant on 16/11/2012 as per Bug 6158(After updating Auto updater 10.4.14,Issue is seen in Credit Note Forwarding.) 
	IF TYPE('Thisform.Mainfrm.Curonmouse') = 'L'
		Thisform.Mainfrm.Curonmouse = .f.
		_toTextboxRef = 'Thisform.Mainfrm'+Thisform.Dpkname
		with &_toTextboxRef
			if !empty(.uControlsource) 
				local tFldName,tAliasName
				tFldName   = justext(.uControlsource)
				tAliasName = juststem(.uControlSource)
				SELECT (taliasname)
				if type(tFldname) ='T'
					replace &tfldname with .value in &taliasname
				endif
			endif
		ENDWITH
		&_toTextboxRef..setfocus()
	Endif	

*!*		_toformRef = Thisform.Toformref
*!*		ACTIVATE WINDOW &_toformRef
*!*		IF TYPE('_Screen.ActiveForm.Curonmouse') = 'L'
*!*			_Screen.ActiveForm.Curonmouse = .f.
*!*			_toTextboxRef = '_Screen.ActiveForm'+Thisform.Dpkname
*!*			with &_toTextboxRef
*!*				if !empty(.uControlsource) 
*!*					local tFldName,tAliasName
*!*					tFldName   = justext(.uControlsource)
*!*					tAliasName = juststem(.uControlSource)
*!*					SELECT (taliasname)
*!*					if type(tFldname) ='T'
*!*						replace &tfldname with .value in &taliasname
*!*					endif
*!*				endif
*!*			ENDWITH
*!*			&_toTextboxRef..setfocus()
*!*		Endif	
	&&Changes done by vasant on 16/11/2012 as per Bug 6158(After updating Auto updater 10.4.14,Issue is seen in Credit Note Forwarding.) 
Endif	
*vasant

ENDPROC
PROCEDURE Deactivate
THIS.Release
ENDPROC
PROCEDURE Init
LPARAMETERS toTextboxRef, toformRef,_dpkname
Thisform.Mainfrm = toformRef	&&Changes done by vasant on 16/11/2012 as per Bug 6158(After updating Auto updater 10.4.14,Issue is seen in Credit Note Forwarding.) 
*vasant
Thisform.Toformref    = ''
Thisform.ToTextboxRef = ''
Thisform.Dpkname      = _dpkname
IF TYPE('ToformRef') = 'O'
	Thisform.Toformref  = ToformRef.name
Endif	
IF TYPE('toTextboxRef') = 'O'
	Thisform.toTextboxRef = toTextboxRef.name
Endif	
IF TYPE('toformRef.curonmouse') = 'L'
	toformRef.curonmouse = .t.
Endif	
*vasant
IF ISNULL(toTextboxRef) OR VARTYPE(toTextboxRef) <> 'O'
	RETURN
ENDIF 
LOCAL oTopLevelForm
oTopLevelForm = this.getTopLevelForm() &&get host form reference for setting position
WITH this
	.textboxRef = toTextboxRef
	.oldValue = .textboxRef.Value 	&&just in case for Esc or Deactivate

	IF NOT EMPTY(.textboxRef.value) AND NOT ISNULL(.textboxRef.value)
		this.oleMonthView.Year = YEAR(.textboxRef.Value)		
		this.oleMonthView.month = MONTH(.textboxRef.Value)
		this.oleMonthView.day = DAY(.textboxRef.Value)
	ELSE
		&&vasant
		this.oleMonthView.Year  = YEAR(DATE())
		this.oleMonthView.month = MONTH(DATE())
		this.oleMonthView.day   = DAY(DATE())
		&&vasant
	ENDIF 
*	this.top=200
*	this.Left=515
	.Top = .textboxRef.Height + OBJTOCLIENT(.textboxRef ,1)  ;
		+ SYSMETRIC(4) + SYSMETRIC(9) + toFormRef.top
	.Left = OBJTOCLIENT(.textboxRef ,2) + SYSMETRIC(3) + toFormRef.left 
	IF (.Left + .Width) > (SYSMETRIC(1) - (SYSMETRIC(1)*0.02))
		.Left = .Left - (SYSMETRIC(1)*0.02)
	ENDIF

*!*	SET STEP ON 
*check to see id "drop up" os horizontal position shifting needed	
*!*		DO CASE 
*!*		CASE toFormRef.ShowWindow = 0 OR toFormRef.ShowWindow = 1
*!*			IF .Left + .Width > oTopLevelForm.Width 	
*!*				.Left = oTopLevelForm.Width - .Width - 4
*!*			ENDIF
*!*		
*!*			IF .Top + .Height  > oTopLevelForm.Height
*!*				.Top = .Top - .textboxRef.Height - .Height - 2
*!*			ENDIF 
*!*		CASE  toFormRef.ShowWindow = 2
*!*			IF .Left + .Width > SYSMETRIC(1)
*!*				.Left = SYSMETRIC(1) - .Width - 4
*!*			ENDIF
*!*		
*!*			IF .Top + .Height  > SYSMETRIC(2)
*!*				.Top = .Top - .textboxRef.Height - .Height - 2
*!*			ENDIF 
*!*		ENDC
ENDWITH 
ENDPROC
PROCEDURE Show
LPARAMETERS nStyle
THIS.OLEMonthView.Width = this.Width 
this.olemonthView.Height = this.Height 
this.oleMonthView.SetFocus()

ENDPROC
     H�PROCEDURE datepartinitfocus_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method

IF INLIST( m.vNewVal, 1, 2, 3 )	&& Month, Day, Year
	THIS.DatePartInitFocus = m.vNewVal
ENDIF

ENDPROC
PROCEDURE setyearsellength
WITH THIS
	DO CASE
		CASE .Century = 0	&& Off
			.YearSelLength = 2

		CASE .Century = 1	&& On
			.YearSelLength = 4

		CASE .Century = 2	&& Default
			IF SET( "CENTURY" ) = "ON"
				.YearSelLength = 4
			ELSE
				.YearSelLength = 2
			ENDIF
	ENDCASE
ENDWITH

ENDPROC
PROCEDURE setstartendpositions
*	We set the positions for testing when moving
*	between date parts, etc.
WITH THIS
	DO CASE
		CASE .DateType = 1			&& MDY 01/34/6789
			.DayStart = 3
			.DayEnd = 4
			.MonthStart = 0
			.MonthEnd = 1
			.YearStart = 6
			.YearEnd = 7			&& Century Off	(01/34/67)

			IF .YearSelLength = 4	&& Century On	(01/34/6789)
				.YearEnd = .YearEnd + 2
			ENDIF

		CASE .DateType = 2			&& DMY 01/34/6789
			.DayStart = 0
			.DayEnd = 1
			.MonthStart = 3
			.MonthEnd = 4
			.YearStart = 6
			.YearEnd = 7			&& Century Off	(01/34/67)

			IF .YearSelLength = 4	&& Century On	(01/34/6789)
				.YearEnd = .YearEnd + 2
			ENDIF

		OTHERWISE					&& YMD 0123/56/89
			.YearStart = 0

			.YearEnd = 1			&& Century Off	(01/34/67)
			.MonthStart = 3
			.MonthEnd = 4
			.DayStart = 6
			.DayEnd = 7

			IF .YearSelLength = 4	&& Century On	(0123/56/89)
				.YearEnd = .YearEnd + 2
				.MonthStart = .MonthStart + 2
				.MonthEnd = .MonthEnd + 2
				.DayStart = .DayStart + 2
				.DayEnd = .DayEnd + 2
			ENDIF
	ENDCASE
ENDWITH

ENDPROC
PROCEDURE lupdowndisabled_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF VARTYPE( vNewVal ) = 'L'
	THIS.lUpDownDisabled = m.vNewVal
ENDIF

ENDPROC
PROCEDURE lexitontabonly_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF VARTYPE( m.vNewVal ) = 'L'
	THIS.lExitOnTabOnly = m.vNewVal
ENDIF

ENDPROC
PROCEDURE dropdown
*SET CLASSLIB TO datepicker additive
_dpkname = STRTRAN(UPPER(SYS(1272,This)),Uppe(Thisform.Name),'')
IF thisform.ShowWindow = 2
	this.calendar_form_ref = CREATEOBJECT('calendar_Form_desktop',this,thisform,_dpkname)
ELSE 
	this.calendar_form_ref = CREATEOBJECT('calendar_Form',this,thisform,_dpkname)
ENDIF 
this.calendar_form_ref.AlwaysOnTop = .T.
this.calendar_form_ref.Show()

ENDPROC
PROCEDURE width_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF vNewVal <> this.Width
	this.Width = vNewVal 
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE height_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF vNewVal <> this.Height 
	this.Height = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE left_assign
LPARAMETERS vNewVal
IF vNewVal <> this.left
	this.left = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE top_assign
LPARAMETERS vNewVal
*To do: Modify this routine for the Assign method
IF vNewVal <> this.Top
	this.top = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.setPositionAndSize(this)
ENDIF 

ENDPROC
PROCEDURE visible_assign
LPARAMETERS vNewVal
IF vNewVal <> this.Visible
	this.Visible = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.Visible = vNewVal		
ENDIF 

ENDPROC
PROCEDURE readonly_assign
LPARAMETERS vNewVal
IF vNewVal <> this.ReadOnly
	this.ReadOnly = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.Enabled = NOT vNewVal AND NOT this.Enabled = .f. 
ENDIF 

ENDPROC
PROCEDURE enabled_assign
LPARAMETERS vNewVal
IF vNewVal <> this.Enabled
	this.Enabled = vNewVal
	oButtonRef = EVALUATE('this.parent.' + this.cbButton_name)
	oButtonRef.Enabled = vNewVal and this.ReadOnly = .f.  
ENDIF 

ENDPROC
PROCEDURE convcontrolsource
with this
	if !empty(.uControlsource) 
		local tContS,convDt
		tConts = allt(.uControlsource)
		if empty(tConts)
			=messagebox("Please enter alias name along with "+chr(13)+"field name in ucontrolsource property",48,"Technical Mess.")
			return .f.
		endif
		if type(tConts) ='T'
			convDt = dtoc(&tConts)
			this.value = ctod(IIF(convdt='01/01/1900','',convdt))	
		endif
	endif
endwith

ENDPROC
PROCEDURE ErrorMessage
IF !EMPTY(this.cErrMsg)
	=messagebox(this.cErrMsg,64,vuMess) 
Endif

ENDPROC
PROCEDURE When
*Do not delete this comment - Vasant
&&vasant
With This
	.SelStart = 0		&&vasant22/06/09
*	IF .VALUE = CTOD('01/01/1900') 
	IF .VALUE = CTOD('01/01/1900') OR EMPTY(.Value) &&Birendra: Bug-434 on 01/12/2011 (OR EMPTY(.Value) added)
		If !empty(.uControlsource) 
			local tFldName,tAliasName
			tFldName   = justext(.uControlsource)
			tAliasName = juststem(.uControlSource)
			SELECT (taliasname)		
			if type(tFldname) ='T' 
				IF &tfldname = CTOD('01/01/1900')
					replace &tfldname with {} in &taliasname
				Endif	
			endif
		endif
	Endif
ENDWITH
&&vasant
*Birendra: Bug-434 on 01/12/2011 :Start:
IF TYPE('This.Whenexpr')='C'
	IF !EMPTY(this.whenexpr)
	RETURN EVALUATE(this.whenexpr )
	ENDIF
ENDIF 
*Birendra: Bug-434 on 01/12/2011 :End:

ENDPROC
PROCEDURE Click
*THIS.SetFocus()	&&vasant22/06/09
ENDPROC
PROCEDURE InteractiveChange
&&vasant22/06/09
*!*	WITH THIS
*!*		IF INLIST( .DateType, 1, 2 )	&& MDY, DMY
*!*			DO CASE
*!*				CASE .SelStart = 3
*!*					.SelLength = 2

*!*				CASE .SelStart = 6
*!*					.SelLength = .YearSelLength
*!*			ENDCASE
*!*		ELSE							&& YMD
*!*			IF INLIST( .SelStart, .MonthStart, .DayStart )	&& 5, 8
*!*				.SelLength = 2
*!*			ENDIF
*!*		ENDIF
*!*	*!*		IF VARTYPE(this.Value) = 'D'
*!*	*!*			IF VAL(DTOC(this.value,1)) = this.blankDateString 
*!*	*!*				*this.ForeColor = this.BackColor 
*!*	*!*				this.ForeColor = this.Parent.ForeColor
*!*	*!*			ELSE
*!*	*!*				this.ForeColor = this.Parent.ForeColor
*!*	*!*			ENDIF 
*!*	*!*		ENDIF	
*!*	ENDWITH
&&vasant22/06/09
ENDPROC
PROCEDURE KeyPress
LPARAMETERS nKeyCode, nShiftAltCtrl
&&Vasant
IF TYPE('Thisform.ObjClickMove') = 'L'
	Thisform.ObjClickMove = .f.
Endif	
&&Vasant
&&vasant22/06/09
*!*	WITH THIS
*!*		DO CASE
*!*			CASE nKeyCode = 32					&& Space Bar ( DATE() )
*!*				NODEFAULT
*!*				IF .ReadOnly
*!*					RETURN
*!*				ENDIF
*!*				.VALUE = DATE()
*!*				.REFRESH
*!*				.SETFOCUS()
*!*			
*!*			CASE nKeyCode = 1					&& Home ( Last Date entered )
*!*				NODEFAULT
*!*				IF .ReadOnly
*!*					RETURN
*!*				ENDIF
*!*				.VALUE = .CurrentDate
*!*				.REFRESH
*!*				.SETFOCUS()
*!*			
*!*			CASE nKeyCode = 4		&& Right Arrow
*!*				IF .ReadOnly
*!*					NODEFAULT
*!*					RETURN
*!*				ENDIF
*!*				IF .DateType = 3	&& YMD 0123/56/89, 01/34/67
*!*					DO CASE
*!*						CASE .SelStart < .MonthStart	&& 5
*!*							NODEFAULT
*!*							.SelStart = .MonthStart		&& Moving to second date part ( Month )
*!*							.SelLength = 2

*!*						CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& 3, 8
*!*							NODEFAULT
*!*							.SelStart = .DayStart		&& Moving to third date part ( Day )
*!*							.SelLength = 2

*!*						CASE .SelStart >= .DayStart
*!*							NODEFAULT
*!*							IF NOT .lExitOnTabOnly
*!*								KEYBOARD '{TAB}'		&& Exiting the control
*!*							ENDIF
*!*						
*!*						OTHERWISE
*!*							NODEFAULT
*!*					ENDCASE
*!*				ELSE				&& MDY/DMY 01/34/6789, 01/34/67
*!*					DO CASE			&& Month and Day SelLengths are the same so we use
*!*						CASE .SelStart < 3	&& Literals for performance here
*!*							NODEFAULT
*!*							.SelStart = 3	&& Moving to second date part ( Day or Month )
*!*							.SelLength = 2

*!*						CASE .SelStart > 1 AND .SelStart < 6
*!*							NODEFAULT
*!*							.SelStart = 6	&& Moving to third date part ( Year )
*!*							.SelLength = .YearSelLength
*!*						
*!*						CASE .SelStart >= .YearStart
*!*							NODEFAULT
*!*							IF NOT .lExitOnTabOnly
*!*								KEYBOARD '{TAB}'		&& Exiting the control
*!*							ENDIF

*!*						OTHERWISE
*!*							NODEFAULT
*!*					ENDCASE
*!*				ENDIF

*!*			CASE nKeyCode = 19					&& Left Arrow
*!*				IF .ReadOnly
*!*					NODEFAULT
*!*					RETURN
*!*				ENDIF
*!*				IF .DateType = 3	&& YMD 0123/56/89, 01/34/67
*!*					DO CASE
*!*						CASE .SelStart > .MonthEnd	&& 7
*!*							NODEFAULT
*!*							.SelStart = .MonthStart	&& Moving to second date part ( Month )
*!*							.SelLength = 2

*!*						CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& 3, 8
*!*							NODEFAULT
*!*							.SelStart = 0	&& Moving to first date part ( Year )
*!*							.SelLength = .YearSelLength

*!*						CASE .SelStart < .MonthStart
*!*							NODEFAULT
*!*							IF NOT .lExitOnTabOnly
*!*								KEYBOARD '{BACKTAB}'	&& Exiting the control
*!*							ENDIF
*!*						
*!*						OTHERWISE
*!*							NODEFAULT
*!*					ENDCASE
*!*				ELSE				&& MDY/DMY 01/34/6789, 01/34/67
*!*					DO CASE			&& Month and Day SelLengths are the same so we use
*!*						CASE .SelStart > 5	&& Literals for performance here
*!*							NODEFAULT
*!*							.SelStart = 3	&& Moving to second date part ( Day or Month )
*!*							.SelLength = 2

*!*						CASE .SelStart > 1 AND .SelStart < 6
*!*							NODEFAULT
*!*							.SelStart = 0	&& Moving to first date part ( Month or Day )
*!*							.SelLength = 2

*!*						CASE .SelStart < 3
*!*							NODEFAULT
*!*							IF NOT .lExitOnTabOnly
*!*								KEYBOARD '{BACKTAB}'	&& Exiting the control
*!*							ENDIF
*!*						
*!*						OTHERWISE
*!*							NODEFAULT
*!*					ENDCASE
*!*				ENDIF

*!*			CASE INLIST( nKeyCode, 5, 43, 61 )	&& Up Arrow, '+'('=') ( Increment )
*!*				NODEFAULT
*!*				IF ( ( .lUpDownDisabled AND nKeyCode = 5 ) OR .ReadOnly )	&& Up Arrow disabled
*!*					RETURN
*!*				ENDIF

*!*				DO CASE
*!*					CASE .DateType = 1	&& MDY 01/34/6789, 01/34/67
*!*						DO CASE			&& Use literals whenever possible for performance
*!*							CASE .SelStart < 3						&& Month
*!*								.VALUE = GOMONTH( .VALUE, 1 )
*!*								.REFRESH
*!*								.SelStart = 0
*!*								.SelLength = 2

*!*							CASE .SelStart > 1 AND .SelStart < 6	&& Day
*!*								.VALUE = .VALUE + 1
*!*								.REFRESH
*!*								.SelStart = 3
*!*								.SelLength = 2

*!*							OTHERWISE								&& Year
*!*								.VALUE = GOMONTH( .VALUE, 12 )
*!*								.REFRESH
*!*								.SelStart = 6
*!*								.SelLength = .YearSelLength	&& 4
*!*						ENDCASE

*!*					CASE .DateType = 2	&& DMY 01/34/6789, 01/34/67
*!*						DO CASE
*!*							CASE .SelStart < 3						&& Day
*!*								.VALUE = .VALUE + 1
*!*								.REFRESH
*!*								.SelStart = 0
*!*								.SelLength = 2

*!*							CASE .SelStart > 1 AND .SelStart < 6	&& Month
*!*								.VALUE = GOMONTH( .VALUE, 1 )
*!*								.REFRESH
*!*								.SelStart = 3
*!*								.SelLength = 2

*!*							OTHERWISE								&& Year
*!*								.VALUE = GOMONTH( .VALUE, 12 )
*!*								.REFRESH
*!*								.SelStart = 6
*!*								.SelLength = .YearSelLength	&& 4
*!*						ENDCASE

*!*					OTHERWISE			&& YMD 0123/56/89, 01/34/67
*!*						DO CASE
*!*							CASE .SelStart < .MonthStart	&& 5	&& Year
*!*								.VALUE = GOMONTH( .VALUE, 12 )
*!*								.REFRESH
*!*								.SelStart = 0
*!*								.SelLength = .YearSelLength	&& 4

*!*							CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& Month ( 3, 8 )
*!*								.VALUE = GOMONTH( .VALUE, 1 )
*!*								.REFRESH
*!*								.SelStart = .MonthStart	&& 5
*!*								.SelLength = 2

*!*							OTHERWISE								&& Day
*!*								.VALUE = .VALUE + 1
*!*								.REFRESH
*!*								.SelStart = .DayStart	&& 8
*!*								.SelLength = 2
*!*						ENDCASE
*!*					ENDCASE

*!*			CASE INLIST( nKeyCode, 24, 45 )	&& Down Arrow, '-' ( Decrement )
*!*				NODEFAULT
*!*				IF ( ( .lUpDownDisabled AND nKeyCode = 24 ) OR .ReadOnly )	&& Down Arrow disabled
*!*					RETURN
*!*				ENDIF

*!*				DO CASE
*!*					CASE .DateType = 1	&& MDY 01/34/6789, 01/34/67
*!*						DO CASE			&& Use literals whenever possible for performance
*!*							CASE .SelStart < 3						&& Month
*!*								.VALUE = GOMONTH( .VALUE, -1 )
*!*								.REFRESH
*!*								.SelStart = 0
*!*								.SelLength = 2

*!*							CASE .SelStart > 1 AND .SelStart < 6	&& Day
*!*								.VALUE = .VALUE - 1
*!*								.REFRESH
*!*								.SelStart = 3
*!*								.SelLength = 2

*!*							OTHERWISE								&& Year
*!*								.VALUE = GOMONTH( .VALUE, -12 )
*!*								.REFRESH
*!*								.SelStart = 6
*!*								.SelLength = .YearSelLength
*!*						ENDCASE

*!*					CASE .DateType = 2	&& DMY 01/34/6789, 01/34/67
*!*						DO CASE
*!*							CASE .SelStart < 3						&& Day
*!*								.VALUE = .VALUE - 1
*!*								.REFRESH
*!*								.SelStart = 0
*!*								.SelLength = 2

*!*							CASE .SelStart > 1 AND .SelStart < 6	&& Month
*!*								.VALUE = GOMONTH( .VALUE, -1 )
*!*								.REFRESH
*!*								.SelStart = 3
*!*								.SelLength = 2

*!*							OTHERWISE								&& Year
*!*								.VALUE = GOMONTH( .VALUE, -12 )
*!*								.REFRESH
*!*								.SelStart = 6
*!*								.SelLength = .YearSelLength
*!*						ENDCASE

*!*					OTHERWISE			&& YMD 0123/56/89, 01/34/67
*!*						DO CASE
*!*							CASE .SelStart < .MonthStart	&& 5	&& Year
*!*								.VALUE = GOMONTH( .VALUE, -12 )
*!*								.REFRESH
*!*								.SelStart = 0
*!*								.SelLength = .YearSelLength	&& 4

*!*							CASE .SelStart > .YearEnd AND .SelStart < .DayStart	&& Month ( 3, 8 )
*!*								.VALUE = GOMONTH( .VALUE, -1 )
*!*								.REFRESH
*!*								.SelStart = .MonthStart	&& 5
*!*								.SelLength = 2

*!*							OTHERWISE								&& Day
*!*								.VALUE = .VALUE - 1
*!*								.REFRESH
*!*								.SelStart = .DayStart	&& 8
*!*								.SelLength = 2
*!*						ENDCASE
*!*				ENDCASE

*!*			CASE nKeyCode = 7								&& Del
*!*				IF ( .ReadOnly OR NOT .lAllowBlankDate )
*!*					NODEFAULT
*!*					RETURN
*!*				ENDIF
*!*				.VALUE = {  /  /  }

*!*			CASE ( nKeyCode > 47 AND nKeyCode < 60 )		&& Pass Digits
*!*				IF .ReadOnly
*!*					NODEFAULT
*!*					RETURN
*!*				ENDIF

*!*			CASE INLIST( nKeyCode, 9, 15 )					&& Pass TAB, Shift+TAB

*!*			CASE ( nKeyCode = 13 )							&& Pass the Enter key

*!*			CASE ( nKeyCode = 127 )							&& Pass the Backspace key

*!*			CASE ( nKeyCode = 27 )							&& Pass the Escape key
*!*			
*!*			CASE nKeyCode = 145 OR nKeycode = -3 OR nKeyCode = 160
*!*				this.DropDown()
*!*			OTHERWISE										&& Inhibit all other key strokes
*!*				NODEFAULT
*!*		ENDCASE
*!*	ENDWITH
&&vasant22/06/09

with this
	if !empty(.uControlsource) 
		local tFldName,tAliasName
		tFldName   = justext(.uControlsource)
		tAliasName = juststem(.uControlSource)
		SELECT (taliasname)		&&vasant
		if type(tFldname) ='T'
			replace &tfldname with this.value in &taliasname
		endif
	endif
ENDWITH

ENDPROC
PROCEDURE GotFocus
LOCAL lnStart, lnLength

WITH THIS
	.SetYearSelLength()		&& Set these property values each time we enter
	.SetStartEndPositions()	&& the control in case of programmatic changes
	.CurrentDate = .VALUE

	&&vasant22/06/09
*!*		DO CASE
*!*			CASE .DatePartInitFocus = 2	&& Day
*!*				lnLength = 2
*!*				lnStart = .DayStart

*!*			CASE .DatePartInitFocus = 3	&& Year
*!*				lnLength = .YearSelLength
*!*				lnStart = .YearStart

*!*			OTHERWISE					&& Month
*!*				lnLength = 2
*!*				lnStart = .MonthStart
*!*		ENDCASE

*!*		.SelStart = lnStart
*!*		.SelLength = lnLength
	&&vasant22/06/09
ENDWITH

ENDPROC
PROCEDURE Valid
_curtbl = Alias()
_currec = Iif(!Eof(),Recno(),0)
If Type('Thisform.ObjClickMove')!='U'
	If Thisform.ObjClickMove = .T.
		Return .T.
	Endif
Endif

If Empty( This.Value )
	If Not This.lAllowBlankDate
		Return .F.
	Endif
Else
	If Year( This.Value ) = 0
		Return .F.
	Endif
Endif
If !Empty(This.Value)
	If !Between(Month(This.Value),1,12) Or !Between(Year(This.Value),1900,2078)
		If !Between(Month(This.Value),1,12)
			This.cErrMsg = 'Month should be between Jan. to Dec.'
		Endif
		If !Between(Year(This.Value),1900,2078)
			This.cErrMsg = 'Year should be between 1900 to 2078'
		Endif
		This.Value = Ctod('')
		Return .F.
	Endif
Endif
If !Empty(This.validexpr)
*!*		RETURN EVAL(this.validexpr)
	_curval = Eval(This.validexpr)
	If Type('_curval') = 'L'
		If _curval = .F.
			If !Empty(This.cErrMsg)
				_curmes = Alltrim(Eval(This.cErrMsg))
				=Messagebox(_curmes,0+64,vumess)
			Endif
			Select (_curtbl)
			Nodefault
			If _currec > 0
				Goto _currec
			Endif
			Retu This.Value
		Endif
	Endif
Endif

With This
	If !Empty(.uControlsource)
		Local tFldName,tAliasName
		tFldName   = Justext(.uControlsource)
		tAliasName = Juststem(.uControlsource)
		Select (tAliasName)		&&vasant
		If Type(tFldName) ='T'
			Replace &tFldName With This.Value In &tAliasName
		Endif
	Endif
Endwith

ENDPROC
PROCEDURE Init
Lpara nTop, nLeft, Txtwidth, cSource, cWhen, cValid, cDefault, cError, flsource, lTextBox
If Type('nTop')<>"L" Or Type('nLeft')<>"L" Or Type('cSource')<>"L" Or Type('cWhen')<>"L" Or ;
		TYPE('cValid')<>"L" Or Type('cDefault')<>"L" Or Type('cError')<>"L" Or Type('flsource')<>"L";
		OR Type('lTextBox')<>"L"
	This.whenexpr      = Iif(!Empty(cWhen),Alltrim(cWhen),'')
	This.validexpr     = Iif(!Empty(cValid),Alltrim(cValid),'')
	This.cErrMsg       = Iif(!Empty(cError),Alltrim(cError),'')
	This.Value         = cDefault
	This.Left          = nLeft
	This.Top           = nTop
	This.Height        = 1.5
	If (Upper(lother.Data_Ty)=[D]) Or (Upper(lother.Data_Ty)=[N])
		This.Width     = 13&&20
	Else
		This.Width     = 13&&20
	Endif
	This.MaxLength     = 10
	This.ControlSource = cSource
	*This.TabIndex      = nTop/2
	This.FontSize      = 8
Endif
With This
	.SetYearSelLength()
	.SetStartEndPositions()
	mcbbtnnm = Alltrim(This.Name)+'dpkcmb'
	.cbButton_name = mcbbtnnm					&&SYS(2015)

*	Set Classlib To \Class\datepicker AddIt
	If Not Inlist( .Parent.BaseClass, 'Toolbar','Column')
		.Parent.Newobject( 	.cbButton_name ,'cbButton', '','',This)
		loBtn = Evaluate( '.parent.'+.cbButton_name)
		loBtn.SetPositionAndSize(This)
		loBtn.Visible = This.Visible
	Endif
Endwith

ENDPROC
PROCEDURE Refresh
With this
	*.ForeColor = .Parent.ForeColor	&&vasant
	.convControlsource
Endwith

ENDPROC
     7 ���    7  7                        ��   %   �.      �5  �  N0          �  U  :  ��  � %�C��  ������3 � T� � ���  �� � U  VNEWVAL THIS DATEPARTINITFOCUS�  ���  ��� � H� �� � ��� � ��: � T�� ���� ��� ���Z � T�� ���� ��� ���� � %�C� CENTURYv� ON��� � T�� ���� �� � T�� ���� � � �� U  THIS CENTURY YEARSELLENGTHD ���  ��=� H� �	� ��� ���� � T�� ���� T�� ���� T�� �� �� T�� ���� T�� ���� T�� ���� %��� ���� � T�� ��� ��� � ��� ���8� T�� �� �� T�� ���� T�� ���� T�� ���� T�� ���� T�� ���� %��� ���4� T�� ��� ��� � 2�	� T�� �� �� T�� ���� T�� ���� T�� ���� T�� ���� T�� ���� %��� ���� T�� ��� ��� T�� ��� ��� T�� ��� ��� T�� ��� ��� T�� ��� ��� � � T��	 ��	�� T��
 ���� T�� ���� T�� ���� T�� �-�� %��� ����� T��	 ���	 ��� T��
 ���
 ��� T�� ��� ��� T�� ��� ��� � T�� ���	 ��� T�� ���
 ��� T�� ��� ���2 %��� �� �� � � C� HOURSv�	��9� T�� ��� �� T�� �a�� � �� U  THIS DATETYPE DAYSTART DAYEND
 MONTHSTART MONTHEND	 YEARSTART YEAREND YEARSELLENGTH	 HOURSTART MINUTESTART SECONDSTART	 AMPMSTART HOURS24FORMAT HOUREND	 MINUTEEND	 SECONDEND HOURS5  ��  � %�C�  ��� L��. � T� � ���  �� � U  VNEWVAL THIS LUPDOWNDISABLED7  ��  � %�C��  ��� L��0 � T� � ���  �� � U  VNEWVAL THIS LEXITONTABONLY�  %��  � ���D �0 T� � �C� calendar_Form_desktop �  �  �N�� �t �( T� � �C� calendar_Form �  �  �N�� � T� � � �a�� ��C� � � �� U  THISFORM
 SHOWWINDOW THIS CALENDAR_FORM_REF ALWAYSONTOP SHOWe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS WIDTH
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS HEIGHT
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS LEFT
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEe  ��  � %��  � � ��^ � T� � ��  ��" T� �C� this.parent.� � ��� ��C � � � �� � U  VNEWVAL THIS TOP
 OBUTTONREF CBBUTTON_NAME SETPOSITIONANDSIZEd  ��  � %��  � � ��] � T� � ��  ��" T� �C� this.parent.� � ��� T� � ��  �� � U  VNEWVAL THIS VISIBLE
 OBUTTONREF CBBUTTON_NAMEp  ��  � %��  � � ��i � T� � ��  ��" T� �C� this.parent.� � ��� T� � ��  �	 � � -	�� � U  VNEWVAL THIS ENABLED
 OBUTTONREF CBBUTTON_NAME READONLYr  ��  � %��  � � ��k � T� � ��  ��" T� �C� this.parent.� � ��� T� � ��  
�
 � � -
	�� � U  VNEWVAL THIS READONLY
 OBUTTONREF CBBUTTON_NAME ENABLEDs  %�C�  � ��� D��l � %�CC�  � ��g�  � ��J � T�  � ��  � �� �h � T�  � ��  � � �� � � U  THIS VALUE BLANKDATETIMESTRING	 FORECOLOR	 BACKCOLOR PARENT  ��C�  � �� U  THIS SETFOCUSO ���  ��H� %�C�� ������ � H�0 �� � ��� ���P � T�� ���� ��� ���q � T�� ��� �� �C�� �� �� �� ���� � T�� ���� � �� �# %�C�� �� ��	 � �� �� ���� � T�� ���� � � %�C�  �
 ��� D��D� %�CC�  �
 ��g�  � ��&� T�� ��� �� �@� T�� ��� � �� � � �� U  THIS DATETYPE SELSTART	 SELLENGTH YEARSELLENGTH	 HOURSTART MINUTESTART SECONDSTART
 MONTHSTART DAYSTART VALUE BLANKDATETIMESTRING	 FORECOLOR	 BACKCOLOR PARENT" ��  � � ��� ��� H�% �� ��  � ��p � �� %��� ��L � B� � T�� �C$�� ��� � ��C�� �� ��  ���� � �� %��� ��� � B� � T�� ��� �� ��� � ��C�� �� ��  ����� %��� ��� � �� B� � %��� ����� H���� ���	 ��
 ��;� �� T��	 ���
 �� T�� ����  ���	 �� �
 ��	 �� 	��|� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��?� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ���� ���	 �� ���� �� %��� 
���� \�� {TAB}�� � 2��� �� � ��� H����� ���	 ���� �� T��	 ���� T�� ���� ���	 ��	 ��	 �	��N� �� T��	 ���� T�� ��� ��  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��R� �� T��	 ��� �� T�� ���� ���	 �� ���� �� %��� 
���� \�� {TAB}�� � 2��� �� � � ��  ����� %��� ���� �� B� � %��� ����� H����� ��� �
 ��	 �� 	��$� �� T��	 ��� �� T�� ���� ���	 �� ��X� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��\� �� T��	 ���
 �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 �� �� T�� ��� �� ���	 ��
 ���� �� %��� 
���� \��	 {BACKTAB}�� � 2��� �� � ��� H����� ��� �
 ��	 �� 	��:� �� T��	 ��� �� T�� ���� ���	 �� ��n� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� �� T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��2� �� T��	 ��� �� T�� ��� �� ���	 ��
 ��	 �� 	��q� �� T��	 ���� T�� ���� ���	 ��	 ��	 �	���� �� T��	 �� �� T�� ���� ���	 ����� �� %��� 
���� \��	 {BACKTAB}�� � 2��� �� � � �C�  ��+�=���w� ��# %��� � �  �	� �� ��A	� B� � H�R	�s� ��� ���	� H�q	�� ���	 ����	�( T�� ��� CC�� ��CC�� ����� ��� � T��	 �� �� T�� ���� ���	 ��	 ��	 �	��'
� T�� ��� ��Q �� ��� � T��	 ���� T�� ���� ���	 ��
 ��	 �� 	���
�( T�� ��� CC�� ��CC�� ����� ��� � T��	 ���� T�� ��� ��  ���	 �� �
 ��	 �� 	���
� T�� ��� ��� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��D� T�� ��� �<�� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� ��� ��� � T��	 ��� �� T�� ���� ���	 �� ����% T�� ��� C�� � �� ���  6�� ��� � T��	 ��� �� T�� ���� 2�� �� � ��� ����� H�(��� ���	 ���t� T�� ��� ��Q �� ��� � T��	 �� �� T�� ���� ���	 ��	 ��	 �	����( T�� ��� CC�� ��CC�� ����� ��� � T��	 ���� T�� ���� ���	 ��
 ��	 �� 	��J�( T�� ��� CC�� ��CC�� ����� ��� � T��	 ���� T�� ��� ��  ���	 �� �
 ��	 �� 	���� T�� ��� ��� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� �<�� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��S� T�� ��� ��� ��� � T��	 ��� �� T�� ���� ���	 �� ����% T�� ��� C�� � �� ���  6�� ��� � T��	 ��� �� T�� ���� 2��� �� � 2�s� H���o� ���	 ��
 ��5�( T�� ��� CC�� ��CC�� ����� ��� � T��	 �� �� T�� ��� ��  ���	 �� �
 ��	 �� 	����( T�� ��� CC�� ��CC�� ����� ��� � T��	 ���
 �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� ��Q �� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��V� T�� ��� ��� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� �<�� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��� T�� ��� ��� ��� � T��	 ��� �� T�� ���� ���	 �� ��c�% T�� ��� C�� � �� ���  6�� ��� � T��	 ��� �� T�� ���� 2�o� �� � � �C�  ��-����� ��# %��� � �  �	� �� ���� B� � H����� ��� ����� H���~� ���	 ���F�) T�� ��� CC�� ��CC�� ������� ��� � T��	 �� �� T�� ���� ���	 ��	 ��	 �	���� T�� ��� ��Q �� ��� � T��	 ���� T�� ����  ���	 �� �
 ��	 �� 	���) T�� ��� CC�� ��CC�� ������� ��� � T��	 ���� T�� ��� ��  ���	 �� �
 ��	 �� 	��e� T�� ��� ��� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� �<�� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��� T�� ��� ��� ��� � T��	 ��� �� T�� ���� ���	 �� ��r�% T�� ��� C�� � �� ���  6�� ��� � T��	 ��� �� T�� ���� 2�~� �� � ��� ���<� H���8� ���	 ����� T�� ��� ��Q �� ��� � T��	 �� �� T�� ���� ���	 ��	 ��	 �	��X�) T�� ��� CC�� ��CC�� ������� ��� � T��	 ���� T�� ����  ���	 �� �
 ��	 �� 	����) T�� ��� CC�� ��CC�� ������� ��� � T��	 ���� T�� ��� ��  ���	 �� �
 ��	 �� 	��� T�� ��� ��� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��w� T�� ��� �<�� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� ��� ��� � T��	 ��� �� T�� ���� ���	 �� ��,�% T�� ��� C�� � �� ���  6�� ��� � T��	 ��� �� T�� ���� 2�8� �� � 2��� H�Q��� ���	 ��
 ����) T�� ��� CC�� ��CC�� ������� ��� � T��	 �� �� T�� ��� ��  ���	 �� �
 ��	 �� 	�� �) T�� ��� CC�� ��CC�� ������� ��� � T��	 ���
 �� T�� ����  ���	 �� �
 ��	 �� 	��{� T�� ��� ��Q �� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� ��� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	��,� T�� ��� �<�� ��� � T��	 ��� �� T�� ����  ���	 �� �
 ��	 �� 	���� T�� ��� ��� ��� � T��	 ��� �� T�� ���� ���	 �� ����% T�� ��� C�� � �� ���  6�� ��� � T��	 ��� �� T�� ���� 2��� �� � � ��  ���>� %��� � �� 
��&� �� B� � T�� ��        ��3 ��  �/� �  �<	� C�  �A�a�P�p����� %��� ���� �� B� � �C�  �	������ ��  ����� ��  ����� ��  �����( ��  ���	 �  ���� �  ����� ��C� � �� 2�� �� � �� U  NKEYCODE NSHIFTALTCTRL THIS READONLY VALUE REFRESH SETFOCUS CURRENTDATETIME DATETYPE SELSTART
 MONTHSTART	 SELLENGTH YEAREND DAYSTART MONTHEND	 HOURSTART DAYEND MINUTESTART HOUREND SECONDSTART	 MINUTEEND	 AMPMSTART LEXITONTABONLY YEARSELLENGTH	 YEARSTART HOURS24FORMAT	 SECONDEND LUPDOWNDISABLED LALLOWBLANKDATE DROPDOWN�  ��  � � ��� ��� � ��C�� �� ��C�� �� T�� ��� �� H�J �� � ��� ���w � T� ���� T�  ��� �� ��� ���� � T� ���	 �� T�  ���
 �� 2�� � T� ���� T�  ��� �� � T�� ��  �� T�� �� �� �� U  LNSTART LNLENGTH THIS SETYEARSELLENGTH SETSTARTENDPOSITIONS CURRENTDATETIME VALUE DATEPARTINITFOCUS DAYSTART YEARSELLENGTH	 YEARSTART
 MONTHSTART SELSTART	 SELLENGTHb  %�C�  � ���0 � %��  � 
��, �	 B�� �� � �[ � %�C�  � i� ��W �	 B�� �� � � U  THIS VALUE LALLOWBLANKDATE�  ���  ��� � ��C�� �� ��C�� �� T�� �C��]��' %�C�� � � Toolbar� Column�
��� �' ��C�� � cbButton�  �   �  �� � �� T� �C� .parent.�� ��� ��C �  � � �� T� �	 ��  �	 �� � �� U
  THIS SETYEARSELLENGTH SETSTARTENDPOSITIONS CBBUTTON_NAME PARENT	 BASECLASS	 NEWOBJECT LOBTN SETPOSITIONANDSIZE VISIBLE datepartinitfocus_assign,     �� setyearsellength�     �� setstartendpositionsi    �� lupdowndisabled_assignq    �� lexitontabonly_assign�    �� dropdown$    �� width_assign    �� height_assign�    �� left_assignc    ��
 top_assign	    �� visible_assign�	    �� enabled_assignW
    �� readonly_assign    �� Refresh�    �� Clickr    �� InteractiveChange�    �� KeyPress�    �� GotFocus+    �� Valid�,    �� Init8-    ��1 q �!A 2 � � !� "� "�� � � A A A 3 � � !� � � � � � "1A "� � � � � � "1A � � � � � � � "11111A A � � � � � "1111A 211"� � A B 3 q R!A 3 q r!A 3 A� �A 3 q B!A 3 q B!A 3 q A!A 3 q B!A 3 q A!B 3 q A!�A 3 q A!�A 3 ��1� aA A 3 � 2 � a� !� "� �� A � 1� A A ��� � !A A A 3 � � � A � A A � � � A � A A � � � � A A A !� 1A � � A � � A � � A � � A � � A � � 2A � � A � A A � � !A � � �A � � A � � A � � A � � A � � 2A � � A � A A A � A A A !� �A � � 2A � � A � � A � � A � � A � � A � � 2A � !A � A A � � �A � � 2A � � A � � A � � A � � �A � � �A � � "A � !A � A A A �A 1A A � !� !�� � � �a� � � ��� � � A� � � 1� � � 1� � � 2Q� � � � A A "� !a� � � ��� � � ��� � � A� � � 1� � � 1� � � 2Q� � � � A A � � 1�� � � �� � � a� � � A� � � 1� � � 1� � � 2Q� � � � A A A RA 1A A � !� !�� � � �a� � � �� � � A� � � 1� � � 1� � � 2Q� � � � A A "� !a� � � ��� � � �� � � A� � � 1� � � 1� � � 2Q� � � � A A � � 1�� � � �� � � a� � � A� � � 1� � � 1� � � 2Q� � � � A A A �A A A A2� A A A R"""�� � A A A 3 � � � � � � !� � "� � � � � A � � A 3 !� A � a� A A 3 � � � qq�1A A 2                    $   �         �     	      2  �     N   �  F	  e   S   r	  
  m   X   !
  +  u   `   N  3     g   W  >  �   n   `    �   u   0    �   |   5  �  �   �     �  �   �   �  �  �   �   �  �  �   �   �  �  �   �   �  X  �   �   w  �X  �   v  �X  [  %  �  +[  �[  @  �  �[  !]  L   )   7                  0    m                   PLATFORM   C                  UNIQUEID   C	   
               TIMESTAMP  N   
               CLASS      M                  CLASSLOC   M!                  BASECLASS  M%                  OBJNAME    M)                  PARENT     M-                  PROPERTIES M1                  PROTECTED  M5                  METHODS    M9                  OBJCODE    M=                 OLE        MA                  OLE2       ME                  RESERVED1  MI                  RESERVED2  MM                  RESERVED3  MQ                  RESERVED4  MU                  RESERVED5  MY                  RESERVED6  M]                  RESERVED7  Ma                  RESERVED8  Me                  USER       Mi                                                                                                                                                                                                                                                                                          COMMENT Class                                                                                               WINDOWS _1SK0YLW1M 895782231N      �  =      �      �&  �          Y  �	  	          �               WINDOWS _1WL0UKZV2 895782231f  n  _  F  ,  z      +r  w�                                              COMMENT RESERVED                        �                                                                 WINDOWS _1SK0YLW1M 895782236      H  
      �      �#  KP          -
  Q	  S          E               WINDOWS _1SK0YLW1N 8957822367      �	   
  
  $      Ҏ  ��                                              COMMENT RESERVED                        5      �                                                           WINDOWS _1SK0YLW1M 898210070 	      $  1	      �      �0  *l          
	  	  �          1               WINDOWS _1SK0YLW1O 898210070�      w  �	  A	        ��  �                                              COMMENT RESERVED                        �                                                                 WINDOWS _2AL0RHEYW 944137842,      �        A        Y          �  ,                             COMMENT RESERVED                              �                                                           WINDOWS _0NU0YJI241051283513�      j	  Z	      �      ��  m�          �  ?              �               COMMENT RESERVED                        q      R                                                           WINDOWS _S050WHPLL1062557720.      }  >      �      �  ��          B  O  M          4             COMMENT RESERVED                        �                                                                 WINDOWS _0NU0YIHSX1066828484�      �  �      @/      Z�  A�          �  �  7          �               COMMENT RESERVED                        �      �                                                           WINDOWS _1SK0YLW1M1100189479Y      �  j      /0      9T  <6          C  P  �          5               WINDOWS _1WL0UKZV21100189479�  �  �  �  y  �      JF  !=                                               COMMENT RESERVED                        �      �                                                           WINDOWS _1SK0YLW1M1147883707f      �  w      :
      �  *          �  �  �          X               WINDOWS _1WH0WR81V 895782297�      �  �  �  {      }�  �Y                                               COMMENT RESERVED                        �      �                                                           WINDOWS _4SI0RKJMG1233350475�.      j        m.      g            /  /              %/               COMMENT RESERVED                        3/      �;                                                           WINDOWS _28Z0RR06H1255641959�      �  �      l      �x �4                         �               COMMENT RESERVED                        �      �                                                           WINDOWS _1Y90Z0A151256617173�      U  �      �      UM �                          �               COMMENT RESERVED                              �                                                           WINDOWS _S050WE6S01257202476�      �  �      �;      �� ��          �  �  �          P  ^           COMMENT RESERVED                        A                                                                 �                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 VERSION =   3.00      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      txtxtra      Pixels      2textbox for getting values from user for extradata      Class      1      textbox      txtxtra      textbox      2      chkxtra      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      cmdexbtn      	ainfodate      	ainfodate      	container      textbox      datepicker.vcx      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      grddate      1      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      ainfochk      Class      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      Pixels      Class      1      Pixels      	container      ainfochk      ainfochk      Check1      checkbox      checkbox      	container      ainfocmd      cmdnarr      Pixels      #Check box for entering logical data      checkbox      chkxtra      (validexpr
whenexpr
defaexpr
cerrmsg
      checkbox      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      Class      commandbutton      cmdnarr      commandbutton      commandbutton      Pixels      Class      1      Pixels      Class      2      	container      grddate      grddate      Dpk1      textbox      !e:\vudyogsdk\class\datepicker.vcx      dpk      	container      cmdbom      Pixels      1      "Tahoma, 0, 8, 5, 13, 11, 21, 2, 0
      textbox      Dpk1      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      �Top = 0
Left = 2
Height = 16
Width = 18
FontSize = 8
Alignment = 4
BackStyle = 0
Caption = ""
TabIndex = 2
Name = "Check1"
      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      Pixels      1      	container      Class      dpk      cmdextra      Pixels      Class      commandbutton      Qactiveobjname
defaexpr
whenexpr
validexpr
cerrmsg
nheight
nwidth
csource
      Class      2      	container      ainfocmd      ainfocmd      2      cmdextra      commandbutton      Qactiveobjname
cerrmsg
defaexpr
validexpr
whenexpr
nheight
nwidth
csource
      2      Command1      textbox      Text1      ainfotxt      ainfotxt      Class      �Width = 198
Height = 17
BorderWidth = 0
activeobjname = Check1
defaexpr = .F.
whenexpr = .F.
validexpr = .F.
cerrmsg = .F.
nheight = .F.
nwidth = .F.
csource = .F.
Name = "ainfochk"
      kTop = 0
Left = 2
Height = 18
Width = 60
FontSize = 8
Caption = ".."
TabIndex = 2
Name = "Command1"
      commandbutton      commandbutton      {activeobjname
cmdcontrol
cmdcaption
lsttable
lstcond
splchk
nheight
nwidth
defaexpr
whenexpr
validexpr
cerrmsg
      	container      ainfotxt      Pixels      Qactiveobjname
defaexpr
whenexpr
validexpr
cerrmsg
nheight
nwidth
csource
      Pixels      	ainfodate      	container     Width = 198
Height = 17
BorderWidth = 0
activeobjname = Command1
cmdcontrol = .F.
cmdcaption = .F.
lsttable = .F.
lstcond = .F.
splchk = .F.
nheight = .F.
nwidth = .F.
defaexpr = .F.
whenexpr = .F.
validexpr = .F.
cerrmsg = .F.
Name = "ainfocmd"
      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      	container      �FontSize = 8
BackStyle = 0
BorderStyle = 0
Height = 18
Left = 0
Margin = 2
TabIndex = 2
Top = 0
Width = 198
Name = "Text1"
      �Width = 198
Height = 18
BorderWidth = 0
activeobjname = Text1
defaexpr = .F.
whenexpr = .F.
validexpr = .F.
cerrmsg = .F.
nheight = .F.
nwidth = .F.
csource = .F.
Name = "ainfotxt"
      �FontSize = 8
BackStyle = 0
BorderStyle = 0
Height = 18
Left = 0
Margin = 2
TabIndex = 2
Top = 0
BorderColor = 0,255,64
Name = "Dpk1"
      1      
cmdreceipt      commandbutton      fHeight = 21
Width = 62
FontSize = 8
Caption = "Receipt"
PicturePosition = 2
Name = "cmdreceipt"
      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      �Width = 198
Height = 17
BackStyle = 1
BorderWidth = 0
activeobjname = Dpk1
cerrmsg = .F.
defaexpr = .F.
validexpr = .F.
whenexpr = .F.
nheight = .F.
nwidth = .F.
csource = .F.
Name = "ainfodate"
     Height = 16
Width = 18
FontSize = 8
AutoSize = .T.
Alignment = 2
BackStyle = 0
Caption = ""
PicturePosition = 14
ForeColor = 255,255,255
BackColor = 0,0,255
DisabledForeColor = 0,0,0
DisabledBackColor = 228,220,250
defaexpr = .F.
cerrmsg = .F.
Name = "chkxtra"
     $FontName = "Arial"
FontSize = 8
BorderStyle = 0
Height = 18
Left = 0
Margin = 0
TabIndex = 2
Top = 0
ForeColor = 255,255,255
BackColor = 0,0,255
DisabledBackColor = 228,220,250
SelectedForeColor = 255,255,255
DisabledForeColor = 0,0,0
SelectedBackColor = 0,0,255
Name = "Dpk1"
      7activeobjname
cerrmsg
defaexpr
validexpr
whenexpr
      Ecmdcaption
lsttable
lstcond
splchk
cmdcontrol
whenexpr
cquery
      !Arial, 0, 8, 5, 14, 11, 29, 3, 0
      commandbutton      cmdbom      Class      commandbutton      Pixels      Class      
cmdreceipt     2���                              �I   %   Q      �  )   n          �  U  �/ ��  � � � � � � � � �	 �
 � %�C� ntopb� N��T � T�  �� �� � %�C� nleftb� N��~ � T� �� �� � %�C� nheightb� N��� � T� �� �� � %�C� nWidthb� N��� � T� �� �� � %�C� cControlb� C��� T� ��  �� � %�C� cCaptionb� C��/� T� ��  �� � %�C�	 cLsttableb� C��]� T� ��  �� � %�C� cLstcondb� C���� T� ��  �� � U  NTOP NLEFT NHEIGHT NWIDTH CCONTROL CCAPTION	 CLSTTABLE CLSTCOND CSPLCHK CWHNCOND CQUERY� ) %�C� Thisform.NoVouRefreshb� L��7 � T�  � �a�� � 7� � � T� ��  � �� T� ��  � ��& %�C� thisform.cmdNarwhnb� L��� � %��  � -��� � T� �-�� T� �-�� � � � ptsrch� U  THISFORM NOVOUREFRESH _CMDPTADDMODE _CMDPTEDITMODE ADDMODE EDITMODE	 CMDNARWHN PTSRCH Init,     �� Click#    ��1 ��� A �� A �� A �� A �� A �� A �� A �� A 4 �� A � a!� � A A � 2                       �        �  �  $    )                          T���    ;  ;                        6   %   �      �     �          �  U  ( %��9�  � � WK� C� item_vw�	���, T� �C� � �� uefrm_bomdetailsIP.scx�� %�C� 0��� T� �C�� T� �CO��? � uefrm_OpWkItemAllocation��9�  � �9�  �	 �9�  �
 � � %�C� �
��� � Select &_Malias
 � %�C� �CN���� �	 #�� �� � � � U 
 ACTIVEFORM PCVTYPE FILENM COMPANY DIR_NM _MALIAS _MRECNO UEFRM_OPWKITEMALLOCATION DATASESSIONID ADDMODE EDITMODE THISFORM Click,     ��1 ��� � � �1A A� A A A 2                       �      )   ;                        commandbutton      cmdexbtn      sHeight = 22
Width = 60
FontSize = 8
Caption = ".. "
PicturePosition = 1
PictureMargin = 2
Name = "cmdexbtn"
      commandbutton      commandbutton      ptbtn      Twhenexpr
validexpr
cerrmsg
defaexpr
defahelp
helptype
defahelpcond
fullname
      |Height = 22
Width = 84
FontSize = 8
Caption = "\<Addl. Info"
PicturePosition = 1
PictureMargin = 2
Name = "cmdextra"
      �Height = 21
Width = 62
FontSize = 8
Picture = ..\bmp\finish_item.gif
Caption = "BOM"
SpecialEffect = 0
PicturePosition = 2
Name = "cmdbom"
     �PROCEDURE Click
IF _SCREEN.ActiveForm.pcVtype='WK' AND USED('item_vw')
	filenm=ALLTRIM(company.dir_nm)+"uefrm_bomdetailsIP.scx"
	IF FILE(filenm)
		_Malias 	= Alias()
		_mRecNo	= Recno()
		do form uefrm_OpWkItemAllocation WITH _SCREEN.ActiveForm.DataSessionId,_screen.activeform.addmode,_screen.activeform.editmode,thisform
		If !Empty(_Malias)
			Select &_Malias
		ENDIF
		If Betw(_mRecNo,1,Reccount())
			Go _mRecNo
		ENDIF
	ENDIF
ENDIF

ENDPROC
     �PROCEDURE Init
LPARA nHeight, nWidth, cSource, cWhen, cDefault, cValid, cError
IF !INLIST(TYPE('nheight'),'N','C')
	nheight = 0
Endif	
IF !INLIST(TYPE('nWidth'),'N','C')
	nWidth = 0
Endif	
IF TYPE('csource') # 'C'
	csource = ''
Endif	
IF TYPE('cwhen') # 'C'
	cwhen = ''
Endif	
IF TYPE('cdefault') # 'C'
	cdefault = ''
Endif	
IF TYPE('cvalid') # 'C'
	cvalid = ''
Endif	
IF TYPE('cerror') # 'C'
	cerror = ''
Endif	
this.Defaexpr      = cDefault
this.whenexpr      = cWhen
this.validexpr     = cValid
this.cErrMsg       = cError
this.nheight	   = nHeight
this.nwidth  	   = nWidth
this.csource  	   = cSource
_curobjname  = 'This.'+This.ActiveObjName
&_curobjname..Left          = 2
&_curobjname..Top           = 0
&_curobjname..Height        = IIF(TYPE('this.nheight')='N',this.nheight,EVALUATE(this.nheight))
&_curobjname..Width     	= &_curobjname..Height
&_curobjname..FontSize      = 8

ENDPROC
     ]PROCEDURE SetFocus
*!*	REPLACE Currow1a WITH .t. IN AInfo_vw
ENDPROC
PROCEDURE Init
LPARA nHeight, nWidth, cSource, cWhen, cDefault, cValid, cError
IF !INLIST(TYPE('nheight'),'N','C')
	nheight = 0
Endif	
IF !INLIST(TYPE('nWidth'),'N','C')
	nWidth = 0
Endif	
IF TYPE('csource') # 'C'
	csource = ''
Endif	
IF TYPE('cwhen') # 'C'
	cwhen = ''
Endif	
IF TYPE('cdefault') # 'C'
	cdefault = ''
Endif	
IF TYPE('cvalid') # 'C'
	cvalid = ''
Endif	
IF TYPE('cerror') # 'C'
	cerror = ''
Endif	
this.Defaexpr      = cDefault
this.whenexpr      = cWhen
this.validexpr     = cValid
this.cErrMsg       = cError
this.nheight	   = nHeight
this.nwidth  	   = nWidth
this.csource  	   = cSource
_curobjname  = 'This.'+This.ActiveObjName
&_curobjname..Left          = 0
&_curobjname..Top           = 0
&_curobjname..FontSize      = 8

ENDPROC
     PROCEDURE Init
LPARA nHeight, nWidth, cSource, cWhen, cDefault, cValid, cError
IF !INLIST(TYPE('nheight'),'N','C')
	nheight = 0
Endif	
IF !INLIST(TYPE('nWidth'),'N','C')
	nWidth = 0
Endif	
IF TYPE('csource') # 'C'
	csource = ''
Endif	
IF TYPE('cwhen') # 'C'
	cwhen = ''
Endif	
IF TYPE('cdefault') # 'C'
	cdefault = ''
Endif	
IF TYPE('cvalid') # 'C'
	cvalid = ''
Endif	
IF TYPE('cerror') # 'C'
	cerror = ''
Endif	
this.Defaexpr      = cDefault
this.whenexpr      = cWhen
this.validexpr     = cValid
this.cErrMsg       = cError
this.nheight	   = nHeight
this.nwidth  	   = nWidth
this.csource  	   = cSource
_curobjname  = 'This.'+This.ActiveObjName
&_curobjname..Left          = 0
&_curobjname..Top           = 0
&_curobjname..FontSize      = 8

ENDPROC
     U���    <  <                        �F   %   �      �  %   �          �  U  � ��  � � � � � � �! %�CC� nheightb� N� C�
��M � T�  �� �� �  %�CC� nWidthb� N� C�
��~ � T� �� �� � %�C� csourceb� C��� � T� ��  �� � %�C� cwhenb� C��� � T� ��  �� � %�C� cdefaultb� C��� T� ��  �� � %�C� cvalidb� C��,� T� ��  �� � %�C� cerrorb� C��W� T� ��  �� � T� � �� �� T� �	 �� �� T� �
 �� �� T� � �� �� T� �  ��  �� T� � �� �� T� � �� �� T� �� This.� � ��# &_curobjname..Left          = 2
# &_curobjname..Top           = 0
c &_curobjname..Height        = IIF(TYPE('this.nheight')='N',this.nheight,EVALUATE(this.nheight))
3 &_curobjname..Width     	= &_curobjname..Height
# &_curobjname..FontSize      = 8
 U  NHEIGHT NWIDTH CSOURCE CWHEN CDEFAULT CVALID CERROR THIS DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG _CUROBJNAME ACTIVEOBJNAME Init,     ��1 �� A � A �� A �� A �� A �� A �� A �111112                       �      )   <                        �Height = 22
Width = 84
FontSize = 8
Caption = "T & C"
SpecialEffect = 2
PicturePosition = 2
PictureMargin = 2
Name = "ptbtn"
      commandbutton      1      Class      Pixels      ptbtn      �Height = 22
Width = 84
FontSize = 8
Caption = "Narration"
PicturePosition = 1
PictureMargin = 2
cmdcaption = .F.
lsttable = .F.
lstcond = .F.
splchk = .F.
cmdcontrol = .F.
whenexpr = .F.
cquery = .F.
Name = "cmdnarr"
      �Width = 198
Height = 17
BackStyle = 0
BorderWidth = 0
activeobjname = Dpk1
cerrmsg = .F.
defaexpr = .F.
validexpr = .F.
whenexpr = .F.
Name = "grddate"
     [PROCEDURE Init
LPARA nHeight, nWidth, cControl, cCaption, cLsttable, cLstcond, cSplchk,cWhen, cDefault, cValid, cError
IF !INLIST(TYPE('nheight'),'N','C')
	nheight = 0
Endif	
IF !INLIST(TYPE('nWidth'),'N','C')
	nWidth = 0
Endif	
IF TYPE('cControl') # 'C'
	cControl = ''
Endif	
IF TYPE('cCaption') # 'C'
	cCaption = ''
Endif	
IF TYPE('cLsttable') # 'C'
	cLsttable = ''
Endif	
IF TYPE('cLstcond') # 'C'
	cLstcond = ''
Endif	
IF TYPE('cwhen') # 'C'
	cwhen = ''
Endif	
IF TYPE('cdefault') # 'C'
	cdefault = ''
Endif	
IF TYPE('cvalid') # 'C'
	cvalid = ''
Endif	
IF TYPE('cerror') # 'C'
	cerror = ''
Endif	

this.Cmdcontrol    = ALLTRIM(cControl)
this.Cmdcaption    = ALLTRIM(cCaption)
this.Lsttable      = ALLTRIM(cLsttable)
this.Lstcond       = ALLTRIM(cLstcond)
this.Splchk        = cSplchk
this.nheight	   = nHeight
this.nwidth  	   = nWidth
this.Defaexpr      = cDefault
this.whenexpr      = cWhen
this.validexpr     = cValid
this.cErrMsg       = cError

_curobjname  = 'This.'+This.ActiveObjName
&_curobjname..Left          = 1
&_curobjname..Top           = 0
&_curobjname..Height        = IIF(TYPE('this.nheight')='N',this.nheight,EVALUATE(this.nheight))
&_curobjname..Width     	= IIF(TYPE('this.nwidth')='N',this.nwidth,EVALUATE(this.nwidth))
&_curobjname..FontSize      = 8
&_curobjname..Caption  		= []

ENDPROC
     N���    5  5                        ޹   %   �      �  /   �          �  U  �' ��  � � � � � � � � � %�C� ntopb� N��L � T�  �� �� � %�C� nleftb� N��v � T� �� �� � %�C� nheightb� N��� � T� �� �� � %�C� nWidthb� N��� � T� �� �� � %�C� csourceb� C��� � T� ��  �� � %�C� cwhenb� C��#� T� ��  �� � %�C� cdefaultb� C��P� T� ��  �� � %�C� cvalidb� C��{� T� ��  �� � %�C� cerrorb� C���� T� ��  �� � T�	 �
 �C� ��� T�	 � �C� ��� T�	 � �C� ��� T�	 � �C� ��� T� �� This.�	 � ��( &_curobjname..Left           = nLeft
 %�C�	 � � f� COLUMN����+ &_curobjname..Top            = nTop + 1
. &_curobjname..Height         = nHeight - 5
+ &_curobjname..Width     	 = nWidth * 8	
 �[�' &_curobjname..Top            = nTop
. &_curobjname..Height         = nHeight - 5
( &_curobjname..Width     	 = nWidth		
 �$ &_curobjname..FontSize       = 8
* &_curobjname..UControlSource = cSource
 U  NTOP NLEFT NHEIGHT NWIDTH CSOURCE CWHEN CDEFAULT CVALID CERROR THIS DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG _CUROBJNAME ACTIVEOBJNAME PARENT NAME Init,     ��1 q�� A �� A �� A �� A �� A �� A �� A �� A �� A !!!!������� q��A B�2                       �      )   5                        !Arial, 0, 8, 5, 14, 11, 29, 3, 0
     ^FontSize = 8
BorderStyle = 0
Height = 22
Margin = 0
TabIndex = 1
Width = 100
ForeColor = 255,255,255
BackColor = 0,0,255
DisabledBackColor = 228,220,250
SelectedForeColor = 255,255,255
DisabledForeColor = 0,0,0
SelectedBackColor = 0,0,255
defaexpr = .F.
defahelp = .F.
helptype = .F.
defahelpcond = .F.
fullname = 
Name = "txtxtra"
     	!���    	  	                        �a   %   �      �  W             �  U  d" %�C�  � � � f� COLUMN��M �  %��  � � � �  � � ��I � B�-�� � � T� �C�  � �� dpkcmb��" %�C�  � � � f� COLUMN����) This.Parent.&mcbbtnnm..Top    = 0 - 1
G This.Parent.&mcbbtnnm..Height = This.Parent.Parent.Parent.RowHeight
A This.Parent.&mcbbtnnm..Width  = This.Parent.&mcbbtnnm..Height
a This.Parent.&mcbbtnnm..Left   = (This.Parent.Parent.Width - This.Parent.&mcbbtnnm..Width) + 1
 ��, This.Parent.&mcbbtnnm..Top    = This.Top
/ This.Parent.&mcbbtnnm..Height = This.Height
A This.Parent.&mcbbtnnm..Width  = This.Parent.&mcbbtnnm..Height
: This.Parent.&mcbbtnnm..Left   = This.left + This.Width
 � T� �C��� T� ��  � �� %�C� �
��=� %�C� b� T����! This.Value	= Ttod(&_defvalue)
 �� This.Value	= &_defvalue
 �1 T�  � �CC�  � i�l� �        � �  � 6�� � %��	 �
 � �	 � ��]� T�	 � ��  �� %�C�  � ����� %�C�  � � �
���� T� �CC�  � � ���� %�C� �
���� T�  � �� �� � � � %�C�  � � �
��Y� T� �C�  � � ��� %�C� _curvalb� L��=� %�� -��9� \�� {TAB}�� � �U� T�  � �� �� � � � U  THIS PARENT NAME CURRENTCONTROL MCBBTNNM	 _DEFAWHEN	 _DEFVALUE UCONTROLSOURCE VALUE THISFORM ADDMODE EDITMODE LISTTBL DEFAEXPR _CURVAL WHENEXPR� %��  � � �  � ���� %��  � a�	 �  � a��< � B� � T� �C��� %�� 
��k � �� B�� � �� � T� �C� �	 ����
 F�� ��  T�
 �CCCO�CN�� CO� � 6�� %�C� � � �
���� T� �CC� � � ���� %�C� _curvalb� L���� %�� -���� %�C� � � ���1� T� �� Invalid Date�� �P� T� �CC� � � ���� � ��C � �  � �  � ��
 F�� �� �� %��
 � ����	 #��
 �� � B�� � �� � � � T�  � ��  �� %�CC|�0�9����� \�� {TAB}�� � � U  THISFORM ADDMODE EDITMODE
 FLAGCANCEL OBJCLICKMOVE
 _DEFAVALID THIS VALUE _CURTBL UCONTROLSOURCE _CURREC PARENT	 VALIDEXPR _CURVAL CERRMSG _CURMES SHOWMESSAGEBOX VUMESS LISTTBL When,     �� Valid2    ��1 "q A A �!�q� ���A � A� �A A �!aqA A A aQ�� � A � A A A 3 ��A A � � A � A 1� aq�� Q�� qA �� A � A � A A A A� A A 2                       g     2   �  �	  7    )   	                       	�PROCEDURE When

IF UPPER(This.Parent.Parent.Name) = 'COLUMN'
	IF This.Parent.Parent.CurrentControl # This.Parent.Name
		RETURN .f.
	Endif	
ENDIF	
mcbbtnnm = Alltrim(This.Name)+'dpkcmb'
IF UPPER(This.Parent.Parent.Name) = 'COLUMN'
	This.Parent.&mcbbtnnm..Top    = 0 - 1
	This.Parent.&mcbbtnnm..Height = This.Parent.Parent.Parent.RowHeight
	This.Parent.&mcbbtnnm..Width  = This.Parent.&mcbbtnnm..Height
	This.Parent.&mcbbtnnm..Left   = (This.Parent.Parent.Width - This.Parent.&mcbbtnnm..Width) + 1
ELSE
	This.Parent.&mcbbtnnm..Top    = This.Top
	This.Parent.&mcbbtnnm..Height = This.Height
	This.Parent.&mcbbtnnm..Width  = This.Parent.&mcbbtnnm..Height
*!*		This.Parent.&mcbbtnnm..Left   = This.Parent.Width - This.Parent.&mcbbtnnm..Width	&& Changed By Sachin N. S. on 22/02/2010 for TKT-438
	This.Parent.&mcbbtnnm..Left   = This.left + This.Width
ENDIF	

_defawhen =DODEFAULT()
_defvalue = This.Ucontrolsource
IF !EMPTY(_defvalue)
	IF TYPE(_defvalue)='T'
		This.Value	= Ttod(&_defvalue)
	ELSE
		This.Value	= &_defvalue
	ENDIF
	this.value=IIF(YEAR(This.Value) <= 1900,{},this.value) &&added by satish pal for bug-5905 dated 14/12/2012		
ENDIF
IF Thisform.AddMode Or Thisform.EditMode
	Thisform.ListTbl = []
	IF EMPTY(This.Value) 
		IF !EMPTY(this.parent.Defaexpr)
			_curval = EVAL(ALLTRIM(this.parent.Defaexpr))
			IF !EMPTY(_curval)
				This.Value = _curval
			Endif	
		Endif	
	ENDIF
	IF !EMPTY(this.parent.whenexpr)
		_curval = EVAL(this.parent.whenexpr)
		IF TYPE('_curval') = 'L'
			IF _curval = .f.
				KEYBOARD '{TAB}'
			Endif	
		Else
			This.Value = _curval
		Endif	
	ENDIF
ENDIF

ENDPROC
PROCEDURE Valid
If Thisform.Addmode Or Thisform.Editmode
	If Thisform.FlagCancel=.t. OR thisform.objclickmove = .t.
		Return
	ENDIF
	_defavalid=DODEFAULT()
	IF !_defavalid
		NoDefault
		Retu This.Value
	Endif	
  	_curtbl = JUSTSTEM(This.UControlSource)
	SELECT (_curtbl)
	_currec = IIF(BETWEEN(RECNO(),1,RECCOUNT()),RECNO(),0)
	IF !EMPTY(this.parent.validexpr)
		_curval = EVAL(ALLTRIM(this.parent.validexpr))
		IF TYPE('_curval') = 'L'
			IF _curval = .f.
				IF EMPTY(this.parent.cerrmsg)
					_curmes = 'Invalid Date'
				Else	
					_curmes = EVAL(ALLTRIM(this.parent.cerrmsg))
				Endif	
				Thisform.ShowMessageBox(_curmes,0+32,vumess)
				SELECT (_curtbl)
				NoDefault
				If _currec > 0
					GoTo _currec
				ENDIF
				Retu This.Value
			Endif	
		Endif	
	ENDIF
	Thisform.ListTbl = []		
	IF BETWEEN(LASTKEY(),48,57)
		KEYBOARD '{TAB}'
	Endif
ENDIF

ENDPROC
     ����    �  �                        7�   %         t  $   ,          �  U    U  P ��  � � � � � � �! %�CC� nheightb� N� C�
��M � T�  �� �� �  %�CC� nWidthb� N� C�
��~ � T� �� �� � %�C� csourceb� C��� � T� ��  �� � %�C� cwhenb� C��� � T� ��  �� � %�C� cdefaultb� C��� T� ��  �� � %�C� cvalidb� C��,� T� ��  �� � %�C� cerrorb� C��W� T� ��  �� � T� � �� �� T� �	 �� �� T� �
 �� �� T� � �� �� T� �  ��  �� T� � �� �� T� � �� �� T� �� This.� � ��# &_curobjname..Left          = 0
# &_curobjname..Top           = 0
# &_curobjname..FontSize      = 8
 U  NHEIGHT NWIDTH CSOURCE CWHEN CDEFAULT CVALID CERROR THIS DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG _CUROBJNAME ACTIVEOBJNAME SetFocus,     �� Init3     ��1 3 �� A � A �� A �� A �� A �� A �� A �1112                       =         X   R      )   �                       �PROCEDURE Init
LPARA nTop, nLeft, nHeight, nWidth, cSource, cWhen, cDefault, cValid, cError
IF TYPE('ntop') # 'N'
	ntop = 0
Endif	
IF TYPE('nleft') # 'N'
	nleft = 0
Endif	
IF TYPE('nheight') # 'N'
	nheight = 0
Endif	
IF TYPE('nWidth') # 'N'
	nWidth = 0
Endif	
IF TYPE('csource') # 'C'
	csource = ''
Endif	
IF TYPE('cwhen') # 'C'
	cwhen = ''
Endif	
IF TYPE('cdefault') # 'C'
	cdefault = ''
Endif	
IF TYPE('cvalid') # 'C'
	cvalid = ''
Endif	
IF TYPE('cerror') # 'C'
	cerror = ''
Endif	
this.Defaexpr      = ALLTRIM(cDefault)
this.whenexpr      = ALLTRIM(cWhen)
this.validexpr     = ALLTRIM(cValid)
this.cErrMsg       = ALLTRIM(cError)
_curobjname  = 'This.'+This.ActiveObjName
&_curobjname..Left           = nLeft
IF UPPER(This.Parent.Name) = 'COLUMN'
	&_curobjname..Top            = nTop + 1
	&_curobjname..Height         = nHeight - 5
	&_curobjname..Width     	 = nWidth * 8	&& Added By Sachin N. S. on 22/02/2010 for TKT-438
ELSE
	&_curobjname..Top            = nTop
*!*		&_curobjname..Height         = nHeight	&& Changed By Sachin N. S. on 22/02/2010 for TKT-438
	&_curobjname..Height         = nHeight - 5
	&_curobjname..Width     	 = nWidth		&& Added By Sachin N. S. on 22/02/2010 for TKT-438
Endif	
*!*	&_curobjname..Width     	 = nWidth * 8		&& Commented By Sachin N. S. on 22/02/2010 for TKT-438
&_curobjname..FontSize       = 8
&_curobjname..UControlSource = cSource

ENDPROC
     <���    #  #                        �J   %   X      �  �   �          �  U  �  %�C|�	� C|���b �  %��  � � � � � � ��^ �# ��C�  � ��� � � � � �� � � %�C|���� �  %��  � � � � � � ��� �/ ��C�  � �� � � � � � � � � � �� � � U  THISFORM
 CUR_ACTCOL THIS PARENT COLUMNCOUNT ACTIVATECELL
 CUR_ACTROW: %��  � � �  � ��3� %��  � a��0 � B� � T� �C� � ���� T� �C� � ����
 F�� �� T� �CC+
� CO� � 6�� %��  �	 ���� � T�  �	 ��  �	 ���
 F�� �� � T�  �	 ��  �	 ��� %�C� �
 � �
��1� T� �C� �
 � ��� %�C� �
��-� T� �C� ��� %�C� _curvalb� L��)� %�� -��%� T� �� Invalid Input�� T� �� �
 � �� %�C� �
���� T� �C� ��� %�C� �
���� T� �CC� Λ�� T� �CC� �
� � � � 6�� � � ��C � �  � �  � ��
 F�� �� �� %�� � ���	 #�� �� � B�� � �� � � � � T� �CC� �
 � ��R�� %�C� g� ���� T� �� Ainfo_vw�� T� �� Currow� �� %�C� � .� b� L����/ REPLACE &_curfldnm WITH .f. IN &_curtblnm		
 � T�  � ��  �� T�  � �� �� � �� �
 �
 �
 � � T�  � ��  �� T�  � �-�� � U  THISFORM ADDMODE EDITMODE
 FLAGCANCEL _CURFLD THIS CONTROLSOURCE _CURTBL _CURREC SETVALUE PARENT	 VALIDEXPR _CURWVAL _CURVAL _CURMES _CURWMES CERRMSG SHOWMESSAGEBOX VUMESS VALUE	 _CURFLDNO CSOURCE	 _CURTBLNM	 _CURFLDNM AINFOOLDFLD AINFOOLDREC REFRESH LISTTBL SETFS?  %��  � � �  � ��8 � T�  � �a�� T�  � ���� � U  THISFORM ADDMODE EDITMODE SETFS SETVALUE  %��  � � � �  � � ��' � B�-�� �6 %�C�" This.Parent.Parent.Text1.BackColorb� N��} � T�  � ��  � � � � �� � %�� � � � � ��� %�� �	 a��� � B� � %�� �
 -��� � T� � ���� � T� � �a�� T� �CC�  � � ��R�� T� �� Ainfo_vw��
 F�� ��  T� �CCCO�CN�� CO� � 6�� %�C� � �
���� T� �C� � ���� T� �C� � ����
 F�� �� T� �� � �� %�� � ����	 #�� �� � %�C� � .� b� L����) REPLACE &_curfld WITH .f. IN &_curtbl
 � �
 F�� �� %�� � ��%�	 #�� �� � %�C� g� ���� T� �� Currow� �� %�C� � .� b� L����- REPLACE &_curfldnm WITH .t. IN &_curtblnm
 � T� � �� � .� �� T� � �� �� � ��  � � � � � T� � ��  ��" %�C�  � �� C�  � � �
	��V� T� �C�  � � ��� %�C� �
��R� This.Value = &_curwval
 � � %�C�  � � �
�� � T� �C�  � � ��� %�C� �
���� T� �C� ��� %�C� _curvalb� L���� %�� -���� \�� {TAB}�� � ��� T�  � �� �� � � � � U  THIS PARENT CURRENTCONTROL NAME	 BACKCOLOR TEXT1 THISFORM ADDMODE EDITMODE
 FLAGCANCEL SETFS SETVALUE
 LOCKSCREEN	 _CURFLDNO CSOURCE	 _CURTBLNM _AINFONEWREC AINFOOLDFLD _CURFLD _CURTBL _AINFOOLDREC AINFOOLDREC	 _CURFLDNM REFRESH LISTTBL VALUE DEFAEXPR _CURWVAL WHENEXPR _CURVAL	 LostFocus,     �� ValidK    �� SetFocus�    �� When    ��1 �1A A �A A 3 �!A A 11� �Aq� A qaQ� �� �1� �A A �� A � A � A A A A �1Qq��A A 1� A 3 �� A 3 q A a�A �!A A !A � �Q� 111� � A ��A A � � A 1q��A �A 1!Q�A A aQ� �� � A � A A A A 2                       �        �  S     B   r  �  F   G   �  i  M    )   #                       PROCEDURE Init
********************
&& NOTE A : When user clicks on this button, he should be able to call his required form instead of calling UeNrEntry. 
*For this, he should write in when condition of addl. info.
********************
***LPARA nTop, nLeft, nHeight, nWidth, cControl, cCaption, cLsttable, cLstcond, cSplchk, cWhnCond	&&vasant101010 - Note A 
LPARA nTop, nLeft, nHeight, nWidth, cControl, cCaption, cLsttable, cLstcond, cSplchk, cWhnCond,cquery	&& added by satish pal for bug-621 dt.02/12/2011

IF TYPE('ntop') # 'N'
	ntop = 0
Endif	
IF TYPE('nleft') # 'N'
	nleft = 0
Endif	
IF TYPE('nheight') # 'N'
	nheight = 0
Endif	
IF TYPE('nWidth') # 'N'
	nWidth = 0
Endif	
IF TYPE('cControl') # 'C'
	cControl = ''
Endif	
IF TYPE('cCaption') # 'C'
	cCaption = ''
Endif	
IF TYPE('cLsttable') # 'C'
	cLsttable = ''
Endif	
IF TYPE('cLstcond') # 'C'
	cLstcond = ''
Endif	


ENDPROC
PROCEDURE Click
if type('Thisform.NoVouRefresh')='L'
	thisform.NoVouRefresh = .t.
ENDIF

PUBLIC _cmdptaddmode,_cmdpteditmode

_cmdptaddmode = thisform.Addmode
_cmdpteditmode = thisform.Editmode
IF TYPE('thisform.cmdNarwhn') = 'L'
	 IF thisform.cmdNarwhn = .f.
		_cmdptaddmode = .f.
		_cmdpteditmode = .f.
	ENDIF 
endif

DO FORM ptsrch	

ENDPROC
     ����    �  �                        �2   %   #      �  3   1          �  U  / ��  � � � � � � � � �	 �
 �! %�CC� nheightb� N� C�
��] � T�  �� �� �  %�CC� nWidthb� N� C�
��� � T� �� �� � %�C� cControlb� C��� � T� ��  �� � %�C� cCaptionb� C��� � T� ��  �� � %�C�	 cLsttableb� C��� T� ��  �� � %�C� cLstcondb� C��C� T� ��  �� � %�C� cwhenb� C��m� T� ��  �� � %�C� cdefaultb� C���� T� ��  �� � %�C� cvalidb� C���� T�	 ��  �� � %�C� cerrorb� C���� T�
 ��  �� � T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �� �� T� �  ��  �� T� � �� �� T� � �� �� T� � �� �� T� � ��	 �� T� � ��
 �� T� �� This.� � ��# &_curobjname..Left          = 1
# &_curobjname..Top           = 0
c &_curobjname..Height        = IIF(TYPE('this.nheight')='N',this.nheight,EVALUATE(this.nheight))
] &_curobjname..Width     	= IIF(TYPE('this.nwidth')='N',this.nwidth,EVALUATE(this.nwidth))
# &_curobjname..FontSize      = 8
! &_curobjname..Caption  		= []
 U  NHEIGHT NWIDTH CCONTROL CCAPTION	 CLSTTABLE CLSTCOND CSPLCHK CWHEN CDEFAULT CVALID CERROR THIS
 CMDCONTROL
 CMDCAPTION LSTTABLE LSTCOND SPLCHK DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG _CUROBJNAME ACTIVEOBJNAME Init,     ��1 �� A � A �� A �� A �� A �� A �� A �� A �� A �� A "!!!�111�12                       P      )   �                       PROCEDURE When
IF This.Parent.Parent.CurrentControl # This.Parent.Name
	RETURN .f.
Endif	
this.Height			= IIF(TYPE('this.parent.nheight')='N',this.parent.nheight,EVALUATE(this.parent.nheight))
this.Width			= IIF(TYPE('this.parent.nwidth')='N',this.parent.nwidth,EVALUATE(this.parent.nwidth)) * 8
_defvalue 			= ALLTRIM(IIF(EMPTY(this.Parent.csource),this.Parent.csource,EVALUATE(this.Parent.csource)))
this.ucontrolsource = _defvalue
_defawhen=DODEFAULT()
IF !_defawhen
	RETURN .f.
Endif	
IF TYPE('This.Parent.Parent.Text1.BackColor')='N'
	This.BackColor=This.Parent.Parent.Text1.BackColor
Endif	
_defvalue = This.Ucontrolsource
IF !EMPTY(_defvalue)
	IF TYPE(_defvalue)='T'
		This.Value	= Ttod(&_defvalue)
	ELSE
		This.Value	= &_defvalue
	Endif		
ENDIF
IF Thisform.AddMode Or Thisform.EditMode
	If Thisform.FlagCancel=.t.
		Return
	ENDIF
	IF Thisform.SetFs		 = .f.
		Thisform.SetValue    = 3
	Endif	
	Thisform.LockScreen = .t.
	_curfldno = Right(ALLTRIM(this.Parent.csource),1)
	_curtblnm = 'Ainfo_vw'
	SELECT (_curtblnm)
	_ainfonewrec = IIF(BETWEEN(RECNO(),1,RECCOUNT()),RECNO(),0)
	IF !EMPTY(Thisform.AinfoOldFld)
		_curfld = JUSTEXT(Thisform.AinfoOldFld)
	  	_curtbl = JUSTSTEM(Thisform.AinfoOldFld)
	  	SELECT (_curtbl)
		_ainfooldrec = Thisform.AinfoOldRec
		IF _ainfooldrec > 0
			GO _ainfooldrec
		Endif	
		IF TYPE(_curtbl+'.'+_curfld)='L'
			REPLACE &_curfld WITH .f. IN &_curtbl
		ENDIF
	ENDIF
	SELECT (_curtblnm)
	IF _ainfonewrec > 0
		GO _ainfonewrec
	Endif
	IF VAL(_curfldno) > 0
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .t. IN &_curtblnm
		ENDIF
		Thisform.AinfoOldFld = _curtblnm+'.'+_curfldnm
		Thisform.AinfoOldRec = _ainfonewrec
	Endif
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	IF EMPTY(This.Value) AND !EMPTY(this.parent.Defaexpr)
		_curwval = EVALUATE(this.parent.Defaexpr)
		IF !EMPTY(_curwval)
			This.Value = _curwval
		Endif	
	ENDIF
	IF !EMPTY(this.parent.whenexpr)
		_curwval= EVAL(this.parent.whenexpr)
		IF !EMPTY(_curwval)
			_curval = EVAL(_curwval)
			IF TYPE('_curval') = 'L'
				IF _curval = .f.
					KEYBOARD '{TAB}'
				Endif	
			Else
				This.Value = _curval
			Endif	
		Endif	
	ENDIF
ENDIF

ENDPROC
PROCEDURE Valid
_defavalid=DODEFAULT()
IF !_defavalid
	NoDefault
	Retu This.Value
Endif	
If Thisform.Addmode Or Thisform.Editmode
	If Thisform.FlagCancel=.t. OR thisform.curonmouse = .t.
		Return
	ENDIF
	_curfld = JUSTEXT(This.UControlSource)
  	_curtbl = JUSTSTEM(This.UControlSource)
	SELECT (_curtbl)
	_currec = IIF(!EOF(),RECNO(),0)
	IF Thisform.SetValue < 2
		Thisform.SetValue = Thisform.SetValue + 1
		SELECT (_curtbl)
		Retu This.Value
	ENDIF
	Thisform.SetValue = Thisform.SetValue + 1
	IF !EMPTY(this.parent.validexpr)
		_curwval= EVAL(this.parent.validexpr)
		IF !EMPTY(_curwval)
			_curval = EVALUATE(_curwval)
			IF TYPE('_curval') = 'L'
				IF _curval = .f.
					_curmes = 'Invalid Date'
					_curwmes= this.parent.cerrmsg
					IF !EMPTY(_curwmes)
						_curwmes = EVALUATE(_curwmes)
						IF !EMPTY(_curwmes)
							_curwmes = ALLTRIM(EVALUATE(_curwmes))
							_curmes  = IIF(!EMPTY(_curwmes),_curwmes,_curmes)
						Endif	
					Endif	
					Thisform.ShowMessageBox(_curmes,0+32,vumess)
					SELECT (_curtbl)
					NoDefault
					If _currec > 0
						GoTo _currec
					ENDIF
					Retu This.Value
				Endif	
			Endif	
		Endif	
	ENDIF
	_curfldno = Right(ALLTRIM(this.Parent.csource),1)
	IF VAL(_curfldno) > 0
		_curtblnm = 'Ainfo_vw'
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .f. IN &_curtblnm		
		ENDIF
		Thisform.AinfoOldFld = ''
		Thisform.AinfoOldRec = 0
		_curfldnm = This.Parent.Parent.ControlSource
		REPLACE &_curfldnm WITH DTOC(This.Value) IN &_curtblnm
	ENDIF
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	Thisform.SetFs   = .f.
ENDIF

ENDPROC
PROCEDURE SetFocus
IF Thisform.Addmode OR Thisform.Editmode
	Thisform.SetFs		 = .t.
	Thisform.SetValue    = 1
Endif	

ENDPROC
PROCEDURE LostFocus
_defavalid=DODEFAULT()
IF !_defavalid
	NoDefault
	Retu This.Value
Endif	
IF (LASTKEY() = 9 OR LASTKEY() = 13)
	IF Thisform.Cur_ActCol = This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,1)
	Endif	
ENDIF
IF LASTKEY()= 15
	IF Thisform.Cur_ActCol # This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow-1,This.Parent.Parent.Parent.ColumnCount)
	Endif	
ENDIF

ENDPROC
     ����    �  �                        )�   %   �      T  <   �          �  U  �/ ��  � � � � � � � � �	 �
 � %�C� ntopb� N��T � T�  �� �� � %�C� nleftb� N��~ � T� �� �� � %�C� nheightb� N��� � T� �� �� � %�C� nWidthb� N��� � T� �� �� � %�C� cControlb� C��� T� ��  �� � %�C� cCaptionb� C��/� T� ��  �� � %�C�	 cLsttableb� C��]� T� ��  �� � %�C� cLstcondb� C���� T� ��  �� � T� �-�� T� � �� �� T� � ��  �� T� � �� �� T� � ���� T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �� �� T� � �� � ��- T� �
 �CC� cqueryb� C� C�
 �� �  6�� T� � ��  �� %�C� cWhnCondb� C���� T� � �C�	 ��� � U  NTOP NLEFT NHEIGHT NWIDTH CCONTROL CCAPTION	 CLSTTABLE CLSTCOND CSPLCHK CWHNCOND CQUERY THIS LEFT TOP HEIGHT FONTSIZE
 CMDCONTROL
 CMDCAPTION LSTTABLE LSTCOND SPLCHK CAPTION WHENEXPRJ) %�C� Thisform.NoVouRefreshb� L��7 � T�  � �a�� � T� ��  � �� T� ��  � ��& %�C� thisform.cmdNarwhnb� L��� � %��  � -��� � T� �-�� T� �-�� � � %�C� � �
��� � ��C� � ��� �C�i �	 ��C� ��]��C� ��]C� ��]�� �
 � � � � � � �  � � � � � � � � � � � U  THISFORM NOVOUREFRESH _CMDNARRADDMODE ADDMODE _CMDNARREDITMODE EDITMODE	 CMDNARWHN THIS WHENEXPR	 UENRENTRY
 CMDCONTROL NAME
 CMDCAPTION	 BACKCOLOR LSTTABLE LSTCOND SPLCHK CQUERY Init,     �� Click�    ��1 ��� A �� A �� A �� A �� A �� A �� A �� A � !!!!1��!A 6 �� A a!� � A A 5� � �A 1                       �     +   �  k  8    )   �                       ����    �  �                        ��   %         Y  #             �  U  P ��  � � � � � � �! %�CC� nheightb� N� C�
��M � T�  �� �� �  %�CC� nWidthb� N� C�
��~ � T� �� �� � %�C� csourceb� C��� � T� ��  �� � %�C� cwhenb� C��� � T� ��  �� � %�C� cdefaultb� C��� T� ��  �� � %�C� cvalidb� C��,� T� ��  �� � %�C� cerrorb� C��W� T� ��  �� � T� � �� �� T� �	 �� �� T� �
 �� �� T� � �� �� T� �  ��  �� T� � �� �� T� � �� �� T� �� This.� � ��# &_curobjname..Left          = 0
# &_curobjname..Top           = 0
# &_curobjname..FontSize      = 8
 U  NHEIGHT NWIDTH CSOURCE CWHEN CDEFAULT CVALID CERROR THIS DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG _CUROBJNAME ACTIVEOBJNAME Init,     ��1 �� A � A �� A �� A �� A �� A �� A �1112                       
      )   �                       �PROCEDURE GotFocus
*!*	IF Thisform.AddMode Or Thisform.EditMode
*!*		IF This.Readonly = .f.
*!*			*Thisform.GrdTest('Thisform.VouPage.Page3.GrdAccount','Thisform.VouPage.Page3.GrdAccount.Column3.Text1',1,[DR,CR],OBJTOCLIENT(this,1),OBJTOCLIENT(this,2),OBJTOCLIENT(this,3),OBJTOCLIENT(this,4))
*!*		ENDIF
*!*	Endif		
ENDPROC
PROCEDURE ErrorMessage
*!*	IF Thisform.AddMode Or Thisform.EditMode
*!*		*RETURN Lother_vw.Val_Err
*!*	Endif

ENDPROC
PROCEDURE When
IF This.Parent.Parent.CurrentControl # This.Parent.Name
	RETURN .f.
Endif	
this.Height			= IIF(TYPE('this.parent.nheight')='N',this.parent.nheight,EVALUATE(this.parent.nheight))
this.Width			= IIF(TYPE('this.parent.nwidth')='N',this.parent.nwidth,EVALUATE(this.parent.nwidth)) * 8
_defvalue 			= ALLTRIM(IIF(EMPTY(this.Parent.csource),this.Parent.csource,EVALUATE(this.Parent.csource)))
this.controlsource = _defvalue
_defawhen=DODEFAULT()
IF !_defawhen
	RETURN .f.
Endif	
IF TYPE('This.Parent.Parent.Text1.BackColor')='N'
	This.BackColor=This.Parent.Parent.Text1.BackColor
Endif	
_defvalue = This.controlsource
IF !EMPTY(_defvalue)
	This.Value	= &_defvalue
ENDIF
IF Thisform.AddMode Or Thisform.EditMode
	If Thisform.FlagCancel=.t.
		Return
	ENDIF
	IF Thisform.SetFs		 = .f.
		Thisform.SetValue    = 3
	Endif	
	Thisform.LockScreen = .t.
	_curfldno = Right(ALLTRIM(this.Parent.csource),1)
	_curtblnm = 'Ainfo_vw'
	SELECT (_curtblnm)
	_ainfonewrec = IIF(BETWEEN(RECNO(),1,RECCOUNT()),RECNO(),0)
	IF !EMPTY(Thisform.AinfoOldFld)
		_curfld = JUSTEXT(Thisform.AinfoOldFld)
	  	_curtbl = JUSTSTEM(Thisform.AinfoOldFld)
	  	SELECT (_curtbl)
		_ainfooldrec = Thisform.AinfoOldRec
		IF _ainfooldrec > 0
			GO _ainfooldrec
		Endif	
		IF TYPE(_curtbl+'.'+_curfld)='L'
			REPLACE &_curfld WITH .f. IN &_curtbl
		ENDIF
	ENDIF
	SELECT (_curtblnm)
	IF _ainfonewrec > 0
		GO _ainfonewrec
	Endif
	IF VAL(_curfldno) > 0
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .t. IN &_curtblnm
		ENDIF
		Thisform.AinfoOldFld = _curtblnm+'.'+_curfldnm
		Thisform.AinfoOldRec = _ainfonewrec
	Endif
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	IF EMPTY(This.Value) AND !EMPTY(this.parent.Defaexpr)
		_curwval = EVALUATE(this.parent.Defaexpr)
		IF !EMPTY(_curwval)
			This.Value = _curwval
		Endif	
	ENDIF
	IF !EMPTY(this.parent.whenexpr)
		_curwval= EVAL(this.parent.whenexpr)
		IF !EMPTY(_curwval)
			_curval = EVAL(_curwval)
			IF TYPE('_curval') = 'L'
				IF _curval = .f.
					KEYBOARD '{TAB}'
				Endif	
			Else
				This.Value = _curval
			Endif	
		Endif	
	ENDIF
ENDIF

ENDPROC
PROCEDURE LostFocus
IF (LASTKEY() = 9 OR LASTKEY() = 13)
	IF Thisform.Cur_ActCol = This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,1)
	Endif	
ENDIF
IF LASTKEY()= 15
	IF Thisform.Cur_ActCol # This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow-1,This.Parent.Parent.Parent.ColumnCount)
	Endif	
ENDIF

*!*		IF This.Parent.Parent.ColumnOrder = This.Parent.Parent.Parent.ColumnCount 
*!*			IF INLIST(LASTKEY(),9,13)
*!*				_curtblnm = 'Ainfo_vw'
*!*				SELECT (_curtblnm)
*!*				IF !EOF()
*!*					SKIP
*!*					WAIT WINDOW Thisform.Cur_ActRow+1	&&Test_x
*!*					This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,7)
*!*				Endif	
*!*			Endif	
*!*		Endif

*!*	nKeyCode = LASTKEY()
*!*	IF (Thisform.AddMode Or Thisform.EditMode) AND This.Parent.Parent.CurrentControl = This.Parent.Name
*!*		SELECT Ainfo_vw
*!*		_currec = IIF(!EOF(),RECNO(),0)
*!*	*!*		IF !EMPTY(This.SetValue)
*!*	*!*			This.SetValue = ''
*!*	*!*			SELECT Ainfo_vw
*!*	*!*			NoDefault
*!*	*!*			If _currec > 0
*!*	*!*				GoTo _currec
*!*	*!*			EndIf
*!*	*!*			Retu
*!*	*!*		Endif
*!*		mfld_nm = Lother_vw.Fld_nm
*!*		mtbl_nm = Thisform.MainAlias
*!*		REPLACE &mfld_nm WITH This.Value IN &mtbl_nm
*!*		IF !EMPTY(Lother_vw.Val_Con) AND !This.Readonly
*!*			_curval = EVAL(Lother_vw.Val_Con)
*!*			IF TYPE('_curval') = 'L'
*!*				IF _curval = .f.
*!*					_curmes = ALLTRIM(EVAL(Lother_vw.Val_Err))
*!*					Thisform.ShowMessageBox(_curmes,0+32,vumess)
*!*					SELECT Ainfo_vw
*!*					NoDefault
*!*					If _currec > 0
*!*						GoTo _currec
*!*					EndIf
*!*					Retu
*!*				Endif	
*!*			Endif	
*!*		ENDIF
*!*		Thisform.cmdsdc.visible= .f.
*!*		Thisform.lstsdc.visible= .f.	
*!*	ENDIF
*!*	IF nKeyCode = 24
*!*		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,Thisform.Cur_ActCol)
*!*		KEYBOARD '{LEFTARROW}'
*!*	ENDIF
*!*	IF nKeyCode = 5
*!*		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow-1,Thisform.Cur_ActCol)
*!*		KEYBOARD '{LEFTARROW}'
*!*	endif		
*!*	IF (LASTKEY() = 9 OR LASTKEY() = 13)
*!*		IF Thisform.Cur_ActCol = This.Parent.Parent.Parent.ColumnCount
*!*			This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,1)
*!*		Endif	
*!*	ENDIF

ENDPROC
PROCEDURE SetFocus
IF Thisform.Addmode OR Thisform.Editmode
	Thisform.SetFs		 = .t.
	Thisform.SetValue    = 1
Endif	

ENDPROC
PROCEDURE Valid
_defavalid=DODEFAULT()
IF !_defavalid
	NoDefault
	Retu This.Value
Endif	
If Thisform.Addmode Or Thisform.Editmode
	If Thisform.FlagCancel=.t. OR thisform.curonmouse = .t.
		Return
	ENDIF
	_curfld = JUSTEXT(This.ControlSource)
  	_curtbl = JUSTSTEM(This.ControlSource)
	SELECT (_curtbl)
	_currec = IIF(!EOF(),RECNO(),0)
	IF Thisform.SetValue < 2
		Thisform.SetValue = Thisform.SetValue + 1
		SELECT (_curtbl)
		Retu This.Value
	ENDIF
	Thisform.SetValue = Thisform.SetValue + 1
	IF !EMPTY(this.parent.validexpr)
		_curwval= EVAL(this.parent.validexpr)
		IF !EMPTY(_curwval)
			_curval = EVALUATE(_curwval)
			IF TYPE('_curval') = 'L'
				IF _curval = .f.
					_curmes = 'Invalid Value'
					_curwmes= this.parent.cerrmsg
					IF !EMPTY(_curwmes)
						_curwmes = EVALUATE(_curwmes)
						IF !EMPTY(_curwmes)
							_curwmes = ALLTRIM(EVALUATE(_curwmes))
							_curmes  = IIF(!EMPTY(_curwmes),_curwmes,_curmes)
						Endif	
					Endif	
					Thisform.ShowMessageBox(_curmes,0+32,vumess)
					SELECT (_curtbl)
					NoDefault
					If _currec > 0
						GoTo _currec
					ENDIF
					Retu This.Value
				Endif	
			Endif	
		Endif	
	ENDIF
	_curfldno = Right(ALLTRIM(this.Parent.csource),1)
	IF VAL(_curfldno) > 0
		_curtblnm = 'Ainfo_vw'
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .f. IN &_curtblnm		
		ENDIF
		Thisform.AinfoOldFld = ''
		Thisform.AinfoOldRec = 0
		_curfldnm = This.Parent.Parent.ControlSource
		REPLACE &_curfldnm WITH This.Value IN &_curtblnm
	Endif
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	Thisform.SetFs   = .f.
ENDIF

ENDPROC
     tPROCEDURE LostFocus
IF (LASTKEY() = 9 OR LASTKEY() = 13)
	IF Thisform.Cur_ActCol = This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,1)
	Endif	
ENDIF
IF LASTKEY()= 15
	IF Thisform.Cur_ActCol # This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow-1,This.Parent.Parent.Parent.ColumnCount)
	Endif	
ENDIF

ENDPROC
PROCEDURE Valid
If Thisform.Addmode Or Thisform.Editmode
	If Thisform.FlagCancel=.t.
		Return
	ENDIF
	_curfld = JUSTEXT(This.ControlSource)
  	_curtbl = JUSTSTEM(This.ControlSource)
	SELECT (_curtbl)
	_currec = IIF(!EOF(),RECNO(),0)
	IF Thisform.SetValue < 2
		Thisform.SetValue = Thisform.SetValue + 1
		SELECT (_curtbl)
	ENDIF
	Thisform.SetValue = Thisform.SetValue + 1
	IF !EMPTY(this.parent.validexpr)
		_curwval= EVAL(this.parent.validexpr)
		IF !EMPTY(_curwval)
			_curval = EVALUATE(_curwval)
			IF TYPE('_curval') = 'L'
				IF _curval = .f.
					_curmes = 'Invalid Input'
					_curwmes= this.parent.cerrmsg
					IF !EMPTY(_curwmes)
						_curwmes = EVALUATE(_curwmes)
						IF !EMPTY(_curwmes)
							_curwmes = ALLTRIM(EVALUATE(_curwmes))
							_curmes  = IIF(!EMPTY(_curwmes),_curwmes,_curmes)
						Endif	
					Endif	
					Thisform.ShowMessageBox(_curmes,0+32,vumess)
					SELECT (_curtbl)
					NoDefault
					If _currec > 0
						GoTo _currec
					ENDIF
					Retu This.Value
				Endif	
			Endif	
		Endif	
	ENDIF
	_curfldno = Right(ALLTRIM(this.Parent.csource),1)
	IF VAL(_curfldno) > 0
		_curtblnm = 'Ainfo_vw'
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .f. IN &_curtblnm		
		ENDIF
		Thisform.AinfoOldFld = ''
		Thisform.AinfoOldRec = 0
	ENDIF
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	Thisform.SetFs   = .f.
ENDIF

ENDPROC
PROCEDURE SetFocus
IF Thisform.Addmode OR Thisform.Editmode
	Thisform.SetFs		 = .t.
	Thisform.SetValue    = 1
Endif	

ENDPROC
PROCEDURE When
IF This.Parent.Parent.CurrentControl # This.Parent.Name
	RETURN .f.
Endif	
IF TYPE('This.Parent.Parent.Text1.BackColor')='N'
	This.BackColor=This.Parent.Parent.Text1.BackColor
Endif	
*!*	_defvalue 			= ALLTRIM(IIF(EMPTY(this.Parent.csource),this.Parent.csource,EVALUATE(this.Parent.csource)))
*!*	this.controlsource  = _defvalue
*!*	IF !EMPTY(_defvalue)
*!*		This.Value	= &_defvalue
*!*	ENDIF
IF Thisform.AddMode Or Thisform.EditMode
	If Thisform.FlagCancel=.t.
		Return
	ENDIF
	IF Thisform.SetFs		 = .f.
		Thisform.SetValue    = 3
	Endif	
	Thisform.LockScreen = .t.
	_curfldno = Right(ALLTRIM(this.Parent.csource),1)
	_curtblnm = 'Ainfo_vw'
	SELECT (_curtblnm)
	_ainfonewrec = IIF(BETWEEN(RECNO(),1,RECCOUNT()),RECNO(),0)
	IF !EMPTY(Thisform.AinfoOldFld)
		_curfld = JUSTEXT(Thisform.AinfoOldFld)
	  	_curtbl = JUSTSTEM(Thisform.AinfoOldFld)
	  	SELECT (_curtbl)
		_ainfooldrec = Thisform.AinfoOldRec
		IF _ainfooldrec > 0
			GO _ainfooldrec
		Endif	
		IF TYPE(_curtbl+'.'+_curfld)='L'
			REPLACE &_curfld WITH .f. IN &_curtbl
		ENDIF
	ENDIF
	SELECT (_curtblnm)
	IF _ainfonewrec > 0
		GO _ainfonewrec
	Endif
	IF VAL(_curfldno) > 0
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .t. IN &_curtblnm
		ENDIF
		Thisform.AinfoOldFld = _curtblnm+'.'+_curfldnm
		Thisform.AinfoOldRec = _ainfonewrec
	ENDIF
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	IF EMPTY(This.Value) AND !EMPTY(this.parent.Defaexpr)
		_curwval = EVALUATE(this.parent.Defaexpr)
		IF !EMPTY(_curwval)
			This.Value = &_curwval
		Endif	
	ENDIF
	IF !EMPTY(this.parent.whenexpr)
		_curwval= EVAL(this.parent.whenexpr)
		IF !EMPTY(_curwval)
			_curval = EVAL(_curwval)
			IF TYPE('_curval') = 'L'
				IF _curval = .f.
					KEYBOARD '{TAB}'
				Endif	
			Else
				This.Value = _curval
			Endif	
		Endif	
	ENDIF
ENDIF

ENDPROC
     ����    �  �                        ��   %   �      O  R   �          �  U  �  %��  � � �  � ��� � T�  � ��  �� %�C� � �
��� � T� �C� � ��� %�C� _curvalb� L��� � %�� -��� �	 B�� �� � �� � T� � �� �� � � � U  THISFORM ADDMODE EDITMODE LISTTBL THIS WHENEXPR _CURVAL VALUE' %��  � � �  � �	 � � -	��x� T� �C� � ���� T� �C� � ����
 F�� �� T� �CC+
� CO� � 6�� %�C� �	 �
��d� T�
 �C� �	 ��� %�C� _curvalb� L��`� %��
 -��\� %�C� � ���� � T� �� Invalid Input�� �� T� �CC� � Λ�� � ��C � �  � �  � ��
 F�� �� �� %�� � ��L�	 #�� �� � B�� � �� � � � T�  � ��  �� � U  THISFORM ADDMODE EDITMODE THIS READONLY _CURFLD CONTROLSOURCE _CURTBL _CURREC	 VALIDEXPR _CURVAL CERRMSG _CURMES SHOWMESSAGEBOX VUMESS VALUE LISTTBLx# ��  � � � � � � � � %�C� ntopb� N��H � T�  �� �� � %�C� nleftb� N��r � T� �� �� � %�C� nheightb� N��� � T� �� �� � %�C� csourceb� C��� � T� ��  �� � %�C� cwhenb� C��� � T� ��  �� � %�C� cdefaultb� C��!� T� ��  �� � %�C� cvalidb� C��L� T� ��  �� � %�C� cerrorb� C��w� T� ��  �� � T� �	 �� �� T� �
 ��  �� T� � �� �� T� � �� �� T� � �� �� T� � ���� T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �C� ��� %�C� � � f� COLUMN��a� T� � �C�  � � � �  ��� � T� � ��  �� U  NTOP NLEFT NHEIGHT CSOURCE CWHEN CDEFAULT CVALID CERROR THIS LEFT TOP HEIGHT WIDTH CONTROLSOURCE FONTSIZE DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG PARENT CLASS CAPTION When,     �� Valid#    �� InitK    ��1 �6!�� � A � A A A 3 q11� �1!�� !�� AA �� A � A � A A A A 3 1�� A �� A �� A �� A �� A �� A �� A �� A !!!!��A 3                       �        �  �     *   �  D  3    )   �                       �PROCEDURE Valid
If Thisform.Addmode Or Thisform.Editmode
	If Thisform.FlagCancel=.t.
		Return
	ENDIF
	cControl = This.Parent.Cmdcontrol
	cControl = EVALUATE(cControl)
	_curfld = JUSTEXT(cControl)
  	_curtbl = JUSTSTEM(cControl)
	SELECT (_curtbl)
	_currec = IIF(!EOF(),RECNO(),0)
	IF Thisform.SetValue < 2
		Thisform.SetValue = Thisform.SetValue + 1
		SELECT (_curtbl)
	ENDIF
	Thisform.SetValue = Thisform.SetValue + 1
	_curfldno = Right(ALLTRIM(this.Parent.Cmdcontrol),1)
	IF VAL(_curfldno) > 0
		_curtblnm = 'Ainfo_vw'
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .f. IN &_curtblnm		
		ENDIF
		Thisform.AinfoOldFld = ''
		Thisform.AinfoOldRec = 0
	Endif
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	Thisform.SetFs   = .f.
ENDIF

ENDPROC
PROCEDURE When
IF This.Parent.Parent.CurrentControl # This.Parent.Name
	RETURN .f.
Endif	
IF Thisform.AddMode Or Thisform.EditMode
	If Thisform.FlagCancel=.t.
		Return
	ENDIF
	IF Thisform.SetFs		 = .f.
		Thisform.SetValue    = 3
	Endif	
	Thisform.LockScreen = .t.
	_curfldno = Right(ALLTRIM(this.Parent.Cmdcontrol),1)
	_curtblnm = 'Ainfo_vw'
	SELECT (_curtblnm)
	_ainfonewrec = IIF(BETWEEN(RECNO(),1,RECCOUNT()),RECNO(),0)
	IF !EMPTY(Thisform.AinfoOldFld)
		_curfld = JUSTEXT(Thisform.AinfoOldFld)
	  	_curtbl = JUSTSTEM(Thisform.AinfoOldFld)
	  	SELECT (_curtbl)
		_ainfooldrec = Thisform.AinfoOldRec
		IF _ainfooldrec > 0
			GO _ainfooldrec
		Endif	
		IF TYPE(_curtbl+'.'+_curfld)='L'
			REPLACE &_curfld WITH .f. IN &_curtbl
		ENDIF
	ENDIF
	SELECT (_curtblnm)
	IF _ainfonewrec > 0
		GO _ainfonewrec
	Endif
	IF VAL(_curfldno) > 0
		_curfldnm = 'Currow'+_curfldno
		IF TYPE(_curtblnm+'.'+_curfldnm)='L'
			REPLACE &_curfldnm WITH .t. IN &_curtblnm
		ENDIF
		Thisform.AinfoOldFld = _curtblnm+'.'+_curfldnm
		Thisform.AinfoOldRec = _ainfonewrec
	Endif
	This.Parent.Parent.Parent.Refresh
	Thisform.ListTbl = []
	cControl = This.Parent.Cmdcontrol
	cControl = EVALUATE(cControl)
	cControlv= ''
	cControlf= JUSTEXT(Thisform.AinfoOldFld)
  	cControlt= JUSTSTEM(Thisform.AinfoOldFld)
	IF TYPE(cControl)='L'
		cControlv= &cControl
	Endif
	IF EMPTY(cControlv) AND !EMPTY(this.parent.Defaexpr)
		_curwval = EVALUATE(this.parent.Defaexpr)
		IF !EMPTY(_curwval)
			REPLACE &cControlf WITH &_curwval IN cControlt
		Endif	
	ENDIF
*!*		IF !EMPTY(this.parent.whenexpr)
*!*			_curwval= EVAL(this.parent.whenexpr)
*!*			IF !EMPTY(_curwval)
*!*				_curval = EVAL(_curwval)
*!*				IF TYPE('_curval') = 'L'
*!*					IF _curval = .f.
*!*						KEYBOARD '{TAB}'
*!*					Endif	
*!*				ELSE
*!*					REPLACE &cControlf WITH _curval IN cControlt
*!*				Endif	
*!*			Endif	
*!*		ENDIF
ENDIF

ENDPROC
PROCEDURE Click
Thisform.NoVouRefresh = .t.
cControl = This.Parent.Cmdcontrol
cControl = EVALUATE(cControl)
cCaption = This.Parent.Cmdcaption
cCaption = EVALUATE(cCaption)
Do UeNrEntry With OBJTOCLIENT(This,2),OBJTOCLIENT(This,1)+OBJTOCLIENT(This,4),cControl,;
	Thisform.Addmode,Thisform.Editmode,This.Name,cCaption,Thisform.BackColor,;
	This.Parent.Lsttable,This.Parent.Lstcond,This.Parent.Splchk

ENDPROC
PROCEDURE SetFocus
IF Thisform.Addmode OR Thisform.Editmode
	Thisform.SetFs		 = .t.
	Thisform.SetValue    = 1
Endif	

ENDPROC
PROCEDURE LostFocus
IF LASTKEY() = 9
	IF Thisform.Cur_ActCol = This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow+1,1)
	Endif	
ENDIF
IF LASTKEY()= 15
	IF Thisform.Cur_ActCol # This.Parent.Parent.Parent.ColumnCount
		This.Parent.Parent.Parent.ActivateCell(Thisform.Cur_ActRow-1,This.Parent.Parent.Parent.ColumnCount)
	Endif	
ENDIF

ENDPROC
     	����    u	  u	                        ~   %   c      ,	  ]   r          �  U  �) %�C� Thisform.NoVouRefreshb� L��7 � T�  � �a�� � T� �-��& %�C� thisform.mainaliasb� C��� � %�C�  � f� MAIN_VW��� � T� �a�� � �( %�C� � � f� COLUMN� � a	���� ���  ���� �� � F� � T� �CC+
� CO� � 6��- >� �	 ��C� �	 ��\� TC� �	 �\�� T� �a�� %��  � ����! %�C� UDAppVouItFire.app0��p� T� �C� �� � %�� ����" %�C� UDTrigVouItFire.fxp0���� T� �C� �� � � ���" %�C� UeTrigVouItFire.Fxp0���� T� �C� �� � � T� �� �� T� ��� �� H�!�}� �� � 0����� T� �C� EXE�� �c Select * From lother  where e_code = ?sql_var  and att_file = .f. and ingrid = .f.  order by serial� lother� thisform.nHandle�� -�� � �� 2�}�� T� �C� EXE�� �_ Select * From lother  where e_code = ?sql_var  and att_file = 0 and ingrid = 0 order by serial � lother� thisform.nHandle�� -�� � �� �  %�� � � C� lother�	���� F� � T� �CN�� �% T� �C� thisform.nHandle�� � �� %�� � ���� B�-�� � %�� � ��;�1 � ��� item_vw��C� item_vwO���� ���� �� �}� %�� ��y�) ��C� No Additional Info. �0 � �� �� � � %��  � ���$ %�C� UDAppVouXtraClose.app0���� T� �a�� � � T� �-�� �% %�C� UDTrigVouXtraClose.fxp0��� T� �a�� �  � T� �-�� � �j�% %�C� UeTrigVouXtraClose.Fxp0��f� T� �a�� �! � T� �-�� � � %�C� lother����� Q� � � F� � %�� � ����	 #�� �� � <� � �� �( %�C� � � f� COLUMN� � -	���� %��  � � V1���� F�" �% T�# �C� Item_Variant_Master_vwO��5 � ��� Item_Variant_Master_vw��# �  � �  � � %�C�# �CN����	 #��# �� � � � U$  THISFORM NOVOUREFRESH VOUCALL	 MAINALIAS THIS PARENT CLASS _XTRAREC ITEM_VW SR_SR IN LL UDNEWTRIGENBL UDAPPVOUITFIRE UDTRIGVOUITFIRE UETRIGVOUITFIRE SQL_REC SQL_VAR PCVTYPE MVU_BACKEND SQL_CON	 SQLCONOBJ DATACONN	 CO_DTBASE DATASESSIONID LOTHER SQLCONNCLOSE UEEXTRA SHOWMESSAGEBOX VUMESS _ITEMGRDCLASS UDAPPVOUXTRACLOSE UDTRIGVOUXTRACLOSE UETRIGVOUXTRACLOSE ITEM_VARIANT_MASTER_VW VRECNO Click,     ��1 �� A � a�� A A �� q q ��� � A � !� A A � "� A A � � � !2
� �	A q � A Rq A � � �A A A� q � A Q� q � A � Q� q � A A S� A q � A q A A �aq QQA� A A A 3                       �      )   u	                       OPROCEDURE When
IF Thisform.AddMode Or Thisform.EditMode
	Thisform.ListTbl = []
*!*		COMMENT BY SATISH PAL DATED.21/10/2011 FOR TKT-9641
*!*		IF EMPTY(This.Value) AND !EMPTY(this.Defaexpr)
*!*			This.Value = this.Defaexpr
*!*		ENDIF
*!*	COMMENT END BY SATISH
	IF !EMPTY(THIS.whenexpr)
		_curval = EVAL(THIS.whenexpr)
		IF TYPE('_curval') = 'L'
			IF _curval = .f.
				RETURN _curval
			Endif	
		Else
			This.Value = _curval
		Endif	
	ENDIF
ENDIF

ENDPROC
PROCEDURE Valid
If (Thisform.Addmode Or Thisform.Editmode) AND This.ReadOnly=.f.
	_curfld = JUSTEXT(This.ControlSource)
  	_curtbl = JUSTSTEM(This.ControlSource)
	SELECT (_curtbl)
	_currec = IIF(!EOF(),RECNO(),0)
	IF !EMPTY(this.validexpr)
		_curval = EVAL(this.validexpr)
		IF TYPE('_curval') = 'L'
			IF _curval = .f.
				IF EMPTY(this.cerrmsg)
					_curmes = 'Invalid Input'
				Else	
					_curmes = ALLTRIM(EVAL(this.cerrmsg))
				Endif	
				Thisform.ShowMessageBox(_curmes,0+32,vumess)
				SELECT (_curtbl)
				NoDefault
				If _currec > 0
					GoTo _currec
				ENDIF
				Retu This.Value
			Endif	
		Endif	
	ENDIF
	Thisform.ListTbl = []		
ENDIF

ENDPROC
PROCEDURE Init
LPARA nTop, nLeft, nHeight, cSource, cWhen, cDefault, cValid, cError
IF TYPE('ntop') # 'N'
	ntop = 0
Endif	
IF TYPE('nleft') # 'N'
	nleft = 0
Endif	
IF TYPE('nheight') # 'N'
	nheight = 0
Endif	
IF TYPE('csource') # 'C'
	csource = ''
Endif	
IF TYPE('cwhen') # 'C'
	cwhen = ''
Endif	
IF TYPE('cdefault') # 'C'
	cdefault = ''
Endif	
IF TYPE('cvalid') # 'C'
	cvalid = ''
Endif	
IF TYPE('cerror') # 'C'
	cerror = ''
Endif	
This.Left          = nLeft   
This.Top           = nTop
This.Height        = nHeight
This.Width     	   = nHeight
This.ControlSource = cSource
This.FontSize      = 8
this.Defaexpr      = ALLTRIM(cDefault)
this.whenexpr      = ALLTRIM(cWhen)
this.validexpr     = ALLTRIM(cValid)
this.cErrMsg       = ALLTRIM(cError)
If upper(this.parent.class) = 'COLUMN'
	this.Caption   = PADR('',This.Parent.Width,' ')
ENDIF
&&Test_x
	this.Caption = ''
	*this.Width = this.Parent.width
&&Test_x
ENDPROC
     vPROCEDURE Init
********************
&& NOTE A : When user clicks on this button, he should be able to call his required form instead of calling UeNrEntry. 
*For this, he should write in when condition of addl. info.
********************
***LPARA nTop, nLeft, nHeight, nWidth, cControl, cCaption, cLsttable, cLstcond, cSplchk, cWhnCond	&&vasant101010 - Note A 
LPARA nTop, nLeft, nHeight, nWidth, cControl, cCaption, cLsttable, cLstcond, cSplchk, cWhnCond,cquery	&& added by satish pal for bug-621 dt.02/12/2011

IF TYPE('ntop') # 'N'
	ntop = 0
Endif	
IF TYPE('nleft') # 'N'
	nleft = 0
Endif	
IF TYPE('nheight') # 'N'
	nheight = 0
Endif	
IF TYPE('nWidth') # 'N'
	nWidth = 0
Endif	
IF TYPE('cControl') # 'C'
	cControl = ''
Endif	
IF TYPE('cCaption') # 'C'
	cCaption = ''
Endif	
IF TYPE('cLsttable') # 'C'
	cLsttable = ''
Endif	
IF TYPE('cLstcond') # 'C'
	cLstcond = ''
Endif	
cSplchk			   = .f.		&&If spell check required for narration,then comment this line.
This.Left          = nLeft   
This.Top           = nTop
This.Height        = nHeight
*!*	This.Width     	   = nWidth * 8        && Commented by Sachin.S on 08/09/09  --> Due to additional Info. selected headerwise
This.FontSize      = 8
this.Cmdcontrol    = ALLTRIM(cControl)
this.Cmdcaption    = ALLTRIM(cCaption)
this.Lsttable      = ALLTRIM(cLsttable)
this.Lstcond       = ALLTRIM(cLstcond)
this.Splchk        = cSplchk
this.Caption	   = this.Cmdcaption
&&vasant101010 - Note A
This.cQuery		   = Iif(TYPE('cquery')='C',Alltrim(cquery),'')		&& Added By satish pal for bug -621
this.whenexpr = ''
IF TYPE('cWhnCond') = 'C'
	this.whenexpr = ALLTRIM(cWhnCond)
Endif	
&&vasant101010 - Note A



ENDPROC
PROCEDURE Click
if type('Thisform.NoVouRefresh')='L'
	thisform.NoVouRefresh = .t.
ENDIF
&&Rup 04/02/10-->
_cmdnarraddmode = thisform.Addmode
_cmdnarreditmode = thisform.Editmode
IF TYPE('thisform.cmdNarwhn') = 'L'
	 IF thisform.cmdNarwhn = .f.
		_cmdnarraddmode = .f.
		_cmdnarreditmode = .f.
	ENDIF 
endif
*!*	do UeNrEntry with objtoclient(this,2),objtoclient(this,1)+objtoclient(this,4),this.Cmdcontrol,;
*!*		thisform.Addmode,thisform.Editmode,this.name,this.Cmdcaption,thisform.backcolor,;
*!*		this.Lsttable,this.Lstcond,this.Splchk
&&vasant101010 - Note A
IF !EMPTY(this.whenexpr)
	=EVAL(THIS.whenexpr)
ELSE
&&vasant101010 - Note A
*!*		do UeNrEntry with objtoclient(this,2),objtoclient(this,1)+objtoclient(this,4),this.Cmdcontrol,;
*!*			_cmdnarraddmode,_cmdnarreditmode,this.name,this.Cmdcaption,thisform.backcolor,;
*!*			this.Lsttable,this.Lstcond,this.Splchk
**added by satish pal for bug-621 dt.02/12/2011
	do UeNrEntry with objtoclient(this,2),objtoclient(this,1)+objtoclient(this,4),this.Cmdcontrol,;
		_cmdnarraddmode,_cmdnarreditmode,this.name,this.Cmdcaption,thisform.backcolor,;
		this.Lsttable,this.Lstcond,this.Splchk,This.cQuery	
Endif	&&vasant101010 - Note A	
ENDPROC
     �PROCEDURE Click
if type('Thisform.NoVouRefresh')='L'
	thisform.NoVouRefresh = .t.
endif
&&TKT-6393 Item Variant Master 19/02/2011--->
Voucall=.F.
IF TYPE("thisform.mainalias")="C" 
	IF UPPER(thisform.mainalias) = 'MAIN_VW'
		Voucall=.T.
	ENDIF 
ENDIF
if upper(this.parent.class) = 'COLUMN' AND Voucall=.T. 
*if upper(this.parent.class) = 'COLUMN'
&&<---TKT-6393 Item Variant Master 19/02/2011

	with thisform
		local _xtrarec
		select Item_vw
		_xtrarec  = iif(!eof(),recno(),0)
		replace Sr_Sr with subs(Item_vw.Sr_Sr,1,3)+'T'+subs(Item_vw.Sr_Sr,5) in Item_vw

		ll = .t.
		&&Changes done for TKT-6089 by Vasant on 05/02/2010
		If Thisform.UdNewTrigEnbl
			If File('UDAppVouItFire.app')
				ll=UDAppVouItFire()
			Endif
			If ll
				If File('UDTrigVouItFire.fxp')
					ll=UDTrigVouItFire()
				Endif
			Endif
		Else
		&&Changes done for TKT-6089 by Vasant on 05/02/2010
			if file('UeTrigVouItFire.Fxp')
				ll = UeTrigVouItFire()
			ENDIF
		Endif			&&Changes done for TKT-6089 by Vasant on 05/02/2010
**!**			if ([vutex] $ vchkprod and inlist(.PcvType,"ST","PT","SS","IR","SR"))
**!**				.lockscreen=.t.
**!**				do TradDetail with [item]
**!**				.lockscreen=.f.
**!**			endif
		sql_rec  = 0
		sql_var = .PcvType
		do case
		case mvu_backend = "0"
			sql_con = .SqlConObj.DataConn([EXE],.Co_dtbase,[Select * From lother  where e_code = ?sql_var ;
					and att_file = .f. and ingrid = .f.  order by serial],[lother],"thisform.nHandle",.datasessionid,.f.)
		otherwise
			sql_con = .SqlConObj.DataConn([EXE],.Co_dtbase,[Select * From lother  where e_code = ?sql_var ;
					and att_file = 0 and ingrid = 0 order by serial ],[lother],"thisform.nHandle",.datasessionid,.f.)
		endcase
		if sql_con > 0 and used('lother')
			select lother
			sql_rec = reccount()
		endif

		sql_con= .SqlConObj.sqlconnclose("thisform.nHandle")		&& Added By Sachin N. S. on 27/05/2010 for TKT-1473
		If sql_con<=0
			Return .F.
		Endif

		if sql_rec > 0
			do UeExtra with [item_vw],recno('item_vw'),.PcvType,.datasessionid
			*.GetChild=.f.
			*.refresh
		else
			if ll
				.ShowMessageBox("No Additional Info. ",48,vumess)
			endif
		endif

		** Added By Shrikant S. on 12/02/2011 for TKT-4580	*** 	Start
		If Thisform.UdNewTrigEnbl
			If File('UDAppVouXtraClose.app')
				_itemgrdclass = .T.
				DO UDAppVouXtraClose
				_itemgrdclass = .F.
			Endif
			If File('UDTrigVouXtraClose.fxp')
				_itemgrdclass = .T.
				DO UDTrigVouXtraClose
				_itemgrdclass = .F.
			Endif
		Else
			If File('UeTrigVouXtraClose.Fxp')
				_itemgrdclass = .T.
				Do UeTrigVouXtraClose
				_itemgrdclass = .F.
			Endif
		Endif
		** Added By Shrikant S. on 12/02/2011 for TKT-4580	*** 	End		

		if used('lother')
			use in lother
		endif
		select Item_vw
		if _xtrarec > 0
			go _xtrarec
		endif
		release _xtrarec
	endwith
endif

&&TKT-6393 Item Variant Master 19/02/2011--->
if upper(this.parent.class) = 'COLUMN' AND Voucall=.F. 
	IF thisform.pcvtype="V1" 
		SELECT Item_Variant_Master_vw
		vrecno=recno('Item_Variant_Master_vw')
		do UeExtra with [Item_Variant_Master_vw],vrecno,thisform.PcvType,thisform.datasessionid
		If Betw(vrecno,1,Reccount())
			Go vrecno
		Endif
	ENDIF
ENDIF 
&&<---TKT-6393 Item Variant Master 19/02/2011

ENDPROC
     !����    �!  �!                        �;
   %   �      
!  S  d          �  U  \  %��  � � �  � ��4 � T� � �C�������^�� �U � T� � �C�������^�� � U  THISFORM ADDMODE EDITMODE THIS DISABLEDBACKCOLOR� ��  �" %�� � � � � � �  	����* T� �CCC��� ]fC� � f� THISFORM���! %�� � � COMMANDBUTTON��� �C ��C � C� ��]C� ��]C� ��]�C� ��]C� ��]� � �� � %�� � � LISTBOX���� %�C� �	 �
��5�L ��C � �� �	 C� ��]C� ��]C� ��]C� ��]C� ��]�� �
 �� ���L ��C � �� � C� ��]C� ��]C� ��]C� ��]C� ��]�� �
 �� � � � U  _HELPREQ THISFORM ADDMODE EDITMODE _CUROBJNAME THIS NAME HELPTYPE GRDCMDSDCGF LISTTBL GRDLSTSDCGF DEFAHELP  U  �  %��  � � �  � ��� � T�  � ��  �� %�C� � ��
 C� � �
	��^ � T� � �C� � ��� � %�C� � �
��� � T� �C� � ��� %�C� _curvalb� L��� � %�� -��� �	 B�� �� � �� � T� � �� �� � � � U	  THISFORM ADDMODE EDITMODE LISTTBL THIS VALUE DEFAEXPR WHENEXPR _CURVAL�/ ��  � � � � � � � � �	 �
 � %�C� ntopb� N��T � T�  �� �� � %�C� nleftb� N��~ � T� �� �� � %�C� nheightb� N��� � T� �� �� � %�C� nWidthb� N��� � T� �� �� � %�C� csourceb� C��� T� ��  �� � %�C� cwhenb� C��+� T� ��  �� � %�C� cdefaultb� C��X� T� ��  �� � %�C� cvalidb� C���� T� ��  �� � %�C� cerrorb� C���� T� ��  �� � %�C� chelpb� C���� T�	 ��  �� � T� � ��	 �� %�C�	 �
��3�7 T�	 �CC� {�	 � � C�	 �C� {�	 �\� �	 6�� � %�C�	 cHelptypeb� C��a� T�
 ��  �� � T� � �� �� T� � ��  �� T� � �� �� T� � �� ��� T� � �� �� T� � �� �� T� � ���� T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �C� ��� T� � �C�	 ��� T� � �C�
 ���/ T� � �CCCC��� ]fC� � f� Thisform���� %�C�	 Lother_vw�����\ %�CC� Lother_vw.Data_Ty�f� N� CC� Lother_vw.Data_Ty�f� DE� C� � fC� f	����  T� �C� Lother_vw.fld_wid���$ %�C� Lother_vw.fld_dec�� ����) T� �� C� Lother_vw.fld_dec����5 T� �C� 9� Q� .C� 9C� Lother_vw.fld_dec�Q�� ��� T� �C� 9� Q�� � T� �  �� �� T� �! �� �� � � U"  NTOP NLEFT NHEIGHT NWIDTH CSOURCE CWHEN CDEFAULT CVALID CERROR CHELP	 CHELPTYPE THIS DEFAHELPCOND LEFT TOP HEIGHT WIDTH	 MAXLENGTH CONTROLSOURCE FONTSIZE DEFAEXPR WHENEXPR	 VALIDEXPR CERRMSG DEFAHELP HELPTYPE FULLNAME THISFORM NAME	 LOTHER_VW FLD_NM NL FORMAT	 INPUTMASK  U  +
 ��  � � %�� � � � � ��$
� %�C� � �
��G � T� � �a�� �J %�� � �	 � � � �
 C��%	� � � �	 � � � �
 C��%	��� � ��Ca� � �� �" %�� � �	 
� � � �	 
	��%� %��  �����!� T� � �a�� T� �C� � ���� T� �C� � ���� %�CC� f� LMC_VW���=� T� �� MAIN_VW�� �! T� �� � CC� f� _VW�  ���6 T� �CC� Thisform.PopupTblb� U� � � � � 6��  T� �� TXTC� �� _F2key�� T� �-�� %�C� � .App0��� T� �� � (this)�� rtnval=&fldFunc
 � %�C� � .Fxp0��5� rtnval=&chklcfld()
 � %�� 
��� T� �� �� H�a���  CASE TYPE('&lcfld')=="N"���b T� �� Select Distinct CAST(� �  as Varchar(25)) as � �  From � �
  Order By � ��  CASE TYPE('&lcfld')=="D"�e�b T� �� Select Distinct CAST(� �  as Varchar(50)) as � �  From � �
  Order By � �� 2���A T� �� Select Distinct � �  From � �
  Order By � �� �K T� �C� EXE� �  � � _lxtrtbl� This.Parent.nHandle� � � � � ��" %�� � � C� _lxtrtbl�	���� F� � T� ��  ��7 T� �C� _lxtrtbl� Select.. �  � �  -�  �  a� �� %�C� �
���� T� �  �� �� � ���( ��C� No Records Found!�  �" � �! �� �* T� �C� This.Parent.nHandle� � �# �� %�C� _lxtrtbl���� Q� � � � T� � �-�� � � %�� � �	 ��~� %��  �����z� T� � �a�� ��C� � �$ �� T� � �-�� � � %�� � �	 �� 
� ��� ��
� %�C�  � �z���� ��% ���(��� �& ���A %�CCC �% �� �' �� �( �\fCC� �) �� �( \C�   f���� T�* �� �( ��� T� �  �C �% �� �' �� T� �( ��* �� %�CCC �% �� �' �>�* ����! T� �+ �CCC �% �� �' �>�* �� � T�� �, �C �% �� �' �� %��� �- � ���� T�� �. ��� �- �� � T��/ �a�� �� !� �� T��/ �-�� � �� � %��  �� � �( � 	��E� ��% ���(��� �& ��A�? %�CCC �% �� �' �� �( �\fCC� �) �� �( �\f��)� T�* �� �( ���! T� �  �CC �% �� �' ��* \�� T� �( ��* �� T�� �, �C �% �� �' �� %��� �- � ��� T�� �. ��� �- �� � T��/ �a�� �� !� �=� T��/ �-�� � �� � %��  ����� %��� �- �� ���� T�� �- ��� �- ��� T�� �0 ��� �- �� T� �  ��� �  �� T��/ �a�� �� B� � �� � %��  ���a	� %��� �- ��� �& ��Y	� T�� �- ��� �- ��� T�� �0 ��� �- �� T� �  ��� �  �� T��/ �a�� �� B� � �� � %��  �� �  �	���	� %��� �- � ���	� %���/ a���	� T� �  �C�� �- �� �' �� T�� �	 �-��
 �� �1 � � � � %��  ���
� %�� �( � ��
� �� B� � � �� � � U2  NKEYCODE NSHIFTALTCTRL THISFORM ADDMODE EDITMODE THIS DEFAHELP READONLY CMDSDC VISIBLE TOP LSTSDC MESSAGE
 CURONMOUSE LCFLD CONTROLSOURCE LCDBF	 ENTRY_TBL POPUPTBL CHKLCFLD RTNVAL FLDFUNC SQL_CON MSQLSTR	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID _LXTRTBL MACNAME UEGETPOP VALUE SHOWMESSAGEBOX VUMESS SQLCONNCLOSE CLICK X	 LISTCOUNT LIST SELSTART TEXT NCURPOS	 SELLENGTH DISPLAYVALUE	 LISTINDEX TOPINDEX XFOUND SELECTED	 LOSTFOCUSt %��  � � �  � ��m�' %�CCC� � f�=� SELE� EXEC����� T� �� ��M T� �C� EXE� �	 � � � _xtrtblw� This.Parent.nHandle�  �
 �  � � ��" %�� � � C� _xtrtblw�	��k� F� � T� �C�/�� If Len(&_sdcflds)>230�� T� �� LEFT(� � ,230)�� �" Index On &_sdcflds Tag ListTbl
 T�  � �� _xtrtblw�� T� � �� COMMANDBUTTON�� ���( ��C� No Records Found!�  � �  � �� �* T� �C� This.Parent.nHandle�  � � �� �6 %�C� � �
� CCC� � f�=� SELE� EXEC�
	��� T� � �� LISTBOX�� � %�C� � ���[�+ T� � �CC� � ��	 � *#*� C� � _6�� � ��Ca� � �� � U  THISFORM ADDMODE EDITMODE THIS DEFAHELP SQL_CON	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID _XTRTBLW _SDCFLDS LISTTBL HELPTYPE SHOWMESSAGEBOX VUMESS SQLCONNCLOSE TAG VALUE MESSAGE  U   %��  � � �  � ��� T� � �a�� %��  � a�	 �  � a��K � B� � T� �C� � ���� T�	 �C� � ����
 F��	 �� T�
 �CC+
� CO� � 6��6 %�C� � �
� CCC� � f�=� SELE� EXEC�
	���� T� �-�� T� �� ,C� � �� ,�� T� �� ,C� � �� ,�� %�C� � � ��3� T� �a�� � %�� -����( ��C� Not Found in List�  � �  � ��
 F��	 �� �� %��
 � ����	 #��
 �� � B�� � �� � � %�C� � �
���� T� �C� � ��� %�C� _curvalb� L��|� %�� -��x� %�C� � �
��<� T� �CC� � Λ�� ��C � �  � �  � �� �
 F��	 �� �� %��
 � ��h�	 #��
 �� � B�� � �� � � � ��C�  � �� ��C�  � �� T�  � ��  �� %�C� _xtrtblw����� Q� � � %�C� _xtrtblv����� Q� � � %�C� _lxtrtbl���� Q� � � � U  THISFORM ADDMODE EDITMODE THIS ENABLED
 FLAGCANCEL
 CURONMOUSE _CURFLD CONTROLSOURCE _CURTBL _CURREC DEFAHELP SQL_FND	 _SDCFLDS1	 _SDCFLDS2 VALUE SHOWMESSAGEBOX VUMESS	 VALIDEXPR _CURVAL CERRMSG _CURMES GRDCMDSDCLF GRDLSTSDCLF LISTTBL _XTRTBLW _XTRTBLV _LXTRTBL Refresh,     �� Message�     �� Valid�    �� When�    �� Init    ��
 RightClick
    �� KeyPress 
    �� GotFocus/    �� SetFocust    ��	 LostFocus{    ��1 ��� �A 3 q !�2A �1�� �A A A 3 `1 ��RA 1!�� � A � A A A 3 ��� A �� A �� A �� A �� A �� A �� A �� A �� A �� A qA �� A A!!!!!!���A�Q� 1A A A 4 P1 � �2� A �� A !!� 11�AA a� sq1A saA � � � !!� A �$q � qA � �A �q� A A � A A 1!� � A A 1� R�A��A �QQA � A A � � A A A ���A�QQA � A A � � A A A ��QA� A A A A A ��QA� A A A A A �Q�� � A A A AA A A A A A A 3 �q� �!q � ��A "��� �A �A aqA "�A � A 3 7 �� �A A 11� �a� ��a� A � �� A � A � A A 1!�� 7A�A � A � A � A A A � � q� A q� A q� A A 2                               -  1  
      M  �        �  '  e   )   B  6  |   k   W  L  �   m   k  �.  �   �   �.  3  �    73  �3  �    �3  �9  �   )   �!                       ����    �  �                        )�   %   `      7  �   �          �  U    ��  � � � � U  NBUTTON NSHIFT NXCOORD NYCOORD  ��  � � � � U  NBUTTON NSHIFT NXCOORD NYCOORD� T�  �C�	 Procedurev�� %�C� � � GA���] �. � uefrm_gst_adj�� � �9� � �9� � � � %�C� �	 � GB���� �> � uedutydebit.App�� � �9� � �9� � �9� �� GST�� �' %�C� �	 � GC� GD� C6� D6����< � udappsupplimentory.app�� � �9� � �9� � �9� � � %�C� � � J5�����5 � uefrmgstadj.app�� � �9� � �9� � �9� � %�� � � � � ���� ��C� � �� ��C� � � � � �� � � %�C� �	 � UB��� �5 � udapprcmdet.app�� � �9� � �9� � �9� � � %�C� � � BP� CP���_�? � uetdspayment.App�� � �9� � �9� � �9� �� TDS�� � %�C� � � B3�����? � uetdspayment.App�� � �9� � �9� � �9� �� TDS�� � %�C� � � B4����? � uetdspayment.App�� � �9� � �9� � �9� �� TCS�� �+ %�C� � � PP� TH� FH� EH� RH����= � ueEmpPayrollPosting.App�� � �9� � �9� � �9� � � %�� vutex� ��3� H���/�' �� �	 � AR� � � � AR��W� %�C� � � f� COLUMN��1�( %�C� � � EXCISE�
 NON-EXCISE���-� � ueetitdetinar�� � � �S� � ueetdetailinar�� � �' �� �	 � DC� � � � DC��� %�C� � � f� COLUMN����( %�C� � � EXCISE�
 NON-EXCISE����� � ueetitdetindc�� � � �� � ueetdetailindc�� � �' �� �	 � SS� � � � SS���� %�C� � � f� COLUMN����( %�C� � � EXCISE�
 NON-EXCISE����� � ueetitdetindc�� � � �' �� �	 � IR� � � � IR��H� %�C� � � f� COLUMN��"�( %�C� � � EXCISE�
 NON-EXCISE���� � ueetitdetindc�� � � �D� � ueetdetailinir�� � �' �� �	 � GT� � � � GT��/� %�C� � � f� COLUMN��	�( %�C� � � EXCISE�
 NON-EXCISE���� %�� �! 
���� � ueetitdetinar�� � �� � ueetitdetindc�� � � � �+� � ueetdetailingt�� � � � �: %�� vuser� � � vuexc� 
	� � vutex� 
	���� %�C� � � PT� P1�����1 � uefrm_pt_exdata1�� � �9� � �9� � � � � %�� vuser� ��(� %�C� � � JV���$�1 � uefrm_jv_exdata1�� � �9� � �9� � � � � %��	 spbillgen� ���� %�C� � � ST�����1 � uefrm_st_exdata1�� � �9� � �9� � � � � %�� vuexc� 
��[	� %�C� � � ST�����1 � uefrm_st_exdata1�� � �9� � �9� � � �, %�C� � � PT� P1�� C� � ��  	��W	�1 � uefrm_pt_exdata1�� � �9� � �9� � � � � %�� vuexc� ���! %�C� � � ST� EI� SI����	�1 � uefrm_st_exdata1�� � �9� � �9� � � � %�C� � � VI���=
�1 � uefrm_servicetax�� � �9� � �9� � � ��9� � � � � � ��9� � � �' � � �6 %�C� � � PT� P1�� C� � ��
 MODVATABLE	���
�1 � uefrm_pt_exdata1�� � �9� � �9� � � �( %�� vuexc� � � vuexp� 
	��W�O %�C� � � PT� P1��0 CCC� � �f� INDIGENIOUS� IMPORTED� CAPITAL�	��S�0 � uefrm_st_bond��9� � �9� � �9� � � � �! %�C� � � BD� LI� LR�����0 � uefrm_st_bond��9� � �9� � �9� � � � %�C� � � SR���P�1 � uefrm_sr_exdata1�� � �9� � �9� � �$ %�CCC� � �f� CT-1� CT-3���L�0 � uefrm_st_bond��9� � �9� � �9� � � � � %�C� � � EP� SB�����1 � uefrm_sertax_det�� � �9� � �9� � � � %�C� � � GI�����@ � uedutydebit.App�� � �9� � �9� � �9� �� RG23A�� � %�C� � � HI���W�@ � uedutydebit.App�� � �9� � �9� � �9� �� RG23C�� � %�C� � � RR�����> � uedutydebit.App�� � �9� � �9� � �9� �� PLA�� � %�C� � � VR����F � uedutydebit.App�� � �9� � �9� � �9� �� SERVICE TAX�� � %�C� � � J2���� T�+ ��  ��' T�+ �CC�, �- �
�	 �, �- � � 1=16�� If !(&vexp) ���) � ueservicetaxcredit.App��� Excise�� ���0 � ueservicetaxcreditaccrual.App��� Excise�� � %�� � � � � ��� ��C� � �� ��C� � � � � �� � � � %�C� � � J3���*� T�+ ��  ��' T�+ �CC�, �- �
�	 �, �- � � 1=16�� If !(&vexp) ���* � ueservicetaxcredit.App��� Service�� ���2 � ueservicetaxcreditaccrual.App��� SERVICES�� � %�� � � � � ��&� ��C� � �� ��C� � � � � �� � � ��0 � F� �m T�1 ��B SELECT * FROM lother WHERE att_file=1 and defa_fld =1 and e_code='C� � �� ' order by serial��F T�2 �C� EXE�5 �6  �1 � lother� thisform.nHandle� � � �3 �4 �� %��2 � ��� B�-�� � T�7 �� ��  %��2 � � C� lother�	��K� F�8 � T�7 �CN�� � %��7 � ����1 �9 ��� main_vw��C� main_vwO�� � � � �
 �� � � � %�C� lother����� Q�8 � � F� � %�� �: ��a�$ %�C� UDAppVouXtraClose.app0��� � UDAppVouXtraClose.App� �% %�C� UDTrigVouXtraClose.Fxp0��]� � UDTrigVouXtraClose.fxp� � ���% %�C� UeTrigVouXtraClose.Fxp0���� �> � � � U?  VSTPROC MAIN_VW ENTRY_TY UEFRM_GST_ADJ THISFORM DATASESSIONID
 ACTIVEFORM ADDMODE EDITMODE PCVTYPE UEDUTYDEBIT APP UDAPPSUPPLIMENTORY UEFRMGSTADJ ACCOUNTSPOSTING VOUPAGE PAGE3 TXTNETAMOUNT REFRESH UDAPPRCMDET UETDSPAYMENT UEEMPPAYROLLPOSTING VCHKPROD BEHAVE THIS PARENT CLASS RULE UEETITDETINAR UEETDETAILINAR UEETITDETINDC UEETDETAILINDC UEETDETAILINIR U_SINFO UEETDETAILINGT UEFRM_PT_EXDATA1 UEFRM_JV_EXDATA1 UEFRM_ST_EXDATA1 UEFRM_SERVICETAX
 GRDACCOUNT UEFRM_ST_BOND UEFRM_SR_EXDATA1 UEFRM_SERTAX_DET VEXP COADDITIONAL SERACCDT UESERVICETAXCREDIT UESERVICETAXCREDITACCRUAL _XTRAREC SQ1 NRETVAL	 SQLCONOBJ DATACONN COMPANY DBNAME SQL_REC LOTHER UEEXTRA UDNEWTRIGENBL UDAPPVOUXTRACLOSE UDTRIGVOUXTRACLOSE FXP UETRIGVOUXTRACLOSE
 MouseEnter,     ��
 MouseLeavei     �� Click�     ��1 13 13 0�s�A ��A q�A rQ�� aA A �QA ��A s�A q�A ��A b� q���A � �A r���A � �A r���A A r���A � �A r���� �A A � �A A A ��A A aqA A �qA A tqA �A A cA qQQA aA ��A A A rAA A �A rA qA q�A qaA r� qA�� A �� aA A A q� qA�� !A �� aA A s q �aq A � q � A � A Q� A q A�A Q�A � Qq A A 3                       T         u   �         �   |+  	    )   �                       ����    �  �                        ?�   %   Y      N  �   h          �  U  < T�  �-�� T� �C�� T� �CO�� %�C� item_vw���� �I T� ��1 select [type] mattype from it_mast where it_code=CC� � Z���I T� �C� EXE�
 �  � �	 TmpITMast� Thisform.nhandle� � � � �	 �� � %�C�	 TmpITMast���!�# %�C� � f� RAW MATERIAL��� T�  �a�� � Q� � � %�C� �
��H� SELECT &_malias
 � %�C� �CN���i�	 #�� �� � %�� � � WK��b�* T� �C�
 � �� uefrm_bomdetails.scx�� %�C� ��� O��$� H��� � �C�	 pharmaind� � �� �4 T� �C�
 � �� uefrm_bomdetails_pharmaind.scx�� � � T� �� '� � '�� IF FILE(&filenm) �+� T� �C�� T� �CO��s Do Form (&filenm) With main_vw.entry_ty,Thisform.DataSessionId, THISFORM.addmode,THISFORM.editmode,item_vw.ITEM
 %�C� �
��� SELECT &_malias
 � %�C� �CN���'�	 #�� �� � �^�+ ��CC� ��  File Not Found...!�� �x�� � � %�� � � DC��;� %��  a��P�* T� �C�
 � �� uefrm_itempickup.scx�� %�C� ��� O��,� H���(� �C�	 pharmaind� � ��(�4 T� �C�
 � �� uefrm_itempickup_pharmaind.scx�� � � T� �� '� � '�� IF FILE(&filenm) �� T� �C�� T� �CO��Y Do Form (&filenm) With Thisform.addmode,Thisform.editmode,Thisform.DataSessionId					
 %�C� �
���� SELECT &_malias
 � %�C� �CN����	 #�� �� � �L�+ ��CC� ��  File Not Found...!�� �x�� � �7�2 T� �C�
 � �� uefrm_OpStItemAllocation.scx�� %�C� ��� O��	� H���� �C�	 pharmaind� � ���< T� �C�
 � ��& uefrm_OpStItemAllocation_pharmaind.scx�� � � T� �� '� � '�� IF FILE(&filenm) � � T� �C�� T� �CO��c Do Form (&filenm) With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform						
 %�C� �
���� SELECT &_malias
 � %�C� �CN�����	 #�� �� � �3�+ ��CC� ��  File Not Found...!�� �x�� � � � %�� � � ST��(� T� �-��$ %�C� main_vw.u_choiceb� L���� T� �� � �� � %�� -���� %��  a���	�* T� �C�
 � �� uefrm_itempickup.scx�� %�C� ��� O��W� H��S� �C�	 pharmaind� � ��S�4 T� �C�
 � �� uefrm_itempickup_pharmaind.scx�� � � T� �� '� � '�� IF FILE(&filenm) ��	� T� �C�� T� �CO��Z Do Form (&filenm) With Thisform.addmode,Thisform.editmode,Thisform.DataSessionId 					
 %�� � � � � ��f	�% %�C� thisform.ItemPageb� L��b	� %�� � a��^	� ��C�� � �� � � � %�C� �
���	� SELECT &_malias
 � %�C� �CN����	�	 #�� �� � ��	�+ ��CC� ��  File Not Found...!�� �x�� � ���2 T� �C�
 � �� uefrm_OpStItemAllocation.scx�� %�C� ��� O���
� H�E
��
� �C�	 pharmaind� � ���
�< T� �C�
 � ��& uefrm_OpStItemAllocation_pharmaind.scx�� � � T� �� '� � '�� IF FILE(&filenm) ��� T� �C�� T� �CO��. %�� � 
� � � 
	� C� ItRef_vw�
	����W T� ��& SELECT * FROM stitref where entry_ty='C� � �� ' and tran_cd=C� � Z��H T� �C� EXE�
 �  � � ItRef_vw� Thisform.nhandle� � � � �	 �� � %�C� ItRef_vw����� F� �6 -�� � � � � � � � � 	� � � � � 	��> %�C� �  �
�
 C� �! �
	�
 C� �" �
	� � �  � DC	���2 T� �C�
 � �� uefrm_DcStItemAllocation.scx�� %�C� 0����9 � uefrm_dcstitemallocation�� � � � � � � � ���+ ��CC� ��  File Not Found...!�� �x�� � ���2 T� �C�
 � �� uefrm_OpStItemAllocation.scx�� %�C� ��� O���� H�^��� �C�	 pharmaind� � ����< T� �C�
 � ��& uefrm_OpStItemAllocation_pharmaind.scx�� � � T� �� '� � '�� IF FILE(&filenm) �O�` Do Form (&filenm) With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform			
 ���+ ��CC� ��  File Not Found...!�� �x�� � � �F� IF FILE(&filenm) ��d DO FORM (&filenm) WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM							
 �B�+ ��CC� ��  File Not Found...!�� �x�� � �- %�� � 
� � � 
	� C� ItRef_vw�	��� Q� � � %�C� �
���� SELECT &_malias
 � %�C� �CN�����	 #�� �� � � � �$�I ��C�7 Differential invoice does not have inventory effect...!�� �x�� � � %�C� � � IP� WI���M�, T� �C�
 � �� uefrm_bomdetailsIP.scx�� %�C� 0��� T� �C�� T� �CO��3 � uefrm_bomdetailsip�� � � � � � � � %�C� �
���� SELECT &_malias
 � %�C� �CN����	 #�� �� � �I�+ ��CC� ��  File Not Found...!�� �x�� � � %�C� � � OP� WO�����, T� �C�
 � �� uefrm_bomdetailsOP.scx�� %�C� 0���� T� �C�� T� �CO��3 � uefrm_bomdetailsop�� � � � � � � � T�& �-�� T�' �-�� T�& �C� exim�( �) �� T�' �C� dbk�( �) �� %��& � �' ���� %�� � a�	 � � a���� %�C� PROJECTITREF_vw����� �* � udappetvalid.APP� � � � F� � ���+ ��CC� ��  File Not Found...!�� �x�� � � %�C� � � RN���5�, T� �C�
 � �� uefrm_bomdetailsRN.scx�� %�C� 0���� T� �C�� T� �CO��[ Do Form &filenm With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform
 %�C� �
���� Select &_malias
 � %�C� �CN�����	 #�� �� � �1�+ ��CC� ��  File Not Found...!�� �x�� � � U-  ISRAWMAT _MALIAS _MRECNO MSQLSTR ITEM_VW IT_CODE NRETVAL THISFORM	 SQLCONOBJ DATACONN COMPANY DBNAME DATASESSIONID	 TMPITMAST MATTYPE PCVTYPE FILENM DIR_NM OGLBLINDFEAT UDCHKIND VUMESS	 ISDEFFINV MAIN_VW U_CHOICE ADDMODE EDITMODE ITEMPAGE ITEMGRDBEFCALC ENTRY_TY TRAN_CD ITREF_VW ITSERIAL	 RENTRY_TY
 ITREF_TRAN	 RITSERIAL UEFRM_DCSTITEMALLOCATION UEFRM_BOMDETAILSIP UEFRM_BOMDETAILSOP _MEXIM _MDBK OGLBLPRDFEAT	 UDCHKPROD SAVEOPPTTAXDET UDAPPETVALID APP Click,     ��1 � � � a��A �1� A � A 1A A� A c�R� �AA A s�� � 41A A� A � �A A a� �R� �AA A s�� � �1A A� A � �A � "R� ��A A s�� � 21A A� A � �A A A b� AA � � �R� �AA A s�� � ��Q!A A A 1A A� A � �A � !R� ��A A s�� � �q�A qr a�!� �� �A � !R� ��A A s�� �A A � �B� �A A �� A 1A A� A A A � �A B ��� � � 11A A� A � �A A ��� � � 1� � �qA���A A A r � �A A s�� � � �1A A� A � �A A 3                       !;      )   �                       +�PROCEDURE MouseEnter
LPARAMETERS nButton, nShift, nXCoord, nYCoord
*THIS.ZOrder(0)
ENDPROC
PROCEDURE MouseLeave
LPARAMETERS nButton, nShift, nXCoord, nYCoord
*THIS.ZOrder(1)
ENDPROC
PROCEDURE Click
**!**	vstproc=SET("Procedure")
**!**	IF INLI(MAIN_VW.ENTRY_TY,'ST')
**!**	   do form UEFRM_ST_EXDATA1 WITH THISFORM.DataSessionId,_screen.activeform.addmode,_screen.activeform.editmode
**!**	ENDIF
**!**	IF INLI(MAIN_VW.ENTRY_TY,'VI')
**!**	   do form UEFRM_SERVICETAX WITH THISFORM.DataSessionId,_screen.activeform.addmode,_screen.activeform.editmode
**!**	   _screen.ActiveForm.voupage.page3.txtnetamount.refresh
**!**	   _screen.ActiveForm.voupage.page3.grdaccount.refresh
**!**
**!**	ENDIF
*:*****************************************************************************
*:        Program: UETRIGvouactivate--Udyog ERP
*:        System: Udyog Software
*:        Author:
*: 		  Last modified:
*:		  AIM  : To Call any Customised Scree on Voucher Activate
*:*****************************************************************************
&&Rup:- This class is used in uetrigvouactivate.prg to activate [Excise Deatails] Button in voucher to Call Customised Forms.

vstproc=Set("Procedure")

&& Added by Shrikant S. on 06/10/2015 for GST		&& Start
If Inli(main_vw.entry_ty,'GA')
	Do Form uefrm_gst_adj With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
ENDIF
If (Inlist(Thisform.PcvType,'GB'))
	Do uedutydebit.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"GST"
ENDIF
If (Inlist(Thisform.PcvType,'GC','GD','C6','D6'))
	Do udappsupplimentory.app With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm
ENDIF

If Inli(main_vw.entry_ty,'J5')
	Do uefrmgstadj.app With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm
	
	If Thisform.addmode Or Thisform.editmode
			Thisform.accountsposting()
			Thisform.voupage.page3.txtnetamount.Refresh()
	Endif
Endif
&& Added by Shrikant S. on 06/10/2015 for GST		&& End

&& Added by Shrikant S. on 15/06/2017 for GST		&& Start
	If (Inlist(Thisform.PcvType,'UB'))
		Do udapprcmdet.app With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm
	ENDIF
&& Added by Shrikant S. on 15/06/2017 for GST		&& end


&&-->Bank Payment TDS Rup 16/06/2009
If Inli(main_vw.entry_ty,'BP','CP')
	Do uetdspayment.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"TDS" &&TCS TKT-5692 Add TDS
Endif
&&<--Bank Payment TDS
&&--->TCS TKT-5692
If Inli(main_vw.entry_ty,'B3')
	Do uetdspayment.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"TDS"
Endif
If Inli(main_vw.entry_ty,'B4')
	Do uetdspayment.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"TCS"
Endif
&&<---TCS TKT-5692
&&--->Payroll Rup TKT-4885
If Inli(main_vw.entry_ty,"PP","TH","FH","EH","RH")
	Do ueEmpPayrollPosting.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm
Endif
&&<---Payroll Rup TKT-4885
If "vutex" $ vchkprod
	Do Case
	Case (Thisform.pcvtype="AR" Or Thisform.behave="AR")
		If Upper(This.Parent.Class) = 'COLUMN'
			If Inlist(main_vw.Rule,'EXCISE','NON-EXCISE')
				Do Form ueetitdetinar With Thisform
			Endif
		Else
			Do Form ueetdetailinar With Thisform
		Endif

	Case (Thisform.pcvtype="DC" Or Thisform.behave="DC")
		If Upper(This.Parent.Class) = 'COLUMN'
			If Inlist(main_vw.Rule,'EXCISE','NON-EXCISE')
				Do Form ueetitdetindc With Thisform
			Endif
		Else
			Do Form ueetdetailindc With Thisform
		Endif

	Case (Thisform.pcvtype="SS" Or Thisform.behave="SS")
		If Upper(This.Parent.Class) = 'COLUMN'
			If Inlist(main_vw.Rule,'EXCISE','NON-EXCISE')
				Do Form ueetitdetindc With Thisform
			Endif
		Endif

	Case (Thisform.pcvtype="IR" Or Thisform.behave="IR")
		If Upper(This.Parent.Class) = 'COLUMN'
			If Inlist(main_vw.Rule,'EXCISE','NON-EXCISE')
				Do Form ueetitdetindc With Thisform
			Endif
		Else
			Do Form ueetdetailinir With Thisform
		Endif

	Case (Thisform.pcvtype="GT" Or Thisform.behave="GT")
		If Upper(This.Parent.Class) = 'COLUMN'
			If Inlist(main_vw.Rule,'EXCISE','NON-EXCISE')
				If !main_vw.u_sinfo
					Do Form ueetitdetinar With Thisform
				Else
					Do Form ueetitdetindc With Thisform
				Endif
			Endif
		Else
			Do Form ueetdetailingt With Thisform
		Endif
	Endcase
Endif

If "vuser" $ vchkprod And !("vuexc" $ vchkprod) And !("vutex" $ vchkprod)&&Sachin 13Nov09
	If Inli(main_vw.entry_ty,'PT','P1')
		Do Form uefrm_pt_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif
Endif
If "vuser" $ vchkprod &&Rup 13Sep09
	If Inli(main_vw.entry_ty,'JV')
		Do Form uefrm_jv_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif
Endif


&& Added by Shrikant S. on 04/082015 for Bug-26514		&& Start
IF ("spbillgen" $ vchkprod)
	If Inli(main_vw.entry_ty,'ST') 
		Do Form uefrm_st_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif
endif
&& Added by Shrikant S. on 04/082015 for Bug-26514		&& End

&& Added by Shrikant S. on 06/07/2017 for GST		&&Start
IF !"vuexc" $ vchkprod 
	IF Inli(main_vw.entry_ty,'ST')
		Do Form uefrm_st_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	ENDIF
	If Inli(main_vw.entry_ty,'PT','P1') And Alltrim(main_vw.Rule)=''
		Do Form uefrm_pt_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif
ENDIF
&& Added by Shrikant S. on 06/07/2017 for GST		&&End

If "vuexc" $ vchkprod
*	If Inli(main_vw.ENTRY_TY,'ST')
*!*		IF INLI(main_vw.entry_ty,'ST','EI') && Birendra : Export Sales in EOU Dated 21 Dec 2010  &&Commented by Priyanka for Export LC on 10022014
	If Inli(main_vw.entry_ty,'ST','EI','SI') && Birendra : Export Sales in EOU Dated 21 Dec 2010  &&Added by Priyanka for Export LC on 10022014
		Do Form uefrm_st_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif
	If Inli(main_vw.entry_ty,'VI')
		Do Form uefrm_servicetax With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
		_Screen.ActiveForm.voupage.page3.txtnetamount.Refresh
		_Screen.ActiveForm.voupage.page3.grdaccount.Refresh
	Endif
	If Inli(main_vw.entry_ty,'PT','P1') And Alltrim(main_vw.Rule)='MODVATABLE'
		Do Form uefrm_pt_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif

*** start : Added by Birendra on 19/08/2010 for EOU
	If "vuexc" $ vchkprod And !("vuexp" $ vchkprod) && Added by Ajay Jaiswal on 22/02/2012 for EXIM
&&  If Inli(main_vw.ENTRY_TY,'PT','P1') AND INLIST(UPPER(ALLTRIM(main_vw.rule)),'INDIGENIOUS','IMPORTED' )
		If Inli(main_vw.entry_ty,'PT','P1') And Inlist(Upper(Alltrim(main_vw.Rule)),'INDIGENIOUS','IMPORTED','CAPITAL')&& Changed by Ajay Jaiswal on 23/11/2010 for EOU
			Do Form uefrm_st_bond With _Screen.ActiveForm.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
		Endif
	Endif  && Added by Ajay Jaiswal on 22/02/2012 for EXIM
*** End : Added by Birendra on 19/08/2010 for EOU

* start : Added by Birendra on 19/08/2010 for EOU
*	If Inli(main_vw.ENTRY_TY,'BD')
* start : Added by Birendra on 25/11/2010 for EOU
	If Inli(main_vw.entry_ty,'BD','LI','LR')
		Do Form uefrm_st_bond With _Screen.ActiveForm.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif &&End

	If Inli(main_vw.entry_ty,'SR') &&Rup 05OCt09
		Do Form uefrm_sr_exdata1 With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
		If Inlist(Upper(Alltrim(main_vw.Rule)),'CT-1','CT-3')	&& start : Added by Birendra on 18/12/2010 for EOU
			Do Form uefrm_st_bond With _Screen.ActiveForm.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
		Endif
	Endif
	If Inli(main_vw.entry_ty,'EP','SB')
		Do Form uefrm_sertax_det With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode
	Endif
&&Duty Debit app Call-->
	If Inli(main_vw.entry_ty,'GI')
		Do uedutydebit.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"RG23A"
	Endif
	If Inli(main_vw.entry_ty,'HI')
		Do uedutydebit.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"RG23C"
	Endif
	If Inli(main_vw.entry_ty,'RR')
		Do uedutydebit.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"PLA"
	Endif
	If Inli(main_vw.entry_ty,'VR')
		Do uedutydebit.App With Thisform.DataSessionId,_Screen.ActiveForm.addmode,_Screen.ActiveForm.editmode,_Screen.ActiveForm,"SERVICE TAX"
	Endif
&&<--Duty Debit app Call
	If Inli(main_vw.entry_ty,'J2') &&Rup TKT-2647
*!*			Do ueservicetaxcredit.App With "Excise" TKT-8658
&&--->TKT-8658
		vexp=""
		vexp=Iif(!Empty(coadditional.seraccdt),coadditional.seraccdt,"1=1")
		If !(&vexp) &&TKT-8320
			Do ueservicetaxcredit.App With "Excise"
		Else
			Do ueservicetaxcreditaccrual.App With "Excise"
		Endif
&&<---TKT-TKT-TKT-8658

		If Thisform.addmode Or Thisform.editmode
			Thisform.accountsposting()
			Thisform.voupage.page3.txtnetamount.Refresh()
		Endif
	Endif
Endif
If Inli(main_vw.entry_ty,'J3') &&Rup TKT-794
&&Do ueservicetaxcredit.App With "Service"  &&TKT-8320
&&--->TKT-8658
	vexp=""
	vexp=Iif(!Empty(coadditional.seraccdt),coadditional.seraccdt,"1=1")
	If !(&vexp) &&TKT-9450
		Do ueservicetaxcredit.App With "Service"
	Else
		Do ueservicetaxcreditaccrual.App With "SERVICES"
	Endif
&&<---TKT-TKT-TKT-9450
	If Thisform.addmode Or Thisform.editmode
		Thisform.accountsposting()
		Thisform.voupage.page3.txtnetamount.Refresh()
	Endif
Endif


Local _xtrarec
Select main_vw
sq1="SELECT * FROM lother WHERE att_file=1 and defa_fld =1 and e_code='"+Alltrim(main_vw.entry_ty)+"' order by serial"
nretval = Thisform.sqlconobj.dataconn([EXE],company.dbname,sq1,"lother","thisform.nHandle",Thisform.DataSessionId)
If nretval<0
	Return .F.
Endif
sql_rec = 0
If nretval > 0 And Used('lother')
	Select lother
	sql_rec = Reccount()
Endif
If sql_rec > 0
	Do ueextra With [main_vw],Recno('main_vw'),main_vw.entry_ty,Thisform.DataSessionId
	Thisform.Refresh
Endif
If Used('lother')
	Use In lother
Endif
Select main_vw

******* Added By Sachin N. S. on 03/01/2014 for Bug-21258 ******* Start
If Thisform.UdNewTrigEnbl
	If File('UDAppVouXtraClose.app')
		Do UDAppVouXtraClose.App
	Endif
	If File('UDTrigVouXtraClose.Fxp')
		Do UDTrigVouXtraClose.fxp
	Endif
Else
	If File('UeTrigVouXtraClose.Fxp')
		Do UeTrigVouXtraClose
	Endif
Endif
******* Added By Sachin N. S. on 03/01/2014 for Bug-21258 ******* End

ENDPROC
     ;,PROCEDURE Click

&& Amrendra : Bug-4973 On 27/11/2012 ---->
isRawMat=.f.
_malias 	= ALIAS()
_mrecno	= RECNO()
IF USED('item_vw')
	msqlstr="select [type] mattype from it_mast where it_code="+ALLTRIM(STR(item_vw.it_code))
	nretval = THISFORM.sqlconobj.dataconn("EXE",company.dbname,msqlstr,"TmpITMast","Thisform.nhandle",THISFORM.DATASESSIONID)
ENDIF
IF USED('TmpITMast')
	IF UPPER(TmpITMast.mattype)=('RAW MATERIAL')
		isRawMat=.t.
	ENDIF
	USE IN TmpITMast
ENDIF
IF !EMPTY(_malias)
	SELECT &_malias
ENDIF
IF BETW(_mrecno,1,RECCOUNT())
	GO _mrecno
ENDIF
&& Amrendra : Bug-4973 On 27/11/2012 <----

IF THISFORM.pcvtype='WK'
	filenm=ALLTRIM(company.dir_nm)+"uefrm_bomdetails.scx"
	&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Start
	If Vartype(oglblIndfeat)='O'
		Do Case
		Case oglblIndfeat.udchkind('pharmaind')
			filenm=Alltrim(company.dir_nm)+"uefrm_bomdetails_pharmaind.scx"
		Endcase
	Endif
    && Added by Shrikant S. on 27/06/2014	for Bug-23280	&& End
*!*		IF FILE(filenm) && commented by suraj k on date 17-03-2015 for bug-25365
		filenm = "'"+filenm+"'" && added by suraj k on date 17-03-2015 for bug-25365
		IF FILE(&filenm) && added by suraj k on date 17-03-2015 for bug-25365
		_malias 	= ALIAS()
		_mrecno	= RECNO()
		&&DO FORM uefrm_bomdetails WITH main_vw.entry_ty,THISFORM.DATASESSIONID,; Commented by Shrikant S. on 27/06/2014	for Bug-23280
*!*			Do Form &filenm With main_vw.entry_ty,Thisform.DataSessionId,		&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Commented by Shrikant S. on 23/06/2015 for Bug-26131
		Do Form (&filenm) With main_vw.entry_ty,Thisform.DataSessionId,;		&& Added by Shrikant S. on 23/06/2015 for Bug-26131
			THISFORM.addmode,THISFORM.editmode,item_vw.ITEM
		IF !EMPTY(_malias)
			SELECT &_malias
		ENDIF
		IF BETW(_mrecno,1,RECCOUNT())
			GO _mrecno
		ENDIF
	ELSE	&&Rup 28/06/2011. TKT-8455
		=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
	ENDIF
ENDIF
IF THISFORM.pcvtype='DC'
&& Amrendra : Bug-4973 On 27/11/2012 ---->
	IF isRawMat=.t.
		filenm=ALLTRIM(company.dir_nm)+"uefrm_itempickup.scx"
		&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Start
		If Vartype(oglblIndfeat)='O'
			Do Case
			Case oglblIndfeat.udchkind('pharmaind')
				filenm=Alltrim(company.dir_nm)+"uefrm_itempickup_pharmaind.scx"
			Endcase
		Endif
&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& End
*!*			IF FILE(filenm) && commented by suraj k on date 17-03-2015 for bug-25365
		filenm = "'"+filenm+"'" && added by suraj k on date 17-03-2015 for bug-25365
		IF FILE(&filenm) && added by suraj k on date 17-03-2015 for bug-25365
			_malias 	= ALIAS()
			_mrecno	= RECNO()
*!*				Do Form &filenm With Thisform.addmode,Thisform.editmode,Thisform.DataSessionId					&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Commented by Shrikant S. on 27/06/2014	for Bug-26131
			Do Form (&filenm) With Thisform.addmode,Thisform.editmode,Thisform.DataSessionId					&& Added by Shrikant S. on 27/06/2014	for Bug-26131
&&			DO FORM uefrm_itempickup WITH THISFORM.addmode,THISFORM.editmode,THISFORM.DATASESSIONID && Commented by Shrikant S. on 27/06/2014	for Bug-23280
*			DO FORM uefrm_bomdetailsip WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM
			IF !EMPTY(_malias)
				SELECT &_malias
			ENDIF
			IF BETW(_mrecno,1,RECCOUNT())
				GO _mrecno
			ENDIF
		ELSE &&Rup 28/06/2011. TKT-8455 Add File exist checking
			=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
		ENDIF
	ELSE   && Amrendra : Bug-4973 On 27/11/2012 <----

		filenm=ALLTRIM(company.dir_nm)+"uefrm_OpStItemAllocation.scx"
		&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Start
		If Vartype(oglblIndfeat)='O'
			Do Case
			Case oglblIndfeat.udchkind('pharmaind')
				filenm=Alltrim(company.dir_nm)+"uefrm_OpStItemAllocation_pharmaind.scx"
			Endcase
		Endif
&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& End
*!*			IF FILE(filenm) &&commented by suraj k on date 17-03-2015 for bug-25365
			filenm = "'"+filenm+"'" && added by suraj k on date 17-03-2015 for bug-25365
			IF FILE(&filenm) && added by suraj k on date 17-03-2015 for bug-25365
			_malias 	= ALIAS()
			_mrecno	= RECNO()
*!*				Do Form &filenm With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform						&& Added by Shrikant S. on 27/06/2014	for Bug-23280		&& Commented by Shrikant S. on 23/06/2015	for Bug-26131
			Do Form (&filenm) With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform						&& Added by Shrikant S. on 23/06/2015	for Bug-26131
&&			DO FORM uefrm_opstitemallocation WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM  && Commented by Shrikant S. on 27/06/2014	for Bug-23280
			IF !EMPTY(_malias)
				SELECT &_malias
			ENDIF
			IF BETW(_mrecno,1,RECCOUNT())
				GO _mrecno
			ENDIF
		ELSE &&Rup 28/06/2011. TKT-8455
			=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
		ENDIF
	ENDIF && Amrendra : Bug-4973 On 27/11/2012
ENDIF

IF THISFORM.pcvtype='ST'
&& Amrendra : Bug-4973 On 27/11/2012 ---->
	isDeffInv=.F.
	IF 	Type('main_vw.u_choice')='L'
		isDeffInv=main_vw.u_choice
	ENDIF
	IF isDeffInv=.F.
		IF isRawMat=.t.
			filenm=ALLTRIM(company.dir_nm)+"uefrm_itempickup.scx"
			&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Start
			If Vartype(oglblIndfeat)='O'
				Do Case
				Case oglblIndfeat.udchkind('pharmaind')
					filenm=Alltrim(company.dir_nm)+"uefrm_itempickup_pharmaind.scx"
				Endcase
			Endif
&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& End
*!*				IF FILE(filenm) && added by suraj k on date 17-03-2015 for bug-25365
				filenm = "'"+filenm+"'" && added by suraj k on date 17-03-2015 for bug-25365
				IF FILE(&filenm) && added by suraj k on date 17-03-2015 for bug-25365
				_malias 	= ALIAS()
				_mrecno	= RECNO()
*!*					Do Form &filenm With Thisform.addmode,Thisform.editmode,Thisform.DataSessionId 					&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Commented by Shrikant S. on 23/06/2015	for Bug-26131
				Do Form (&filenm) With Thisform.addmode,Thisform.editmode,Thisform.DataSessionId 					&& Added by Shrikant S. on 23/06/2015	for Bug-26131
				&&DO FORM uefrm_itempickup WITH THISFORM.addmode,THISFORM.editmode,THISFORM.DATASESSIONID   && Commented by Shrikant S. on 27/06/2014	for Bug-23280
*			DO FORM uefrm_bomdetailsip WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM
				
** Added by Satish Pal for bug-19551 dated 01092013-start
				IF THISFORM.addmode OR THISFORM.editmode
					IF TYPE('thisform.ItemPage')='L'
						IF thisform.ItemPage= .T.
							thisform.itemgrdbefcalc(1)
						ENDIF
					ENDIF
				endif
** Added by Satish Pal for bug-19551 dated 01092013-End

				IF !EMPTY(_malias)
					SELECT &_malias
				ENDIF
				IF BETW(_mrecno,1,RECCOUNT())
					GO _mrecno
				ENDIF
			ELSE &&Rup 28/06/2011. TKT-8455 Add File exist checking
				=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
			ENDIF
		ELSE  && Amrendra : Bug-4973 On 27/11/2012 ---->
			filenm=ALLTRIM(company.dir_nm)+"uefrm_OpStItemAllocation.scx"
			&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Start
			If Vartype(oglblIndfeat)='O'
				Do Case
				Case oglblIndfeat.udchkind('pharmaind')
					filenm=Alltrim(company.dir_nm)+"uefrm_OpStItemAllocation_pharmaind.scx"
				Endcase
			Endif
&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& End
*!*				IF FILE(filenm) && commented by suraj k on date 17-03-2015 for bug-25365
				filenm="'"+filenm+"'"  && added by suraj k on date 17-03-2015 for bug-25365
				IF FILE(&filenm) && added by suraj k on date 17-03-2015 for bug-25365
				_malias 	= ALIAS()
				_mrecno	= RECNO()
				
*!*			*MESSAGEBOX(ItRef_vw.rentry_ty+str(ItRef_vw.itref_tran))
*!*			IF USED('ItRef_vw')
*!*				IF !EMPTY(ItRef_vw.rentry_ty) AND !EMPTY(ItRef_vw.itref_tran) AND !EMPTY(ItRef_vw.ritserial)
*!*					do form uefrm_DcStItemAllocation WITH Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,thisform
*!*				ENDIF
*!*			ELSE
*!*				do form uefrm_OpStItemAllocation WITH Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,thisform
*!*			ENDIF
&&--->Rup 14/08/2009
				IF !THISFORM.addmode AND !THISFORM.editmode AND !USED('ItRef_vw')
					msqlstr="SELECT * FROM stitref where entry_ty='"+ALLTRIM(main_vw.entry_ty)+"' and tran_cd="+STR(main_vw.tran_cd)
					nretval = THISFORM.sqlconobj.dataconn("EXE",company.dbname,msqlstr,"ItRef_vw","Thisform.nhandle",THISFORM.DATASESSIONID)
				ENDIF
				IF USED('ItRef_vw')
*Birendra : for Bug-3665 on 27/04/2012 : Start:
					SELECT ITREF_VW
					LOCATE FOR ITREF_VW.entry_ty=item_vw.entry_ty AND ITREF_VW.tran_cd=item_vw.tran_cd AND ITREF_VW.itserial=item_vw.itserial
*Birendra : for Bug-3665 on 27/04/2012 : End:
					IF !EMPTY(ITREF_VW.rentry_ty) AND !EMPTY(ITREF_VW.itref_tran) AND !EMPTY(ITREF_VW.ritserial) AND (ITREF_VW.rentry_ty)='DC'
						filenm=ALLTRIM(company.dir_nm)+"uefrm_DcStItemAllocation.scx" &&Rup 28/06/2011. TKT-8455 Add File exist checking
						IF FILE(filenm)
							DO FORM uefrm_dcstitemallocation WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM
						ELSE	&&Rup 28/06/2011. TKT-8455
							=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
						ENDIF
					ELSE
						filenm=ALLTRIM(company.dir_nm)+"uefrm_OpStItemAllocation.scx" &&Rup 28/06/2011. TKT-8455 Add File exist checking
						&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& Start
						If Vartype(oglblIndfeat)='O'
							Do Case
							Case oglblIndfeat.udchkind('pharmaind')
								filenm=Alltrim(company.dir_nm)+"uefrm_OpStItemAllocation_pharmaind.scx"
							Endcase
						Endif
&& Added by Shrikant S. on 27/06/2014	for Bug-23280	&& End
*!*						IF FILE(filenm)	&&commented by suraj k on date 17-03-2015 for bug-25365
						filenm="'"+filenm+"'" &&added by suraj k on date 17-03-2015 for bug-25365
						IF FILE(&filenm) &&added by suraj k on date 17-03-2015 for bug-25365
*!*							Do Form &filenm With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform			&& Added by Shrikant S. on 27/06/2014	for Bug-23280
						Do Form (&filenm) With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform			&& Added by Shrikant S. on 23/06/2015 for Bug-26131
							&&DO FORM uefrm_opstitemallocation WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM && Commented by Shrikant S. on 27/06/2014	for Bug-23280
						ELSE	&&Rup 28/06/2011. TKT-8455
							=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
						ENDIF
					ENDIF
				ELSE
*!*						filenm=ALLTRIM(company.dir_nm)+"uefrm_OpStItemAllocation.scx" &&Rup 28/06/2011. TKT-8455 Add File exist checking		&& Commented by Shrikant  S. on 13/03/2015 for Bug-25535
*!*					IF FILE(filenm)&&commented by suraj k on date 17-03-2015 for bug-25365
*!*						filenm="'"+filenm+"'" &&added by suraj k on date 17-03-2015 for bug-25365		&& Commented by Shrikant S. on 23/06/2015 for Bug-26131
					IF FILE(&filenm) &&added by suraj k on date 17-03-2015 for bug-25365		
*!*						DO FORM (&filenm) WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM							&& Added by Shrikant  S. on 13/03/2015 for Bug-25535	&& Commented by Shrikant  S. on 23/06/2015 for Bug-26131
					DO FORM (&filenm) WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM							&& Added by Shrikant  S. on 23/06/2015 for Bug-26131
*!*							DO FORM uefrm_opstitemallocation WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM			&& Commented by Shrikant  S. on 13/03/2015 for Bug-25535
					ELSE &&Rup 28/06/2011. TKT-8455 Add File exist checking
						=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
					ENDIF
				ENDIF
				IF !THISFORM.addmode AND !THISFORM.editmode AND USED('ItRef_vw')
					USE IN ITREF_VW
				ENDIF
&&---Rup 14/08/2009
				IF !EMPTY(_malias)
					SELECT &_malias
				ENDIF
				IF BETW(_mrecno,1,RECCOUNT())
					GO _mrecno
				ENDIF
			ENDIF
		ENDIF
&& Amrendra : Bug-4973 On 27/11/2012
	ELSE
		=MESSAGEBOX('Differential invoice does not have inventory effect...!',16,vumess)
	ENDIF
&& Amrendra : Bug-4973 On 27/11/2012
ENDIF



*IF THISFORM.pcvtype='IP'
IF INLIST(THISFORM.pcvtype, 'IP','WI') &&Birendra :  Bug-4543 on 02/08/2012 :Commented above and modified in below one line:
	filenm=ALLTRIM(company.dir_nm)+"uefrm_bomdetailsIP.scx"
	IF FILE(filenm)
		_malias 	= ALIAS()
		_mrecno	= RECNO()
		DO FORM uefrm_bomdetailsip WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM
		IF !EMPTY(_malias)
			SELECT &_malias
		ENDIF
		IF BETW(_mrecno,1,RECCOUNT())
			GO _mrecno
		ENDIF
	ELSE &&Rup 28/06/2011. TKT-8455 Add File exist checking
		=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
	ENDIF
ENDIF
*IF THISFORM.pcvtype='OP'
IF INLIST(THISFORM.pcvtype, 'OP','WO') &&Birendra :  Bug-4543 on 02/08/2012 :Commented above and modified in below one line:	filenm=ALLTRIM(company.dir_nm)+"uefrm_bomdetailsOP.scx"
	filenm=ALLTRIM(company.dir_nm)+"uefrm_bomdetailsOP.scx"
	IF FILE(filenm)
		_malias 	= ALIAS()
		_mrecno	= RECNO()
		DO FORM uefrm_bomdetailsop WITH THISFORM.DATASESSIONID,THISFORM.addmode,THISFORM.editmode,THISFORM
*!*			If !Empty(_Malias)
*!*				Select &_Malias
*!*			ENDIF
*!*	** Start : Added by Uday on 26/12/2011 for EXIM
		_mexim = .F.                              && Added by Ajay Jaiswal on 22/02/2012 for EXIM
		_mdbk = .F.                               && Added by Ajay Jaiswal on 03/04/2012 for DBK
		_mexim = oglblprdfeat.udchkprod('exim')   && Added by Ajay Jaiswal on 22/02/2012 for EXIM
		_mdbk = oglblprdfeat.udchkprod('dbk')     && Added by Ajay Jaiswal on 03/04/2012 for DBK
		IF _mexim  OR _mdbk                       && Added by Ajay Jaiswal on 21/02/2012 for EXIM & DBK
			IF THISFORM.addmode = .T. OR THISFORM.editmode = .T.
				IF USED("PROJECTITREF_vw")
					DO saveoppttaxdet IN udappetvalid.APP
				ENDIF
			ENDIF
		ENDIF										&& Added by Ajay Jaiswal on 21/02/2012 for EXIM
*!*	** Start : Added by Uday on 26/12/2011 for EXIM
		SELECT main_vw
* Birendra : commented below for TKT-8913 on 26/07/2011 :Start:
*!*			If Betw(_mRecNo,1,Reccount())
*!*				Go _mRecNo
*!*			ENDIF
* Birendra : commented below for TKT-8913 on 26/07/2011 :End:
	ELSE &&Rup 28/06/2011. TKT-8455 Add File exist checking
		=MESSAGEBOX(ALLTRIM(filenm)+' File Not Found...!',16,vumess)
	ENDIF
ENDIF

&& Added by Shrikant S. on 09/05/2016  		&& Start
If Inlist(Thisform.pcvtype, 'RN')
	filenm=Alltrim(company.dir_nm)+"uefrm_bomdetailsRN.scx"
	If File(filenm)
		_malias 	= Alias()
		_mrecno	= Recno()
		Do Form &filenm With Thisform.DataSessionId,Thisform.addmode,Thisform.editmode,Thisform
		If !Empty(_malias)
			Select &_malias
		Endif
		If Betw(_mrecno,1,Reccount())
			Go _mrecno
		Endif
	Else
		=Messagebox(Alltrim(filenm)+' File Not Found...!',16,vumess)
	Endif
Endif
&& Added by Shrikant S. on 09/05/2016 		&& End

ENDPROC
     t���    [  [                        �x   %   �
      �  o   �
          �  U  � %��  � � �  � ���� %��  � a��0 � B� � T� �� � � �� T� �C� ��� T� �C� ���� T�	 �C� ����
 F��	 �� T�
 �CC+
� CO� � 6�� %��  � ���� � T�  � ��  � ���
 F��	 �� � T�  � ��  � ��� T� �CC� � � ��R�� %�C� g� ���� T� �� Ainfo_vw�� T� �� Currow� �� %�C� � .� b� L����/ REPLACE &_curfldnm WITH .f. IN &_curtblnm		
 � T�  � ��  �� T�  � �� �� � �� � � � � � T�  � ��  �� T�  � �-�� � U  THISFORM ADDMODE EDITMODE
 FLAGCANCEL CCONTROL THIS PARENT
 CMDCONTROL _CURFLD _CURTBL _CURREC SETVALUE	 _CURFLDNO	 _CURTBLNM	 _CURFLDNM AINFOOLDFLD AINFOOLDREC REFRESH LISTTBL SETFS�  %��  � � � �  � � ��' � B�-�� � %�� � � � � ���� %�� � a��[ � B� � %�� � -��� � T� �	 ���� � T� �
 �a�� T� �CC�  � � ��R�� T� �� Ainfo_vw��
 F�� ��  T� �CCCO�CN�� CO� � 6�� %�C� � �
���� T� �C� � ���� T� �C� � ����
 F�� �� T� �� � �� %�� � ��Y�	 #�� �� � %�C� � .� b� L����) REPLACE &_curfld WITH .f. IN &_curtbl
 � �
 F�� �� %�� � ����	 #�� �� � %�C� g� ��t� T� �� Currow� �� %�C� � .� b� L��G�- REPLACE &_curfldnm WITH .t. IN &_curtblnm
 � T� � �� � .� �� T� � �� �� � ��  � � � � � T� � ��  �� T� ��  � � �� T� �C� ��� T� ��  �� T� �C� � ���� T� �C� � ���� %�C� b� L��� cControlv= &cControl
 � %�C� �� C�  � � �
	���� T� �C�  � � ��� %�C� �
����2 REPLACE &cControlf WITH &_curwval IN cControlt
 � � � U  THIS PARENT CURRENTCONTROL NAME THISFORM ADDMODE EDITMODE
 FLAGCANCEL SETFS SETVALUE
 LOCKSCREEN	 _CURFLDNO
 CMDCONTROL	 _CURTBLNM _AINFONEWREC AINFOOLDFLD _CURFLD _CURTBL _AINFOOLDREC AINFOOLDREC	 _CURFLDNM REFRESH LISTTBL CCONTROL	 CCONTROLV	 CCONTROLF	 CCONTROLT DEFAEXPR _CURWVAL�  T�  � �a�� T� �� � � �� T� �C� ��� T� �� � � �� T� �C� ���k � ��C� ��]��C� ��]C� ��]�� �  �	 �  �
 � � � �  � � � � � � � � � � � U  THISFORM NOVOUREFRESH CCONTROL THIS PARENT
 CMDCONTROL CCAPTION
 CMDCAPTION	 UENRENTRY ADDMODE EDITMODE NAME	 BACKCOLOR LSTTABLE LSTCOND SPLCHK?  %��  � � �  � ��8 � T�  � �a�� T�  � ���� � U  THISFORM ADDMODE EDITMODE SETFS SETVALUE�  %�C|�	��W �  %��  � � � � � � ��S �# ��C�  � ��� � � � � �� � � %�C|���� �  %��  � � � � � � ��� �/ ��C�  � �� � � � � � � � � � �� � � U  THISFORM
 CUR_ACTCOL THIS PARENT COLUMNCOUNT ACTIVATECELL
 CUR_ACTROW Valid,     �� When�    �� Click�    �� SetFocus	    ��	 LostFocus�	    ��1 �!A A 1� � �Aq� A q�1Qq��A A 1� A 3 q A �!A A !A � �Q� 111� � A ��A A � � A 1q��A �A 11� � 11A�A �Q!A A N 3 � 1� 1� �3 �� A 3 1A A �A A 2                       ;        V  �
  !   X   �
  s  i   _   �  �  t   d     �  {    )   [                       ����    �  �                        �   %   �      )  �   �          �  U    U    U  8  %��  � � � �  � � ��' � B�-�� �F T�  � �CC� this.parent.nheightb� N� �  � � � C�  � � �6��I T�  � �CC� this.parent.nwidthb� N� �  � � � C�  � � �6���3 T� �CCC�  � �	 �� �  � �	 � C�  � �	 �6��� T�  �
 �� �� T� �C��� %�� 
��� B�-�� �6 %�C�" This.Parent.Parent.Text1.BackColorb� N��u� T�  � ��  � � � � �� � T� ��  �
 �� %�C� �
���� This.Value	= &_defvalue
 � %�� � � � � ��1� %�� � a���� B� � %�� � -��� T� � ���� � T� � �a�� T� �CC�  � �	 ��R�� T� �� Ainfo_vw��
 F�� ��  T� �CCCO�CN�� CO� � 6�� %�C� � �
��4� T� �C� � ���� T� �C� � ����
 F�� �� T� �� � �� %�� � ����	 #�� �� � %�C� � .� b� L��0�) REPLACE &_curfld WITH .f. IN &_curtbl
 � �
 F�� �� %�� � ��\�	 #�� �� � %�C� g� ��� T� �� Currow� �� %�C� � .� b� L����- REPLACE &_curfldnm WITH .t. IN &_curtblnm
 � T� � �� � .� �� T� � �� �� � ��  � � � � � T� � ��  ��" %�C�  �  �� C�  � �! �
	���� T�" �C�  � �! ��� %�C�" �
��� T�  �  ��" �� � � %�C�  � �# �
��-� T�" �C�  � �# ��� %�C�" �
��)� T�$ �C�" ��� %�C� _curvalb� L��� %��$ -��	� \�� {TAB}�� � �%� T�  �  ��$ �� � � � � U%  THIS PARENT CURRENTCONTROL NAME HEIGHT NHEIGHT WIDTH NWIDTH	 _DEFVALUE CSOURCE CONTROLSOURCE	 _DEFAWHEN	 BACKCOLOR TEXT1 THISFORM ADDMODE EDITMODE
 FLAGCANCEL SETFS SETVALUE
 LOCKSCREEN	 _CURFLDNO	 _CURTBLNM _AINFONEWREC AINFOOLDFLD _CURFLD _CURTBL _AINFOOLDREC AINFOOLDREC	 _CURFLDNM REFRESH LISTTBL VALUE DEFAEXPR _CURWVAL WHENEXPR _CURVAL�  %�C|�	� C|���b �  %��  � � � � � � ��^ �# ��C�  � ��� � � � � �� � � %�C|���� �  %��  � � � � � � ��� �/ ��C�  � �� � � � � � � � � � �� � � U  THISFORM
 CUR_ACTCOL THIS PARENT COLUMNCOUNT ACTIVATECELL
 CUR_ACTROW?  %��  � � �  � ��8 � T�  � �a�� T�  � ���� � U  THISFORM ADDMODE EDITMODE SETFS SETVALUE� T�  �C��� %��  
��+ � �� B�� � �� � %�� � � � � ���� %�� � a�	 � � a��k � B� � T� �C� �	 ���� T�
 �C� �	 ����
 F��
 �� T� �CC+
� CO� � 6�� %�� � ���� � T� � �� � ���
 F��
 �� B�� � �� � T� � �� � ��� %�C� � � �
��x� T� �C� � � ��� %�C� �
��t� T� �C� ��� %�C� _curvalb� L��p� %�� -��l� T� �� Invalid Value�� T� �� � � �� %�C� �
��� T� �C� ��� %�C� �
��� T� �CC� Λ�� T� �CC� �
� � � � 6�� � � ��C � �  � � � ��
 F��
 �� �� %�� � ��\�	 #�� �� � B�� � �� � � � � T� �CC� � � ��R�� %�C� g� ���� T� �� Ainfo_vw�� T� �� Currow� �� %�C� � .� b� L��!�/ REPLACE &_curfldnm WITH .f. IN &_curtblnm		
 � T� � ��  �� T� � �� �� T� �� � � �	 ��4 REPLACE &_curfldnm WITH This.Value IN &_curtblnm
 � �� � � � � � T� � ��  �� T� � �-�� � U 
 _DEFAVALID THIS VALUE THISFORM ADDMODE EDITMODE
 FLAGCANCEL
 CURONMOUSE _CURFLD CONTROLSOURCE _CURTBL _CURREC SETVALUE PARENT	 VALIDEXPR _CURWVAL _CURVAL _CURMES _CURWMES CERRMSG SHOWMESSAGEBOX VUMESS	 _CURFLDNO CSOURCE	 _CURTBLNM	 _CURFLDNM AINFOOLDFLD AINFOOLDREC REFRESH LISTTBL SETFS GotFocus,     �� ErrorMessage3     �� When:     ��	 LostFocus�    �� SetFocus    �� Validy    ��1 7 6 q A a�1� � q A a�A �A �!A A !A � �Q� 111� � A ��A A � � A 1q��A �A 1!QA A aQ� �� � A � A A A A 3 �1A A �A A �1 �� A 3 � � A � A ��A A 11� �Aq� � A qaQ� �� �1� �A A �� A � A � A A A A �1Qq��A aAA 1� A 2                       @        c  �        �  b
     P   �
  w  ]   \   �  �  �   a     �  �    )   �                       *���                              �r   %         �  �   N          �  U  y  %��  � � � �  � � ��' � B�-�� �F T�  � �CC� this.parent.nheightb� N� �  � � � C�  � � �6��I T�  � �CC� this.parent.nwidthb� N� �  � � � C�  � � �6���3 T� �CCC�  � �	 �� �  � �	 � C�  � �	 �6��� T�  �
 �� �� T� �C��� %�� 
��� B�-�� �6 %�C�" This.Parent.Parent.Text1.BackColorb� N��u� T�  � ��  � � � � �� � T� ��  �
 �� %�C� �
���� %�C� b� T����! This.Value	= Ttod(&_defvalue)
 ��� This.Value	= &_defvalue
 � � %�� � � � � ��r� %�� � a��)� B� � %�� � -��O� T� � ���� � T� � �a�� T� �CC�  � �	 ��R�� T� �� Ainfo_vw��
 F�� ��  T� �CCCO�CN�� CO� � 6�� %�C� � �
��u� T� �C� � ���� T� �C� � ����
 F�� �� T� �� � �� %�� � ��'�	 #�� �� � %�C� � .� b� L��q�) REPLACE &_curfld WITH .f. IN &_curtbl
 � �
 F�� �� %�� � ����	 #�� �� � %�C� g� ��B� T� �� Currow� �� %�C� � .� b� L���- REPLACE &_curfldnm WITH .t. IN &_curtblnm
 � T� � �� � .� �� T� � �� �� � ��  � � � � � T� � ��  ��" %�C�  �  �� C�  � �! �
	���� T�" �C�  � �! ��� %�C�" �
���� T�  �  ��" �� � � %�C�  � �# �
��n� T�" �C�  � �# ��� %�C�" �
��j� T�$ �C�" ��� %�C� _curvalb� L��N� %��$ -��J� \�� {TAB}�� � �f� T�  �  ��$ �� � � � � U%  THIS PARENT CURRENTCONTROL NAME HEIGHT NHEIGHT WIDTH NWIDTH	 _DEFVALUE CSOURCE UCONTROLSOURCE	 _DEFAWHEN	 BACKCOLOR TEXT1 THISFORM ADDMODE EDITMODE
 FLAGCANCEL SETFS SETVALUE
 LOCKSCREEN	 _CURFLDNO	 _CURTBLNM _AINFONEWREC AINFOOLDFLD _CURFLD _CURTBL _AINFOOLDREC AINFOOLDREC	 _CURFLDNM REFRESH LISTTBL VALUE DEFAEXPR _CURWVAL WHENEXPR _CURVAL� T�  �C��� %��  
��+ � �� B�� � �� � %�� � � � � ���� %�� � a�	 � � a��k � B� � T� �C� �	 ���� T�
 �C� �	 ����
 F��
 �� T� �CC+
� CO� � 6�� %�� � ���� � T� � �� � ���
 F��
 �� B�� � �� � T� � �� � ��� %�C� � � �
��w� T� �C� � � ��� %�C� �
��s� T� �C� ��� %�C� _curvalb� L��o� %�� -��k� T� �� Invalid Date�� T� �� � � �� %�C� �
��� T� �C� ��� %�C� �
��� T� �CC� Λ�� T� �CC� �
� � � � 6�� � � ��C � �  � � � ��
 F��
 �� �� %�� � ��[�	 #�� �� � B�� � �� � � � � T� �CC� � � ��R�� %�C� g� ���� T� �� Ainfo_vw�� T� �� Currow� �� %�C� � .� b� L�� �/ REPLACE &_curfldnm WITH .f. IN &_curtblnm		
 � T� � ��  �� T� � �� �� T� �� � � � ��: REPLACE &_curfldnm WITH DTOC(This.Value) IN &_curtblnm
 � �� � � � � � T� � ��  �� T� � �-�� � U  
 _DEFAVALID THIS VALUE THISFORM ADDMODE EDITMODE
 FLAGCANCEL
 CURONMOUSE _CURFLD UCONTROLSOURCE _CURTBL _CURREC SETVALUE PARENT	 VALIDEXPR _CURWVAL _CURVAL _CURMES _CURWMES CERRMSG SHOWMESSAGEBOX VUMESS	 _CURFLDNO CSOURCE	 _CURTBLNM	 _CURFLDNM AINFOOLDFLD AINFOOLDREC CONTROLSOURCE REFRESH LISTTBL SETFS?  %��  � � �  � ��8 � T�  � �a�� T�  � ���� � U  THISFORM ADDMODE EDITMODE SETFS SETVALUE�  T�  �C��� %��  
��+ � �� B�� � �� � %�C|�	� C|���� �  %�� � � � � � � ��� �# ��C� � ��� � � � � �� � � %�C|���� �  %�� � � � � � � ��� �/ ��C� � �� � � � � � � � � � �� � � U	 
 _DEFAVALID THIS VALUE THISFORM
 CUR_ACTCOL PARENT COLUMNCOUNT ACTIVATECELL
 CUR_ACTROW When,     �� Valid    �� SetFocus:    ��	 LostFocus�    ��1 q A a�1� � q A a�A A� �A A �!A A !A � �Q� 111� � A ��A A � � A 1q��A �A 1!QA A aQ� �� � A � A A A A 3 � � A � A ��A A 11� �Aq� � A qaQ� �� �1� �A A �� A � A � A A A A �1Qq��A a�A 1� A 3 �� A 3 � � A � A �1A A �A A 2                       �     R   	  �  T   �   �    �   �   1    �    )                          9�PROCEDURE Refresh
&&added by satish pal for bug-2829 on 31/05/2012--Start
IF (THISFORM.addmode OR THISFORM.editmode)
	THIS.DISABLEDBACKCOLOR =RGB(255,255,255)
ELSE
	THIS.DISABLEDBACKCOLOR=RGB(229,229,229)
ENDIF
&&added by satish pal for bug-2829 on 31/05/2012--End
ENDPROC
PROCEDURE Message
LPARAMETERS _HelpReq
If (Thisform.Addmode Or Thisform.Editmode) AND _HelpReq
	_curobjName = STRTRAN(UPPER(SYS(1272,This)),Uppe(Thisform.Name),'THISFORM')
	IF this.HelpType = 'COMMANDBUTTON'
		Thisform.GrdCmdSdcGf(_curobjName,OBJTOCLIENT(This,1),OBJTOCLIENT(This,2)+OBJTOCLIENT(This,3)+2,;
				OBJTOCLIENT(This,4),OBJTOCLIENT(This,4))
	Endif
	IF this.HelpType = 'LISTBOX'
		IF !EMPTY(Thisform.ListTbl)
			Thisform.GrdLstSdcGf(_curobjName,2,Thisform.ListTbl,OBJTOCLIENT(This,1)+OBJTOCLIENT(This,4),;
				OBJTOCLIENT(This,2),OBJTOCLIENT(This,3),OBJTOCLIENT(This,4)*5)
		ELSE
			Thisform.GrdLstSdcGf(_curobjName,1,This.Defahelp,OBJTOCLIENT(This,1)+OBJTOCLIENT(This,4),;
				OBJTOCLIENT(This,2),OBJTOCLIENT(This,3),OBJTOCLIENT(This,4)*5)
		Endif
	Endif	
Endif

ENDPROC
PROCEDURE Valid
*!*	If (Thisform.Addmode Or Thisform.Editmode)
*!*		If Thisform.FlagCancel=.t. OR thisform.curonmouse = .t.
*!*			Return
*!*		ENDIF
*!*		_curfld = JUSTEXT(This.ControlSource)
*!*	  	_curtbl = JUSTSTEM(This.ControlSource)
*!*		SELECT (_curtbl)
*!*		_currec = IIF(!EOF(),RECNO(),0)
*!*	*!*		If Thisform.SetValue < 2
*!*	*!*			Thisform.SetValue = Thisform.SetValue + 1
*!*	*!*			SELECT (_curtbl)
*!*	*!*			NoDefault
*!*	*!*			If _currec > 0
*!*	*!*				GoTo _currec
*!*	*!*			EndIf
*!*	*!*			RETURN This.Value
*!*	*!*		ENDIF
*!*	*!*		Thisform.SetValue = Thisform.SetValue + 1
*!*		IF !EMPTY(This.Defahelp) AND LEFT(UPPER(This.Defahelp),4) != 'SELE'
*!*			sql_fnd = .f.
*!*			_sdcflds1 = ','+ALLTRIM(This.Defahelp)+','
*!*			_sdcflds2 = ','+ALLTRIM(this.value)+','
*!*			if AT(_sdcflds2,_sdcflds1) > 0
*!*				sql_fnd = .t.
*!*			endif	
*!*			IF sql_fnd = .f.		
*!*				Thisform.ShowMessageBox("Not Found in Master",0+32,vumess)
*!*				SELECT (_curtbl)
*!*				NoDefault
*!*				If _currec > 0
*!*					GoTo _currec
*!*				EndIf
*!*				RETURN This.Value
*!*			ENDIF
*!*		ENDIF
*!*		IF !EMPTY(this.validexpr)
*!*			_curval = EVAL(this.validexpr)
*!*			IF TYPE('_curval') = 'L'
*!*				IF _curval = .f.
*!*					IF EMPTY(this.cerrmsg)
*!*						_curmes = 'Invalid Input'
*!*					Else	
*!*						_curmes = ALLTRIM(EVAL(this.cerrmsg))
*!*					Endif	
*!*					Thisform.ShowMessageBox(_curmes,0+32,vumess)
*!*					SELECT (_curtbl)
*!*					NoDefault
*!*					If _currec > 0
*!*						GoTo _currec
*!*					ENDIF
*!*					Retu This.Value
*!*				Endif	
*!*			Endif	
*!*		ENDIF
*!*		Thisform.GrdCmdSdcLf()
*!*		Thisform.GrdLstSdcLf()
*!*	*!*		Thisform.SetFs   = .f.
*!*		Thisform.ListTbl = []		
*!*		IF USED('_xtrtblw')
*!*			USE IN _xtrtblw
*!*		Endif	
*!*		IF USED('_xtrtblv')
*!*			USE IN _xtrtblv
*!*		Endif	
*!*		IF USED('_lxtrtbl')
*!*			USE IN _lxtrtbl
*!*		Endif	
*!*	ENDIF

ENDPROC
PROCEDURE When
IF Thisform.AddMode Or Thisform.EditMode
*!*		IF Thisform.SetFs		 = .f.
*!*			Thisform.SetValue    = 3
*!*		Endif	
	Thisform.ListTbl = []
	IF EMPTY(This.Value) AND !EMPTY(this.Defaexpr)
*!*			This.Value = this.Defaexpr		&& Commented by Shrikant S. on 15/07/2017 for GST		
		This.Value =EVALUATE(this.Defaexpr)			&& Added by Shrikant S. on 15/07/2017 for GST		
	ENDIF
	IF !EMPTY(THIS.whenexpr)
		_curval = EVAL(THIS.whenexpr)
		IF TYPE('_curval') = 'L'
			IF _curval = .f.
				RETURN _curval
			Endif	
		Else
			This.Value = _curval
		Endif	
	ENDIF
ENDIF

ENDPROC
PROCEDURE Init
Lpara nTop, nLeft, nHeight, nWidth, cSource, cWhen, cDefault, cValid, cError, cHelp,cHelptype
If Type('ntop') # 'N'
	nTop = 0
Endif
If Type('nleft') # 'N'
	nLeft = 0
Endif
If Type('nheight') # 'N'
	nHeight = 0
Endif
If Type('nWidth') # 'N'
	nWidth = 0
Endif
If Type('csource') # 'C'
	cSource = ''
Endif
If Type('cwhen') # 'C'
	cWhen = ''
Endif
If Type('cdefault') # 'C'
	cDefault = ''
Endif
If Type('cvalid') # 'C'
	cValid = ''
Endif
If Type('cerror') # 'C'
	cError = ''
Endif
If Type('chelp') # 'C'
	cHelp = ''
Endif
This.Defahelpcond = cHelp
If !Empty(cHelp)
	cHelp = Iif(At('{',cHelp)>0,Substr(cHelp,1,At('{',cHelp)-1),cHelp)
Endif
If Type('cHelptype') # 'C'
	cHelptype = ''
Endif

This.Left          = nLeft
This.Top           = nTop
This.Height        = nHeight
This.Width     	   = nWidth * 8
This.MaxLength 	   = nWidth
This.ControlSource = cSource
This.FontSize      = 8
This.Defaexpr      = Alltrim(cDefault)
This.whenexpr      = Alltrim(cWhen)
This.validexpr     = Alltrim(cValid)
This.cErrMsg       = Alltrim(cError)
This.DefaHelp      = Alltrim(cHelp)
This.HelpType	   = Alltrim(cHelptype)
&&vasant250410
This.Fullname = PROPER(STRTRAN(UPPER(SYS(1272,This)),UPPER(thisform.Name),'Thisform'))
&&vasant250410

************ Added By Sachin N. S. on 21/01/2010 - TKT-215 ************ Start
If Used('Lother_vw')
*!*		If Upper(Evaluate('Lother_vw.Data_Ty')) = [N] Or Upper(Evaluate('Lother_vw.Data_Ty')) = 'DE'
	If (Upper(Evaluate('Lother_vw.Data_Ty')) = [N] Or Upper(Evaluate('Lother_vw.Data_Ty')) = 'DE' ) AND UPPER(lother_vw.fld_nm)==UPPER(cSource) && Changed By Rupesh for Work contract on 10/10/10
		nl = Evaluate('Lother_vw.fld_wid')
		If Evaluate('Lother_vw.fld_dec') > 0
			nl = nl-(Evaluate('Lother_vw.fld_dec')+1)
			nl = Repl('9',nl)+[.]+Repl('9',Evaluate('Lother_vw.fld_dec'))
		Else
			nl = Repl('9',nl)
		Endif
		This.Format    = nl
		This.InputMask = nl
	Endif
Endif
************ Added By Sachin N. S. on 21/01/2010 - TKT-215 ************ End

ENDPROC
PROCEDURE RightClick
*!*	IF !Thisform.CmdSdc.Visible And !Thisform.LstSdc.Visible AND !This.ReadOnly
*!*		lcfld = JUSTEXT(This.ControlSource)
*!*		lcdbf = JUSTSTEM(This.ControlSource)
*!*		lcdbf = Thisform.Entry_tbl+Strtran(upper(lcdbf),'_VW','')
*!*		sql_con = 0
*!*		sql_con = DataConn([EXE],Company.DbName,[Select Distinct ]+lcfld+[ From ]+lcdbf+[ Order By ]+lcfld,[_lxtrtbl],.f.)
*!*		IF sql_con > 0 AND USED('_lxtrtbl')
*!*			SELECT _lxtrtbl
*!*			mAcName = []
*!*			mAcName = GetPop('_lxtrtbl','Select..',lcfld,lcfld,'',.f.,'','',.t.)
*!*			If !Empty(mAcName) 
*!*				this.value = mAcName
*!*			ENDIF
*!*		ELSE
*!*			Thisform.ShowMessageBox("No Records Found!",0+32,vumess)
*!*		ENDIF
*!*		IF USED('_lxtrtbl')
*!*			USE IN _lxtrtbl
*!*		Endif	
*!*	ENDIF
ENDPROC
PROCEDURE KeyPress
LPARAMETERS nkeycode, nshiftaltctrl

IF (THISFORM.addmode OR THISFORM.editmode)
&&added by satish pal for bug-619 dt.16/12/2011
	IF !EMPTY(THIS.defahelp)
*			This.Enabled= .F.	&& Commented by Amrendra for bug-1099 on 30/03/2012
		THIS.READONLY = .T.	 && Added by Amrendra for bug-1099 on 30/03/2012
	ENDIF
&&end by satish pal
	IF (THISFORM.cmdsdc.VISIBLE AND THISFORM.cmdsdc.TOP > SYSMETRIC(2)) OR ;
			(THISFORM.lstsdc.VISIBLE AND THISFORM.lstsdc.TOP > SYSMETRIC(2))
		THIS.MESSAGE(.T.)
	ENDIF
	IF !THISFORM.cmdsdc.VISIBLE AND !THISFORM.lstsdc.VISIBLE
		IF nkeycode=-1
			THISFORM.curonmouse = .T.		&& Added By Sachin N. S. on 25/05/2010 for TKT-1535
			lcfld = JUSTEXT(THIS.CONTROLSOURCE)
			lcdbf = JUSTSTEM(THIS.CONTROLSOURCE)
&&vasant140810 TKT-381
			IF INLIST(UPPER(lcdbf),'LMC_VW')
				lcdbf = 'MAIN_VW'
			ENDIF
&&vasant140810
			lcdbf = THISFORM.entry_tbl+STRTRAN(UPPER(lcdbf),'_VW','')
			lcdbf = Iif(Type('Thisform.PopupTbl')='U',lcdbf,Thisform.PopupTbl)	&& Added by Sachin N. S. on 20/11/2012 for Bug-7313

&& Added by Raghu on 07/10/2011 for EXIM ---> Start
			chklcfld = [TXT]+ALLTRIM(lcfld)+[_F2key]		&&&Raghu 071011 --> to call getpop
			rtnval = .F.
			
&& Added by Shrikant S. on 05/11/2016 for GST	&& Start			
			IF FILE(chklcfld+'.App')
				fldFunc=chklcfld+"(this)" 
				rtnval=&fldFunc
			ENDIF
&& Added by Shrikant S. on 05/11/2016 for GST	&& End
						
			IF FILE(chklcfld+'.Fxp')
				rtnval=&chklcfld()
			ENDIF

			IF ! rtnval
&& Added by Raghu on 07/10/2011 for EXIM ---> End
				sql_con = 0
&& Added By Satish Pal  on 20/10/2011 for TKT-9669
				DO CASE
					CASE TYPE('&lcfld')=="N"
						msqlstr=[Select Distinct CAST(]+lcfld+[ as Varchar(25)) as ]+lcfld+[ From ]+lcdbf+[ Order By ]+lcfld
					CASE TYPE('&lcfld')=="D"
						msqlstr=[Select Distinct CAST(]+lcfld+[ as Varchar(50)) as ]+lcfld+[ From ]+lcdbf+[ Order By ]+lcfld
					OTHERWISE
						msqlstr=[Select Distinct ]+lcfld+[ From ]+lcdbf+[ Order By ]+lcfld
				ENDCASE

				sql_con = THISFORM.sqlconobj.dataconn([EXE],company.dbname,msqlstr,[_lxtrtbl],"This.Parent.nHandle",THISFORM.DATASESSIONID)
*!*				sql_con = Thisform.SqlConObj.DataConn([EXE],Company.DbName,[Select Distinct ]+lcfld+[ From ]+lcdbf+;
*!*					[ Order By ]+lcfld,[_lxtrtbl],"This.Parent.nHandle",Thisform.DataSessionId)
&& End By Satish Pal
				IF sql_con > 0 AND USED('_lxtrtbl')
					SELECT _lxtrtbl
					macname = []
					macname = uegetpop('_lxtrtbl','Select..',lcfld,lcfld,'',.F.,'','',.T.)
					IF !EMPTY(macname)
						THIS.VALUE = macname
					ENDIF
				ELSE
					THISFORM.showmessagebox("No Records Found!",0+32,vumess)
				ENDIF
				sql_con = THISFORM.sqlconobj.sqlconnclose("This.Parent.nHandle")
				IF USED('_lxtrtbl')
					USE IN _lxtrtbl
				ENDIF
			ENDIF && Added by Raghu on 07/10/2011 for EXIM to call getpop

			THISFORM.curonmouse = .F.		&& Added By Sachin N. S. on 25/05/2010 for TKT-1535
		ENDIF
	ENDIF
	IF THISFORM.cmdsdc.VISIBLE
		IF nkeycode=-1
			THISFORM.curonmouse = .T.
			THISFORM.cmdsdc.CLICK()
			THISFORM.curonmouse = .F.
		ENDIF
	ENDIF
	IF THISFORM.lstsdc.VISIBLE
		WITH THISFORM
*-------------Character Keys
			IF BETWEEN(nkeycode, 32, 122)
*!*					if .lstsdc.visible=.f.
*!*					.lstSdc.visible=.t.
*!*					endif
				FOR x = 1 TO .lstsdc.LISTCOUNT
					IF UPPER(SUBS(.lstsdc.LIST(x), 1, THIS.SELSTART+ 1)) = ;
							UPPER(SUBS(THIS.TEXT, 1, THIS.SELSTART)+CHR(nkeycode))
						ncurpos = THIS.SELSTART + 1
						THIS.VALUE = .lstsdc.LIST(x)
						THIS.SELSTART = ncurpos
						IF LEN(ALLT(.lstsdc.LIST(x))) > ncurpos
							THIS.SELLENGTH = LEN(ALLT(.lstsdc.LIST(x))) - ncurpos
						ENDIF
						.lstsdc.DISPLAYVALUE=.lstsdc.LIST(x)
						IF .lstsdc.LISTINDEX>0
							.lstsdc.TOPINDEX=.lstsdc.LISTINDEX
						ENDIF
						.xfound=.T.
						NODEFAULT
						EXIT
					ELSE
						.xfound=.F.
					ENDIF
				NEXT x
			ENDIF
*-------------Backspace
			IF nkeycode=127 AND THIS.SELSTART > 0
				FOR x = 1 TO .lstsdc.LISTCOUNT
					IF UPPER(SUBS(.lstsdc.LIST(x), 1, THIS.SELSTART-1)) = ;
							UPPER(SUBS(THIS.TEXT, 1, THIS.SELSTART-1))
						ncurpos = THIS.SELSTART - 1
						THIS.VALUE = SUBSTR(.lstsdc.LIST(x),1,ncurpos)
						THIS.SELSTART = ncurpos
						.lstsdc.DISPLAYVALUE=.lstsdc.LIST(x)
						IF .lstsdc.LISTINDEX>0
							.lstsdc.TOPINDEX=.lstsdc.LISTINDEX
						ENDIF
						.xfound=.T.
						NODEFAULT
						EXIT
					ELSE
						.xfound=.F.
					ENDIF
				NEXT x
			ENDIF
*-------------Up Arrow Key
			IF nkeycode=5
				IF .lstsdc.LISTINDEX-1>0
					.lstsdc.LISTINDEX=.lstsdc.LISTINDEX-1
					.lstsdc.SELECTED=.lstsdc.LISTINDEX
					THIS.VALUE=.lstsdc.VALUE
					.xfound=.T.
					NODEFA
					RETU
				ENDIF
				NODEFA
			ENDIF
*-------------Down Arrow Key
			IF nkeycode=24
				IF .lstsdc.LISTINDEX+1<=.lstsdc.LISTCOUNT
					.lstsdc.LISTINDEX=.lstsdc.LISTINDEX+1
					.lstsdc.SELECTED=.lstsdc.LISTINDEX
					THIS.VALUE=.lstsdc.VALUE
					.xfound=.T.
					NODEFA
					RETU
				ENDIF
				NODEFA
			ENDIF
*-----------Enter Key / Tab Key
			IF nkeycode=13 OR nkeycode=9
				IF .lstsdc.LISTINDEX>0
					IF .xfound=.T.
						THIS.VALUE=.lstsdc.LIST(.lstsdc.LISTINDEX)
						.lstsdc.VISIBLE=.F.
						THIS.LOSTFOCUS
					ENDIF
				ENDIF
			ENDIF
*--------Left Arrow Key
			IF nkeycode=19
				IF THIS.SELSTART=0
					NODEFA
					RETU
				ENDIF
			ENDIF
		ENDWITH
	ENDIF
ENDIF

ENDPROC
PROCEDURE GotFocus
If (Thisform.Addmode Or Thisform.Editmode)
	If Inlist(Left(Upper(This.Defahelp),4),'SELE','EXEC') 		&&Rup
		sql_con = 0
		sql_con = Thisform.SqlConObj.DataConn([EXE],Company.DbName,This.Defahelp,[_xtrtblw],"This.Parent.nHandle",;
			Thisform.DataSessionId)
		If sql_con > 0 And Used('_xtrtblw')
			Select _xtrtblw
			_sdcflds = Field(1)
**** Added By Sachin N. S. on 15/11/2011 for Bug-437 **** Start
			If Len(&_sdcflds)>230
				_sdcflds = 'LEFT('+_sdcflds+',230)'
			Endif
**** Added By Sachin N. S. on 15/11/2011 for Bug-437 **** End
			Index On &_sdcflds Tag ListTbl
			Thisform.ListTbl = [_xtrtblw]
			This.HelpType    = 'COMMANDBUTTON'
		Else
			Thisform.ShowMessageBox("No Records Found!",0+32,vumess)
		Endif
		sql_con = Thisform.SqlConObj.SqlConnClose("This.Parent.nHandle")
	Endif
	If (!Empty(This.Defahelp) And !Inlist(Left(Upper(This.Defahelp),4),'SELE','EXEC')) 	&&Rup
		This.HelpType    = 'LISTBOX'
	Endif
&&vasant250410
	If Empty(This.Tag)
		This.Tag = Iif(Empty(This.Value),'*#*',Transform(This.Value))
	Endif
&&vasant250410
	This.Message(.T.)
Endif

ENDPROC
PROCEDURE SetFocus
*!*	IF Thisform.Addmode OR Thisform.Editmode
*!*		Thisform.SetFs		 = .t.
*!*		Thisform.SetValue    = 1
*!*	Endif	

ENDPROC
PROCEDURE LostFocus
If (Thisform.Addmode Or Thisform.Editmode)
	This.Enabled= .T.
	If Thisform.FlagCancel=.t. OR thisform.curonmouse = .t.
		Return
	ENDIF
	_curfld = JUSTEXT(This.ControlSource)
  	_curtbl = JUSTSTEM(This.ControlSource)
	SELECT (_curtbl)
	_currec = IIF(!EOF(),RECNO(),0)
	IF (!EMPTY(This.Defahelp) AND !INLIST(LEFT(UPPER(This.Defahelp),4),'SELE','EXEC')) &&Rup
		sql_fnd = .f.
		_sdcflds1 = ','+ALLTRIM(This.Defahelp)+','
		_sdcflds2 = ','+ALLTRIM(this.value)+','
		if AT(_sdcflds2,_sdcflds1) > 0
			sql_fnd = .t.
		endif	
		IF sql_fnd = .f.		
			Thisform.ShowMessageBox("Not Found in List",0+32,vumess) && changes by sandeep for the bug-3800/27-Apr-12
			SELECT (_curtbl)
			NoDefault
			If _currec > 0
				GoTo _currec
			EndIf
			RETURN This.Value
		ENDIF
	ENDIF
	IF !EMPTY(this.validexpr)
		_curval = EVAL(this.validexpr)
		IF TYPE('_curval') = 'L'
			IF _curval = .f.
				&&Rup
*!*					IF EMPTY(this.cerrmsg)
*!*						_curmes = 'Invalid Input'
*!*					Else	
*!*						_curmes = ALLTRIM(EVAL(this.cerrmsg))
*!*					Endif	
				IF !EMPTY(this.cerrmsg)
					_curmes = ALLTRIM(EVAL(this.cerrmsg))
					Thisform.ShowMessageBox(_curmes,0+32,vumess)
				ENDIF
				&&Rup	
				SELECT (_curtbl)
				NoDefault
				If _currec > 0
					GoTo _currec
				ENDIF
				Retu This.Value
			Endif	
		Endif	
	ENDIF
	Thisform.GrdCmdSdcLf()
	Thisform.GrdLstSdcLf()
	Thisform.ListTbl = []		
	IF USED('_xtrtblw')
		USE IN _xtrtblw
	Endif	
	IF USED('_xtrtblv')
		USE IN _xtrtblv
	Endif	
	IF USED('_lxtrtbl')
		USE IN _lxtrtbl
	Endif	
ENDIF

ENDPROC
0   m                   PLATFORM   C                  UNIQUEID   C	   
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
prgcurrmast.prg c:\users\shrikant\appdata\local\temp\ prgcurrmast.fxp .\ frmcntrmast.scx frmcntrmast.sct e:\u3\udyoggstsdk\class\ standardui.vcx standardui.vct datepicker.vcx datepicker.vct vouclass.vcx vouclass.vct ctl32_progressbar.vcx ctl32_progressbar.vct  )   =     6           	=  �
  F   I           �
  �b  F   Y           �b  �j  i   �           �j  v�  i   �           v�  ��  i   �           ��  %9 i   �           %9 �J i   �           �J |e i   �           |e �l i   �           �l �K i   �           