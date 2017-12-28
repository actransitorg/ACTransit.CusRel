USE [GTFS]
GO


--########################################################
-- PART 1
--########################################################


USE master;
GO

-------------------------------------------------------------------------------
-- Enable CLR on the Server

sp_configure 'show advanced options', 1;  
GO  
RECONFIGURE;  
GO  
sp_configure 'clr enabled', 1;  
GO  
RECONFIGURE;  
GO 

----------------------------------------------------------------------------------------------------
-- Create strong signing key (assumes P:\ACTransit.Projects is your TFS $/ACTransit.Projects folder)

IF NOT EXISTS (SELECT 1 FROM sys.asymmetric_keys WHERE [Name] LIKE '%CLRBuildInfoKey%')
BEGIN
	CREATE ASYMMETRIC KEY CLRBuildInfoKey
	FROM FILE = 'P:\ACTransit.Projects\trunk\ACTransit.CLR\FileUtility\FileUtility2016\strongKey.snk'
	ENCRYPTION BY PASSWORD = 'SomePasswordHere123'
END
GO

IF NOT EXISTS (SELECT 1 FROM syslogins WHERE [Name] like '%CLRBuildInfoLogin%')
BEGIN
	CREATE LOGIN CLRBuildInfoLogin FROM ASYMMETRIC KEY CLRBuildInfoKey;
	GRANT UNSAFE ASSEMBLY TO CLRBuildInfoLogin; 
END
GO

-------------------------------------------------------------------------------
-- Enable TrustWorthy on the database or create a key and login for the DLL

USE GTFS;
GO
ALTER DATABASE GTFS SET TRUSTWORTHY ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE [Name] LIKE '%CLRBuildInfo%')
BEGIN
	CREATE USER CLRBuildInfo FOR LOGIN CLRBuildInfoLogin
END
GO

-------------------------------------------------------------------------------
-- Add the assemblies

IF OBJECT_ID('.dbo.UtilityCreateEmptyFile') IS NOT NULL DROP PROCEDURE dbo.UtilityCreateEmptyFile
GO
IF OBJECT_ID('.dbo.UtilityCreateDirectory') IS NOT NULL DROP PROCEDURE dbo.UtilityCreateDirectory
GO
IF OBJECT_ID('.dbo.UtilityCreateZipFile') IS NOT NULL DROP PROCEDURE dbo.UtilityCreateZipFile
GO
IF OBJECT_ID('.dbo.UtilityCopyFile') IS NOT NULL DROP PROCEDURE dbo.UtilityCopyFile
GO
IF OBJECT_ID('.dbo.UtilityDeleteFile') IS NOT NULL DROP PROCEDURE dbo.UtilityDeleteFile
GO
IF OBJECT_ID('.dbo.UtilityDeleteDirectory') IS NOT NULL DROP PROCEDURE dbo.UtilityDeleteDirectory
GO
IF OBJECT_ID('.dbo.UtilityRenameDirectory') IS NOT NULL DROP PROCEDURE dbo.UtilityRenameDirectory
GO
IF OBJECT_ID('.dbo.UtilityRenameFile') IS NOT NULL DROP PROCEDURE dbo.UtilityRenameFile
GO
IF OBJECT_ID('.dbo.UtilityUnzipFile') IS NOT NULL DROP PROCEDURE dbo.UtilityUnzipFile
GO
IF OBJECT_ID('.dbo.UtilityUnzipFileToPath') IS NOT NULL DROP PROCEDURE dbo.UtilityUnzipFileToPath
GO

DROP ASSEMBLY IF EXISTS [FileUtilityAssembly]
GO
DROP ASSEMBLY IF EXISTS [System.IO.Compression.ZipFile]
GO
DROP ASSEMBLY IF EXISTS [System.IO.Compression.FileSystem]
GO
DROP ASSEMBLY IF EXISTS [System.IO.Compression]
GO

CREATE ASSEMBLY [System.IO.Compression]  
AUTHORIZATION [CLRBuildInfo] 
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IO.Compression.dll' -- might need to change paths with future .NET framework upgrades (e.g. 5.0)
WITH PERMISSION_SET = UNSAFE; 
GO 

CREATE ASSEMBLY [System.IO.Compression.FileSystem]
AUTHORIZATION [CLRBuildInfo] 
FROM N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IO.Compression.FileSystem.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE ASSEMBLY FileUtilityAssembly
AUTHORIZATION [CLRBuildInfo] 
FROM 'P:\ACTransit.Projects\trunk\ACTransit.CLR\FileUtility\FileUtility2016\bin\Debug\FileUtility.dll'  -- NEED TO COMPILE FIRST
WITH PERMISSION_SET = UNSAFE 
GO

-------------------------------------------------------------------------------
-- Create the procedures to execute the assemblies


