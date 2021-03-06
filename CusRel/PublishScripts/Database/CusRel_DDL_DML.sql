USE [CusRel]
GO
SET NOCOUNT ON
/****** Object:  User [cusRelWriter]    Script Date: 12/13/2017 2:56:32 PM ******/
CREATE USER [cusRelWriter] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [cusRelReader]    Script Date: 12/13/2017 2:56:32 PM ******/
CREATE USER [cusRelReader] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [actwebsrv]    Script Date: 12/13/2017 2:56:32 PM ******/
ALTER ROLE [db_datareader] ADD MEMBER [cusRelWriter]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [cusRelWriter]
GO
ALTER ROLE [db_datareader] ADD MEMBER [cusRelReader]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[InitCap] ( @InputString varchar(4000) ) 
RETURNS VARCHAR(4000)
AS
BEGIN

DECLARE @Index          INT
DECLARE @Char           CHAR(1)
DECLARE @PrevChar       CHAR(1)
DECLARE @OutputString   VARCHAR(255)

SET @OutputString = LOWER(@InputString)
SET @Index = 1

WHILE @Index <= LEN(@InputString)
BEGIN
    SET @Char     = SUBSTRING(@InputString, @Index, 1)
    SET @PrevChar = CASE WHEN @Index = 1 THEN ' '
                         ELSE SUBSTRING(@InputString, @Index - 1, 1)
                    END

    IF @PrevChar IN (' ', ';', ':', '!', '?', ',', '.', '_', '-', '/', '&', '''', '(')
    BEGIN
        IF @PrevChar != '''' OR UPPER(@Char) != 'S'
            SET @OutputString = STUFF(@OutputString, @Index, 1, UPPER(@Char))
    END

    SET @Index = @Index + 1
END

if (LEFT(@OutputString, 4) = 'Pbx-')
	SET @OutputString = 'PBX-' + SUBSTRING(@OutputString, 5, 4000)

if (LEFT(@OutputString, 4) = 'Ada-')
	SET @OutputString = 'ADA-' + SUBSTRING(@OutputString, 5, 4000)

RETURN @OutputString

END
GO
/****** Object:  View [dbo].[EmplDW_Department]    Script Date: 12/13/2017 2:56:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[EmplDW_Department]
AS

		SELECT 
		       DepartmentId as Id, DeptName as DeptId, AddUserId, AddDateTime, UpdUserId, UpdDateTime, DeptName, Location
		  FROM 
		       EmployeeDW.dbo.Department


GO
/****** Object:  View [dbo].[EmplDW_Employees]    Script Date: 12/13/2017 2:56:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[EmplDW_Employees]
AS

		SELECT 
		       Emp_Id, AddUserId, AddDateTime, UpdatedBy, UpdatedOn, Badge, Name, FirstName, LastName, MiddleName, Suffix, Pref_Name, NTLogin, Hire_Dt, HireTime, Rehire_Dt, BusDriverQualDate, BusDriverQualTime, LastWorkDate, BirthDate, Address01, Address02, City, State, ZIP, PreferredPhone as HomePhone, WorkPhone, CellPhone, EmailAddress, Empl_Status, DeptId, DeptName, Location, Company, JobCode, PayGroup, JobTitle, BusinessTitle, Empl_Type, RegTempFlag, FT_PT_Flag, Per_Org, SupervisorId, SupervisorName, Action, ActionDate, ActionReason, TermDate, UnionCode, EEO4Code, EEOJobGroup, CompFrequency, CompRate, AnnualRate, MonthlyRate, HourlyRate, Grade, Step, PositionNumber, PositionEntryDate, Sex, EthnicGroup, Veteran, Citizen, Disabled, Marital, AsOfDate, EffectiveDate
		      ,cast(
					upper(rtrim(lastName)) 
					+ case when (isnull(suffix, '')) = '' then '' else ' ' + upper(rtrim(suffix)) end 
					+ ',' 
					+ upper(rtrim(firstName)) 
					+ isnull(' ' + upper(rtrim(middlename)), '') 
				  as char(30)
				) as FullName_with_LastName_First
		   FROM EmployeeDW.dbo.Employees


GO
/****** Object:  Table [dbo].[AuthorizedUsers]    Script Date: 12/13/2017 2:56:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorizedUsers](
	[UserId] [varchar](16) NOT NULL,
	[UserName] [varchar](30) NOT NULL,
	[ClerkId] [varchar](16) NULL,
	[AddComments] [char](1) NULL,
	[AllowAssignTo] [char](1) NULL,
	[AllowSecurity] [char](1) NULL,
	[Division] [char](2) NULL,
	[ReferTo] [varchar](5) NULL,
	[Email] [varchar](50) NULL,
	[AllowCloseTicket] [char](1) NULL,
	[AllowSearchTickets] [char](1) NULL,
	[AllowViewUnassigned] [char](1) NULL,
	[NotifyWhenAssigned] [char](1) NOT NULL,
	[ReminderDelayDays] [tinyint] NOT NULL,
	[AllowOnlyDeptTickets] [char](1) NULL,
 CONSTRAINT [PK_AuthorizedUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [uniqueidentifier] NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatusTab]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusTab](
	[ForActionTo] [varchar](34) NULL,
	[NewCount] [int] NULL,
	[ResearchingCount] [int] NULL,
	[PendingContactCount] [int] NULL,
	[ReadyToCloseCount] [int] NULL,
	[RedirectedCount] [int] NULL,
	[IncompleteCount] [int] NULL,
	[RejectedCount] [int] NULL,
	[AssignedCount] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAttachments]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAttachments](
	[FileNum] [int] NOT NULL,
	[AttachmentNum] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](128) NULL,
	[FileSize] [int] NULL,
	[ContentType] [varchar](128) NULL,
	[BinaryData] [image] NULL,
	[DateUploaded] [datetime] NULL,
	[UploadedBy] [varchar](16) NULL,
	[Description] [varchar](128) NULL,
 CONSTRAINT [PK_tblAttachments] PRIMARY KEY CLUSTERED 
(
	[AttachmentNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAttachmentsTemp]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAttachmentsTemp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentNum] [int] NOT NULL,
	[FileName] [varchar](128) NULL,
	[FileSize] [int] NULL,
	[ContentType] [varchar](128) NULL,
	[Base64Data] [varchar](max) NULL,
	[DateUploaded] [datetime] NULL,
	[DateScanStart] [datetime] NULL,
	[ScanId] [varchar](300) NULL,
	[Sha256] [varchar](256) NULL,
	[ScanResult] [varchar](max) NULL,
	[FileNum] [int] NOT NULL,
	[ForceClose] [bit] NULL,
 CONSTRAINT [PK_tblAttachmentsTemp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblContactHistory]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblContactHistory](
	[FileNum] [int] NOT NULL,
	[UserId] [char](16) NULL,
	[ContactDateTime] [datetime] NULL,
	[Via] [char](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblContactHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblContacts]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblContacts](
	[FileNum] [int] IDENTITY(292566,1) NOT NULL,
	[FeedbackId] [int] NULL,
	[ClerkId] [varchar](16) NULL,
	[UserId] [varchar](16) NULL,
	[ADAComplaint] [char](1) NULL,
	[SeniorComplaint] [char](1) NULL,
	[ContactSource] [varchar](16) NULL,
	[ReceivedDateTime] [datetime] NULL,
	[Priority] [char](6) NULL,
	[ResolvedDateTime] [datetime] NULL,
	[FirstName] [varchar](30) NULL,
	[LastName] [varchar](30) NULL,
	[Addr1] [varchar](30) NULL,
	[Addr2] [varchar](30) NULL,
	[CustCity] [varchar](50) NULL,
	[CustState] [char](2) NULL,
	[CustZip] [char](10) NULL,
	[HomePhone] [varchar](16) NULL,
	[WorkPhone] [varchar](16) NULL,
	[CellPhone] [varchar](16) NULL,
	[Email] [varchar](50) NULL,
	[RespondVia] [char](12) NULL,
	[IncidentDateTime] [datetime] NULL,
	[VehNo] [char](4) NULL,
	[Route] [varchar](50) NULL,
	[Destination] [varchar](40) NULL,
	[Location] [varchar](100) NULL,
	[IncidentCity] [varchar](30) NULL,
	[Badge] [char](6) NULL,
	[EmployeeDesc] [varchar](1024) NULL,
	[ResponseRequested] [char](1) NULL,
	[Division] [char](2) NULL,
	[CustComments] [varchar](8000) NULL,
	[AssignedTo] [char](16) NULL,
	[ForAction] [varchar](34) NULL,
	[ForInfos] [varchar](256) NULL,
	[ContactStatus] [varchar](32) NULL,
	[TicketStatus] [varchar](32) NULL,
	[Resolution] [varchar](2) NULL,
	[ResolutionComment] [varchar](1024) NULL,
	[updatedBy] [varchar](16) NULL,
	[updatedOn] [datetime] NULL,
	[Reasons] [varchar](256) NULL,
	[TitleVI] [char](1) NULL,
	[LostItemCategory] [varchar](50) NULL,
	[LostItemType] [varchar](50) NULL,
	[OperatorName] [varchar](130) NULL,
 CONSTRAINT [PK_tblContacts] PRIMARY KEY NONCLUSTERED 
(
	[FileNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCustomerComplaintCodes]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCustomerComplaintCodes](
	[ComplaintCode] [varchar](5) NOT NULL,
	[ComplaintGroup] [char](12) NULL,
	[ComplaintCategory] [varchar](50) NULL,
	[Description] [char](40) NULL,
	[PastDueDays] [int] NULL,
	[Order] [int] NULL,
	[IsVisible] [bit] NULL,
 CONSTRAINT [PK_tblCustomerComplaintCodes] PRIMARY KEY CLUSTERED 
(
	[ComplaintCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCustomerReferenceCodes]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCustomerReferenceCodes](
	[REFER_CODE] [varchar](5) NOT NULL,
	[REFER_DESC] [varchar](30) NULL,
	[Email] [varchar](50) NULL,
	[Order] [int] NULL,
	[IsVisible] [bit] NULL,
 CONSTRAINT [PK_tblCustomerReferenceCodes] PRIMARY KEY CLUSTERED 
(
	[REFER_CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblIncidentUpdateHistory]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblIncidentUpdateHistory](
	[FileNum] [int] NOT NULL,
	[UserId] [varchar](16) NULL,
	[UpdatedOn] [datetime] NULL,
	[LastIncidentDateTime] [datetime] NULL,
	[LastVehNo] [char](4) NULL,
	[LastRoute] [char](4) NULL,
	[LastBadge] [char](6) NULL,
	[NewIncidentDateTime] [datetime] NULL,
	[NewVehNo] [char](4) NULL,
	[NewRoute] [char](4) NULL,
	[NewBadge] [char](6) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblIncidentUpdateHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblLinkedContacts]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblLinkedContacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileNum] [int] NOT NULL,
	[LinkedFileNum] [int] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_tblLinkedContacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblOpenContacts]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblOpenContacts](
	[FileNum] [int] NULL,
	[RefCode] [char](1) NULL,
	[ComplaintCode] [char](2) NULL,
	[ComplaintGroup] [char](12) NULL,
	[ComplaintCategory] [varchar](50) NULL,
	[ReceivedDateTime] [datetime] NULL,
	[PastDueDays] [int] NULL,
	[DaysPastDue] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblResearchHistory]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblResearchHistory](
	[FileNum] [int] NOT NULL,
	[UserId] [varchar](16) NULL,
	[EnteredDateTime] [datetime] NULL,
	[Comment] [nvarchar](max) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblResearchHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUpdateLog]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUpdateLog](
	[UserId] [varchar](16) NULL,
	[FileNum] [int] NOT NULL,
	[TableName] [varchar](24) NULL,
	[UpdateAction] [varchar](128) NULL,
	[DateUpdated] [datetime] NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColumnName] [varchar](128) NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblUpdateLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblViewHistory]    Script Date: 12/13/2017 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblViewHistory](
	[ViewHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[FileNum] [int] NOT NULL,
	[UserId] [varchar](16) NOT NULL,
	[DateFirstView] [datetime] NOT NULL,
	[DateLastView] [datetime] NOT NULL,
 CONSTRAINT [PK_tblViewHistory] PRIMARY KEY CLUSTERED 
(
	[ViewHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [_dta_index_AuthorizedUsers_17_114099447__K2_K7_1_3_4_5_6_8_9_10_11]    Script Date: 12/13/2017 2:56:33 PM ******/
CREATE NONCLUSTERED INDEX [_dta_index_AuthorizedUsers_17_114099447__K2_K7_1_3_4_5_6_8_9_10_11] ON [dbo].[AuthorizedUsers]
(
	[UserName] ASC,
	[Division] ASC
)
INCLUDE ( 	[UserId],
	[ClerkId],
	[AddComments],
	[AllowAssignTo],
	[AllowSecurity],
	[ReferTo],
	[Email],
	[AllowCloseTicket],
	[AllowSearchTickets]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ReferToIndex]    Script Date: 12/13/2017 2:56:33 PM ******/
CREATE NONCLUSTERED INDEX [ReferToIndex] ON [dbo].[AuthorizedUsers]
(
	[ReferTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserIdIndex]    Script Date: 12/13/2017 2:56:33 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserIdIndex] ON [dbo].[AuthorizedUsers]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_dbo_tblUpdateLog_FileNum]    Script Date: 12/13/2017 2:56:33 PM ******/
