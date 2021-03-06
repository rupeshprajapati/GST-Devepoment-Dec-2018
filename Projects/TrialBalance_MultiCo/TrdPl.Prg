Define Class TrdPl As Custom
	ParentObj = ''
	leftcursor = 'L'+Sys(3)		&& Trading Accounts
	Rightcursor = 'R'+Sys(3)	&& Profit And Loss Accounts

	Procedure CurCreation
	Lparameters CurType As String
	Do Case
	Case CurType = 'L'
		Create Cursor (This.leftcursor) (;
			LAC_NAME2 C(54),LExpand l,LExpanded C(1),LORDERLEVEL C(50),LLevel I(4),LMAINFLG C(1),;
			LAC_ID N(20,0),LAC_GROUP_ID N(20,0),LAC_NAME C(50),LGroup C(50),LCLBAL N(19,2),;
			RAC_NAME2 C(54),RExpand l,RExpanded C(1),RORDERLEVEL C(50),RLevel I(4),RMAINFLG C(1),RAC_ID N(20,0),;
			RAC_GROUP_ID N(20,0),RAC_NAME C(50),RGroup C(50),RCLBAL N(19,2), DISPTYPE C(3))
	Case CurType = 'R'
		Create Cursor (This.Rightcursor) (;
			LAC_NAME2 C(54),LExpand l,LExpanded C(1),LORDERLEVEL C(50),LLevel I(4),LMAINFLG C(1),;
			LAC_ID N(20,0),LAC_GROUP_ID N(20,0),LAC_NAME C(50),LGroup C(50),LCLBAL N(19,2),;
			RAC_NAME2 C(54),RExpand l,RExpanded C(1),RORDERLEVEL C(50),RLevel I(4),RMAINFLG C(1),RAC_ID N(20,0),;
			RAC_GROUP_ID N(20,0),RAC_NAME C(50),RGroup C(50),RCLBAL N(19,2), DISPTYPE C(3))
	Case CurType = 'B'
		Create Cursor (This.leftcursor) (;
			LAC_NAME2 C(54),LExpand l,LExpanded C(1),LORDERLEVEL C(50),LLevel I(4),LMAINFLG C(1),;
			LAC_ID N(20,0),LAC_GROUP_ID N(20,0),LAC_NAME C(50),LGroup C(50),LCLBAL N(19,2),;
			RAC_NAME2 C(54),RExpand l,RExpanded C(1),RORDERLEVEL C(50),RLevel I(4),RMAINFLG C(1),RAC_ID N(20,0),;
			RAC_GROUP_ID N(20,0),RAC_NAME C(50),RGroup C(50),RCLBAL N(19,2), DISPTYPE C(3))
		Create Cursor (This.Rightcursor) (;
			LAC_NAME2 C(54),LExpand l,LExpanded C(1),LORDERLEVEL C(50),LLevel I(4),LMAINFLG C(1),;
			LAC_ID N(20,0),LAC_GROUP_ID N(20,0),LAC_NAME C(50),LGroup C(50),LCLBAL N(19,2),;
			RAC_NAME2 C(54),RExpand l,RExpanded C(1),RORDERLEVEL C(50),RLevel I(4),RMAINFLG C(1),RAC_ID N(20,0),;
			RAC_GROUP_ID N(20,0),RAC_NAME C(50),RGroup C(50),RCLBAL N(19,2), DISPTYPE C(3))
	Endcase
	Endproc

	Procedure PandLcurupdate
	Lparameters mLOR As String ,CurName As String ,CFlg As String ,NLevel As Integer,NFltrLevel As Integer
