IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetCommanParaVal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_GetCommanParaVal]
Go
set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

Create Procedure [dbo].[Usp_GetCommanParaVal]
@CommanPara varchar(4000),@VarCode varchar(90),@VarVal VARCHAR(200) OUTPUT
As
Begin
--	Declare @CommanPara varchar(4000),@VarCode varchar(90),@VarVal VARCHAR(200)
--	set @VarCode='sDate'

	--set @CommanPara='<<TablNm=ac_group_mast>><<FileTye=xsd>><<ExpCode=''ALM''+''#''+Ac_Group_Name>><<ExpDataVol=Full>><<DtFld=Date>><<sDate=11/1/2011>><<eDate=11/24/2011>>'
	declare @pos1 int,@pos2 int,@tempStr1 varchar(8000),@valstr varchar(1000)
	set @VarCode='<<'+rtrim(@VarCode)+'='
--	print 'aa2 '+@VarCode
	if charindex(@VarCode,@CommanPara)>0 
	begin
		set @pos1=charindex(@VarCode,@CommanPara)
		set @valstr=substring(@CommanPara,@pos1,len(@CommanPara))

		set @pos1=charindex('>>',@valstr)
		set @valstr=substring(@valstr,1,@pos1+1)

		set @valstr=replace(@valstr,@VarCode,'')

		set @valstr=replace(@valstr,'>>','')
		set @VarVal =@valstr
	end
	--print 'c '+@VarVal
End	
