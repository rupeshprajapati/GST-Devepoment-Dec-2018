IF EXISTS(SELECT * FROM SYSOBJECTS WHERE [NAME]='USP_ENT_EMAILCLIENT_SELECT' AND XTYPE='P')
BEGIN
	DROP PROCEDURE USP_ENT_EMAILCLIENT_SELECT
END
GO
Create Procedure [dbo].[USP_ENT_EMAILCLIENT_SELECT]
@action varchar(10),
@id varchar(20)='',
@custnm varchar(100)=''
As
Begin
	If rtrim(upper(@action)) = 'SELECT'
	Begin
		Select * From eMailClient Where id=@id
	End
	
	If rtrim(upper(@action)) = 'AC_MAST'
	Begin
	if @custnm <> '*'
	Begin
		Select Mailname,Email From Ac_Mast 
		Where rtrim(Mailname) like '%' + @custnm +'%'
		and [group] = 'SUNDRY CREDITORS'
		Order by Mailname
	End
	Else
	Begin
		Select Mailname,Email From Ac_Mast 
		Where [group] = 'SUNDRY CREDITORS'
		Order by Mailname
	End
	End
End