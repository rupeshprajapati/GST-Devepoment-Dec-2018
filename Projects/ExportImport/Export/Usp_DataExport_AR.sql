IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_DataExport_AR]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_DataExport_AR]
Go

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go




Create Procedure [dbo].[Usp_DataExport_AR]
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
	
	 --select @TablNm ,@FileType ,@ExpCode ,@ExpDataVol,@DtFld ,@sDate ,@eDate 

	--execute Usp_GetCommanParaVal @CommanPara,'TablNm',@TablNm out

	

		
	Declare @SqlCommand nvarchar(4000)
	Declare @Manu_Det_Cmd nvarchar(4000)
	set @ExpCode=replace(@ExpCode,'`','''')
	if  @TablNm='Manu_det'
	begin
		if @FileType='xsd'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',manuAc_Name=b.Ac_Name,manuSAc_Name=c.Location_Id'
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'
			set @SqlCommand=rtrim(@SqlCommand)+' Left join ac_mast b on (a.ManuAc_ID=b.ac_id) left join shipto c on (a.ManuSAc_Id=c.Shipto_Id) where 1=2'
			print '1.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
			set @SqlCommand='Select (select * from 	'+@TempTbl+' manu_det'	
			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			execute sp_executesql @SqlCommand
			print '2.'
			print @SqlCommand
		end
		if @FileType='xml'
		begin
			---
			set @SqlCommand=' SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',manuAc_Name=b.Ac_Name,manuSAc_Name=c.Location_Id '
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.*  into '+@TempTbl+' FROM  '+@TablNm+' a '
			set @SqlCommand=rtrim(@SqlCommand)+' Left join ac_mast b on (a.ManuAc_ID=b.ac_id) left join shipto c on (a.ManuSAc_Id=c.Shipto_Id)'
			set @SqlCommand=rtrim(@SqlCommand)+'  where '

			if @ExpDataVol='Updated' 
			begin
				set @SqlCommand=rtrim(@SqlCommand)+' isnull(a.DataExport,'''')='''' '
			end
			
			if isnull(@DtFld,'')<>''
			begin
				set @SqlCommand = rtrim(@SqlCommand)+ ' and (a.'+@DtFld+' between '+char(39)+@sDate+Char(39)+' and '+char(39)+@eDate+Char(39)+')'
			end

			set @SqlCommand = rtrim(@SqlCommand)+ ' and (Entry_ty in (''AR''))'
			print '3.'
			print @SqlCommand

			execute sp_executesql @SqlCommand
			set @SqlCommand='Select (select * from 	'+@TempTbl+' manu_det'
			set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			print '4.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
	end		


	if @TablNm='ARMain' 
	begin
		print 'b'
		if @FileType='xsd'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',cons_Name=b.Ac_Name,Scons_Name=c.Location_Id,sAc_Name=d.Location_id '
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.cons_id=b.ac_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.scons_id=c.shipto_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto d on (a.scons_id=d.shipto_id) where 1=2'
--			print '5.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
			print '6.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
			set @SqlCommand='Select (select * from 	'+@TempTbl+' armain'
			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			print '7.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',cons_Name=b.Ac_Name,Scons_Name=c.Location_Id,sAc_Name=d.Location_id '
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.cons_id=b.ac_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.scons_id=c.shipto_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto d on (a.scons_id=d.shipto_id)'
			
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

		execute sp_executesql @SqlCommand
		set @SqlCommand='Select (select * from 	'+@TempTbl+' armain'
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '9.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end
----****
	if @TablNm='ARItem' 
	begin
		print 'b'
		if @FileType='xsd'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',manuAc_Name=b.Ac_Name,manuSAc_Name=c.Location_Id'
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.manuAc_ID=b.ac_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.ManuSAc_Id=c.shipto_id) where 1=2'
--			print '5.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
			print '6.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
			set @SqlCommand='Select (select * from 	'+@TempTbl+' aritem'
			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			print '7.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
		if @FileType='xml'
		begin
			set @SqlCommand='SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',manuAc_Name=b.Ac_Name,manuSAc_Name=c.Location_Id'
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,a.* into '+@TempTbl+' FROM '+@TablNm+' a'
			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.manuAc_ID=b.ac_id)'
			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.ManuSAc_Id=c.shipto_id) '
			
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

		execute sp_executesql @SqlCommand
		set @SqlCommand='Select (select * from 	'+@TempTbl+' aritem'
		set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
		print '9.'
		print @SqlCommand
		execute sp_executesql @SqlCommand
	end

----****
--****
	if @TablNm='GEN_SRNO' 
	begin
		print 'b'
		if @FileType='xsd'
		begin
			set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,* FROM '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.cons_id=b.ac_id)'
--			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.scons_id=c.shipto_id)'
--			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto d on (a.scons_id=d.shipto_id) where 1=2'
--			print '5.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			print '6.'
--			print @SqlCommand
--			execute sp_executesql @SqlCommand
--			set @SqlCommand='Select (select * from 	'+@TempTbl+' armain'
			set @SqlCommand=rtrim(@SqlCommand)+' where 1=2 FOR XML auto,XMLSCHEMA,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			print '7.'
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
		set @SqlCommand=''
		if @FileType='xml'
		begin
			set @SqlCommand='Select (SELECT Dataexport1='+@ExpCode
--			set @SqlCommand=rtrim(@SqlCommand)+',cons_Name=b.Ac_Name,Scons_Name=c.Location_Id,sAc_Name=d.Location_id '
			set @SqlCommand=rtrim(@SqlCommand)+',oldTran_cd=Tran_cd,* FROM '+@TablNm
--			set @SqlCommand=rtrim(@SqlCommand)+' left join ac_mast b on (a.cons_id=b.ac_id)'
--			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto c on (a.scons_id=c.shipto_id)'
--			set @SqlCommand=rtrim(@SqlCommand)+' left join shipto d on (a.scons_id=d.shipto_id)'
			
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
		
	--		print '8.'
	--		print @SqlCommand

	--		execute sp_executesql @SqlCommand
	--		set @SqlCommand='Select (select * from 	'+@TempTbl+' armain'
			set @SqlCommand=rtrim(@SqlCommand)+' FOR XML auto,ROOT('+char(39)+@TablNm+char(39)+'))  as cxml'
			print '9. Amrendra'
			print @SqlCommand
			execute sp_executesql @SqlCommand
		end
	end
--****
end