*!*	mLOR 		: L/R i.e Left/Right
*!*	CurName		: CursorName Update Which
*!*	CFlg		: Flag for TradingI/E,PAndL I/E
*!*	NLevel		: Accounts Level

	Local oErr1 As Exception
	_mLOR = mLOR
	Try
		mLOR = Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T',mLOR,'L')
		Select(CurName)
		Go Top
		Do Case
		Case mLOR = 'L'
			Locate For Empty(LAC_NAME2)
			If ! Found()
				Append Blank
			Endif
		Case mLOR = 'R'
			Locate For Empty(RAC_NAME2)
			If ! Found()
				Append Blank
			Endif
		Endcase

		Laexact = Set("Exact")
		Set Exact On
		Select _TBAcMast
		If Type('NFltrLevel') = 'N'
			Set Filter To LevelFlg = CFlg And Between(Level,NLevel,NFltrLevel)
		Else
			Set Filter To LevelFlg = CFlg And Level = NLevel
		Endif
		If This.ParentObj.Shownull > 1
			mOldFilter = Set("Filter")
			Set Filter To (!Empty(opbal) Or !Empty(Debit) Or !Empty(Credit)) And &mOldFilter
		Endif
		mLastCursor = Alias()
		mTotalFlds = Fcount()
		Go Top
		Do While ! Eof()
			Select (mLastCursor)
			mFieldName = 'CLBAL'
			mFieldVal = Evaluate(mFieldName)
			_macName = &mLastCursor..ac_name		&& Added by Sachin N. S. on 17/07/2017 for GST
			_mOrdLevel = &mLastCursor..OrderLevel	&& Added by Sachin N. S. on 17/07/2017 for GST
			If mFieldVal <> 0
****** Added by Sachin N. S. on 17/07/2017 for GST -- Start
				Select (CurName)
				Go Top
				Locate For Evaluate(Iif(mLOR='L','L','R')+Alltrim("Ac_Name"))=_macName And Evaluate(Iif(mLOR='L','L','R')+Alltrim("OrderLevel"))=_mOrdLevel
				If !Found()
					Locate For Empty(Evaluate(Iif(mLOR='L','L','R')+Alltrim("Ac_Name")))
					If !Found()
						Append Blank
					Endif
				Endif
****** Added by Sachin N. S. on 17/07/2017 for GST -- End
				For I = 1 To mTotalFlds Step 1
					Select (mLastCursor)
					mFieldName = Iif(mLOR='L','L','R')+Allt(Field(I))
					mFieldVal = Evaluate(Field(I))
					If This.ParentObj.torvformat='V' And Allt(Field(I)) == 'LEVEL'
						mFieldVal = mFieldVal + 1
					Endif

					Select (CurName)
					If Inlist(mFieldName,mLOR+'AC_NAME2',mLOR+'ORDERLEVEL',mLOR+'LEVEL',mLOR+'MAINFLG',mLOR+'AC_ID',mLOR+'AC_GROUP_ID',mLOR+'AC_NAME',mLOR+'GROUP',mLOR+'CLBAL') = .T.
						If mFieldName = 'RCLBAL' Or (_mLOR='R' And This.ParentObj.torvformat='V' And mFieldName = 'LCLBAL')
							Replace (mFieldName) With Evaluate(mFieldName)+Iif((mFieldVal)<0,Abs(mFieldVal),Iif((mFieldVal)>0,-1*(mFieldVal),0)) In (CurName)
						Else
							If mFieldName=mLOR+'AC_NAME2' And This.ParentObj.torvformat='V'
								_Level = &mLastCursor..Level+1
								mFieldVal=Iif(_Level<>1,Space(_Level*2)+(mFieldVal),(mFieldVal))
							Endif

							If mFieldName = 'LCLBAL'
								Replace (mFieldName) With Evaluate(mFieldName)+(mFieldVal) In (CurName)
							Else
								Replace (mFieldName) With (mFieldVal) In (CurName)
							Endif
						Endif
					Endif
				Endfor
				Select (CurName)
				mFieldName = mLOR+'Expand'
				mFieldLevel = mLOR+'Level'
				MFldVal = This.ParentObj.tbrfinalac.TxtLevel.Value
				Replace (mFieldName) With Iif(Evaluate(mFieldLevel)=MFldVal,.T.,.F.) In (CurName)
				Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H','V')+_mLOR
				mFieldName = mLOR+'Expanded'
				Replace (mFieldName) With Iif(Evaluate(mFieldLevel)=MFldVal,'N','Y') In (CurName)
