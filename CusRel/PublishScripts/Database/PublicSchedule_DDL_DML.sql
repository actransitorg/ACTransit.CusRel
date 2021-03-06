USE [PublicSchedule]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_day_codes](
	[day_code] [varchar](5) NOT NULL,
	[adjectives] [varchar](50) NULL,
	[long_names] [varchar](50) NULL,
	[longer_names] [varchar](100) NULL,
	[sort_order] [int] NULL,
 CONSTRAINT [PK_sch_day_codes] PRIMARY KEY CLUSTERED 
(
	[day_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_direction_codes]    Script Date: 12/13/2017 3:37:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_direction_codes](
	[direction_code] [varchar](5) NOT NULL,
	[direction_names] [varchar](50) NULL,
	[direction_replace] [varchar](50) NULL,
 CONSTRAINT [PK_sch_direction_codes] PRIMARY KEY CLUSTERED 
(
	[direction_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_schedule]    Script Date: 12/13/2017 3:37:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_schedule](
	[schedule_id] [int] IDENTITY(1,1) NOT NULL,
	[schedule_code] [varchar](50) NULL,
	[schedule_type] [int] NULL,
	[line_group_id] [int] NULL,
	[type] [varchar](5) NULL,
	[notes] [text] NULL,
	[schedule_id_equivalent] [int] NULL,
	[direction_code] [varchar](5) NULL,
	[day_code] [varchar](5) NULL,
 CONSTRAINT [PK_sch_schedule] PRIMARY KEY CLUSTERED 
(
	[schedule_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_schedule_lines]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_schedule_lines](
	[recid] [int] IDENTITY(1,1) NOT NULL,
	[line_id] [int] NULL,
	[schedule_id] [int] NULL,
 CONSTRAINT [PK_sch_schedule_lines] PRIMARY KEY CLUSTERED 
(
	[recid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[sch_schedule_line_name_info]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sch_schedule_line_name_info]
AS
SELECT     dbo.sch_direction_codes.direction_replace, dbo.sch_direction_codes.direction_names, dbo.sch_direction_codes.direction_code, 
                      dbo.sch_day_codes.day_code, dbo.sch_day_codes.adjectives, s.schedule_id, sl.line_id, dbo.sch_day_codes.sort_order
FROM         dbo.sch_schedule s INNER JOIN
                      dbo.sch_day_codes ON s.day_code = dbo.sch_day_codes.day_code INNER JOIN
                      dbo.sch_direction_codes ON s.direction_code = dbo.sch_direction_codes.direction_code INNER JOIN
                      dbo.sch_schedule_lines sl ON s.schedule_id = sl.schedule_id
GO
/****** Object:  Table [dbo].[sch_lines]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_lines](
	[line_id] [int] IDENTITY(1,1) NOT NULL,
	[category_id] [int] NULL,
	[version_id] [int] NULL,
	[line_name] [varchar](50) NULL,
	[description] [text] NULL,
	[directions_file] [varchar](50) NULL,
	[stop_lists_file] [varchar](50) NULL,
	[map_file] [varchar](50) NULL,
	[last_update] [varchar](50) NULL,
	[cmyk_value] [varchar](50) NULL,
	[rgb_value] [varchar](50) NULL,
	[is_draft] [int] NULL,
	[is_processing] [int] NULL,
	[line_pdb_updated] [int] NULL,
	[line_update_by] [varchar](20) NULL,
	[line_update_timestamp] [datetime] NULL,
 CONSTRAINT [PK_sch_lines] PRIMARY KEY CLUSTERED 
(
	[line_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[sch_schedule_line_search]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sch_schedule_line_search]
AS
SELECT     s.schedule_id, l.line_id, l.line_name, s.notes, dir.direction_replace, dir.direction_names, dir.direction_code, [day].day_code, [day].adjectives, 
                      [day].sort_order
FROM         dbo.sch_schedule s LEFT OUTER JOIN
                      dbo.sch_schedule_lines sl ON sl.schedule_id = s.schedule_id LEFT OUTER JOIN
                      dbo.sch_lines l ON l.line_id = sl.line_id LEFT OUTER JOIN
                      dbo.sch_direction_codes dir ON s.direction_code = dir.direction_code LEFT OUTER JOIN
                      dbo.sch_day_codes [day] ON s.day_code = [day].day_code
GO
/****** Object:  Table [dbo].[sch_footnote]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_footnote](
	[footnote_letter] [varchar](50) NOT NULL,
	[footnote_text] [text] NULL,
 CONSTRAINT [PK_sch_footnote] PRIMARY KEY CLUSTERED 
(
	[footnote_letter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_special_day_codes]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_special_day_codes](
	[abbreviation] [varchar](2) NOT NULL,
	[description] [text] NULL,
 CONSTRAINT [PK_sch_special_day_codes] PRIMARY KEY CLUSTERED 
(
	[abbreviation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_timepoints]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_timepoints](
	[timepoint_id] [int] IDENTITY(1,1) NOT NULL,
	[schedule_id] [int] NULL,
	[timepoint_code] [varchar](50) NULL,
	[timepoint_name] [varchar](150) NULL,
	[city] [varchar](150) NULL,
	[city_redundancy] [int] NULL,
	[neighborhood] [varchar](50) NULL,
	[latitude] [decimal](18, 6) NULL,
	[longitude] [decimal](18, 6) NULL,
	[timepoint_notes] [text] NULL,
	[sort_order] [int] NULL,
 CONSTRAINT [PK_sch_stops] PRIMARY KEY CLUSTERED 
(
	[timepoint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_trips]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_trips](
	[trip_id] [int] IDENTITY(1,1) NOT NULL,
	[abbreviation] [varchar](2) NULL,
	[line_id] [int] NULL,
	[timepoint_id] [int] NULL,
	[footnote_letter] [varchar](50) NULL,
	[stop_time] [varchar](10) NULL,
	[sort_order] [int] NULL,
 CONSTRAINT [PK_sch_trips] PRIMARY KEY CLUSTERED 
(
	[trip_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[sch_schedule_search_time_stops]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sch_schedule_search_time_stops]
AS
SELECT     tm.schedule_id, tr.sort_order, tr.line_id, tr.stop_time, tr.trip_id, sdc.abbreviation AS special_day_code_abbr, 
                      sdc.description AS special_day_code
FROM         dbo.sch_timepoints tm LEFT OUTER JOIN
                      dbo.sch_trips tr ON tm.timepoint_id = tr.timepoint_id LEFT OUTER JOIN
                      dbo.sch_special_day_codes sdc ON tr.abbreviation = sdc.abbreviation LEFT OUTER JOIN
                      dbo.sch_footnote f ON tr.footnote_letter = f.footnote_letter
WHERE     (tr.trip_id <> '')
GO
/****** Object:  Table [dbo].[map_stops]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[map_stops](
	[route_id] [varchar](50) NULL,
	[trip_id] [varchar](50) NULL,
	[service_day] [varchar](50) NULL,
	[direction_of_travel] [varchar](50) NULL,
	[stop_id] [varchar](50) NULL,
	[stop_description] [varchar](50) NULL,
	[longitude] [varchar](50) NULL,
	[latitude] [varchar](50) NULL,
	[stop_order] [int] NULL,
	[block_num] [varchar](50) NULL,
	[stop_time] [varchar](50) NULL,
	[time_point_name] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_line_group]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_line_group](
	[line_group_id] [int] IDENTITY(1,1) NOT NULL,
	[notes] [text] NULL,
	[type] [varchar](50) NULL,
	[line_group_equivalent] [varchar](50) NULL,
 CONSTRAINT [PK_sch_line_group] PRIMARY KEY CLUSTERED 
(
	[line_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_maps]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_maps](
	[map_id] [int] IDENTITY(1,1) NOT NULL,
	[version_id] [int] NULL,
	[map_code] [varchar](50) NULL,
	[map_category_id] [int] NULL,
	[map_city] [varchar](100) NULL,
	[map_type] [int] NULL,
	[map_center_lat_lng_zoom] [varchar](100) NULL,
 CONSTRAINT [PK_sch_maps] PRIMARY KEY CLUSTERED 
(
	[map_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_maps_limit]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_maps_limit](
	[map_limit_id] [int] IDENTITY(1,1) NOT NULL,
	[map_id] [int] NULL,
	[limit_zoom] [int] NULL,
	[limit_x_min] [int] NULL,
	[limit_x_max] [int] NULL,
	[limit_y_min] [int] NULL,
	[limit_y_max] [int] NULL,
 CONSTRAINT [PK_sch_maps_limit] PRIMARY KEY CLUSTERED 
(
	[map_limit_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sch_schedule_versions]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sch_schedule_versions](
	[version_id] [int] IDENTITY(1,1) NOT NULL,
	[version_name] [varchar](255) NULL,
	[version_preview_date] [datetime] NULL,
	[version_start_date] [datetime] NULL,
	[version_update_by] [varchar](20) NULL,
	[version_update_in_progress] [int] NULL,
	[version_update_timestamp] [datetime] NULL,
	[is_processing] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [route_id_ix]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [route_id_ix] ON [dbo].[map_stops]
(
	[route_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [sch_maps_map_code_idx]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [sch_maps_map_code_idx] ON [dbo].[sch_maps]
(
	[map_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [sch_map_limits_map_id_idx]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [sch_map_limits_map_id_idx] ON [dbo].[sch_maps_limit]
(
	[map_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [sch_schedule_lines_line_id_idx]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [sch_schedule_lines_line_id_idx] ON [dbo].[sch_schedule_lines]
(
	[line_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [sch_schedule_lines_schedule_id_idx]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [sch_schedule_lines_schedule_id_idx] ON [dbo].[sch_schedule_lines]
(
	[schedule_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [sch_timepoints_schedule_id_idx]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [sch_timepoints_schedule_id_idx] ON [dbo].[sch_timepoints]
(
	[schedule_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [tr_timepoint_id]    Script Date: 12/13/2017 3:37:17 PM ******/
CREATE NONCLUSTERED INDEX [tr_timepoint_id] ON [dbo].[sch_trips]
(
	[timepoint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[sch_day_codes] ADD  CONSTRAINT [DF_sch_day_codes_day_code]  DEFAULT (0) FOR [day_code]
GO
ALTER TABLE [dbo].[sch_lines] ADD  CONSTRAINT [DF_sch_lines_is_draft]  DEFAULT (0) FOR [is_draft]
GO
ALTER TABLE [dbo].[sch_lines] ADD  CONSTRAINT [DF_sch_lines_is_processing]  DEFAULT (0) FOR [is_processing]
GO
ALTER TABLE [dbo].[sch_lines] ADD  CONSTRAINT [DF_sch_lines_line_pdb_updated]  DEFAULT (0) FOR [line_pdb_updated]
GO
ALTER TABLE [dbo].[sch_lines] ADD  CONSTRAINT [DF_sch_lines_line_update_timestamp]  DEFAULT (getdate()) FOR [line_update_timestamp]
GO
ALTER TABLE [dbo].[sch_schedule] ADD  CONSTRAINT [DF_sch_schedule_schedule_type]  DEFAULT (1) FOR [schedule_type]
GO
ALTER TABLE [dbo].[sch_schedule_versions] ADD  CONSTRAINT [DF_sch_schedule_versions_version_update_in_progress]  DEFAULT ((0)) FOR [version_update_in_progress]
GO
ALTER TABLE [dbo].[sch_schedule_versions] ADD  CONSTRAINT [DF_sch_schedule_versions_version_update_timestamp]  DEFAULT (getdate()) FOR [version_update_timestamp]
GO
ALTER TABLE [dbo].[sch_schedule_versions] ADD  CONSTRAINT [DF_sch_schedule_versions_is_processing]  DEFAULT ((0)) FOR [is_processing]
GO
/****** Object:  StoredProcedure [dbo].[act_InsertSearch]    Script Date: 12/13/2017 3:37:17 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
USE [master]
GO
ALTER DATABASE [PublicSchedule] SET  READ_WRITE 
GO
