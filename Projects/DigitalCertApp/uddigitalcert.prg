Lparameters pPdfPath,pDept,pCat,pInv_sr,pEntry_ty,pDate
*!*	IF VARTYPE(Company) <> "O"
*!*		RETURN .F.
*!*	ENDIF
*!*	IF PARAMETERS() < 1
*!*		=MESSAGEBOX('Pass Valid Parameters',0,"")
*!*		RETURN .T.
*!*	ENDIF
oWSHELL = Createobject("WScript.Shell")
If Vartype(oWSHELL) <> "O"
	Messagebox("WScript.Shell Object Creation Error...",16,VuMess)
	Return .F.
Endif

Declare Integer GetCurrentProcessId  In kernel32
pid=GetCurrentProcessId()
tcCompId = Company.CompId
tcCompdb =Company.Dbname
tcCompNm=Company.co_name
SqlConObj = Newobject('SqlConnUdObj','SqlConnection',xapps)

*!*	mvu_user1 = SqlConObj.dec(SqlConObj.ondecrypt(mvu_user))		&& Commented by Shrikant S. on 26/04/2018 for Bug-31488
*!*	mvu_pass1 = SqlConObj.dec(SqlConObj.ondecrypt(mvu_pass))		&& Commented by Shrikant S. on 26/04/2018 for Bug-31488
&& Added by Shrikant S. on 26/04/2018 for Bug-31488		&& Start
If Type('mvu_nenc')='U'
	mvu_nenc=0
Endif
If mvu_nenc=1
	mvu_user1 =SqlConObj.newdecry(Strconv(mvu_user,16),"Ud*yog+1993Uid")
	mvu_pass1 =SqlConObj.newdecry(Strconv(mvu_pass,16),"Ud*yog+1993Pwd")
Else
	mvu_user1 = SqlConObj.dec(SqlConObj.ondecrypt(mvu_user))
	mvu_pass1 = SqlConObj.dec(SqlConObj.ondecrypt(mvu_pass))
Endif
&& Added by Shrikant S. on 26/04/2018 for Bug-31488		&& End


vicopath=Strtran(icopath,' ','<*#*>')
pPdfPath=Strtran(pPdfPath,' ','<*#*>')   &&added by Prajakta B. on 30032018 for Bug 31386
pApplCaption=Strtran(VuMess,' ','<*#*>')
pDept=Strtran(pDept,' ','<*#*>')
pInv_sr=Strtran(pInv_sr,' ','<*#*>')
pCat=Strtran(pCat,' ','<*#*>')
pDate=Ttod(pDate)

pApplName=Justfname(Sys(16))

_ShellExec ="udPdfSignature.exe " +Transform(pPdfPath)+" "+Transform(tcCompId)+" "+Alltrim(tcCompdb)+" "+Alltrim(mvu_server)+" "+Alltrim(mvu_user1)+" "+Alltrim(mvu_pass1)+" "+Alltrim(pDept)+" "+Alltrim(pCat)+" "+Alltrim(pInv_sr)+" "+Alltrim(pEntry_ty)+" "+Transform(pDate)
_ShellExec = _ShellExec+" "+Alltrim(pApplCaption)+" "+Alltrim(pApplName)+" "+Alltrim(Str(pApplId))  +" "+Alltrim(pApplCode)	&& Added by Sachin N. S. on 18/09/2015 for Bug-26664


oWSHELL.Exec(_ShellExec)
SqlConObj = Null
mvu_user1 = Null
mvu_pass1 = Null
Release SqlConObj,mvu_user1,mvu_pass1