***** Added by Sachin N. S. on 18/07/2017 for GST -- Start
				Select (mLastCursor)
				If This.ParentObj.tbrfinalac.chkShwComp.Value=1 And ((Level=MaxLevel And Level<=MFldVal) Or (Level<MaxLevel And Level=MFldVal))
					Select (CurName)
					Go Top
					Locate For Empty(Evaluate(Iif(mLOR='L','L','R')+Alltrim("Ac_Name")))
					If !Found()
						Append Blank
					Endif
					For I = 1 To mTotalFlds Step 1
						Select (mLastCursor)
						mFieldName = Iif(mLOR='L','L','R')+Allt(Field(I))
						mFieldVal = Evaluate(Field(I))
						If This.ParentObj.torvformat='V' And Allt(Field(I)) == 'LEVEL'
							mFieldVal = mFieldVal + 1
						Endif

						mFieldVal = Icase(Upper(Field(I))='ORDERLEVEL',Alltrim(mFieldVal)+'.1',Upper(Field(I))='MAINFLG','D',Upper(Field(I))='AC_NAME2',&mLastCursor..AC_NAME3,mFieldVal)
						Select (CurName)
						If Inlist(mFieldName,mLOR+'AC_NAME2',mLOR+'ORDERLEVEL',mLOR+'LEVEL',mLOR+'MAINFLG',mLOR+'AC_ID',mLOR+'AC_GROUP_ID',mLOR+'AC_NAME',mLOR+'GROUP',mLOR+'CLBAL') = .T.
							If mFieldName = 'RCLBAL'
								Replace (mFieldName) With Evaluate(mFieldName)+Iif((mFieldVal)<0,Abs(mFieldVal),Iif((mFieldVal)>0,-1*(mFieldVal),0)) In (CurName)
							Else
								If mFieldName=mLOR+'AC_NAME2' And This.ParentObj.torvformat='V'
									_Level = &mLastCursor..Level+1
									mFieldVal=Iif(_Level<>1,Space(_Level*2)+(mFieldVal),(mFieldVal))
								Endif

								If mFieldName = 'LCLBAL'
									Replace (mFieldName) With Evaluate(mFieldName)+(mFieldVal) In (CurName)
								Else
									Replace (mFieldName) With (mFieldVal) In (CurName)
								Endif
							Endif
						Endif
					Endfor
					Select (CurName)
					mFieldName = mLOR+'Expand'
					mFieldLevel = mLOR+'Level'
					MFldVal = This.ParentObj.tbrfinalac.TxtLevel.Value
					mlShwComp = This.ParentObj.tbrfinalac.chkShwComp.Value
					Replace (mFieldName) With Iif(Evaluate(mFieldLevel)=MFldVal And mlShwComp=1,.T.,.F.) In (CurName)
					Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H','V')+_mLOR
					mFieldName = mLOR+'Expanded'
					Replace (mFieldName) With Iif(Evaluate(mFieldLevel)=MFldVal And mlShwComp=1,'N','Y') In (CurName)
				Endif
