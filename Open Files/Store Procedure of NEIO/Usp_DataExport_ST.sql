If Exists(Select [name] From SysObjects Where xType='P' and [Name]='Usp_DataExport_ST')
Begin
	DROP PROCEDURE Usp_DataExport_ST
End
GO
CREATE Procedure [Usp_DataExport_ST]
@CommanPara varchar(4000)
as
Begin
	Declare @TablNm varchar(60),@FileType varchar(3),@ExpCode varchar(200),@ExpDataVol varchar(30),@DtFld varchar(30),@sDate Varchar(30),@eDate Varchar(30)
	Declare @TempTbl varchar(100)

	Set @TempTbl = '##DATAImp'+(SELECT substring(rtrim(ltrim(str(RAND( (DATEPART(mm, GETDATE()) * 100000 )
					+ (DATEPART(ss, GETDATE()) * 1000 )
					+ DATEPART(ms, GETDATE())) , 20,15))),3,20) as No)
	
	execute Usp_GetCommanParaVal @CommanPara,'TablNm',@TablNm out
	execute Usp_GetCommanParaVal @CommanPara,'FileType',@FileType out
	execute Usp_GetCommanParaVal @CommanPara,'ExpCode',@ExpCode out
	execute Usp_GetCommanParaVal @CommanPara,'ExpDataVol',@ExpDataVol  out
	execute Usp_GetCommanParaVal @CommanPara,'DtFld',@DtFld out
	execute Usp_GetCommanParaVal @CommanPara,'sDate',@sDate out
	execute Usp_GetCommanParaVal @CommanPara,'eDate',@eDate out
	
	PRINT @ExpCode
	 --select @TablNm ,@FileType ,@ExpCode ,@ExpDataVol,@DtFld ,@sDate ,@eDate 

	--execute Usp_GetCommanParaVal @CommanPara,'TablNm',@TablNm out
		
	Declare @SqlCommand nvarchar(4000)
	Declare @Manu_Det_Cmd nvarchar(4000)
	set @ExpCode=replace(@ExpCode,'`','''')
--Commented by Archana K. on 29/09/12 for Bug-5837 start
--	if  @TablNm='LITEMALL'
--	begin
--		if @FileType='xsd'
--		begin
--			set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode
----			set @SqlCommand=rtrim(@SqlCommand)+',manuAc_Name=b.Ac_Name,manuSAc_Name=c.Location_Id'
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,* FROM '+@TablNm
----			set @SqlCommand=rtrim(@SqlCommand)+' Left join ac_mast b on (a.ManuAc_ID=b.ac_id) left join shipto c on (a.ManuSAc_Id=c.Shipto_Id) where 1=2'
--			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--
--			print '1.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
----			set @SqlCommand='Select (select * from 	'+@TempTbl+' manu_det'	
----			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
----			execute sp_executesql @SqlCommand
----			print '2.'
----			print @SqlCommand
--		end
--		if @FileType='xml'
--		begin
----####### date field condition
--			if isnull(@DtFld,'')<>''
--			begin
--				set @ExpCode=replace(@ExpCode,'Tran_cd','a.Tran_cd')
--				set @ExpCode=replace(@ExpCode,'Entry_ty','a.Entry_ty')
--				print 'Exp Code : '+@ExpCode
--				set @SqlCommand='SELECT Dataexport1='+@ExpCode
--				set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=a.Tran_cd,a.* into '+@TempTbl+' FROM  '+@TablNm+' a'
--				set @SqlCommand=rtrim(@SqlCommand)+' inner join stmain b on (a.tran_cd=b.Tran_cd)'
--				set @SqlCommand=rtrim(@SqlCommand)+'  where '
--
--				if @ExpDataVol='Updated' 
--				begin
--					set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
--				end				
--				--*** date field condition
--					set @SqlCommand = rtrim(@SqlCommand)+ ' and (b.'+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
--				--*** 
--				set @SqlCommand = rtrim(@SqlCommand)+ ' and (a.Entry_ty in (''ST''))'
--				print '3.a'
--				print @SqlCommand
--
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from 	'+@TempTbl+' litemall'
--			end 
----#######  date field condition
--			else
--			begin
--				set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode
--	--			set @SqlCommand=rtrim(@SqlCommand)+',manuAc_Name=b.Ac_Name,manuSAc_Name=c.Location_Id '
--				set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,* FROM  '+@TablNm
--	--			set @SqlCommand=rtrim(@SqlCommand)+' Left join ac_mast b on (a.ManuAc_ID=b.ac_id) left join shipto c on (a.ManuSAc_Id=c.Shipto_Id)'
--				set @SqlCommand=rtrim(@SqlCommand)+'  where '
--
--				if @ExpDataVol='Updated' 
--				begin
--					set @SqlCommand=rtrim(@SqlCommand)+' isnull(DataExport,'''')='''' '
--				end
--				
--				set @SqlCommand = rtrim(@SqlCommand)+ ' and (Entry_ty in (''ST''))'
--				print '3.b'
--				print @SqlCommand
--			end 
--			
--
----			execute sp_executesql @SqlCommand
----			set @SqlCommand='Select (select * from 	'+@TempTbl+' manu_det'
--			set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--			print '4.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--		end
--	end		
--
--Commented by Archana K. on 29/09/12 for Bug-5837 end

	if @TablNm='STMAIN' 
	begin
		print 'b'
--Commented by Archana K. on 31/05/13 for Bug-5837 start
--		if @FileType='xsd'
--		begin
--			set @SqlCommand='SELECT Dataexport1='+@ExpCode
--			set @SqlCommand=rtrim(@SqlCommand)+',cons_Name=b.Ac_Name,Scons_Name=c.Location_Id,sAc_Name=d.Location_id '
--			--set @SqlCommand=rtrim(@SqlCommand)+',MANUAC_Name=E.Ac_Name,MANUSAC_Name=F.Location_Id '
--			--set @SqlCommand=rtrim(@SqlCommand)+',SUPPAC_Name=G.Ac_Name,SUPPSAC_Name=H.Location_Id '
----			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.cons_id=b.ac_id)'
--			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.scons_id=c.shipto_id)'
--			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto d on (a.scons_id=d.shipto_id)'
--			--set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast E on (a.MANUAC_id=E.ac_id)'
--			--set @SqlCommand=rtrim(@SqlCommand)+' left join shipto F on (a.MANUSAC_id=F.shipto_id)'
--			--set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast G on (a.SUPPAC_id=G.ac_id)'
--			--set @SqlCommand=rtrim(@SqlCommand)+' left join shipto H on (a.SUPPSAC_id=H.shipto_id) where 1=2'
----			print '5.'
----			print @SqlCommand
----			execute sp_executesql @SqlCommand
--			print '6.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--			print '7.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--		end
--Commented by Archana K. on 31/05/13 for Bug-5837 end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',cons_Name=b.Ac_Name,Scons_Name=c.Location_Id,sAc_Name=d.Location_id '
			--set @SqlCommand=rtrim(@SqlCommand)+',MANUAC_Name=E.Ac_Name,MANUSAC_Name=F.Location_Id '
			--set @SqlCommand=rtrim(@SqlCommand)+',SUPPAC_Name=G.Ac_Name,SUPPSAC_Name=H.Location_Id '
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.cons_id=b.ac_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.scons_id=c.shipto_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto d on (a.scons_id=d.shipto_id)'
			--set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast E on (a.MANUAC_id=E.ac_id)'
			--set @SqlCommand=rtrim(@SqlCommand)+' left join shipto F on (a.MANUSAC_id=F.shipto_id)'
			--set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast G on (a.SUPPAC_id=G.ac_id)'
			--set @SqlCommand=rtrim(@SqlCommand)+' left join shipto H on (a.SUPPSAC_id=H.shipto_id)'
			
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
		end
		print '8.'
		print @SqlCommand
		set @SqlCommand=@SqlCommand+' '+' and a.apgen=''YES'' '			--Added by Shrikant S. on 15/01/2019 for Bug-31784

		execute sp_executesql @SqlCommand

--Added by Priyanka B on 14072018 for Bug-31707 Start
	--Exclude fields From Table
		
		Declare @pos1 int, @ExclFld varchar(50), @fld_nm varchar(50)
		set @ExclFld=(select ExcludFld from exp_master where [Code]='ST')
		Print @ExclFld

		set @ExclFld=@ExclFld+','

		if  (@ExclFld<>'')
		begin
			while(charindex(',',@ExclFld)>0)
			begin
				set @pos1=charindex(',',@ExclFld)
				
				set @fld_nm=substring(@ExclFld,1,@pos1-1)
				set @SqlCommand='Alter table '+@TempTbl+@TablNm+' Drop column '+@fld_nm
				Print @SqlCommand
				execute sp_executesql @SqlCommand
				
				set @ExclFld=substring(@ExclFld,@pos1,len(@ExclFld))
				if (substring(@ExclFld,1,1)=',') 
				begin	
					set @ExclFld=substring(@ExclFld,2,len(@ExclFld)-1)
				end
			end
		end

	--Exclude fields From Table
--Added by Priyanka B on 14072018 for Bug-31707 End
		set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto, BINARY BASE64,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '9.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end
----****
	if @TablNm='STITEM' 
	begin
		print 'b'
--Commented by Archana K. on 31/05/13 for Bug-5837 start
--		if @FileType='xsd'
--		begin
--			set @SqlCommand='SELECT Dataexport1='+@ExpCode
--			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
----			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id) where 1=2'
--
--			print '10.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--			print '11.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--		end
--Commented by Archana K. on 31/05/13 for Bug-5837 end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id)'
			
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
		end
		print '12.'
		print @SqlCommand

		execute sp_executesql @SqlCommand
		set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto, BINARY BASE64,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '13.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end
----****
	if @TablNm='STACDET' 
	begin
		print 'b'
--Commented by Archana K. on 31/05/13 for Bug-5837 start
--		if @FileType='xsd'
--		begin
--			set @SqlCommand='SELECT Dataexport1='+@ExpCode
----			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
----			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id) where 1=2'
--
--			print '14.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--			print '15.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--		end
--Commented by Archana K. on 31/05/13 for Bug-5837 end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
--			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id)'
			
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
		end
		print '16.'
		print @SqlCommand

		execute sp_executesql @SqlCommand
		set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '17.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end
	if @TablNm='STMALL' 
	begin
--Commented by Archana K. on 31/05/13 for Bug-5837 start
--		if @FileType='xsd'
--		begin
--			set @SqlCommand='SELECT Dataexport1='+@ExpCode
----			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
----			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id) where 1=2'
--
--			print '18.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from 	'+@TempTbl+@TablNm+' as '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--			print '19.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--		end
--Commented by Archana K. on 31/05/13 for Bug-5837 end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
--			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id)'
			
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
		end
		print '20.'
		print @SqlCommand

		execute sp_executesql @SqlCommand
		set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '21.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end
	if @TablNm='STITREF' 
	begin
--Commented by Archana K. on 31/05/13 for Bug-5837 start
--		if @FileType='xsd'
--		begin
--			set @SqlCommand='SELECT Dataexport1='+@ExpCode
----			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
----			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837
--		
--			print '22.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
--			print '23.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--		end
--Commented by Archana K. on 31/05/13 for Bug-5837 end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
--			set @SqlCommand=rtrim(@SqlCommand)+',Ac_Name=b.Ac_Name'
--			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'--Commented by Archana K. on 09/04/13 for Bug-5837
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+@TablNm+' FROM '+@TablNm+' a'--Changed by Archana K. on 09/04/13 for Bug-5837

			--set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.Ac_ID=b.ac_id)'
			
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
		end
		print '24.'
		print @SqlCommand

		execute sp_executesql @SqlCommand
		set @SqlCommand='Select (select * from '+@TempTbl+@TablNm+' as '+@TablNm
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '25.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end

--Added by Archana K. on 10/12/13 for Bug-20512 start
	if @TablNm='GEN_SRNO' 
	begin
		set @SqlCommand=''
		if @FileType='xml'
		begin
			set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,* FROM '+@TablNm 
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
			
			set @SqlCommand = rtrim(@SqlCommand)+ ' and entry_ty=''ST'''

			set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
	end
--Added by Archana K. on 10/12/13 for Bug-20512 end


--------------****-----------------------*****------------------------------
--Added By Kishor A. for bug-26960 on 21/09/2015 Start..

	if @TablNm='IT_SRTRN' 
	begin						
		set @SqlCommand=''
		if @FileType='xml'
		begin
			--set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode+',TmpKey='+SUBSTRING(@ExpCode,1,4)+CHAR(39)+'+''#''+cast(iTran_cd as varchar)'  --Commented by Priyanka B on 10022018 for Bug-30849
			set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode+',TmpKey='+SUBSTRING(@ExpCode,1,charindex('+',@ExpCode)-1)+'+'+char(39)+'#'+char(39)+'+cast(iTran_cd as varchar)'  --Modified by Priyanka B on 19022018 for Bug-30849
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,Entry_ty,[Date],Tran_cd,inv_no,Itserial,It_code,Qty,REntry_ty,RDate,RTran_cd
												,Rinv_no,RItserial,Rqty,Dc_No,iTran_cd,pmKey,WARE_NM,WARE_NMFR,DataExport,DataImport FROM '+@TablNm
			set @SqlCommand=rtrim(@SqlCommand)+'  where '
			
			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(DataExport,'''')='''' '
			end
			else
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' 1=1'
			end
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and ('+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end				
			
			set @SqlCommand = rtrim(@SqlCommand)+ ' and Entry_ty=''ST'''

			set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
	end	
--Added By Kishor A. for bug-26960 on 21/09/2015 End..

--****
----Added by Archana K. on 12/04/13 for Bug-5837 start
if @TablNm='STMAIN' or @TablNm='STITEM' OR  @TablNm='STACDET' OR  @TablNm='STMALL' OR  @TablNm='STITREF'
	begin
	SET @SQLCOMMAND = 'DROP TABLE '+@TempTbl+@TablNm
	EXECUTE SP_EXECUTESQL @SQLCOMMAND
END
----Added by Archana K. on 12/04/13 for Bug-5837 end
end
--