CREATE NONCLUSTERED INDEX [IX_dbo_tblUpdateLog_FileNum] ON [dbo].[tblUpdateLog]
(
	[FileNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuthorizedUsers] ADD  DEFAULT ('N') FOR [NotifyWhenAssigned]
GO
ALTER TABLE [dbo].[AuthorizedUsers] ADD  DEFAULT ((0)) FOR [ReminderDelayDays]
GO
ALTER TABLE [dbo].[AuthorizedUsers] ADD  CONSTRAINT [DF_AuthorizedUsers_AllowOnlyDeptTickets]  DEFAULT ('N') FOR [AllowOnlyDeptTickets]
GO
ALTER TABLE [dbo].[tblAttachmentsTemp] ADD  DEFAULT ((1)) FOR [AttachmentNum]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_ADAComplaint]  DEFAULT ('N') FOR [ADAComplaint]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_ReceivedDateTime]  DEFAULT (getdate()) FOR [ReceivedDateTime]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_IncidentDateTime]  DEFAULT (getdate()) FOR [IncidentDateTime]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_IncidentCity]  DEFAULT ('') FOR [IncidentCity]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_ResponseRequested]  DEFAULT ('N') FOR [ResponseRequested]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_ForInfos]  DEFAULT ('') FOR [ForInfos]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_ContactStatus_1]  DEFAULT ('New') FOR [ContactStatus]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_ContactStatus]  DEFAULT ('New') FOR [TicketStatus]
GO
ALTER TABLE [dbo].[tblContacts] ADD  CONSTRAINT [DF_tblContacts_updatedOn]  DEFAULT (getdate()) FOR [updatedOn]
GO
ALTER TABLE [dbo].[tblLinkedContacts] ADD  CONSTRAINT [DF_tblLinkedContacts_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tblResearchHistory] ADD  CONSTRAINT [DF_tblResolutionComments_EnteredDateTime]  DEFAULT (getdate()) FOR [EnteredDateTime]
GO
ALTER TABLE [dbo].[tblViewHistory] ADD  CONSTRAINT [DF_tblViewHistory_DateLastView]  DEFAULT (getdate()) FOR [DateLastView]
GO
ALTER TABLE [dbo].[tblAttachments]  WITH CHECK ADD  CONSTRAINT [FK_tblAttachments_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblAttachments] CHECK CONSTRAINT [FK_tblAttachments_tblContacts]
GO
ALTER TABLE [dbo].[tblAttachmentsTemp]  WITH CHECK ADD  CONSTRAINT [FK_tblAttachmentsTemp_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblAttachmentsTemp] CHECK CONSTRAINT [FK_tblAttachmentsTemp_tblContacts]
GO
ALTER TABLE [dbo].[tblContactHistory]  WITH CHECK ADD  CONSTRAINT [FK_tblContactHistory_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblContactHistory] CHECK CONSTRAINT [FK_tblContactHistory_tblContacts]
GO
ALTER TABLE [dbo].[tblIncidentUpdateHistory]  WITH CHECK ADD  CONSTRAINT [FK_tblIncidentUpdateHistory_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblIncidentUpdateHistory] CHECK CONSTRAINT [FK_tblIncidentUpdateHistory_tblContacts]
GO
ALTER TABLE [dbo].[tblLinkedContacts]  WITH CHECK ADD  CONSTRAINT [FK_tblLinkedContacts_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblLinkedContacts] CHECK CONSTRAINT [FK_tblLinkedContacts_tblContacts]
GO
ALTER TABLE [dbo].[tblLinkedContacts]  WITH CHECK ADD  CONSTRAINT [FK_tblLinkedContacts_tblContacts1] FOREIGN KEY([LinkedFileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblLinkedContacts] CHECK CONSTRAINT [FK_tblLinkedContacts_tblContacts1]
GO
ALTER TABLE [dbo].[tblResearchHistory]  WITH CHECK ADD  CONSTRAINT [FK_tblResearchHistory_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblResearchHistory] CHECK CONSTRAINT [FK_tblResearchHistory_tblContacts]
GO
ALTER TABLE [dbo].[tblUpdateLog]  WITH CHECK ADD  CONSTRAINT [FK_tblUpdateLog_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblUpdateLog] CHECK CONSTRAINT [FK_tblUpdateLog_tblContacts]
GO
ALTER TABLE [dbo].[tblViewHistory]  WITH CHECK ADD  CONSTRAINT [FK_tblViewHistory_tblContacts] FOREIGN KEY([FileNum])
REFERENCES [dbo].[tblContacts] ([FileNum])
GO
ALTER TABLE [dbo].[tblViewHistory] CHECK CONSTRAINT [FK_tblViewHistory_tblContacts]
GO
/****** Object:  StoredProcedure [dbo].[addCustomerContact]    Script Date: 12/13/2017 2:56:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================================================================================
-- Updated By:	SAparicio
-- Update date: 10/26/2016
-- Description:	Updated @Route input parameter from char(4) to varchar(50)
-- Test:
-- DECLARE @FeedbackId int
-- DECLARE @ClerkId varchar(16)
-- DECLARE @UserId varchar(16)
-- DECLARE @ADAComplaint char(1)
-- DECLARE @TitleVI char(1)
-- DECLARE @ContactSource varchar(16)
-- DECLARE @ReceivedDateTime datetime
-- DECLARE @Priority char(6)
-- DECLARE @FirstName varchar(30)
-- DECLARE @LastName varchar(30)
-- DECLARE @Addr1 varchar(30)
-- DECLARE @Addr2 varchar(30)
-- DECLARE @CustCity varchar(50)
-- DECLARE @CustState char(2)
-- DECLARE @CustZip char(10)
-- DECLARE @HomePhone varchar(16)
-- DECLARE @WorkPhone varchar(16)
-- DECLARE @FaxPhone varchar(16)
-- DECLARE @Email varchar(50)
-- DECLARE @RespondVia char(12)
-- DECLARE @IncidentDateTime datetime
-- DECLARE @VehNo char(4)
-- DECLARE @Route varchar(50)
-- DECLARE @Destination varchar(40)
-- DECLARE @Location varchar(60)
-- DECLARE @Badge char(6)
-- DECLARE @EmployeeDesc varchar(1024)
-- DECLARE @ResponseRequested char(1)
-- DECLARE @Division char(2)
-- DECLARE @Reasons varchar(256)
-- DECLARE @ForAction varchar(34)
-- DECLARE @ForInfos varchar(256)
-- DECLARE @AssignedTo char(16)
-- DECLARE @ContactStatus char(16)
-- DECLARE @TicketStatus char(16)
-- DECLARE @CustComments varchar(8000)
-- DECLARE @UpdatedBy char(16)
-- DECLARE @LostItemCategory varchar(50)
-- DECLARE @LostItemType varchar(50)
-- DECLARE @FileBase64Data varchar(max)
-- DECLARE @FileName varchar(128)
-- DECLARE @FileSize int
-- DECLARE @FileContentType varchar(128)
 --EXECUTE [dbo].[addCustomerContact] @FeedbackId, @ClerkId, @UserId, @ADAComplaint, @TitleVI, @ContactSource, @ReceivedDateTime, @Priority,
  --								  @FirstName, @LastName, @Addr1, @Addr2, @CustCity, @CustState, @CustZip, @HomePhone, @WorkPhone, @FaxPhone,
  --								  @Email, @RespondVia, @IncidentDateTime, @VehNo, @Route, @Destination, @Location, @Badge, @EmployeeDesc,
  --								  @ResponseRequested, @Division, @Reasons, @ForAction, @ForInfos, @AssignedTo ,@ContactStatus, @TicketStatus,
  --								  @CustComments, @UpdatedBy, @LostItemCategory, @LostItemType, @FileBase64Data, @FileName, @FileSize, @FileContentType

-- ============================================================================================================================================

CREATE PROCEDURE [dbo].[addCustomerContact] 
      @FeedbackId INT = NULL
     ,@ClerkId VARCHAR(16) = 'WEB'
     ,@UserId VARCHAR(16) = 'WEB'
     ,@ADAComplaint CHAR(01) = 'N'
     ,@TitleVI CHAR(01) = 'N'
     ,@ContactSource VARCHAR(16) = 'WEB'
     ,@ReceivedDateTime DATETIME = NULL
     ,@Priority CHAR(06) = 'Normal'
     ,@FirstName VARCHAR(30) = ' '
     ,@LastName VARCHAR(30) = ' '
     ,@Addr1 VARCHAR(30) = NULL
     ,@Addr2 VARCHAR(30) = NULL
     ,@CustCity VARCHAR(50) = NULL
     ,@CustState CHAR(02) = 'CA'
     ,@CustZip CHAR(10) = NULL
     ,@HomePhone VARCHAR(16) = NULL
     ,@WorkPhone VARCHAR(16) = NULL
     ,@FaxPhone VARCHAR(16) = NULL
     ,@Email VARCHAR(50) = NULL
     ,@RespondVia CHAR(12) = 'None'
     ,@IncidentDateTime DATETIME = DEFAULT
     ,@VehNo CHAR(04) = NULL
     ,@Route VARCHAR(50) = NULL
     ,@Destination VARCHAR(40) = NULL
     ,@Location VARCHAR(60) = NULL
     ,@Badge CHAR(06) = NULL
     ,@EmployeeDesc VARCHAR(1024) = NULL
     ,@ResponseRequested CHAR(01) = 'N'
     ,@Division CHAR(02) = ' '
     ,@Reasons VARCHAR(256) = '00. Web-Unspecified'
     ,@ForAction VARCHAR(34) = '5. Unassigned'
     ,@ForInfos VARCHAR(256) = ' '
     ,@AssignedTo CHAR(16) = NULL
     ,@ContactStatus CHAR(16) = 'New'
     ,@TicketStatus CHAR(16) = 'New'
     ,@CustComments VARCHAR(8000)
     ,@UpdatedBy CHAR(16) = 'WEB'
     ,@LostItemCategory VARCHAR(50) = NULL
     ,@LostItemType VARCHAR(50) = NULL
	 ,@FileBase64Data VARCHAR(MAX) = NULL
	 ,@FileName VARCHAR(128) = NULL
	 ,@FileSize int = NULL
	 ,@FileContentType varchar(128) = NULL
AS
BEGIN
    --IF ISDATE(@Incidentdatetime) <> 1
	--   SET @Incidentdatetime = GETDATE();

    IF @ResponseRequested = 'Y'
	   SET @Respondvia = 'Email';

    DECLARE @FileNum int

    INSERT INTO dbo.tblContacts(
	   FeedbackId, ClerkId, UserId, ADAComplaint, TitleVI, ContactSource, [Priority],
	   FirstName, LastName, Addr1, Addr2, CustCity, CustState, CustZip, 
	   HomePhone, WorkPhone, CellPhone, Email, RespondVia, 
	   IncidentDateTime, VehNo, [Route], Destination, Location, Badge, EmployeeDesc,
	   ResponseRequested, Division, Reasons, ForAction, ForInfos, AssignedTo, 
	   ContactStatus, TicketStatus, CustComments, UpdatedBy, LostItemCategory, LostItemType)
    VALUES (
	   @FeedbackId, @ClerkId, @UserId, @ADAComplaint, @TitleVI, @ContactSource, @Priority,
	   @FirstName, @LastName, @Addr1, @Addr2, @CustCity, @CustState, @CustZip,
	   @HomePhone, @WorkPhone, @FaxPhone, @Email, @RespondVia, 
	   @IncidentDateTime, @VehNo, @Route, @Destination, @Location, @Badge, @EmployeeDesc,
	   @ResponseRequested, @Division, @Reasons, @ForAction, @ForInfos, @AssignedTo,
	   @ContactStatus, @TicketStatus, @CustComments, @UpdatedBy, @LostItemCategory, @LostItemType);

    SET @FileNum = SCOPE_IDENTITY()

    IF @FileBase64Data IS NOT NULL AND @FileSize > 0
	   INSERT INTO dbo.tblAttachmentsTemp 
		  (FileNum, [FileName], FileSize, ContentType, Base64Data, DateUploaded)
	   VALUES
		  (@FileNum, @FileName, @FileSize, @FileContentType, @FileBase64Data, GETDATE())
END
GO
/****** Object:  StoredProcedure [dbo].[addCustomerInternal]    Script Date: 12/13/2017 2:56:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================================================================================
-- Updated By:	SAparicio
-- Update date: 10/26/2016
-- Description:	Updated @Route input parameter from char(4) to varchar(50)
-- Test:
--DECLARE @FeedBackId int
--DECLARE @ClerkId varchar(16)
--DECLARE @UserId varchar(16)
--DECLARE @ADAComplaint char(1)
--DECLARE @TitleVI char(1)
--DECLARE @ContactSource varchar(16)
--DECLARE @Priority char(6)
--DECLARE @FirstName varchar(30)
--DECLARE @LastName varchar(30)
--DECLARE @Addr1 varchar(30)
--DECLARE @Addr2 varchar(30)
--DECLARE @CustCity varchar(50)
--DECLARE @CustState char(2)
--DECLARE @CustZip char(10)
--DECLARE @HomePhone varchar(16)
--DECLARE @WorkPhone varchar(16)
--DECLARE @CellPhone varchar(16)
--DECLARE @Email varchar(50)
--DECLARE @RespondVia char(12)
--DECLARE @IncidentDateTime datetime
--DECLARE @VehNo char(4)
--DECLARE @Route char(4)
--DECLARE @Destination varchar(40)
--DECLARE @Location varchar(60)
--DECLARE @IncidentCity varchar(30)
--DECLARE @Badge char(6)
--DECLARE @EmployeeDesc varchar(1024)
--DECLARE @ResponseRequested char(1)
--DECLARE @Division char(2)
--DECLARE @Reasons varchar(256)
--DECLARE @ForAction varchar(34)
--DECLARE @ForInfos varchar(256)
--DECLARE @AssignedTo char(16)
--DECLARE @ContactStatus varchar(32)
--DECLARE @TicketStatus varchar(32)
--DECLARE @CustComments varchar(8000)
--DECLARE @UpdatedBy varchar(16)

--EXECUTE [dbo].[addCustomerInternal] @FeedBackId, @ClerkId, @UserId, @ADAComplaint, @TitleVI, @ContactSource, @Priority ,@FirstName, @LastName,
--									  @Addr1, @Addr2, @CustCity, @CustState, @CustZip, @HomePhone, @WorkPhone, @CellPhone, @Email, @RespondVia,
--									  @IncidentDateTime, @VehNo, @Route, @Destination, @Location, @IncidentCity, @Badge, @EmployeeDesc, @ResponseRequested,
--									  @Division, @Reasons, @ForAction, @ForInfos, @AssignedTo, @ContactStatus, @TicketStatus, @CustComments, @UpdatedBy

-- ============================================================================================================================================

CREATE PROCEDURE  [dbo].[addCustomerInternal] 
	@FeedBackId			int = null,
	@ClerkId			varchar(16) = null,
	@UserId				varchar(16) = null,
	@ADAComplaint		varchar(01) = 'N',
	@TitleVI			char(01) = 'N',
	@ContactSource		varchar(16) = null,
	@Priority			char(06) ='Normal',
	@FirstName			varchar(30) = ' ',
	@LastName 			varchar(30) = ' ',
	@Addr1  			varchar(30)	= Null,
	@Addr2				varchar(30) = Null,
	@CustCity  			varchar(50) = Null,
	@CustState 			char(02) = 'CA',
	@CustZip  			char(10) = Null,
	@HomePhone 			varchar(16) = Null,
	@WorkPhone 			varchar(16) = Null,
	@CellPhone  		varchar(16) = Null,
	@Email  			varchar(50) = Null,
	@RespondVia   		char(12) = 'None',
	@IncidentDateTime	datetime = DEFAULT,
	@VehNo          	char(04) = Null,
	@Route          	varchar(50) = Null,
	@Destination        varchar(40) = Null,
	@Location           varchar(60) = Null,
	@IncidentCity       varchar(30) = Null,
	@Badge          	char(06) = Null,
	@EmployeeDesc   	varchar(1024) = Null,
	@ResponseRequested 	char(01) = 'N',
	@Division           char(02) = ' ',
	@Reasons            varchar(256) = Null,
	@ForAction          varchar(34) = '5. Unassigned',
	@ForInfos           varchar(256) = ' ',
	@AssignedTo         char(16) = Null,
	@ContactStatus      varchar(32) = 'New',
	@TicketStatus       varchar(32) = 'New',
	@CustComments   	varchar(8000),
	@UpdatedBy          varchar(16) = Null
AS

If IsDate(@IncidentDateTime)  <> 1
  set @IncidentDateTime = getdate()

insert into tblContacts (FeedbackId,ClerkId,UserId,ADAComplaint,TitleVI,ContactSource,Priority,FirstName,LastName,
        Addr1,Addr2,CustCity,CustState,CustZip,HomePhone,WorkPhone,CellPhone,Email,RespondVia,
        IncidentDateTime,VehNo,Route,Destination,Location,IncidentCity,Badge,EmployeeDesc,ResponseRequested,Division,
        Reasons,ForAction,ForInfos,AssignedTo,ContactStatus,TicketStatus,CustComments,UpdatedBy)
          
        values(@FeedbackId,@ClerkId,@UserId,@ADAComplaint,@TitleVI,@ContactSource,@Priority, @FirstName,@LastName,
		@Addr1,@Addr2,@CustCity,@CustState,@CustZip,@HomePhone,@WorkPhone,@CellPhone,@Email,@RespondVia,
		@IncidentDateTime,@VehNo,@Route,@Destination,@Location,@IncidentCity,@Badge,@EmployeeDesc,@ResponseRequested,@Division,
		@Reasons,@ForAction,@ForInfos,@AssignedTo,@ContactStatus,@TicketStatus,@CustComments,@UpdatedBy)
GO
/****** Object:  StoredProcedure [dbo].[CusrelOpenStats]    Script Date: 12/13/2017 2:56:34 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[CusrelOpenStats] AS
create table #OpenStats (ForAction char(34), NumUpto30 int,NumFrom31To60 int, NumOver60 int)
insert into #OpenStats
  select  (REFER_CODE + '. ' + rtrim(REFER_DESC)),0,0,0
    from dbo.tblCustomerReferenceCodes


update #OpenStats 
  set NumUpto30 = (select count(*) from dbo.tblContacts
                    where datediff(dd,ReceivedDateTime,getdate()) <= 30 
                      and ResolvedDateTime is null
                      and left(Foraction,1) = left(#OpenStats.Foraction,1))

update #OpenStats 
  set NumFrom31To60 = (select count(*) from dbo.tblContacts
                    where datediff(dd,ReceivedDateTime,getdate()) between 31 and 60 
                      and ResolvedDateTime is null
                      and left(Foraction,1) = left(#OpenStats.Foraction,1))

update #OpenStats 
  set NumOver60 = (select count(*) from dbo.tblContacts
                    where datediff(dd,ReceivedDateTime,getdate()) > 60
                      and ResolvedDateTime is null
                      and left(Foraction,1) = left(#OpenStats.Foraction,1))

select ForAction, NumUpto30, NumFrom31To60, NumOver60
  from  #OpenStats
   where NumUpto30     > 0 
      or NumFrom31To60 > 0 
      or NumOver60     > 0
GO
/****** Object:  StoredProcedure [dbo].[TrackFileView]    Script Date: 12/13/2017 2:56:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================================================================================
-- Created By:	SAparicio
-- Description:	Inserts or Updates a record indicating a ticket view into the tblViewHistory table 
-- Test: EXEC [dbo].[TrackFileView]
-- ============================================================================================================================================


CREATE PROCEDURE [dbo].[TrackFileView]
	@FileNum INT,
	@UserId VARCHAR(16),
	@DateView DATETIME
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF EXISTS(SELECT FileNum FROM tblViewHistory WHERE FileNum = @FileNum AND UserId = @UserId)
		BEGIN
			UPDATE tblViewHistory
			SET DateLastView = @DateView
			WHERE FileNum = @FileNum AND UserId = @UserId
		END
	ELSE
		BEGIN
			INSERT INTO tblViewHistory (FileNum, UserId, DateFirstView, DateLastView)
			VALUES					   (@FileNum, @UserId, @DateView, @DateView)
		END
	
END
GO
USE [master]
GO
ALTER DATABASE [CusRel] SET  READ_WRITE 
GO


USE [CusRel]
GO

INSERT [dbo].[AuthorizedUsers] ([UserId], [UserName], [ClerkId], [AddComments], [AllowAssignTo], [AllowSecurity], [Division], [ReferTo], [Email], [AllowCloseTicket], [AllowSearchTickets], [AllowViewUnassigned], [NotifyWhenAssigned], [ReminderDelayDays], [AllowOnlyDeptTickets])
VALUES 
(N'bsmith', N'Bob Smith', NULL, N'Y', N'Y', N'Y', N'GO', N'A', N'bsmith@your.company.url', N'Y', N'Y', N'Y', N'Y', 1, NULL)
GO


SET IDENTITY_INSERT [dbo].[Settings] ON 
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (1, NULL, N'HomeContent', N'<div class="row">
<h2 style="text-align: center;"><span style="color: #000000;"><strong>Welcome to the Customer Relations database!</strong></span></h2>
</div>')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (2, NULL, N'EmailHighPriorityChange', N'BCC:CusRelTeam@your.company.url|Subject:Customer Relations HIGH PRIORITY ISSUE - File Number ''{Ticket.Id}''|Body:File Number ''{Ticket.Id}'' in the Customer Relations application has just been modified and it has a HIGH priority.  The reasons are: {Ticket.Reasons}<br />
<br />
Customer comments begin with:<br />
<br />
{Ticket.Comments}<br />
<br />
<br />
This is an automated message - click here to access the contact resolution system for details:<br />
<a href="http://your.company.url/CusRel/Ticket/Update/{Ticket.Id}">http://your.company.url/CusRel/Ticket/Update/{Ticket.Id}</a>')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (3, NULL, N'LostProperty', N'|Bags and Luggage: Backpack; Briefcase; Duffle bag; Lunchbox; Messenger Bag; Purse; Shopping Bag; Suitcase; Other |Books, Paperwork, Writing Instruments: Folder/Binder; Library Book; Paperback or Hardcover Book; Paperwork; Pen/Pencil; Photos; Textbook; Other |Clothing: Jacket/Coat; Shoes/Boots; Gloves; Hat; Scarf; Sweatshirt; Shirt; Pants; Other |Electronics: Audio Player; Cell Phone; Computer; eReader; Laptop; Tablet; Video Game; Media (CD, DVD, Flash drive, etc.); Accessory (Headphones, Cables, etc.); Other |Jewelry and Cash: U.S. Currency; Foreign Currency; Bracelet; Earrings; Necklace; Ring; Watch; Other |Medical: Cane; Crutches; Hearing Aid; Medication; Walker; Other |Personal Items: Bank Card; Eyeglasses; Keys; Membership Card; Passport; Purse; State-Issued ID/Driver''s License; Sunglasses; Umbrella; Wallet; Work ID/Badge; Other |Sporting Equipment, Toys, Musical Instruments: Bicycle; Helmet; Musical Instrument; Skateboard; Scooter; Skates/Rollerblades; Toy; Other |Tickets and Passes: Event Tickets; Clipper Card; Youth Clipper Card; Senior Clipper Card; RTC Card; Other Transit Agency Pass or Ticket |Other: Weapon; Perishables/Food; Other')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (4, NULL, N'FlexRoute', N'FLEX - Newark, FLEX - Castro Valley')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (5, NULL, N'FlexRouteDivision', N'D6')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (6, NULL, N'FlexRouteDirection', N'')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (7, NULL, N'SmtpSettings', N'HOST:your.smtp.server.fqdn|FROM:CustomerRelations@your.company.url')
GO
INSERT [dbo].[Settings] ([SettingId], [GroupId], [Name], [Value]) VALUES (8, NULL, N'EmailTicketAssigned', N'SUBJECT:File Number {Ticket.Id} has been assigned to you|BODY:Greetings - <br /><br />File Number {Ticket.Id} in the Customer Relations application has been assigned to you with a {Ticket.Priority} priority. <br /> <br />This is an automated message - click on the following link to access the contact resolution system for details <br /><a href="http://your.company.url/Ticket/Update/{Ticket.Id}">http://your.company.url/Ticket/Update/{Ticket.Id}</a>')
GO
SET IDENTITY_INSERT [dbo].[Settings] OFF
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'00', N'OTHER       ', N'Other       ', N'Web-Unspecified                         ', 30, 46, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'01', N'OTHER       ', N'Schedule Adherence (Late/No Show / Early)', N'Bunching                                ', 30, 6, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'02', N'OTHER       ', N'Schedule Adherence (Late/No Show / Early)', N'No Show                                 ', 30, 32, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'03', N'OTHER       ', N'Schedule Adherence (Late/No Show / Early)', N'Sharp                                   ', 30, 41, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'04', N'OTHER       ', N'Schedule Adherence (Late/No Show / Early)', N'Late                                    ', 30, 28, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'05', N'OTHER       ', N'Pass Up', N'Pass-Up                                 ', 30, 36, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'06', N'OTHER       ', N'Pass Up', N'Carry-By                                ', 30, 13, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'07', N'OTHER       ', N'Pass Up', N'Turnback                                ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'08', N'SAFETY      ', N'Pass Up', N'Improper/Unauthorized Stop              ', 10, 25, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'09', N'OTHER       ', N'Pass Up', N'Off Route                               ', 30, 33, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'10', N'SAFETY      ', N'Unsafe Operations', N'Boarding/Alighting                      ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'11', N'RISK        ', N'Unsafe Operations', N'Hazardous Operation                     ', 10, 23, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'12', N'SAFETY      ', N'Unsafe Operations', N'Bus Overloaded                          ', 10, 9, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'13', N'OTHER       ', N'Miscellaneous', N'Fare Dispute                            ', 10, 22, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'14', N'OTHER       ', N'Miscellaneous', N'Routes & Schedules                      ', 30, 38, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'15', N'OTHER       ', N'Schedule Adherence (Late/No Show / Early)', N'Insufficient Running Time               ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'16', N'RISK        ', N'Operator Discourtesy/ Conduct', N'Conduct/Discourtesy                     ', 10, 17, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'17', N'OTHER       ', N'Pass Up', N'Boarding Denied                         ', 10, 5, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'18', N'OTHER       ', N'Miscellaneous', N'Lack Of Information                     ', 30, 27, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'19', N'SAFETY      ', N'Miscellaneous', N'Signage                                 ', 10, 42, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'20', N'RISK        ', N'Equipment/Infrastructure/System', N'Bus Maintenance                         ', 30, 8, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'21', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Heating/Ventilation                     ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'22', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Untidy Bus                              ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'23', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Bus Stop                                ', 10, 11, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'24', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Bus Shelters                            ', 30, 10, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'25', N'RISK        ', N'Unsafe Operations', N'Injury Claimed                          ', 10, 26, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'26', N'OTHER       ', N'Equipment/Infrastructure/System', N'Noise/Radios                            ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'27', N'RISK        ', N'Miscellaneous', N'Crime/Vandalism/Security                ', 10, 18, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'28', N'OTHER       ', N'Unsafe Operations', N'Smoking On Bus                          ', 10, 43, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'29', N'OTHER       ', N'Miscellaneous', N'Eating On Bus                           ', 10, 19, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'30', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Call Center Feedback                    ', 10, 12, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'31', N'SAFETY      ', N'Miscellaneous', N'Clipper And Passes                      ', 10, 15, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'32', N'SAFETY      ', N'Operator Discourtesy/ Conduct', N'PBX-Discourtesy                         ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'33', N'SAFETY      ', N'Miscellaneous', N'Routes & Schedules                      ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'34', N'SAFETY      ', N'Miscellaneous', N'Routes & Schedules                      ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'35', N'SAFETY      ', N'Miscellaneous', N'Routes & Schedules                      ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'36', N'SAFETY      ', N'Miscellaneous', N'Routes & Schedules                      ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'37', N'SAFETY      ', N'Miscellaneous', N'Lost Property                           ', 10, 29, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'38', N'OTHER       ', N'Operator Discourtesy/ Conduct', N'Uniform                                 ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'39', N'OTHER       ', N'Commendation', N'Commendation                            ', 30, 16, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'40', N'OTHER       ', N'Schedule Adherence (Late/No Show / Early)', N'Cancellation                            ', 30, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'41', N'OTHER       ', N'Operator Discourtesy/ Conduct', N'End Of Line Violation                   ', 10, 20, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'42', N'SAFETY      ', N'Miscellaneous', N'Witness                                 ', 10, 47, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'43', N'RISK        ', N'Unsafe Operations', N'Alcohol/Drugs                           ', 10, 3, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'44', N'OTHER       ', N'Operator Discourtesy/ Conduct', N'Poor Appearance                         ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'45', N'RISK        ', N'Operator Discourtesy/ Conduct', N'Sexual Harassment/Misconduct            ', 10, 40, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'46', N'SAFETY      ', N'Unsafe Operations', N'Accident                                ', 10, 1, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'47', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Maintenance Conduct                     ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'48', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Maintenance Operations                  ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'49', N'SAFETY      ', N'Miscellaneous', N'General                                 ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'50', N'SAFETY      ', N'Miscellaneous', N'Other                                   ', 10, 34, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'51', N'OTHER       ', N'Equipment/Infrastructure/System', N'Nextbus                                 ', 10, 31, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'64', N'RISK        ', N'Operator Discourtesy/ Conduct', N'Cell Phone                              ', 10, 14, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'65', N'OTHER       ', N'Miscellaneous', N'Idling                                  ', 10, 24, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'66', N'SAFETY      ', N'Miscellaneous', N'Routes & Schedules                      ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'67', N'SAFETY      ', N'Miscellaneous', N'Fare Changes                            ', 10, 21, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'68', N'SAFETY      ', N'Equipment/Infrastructure/System', N'Bus Design                              ', 10, 7, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'69', N'OTHER       ', N'Miscellaneous', N'Bicycle                                 ', 10, 4, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'80', N'ADA         ', N'Operator Discourtesy/ Conduct', N'ADA-Kneeler                             ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'81', N'ADA         ', N'Unsafe Operations', N'ADA-Securement Issue                    ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'82', N'ADA         ', N'Schedule Adherence (Late/No Show / Early)', N'ADA-Call Stop Issue                     ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'83', N'ADA         ', N'Miscellaneous', N'ADA-Priority Seating Issue              ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'84', N'ADA         ', N'Operator Discourtesy/ Conduct', N'ADA-Conduct/Discourtesy                 ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'85', N'ADA         ', N'Equipment/Infrastructure/System', N'ADA-Lift/Ramp Issue                     ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'86', N'ADA         ', N'Miscellaneous', N'ADA-Discount Fare Dispute/Show Id       ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'87', N'ADA         ', N'Pass Up', N'ADA-Pass Up                             ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'88', N'ADA         ', N'Pass Up', N'ADA-Refused Access                      ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'89', N'ADA         ', N'Miscellaneous', N'Service Animals                         ', 10, 39, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'90', N'ADA         ', N'Pass Up', N'ADA-Carried Beyond Stop                 ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'91', N'ADA         ', N'Unsafe Operations', N'ADA-Boarding And Alighting Issue        ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'92', N'ADA         ', N'Unsafe Operations', N'ADA-Hazardous Operation                 ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'93', N'ADA         ', N'Equipment/Infrastructure/System', N'ADA-Related Equipment Or Signage        ', 10, 2, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'94', N'ADA         ', N'Miscellaneous', N'Paratransit                             ', 10, 35, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'95', N'ADA         ', N'Miscellaneous', N'ADA-Other                               ', 10, NULL, 0)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'96', N'OTHER       ', N'Equipment/Infrastructure/System', N'Web Page/Mobile Site                    ', 30, 45, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'97', N'ADA         ', N'Other', N'Reasonable Modification/Accommodation   ', 30, 37, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'98', N'OTHER       ', N'Equipment/Infrastructure/System', N'Map/Timetable Request                   ', 30, 30, 1)
GO
INSERT [dbo].[tblCustomerComplaintCodes] ([ComplaintCode], [ComplaintGroup], [ComplaintCategory], [Description], [PastDueDays], [Order], [IsVisible]) VALUES (N'99', N'OTHER       ', N'Miscellaneous', N'Title VI                                ', 30, 44, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'0', N'Not Referred                  ', N'CustomerRelations@your.company.url', 999, 0)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'1', N'Other                         ', N'CustomerRelations@your.company.url', 23, 0)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'2', N'Legal                         ', N'legalrecords@your.company.fqdn', 21, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'3', N'Scheduling', N'some-user2@your.company.fqdn', 26, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'4', N'Customer Relations', N'CustomerRelations@your.company.url', 9, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'5', N'Unassigned', N'CustomerRelations@your.company.url', 33, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'6', N'Bus Stops                     ', N'some-user1@your.company.fqdn', 4, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'7', N'Web Master', N'some-user3@your.company.fqdn', 34, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'8', N'Bus Comments                  ', N'some-user4@your.company.fqdn', 2, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'9', N'Title VI', N'some-user5@your.company.fqdn', 31, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'A', N'Information Services', N'some-user6@your.company.fqdn', 20, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'AA', N'Technical Services', N'some-user7@your.company.fqdn', 29, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'AB', N'Central Maintenance', N'some-user8@your.company.fqdn', 5, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'B', N'Chief Trans. Officer          ', N'some-user9@your.company.fqdn', 7, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'C', N'Maintenance Lift Supervisor   ', N'some-user10@your.company.fqdn', 999, 0)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'D', N'Service Supervision', N'some-user11@your.company.fqdn', 28, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'E', N'D2-Superintendent', N'some-user12@your.company.fqdn', 11, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'F', N'D2-Maintenance', N'some-user13@your.company.fqdn', 10, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'G', N'D3-Superintendent', N'some-user14@your.company.fqdn', 13, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'H', N'D3-Maintenance', N'some-user15@your.company.fqdn', 12, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'I', N'D4-Superintendent', N'some-user16@your.company.fqdn', 15, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'J', N'D4-Maintenance', N'some-user17@your.company.fqdn', 14, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'K', N'D6-Superintendent', N'some-user18@your.company.fqdn', 17, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'L', N'D6-Maintenance', N'some-user19@your.company.fqdn', 16, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'M', N'Director of Transportation    ', N'some-user20@your.company.fqdn', 18, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'N', N'Planning', N'some-user21@your.company.fqdn', 24, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'O', N'Marketing                     ', N'some-user22@your.company.fqdn', 22, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'P', N'Clipper and Passes', N'some-user22@your.company.fqdn', 8, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'Q', N'Ticket Office', N'lostandfound@your.company.fqdn', 30, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'R', N'ADA Coordinator               ', N'some-user23@your.company.fqdn', 10, 0)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'S', N'Human Resources', N'some-user24@your.company.fqdn', 19, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'T', N'Traffic/Schedules             ', N'some-user25@your.company.fqdn', 999, 0)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'U', N'Training Center               ', N'some-user26your.company.fqdn', 32, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'V', N'Accessible Services           ', N'some-user27@your.company.fqdn', 1, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'W', N'Chief of Protective Services', N'some-user28@your.company.fqdn', 6, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'X', N'Risk/Claims                   ', N'some-user29@your.company.fqdn', 25, 1)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'Y', N'Bus Shelter Maintenance       ', N'some-user30@your.company.fqdn', 3, 0)
GO
INSERT [dbo].[tblCustomerReferenceCodes] ([REFER_CODE], [REFER_DESC], [Email], [Order], [IsVisible]) VALUES (N'Z', N'School Trips Manager          ', N'some-user31@your.company.fqdn', 27, 1)
GO


CREATE TRIGGER [dbo].[tblContacts_AuditTrigger] ON [dbo].[tblContacts]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @bit INT,
    @field INT,
    @maxfield INT,
    @char INT,
    @fieldname VARCHAR(128),
    @TableName VARCHAR(128),
    @PKCols VARCHAR(1000),
    @sql NVARCHAR(MAX),
    @UpdatedOn VARCHAR(32),
    @UpdatedBy VARCHAR(128),
    @Type CHAR(1),
    @PKSelect VARCHAR(1000), 
    @params nvarchar(max),
    @TableTitle VARCHAR(128)

-- 1. You will need to change @TableName to match the table to be audited
-- 2. Check excluded columns prior to EXEC(@SQL)
-- 3. SQL Azure: make sure temp tables always includes correct columns ('SELECT * INTO #ins' doesn't work)
SELECT @TableName = 'tblContacts', @TableTitle = 'Contact'

IF EXISTS(SELECT COUNT(1) 
          FROM inserted
          HAVING COUNT(1) > 1) 
    OR EXISTS(SELECT COUNT(1) 
          FROM deleted
          HAVING COUNT(1) > 1) 
BEGIN
    --RAISERROR('ignoring multi-record set on table %s',	16, -1,	@TableName)
    RETURN
END

-- date and user
SELECT @UpdatedBy = SYSTEM_USER, @UpdatedOn = CONVERT(varchar(11), CONVERT(datetime, GETDATE(), 103), 101) + ' ' + LTRIM(RIGHT(CONVERT(varchar(20), GETDATE(), 22), 11))

-- Action
IF EXISTS (SELECT * FROM inserted)
    IF EXISTS (SELECT * FROM deleted)
        SELECT @Type = 'U'
    ELSE
        SELECT @Type = 'I'
ELSE
    SELECT @Type = 'D'

-- get real date/time and real user name
IF @Type = 'I' OR @Type = 'U'
BEGIN
    --SELECT @UpdatedOn = (CASE WHEN UpdatedOn IS NULL THEN @UpdatedOn ELSE UpdatedOn END) FROM inserted
    SELECT @UpdatedBy = (CASE WHEN UpdatedBy IS NULL OR LTRIM(RTRIM(UpdatedBy)) = '' THEN @UpdatedBy ELSE UpdatedBy END) FROM inserted
END

-- shortcut to single audit record for insert/delete
IF @Type = 'I'
BEGIN
    insert into tblUpdateLog (UserId, FileNum, TableName, UpdateAction, DateUpdated, ColumnName, OldValue, NewValue)
    select
        convert(varchar(16), @UpdatedBy), FileNum, @TableTitle, 'Added', @UpdatedOn, null, null, null
    from inserted
    return
END

IF @Type = 'D'
BEGIN
    --insert into tblUpdateLog (UserId, FileNum, TableName, UpdateAction, DateUpdated, ColumnName, OldValue, NewValue)
    --select
    --	convert(varchar(16), @UpdatedBy), FileNum, @TableTitle, 'Deleted', @UpdatedOn, null, null, null
    --from deleted
    return
END

-- get list of columns
SELECT * INTO #ins FROM inserted
SELECT * INTO #del FROM deleted

-- Get primary key columns for full outer join
SELECT @PKCols = COALESCE(@PKCols + ' and', ' on') + ' i.' + c.COLUMN_NAME + ' = d.' + c.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk,
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE c
WHERE pk.TABLE_NAME = @TableName
    AND CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND c.TABLE_NAME = pk.TABLE_NAME
    AND c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME

-- Get primary key select for insert
SELECT @PKSelect = 'coalesce(i.' + COLUMN_NAME + ',d.' + COLUMN_NAME + ')'
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk,
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE c
WHERE pk.TABLE_NAME = @TableName
    AND CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND c.TABLE_NAME = pk.TABLE_NAME
    AND c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME

IF @PKCols IS NULL
BEGIN
    RAISERROR ('no PK on table %s',	16, -1,	@TableName)
    RETURN
END

SELECT @field = 0, @maxfield = MAX(ORDINAL_POSITION)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @TableName

DECLARE @insValue nvarchar(max), @delValue nvarchar(max)

WHILE @field < @maxfield
BEGIN
    SELECT @field = MIN(ORDINAL_POSITION)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = @TableName
        AND ORDINAL_POSITION > @field

    SELECT @bit = (@field - 1) % 8 + 1
    SELECT @bit = POWER(2, @bit - 1)
    SELECT @char = ((@field - 1) / 8) + 1

    SELECT @fieldname = COLUMN_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = @TableName AND ORDINAL_POSITION = @field

    --SELECT @sql = 'SELECT @out = convert(nvarchar(MAX), ' + @fieldname + ') from #del'
    --SELECT @params = N'@out nvarchar(MAX) output'
    --EXEC sp_executesql @sql, N'@out nvarchar(MAX) output', @out = @delValue output

    --SELECT @sql = 'SELECT @out = convert(nvarchar(MAX), ' + @fieldname + ') from #ins'
    --SELECT @params = N'@out nvarchar(MAX) output'
    --EXEC sp_executesql @sql, N'@out nvarchar(MAX) output', @out = @insValue output

    --print CASE WHEN @delValue = @insValue THEN 'true' ELSE 'FALSE' END + ', ' + @fieldname + ', ' + @delValue + ', ' + @insValue

    IF @fieldname NOT IN ('UpdatedOn', 'UpdatedBy') AND (SUBSTRING(COLUMNS_UPDATED(), @char, 1) & @bit > 0 OR @Type IN ('I', 'D'))
    --IF @delValue != @insValue OR (SUBSTRING(COLUMNS_UPDATED(), @char, 1) & @bit > 0 OR @Type IN ('I', 'D'))
    BEGIN

        --select coalesce(i.FileNum,d.FileNum) from #ins i full outer join #del d on i.FileNum = d.FileNum where i.Resolution <> d.Resolution or (i.Resolution is null and  d.Resolution is not null) or (i.Resolution is not null and  d.Resolution is null)

        --print '@UpdatedBy = ' + ISNULL(@UpdatedBy, 'NULL')
        --print '@PKSelect = ' + ISNULL(@PKSelect, 'NULL')
        --print '@TableName = ' + ISNULL(@TableName, 'NULL') 
        --print '@Type = ' + ISNULL(@Type, 'NULL')
        --print '@UpdatedOn = ' + ISNULL(@UpdatedOn, 'NULL')
        --print '@fieldname = ' + ISNULL(@fieldname, 'NULL')
        --print '@PKCols = ' + ISNULL(@PKCols, 'NULL')
        SELECT @sql = '
insert tblUpdateLog (
    UserId, 
    FileNum, 
    TableName, 
    UpdateAction, 
    DateUpdated, 
    ColumnName, 
    OldValue,
    NewValue)
select ''' + convert(varchar(16), @UpdatedBy) + ''',' + @PKSelect + ',''' + @TableTitle + ''',''' + 'Updated' + ''',''' + @UpdatedOn + ''',''' + @fieldname + ''',convert(nvarchar(MAX),d.' + @fieldname + ')' + ',convert(nvarchar(MAX),i.' + @fieldname + ') from #ins i full outer join #del d' + @PKCols + ' where i.' + @fieldname + ' <> d.' + @fieldname + ' or (i.' + @fieldname + ' is null and  d.' + @fieldname + ' is not null)' + ' or (i.' + @fieldname + ' is not null and  d.' + @fieldname + ' is null)'

        --print @sql
        EXEC (@sql)
    END
END
GO

ALTER TABLE [dbo].[tblContacts] ENABLE TRIGGER [tblContacts_AuditTrigger]
GO


