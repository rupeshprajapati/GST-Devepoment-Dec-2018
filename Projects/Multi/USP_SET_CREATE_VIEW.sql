USE [x010809]
GO
/****** Object:  StoredProcedure [dbo].[USP_SET_CREATE_VIEW]    Script Date: 12/27/2008 10:31:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- AUTHOR:		RUEPESH PRAJAPATI.
-- CREATE DATE: 16/05/2007
-- DESCRIPTION:	THIS STORED PROCEDURE IS USEFUL TO GENERATE EXCISE EXPORT ANNEXEURE19A PART-I REPORT.
-- MODIFY DATE: 16/05/2007
-- MODIFIED BY: 
-- MODIFY DATE: 
-- REMARK:
-- =============================================
CREATE PROCEDURE  [dbo].[USP_SET_CREATE_VIEW]
as
insert into t1 (t1) values ('a')
select vw_nm=space(100),vw_str=cast('' as text) into #vw_str from sysobjects where 1=2
select vw_nm=space(100),table_nm=space(20) into #table_nm from sysobjects where 1=2
select vw_nm=space(100),fld_nm=space(20),defval=space(100) into #fld_nm from sysobjects where 1=2

declare @view_name varchar(50),@table_list varchar(8000),@fld_list varchar(8000),@vw_str varchar(100)
declare @coma_pos int,@fld_nm varchar(100),@defval varchar(100),@table_nm varchar(100),@dbname varchar(20)
declare @selstr varchar(8000),@fldcnt bit,@tblcnt bit
declare @sqlcommand nvarchar(4000)
DECLARE @ptrval binary(16)

declare cur_view_defination cursor for 
select view_name,table_list,fld_list from view_defination
open cur_view_defination
fetch next from cur_view_defination into @view_name,@table_list,@fld_list
while (@@fetch_status=0)
begin
	delete from #table_nm
	delete from #fld_nm
	set @selstr='if exists(select [name] from sysobjects where [name]='+char(39)+@view_name+char(39)+')'
	set @selstr=rtrim(@selstr)+' '+' begin '	
	set @selstr=rtrim(@selstr)+' '+' drop view '+@view_name
	set @selstr=rtrim(@selstr)+' '+' end'
	insert into #vw_str(vw_nm,vw_str) values (@view_name,@selstr)

	set @vw_str=' '
	insert into #vw_str(vw_nm,vw_str) values (@view_name,@vw_str)
--	print @view_name
--	print @table_list
--	print @fld_list
	-->Table List Loop
	while(len(rtrim(@table_list))<>0)
	begin
		SET @coma_pos=charindex(',',@table_list)
		if (@coma_pos=0)
		begin
			set @coma_pos=len(@table_list)+1
		end
		print @coma_pos
		set @table_nm=substring(@table_list,1,@coma_pos-1)
		set @table_list=substring(@table_list,@coma_pos+1,len(rtrim(@table_list)))
		insert into #table_nm (vw_nm,table_nm) values( @view_name,@table_nm)
		--print 'table- '+@table_nm
		--print 'tab list- '+@table_list
	end--while(len(rtrim(@fld_list))<>0)
	--<--Table List Loop
	-->Field List Loop
	while(len(rtrim(@fld_list))<>0)
	begin
		SET @coma_pos=charindex(',',@fld_list)
		if (@coma_pos=0)
		begin
			set @coma_pos=len(@fld_list)+1
		end
		--print @coma_pos
		set @fld_nm=substring(@fld_list,1,@coma_pos-1)
		set @fld_list=substring(@fld_list,@coma_pos+1,len(rtrim(@fld_list)))
		SET @coma_pos=charindex(' ',@fld_nm)
		set @defval=substring(@fld_nm,@coma_pos+1,len(rtrim(@fld_nm)))
		set @fld_nm=substring(@fld_nm,1,@coma_pos)
		insert into #fld_nm (vw_nm,fld_nm,defval) values(@view_name,@fld_nm,@defval)
		--print 'fld- '+@fld_nm
		--print 'def- '+@defval
		--print 'list- '+@fld_list
	end--while(len(rtrim(@fld_list))<>0)
	--<--Field List Loop


	set @tblcnt=0
	declare cur_vw_dbname cursor for select b.dbname from com_det a inner join vudyog..co_mast b (nolock) on (a.co_name=b.co_name)
	open cur_vw_dbname
	fetch next from cur_vw_dbname into @dbname
	while(@@fetch_status=0)		
	begin
		---
		
		declare cur_vw_table_nm cursor for select distinct table_nm from #table_nm
		open cur_vw_table_nm
		fetch next from cur_vw_table_nm into @table_nm
		while(@@fetch_status=0)		
		begin
			print 'tab-'+@table_nm
			--
			--if (@tblcnt=0)
			--begin
			--	set @tblcnt=1
				set @fldcnt=0
			--	set @selstr=' select '
			--end
			--else
			--begin
				set @selstr=' union select '
			--end
			
			--print @selstr
			declare cur_vw_fld_nm cursor for select distinct fld_nm,defval from #fld_nm
			open cur_vw_fld_nm
			fetch next from cur_vw_fld_nm into @fld_nm,@defval
			while(@@fetch_status=0)		
			begin
				if (@fldcnt=0)
				begin
					set @fldcnt=1
					set @selstr=rtrim(@selstr)+' '
				end
				else
				begin
					set @selstr=rtrim(@selstr)+','
				end
				print @table_nm
				print @fld_nm
				if exists(select [name] from syscolumns where [name]=@fld_nm and id in( select id from sysobjects where [name]=@table_nm ))
				begin
					set @selstr=rtrim(@selstr)+' '+@fld_nm
				end
				else
				begin
				    set @selstr=rtrim(@selstr)+' '+@fld_nm+'='+@defval	
				end
				--
				fetch next from cur_vw_fld_nm into @fld_nm,@defval
			end --cur_vw_fld_nm --@@fetch_status=0
			close cur_vw_fld_nm
			deallocate cur_vw_fld_nm
			--	
			set @selstr=rtrim(@selstr)+' from '+rtrim(@dbname)+'..'+@table_nm
			print @selstr
			
			SELECT @ptrval = TEXTPTR(vw_str) from #vw_str --where tran_cd=1
			UPDATETEXT #vw_str.vw_str @ptrval 0 0 @selstr

			fetch next from cur_vw_table_nm into @table_nm
		end --cur_vw_table_nm --@@fetch_status=0
		close cur_vw_table_nm
		deallocate cur_vw_table_nm
		---
		fetch next from cur_vw_dbname into @dbname
	end --cur_vw_fld_nm --@@fetch_status=0
	close cur_vw_dbname
	deallocate cur_vw_dbname
	
	set @selstr=' create view '+@view_name+' as '
	SELECT @ptrval = TEXTPTR(vw_str) from #vw_str --where tran_cd=1
	UPDATETEXT #vw_str.vw_str @ptrval 0 6 @selstr
	
	fetch next from cur_view_defination into @view_name,@table_list,@fld_list
end
close cur_view_defination
deallocate cur_view_defination

select * from #vw_str
--select * from #table_nm
--select * from #fld_nm


