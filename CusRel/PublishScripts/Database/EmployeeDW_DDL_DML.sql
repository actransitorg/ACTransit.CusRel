USE [EmployeeDW]
GO
/****** Object:  Table [dbo].[EmployeesLocation]    Script Date: 11/16/2017 3:53:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeesLocation](
	[EmployeesLocationId] [bigint] IDENTITY(1,1) NOT NULL,
	[Badge] [varchar](6) NOT NULL,
	[EmployeeName] [varchar](50) NULL,
	[BeginEffDate] [date] NOT NULL,
	[EndEffDate] [date] NOT NULL,
	[Empl_Status] [char](1) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[MiddleName] [varchar](30) NULL,
	[LastName] [varchar](70) NULL,
	[Location] [varchar](10) NULL,
	[Per_Org] [varchar](5) NULL,
	[AddUserId] [varchar](50) NOT NULL,
	[AddDateTime] [datetime2](7) NOT NULL,
	[UpdUserId] [varchar](50) NOT NULL,
	[UpdDateTime] [datetime2](7) NOT NULL,
 CONSTRAINT [EmployeesLocation_PK] PRIMARY KEY CLUSTERED 
(
	[EmployeesLocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Department](
	[DepartmentId] [varchar](10) NOT NULL,
	[DeptName] [varchar](30) NULL,
	[Location] [varchar](12) NULL,
	[KPIReport] [bit] NULL,
	[AddUserId] [varchar](50) NOT NULL,
	[AddDateTime] [datetime2](7) NOT NULL,
	[UpdUserId] [varchar](50) NOT NULL,
	[UpdDateTime] [datetime2](7) NOT NULL,
 CONSTRAINT [Department_PK] PRIMARY KEY CLUSTERED 
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [Department_KPIReport_DF]  DEFAULT (NULL) FOR [KPIReport]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [Department_DF_AddUserId]  DEFAULT (suser_name()) FOR [AddUserId]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [Department_DF_AddDateTime]  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [Department_DF_UpdUserId]  DEFAULT (suser_name()) FOR [UpdUserId]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [Department_DF_UpdDateTime]  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO


INSERT [dbo].[Department] ([DepartmentId], [DeptName], [Location], [KPIReport], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) 
VALUES 
(N'0001', N'Information Systems', N'GO', 0, N'Employees.dtsx', CAST(N'2016-09-30T05:00:34.9257024' AS DateTime2), N'Employees.dtsx', CAST(N'2016-09-30T05:00:34.9257024' AS DateTime2))
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[Emp_Id] [int] IDENTITY(1,1) NOT NULL,
	[Badge] [varchar](6) NOT NULL,
	[ETLDateTime] [datetime2](7) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[MiddleName] [varchar](30) NULL,
	[Suffix] [varchar](10) NULL,
	[Pref_Name] [varchar](30) NULL,
	[NTLogin] [varchar](50) NULL,
	[Hire_Dt] [datetime] NULL,
	[HireTime] [time](7) NULL,
	[Rehire_Dt] [datetime] NULL,
	[BusDriverQualDate] [datetime] NULL,
	[BusDriverQualTime] [time](7) NULL,
	[LastWorkDate] [datetime] NULL,
	[BirthDate] [datetime] NULL,
	[Address01] [varchar](35) NULL,
	[Address02] [varchar](35) NULL,
	[City] [varchar](30) NULL,
	[State] [varchar](6) NULL,
	[ZIP] [varchar](10) NULL,
	[PreferredPhone] [varchar](24) NULL,
	[WorkPhone] [varchar](24) NULL,
	[CellPhone] [varchar](26) NULL,
	[EmailAddress] [varchar](50) NULL,
	[Empl_Status] [varchar](1) NOT NULL,
	[DeptId] [varchar](10) NOT NULL,
	[DeptName] [varchar](30) NOT NULL,
	[Location] [varchar](10) NULL,
	[Company] [varchar](3) NOT NULL,
	[JobCode] [varchar](6) NOT NULL,
	[PayGroup] [varchar](3) NOT NULL,
	[JobTitle] [varchar](30) NOT NULL,
	[BusinessTitle] [varchar](30) NULL,
	[Empl_Type] [varchar](1) NOT NULL,
	[RegTempFlag] [varchar](1) NULL,
	[FT_PT_Flag] [varchar](1) NULL,
	[Per_Org] [varchar](5) NULL,
	[SupervisorId] [varchar](11) NULL,
	[SupervisorName] [varchar](55) NULL,
	[Action] [varchar](3) NULL,
	[ActionDate] [datetime] NULL,
	[ActionReason] [varchar](3) NULL,
	[TermDate] [datetime] NULL,
	[UnionCode] [varchar](3) NULL,
	[EEO4Code] [varchar](1) NULL,
	[EEOJobGroup] [varchar](2) NULL,
	[CompFrequency] [varchar](1) NULL,
	[CompRate] [money] NULL,
	[AnnualRate] [money] NULL,
	[MonthlyRate] [money] NULL,
	[HourlyRate] [money] NULL,
	[Grade] [varchar](3) NULL,
	[Step] [varchar](2) NULL,
	[PositionNumber] [varchar](8) NULL,
	[PositionEntryDate] [datetime] NULL,
	[Sex] [varchar](1) NOT NULL,
	[EthnicGroup] [varchar](5) NULL,
	[Veteran] [varchar](1) NULL,
	[Citizen] [varchar](1) NULL,
	[Disabled] [varchar](2) NULL,
	[Marital] [varchar](1) NULL,
	[AsOfDate] [datetime] NULL,
	[EffectiveDate] [datetime] NULL,
	[VacHoursEntitlement] [decimal](6, 2) NULL,
	[VacHoursCarryover] [decimal](6, 2) NULL,
	[DriverLicenseNumber] [varchar](20) NULL,
	[DriverLicenseExpirationDate] [date] NULL,
	[MedicalExpirationDate] [date] NULL,
	[StepStartDate] [date] NULL,
	[WorkShift] [varchar](30) NULL,
	[ScheduledDaysOff] [varchar](27) NULL,
	[PSBadge] [varchar](7) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedOn] [datetime] NULL,
	[SecurityCardFormatted] [varchar](10) NULL,
	[SecurityCardNumber] [varchar](20) NULL,
	[SecurityCardEnabled] [bit] NULL,
	[SecurityCardEncoded] [decimal](24, 0) NULL,
	[SecurityETLDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[Emp_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[EmployeeAll]    Script Date: 11/16/2017 3:53:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[EmployeeAll]
AS
SELECT 
	/*
		Empl_Status 

		R=Retired  
		A=Active  
		L=Leave   
		U=Terminated with pay
		Q=Retired with pay 
		D=Deceased  
		P=Leave with Pay 
		S=Suspension 
		T=Terminated
	*/
	 l.Badge Badge
    ,l.FirstName + ' ' + ISNULL( NULLIF(l.MiddleName,'') + ' ' ,'') + l.LastName EmployeeName
	,l.FirstName FirstName
	,l.LastName LastName
	,l.Location Location
	,RTRIM(LEFT(l.Location,3))Division
	,l.Per_Org Per_Org
	,e.Suffix Suffix
	,e.NTLogin NTLogin
	,l.Empl_Status Empl_Status 
	,ISNULL(e.DeptName,RTRIM(LEFT(l.Location,3))) DeptName
	,l.BeginEffDate
	,l.EndEffDate
	,l.EmployeesLocationId
	,CASE WHEN e.Badge IS NOT NULL THEN 1 ELSE 0 END InEmpTable
	,e.JobTitle
	,e.StepStartDate
	,e.WorkShift
	,e.ScheduledDaysOff	
	,e.PSBadge

