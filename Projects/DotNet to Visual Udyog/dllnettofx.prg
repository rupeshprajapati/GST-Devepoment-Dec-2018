Define Class ClNetToFx As Session OlePublic

	Procedure PrNetToFx(_mCoDbName As String,_mCoSta_dt As String,_mCoEnd_dt As String,_mLogUserName As String,_mAppDefaPath As String,_mAppPath As String,_mIcoPath As String,_mProdCode As String,_mProgName As String)
	_MainScrnDataSession = Set("Datasession")
	Set DataSession To _MainScrnDataSession

	Application.Visible = .F.
	_vfp.Visible        = .F.
	_Screen.Visible     = .F.
	Set Safety Off
	Set Multilocks On
	Set Deleted On
	Set Century On
	Set Date To british
	Set Resource Off
	Set Talk Off
	Set Scoreboard Off
	Set Escape Off
	Set Exclusive Off
	Set Exact Off
	Set Clock Status
	Set Multilocks On
	Set Resource Off
	Set Help On

	Public apath,mvu_backend,mvu_server,mvu_user,mvu_pass,chqcon,musername,vchkprod,;
		icopath,company,coadditional,forceexit,tbrdesktop,statdesktop,exitclick,exitonce,vumess,treedesktop,;
		addbutton1,editbutton1,printbutton1,deletebutton1,xapps,mvu_MenuType,_Defaultdb,;
		GlobalObj,_CoDbName,_CoSta_dt,_CoEnd_dt,_CoProgName,UdNewTrigEnbl,oGlblPrdFeat,ueReadRegMe

	apath			= _mAppDefaPath
	mvu_backend		= 0
	mvu_server		= ''
	mvu_user		= ''
	mvu_pass		= ''
	chqcon			= 0
	musername		= _mLogUserName
	vchkprod		= _mProdCode
	icopath			= _mIcoPath
	forceexit		= .F.
	exitclick		= .F.
	exitonce		= .F.
	vumess			= ''
	addbutton1		= .F.
	editbutton1		= .F.
	printbutton1	= .F.
	deletebutton1	= .F.
	xapps			= ''
	mvu_MenuType	= ''
	_Defaultdb		= 'Vudyog'
	_CoDbName		= _mCoDbName
*_CoSta_dt		= CTOT(_mCoSta_dt)
*_CoEnd_dt		= CTOT(_mCoEnd_dt)
	If Type('_mCoSta_dt') = 'C'
		_mCoSta_dt	= Ctot(_mCoSta_dt)
		_mCoEnd_dt	= Ctot(_mCoEnd_dt)
	Endif
	_CoSta_dt		= _mCoSta_dt
	_CoEnd_dt		= _mCoEnd_dt
	_CoProgName		= _mProgName
	UdNewTrigEnbl	= .F.

	Declare Integer GetPrivateProfileString In Win32API As GetPrivStr ;
		string cSection, String cKey, String cDefault, String @cBuffer, ;
		integer nBufferSize, String cINIFile
	Declare Integer WritePrivateProfileString In Win32API As WritePrivStr ;
		string cSection, String cKey, String cValue, String cINIFile
	Declare Integer GetProfileString In Win32API As GetProStr ;
		string cSection, String cKey, String cDefault, ;
		string @cBuffer, Integer nBufferSize
	Declare Integer Beep In kernel32 Integer pn_Freq,Integer pn_Duration

	Local iniFilePath
	iniFilePath = apath+"visudyog.ini"
	If !File(iniFilePath)
		Messagebox("Configuration file not found",16,vumess)
		Retu .F.
	Endif
	Public mvu_nenc				&& Added by Shrikant S. on 25/04/2018 for Bug-31488


	mvu_one     = Space(2000)
	mvu_two     = 0
	mvu_two	    = GetPrivStr([Settings],"NENC", "", @mvu_one, Len(mvu_one), apath + "visudyog.ini")			&& Added by Shrikant S. on 25/04/2018 for Bug-31488
	mvu_nenc	= Left(mvu_one,mvu_two)																		&& Added by Shrikant S. on 25/04/2018 for Bug-31488
	mvu_two	    = GetPrivStr([Settings],"Backend", "", @mvu_one, Len(mvu_one), iniFilePath)
	mvu_backend = Left(mvu_one,mvu_two)
	mvu_two     = GetPrivStr([DataServer],"Name", "", @mvu_one, Len(mvu_one), iniFilePath)
	mvu_server  = Left(mvu_one,mvu_two)

&& Added by Shrikant S. on 25/04/2018 for Bug-31488			&& Start
	IF TYPE('mvu_nenc')='U'
		mvu_nenc=0
	endif
	If !Empty(mvu_nenc)
		mvu_nenc=Val(Transform(mvu_nenc))
	Else
		mvu_nenc=0
	Endif
&& Added by Shrikant S. on 25/04/2018 for Bug-31488			&& End


	mvu_two     = GetPrivStr([DataServer],onencrypt(enc("User")), "", @mvu_one, Len(mvu_one), iniFilePath)
	mvu_user    = Left(mvu_one,mvu_two)
	mvu_two     = GetPrivStr([DataServer],onencrypt(enc("Pass")), "", @mvu_one, Len(mvu_one), iniFilePath)
	mvu_pass    = Left(mvu_one,mvu_two)
	mvu_backend = Iif(Empty(mvu_backend),"0",mvu_backend)
	mvu_two		= GetPrivStr([Settings],"Title", "", @mvu_one, Len(mvu_one), iniFilePath)
	lcvumess  	= Left(mvu_one,mvu_two)
	vumess      = Iif(Empty(lcvumess),vumess,lcvumess)
	mvu_two		= GetPrivStr([Settings],"XFile", "", @mvu_one, Len(mvu_one), iniFilePath)
	xapps	  	= Left(mvu_one,mvu_two)

	Set Default To &_mAppDefaPath
	Set Path To &_mAppPath
	Set Procedure To vu_udfs Additive
	Set Classlib To sqlConnection In &xapps Additive
	Set Classlib To vutool In &xapps Additive
	Set Classlib To UdGeneral.vcx  In &xapps Additive

	GlobalObj = Createobject("UdGeneral")
	If Type('GlobalObj') != 'O'
		Retu .F.
	Endif

	ueReadRegMe = Createobject("ueReadRegisterMe")
	If Type('ueReadRegMe') != 'O'
		=Messagebox("Registration details not found.",0+16,vumess)
		Return .F.
	Endif
	_UnqVal = GlobalObj.getPropertyval('UnqVal')
	_UnqVal = Substr(Dec(_UnqVal),2,8)
	ueReadRegMe._ueReadRegisterMe(apath,_UnqVal)

	oGlblPrdFeat = Createobject("UdProductFeature",vchkprod)

	vumess = GlobalObj.getPropertyval("vumess")
	If mvu_backend # "0"
		Do Case
		Case Empty(mvu_server)
			Messagebox("ERROR !!!, Data server not defined",32,vumess)
			Retu .F.
		Case Empty(mvu_user)
			Messagebox("ERROR !!!, User name not defined",32,vumess)
			Retu .F.
		Endcase
	Endif

	Do Form frmNetToFx.scx With _MainScrnDataSession
	Read Events

	Endproc

Enddefine
