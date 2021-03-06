--Select * From sysobjects where [name] ='Emp_Payroll_Section_Master'
--Select * From sysobjects where [name] ='Emp_Payroll_Main_Section_Master'

--Drop Table Emp_Payroll_Section_Master
If Exists(Select [Name] From sysobjects where [name] ='Emp_Payroll_Section_Master') and Exists(Select [Name] From sysobjects where [name] ='Emp_Payroll_Section_Main_Master')
Begin
	Drop table Emp_Payroll_Section_Master
	Drop table Emp_Payroll_Section_Main_Master
end

IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Emp_Payroll_Section_Master]') AND type in (N'U'))
Begin
	CREATE TABLE [dbo].[Emp_Payroll_Section_Master](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SortOrd] [int] NULL,
	[Section] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Form16Group] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
	) ON [PRIMARY]
End
IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Emp_Payroll_Declaration_Master]') AND type in (N'U'))
Begin
	CREATE TABLE [dbo].[Emp_Payroll_Declaration_Master](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SortOrd] [int] NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Master_SortOrd]  DEFAULT ((0)),
	[Section] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Master_Section]  DEFAULT (''),
	[DeclarationDet] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Master_DeclarationDet]  DEFAULT (''),
	[Fld_Nm] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Master_Fld_Nm]  DEFAULT (''),
	[IsDeactive] [bit] NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Master_IsDeactive]  DEFAULT ((0)),
	[DeactFrom] [smalldatetime] NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Master_DeactFrom]  DEFAULT ('')
	) ON [PRIMARY]
end
IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Emp_Payroll_Declaration_Details]') AND type in (N'U'))
Begin
	CREATE TABLE [dbo].[Emp_Payroll_Declaration_Details](
	[Id] [decimal](10, 0) IDENTITY(1,1) NOT NULL,
	[Pay_Year] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[EmployeeCode] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[section17_1] [decimal](14, 3) NULL CONSTRAINT [DF_Emp_Payroll_Declaration_Details_section17_1]  DEFAULT ((0)),
	[Section17_2] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_11]  DEFAULT ((0)),
	[Section17_3] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_12]  DEFAULT ((0)),
	[OthIncomeDivedends] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_13]  DEFAULT ((0)),
	[OthIncomeInterest] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_14]  DEFAULT ((0)),
	[OthIncomeSpcify] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_15]  DEFAULT ((0)),
	[ChildEduAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_16]  DEFAULT ((0)),
	[MediAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_17]  DEFAULT ((0)),
	[ConvAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_18]  DEFAULT ((0)),
	[LTAAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_19]  DEFAULT ((0)),
	[FoodAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_110]  DEFAULT ((0)),
	[JorAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_111]  DEFAULT ((0)),
	[TeleAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_112]  DEFAULT ((0)),
	[CarAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_113]  DEFAULT ((0)),
	[InterAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_114]  DEFAULT ((0)),
	[DriverAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_115]  DEFAULT ((0)),
	[OthNonTaxAllow] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_116]  DEFAULT ((0)),
	[PTAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_117]  DEFAULT ((0)),
	[EntAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_118]  DEFAULT ((0)),
	[HBLoanAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_119]  DEFAULT ((0)),
	[LIPAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_section17_120]  DEFAULT ((0)),
	[EPFAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt1]  DEFAULT ((0)),
	[PPFAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt2]  DEFAULT ((0)),
	[ELSSAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt3]  DEFAULT ((0)),
	[NSSAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt4]  DEFAULT ((0)),
	[NSCAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt5]  DEFAULT ((0)),
	[SCSSAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt6]  DEFAULT ((0)),
	[TaxBondAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt7]  DEFAULT ((0)),
	[NPSAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt8]  DEFAULT ((0)),
	[ChilTutionAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt9]  DEFAULT ((0)),
	[HBPinPayAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt1]  DEFAULT ((0)),
	[TaxFDAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_LIPAmt10]  DEFAULT ((0)),
	[Oth80cAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt21]  DEFAULT ((0)),
	[Pen80CCCAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt22]  DEFAULT ((0)),
	[MediSelfAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt23]  DEFAULT ((0)),
	[MediPerentAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt24]  DEFAULT ((0)),
	[MainDisableAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt25]  DEFAULT ((0)),
	[SpecificDiseaseTreatAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt26]  DEFAULT ((0)),
	[EduLoanInt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt27]  DEFAULT ((0)),
	[DisabilityAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt28]  DEFAULT ((0)),
	[80G100Amt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt29]  DEFAULT ((0)),
	[80G50Amt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt210]  DEFAULT ((0)),
	[InfaBondAmt] [decimal](14, 3) NULL CONSTRAINT [DF_Table_1_HBPinPayAmt211]  DEFAULT ((0)),
	[MFAmt] [decimal](14, 3) DEFAULT ((0))
	) ON [PRIMARY]
end