***** Added by Sachin N. S. on 18/07/2017 for GST -- End

				Select (CurName)
				Go Top
				Locate For Empty(Evaluate(Iif(mLOR='L','L','R')+Alltrim("Ac_Name")))
				If !Found()
					Append Blank
				Endif
			Endif
			Select (mLastCursor)
			Skip
		Enddo
		Set Exact &Laexact
		Select _TBAcMast
		Set Filter To
		Select(CurName)
	Catch To oErr1
		Thisform.errorhandler(oErr1)
	Endtry
	Endproc

	Procedure UpdateGP
	Select (This.leftcursor)
	If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T'
		Sum LCLBAL To m.dr For LLevel = 1 And LAC_NAME <> "GROSS PROFIT c/d" And LMAINFLG!='D'
	Else
		Sum LCLBAL To m.dr For LLevel = 1 And LAC_NAME <> "GROSS PROFIT c/d" And (Substr(Alltrim(DISPTYPE),2,1)='L') And Deleted()=.F.  And LMAINFLG!='D'
	Endif

	If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T'
		Sum RCLBAL To m.cr For RLevel = 1 And RAC_NAME <> "GROSS LOSS c/d" And RMAINFLG!='D'
	Else
		Sum RCLBAL To m.cr For RLevel = 1 And RAC_NAME <> "GROSS LOSS c/d" And (Substr(Alltrim(DISPTYPE),2,1)='R') And Deleted()=.F. And RMAINFLG!='D'
	Endif

	Do Case
	Case m.cr > m.dr
		m.diff = m.cr - m.dr
		Locate For Empty(LAC_NAME2)
		If ! Found()
			Append Blank
		Endif
		Replace LAC_NAME2 With "GROSS PROFIT c/d",;
			LExpand With .F.,;
			LExpanded With 'Y',;
			LORDERLEVEL With '',;
			LLevel With 1,;
			LMAINFLG With 'L',;
			LAC_ID With 1,;
			LAC_GROUP_ID With 2,;
			LAC_NAME With "GROSS PROFIT c/d",;
			LGroup With "GROSS PROFIT c/d",;
			LCLBAL With Abs(m.diff)
		Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H'+'L2','V'+'Z2'),  LORDERLEVEL With 'Z'
	Case m.dr > m.cr
		m.diff = m.dr-m.cr
		If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T' 	&& Added By Sachin N. S. on 03/07/2012 for Bug-4539
			Locate For Empty(RAC_NAME2)
			If ! Found()
				Append Blank
			Endif
			Replace RAC_NAME2 With "GROSS LOSS c/d",;
				RExpand With .F.,;
				RExpanded With 'Y',;
				RORDERLEVEL With '',;
				RLevel With 1,;
				RMAINFLG With 'L',;
				RAC_ID With 1,;
				RAC_GROUP_ID With 2,;
				RAC_NAME With "GROSS LOSS c/d",;
				RGroup With "GROSS LOSS c/d",;
				RCLBAL With Abs(m.diff)
		Else
			Locate For Empty(LAC_NAME2)
			If ! Found()
				Append Blank
			Endif
			Replace LAC_NAME2 With "GROSS LOSS c/d",;
				LExpand With .F.,;
				LExpanded With 'Y',;
				LORDERLEVEL With '',;
				LLevel With 1,;
				LMAINFLG With 'L',;
				LAC_ID With 1,;
				LAC_GROUP_ID With 2,;
				LAC_NAME With "GROSS LOSS c/d",;
				LGroup With "GROSS LOSS c/d",;
				LCLBAL With Abs(m.diff)
		Endif
		Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H'+'R2','V'+'Z2'),  LORDERLEVEL With 'Z'

	Endcase

	Select (This.Rightcursor)		&& == Carry Forward Gpl
	Do Case
	Case m.cr > m.dr				&& == Income is more than expense
		m.diff = m.cr - m.dr
		Append Blank
		If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T'
			Replace RAC_NAME2 With "GROSS PROFIT b/d",;
				RExpand With .F.,;
				RExpanded With 'Y',;
				RORDERLEVEL With '',;
				RLevel With 1,;
				RMAINFLG With 'L',;
				RAC_ID With 1,;
				RAC_GROUP_ID With 2,;
				RAC_NAME With "GROSS PROFIT b/d",;
				RGroup With "GROSS PROFIT b/d",;
				RCLBAL With Abs(m.diff)
		Else
			Replace LAC_NAME2 With "GROSS PROFIT b/d",;
				LExpand With .F.,;
				LExpanded With 'Y',;
				LORDERLEVEL With '',;
				LLevel With 1,;
				LMAINFLG With 'L',;
				LAC_ID With 1,;
				LAC_GROUP_ID With 2,;
				LAC_NAME With "GROSS PROFIT b/d",;
				LGroup With "GROSS PROFIT b/d",;
				LCLBAL With Abs(m.diff)
		Endif
		Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H'+'R1','V'+'A')
		cLvlFlg = This.ParentObj.findlevelflg('INCOME')
		Replace LORDERLEVEL With cLvlFlg In (This.Rightcursor)

	Case m.dr > m.cr				&& == Expense is greater than income
		m.diff = m.dr-m.cr
		Append Blank
		Replace LAC_NAME2 With "GROSS LOSS  b/d",;
			LExpand With .F.,;
			LExpanded With 'Y',;
			LORDERLEVEL With '',;
			LLevel With 1,;
			LMAINFLG With 'L',;
			LAC_ID With 1,;
			LAC_GROUP_ID With 2,;
			LAC_NAME With "GROSS LOSS  b/d",;
			LGroup With "GROSS LOSS  b/d",;
			LCLBAL With Abs(m.diff)
		Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H'+'L1','V'+'B')
		cLvlFlg = This.ParentObj.findlevelflg('EXPENSE')
		Replace LORDERLEVEL With cLvlFlg In (This.Rightcursor)

	Otherwise
		Append Blank
	Endcase
	Endproc

	Procedure UpdateNP
	Select (This.Rightcursor)
		
	If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T'
		Sum LCLBAL To m.dr For LLevel = 1 And LAC_NAME <> "NET PROFIT b/d" And LMAINFLG!='D'
	Else
		Sum LCLBAL To m.dr For LLevel = 1 And LAC_NAME <> "NET PROFIT b/d" And (Substr(Alltrim(DISPTYPE),2,1)='L' Or Substr(Alltrim(DISPTYPE),2,1)='B') And Deleted()=.F. And LMAINFLG!='D'
	Endif

	If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T'
		Sum RCLBAL To m.cr For RLevel = 1 And RAC_NAME <> "NET LOSS b/d" And RMAINFLG!='D'
	Else
		Sum RCLBAL To m.cr For RLevel = 1 And RAC_NAME <> "NET LOSS b/d" And (Substr(Alltrim(DISPTYPE),2,1)='R' Or Substr(Alltrim(DISPTYPE),2,1)='A') And Deleted()=.F. And RMAINFLG!='D'
	Endif

	Go Top
	Do Case
	Case m.cr > m.dr			&& ====== INCOME IS GREATER THAN EXPENSE
		m.diff =  m.cr - m.dr
		Locate For Empty(LAC_NAME2)
		If ! Found()
			Append Blank
		Endif
		Replace LAC_NAME2 With "NET PROFIT b/d",;
			LExpand With .F.,;
			LExpanded With 'Y',;
			LORDERLEVEL With '',;
			LLevel With 1,;
			LMAINFLG With 'L',;
			LAC_ID With 1,;
			LAC_GROUP_ID With 2,;
			LAC_NAME With "NET PROFIT b/d",;
			LGroup With "NET PROFIT b/d",;
			LCLBAL With Abs(m.diff)
		Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H'+'L2','V'+'Z2'),  LORDERLEVEL With 'Z'

	Case m.dr > m.cr			&& ===== EXPENSE IS GREATER THAN INCOME
		m.diff = m.dr-m.cr
		If Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T' 	&& Added By Sachin N. S. on 03/07/2012 for Bug-4539
			Locate For Empty(RAC_NAME2)
			If ! Found()
				Append Blank
			Endif
			Replace RAC_NAME2 With "NET LOSS b/d",;
				RExpand With .F.,;
				RExpanded With 'Y',;
				RORDERLEVEL With '',;
				RLevel With 1,;
				RMAINFLG With 'L',;
				RAC_ID With 1,;
				RAC_GROUP_ID With 2,;
				RAC_NAME With "NET LOSS b/d",;
				RGroup With "NET LOSS b/d",;
				RCLBAL With -Abs(m.diff)
		Else
			Locate For Empty(LAC_NAME2)
			If ! Found()
				Append Blank
			Endif
			Replace LAC_NAME2 With "NET LOSS b/d",;
				LExpand With .F.,;
				LExpanded With 'Y',;
				LORDERLEVEL With '',;
				LLevel With 1,;
				LMAINFLG With 'L',;
				LAC_ID With 1,;
				LAC_GROUP_ID With 2,;
				LAC_NAME With "NET LOSS b/d",;
				LGroup With "NET LOSS b/d",;
				LCLBAL With Abs(m.diff)
		Endif
		Replace DISPTYPE With Iif(Empty(This.ParentObj.torvformat) Or This.ParentObj.torvformat='T','H'+'R2','V'+'Z2'),  LORDERLEVEL With 'Z'


	Endcase
	Endproc

	Procedure findlevelflg
	Parameters mAc_Name
	Local oErr1 As Exception
	Try
		Select A.LevelFlg;
			FROM _TBAcMast A;
			WHERE A.ac_name = mAc_Name;
			INTO Cursor findlevelflg
		Go Top
	Catch To oErr1
		Thisform.errorhandler(oErr1)
	Endtry
	Return LevelFlg
	Endproc

	Procedure errorhandler
	Lparameters ErrorObj As Object
	Local StingMess As String
	tingMess = ''
	StingMess = StingMess +Chr(13)+[  Error: ] + Str(ErrorObj.ErrorNo)
	StingMess = StingMess +Chr(13)+[  LineNo: ] + Str(ErrorObj.Lineno)
	StingMess = StingMess +Chr(13)+[  Message: ] + ErrorObj.Message
	StingMess = StingMess +Chr(13)+[  Procedure: ] + ErrorObj.Procedure
	StingMess = StingMess +Chr(13)+[  Details: ] + ErrorObj.Details
	StingMess = StingMess +Chr(13)+[  StackLevel: ] + Str(ErrorObj.StackLevel)
	StingMess = StingMess +Chr(13)+[  LineContents: ] + ErrorObj.LineContents
	StingMess = StingMess +Chr(13)+[  UserValue: ] + ErrorObj.UserValue
	Messagebox(StingMess,0+64,VuMess)
	Endproc

Enddefine