CREATE PROCEDURE dbo.UtilityCreateDirectory
(
	 @Directory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCreateDirectory
				@Directory = N'C:\test-directory'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CreateDirectory
GO



CREATE PROCEDURE dbo.UtilityCreateEmptyFile
(
	 @File NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCreateEmptyFile
				@file = N'C:\test-directory\test.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CreateEmptyFile
GO



CREATE PROCEDURE dbo.UtilityCreateZipFile
(
	 @SourceDirectoryName NVARCHAR(MAX)
	,@DestinationArchiveFileName NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCreateZipFile
				@SourceDirectoryName = N'C:\test-directory'
			   ,@DestinationArchiveFileName = N'C:\test-directory\test.zip'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CreateZipFile
GO



CREATE PROCEDURE dbo.UtilityCopyFile
(
	 @SourceFile NVARCHAR(MAX)
	,@DestinationFile NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCopyFile
				@SourceFile = N'C:\test-directory\test.txt'
			   ,@DestinationFile = N'C:\test-directory\test-copy.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CopyFile
GO





CREATE PROCEDURE dbo.UtilityRenameFile
(
	 @SourceFile NVARCHAR(MAX),
	 @DestinationFile NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityRenameFile 
			@SourceFile = N'C:\test-directory\test-copy.txt'
		   ,@DestinationFile = N'C:\test-directory\test-renamed.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].RenameFile
GO



CREATE PROCEDURE dbo.UtilityUnzipFile
(
	 @ZipFile NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityUnzipFile 
			@ZipFile = N'C:\test-directory\test.zip'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].Unzip
GO



CREATE PROCEDURE dbo.UtilityUnzipFileToPath
(
	 @ZipFile NVARCHAR(MAX)
	,@ExtractDirectory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityUnzipFileToPath 
			@ZipFile = N'C:\test-directory\test.zip', 
			@ExtractDirectory = N'C:\test-directory\test-extract'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].UnzipToPath
GO



CREATE PROCEDURE dbo.UtilityDeleteFile
(
	 @File NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:
		
		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityDeleteFile 
			@File = N'C:\test-directory\test.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].DeleteFile
GO





CREATE PROCEDURE dbo.UtilityRenameDirectory
(
	 @SourceDirectory NVARCHAR(MAX)
    ,@DestinationDirectory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityRenameDirectory 
			@SourceDirectory = N'C:\test-directory'
		   ,@DestinationDirectory = N'C:\test-directory-renamed'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].RenameDirectory
GO



CREATE PROCEDURE dbo.UtilityDeleteDirectory
(
	 @Directory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityDeleteDirectory 
			@Directory = N'C:\test-directory-renamed'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].DeleteDirectory
GO






--########################################################
-- PART 2
--########################################################








USE [GTFS]
GO

/****** Object:  Schema [HASTUS]    Script Date: 12/13/2017 3:40:06 PM ******/
CREATE SCHEMA [HASTUS]
GO
/****** Object:  Table [dbo].[Agency]    Script Date: 12/13/2017 3:40:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agency](
	[AgencyId] [varchar](100) NOT NULL,
	[BookingId] [varchar](10) NOT NULL,
	[Name] [varchar](100) NULL,
	[Url] [varchar](256) NULL,
	[TimeZone] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Language] [varchar](20) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Agency] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[AgencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Calendar]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Calendar](
	[BookingId] [varchar](10) NOT NULL,
	[ServiceId] [varchar](100) NOT NULL,
	[Monday] [bit] NOT NULL,
	[Tuesday] [bit] NOT NULL,
	[Wednesday] [bit] NOT NULL,
	[Thursday] [bit] NOT NULL,
	[Friday] [bit] NOT NULL,
	[Saturday] [bit] NOT NULL,
	[Sunday] [bit] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Calendar] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[ServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CalendarDate]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CalendarDate](
	[BookingId] [varchar](10) NOT NULL,
	[ServiceId] [varchar](100) NOT NULL,
	[Date] [date] NOT NULL,
	[ExceptionType] [varchar](100) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_CalendarDates] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[ServiceId] ASC,
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportInfo]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportInfo](
	[ImportInfoId] [int] IDENTITY(1,1) NOT NULL,
	[CreationDateTime] [datetime] NOT NULL,
	[BookingId] [varchar](10) NULL,
	[FileName] [varchar](256) NULL,
	[EarliestServiceDate] [date] NULL,
	[LatestServiceDate] [date] NULL,
	[LastImportStepId] [int] NULL,
	[WorkingDirectory] [varchar](1024) NULL,
	[ImportDirectory] [varchar](1024) NULL,
	[AddUserId] [nvarchar](50) NOT NULL,
	[AddDateTime] [datetime2](3) NOT NULL,
	[UpdUserId] [nvarchar](50) NULL,
	[UpdDateTime] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[ImportInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportLog](
	[ImportLogId] [int] IDENTITY(1,1) NOT NULL,
	[ImportInfoId] [int] NOT NULL,
	[ImportStepId] [int] NOT NULL,
	[LogDate] [datetime2](7) NOT NULL,
	[Description] [varchar](2048) NOT NULL,
	[Successful] [bit] NOT NULL,
	[ErrorText] [varchar](max) NULL,
	[AddUserId] [nvarchar](50) NOT NULL,
	[AddDateTime] [datetime2](3) NOT NULL,
	[UpdUserId] [nvarchar](50) NULL,
	[UpdDateTime] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[ImportLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportStep]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportStep](
	[ImportStepId] [int] IDENTITY(1,1) NOT NULL,
	[StepName] [varchar](24) NOT NULL,
	[StepDescription] [varchar](1024) NOT NULL,
	[AddUserId] [nvarchar](50) NOT NULL,
	[AddDateTime] [datetime2](3) NOT NULL,
	[UpdUserId] [nvarchar](50) NULL,
	[UpdDateTime] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[ImportStepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Route]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Route](
	[RouteId] [varchar](100) NOT NULL,
	[BookingId] [varchar](10) NOT NULL,
	[AgencyId] [varchar](100) NOT NULL,
	[ShortName] [varchar](256) NULL,
	[LongName] [varchar](256) NULL,
	[RouteType] [varchar](100) NULL,
	[Url] [varchar](256) NULL,
	[TextColor] [varchar](100) NULL,
	[Color] [varchar](100) NULL,
	[Description] [varchar](512) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[RouteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shape]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shape](
	[ShapeId] [varchar](100) NOT NULL,
	[BookingId] [varchar](10) NOT NULL,
	[Sequence] [int] NOT NULL,
	[Latitude] [numeric](18, 12) NULL,
	[Longitude] [numeric](18, 12) NULL,
	[DistanceTraveled] [numeric](18, 12) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Shape] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[ShapeId] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stop]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stop](
	[StopId] [varchar](100) NOT NULL,
	[BookingId] [varchar](10) NOT NULL,
	[StopCode] [varchar](100) NULL,
	[Name] [varchar](100) NULL,
	[Description] [varchar](256) NULL,
	[Latitude] [numeric](18, 12) NULL,
	[Longitude] [numeric](18, 12) NULL,
	[ZoneId] [varchar](100) NULL,
	[Url] [varchar](256) NULL,
	[LocationType] [varchar](100) NULL,
	[ParentStation] [varchar](100) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Stop] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[StopId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StopTime]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StopTime](
	[BookingId] [varchar](10) NOT NULL,
	[TripId] [varchar](100) NOT NULL,
	[StopId] [varchar](100) NOT NULL,
	[ArrivalTime] [varchar](100) NULL,
	[DepartureTime] [varchar](100) NULL,
	[Sequence] [int] NOT NULL,
	[PickupType] [varchar](100) NULL,
	[DropOffType] [varchar](100) NULL,
	[StopHeadsign] [varchar](256) NULL,
	[DistanceTraveled] [numeric](18, 12) NULL,
	[Timepoint] [bit] NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_StopTime] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[TripId] ASC,
	[StopId] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Trip]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Trip](
	[TripId] [varchar](100) NOT NULL,
	[BookingId] [varchar](10) NOT NULL,
	[RouteId] [varchar](100) NOT NULL,
	[ServiceId] [varchar](100) NULL,
	[TripHeadsign] [varchar](256) NULL,
	[DirectionId] [varchar](100) NULL,
	[BlockId] [varchar](100) NULL,
	[ShapeId] [varchar](100) NULL,
	[AddUserId] [varchar](50) NULL,
	[AddDateTime] [datetime2](7) NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Trip] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[TripId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TripMap]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TripMap](
	[BookingId] [varchar](10) NOT NULL,
	[TripId] [int] NOT NULL,
	[GtfsTripId] [varchar](100) NOT NULL,
 CONSTRAINT [PK_TripMap] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[TripId] ASC,
	[GtfsTripId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HASTUS].[Booking]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HASTUS].[Booking](
	[BookingId] [varchar](10) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[AddUserId] [nvarchar](50) NOT NULL,
	[AddDateTime] [datetime2](3) NOT NULL,
	[UpdUserId] [nvarchar](50) NULL,
	[UpdDateTime] [datetime2](3) NULL,
	[SysRecNo] [bigint] NOT NULL,
 CONSTRAINT [pk_booking_booking] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_BookingBookingID] UNIQUE NONCLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TripMap_TripId]    Script Date: 12/13/2017 3:40:07 PM ******/
CREATE NONCLUSTERED INDEX [IX_TripMap_TripId] ON [dbo].[TripMap]
(
	[TripId] ASC
)
INCLUDE ( 	[BookingId],
	[GtfsTripId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Agency] ADD  CONSTRAINT [DF_Agency_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[Agency] ADD  CONSTRAINT [DF_Agency_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[Agency] ADD  CONSTRAINT [DF_Agency_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[Agency] ADD  CONSTRAINT [DF_Agency_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_monday]  DEFAULT ((0)) FOR [Monday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_tuesday]  DEFAULT ((0)) FOR [Tuesday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_wednesday]  DEFAULT ((0)) FOR [Wednesday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_thursday]  DEFAULT ((0)) FOR [Thursday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_friday]  DEFAULT ((0)) FOR [Friday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_saturday]  DEFAULT ((0)) FOR [Saturday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [df_calendar_sunday]  DEFAULT ((0)) FOR [Sunday]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [DF_Calendar_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [DF_Calendar_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [DF_Calendar_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[Calendar] ADD  CONSTRAINT [DF_Calendar_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[CalendarDate] ADD  CONSTRAINT [DF_CalendarDate_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[CalendarDate] ADD  CONSTRAINT [DF_CalendarDate_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[CalendarDate] ADD  CONSTRAINT [DF_CalendarDate_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[CalendarDate] ADD  CONSTRAINT [DF_CalendarDate_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[ImportInfo] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[ImportInfo] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[ImportInfo] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[ImportInfo] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[ImportLog] ADD  CONSTRAINT [DF_ImportLog_LogDate]  DEFAULT (sysdatetime()) FOR [LogDate]
GO
ALTER TABLE [dbo].[ImportLog] ADD  CONSTRAINT [DF_ImportLog_Successful]  DEFAULT ((0)) FOR [Successful]
GO
ALTER TABLE [dbo].[ImportLog] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[ImportLog] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[ImportLog] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[ImportLog] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[ImportStep] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[ImportStep] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[ImportStep] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[ImportStep] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[Shape] ADD  CONSTRAINT [DF_Shape_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[Shape] ADD  CONSTRAINT [DF_Shape_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[Shape] ADD  CONSTRAINT [DF_Shape_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[Shape] ADD  CONSTRAINT [DF_Shape_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[Stop] ADD  CONSTRAINT [DF_Stop_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[Stop] ADD  CONSTRAINT [DF_Stop_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[Stop] ADD  CONSTRAINT [DF_Stop_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[Stop] ADD  CONSTRAINT [DF_Stop_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[StopTime] ADD  CONSTRAINT [DF_StopTime_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[StopTime] ADD  CONSTRAINT [DF_StopTime_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[StopTime] ADD  CONSTRAINT [DF_StopTime_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[StopTime] ADD  CONSTRAINT [DF_StopTime_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[Trip] ADD  CONSTRAINT [DF_Trip_AddUserId]  DEFAULT (original_login()) FOR [AddUserId]
GO
ALTER TABLE [dbo].[Trip] ADD  CONSTRAINT [DF_Trip_AddDateTime]  DEFAULT (getdate()) FOR [AddDateTime]
GO
ALTER TABLE [dbo].[Trip] ADD  CONSTRAINT [DF_Trip_UpdUserId]  DEFAULT (original_login()) FOR [UpdUserId]
GO
ALTER TABLE [dbo].[Trip] ADD  CONSTRAINT [DF_Trip_UpdDateTimee]  DEFAULT (getdate()) FOR [UpdDateTime]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [dbo].[Agency]  WITH NOCHECK ADD  CONSTRAINT [FK_Agency_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[Agency] CHECK CONSTRAINT [FK_Agency_Booking]
GO
ALTER TABLE [dbo].[Calendar]  WITH NOCHECK ADD  CONSTRAINT [FK_Calendar_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[Calendar] CHECK CONSTRAINT [FK_Calendar_Booking]
GO
ALTER TABLE [dbo].[CalendarDate]  WITH NOCHECK ADD  CONSTRAINT [FK_CalendarDates_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[CalendarDate] CHECK CONSTRAINT [FK_CalendarDates_Booking]
GO
ALTER TABLE [dbo].[ImportInfo]  WITH CHECK ADD  CONSTRAINT [FK_ImportInfo_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[ImportInfo] CHECK CONSTRAINT [FK_ImportInfo_Booking]
GO
ALTER TABLE [dbo].[ImportInfo]  WITH CHECK ADD  CONSTRAINT [FK_ImportInfo_ImportStep] FOREIGN KEY([LastImportStepId])
REFERENCES [dbo].[ImportStep] ([ImportStepId])
GO
ALTER TABLE [dbo].[ImportInfo] CHECK CONSTRAINT [FK_ImportInfo_ImportStep]
GO
ALTER TABLE [dbo].[ImportLog]  WITH CHECK ADD  CONSTRAINT [FK_ImportLog_ImportInfo] FOREIGN KEY([ImportInfoId])
REFERENCES [dbo].[ImportInfo] ([ImportInfoId])
GO
ALTER TABLE [dbo].[ImportLog] CHECK CONSTRAINT [FK_ImportLog_ImportInfo]
GO
ALTER TABLE [dbo].[ImportLog]  WITH CHECK ADD  CONSTRAINT [FK_ImportLog_ImportStep] FOREIGN KEY([ImportStepId])
REFERENCES [dbo].[ImportStep] ([ImportStepId])
GO
ALTER TABLE [dbo].[ImportLog] CHECK CONSTRAINT [FK_ImportLog_ImportStep]
GO
ALTER TABLE [dbo].[Route]  WITH NOCHECK ADD  CONSTRAINT [FK_Route_Agency] FOREIGN KEY([BookingId], [AgencyId])
REFERENCES [dbo].[Agency] ([BookingId], [AgencyId])
GO
ALTER TABLE [dbo].[Route] CHECK CONSTRAINT [FK_Route_Agency]
GO
ALTER TABLE [dbo].[Route]  WITH NOCHECK ADD  CONSTRAINT [FK_Route_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[Route] CHECK CONSTRAINT [FK_Route_Booking]
GO
ALTER TABLE [dbo].[Shape]  WITH NOCHECK ADD  CONSTRAINT [FK_Shape_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[Shape] CHECK CONSTRAINT [FK_Shape_Booking]
GO
ALTER TABLE [dbo].[Stop]  WITH NOCHECK ADD  CONSTRAINT [FK_Stop_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[Stop] CHECK CONSTRAINT [FK_Stop_Booking]
GO
ALTER TABLE [dbo].[StopTime]  WITH NOCHECK ADD  CONSTRAINT [FK_StopTime_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[StopTime] CHECK CONSTRAINT [FK_StopTime_Booking]
GO
ALTER TABLE [dbo].[StopTime]  WITH NOCHECK ADD  CONSTRAINT [FK_StopTime_Stop] FOREIGN KEY([BookingId], [StopId])
REFERENCES [dbo].[Stop] ([BookingId], [StopId])
GO
ALTER TABLE [dbo].[StopTime] CHECK CONSTRAINT [FK_StopTime_Stop]
GO
ALTER TABLE [dbo].[StopTime]  WITH NOCHECK ADD  CONSTRAINT [FK_StopTime_Trip] FOREIGN KEY([BookingId], [TripId])
REFERENCES [dbo].[Trip] ([BookingId], [TripId])
GO
ALTER TABLE [dbo].[StopTime] CHECK CONSTRAINT [FK_StopTime_Trip]
GO
ALTER TABLE [dbo].[Trip]  WITH NOCHECK ADD  CONSTRAINT [FK_Trip_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_Booking]
GO
ALTER TABLE [dbo].[Trip]  WITH NOCHECK ADD  CONSTRAINT [FK_Trip_Route] FOREIGN KEY([BookingId], [RouteId])
REFERENCES [dbo].[Route] ([BookingId], [RouteId])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_Route]
GO
ALTER TABLE [dbo].[TripMap]  WITH CHECK ADD  CONSTRAINT [FK_TripMap_Booking] FOREIGN KEY([BookingId])
REFERENCES [HASTUS].[Booking] ([BookingId])
GO
ALTER TABLE [dbo].[TripMap] CHECK CONSTRAINT [FK_TripMap_Booking]
GO
ALTER TABLE [dbo].[TripMap]  WITH CHECK ADD  CONSTRAINT [FK_TripMap_GtfsTripId] FOREIGN KEY([BookingId], [GtfsTripId])
REFERENCES [dbo].[Trip] ([BookingId], [TripId])
GO
ALTER TABLE [dbo].[TripMap] CHECK CONSTRAINT [FK_TripMap_GtfsTripId]
GO
ALTER TABLE [HASTUS].[Booking]  WITH CHECK ADD  CONSTRAINT [ck_booking_StartDateLessThanEndDate] CHECK  (([StartDate]<[EndDate]))
GO
ALTER TABLE [HASTUS].[Booking] CHECK CONSTRAINT [ck_booking_StartDateLessThanEndDate]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSignupData]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteSignupData]
	@BookingId varchar(10)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRY 
		BEGIN TRANSACTION

		DELETE FROM dbo.TripMap WHERE [BookingId] = @BookingId
		DELETE FROM dbo.StopTime WHERE [BookingId] = @BookingId
		DELETE FROM dbo.Stop WHERE [BookingId] = @BookingId
		DELETE FROM dbo.Trip WHERE [BookingId] = @BookingId
		DELETE FROM dbo.Shape WHERE [BookingId] = @BookingId
		DELETE FROM dbo.Route WHERE [BookingId] = @BookingId
		DELETE FROM dbo.CalendarDate WHERE [BookingId] = @BookingId
		DELETE FROM dbo.Calendar WHERE [BookingId] = @BookingId
		DELETE FROM dbo.Agency WHERE [BookingId] = @BookingId

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH

		ROLLBACK TRANSACTION

		DECLARE @ErrorMEssage NVARCHAR(MAX) = ERROR_MESSAGE();
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[GenerateTripIdMap]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GenerateTripIdMap]
	@BookingId varchar(10)
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @CurrentTransactionCount INT 
	
	SET @CurrentTransactionCount = @@TRANCOUNT

	BEGIN TRY 
		IF(@CurrentTransactionCount = 0)
			BEGIN TRANSACTION
		ELSE 
			SAVE TRANSACTION GenerateTripIdTransaction

		IF EXISTS(SELECT TOP(1) 1 FROM dbo.TripMap WHERE BookingId = @BookingId)
			DELETE FROM dbo.TripMap WHERE BookingId = @BookingId

		INSERT INTO dbo.TripMap(BookingId, TripId, GtfsTripId)
		SELECT 
			@BookingId,
			SUBSTRING(TripId, 1, 7),
			TripId
		FROM 
			dbo.Trip
		WHERE 
			Trip.BookingId = @BookingId

		IF(@CurrentTransactionCount = 0)
			COMMIT TRANSACTION
	END TRY
	BEGIN CATCH

		IF(@CurrentTransactionCount = 0)
		BEGIN
			ROLLBACK TRANSACTION
		END
		ELSE 
		BEGIN
			IF(XACT_STATE() <> -1)
				ROLLBACK TRANSACTION GenerateTripIdTransaction
		END

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE();
        SELECT @ErrorSeverity = ERROR_SEVERITY();
        SELECT @ErrorState = ERROR_STATE();

        RAISERROR ( @ErrorMessage, @ErrorSeverity, @ErrorState );		

	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[PopulateAgency]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateAgency]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1  
	,@BookingId VARCHAR(10)
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX)
		,@FileName VARCHAR(64) = 'Agency.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSAgency') IS NOT NULL
		DROP TABLE #TempTransferGTFSAgency;

	CREATE TABLE #TempTransferGTFSAgency 
	(
		 Phone VARCHAR(20)
		,Url VARCHAR(256)
		,AgencyId VARCHAR(100)
		,Name VARCHAR(100)
		,TimeZone VARCHAR(100)
		,Language VARCHAR(20)
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSAgency
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSAgency WHERE Phone = ''agency_phone'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSAgency'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error Agency.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.Agency TARGET
	USING #TempTransferGTFSAgency SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.AgencyId = SOURCE.AgencyId

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 Name = SOURCE.Name
			,Url = SOURCE.Url
			,TimeZone = SOURCE.TimeZone
			,Phone = SOURCE.Phone
			,Language = SOURCE.Language
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,AgencyId 
			,Name 
			,Url
			,TimeZone 
			,Phone
			,Language
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,AgencyId 
			,Name 
			,Url
			,TimeZone 
			,Phone
			,Language
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'Agency MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSAgency') IS NOT NULL 
    DROP TABLE #TempTransferGTFSAgency;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[PopulateAllGTFS]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[PopulateAllGTFS]
	@ZipPathFile NVARCHAR(512) = NULL --NULL to use default your.SQL.server or your.SQL.server Paths
AS
BEGIN

SET NOCOUNT ON;
-- /*Parameters*/ DECLARE @ZipPathFile NVARCHAR(255) -- = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit.zip';

DECLARE @ExtractedFilesDirectory VARCHAR(512)
		,@DestinationCopyZip1 VARCHAR(512)
		,@DestinationCopyZip2 VARCHAR(512) 
		,@WorkingZipFileDirectory VARCHAR(512) 
		,@WorkingZipFile VARCHAR(512) 
		,@ReverseZip VARCHAR(512)
		,@ZipPath VARCHAR(512)
		,@ZipFile VARCHAR(512)
		,@Finalize BIT = 0
		,@FileExistsResult INT = 0

IF @ZipPathFile IS NULL 
	IF @@SERVERNAME = 'your.SQL.server\PROD01'
		SELECT @ZipPathFile = '\\appfs1\Interfaces\gtfs\google_transit.zip'
/*Commented out because no write permissions to your.CusRel.server and your.CusRel.server & B*/
	--ELSE IF @@SERVERNAME = 'SQLServer\Instance'
	--	SELECT @ZipPathFile = '\\your.CusRel.server\e$\Projects\GtfsImportService\Import\WatchPath\google_transit.zip'
	ELSE --Test Location
		SELECT @ZipPathFile = '\\server1\share2\GTFS\WatchPath\google_transit.zip'

EXEC master.dbo.xp_fileexist @ZipPathFile, @FileExistsResult OUTPUT

IF @FileExistsResult = 1
BEGIN --@FileExistsResult = 0

	BEGIN TRY


		IF @@SERVERNAME = 'your.SQL.server\PROD01'
			SELECT @ExtractedFilesDirectory = '\\server1\share1\Transportation\GTFS\Import\ImportService\ExtractedFiles\'
					,@WorkingZipFileDirectory = '\\server1\share1\Transportation\GTFS\Import\ImportService\WorkingDirectory\'
					,@DestinationCopyZip1 = '\\your.CusRel.server\ACTransit.Transit.GtfsFiles\'
					,@DestinationCopyZip2 = '\\your.CusRel.server\ACTransit.Transit.GtfsFiles\'
		/*Commented out because no write permissions to your.CusRel.server and your.CusRel.server & B*/
		--ELSE IF @@SERVERNAME = 'SQLServer\Instance'
		--	SELECT @ExtractedFilesDirectory = '\\your.CusRel.server\e$\Projects\GtfsImportService\Import\ExtractionPath\'
		--			,@WorkingZipFileDirectory = '\\your.CusRel.server\e$\Projects\GtfsImportService\Import\WorkingDirectory\'
		--			,@DestinationCopyZip1 = '\\your.CusRel.server\ACTransit.Transit.GtfsFiles\'
		--			,@DestinationCopyZip2 = '\\your.CusRel.server\ACTransit.Transit.GtfsFiles\'
		ELSE --Test Location
			SELECT @ExtractedFilesDirectory = '\\server1\share2\GTFS\ExtractedFiles\'
					,@WorkingZipFileDirectory = '\\server1\share2\GTFS\WorkingDirectory\'
					,@DestinationCopyZip1 = '\\server1\share2\GTFS\CopyZip1\'
					,@DestinationCopyZip2 = '\\server1\share2\GTFS\CopyZip2\'

		SELECT @ReverseZip= REVERSE(@ZipPathFile)
			  ,@ZipPath = REVERSE(SUBSTRING(@ReverseZip, CHARINDEX('\', @ReverseZip)+1, 512))
			  ,@ZipFile = REPLACE(@ZipPathFile,@ZipPath+'\','')

		BEGIN TRAN 

		DECLARE 
			@Error INT = 0
			,@AlwaysRollback BIT = 0
			,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
			,@ErrDescription VARCHAR(255)
			,@YYYYMMDD_HHMMSS VARCHAR(20) = CONVERT(VARCHAR(128),CURRENT_TIMESTAMP,112) + '_' + REPLACE(LEFT(CONVERT(VARCHAR(128),CURRENT_TIMESTAMP,114),8),':','_') 
			,@YYYYMMDD_NEWID VARCHAR(128) = CONVERT(VARCHAR(128),CURRENT_TIMESTAMP,112) + '_' + LOWER(CAST(NEWID() AS VARCHAR(64)))

		SELECT @WorkingZipFileDirectory += @YYYYMMDD_NEWID
				,@ExtractedFilesDirectory += @YYYYMMDD_HHMMSS
				,@WorkingZipFile = @WorkingZipFileDirectory + '\' + @ZipFile
		
		EXEC dbo.UtilityCreateDirectory @WorkingZipFileDirectory;
		EXEC dbo.UtilityCopyFile @SourceFile = @ZipPathFile ,@DestinationFile = @WorkingZipFile;

		EXEC dbo.UnZip
			  @ZipFilePath = @WorkingZipFileDirectory
			, @ZipFileName = @ZipFile
			, @DestinationPath = @ExtractedFilesDirectory --Creates Directory
			, @Error = @Error OUTPUT
			, @ErrDescription = @ErrDescription OUTPUT
		  
		IF @Error != 0
			RAISERROR ('Error unzipping the file', 16, 1); --Severity,State

		DECLARE @BookingId VARCHAR(10);
		EXEC dbo.PopulateCalendar		@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId OUT
		--Populate Sequence Because of FK's
		EXEC dbo.PopulateCalendarDate	@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;
		EXEC dbo.PopulateAgency			@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;
		EXEC dbo.PopulateShape			@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;
		EXEC dbo.PopulateRoute			@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;
		EXEC dbo.PopulateTrip			@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;
		EXEC dbo.PopulateStop			@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;
		EXEC dbo.PopulateStopTime		@FolderPath = @ExtractedFilesDirectory ,@DoTransaction = 0 ,@BookingId = @BookingId;

		DECLARE @NewZipFileName VARCHAR(128)
		SELECT @NewZipFileName = @BookingId + '_' + @YYYYMMDD_HHMMSS + '.zip'
				,@DestinationCopyZip1 += @BookingId --Directory Path
				,@DestinationCopyZip2 += @BookingId --Directory Path
	
		EXEC dbo.UtilityCreateDirectory @DestinationCopyZip1;
		EXEC dbo.UtilityCreateDirectory @DestinationCopyZip2;

		SELECT @DestinationCopyZip1 += '\'+@NewZipFileName --Directory Path+FileName
			  ,@DestinationCopyZip2 += '\'+@NewZipFileName --Directory Path+FileName

		EXEC dbo.UtilityCopyFile @SourceFile = @WorkingZipFile ,@DestinationFile = @DestinationCopyZip1;
		EXEC dbo.UtilityCopyFile @SourceFile = @WorkingZipFile ,@DestinationFile = @DestinationCopyZip2;
		EXEC dbo.UtilityDeleteFile @File = @ZipPathFile;

		EXEC dbo.UtilityCopyFile @SourceFile = @WorkingZipFile ,@DestinationFile = @DestinationCopyZip1;
	
		INSERT dbo.ImportInfo
		(
			CreationDateTime	
			,BookingId	
			,FileName	
			,EarliestServiceDate	
			,LatestServiceDate	
			,LastImportStepId	
			,WorkingDirectory	
			,ImportDirectory	
			,AddUserId	
			,AddDateTime	
			,UpdUserId	
			,UpdDateTime
		)
		SELECT
			CURRENT_TIMESTAMP CreationDateTime	
			,@BookingId	BookingId
			,@NewZipFileName FileName	
			,MIN(StartDate) EarliestServiceDate	
			,MAX(EndDate) LatestServiceDate	
			,5 LastImportStepId	-- 5 = FINALIZE
			,@WorkingZipFileDirectory WorkingDirectory	
			,@ExtractedFilesDirectory ImportDirectory	
			,SYSTEM_USER AddUserId	
			,CURRENT_TIMESTAMP AddDateTime	
			,SYSTEM_USER UpdUserId	
			,CURRENT_TIMESTAMP UpdDateTime
		FROM dbo.Calendar
		WHERE BookingId = @BookingId;

		SET @Finalize = 1
	
	END TRY

	BEGIN CATCH

		SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
			  ,@Error = 1
	
		IF @@TRANCOUNT > 0 
		BEGIN
			ROLLBACK TRAN;
			PRINT 'ROLLBACK'
			EXEC(
			'master.dbo.sp_send_cdomail 
				''your.email@your.company.dns'' /*From*/
				,''your.email@your.company.dns'' /*To*/
				,''GTFS Zip File Import Failed'' /*Title*/
				,''GTFS zip file import failed. Review GTFS.dbo.PopulateAllGTFS.'' /*Body*/
				,NULL /*Attachment*/
			');
		END
	
		RAISERROR (@ErrorMessage, 16, 1);
	
	END CATCH

	IF @@TRANCOUNT > 0
		IF @AlwaysRollback = 0 AND @Error = 0 
		BEGIN
			COMMIT TRAN;
			PRINT 'COMMIT'
		END
		ELSE
		BEGIN
			ROLLBACK TRAN;
			PRINT 'ROLLBACK'
		END

	IF @Finalize = 1 --Do After COMMIT b/c proc has its own transaction
		EXEC dbo.GenerateTripIdMap @BookingId;

END --@FileExistsResult = 0
 
END-- proc
GO
/****** Object:  StoredProcedure [dbo].[PopulateCalendar]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateCalendar]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10) OUT
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'Calendar.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSCalendar') IS NOT NULL
		DROP TABLE #TempTransferGTFSCalendar;

	CREATE TABLE #TempTransferGTFSCalendar 
	(
		 ServiceId VARCHAR(100)
		,StartDate CHAR(8) --DATE
		,EndDate CHAR(8) --DATE
		,Monday  VARCHAR(10)
		,Tuesday  VARCHAR(10)
		,Wednesday VARCHAR(10)
		,Thursday  VARCHAR(10)
		,Friday  VARCHAR(10)
		,Saturday  VARCHAR(10)
		,Sunday  VARCHAR(10)
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSCalendar
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSCalendar WHERE ServiceId = ''service_id'';
	');
	
	SET @BookingId = (SELECT TOP 1 LEFT(ServiceId,6) FROM #TempTransferGTFSCalendar ORDER BY StartDate DESC)
	
	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSCalendar'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error Calendar.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.Calendar TARGET
	USING #TempTransferGTFSCalendar SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.ServiceId = SOURCE.ServiceId

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 StartDate = SOURCE.StartDate
			,EndDate = SOURCE.EndDate
			,Monday = SOURCE.Monday
			,Tuesday = SOURCE.Tuesday
			,Wednesday = SOURCE.Wednesday
			,Thursday = SOURCE.Thursday
			,Friday = SOURCE.Friday
			,Saturday = SOURCE.Saturday
			,Sunday = SOURCE.Sunday
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,ServiceId
			,StartDate
			,EndDate
			,Monday
			,Tuesday
			,Wednesday
			,Thursday
			,Friday
			,Saturday 
			,Sunday 
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,ServiceId
			,StartDate
			,EndDate
			,Monday
			,Tuesday
			,Wednesday
			,Thursday
			,Friday
			,Saturday 
			,Sunday
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'Calendar MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSCalendar') IS NOT NULL 
    DROP TABLE #TempTransferGTFSCalendar;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[PopulateCalendarDate]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateCalendarDate]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10) 
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'calendar_dates.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSCalendarDate') IS NOT NULL
		DROP TABLE #TempTransferGTFSCalendarDate;

	CREATE TABLE #TempTransferGTFSCalendarDate 
	(
		 ServiceId VARCHAR(100)
		,Date CHAR(8) --DATE
		,ExceptionType VARCHAR(100)
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSCalendarDate
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSCalendarDate WHERE ServiceId = ''service_id'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSCalendarDate'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error CalendarDate.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.CalendarDate TARGET
	USING #TempTransferGTFSCalendarDate SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.ServiceId = SOURCE.ServiceId
		AND TARGET.Date = SOURCE.Date

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 ExceptionType = SOURCE.ExceptionType
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,ServiceId
			,Date
			,ExceptionType
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,ServiceId
			,CAST(Date AS DATE)
			,ExceptionType
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'CalendarDate MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSCalendarDate') IS NOT NULL 
    DROP TABLE #TempTransferGTFSCalendarDate;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[PopulateRoute]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateRoute]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10)
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'Routes.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSRoute') IS NOT NULL
		DROP TABLE #TempTransferGTFSRoute;

	CREATE TABLE #TempTransferGTFSRoute 
	(
		 LongName VARCHAR(256)
		,RouteId VARCHAR(100)
		,RouteType VARCHAR(100)
		,TextColor VARCHAR(100)
		,AgencyId VARCHAR(100)
		,Color VARCHAR(100)
		,Url VARCHAR(256)
		,Description VARCHAR(512)
		,ShortName VARCHAR(256)
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSRoute
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSRoute WHERE RouteId = ''route_id'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSRoute'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error Routes.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.Route TARGET
	USING #TempTransferGTFSRoute SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.RouteId = SOURCE.RouteId

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 LongName = SOURCE.LongName
			,RouteType = SOURCE.RouteType
			,TextColor = SOURCE.TextColor
			,AgencyId = SOURCE.AgencyId
			,Color = SOURCE.Color
			,Url = SOURCE.Url
			,Description = SOURCE.Description
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,RouteId
			,LongName
			,RouteType
			,TextColor
			,AgencyId
			,Color
			,Url
			,Description
			,ShortName
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,RouteId
			,LongName
			,RouteType
			,TextColor
			,AgencyId
			,Color
			,Url
			,Description
			,ShortName
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'Route MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSRoute') IS NOT NULL 
    DROP TABLE #TempTransferGTFSRoute;
     
END --proc


GO
/****** Object:  StoredProcedure [dbo].[PopulateShape]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateShape]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10)
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'Shapes.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSShape') IS NOT NULL
		DROP TABLE #TempTransferGTFSShape;

	CREATE TABLE #TempTransferGTFSShape 
	(
		 ShapeId VARCHAR(100)
		,Latitude VARCHAR(100)
		,Longitude VARCHAR(100)
		,Sequence VARCHAR(100)		
		,DistanceTraveled VARCHAR(100)	
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSShape
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSShape WHERE ShapeId = ''shape_id'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSShape'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error Shape.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.Shape TARGET
	USING #TempTransferGTFSShape SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.ShapeId = SOURCE.ShapeId
		AND TARGET.Sequence = SOURCE.Sequence

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 Latitude = SOURCE.Latitude
			,Longitude = SOURCE.Longitude
			,DistanceTraveled = SOURCE.DistanceTraveled
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,ShapeId	
			,Sequence	
			,Latitude	
			,Longitude		
			,DistanceTraveled
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,ShapeId	
			,Sequence	
			,Latitude	
			,Longitude		
			,DistanceTraveled
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'Shape MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSShape') IS NOT NULL 
    DROP TABLE #TempTransferGTFSShape;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[PopulateStop]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateStop]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10)
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'Stops.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSStop') IS NOT NULL
		DROP TABLE #TempTransferGTFSStop;
		
	CREATE TABLE #TempTransferGTFSStop 
	(
		 Latitude NUMERIC(18,2)
		,StopCode VARCHAR(100)		
		,Longitude NUMERIC(18,2)
		,StopId VARCHAR(100)
		,Url VARCHAR(256)
		,ParentStation VARCHAR(100)
		,Description VARCHAR(256)	
		,Name VARCHAR(100)
		,LocationType VARCHAR(100)
		,ZoneId VARCHAR(100)	
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSStop
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSStop WHERE StopId = ''stop_id'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSStop'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error Stop.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.Stop TARGET
	USING #TempTransferGTFSStop SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.StopId = SOURCE.StopId

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 Latitude = SOURCE.Latitude
			,StopCode = SOURCE.StopCode
			,Longitude = SOURCE.Longitude
			,Url = SOURCE.Url
			,ParentStation = SOURCE.ParentStation
			,Description = SOURCE.Description
			,Name = SOURCE.Name
			,LocationType = SOURCE.LocationType
			,ZoneId = SOURCE.ZoneId
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,StopId	
			,Latitude
			,StopCode		
			,Longitude	
			,Url	
			,ParentStation
			,Description	
			,Name	
			,LocationType
			,ZoneId	
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,StopId	
			,Latitude
			,StopCode		
			,Longitude	
			,Url	
			,ParentStation
			,Description	
			,Name	
			,LocationType
			,ZoneId	
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'Stop MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSStop') IS NOT NULL 
    DROP TABLE #TempTransferGTFSStop;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[PopulateStopTime]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateStopTime]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10)
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'stop_times.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSStopTime') IS NOT NULL
		DROP TABLE #TempTransferGTFSStopTime;

	CREATE TABLE #TempTransferGTFSStopTime 
	(
		 TripId VARCHAR(100)	
		,ArrivalTime VARCHAR(100)	
		,DepartureTime VARCHAR(100)
		,StopId VARCHAR(100)		
		,Sequence INT	
		,StopHeadsign VARCHAR(256)	
		,PickupType VARCHAR(100)	
		,DropOffType VARCHAR(100)	
		,DistanceTraveled NUMERIC(18,2)	
		,Timepoint BIT
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSStopTime
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSStopTime WHERE StopId = ''stop_id'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSStopTime'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error stop_times.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.StopTime TARGET
	USING #TempTransferGTFSStopTime SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.TripId = SOURCE.TripId
		AND TARGET.StopId = SOURCE.StopId
		AND TARGET.Sequence = SOURCE.Sequence

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 ArrivalTime = SOURCE.ArrivalTime
			,DepartureTime = SOURCE.DepartureTime
			,StopHeadsign = SOURCE.StopHeadsign
			,PickupType = SOURCE.PickupType
			,DropOffType = SOURCE.DropOffType
			,DistanceTraveled = SOURCE.DistanceTraveled
			,Timepoint = SOURCE.Timepoint
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,TripId	
			,StopId	
			,Sequence
			,ArrivalTime	
			,DepartureTime		
			,StopHeadsign
			,PickupType	
			,DropOffType		
			,DistanceTraveled	
			,Timepoint
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,TripId	
			,StopId	
			,Sequence
			,ArrivalTime	
			,DepartureTime		
			,StopHeadsign
			,PickupType	
			,DropOffType		
			,DistanceTraveled	
			,Timepoint
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'StopTime MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSStopTime') IS NOT NULL 
    DROP TABLE #TempTransferGTFSStopTime;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[PopulateTrip]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateTrip]
	 @FolderPath NVARCHAR(255) = NULL
	,@DoTransaction BIT = 1 
	,@BookingId VARCHAR(10)
AS
BEGIN 

SET NOCOUNT ON;
BEGIN TRY
	-- /*Parameters*/  DECLARE @FolderPath VARCHAR(128) = 'C:\(Files)\Dropbox\ACT\GTFS\google_transit - Not GTFS-Plus Files' ,@DoTransaction BIT = 0

   IF @DoTransaction = 1
	  BEGIN TRAN;

	DECLARE @Error INT = 0
		,@AlwaysRollback BIT = 0
		,@ErrorMessage VARCHAR(2047) = 'Success' -- MAX 2047
		,@BulkSource VARCHAR(MAX) 
		,@FileName VARCHAR(64) = 'Trips.txt'
		;
	IF @FolderPath IS NULL AND @@SERVERNAME IN ('SQLServer\Instance')
		SET @BulkSource = '\\server1\share1\Transportation\GTFS\Import\ImportService\WatchPath\' + @FileName;
	ELSE IF @FolderPath IS NOT NULL
		SET @BulkSource = @FolderPath + CASE WHEN RIGHT(@FolderPath,1) != '\' THEN '\' ELSE '' END + @FileName;
	ELSE --Test Location
		SET @BulkSource = '\\server1\share2\GTFS\' + @FileName;

	IF OBJECT_ID('TEMPDB..#TempTransferGTFSTrip') IS NOT NULL
		DROP TABLE #TempTransferGTFSTrip;

	CREATE TABLE #TempTransferGTFSTrip 
	(
		 BlockId VARCHAR(100)	
		,RouteId VARCHAR(100)
		,DirectionId VARCHAR(100)	
		,TripHeadsign VARCHAR(256)	
		,ShapeId VARCHAR(100)	
		,ServiceId VARCHAR(100)	
		,TripId VARCHAR(100)
	);

	EXEC
	('
		BULK
		INSERT #TempTransferGTFSTrip
		FROM ''' + @BulkSource + '''
		WITH
		(
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		);
		DELETE #TempTransferGTFSTrip WHERE RouteId = ''route_id'';
	');

	DECLARE @RowCount INT = 0;
	EXEC master.dbo.sp_executesql 
		 N'SELECT @RowCount=COUNT(*) FROM #TempTransferGTFSTrip'
		,N'@RowCount INT OUTPUT'
		,@RowCount OUTPUT;

	IF @RowCount = 0
	    RAISERROR ('Error Trips.txt uploaded no data in staging table after bulk insert', 16, 1); --Severity,State
		
	MERGE dbo.Trip TARGET
	USING #TempTransferGTFSTrip SOURCE
	ON 1=1
		AND TARGET.BookingId = @BookingId
		AND TARGET.TripId = SOURCE.TripId

	WHEN MATCHED 
	THEN --update if match
		UPDATE SET
			 BlockId = SOURCE.BlockId
			,RouteId = SOURCE.RouteId
			,DirectionId = SOURCE.DirectionId
			,TripHeadsign = SOURCE.TripHeadsign
			,ShapeId = SOURCE.ShapeId
			,ServiceId = SOURCE.ServiceId
			,UpdUserId = ORIGINAL_LOGIN()
			,UpdDateTime = CURRENT_TIMESTAMP

	WHEN NOT MATCHED BY TARGET 
	THEN --insert if source not in target
		INSERT 
		(
			 BookingId
			,TripId	
			,BlockId	
			,RouteId	
			,DirectionId	
			,TripHeadsign	
			,ShapeId	
			,ServiceId	
			,AddUserId
			,AddDateTime
		)
		VALUES 
		(
			 @BookingId
			,TripId	
			,BlockId	
			,RouteId	
			,DirectionId	
			,TripHeadsign	
			,ShapeId	
			,ServiceId	
			,ORIGINAL_LOGIN()
			,CURRENT_TIMESTAMP
		)
	;
	PRINT 'Trip MERGED: ' + CAST(@@ROWCOUNT AS VARCHAR(25));	 
END TRY

BEGIN CATCH

    SELECT @ErrorMessage = 'Error ' + ISNULL(' at line ' + CAST(ERROR_LINE() AS VARCHAR(5)),'') + ISNULL(' * ' + ERROR_MESSAGE(),'')
		  ,@Error = 1
	
    IF @@TRANCOUNT > 0 AND @DoTransaction = 1
	  ROLLBACK TRAN;
	
    RAISERROR (@ErrorMessage, 16, 1);
	
END CATCH

IF @@TRANCOUNT > 0 AND @DoTransaction = 1 
	IF @AlwaysRollback = 0 AND @Error = 0 
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;

IF OBJECT_ID('TEMPDB..#TempTransferGTFSTrip') IS NOT NULL 
    DROP TABLE #TempTransferGTFSTrip;
     
END --proc



GO
/****** Object:  StoredProcedure [dbo].[UnZip]    Script Date: 12/13/2017 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UnZip]
	 @ZipFilePath VARCHAR(1024)
	,@ZipFileName VARCHAR(255) = ''
	,@DestinationPath VARCHAR(1024) = ''
	,@Error INT = 0 OUTPUT
	,@ErrDescription VARCHAR(255) = '' OUTPUT
AS
BEGIN

SET NOCOUNT ON;
BEGIN TRY
	DECLARE 
		 @fso INT
		,@shell INT
		,@items INT
		,@item INT
		,@zipFile INT
		,@destFolder INT
		,@hr INT
		,@src VARCHAR(255)
		,@count INT
		,@ii INT
		,@command VARCHAR(8000)
		,@isDirectory TINYINT
		,@zipFullPathName VARCHAR(1000)
  
	SET @zipFullPathName = @ZipFilePath+'\'+@ZipFileName
   
	SET @ZipFileName=REVERSE(@zipFullPathName)
	SET @ZipFileName=REVERSE(SUBSTRING(@ZipFileName,1,CHARINDEX('\',@ZipFileName)-1))

	SET @ZipFilePath=REPLACE(@zipFullPathName,'\'+@ZipFileName,'') 
         
	EXEC @hr = sp_OACreate 'Shell.Application', @shell out
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @shell, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END  
    
	EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @fso OUT
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @fso, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END
    
	IF ISNULL(@DestinationPath,'') = ''
	BEGIN
		SET @DestinationPath = @DestinationPath+'\'+REPLACE(REPLACE(@ZipFileName,'.zip',''),'.bak','')
		SET @DestinationPath = @DestinationPath+'_'+REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR,GETDATE(),120),'-',''),' ',''),':','')
	END

	SET @command = 'CreateFolder("' + @DestinationPath + '")'    
	EXEC @hr = sp_OAMethod @fso, @command, @destFolder OUT  
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @fso, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END    
    
	SET @command = 'NameSpace("' + @DestinationPath + '")'    
	EXEC @hr = sp_OAMethod @shell, @command, @destFolder OUT  
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @shell, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END    
    
	SET @command = 'NameSpace("' + @zipFullPathName + '")'    
	EXEC @hr = sp_OAMethod @shell , @command , @zipFile OUT  
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @shell, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END    
  
	SET @command = 'Items'    
	EXEC @hr = sp_OAMethod @zipFile , @command, @items OUT  
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @zipFile, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END    
   
	SET @command = 'Count'    
	EXEC @hr = sp_OAMethod @items , @command, @count OUT  
	IF @hr <> 0
	BEGIN
		EXEC sp_OAGetErrorInfo @items, @src OUT, @ErrDescription OUT
		SET @Error = @hr
	END
      
	SET @ii = 0
	WHILE (@ii<@count)
	BEGIN   
		SET @command = 'Item('+CONVERT(VARCHAR(20),@ii)+')'
		EXEC @hr = sp_OAMethod @items , @command, @item OUT  
		IF @hr <> 0
		BEGIN
			EXEC sp_OAGetErrorInfo @items, @src OUT, @ErrDescription OUT
			SET @Error = @hr
		END 
     
		SET @command = 'CopyHere'
		EXEC @hr = sp_OAMethod @destFolder, @command, null, @item
		IF @hr <> 0
		BEGIN
			EXEC sp_OAGetErrorInfo @destFolder , @src OUT, @ErrDescription OUT
			SET @Error = @hr
		END   
    
		SET @ii += 1
	END --WHILE
           
END TRY
BEGIN CATCH
	DECLARE @ErrorSeverity INT
	DECLARE @ErrorState INT

	SET @Error = ISNULL(ERROR_NUMBER(),ISNULL(@Error,0))
	SET @ErrDescription = ISNULL(ERROR_MESSAGE(),ISNULL(@ErrDescription,'')) 
	SET @ErrorSeverity = ERROR_SEVERITY()
	SET @ErrorState = ERROR_STATE()
  
	IF @fso <> 0 EXEC sp_OADestroy @fso
	IF @shell <> 0 EXEC sp_OADestroy @shell    
	IF @zipFile <> 0 EXEC sp_OADestroy @zipFile 
  
	RAISERROR (@ErrDescription,@ErrorSeverity,@ErrorState,@Error)
END CATCH
  
SET @Error = ISNULL(@Error,0)
SET @ErrDescription = ISNULL(@ErrDescription,'')
 
END --proc

GO

EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Clean all related data from GTFS tables for a given BookingId value.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'DeleteSignupData'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'Not found in TFS application' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'DeleteSignupData'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'Nothing calls it.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'DeleteSignupData'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'DeleteSignupData'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'Not Found in TFS Report Solution' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'DeleteSignupData'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates cross reference table GTFS.TripMap to link HASTUS booking and GTFS trips by delete reinsert per booking parameter.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'GenerateTripIdMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'Not found in TFS application' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'GenerateTripIdMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'GenerateTripIdMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'GenerateTripIdMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'Not Found in TFS Report Solution' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'GenerateTripIdMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.Agency data from the Planning Department Agency.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAgency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAgency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAgency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAgency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAgency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS tables from the Planning Department text files' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAllGTFS'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAllGTFS'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAllGTFS'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAllGTFS'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateAllGTFS'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.Calendar data from the Planning Department Calendar.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.CalendarDate data from the Planning Department calendar_dates.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateCalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.Route data from the Planning Department Routes.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateRoute'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateRoute'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateRoute'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateRoute'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateRoute'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.Shape data from the Planning Department Shapes.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateShape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateShape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateShape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateShape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateShape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.Stop data from the Planning Department Stops.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.StopTime data from the Planning Department stop_times.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateStopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Populates GTFS dbo.Trip data from the Planning Department Trips.txt file' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateTrip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateTrip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateTrip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateTrip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'PopulateTrip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Unzips a file into the destination directory' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'UnZip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_InternalApp', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'UnZip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'UnZip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackage', @value=N'Not Directly Called from SSIS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'UnZip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSRS', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'UnZip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Transit agencies that provide data to the General Transit Feed Specification (GTFS) feed.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, Agency.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Agency'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Dates for service IDs using a weekly schedule. It specifies when service starts and ends, as well as days of the week where service is available.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, calendar.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Calendar'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Exceptions for service IDs defined in the calendar.txt file. If calendar_dates.txt includes ALL dates of service, this file may be specified instead of calendar.txt.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, calendar_dates.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CalendarDate'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Execution log records for the GTFS data import.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'MAN_Load' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Execution log records for step by step GTFS data import. ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'MAN_Load' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportLog'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Execution step category of the GTFS data import process.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'MAN_Load' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImportStep'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Transit routes. A route is a group of trips that are displayed to riders as a single service' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, routes.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Rules for drawing lines on a map to represent a transit organization''s routes.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, shapes.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shape'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Locations where vehicles pick up or drop off passengers.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, stops.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stop'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Time that a vehicle arrives at and departs from individual stop for each trip' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, stop_times.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StopTime'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Trips per route. A trip is a sequence of two or more stops that occurs at specific time.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus, trips.txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Trip'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Cross reference table between HASTUS booking and GTFS trips.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'Windows Service "Actransit - GTFS File Watcher" audit on \\ftp-public\google$ and copy new .zip file to 2. .zip file is moved to \\server1\share1\Transportation\Hastus\Import folder. This zip file is then extracted to a temporary directory where the proper file is imported.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TripMap'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Bookings that are based on Sign-up periods.' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'your.SQL.server proc imports file \\server1\share1\Transportation\Hastus\Import\Bookings.xml' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'HASTUS.PopulateBooking' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
USE [master]
GO
ALTER DATABASE [GTFS] SET  READ_WRITE 
GO