FROM dbo.EmployeesLocation l
LEFT JOIN dbo.Employees e ON l.Badge = e.Badge
WHERE EndEffDate = '9999-12-31';




GO
SET IDENTITY_INSERT [dbo].[Employees] ON 
GO
INSERT [dbo].[Employees] ([Emp_Id], [Badge], [ETLDateTime], [Name], [FirstName], [LastName], [MiddleName], [Suffix], [Pref_Name], [NTLogin], [Hire_Dt], 
	[HireTime], [Rehire_Dt], [BusDriverQualDate], [BusDriverQualTime], [LastWorkDate], [BirthDate], 
	[Address01], [Address02], [City], [State], [ZIP], [PreferredPhone], [WorkPhone], [CellPhone], [EmailAddress], [Empl_Status], [DeptId], [DeptName], [Location], [Company], 
	[JobCode], [PayGroup], [JobTitle], 	[BusinessTitle], [Empl_Type], [RegTempFlag], [FT_PT_Flag], [Per_Org], [SupervisorId], [SupervisorName], 
	[Action], [ActionDate], [ActionReason], [TermDate], [UnionCode], [EEO4Code], [EEOJobGroup], [CompFrequency], [CompRate], [AnnualRate], [MonthlyRate], [HourlyRate], [Grade], [Step], [PositionNumber], [PositionEntryDate], 
	[Sex], [EthnicGroup], [Veteran], [Citizen], [Disabled], [Marital], [AsOfDate], [EffectiveDate], [VacHoursEntitlement], [VacHoursCarryover], 
	[DriverLicenseNumber], [DriverLicenseExpirationDate], [MedicalExpirationDate], [StepStartDate], [WorkShift], [ScheduledDaysOff], [PSBadge], [AddUserId], 
	[AddDateTime], [UpdatedBy], [UpdatedOn], [SecurityCardFormatted], [SecurityCardNumber], [SecurityCardEnabled], [SecurityCardEncoded], [SecurityETLDateTime]) 
	VALUES 
	(2215857, N'000001', CAST(N'2017-07-03T12:02:48.0000000' AS DateTime2), N'Bob Smith', N'Bob', N'Smith', N'', NULL, NULL, N'bsmith', CAST(N'1979-05-21T00:00:00.000' AS DateTime), 
	NULL, NULL, CAST(N'1979-07-02T00:00:00.000' AS DateTime), CAST(N'14:25:00' AS Time), CAST(N'2015-09-02T00:00:00.000' AS DateTime), CAST(N'1957-09-22T00:00:00.000' AS DateTime), 
	N'123 Oak Lane', NULL, N'Somewhere', N'CA', N'912345', N'8005551212', NULL, NULL, NULL, N'P', N'1105', N'Special Division 7', N'GO', N'ACT',
	N'990', N'OFT', N'Bus Operator', N'Bus Driver', N'H', N'R', N'F', N'EMP', N'000002', N'Alice Person', 
	N'PAY', CAST(N'2017-07-03T00:00:00.000' AS DateTime), N'CON', NULL, N'TRN', N'9', NULL, N'H', 30.5800, 63606.4000, 5300.5330, 30.5800, N'05A', N'4', N'00001540', CAST(N'2016-03-01T00:00:00.000' AS DateTime), 
	N'F', N'BLACK', NULL, NULL, NULL, N'2', NULL, CAST(N'2017-07-01T00:00:00.000' AS DateTime), CAST(240.00 AS Decimal(6, 2)), CAST(0.00 AS Decimal(6, 2)),
	N'D123456', CAST(N'2020-09-22' AS Date), CAST(N'2015-10-20' AS Date), CAST(N'2008-08-01' AS Date), N'Not Applicable', NULL, N'012345', NULL, 
	CAST(N'2017-05-26T09:46:57.460' AS DateTime), N'user1', CAST(N'2017-09-14T16:03:29.007' AS DateTime), N'12:30381', N'00000000000001210029', 1, CAST(6914633051 AS Decimal(24, 0)), CAST(N'2017-09-14T16:03:29.0068481' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[EmployeesLocation] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_Employees_Badge]    Script Date: 11/16/2017 3:53:47 PM ******/
ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [UK_Employees_Badge] UNIQUE NONCLUSTERED 
(
	[Badge] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [Employees_DF_ETLDateTime]  DEFAULT (sysdatetime()) FOR [ETLDateTime]
GO
ALTER TABLE [dbo].[EmployeesLocation] ADD  CONSTRAINT [EmployeesLocation_DF_AddUserId]  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[EmployeesLocation] ADD  CONSTRAINT [EmployeesLocation_DF_AddDateTime]  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[EmployeesLocation] ADD  CONSTRAINT [EmployeesLocation_DF_UpdUserId]  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[EmployeesLocation] ADD  CONSTRAINT [EmployeesLocation_DF_UpdDateTime]  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Employee demograpics, telephone and dept information' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'PeopleSoft file EMPLOYEES.csv' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'Med change datasource for Ellipse job' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'dbo.PopulateDepartmentEmployeePeopleSoft' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'GetEmployees' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'\your.SQL.server\c$\Program Files\Microsoft SQL Server\100\DTS\Packages\Employee_SSIS\Employees.dtsx' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'PeopleSoft' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'ACTransit.SSIS.Employee' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Employees'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Service Efficiency KPI shows Employees contact information with their department locations.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'PeopleSoft file EMPLOC.csv' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'dbo.PopulateEmployeesLocation' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'GetEmployees' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'\your.SQL.server\c$\Program Files\Microsoft SQL Server\100\DTS\Packages\Employee_SSIS\Employees.dtsx' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'PeopleSoft' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'ACTransit.SSIS.Employee' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmployeesLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DataDesc', @value=N'Show current and history employee information - limited for non-current staff.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EmployeeAll'
GO

