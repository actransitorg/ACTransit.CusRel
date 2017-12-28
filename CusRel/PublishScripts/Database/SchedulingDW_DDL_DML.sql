USE [SchedulingDW]
GO
/****** Object:  Table [HASTUS].[Booking]    Script Date: 11/16/2017 4:06:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE SCHEMA [HASTUS]

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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HASTUS].[Route]    Script Date: 11/16/2017 4:06:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HASTUS].[Route](
	[BookingId] [varchar](10) NOT NULL,
	[RouteAlpha] [varchar](5) NOT NULL,
	[RouteDescription] [varchar](50) NULL,
	[RouteTypeId] [int] NOT NULL,
	[Color] [varchar](12) NULL,
	[SortOrder] [int] NOT NULL,
	[AddUserId] [varchar](50) NOT NULL,
	[AddDateTime] [datetime2](7) NOT NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
	[SysRecNo] [bigint] NOT NULL,
 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC,
	[RouteAlpha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HASTUS].[RouteList]    Script Date: 11/16/2017 4:06:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HASTUS].[RouteList](
	[RouteAlpha] [varchar](5) NOT NULL,
 CONSTRAINT [PK_RouteList] PRIMARY KEY CLUSTERED 
(
	[RouteAlpha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HASTUS].[RouteType]    Script Date: 11/16/2017 4:06:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HASTUS].[RouteType](
	[RouteTypeId] [int] IDENTITY(1,1) NOT NULL,
	[RouteTypeName] [varchar](40) NOT NULL,
	[AddUserId] [varchar](50) NOT NULL,
	[AddDateTime] [datetime2](7) NOT NULL,
	[UpdUserId] [varchar](50) NULL,
	[UpdDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_RouteType] PRIMARY KEY CLUSTERED 
(
	[RouteTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [HASTUS].[Booking] ([BookingId], [StartDate], [EndDate], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [SysRecNo]) 
VALUES (N'1708FA', CAST(N'2017-08-20' AS Date), CAST(N'2017-12-23' AS Date), N'user2', CAST(N'2017-08-02T10:20:55.9130000' AS DateTime2), N'user2', CAST(N'2017-08-02T10:20:55.9130000' AS DateTime2), 109)
GO

INSERT [HASTUS].[Route] ([BookingId], [RouteAlpha], [RouteDescription], [RouteTypeId], [Color], [SortOrder], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [SysRecNo]) 
VALUES (N'1708FA', N'1', N'Route 1', 10, N'Lavender', 1, N'user1', CAST(N'2015-07-01T11:32:15.5434801' AS DateTime2), N'user1', CAST(N'2015-07-01T11:32:15.5434801' AS DateTime2), -1)
GO

INSERT [HASTUS].[RouteList] ([RouteAlpha]) VALUES (N'1')
GO

SET IDENTITY_INSERT [HASTUS].[RouteType] ON 
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (1, N'Contract Shuttle', N'user1', CAST(N'2015-05-15T09:09:45.5919581' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5919581' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (2, N'Feeder', N'user1', CAST(N'2015-05-15T09:09:45.5929347' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5929347' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (3, N'Major Corridor', N'user1', CAST(N'2015-05-15T09:09:45.5939113' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5939113' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (4, N'Owl', N'user1', CAST(N'2015-05-15T09:09:45.5948879' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5948879' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (5, N'Rapid', N'user1', CAST(N'2015-05-15T09:09:45.5958645' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5958645' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (6, N'School', N'user1', CAST(N'2015-05-15T09:09:45.5968411' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5968411' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (7, N'Shuttle', N'user1', CAST(N'2015-05-15T09:09:45.5968411' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5968411' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (8, N'Suburban Crosstown', N'user1', CAST(N'2015-05-15T09:09:45.5987943' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.5987943' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (9, N'Transbay', N'user1', CAST(N'2015-05-15T09:09:45.6007475' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.6007475' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (10, N'Trunk', N'user1', CAST(N'2015-05-15T09:09:45.6007475' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.6007475' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (11, N'Urban Crosstown', N'user1', CAST(N'2015-05-15T09:09:45.6017241' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.6017241' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (12, N'Very Low Density', N'user1', CAST(N'2015-05-15T09:09:45.6027007' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.6027007' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (13, N'Standby', N'user1', CAST(N'2015-05-15T09:09:45.6036773' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.6036773' AS DateTime2))
GO
INSERT [HASTUS].[RouteType] ([RouteTypeId], [RouteTypeName], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime]) VALUES (14, N'Others', N'user1', CAST(N'2015-05-15T09:09:45.6046539' AS DateTime2), N'user1', CAST(N'2015-05-15T09:09:45.6046539' AS DateTime2))
GO
SET IDENTITY_INSERT [HASTUS].[RouteType] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_BookingBookingID]    Script Date: 11/16/2017 4:06:25 PM ******/
ALTER TABLE [HASTUS].[Booking] ADD  CONSTRAINT [UK_BookingBookingID] UNIQUE NONCLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [HASTUS].[Booking] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [HASTUS].[Route] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [HASTUS].[Route] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [HASTUS].[Route] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [HASTUS].[Route] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [HASTUS].[RouteType] ADD  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [HASTUS].[RouteType] ADD  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [HASTUS].[RouteType] ADD  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [HASTUS].[RouteType] ADD  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
ALTER TABLE [HASTUS].[Route]  WITH CHECK ADD  CONSTRAINT [FK_Route_RouteList] FOREIGN KEY([RouteAlpha])
REFERENCES [HASTUS].[RouteList] ([RouteAlpha])
GO
ALTER TABLE [HASTUS].[Route] CHECK CONSTRAINT [FK_Route_RouteList]
GO
ALTER TABLE [HASTUS].[Route]  WITH CHECK ADD  CONSTRAINT [FK_Route_RouteType] FOREIGN KEY([RouteTypeId])
REFERENCES [HASTUS].[RouteType] ([RouteTypeId])
GO
ALTER TABLE [HASTUS].[Route] CHECK CONSTRAINT [FK_Route_RouteType]
GO
ALTER TABLE [HASTUS].[Booking]  WITH CHECK ADD  CONSTRAINT [ck_booking_StartDateLessThanEndDate] CHECK  (([StartDate]<[EndDate]))
GO
ALTER TABLE [HASTUS].[Booking] CHECK CONSTRAINT [ck_booking_StartDateLessThanEndDate]
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Bookings that are based on Sign-up periods.' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'your.SQL.server proc imports file \\appfs1\EnterpriseDatabase\Transportation\Hastus\Import\Bookings.xml' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Booking'
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
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Route information records including description and color, group by different BookingIds.' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'your.SQL.server proc imports file \\appfs1\EnterpriseDatabase\Transportation\Hastus\Import\Routes.xml' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'HASTUS.PopulateRoutes' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'Route'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Distinct routes.' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'your.SQL.server proc imports file \\appfs1\EnterpriseDatabase\Transportation\Hastus\Import\Routes.xml' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_DEL_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'It can be a view out of HASTUS.Route table therefore it is a delete candidate.' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'HASTUS.PopulateRoutes' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteList'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Desc', @value=N'Types of Route.' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_DescOfProcessFillingTheTable', @value=N'your.SQL.server proc imports file \\appfs1\EnterpriseDatabase\Transportation\Hastus\Import\Routes.xml' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_ETLUpdateStrategy', @value=N'UPD_INS_FLTR' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_Note', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SPName', @value=N'HASTUS.PopulateRoutes' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SQLJobName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_SSISPackageName', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TableSource', @value=N'Hastus' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO
EXEC sys.sp_addextendedproperty @name=N'ACT_TFSSolutionPackage', @value=N'' , @level0type=N'SCHEMA',@level0name=N'HASTUS', @level1type=N'TABLE',@level1name=N'RouteType'
GO

