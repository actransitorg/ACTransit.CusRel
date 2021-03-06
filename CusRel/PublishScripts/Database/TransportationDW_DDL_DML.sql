USE [TransportationDW]
GO
/****** Object:  Table [CADAVL].[Division]    Script Date: 12/13/2017 3:45:51 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE SCHEMA [CADAVL]
GO

CREATE TABLE [CADAVL].[Division](
	[division_id] [tinyint] NOT NULL,
	[division_description] [varchar](40) NOT NULL,
	[division_sname] [varchar](8) NOT NULL,
 CONSTRAINT [pk_division] PRIMARY KEY CLUSTERED 
(
	[division_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [CADAVL].[HolidayList]    Script Date: 12/13/2017 3:45:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CADAVL].[HolidayList](
	[HolidayListId] [bigint] IDENTITY(1,1) NOT NULL,
	[AddUserId] [varchar](50) NOT NULL,
	[AddDateTime] [datetime2](7) NOT NULL,
	[UpdUserId] [varchar](50) NOT NULL,
	[UpdDateTime] [datetime2](7) NOT NULL,
	[HolidayDate] [date] NOT NULL,
 CONSTRAINT [HolidayList_PK] PRIMARY KEY CLUSTERED 
(
	[HolidayListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (0, N' ', N'Unknown')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (1, N'East Oakland Garage', N'D4')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (2, N'Emeryville Garage', N'D2')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (3, N'Richmond Garage', N'D3')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (4, N'Hayward Garage', N'D6')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (5, N'Paratransit', N'D8')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (6, N'S.F Terminal', N'ET')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (7, N'Training Dept', N'TRN')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (8, N'Central Division', N'CMF')
GO
INSERT [CADAVL].[Division] ([division_id], [division_description], [division_sname]) VALUES (9, N'Test Base', N'D10')
GO
SET IDENTITY_INSERT [CADAVL].[HolidayList] ON 
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (1, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-01-02' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (2, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-01-17' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (3, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-02-21' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (4, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-05-30' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (5, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-07-04' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (6, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-09-05' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (7, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-11-24' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (8, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2011-12-26' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (9, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-01-02' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (10, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-01-16' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (11, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-02-20' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (12, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-05-28' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (13, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-07-04' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (14, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-09-03' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (15, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-11-22' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (16, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2012-12-25' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (17, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-01-01' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (18, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-01-21' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (19, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-02-18' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (20, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-05-27' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (21, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-07-04' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (22, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-09-02' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (23, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-11-28' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (24, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2013-12-25' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (25, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-01-01' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (26, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-01-20' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (27, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-02-17' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (28, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-05-26' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (29, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-07-04' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (30, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-09-01' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (31, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-11-27' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (32, N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), N'SYSTEM', CAST(N'2013-08-14T17:35:36.6821666' AS DateTime2), CAST(N'2014-12-25' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (33, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-01-01' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (34, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-01-19' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (35, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-02-16' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (36, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-05-25' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (37, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-07-04' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (38, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-09-07' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (39, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-11-26' AS Date))
GO
INSERT [CADAVL].[HolidayList] ([HolidayListId], [AddUserId], [AddDateTime], [UpdUserId], [UpdDateTime], [HolidayDate]) VALUES (40, N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), N'ACTRANSIT\btwilliams', CAST(N'2015-05-04T06:50:17.0171765' AS DateTime2), CAST(N'2015-12-25' AS Date))
GO
SET IDENTITY_INSERT [CADAVL].[HolidayList] OFF
GO
/****** Object:  Index [HolidayList_IX0]    Script Date: 12/13/2017 3:45:51 PM ******/
ALTER TABLE [CADAVL].[HolidayList] ADD  CONSTRAINT [HolidayList_IX0] UNIQUE NONCLUSTERED 
(
	[HolidayDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [CADAVL].[Division] ADD  DEFAULT ((0)) FOR [division_id]
GO
ALTER TABLE [CADAVL].[Division] ADD  DEFAULT (' ') FOR [division_description]
GO
ALTER TABLE [CADAVL].[Division] ADD  DEFAULT (' ') FOR [division_sname]
GO
ALTER TABLE [CADAVL].[HolidayList] ADD  CONSTRAINT [HolidayList_DF_AddUserId]  DEFAULT (suser_name()) FOR [AddUserId]
GO
ALTER TABLE [CADAVL].[HolidayList] ADD  CONSTRAINT [HolidayList_DF_AddDateTime]  DEFAULT (sysdatetime()) FOR [AddDateTime]
GO
ALTER TABLE [CADAVL].[HolidayList] ADD  CONSTRAINT [HolidayList_DF_UpdUserId]  DEFAULT (suser_name()) FOR [UpdUserId]
GO
ALTER TABLE [CADAVL].[HolidayList] ADD  CONSTRAINT [HolidayList_DF_UpdDateTime]  DEFAULT (sysdatetime()) FOR [UpdDateTime]
GO
