Parameters oControl

sql_con = 0
oactive=_Screen.ActiveForm
If Type('oactive.pcvtype')<>'U'
	If Inlist(oactive.pcvtype,"PK","SK","MO","IU","PU","SU","SV","IE","PG","PX" )
*!*			lcfld=Justext(oControl.ControlSource)	&& Commented by Shrikant S. on 14/12/2018 for Installer 2.1.0
		If Type('oControl')='C'
			lcfld=oControl
		Else
			lcfld=Justext(oControl.ControlSource)
		Endif
		If (oactive.addmode Or oactive.editmode) And Upper(Alltrim(lcfld))=="BATCHNO"

			msqlstr="SELECT BatchNo, MfgDt, ExpDt,BatchRate,Inc_gst,Qty,MRPInc_gst FROM pRetBatchDetails WHERE it_code = ?item_vw.it_code and BatchonHold = 0  "
			If !Inlist(oactive.pcvtype,'SV','SU','PU')
				msqlstr=msqlstr+" "+" AND  ?Main_vw.Date BETWEEN MFGDT AND EXPDT "
			Endif
			If !Inlist(oactive.pcvtype,'PK','MO')
				msqlstr=msqlstr+" "+" and Qty >0 "
			Endif

			If !Empty(Item_vw.Ware_nm)
				msqlstr=msqlstr+" "+" and StoreNm=?Item_vw.Ware_nm "
			Endif

			msqlstr=msqlstr+" "+" Order by Expdt Asc,Batchno Asc "			&& added by Shrikant S. on 11/12/2018 for Installer 2.1.0.0
*!*				msqlstr=msqlstr+" "+" Order by Expdt Desc,Batchno Asc "		&& Commented by Shrikant S. on 11/12/2018 for Installer 2.1.0.0
			lcCaption = 'Select Batchno'
			If Type('oControl')='O'
				lcFieldValue = oControl.Value
			Else
				lcFieldValue =""
			Endif
			lcField = 'BatchNo'
			lcFields = 'BatchNo,MfgDt,ExpDt,BatchRate,Inc_gst,Qty,MRPInc_gst'
			lcExfld = ''
			lcFldsCaption = [BatchNo:Batch No.,MfgDt:Mfg. Date,ExpDt:Exp. Date,Qty:Available Stock,BatchRate:MRP]
			lcFldRtrn =[BatchNo,MfgDt,ExpDt,BatchRate,Inc_gst,MRPInc_gst]
			lcfldxdisp=[Inc_gst,MRPInc_gst]
			sql_con = oactive.sqlconobj.dataconn([EXE],company.dbname,msqlstr,[_lxtrtbl],"oactive.nHandle",oactive.DataSessionId)
			If sql_con > 0 And Used('_lxtrtbl')
				Select _lxtrtbl
				macname = []

				lcGPopVal=Uegetpop('_lxtrtbl',lcCaption,lcField,lcFields,lcFieldValue,'','','',.T.,lcFldRtrn,lcFields,lcFldsCaption,lcfldxdisp)
				If Vartype(lcGPopVal)="O"
					If Type('oControl')='O'
						oControl.Value = lcGPopVal.BatchNo
					Endif
					Replace BatchNo With lcGPopVal.BatchNo,mfgdt With lcGPopVal.mfgdt,expdt With lcGPopVal.expdt,gstincl With lcGPopVal.Inc_gst In Item_vw
*,rate With Iif(lcGPopVal.batchrate >0 And Item_vw.rate <=0 ,lcGPopVal.batchrate,rate )
					If Empty(Item_vw.rate)
						oactive.Voupage.Page1.Grditem.Column3.Text1.Value = 0
						oactive.Voupage.Page1.Grditem.Column3.Text1.Value=oactive.ItemRateactual(Item_vw.Item,'P')
					Endif
					If Inlist(oactive.pcvtype,'SK')
						Replace gstincl With lcGPopVal.MRPInc_gst In Item_vw
					Endif

					msqlstr="Select Top 1 ConvRatio from Groupuom Where SubUom=?Item_vw.gpunit and baseuom=?Item_vw.stkUnit "
					sql_con = oactive.sqlconobj.dataconn([EXE],company.dbname,msqlstr,[_lxtrtbl],"oactive.nHandle",oactive.DataSessionId)
					If Used('_lxtrtbl') And Empty(Item_vw.gprate)
						Replace gprate With _lxtrtbl.convratio * Item_vw.rate In Item_vw
					Endif
				Endif
				If Lcode_vw.Inv_stk='-'
					If !oactive.stock_check_batchwise(Item_vw.It_code,Main_vw.Tran_cd,Item_vw.Ware_nm,{},"",Item_vw.BatchNo)
*						oactive.showmessagebox("Supply "+Alltrim(Item_vw.Item)+" for Batch no.: "+Alltrim(Item_vw.BatchNo)+" is OUT OF STOCK!!!",0+32,vumess)
						Replace BatchNo With "",mfgdt With {},expdt With {} In Item_vw
					Endif
				Endif
			Else
				oactive.showmessagebox("No Records Found!",0+32,vumess)
			Endif

			sql_con = oactive.sqlconobj.sqlconnclose("oactive.nHandle")
			If Used('_lxtrtbl')
				Use In _lxtrtbl
			Endif
		Endif
	Endif
Endif
