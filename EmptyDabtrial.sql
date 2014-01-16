USE [master]
GO
/****** Object:  Database [dabtrial_com_participantdata]    Script Date: 06/12/2013 15:17:02 ******/
CREATE DATABASE [dabtrial_com_participantdata] ON  PRIMARY 
( NAME = N'dabtrial_com_participantdata', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\dabtrial_com_participantdata.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'dabtrial_com_participantdata_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\dabtrial_com_participantdata_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [dabtrial_com_participantdata] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dabtrial_com_participantdata].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [dabtrial_com_participantdata] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET ANSI_NULLS OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET ANSI_PADDING OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET ARITHABORT OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [dabtrial_com_participantdata] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [dabtrial_com_participantdata] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [dabtrial_com_participantdata] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET  DISABLE_BROKER
GO
ALTER DATABASE [dabtrial_com_participantdata] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [dabtrial_com_participantdata] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [dabtrial_com_participantdata] SET  READ_WRITE
GO
ALTER DATABASE [dabtrial_com_participantdata] SET RECOVERY SIMPLE
GO
ALTER DATABASE [dabtrial_com_participantdata] SET  MULTI_USER
GO
ALTER DATABASE [dabtrial_com_participantdata] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [dabtrial_com_participantdata] SET DB_CHAINING OFF
GO
USE [dabtrial_com_participantdata]
GO
/****** Object:  Table [dbo].[NoConsentAttempts]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoConsentAttempts](
	[Id] [int] IDENTITY(8,1) NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
	[Abbreviation] [nvarchar](50) NULL,
	[RequiresDetail] [bit] NOT NULL,
	[IsFullyAware] [bit] NULL,
 CONSTRAINT [PK_NoConsentAttempt] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__NoConsentAttempt__000000000000031F] ON [dbo].[NoConsentAttempts] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__NoConsentAttempt__0000000000000329] ON [dbo].[NoConsentAttempts] 
(
	[Description] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__NoConsentAttempt__000000000000035C] ON [dbo].[NoConsentAttempts] 
(
	[Abbreviation] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[NoConsentAttempts] ON
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (1, N'Staff were not aware trial was in progress', N'Unaware', 0, 0)
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (2, N'Clinician is not in equipoise about trial', N'No Equipoise', 0, 1)
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (3, N'Forgot to consider this patient in the trial', N'Forgot', 0, 1)
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (4, N'Misunderstood inclusion/exclusion criteria', N'Misunderstood', 0, 0)
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (5, N'Other reason', N'Other', 1, NULL)
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (6, N'Clinical priorities meant staff did not have time to enroll within 4 hours', N'Too Busy', 0, 1)
INSERT [dbo].[NoConsentAttempts] ([Id], [Description], [Abbreviation], [RequiresDetail], [IsFullyAware]) VALUES (7, N'Clinician felt insufficiently familiar with trial to obtain valid informed consent', N'Partially Aware', 0, 0)
SET IDENTITY_INSERT [dbo].[NoConsentAttempts] OFF
/****** Object:  Table [dbo].[LocalRecordProviders]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocalRecordProviders](
	[Id] [int] IDENTITY(4,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[HospitalNoRegEx] [nvarchar](50) NULL,
	[NotationDescription] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_dbo.LocalRecordProviders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Name] ON [dbo].[LocalRecordProviders] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[LocalRecordProviders] ON
INSERT [dbo].[LocalRecordProviders] ([Id], [Name], [HospitalNoRegEx], [NotationDescription]) VALUES (1, N'Unique Record (UR) number', N'\d{7,8}', N'An 8 digit number')
INSERT [dbo].[LocalRecordProviders] ([Id], [Name], [HospitalNoRegEx], [NotationDescription]) VALUES (2, N'NZ National Health Index (NHI)', N'[A-Za-z]{3}[0-9]{3,4}', N'3 Letters followed by 3 to 4 numbers')
INSERT [dbo].[LocalRecordProviders] ([Id], [Name], [HospitalNoRegEx], [NotationDescription]) VALUES (3, N'Topaz Index', N'[A-Za-z]\d{7}', N'1 Letter followed by a 7 digit number')
SET IDENTITY_INSERT [dbo].[LocalRecordProviders] OFF
/****** Object:  Table [dbo].[AuditLogEntries]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLogEntries](
	[AuditLogId] [int] IDENTITY(161,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[EventDateUTC] [datetime] NOT NULL,
	[EventType] [nvarchar](1) NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[RecordId] [nvarchar](100) NOT NULL,
	[ColumnName] [nvarchar](100) NOT NULL,
	[OriginalValue] [nvarchar](4000) NULL,
	[NewValue] [nvarchar](4000) NULL,
 CONSTRAINT [PK_dbo.AuditLogEntries] PRIMARY KEY CLUSTERED 
(
	[AuditLogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AuditLogEntries] ON
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (50, N'Brentm', CAST(0x0000A1C00030F1A8 AS DateTime), N'M', N'User', N'6', N'FirstName', NULL, N'siva')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (51, N'Brentm', CAST(0x0000A1C00030F1A8 AS DateTime), N'M', N'User', N'6', N'LastName', NULL, N'namachivayam')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (52, N'Brentm', CAST(0x0000A1C00030F1A8 AS DateTime), N'M', N'User', N'6', N'StudyCentreId', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (53, N'Brentm', CAST(0x0000A1C00030F1A8 AS DateTime), N'M', N'User', N'6', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (54, N'brentm', CAST(0x0000A1C0003100BF AS DateTime), N'M', N'User', N'6', N'FirstName', N'siva', N'Siva')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (55, N'brentm', CAST(0x0000A1C0003100BF AS DateTime), N'M', N'User', N'6', N'LastName', N'namachivayam', N'Namachivayam')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (56, N'Brentm', CAST(0x0000A1C00031475A AS DateTime), N'M', N'User', N'7', N'FirstName', NULL, N'Warwick')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (57, N'Brentm', CAST(0x0000A1C00031475A AS DateTime), N'M', N'User', N'7', N'LastName', NULL, N'Butt')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (58, N'Brentm', CAST(0x0000A1C00031475A AS DateTime), N'M', N'User', N'7', N'StudyCentreId', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (59, N'Brentm', CAST(0x0000A1C00031475A AS DateTime), N'M', N'User', N'7', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (60, N'Brentm', CAST(0x0000A1C000319FCF AS DateTime), N'M', N'User', N'8', N'FirstName', NULL, N'Mike')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (61, N'Brentm', CAST(0x0000A1C000319FCF AS DateTime), N'M', N'User', N'8', N'LastName', NULL, N'South')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (62, N'Brentm', CAST(0x0000A1C000319FCF AS DateTime), N'M', N'User', N'8', N'StudyCentreId', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (63, N'Brentm', CAST(0x0000A1C000319FCF AS DateTime), N'M', N'User', N'8', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (64, N'Brentm', CAST(0x0000A1C00031F03C AS DateTime), N'M', N'User', N'9', N'FirstName', NULL, N'Carmel')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (65, N'Brentm', CAST(0x0000A1C00031F03C AS DateTime), N'M', N'User', N'9', N'LastName', NULL, N'Delzoppo')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (66, N'Brentm', CAST(0x0000A1C00031F03C AS DateTime), N'M', N'User', N'9', N'StudyCentreId', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (67, N'Brentm', CAST(0x0000A1C00031F03C AS DateTime), N'M', N'User', N'9', N'ProfessionalRoleID', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (68, N'Brentm', CAST(0x0000A1C00032C903 AS DateTime), N'M', N'User', N'10', N'FirstName', NULL, N'Tracey')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (69, N'Brentm', CAST(0x0000A1C00032C903 AS DateTime), N'M', N'User', N'10', N'LastName', NULL, N'Bushell')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (70, N'Brentm', CAST(0x0000A1C00032C903 AS DateTime), N'M', N'User', N'10', N'StudyCentreId', NULL, N'2')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (71, N'Brentm', CAST(0x0000A1C00032C903 AS DateTime), N'M', N'User', N'10', N'ProfessionalRoleID', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (72, N'brentm', CAST(0x0000A1C00032FBEF AS DateTime), N'M', N'User', N'10', N'ProfessionalRoleID', N'1', N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (73, N'brentm', CAST(0x0000A1C00033B684 AS DateTime), N'M', N'User', N'9', N'ProfessionalRoleID', N'1', N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (74, N'Brentm', CAST(0x0000A1C000347AA0 AS DateTime), N'M', N'User', N'11', N'FirstName', NULL, N'Miriam')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (75, N'Brentm', CAST(0x0000A1C000347AA0 AS DateTime), N'M', N'User', N'11', N'LastName', NULL, N'Rea')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (76, N'Brentm', CAST(0x0000A1C000347AA0 AS DateTime), N'M', N'User', N'11', N'StudyCentreId', NULL, N'2')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (77, N'Brentm', CAST(0x0000A1C000347AA0 AS DateTime), N'M', N'User', N'11', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (78, N'Brentm', CAST(0x0000A1C000351A33 AS DateTime), N'M', N'User', N'12', N'FirstName', NULL, N'John')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (79, N'Brentm', CAST(0x0000A1C000351A33 AS DateTime), N'M', N'User', N'12', N'LastName', NULL, N'Beca')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (80, N'Brentm', CAST(0x0000A1C000351A33 AS DateTime), N'M', N'User', N'12', N'StudyCentreId', NULL, N'2')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (81, N'Brentm', CAST(0x0000A1C000351A33 AS DateTime), N'M', N'User', N'12', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (82, N'brentm', CAST(0x0000A1C000393CC0 AS DateTime), N'M', N'StudyCentre', N'3', N'ValidEmailDomains', N'@cmdhb.govt.nz', N'@HUARAHI.HEALTH.GOVT.NZ,@middlemore.co.nz,@cmdhb.org.nz')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (83, N'brentm', CAST(0x0000A1C00067DCD4 AS DateTime), N'M', N'User', N'9', N'IsPublicContact', N'False', N'True')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (84, N'Brentm', CAST(0x0000A1C0006868DE AS DateTime), N'M', N'User', N'13', N'FirstName', NULL, N'Tony')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (85, N'Brentm', CAST(0x0000A1C0006868DE AS DateTime), N'M', N'User', N'13', N'LastName', NULL, N'Williams')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (86, N'Brentm', CAST(0x0000A1C0006868DE AS DateTime), N'M', N'User', N'13', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (87, N'Brentm', CAST(0x0000A1C0006868DE AS DateTime), N'M', N'User', N'13', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (88, N'Brentm', CAST(0x0000A1C0006868DE AS DateTime), N'M', N'User', N'13', N'IsPublicContact', N'False', N'True')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (89, N'Brentm', CAST(0x0000A1C0006B5AC0 AS DateTime), N'M', N'User', N'14', N'FirstName', NULL, N'Carl')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (90, N'Brentm', CAST(0x0000A1C0006B5AC0 AS DateTime), N'M', N'User', N'14', N'LastName', NULL, N'Horsley')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (91, N'Brentm', CAST(0x0000A1C0006B5AC0 AS DateTime), N'M', N'User', N'14', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (92, N'Brentm', CAST(0x0000A1C0006B5AC0 AS DateTime), N'M', N'User', N'14', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (93, N'Brentm', CAST(0x0000A1C1003352DC AS DateTime), N'M', N'User', N'15', N'FirstName', NULL, N'Anna')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (94, N'Brentm', CAST(0x0000A1C1003352DC AS DateTime), N'M', N'User', N'15', N'LastName', NULL, N'Tilsley')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (95, N'Brentm', CAST(0x0000A1C1003352DC AS DateTime), N'M', N'User', N'15', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (96, N'Brentm', CAST(0x0000A1C1003352DC AS DateTime), N'M', N'User', N'15', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (97, N'Brentm', CAST(0x0000A1C10033B4F7 AS DateTime), N'M', N'User', N'16', N'FirstName', NULL, N'Rima')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (98, N'Brentm', CAST(0x0000A1C10033B4F7 AS DateTime), N'M', N'User', N'16', N'LastName', NULL, N'Song')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (99, N'Brentm', CAST(0x0000A1C10033B4F7 AS DateTime), N'M', N'User', N'16', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (100, N'Brentm', CAST(0x0000A1C10033B4F7 AS DateTime), N'M', N'User', N'16', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (101, N'Brentm', CAST(0x0000A1C10034032B AS DateTime), N'M', N'User', N'17', N'FirstName', NULL, N'Laura')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (102, N'Brentm', CAST(0x0000A1C10034032B AS DateTime), N'M', N'User', N'17', N'LastName', NULL, N'Rust')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (103, N'Brentm', CAST(0x0000A1C10034032B AS DateTime), N'M', N'User', N'17', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (104, N'Brentm', CAST(0x0000A1C10034032B AS DateTime), N'M', N'User', N'17', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (105, N'Brentm', CAST(0x0000A1C10034513A AS DateTime), N'M', N'User', N'18', N'FirstName', NULL, N'Chantel')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (106, N'Brentm', CAST(0x0000A1C10034513A AS DateTime), N'M', N'User', N'18', N'LastName', NULL, N'Hogan')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (107, N'Brentm', CAST(0x0000A1C10034513A AS DateTime), N'M', N'User', N'18', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (108, N'Brentm', CAST(0x0000A1C10034513A AS DateTime), N'M', N'User', N'18', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (109, N'Brentm', CAST(0x0000A1C100351D80 AS DateTime), N'D', N'User', N'15', N'*ALL', N'UserName:"Anna Tilsley" Email:"Anna.Tilsley@middlemore.co.nz"', NULL)
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (110, N'Brentm', CAST(0x0000A1C100355712 AS DateTime), N'M', N'User', N'19', N'FirstName', NULL, N'Anna')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (111, N'Brentm', CAST(0x0000A1C100355712 AS DateTime), N'M', N'User', N'19', N'LastName', NULL, N'Tilsley')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (112, N'Brentm', CAST(0x0000A1C100355712 AS DateTime), N'M', N'User', N'19', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (113, N'Brentm', CAST(0x0000A1C100355712 AS DateTime), N'M', N'User', N'19', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (114, N'Brentm', CAST(0x0000A1C100361B4D AS DateTime), N'D', N'User', N'18', N'*ALL', N'UserName:"ChantelH" Email:"Chantel.Hogan@middlemore.co.nz"', NULL)
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (115, N'Brentm', CAST(0x0000A1C100366AE6 AS DateTime), N'M', N'User', N'20', N'FirstName', NULL, N'Chantal')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (116, N'Brentm', CAST(0x0000A1C100366AE6 AS DateTime), N'M', N'User', N'20', N'LastName', NULL, N'Hogan')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (117, N'Brentm', CAST(0x0000A1C100366AE6 AS DateTime), N'M', N'User', N'20', N'StudyCentreId', NULL, N'3')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (118, N'Brentm', CAST(0x0000A1C100366AE6 AS DateTime), N'M', N'User', N'20', N'ProfessionalRoleID', NULL, N'0')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (119, N'Brentm', CAST(0x0000A1C3014D4C0D AS DateTime), N'M', N'User', N'3', N'Email', N'claire@adhb.govt.nz', N'clairesh@adhb.govt.nz')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (120, N'Brentm', CAST(0x0000A1C3014F5EA7 AS DateTime), N'C', N'LocalRecordProvider', N'auto-assign', N'*ALL', NULL, N'Name:"Topaz Index"')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (121, N'Brentm', CAST(0x0000A1C301500C7A AS DateTime), N'C', N'StudyCentre', N'auto-assign', N'*ALL', NULL, N'Abbreviation:"PMH"')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (122, N'Brentm', CAST(0x0000A1C30150A2D3 AS DateTime), N'M', N'User', N'21', N'FirstName', NULL, N'Simon')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (123, N'Brentm', CAST(0x0000A1C30150A2D3 AS DateTime), N'M', N'User', N'21', N'LastName', NULL, N'Erikson')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (124, N'Brentm', CAST(0x0000A1C30150A2D3 AS DateTime), N'M', N'User', N'21', N'StudyCentreId', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (125, N'Brentm', CAST(0x0000A1C30150A2D3 AS DateTime), N'M', N'User', N'21', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (126, N'Brentm', CAST(0x0000A1C30150A2D3 AS DateTime), N'M', N'User', N'21', N'IsPublicContact', N'False', N'True')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (127, N'Brentm', CAST(0x0000A1C3015106D1 AS DateTime), N'M', N'User', N'22', N'FirstName', NULL, N'Geoff')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (128, N'Brentm', CAST(0x0000A1C3015106D1 AS DateTime), N'M', N'User', N'22', N'LastName', NULL, N'Knight')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (129, N'Brentm', CAST(0x0000A1C3015106D1 AS DateTime), N'M', N'User', N'22', N'StudyCentreId', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (130, N'Brentm', CAST(0x0000A1C3015106D1 AS DateTime), N'M', N'User', N'22', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (131, N'Brentm', CAST(0x0000A1C301513969 AS DateTime), N'M', N'User', N'23', N'FirstName', NULL, N'Daniel')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (132, N'Brentm', CAST(0x0000A1C301513969 AS DateTime), N'M', N'User', N'23', N'LastName', NULL, N'Alexander')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (133, N'Brentm', CAST(0x0000A1C301513969 AS DateTime), N'M', N'User', N'23', N'StudyCentreId', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (134, N'Brentm', CAST(0x0000A1C301513969 AS DateTime), N'M', N'User', N'23', N'ProfessionalRoleID', NULL, N'4')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (135, N'Brentm', CAST(0x0000A1C30151798D AS DateTime), N'M', N'StudyCentre', N'4', N'PublicPhoneNumber', N'(+61) 8 93408222', N'(+61) 8 9340 8222')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (136, N'Brentm', CAST(0x0000A1C800190F1D AS DateTime), N'M', N'StudyCentre', N'2', N'SiteRegistrationPwd', N'P@ssword1', N'Coffee2go;)')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (137, N'Beng', CAST(0x0000A1CA0079E4F1 AS DateTime), N'M', N'StudyCentre', N'1', N'SiteRegistrationPwd', N'P@ssword2', N'rchdab!')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (149, N'Brentm', CAST(0x0000A1D900A29002 AS DateTime), N'M', N'User', N'24', N'FirstName', NULL, N'Frank')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (150, N'Brentm', CAST(0x0000A1D900A29002 AS DateTime), N'M', N'User', N'24', N'LastName', NULL, N'Shann')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (151, N'Brentm', CAST(0x0000A1D900A29002 AS DateTime), N'M', N'User', N'24', N'StudyCentreId', NULL, N'1')
INSERT [dbo].[AuditLogEntries] ([AuditLogId], [UserName], [EventDateUTC], [EventType], [TableName], [RecordId], [ColumnName], [OriginalValue], [NewValue]) VALUES (152, N'Brentm', CAST(0x0000A1D900A29002 AS DateTime), N'M', N'User', N'24', N'ProfessionalRoleId', NULL, N'4')
SET IDENTITY_INSERT [dbo].[AuditLogEntries] OFF
/****** Object:  Table [dbo].[AdverseEventTypes]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdverseEventTypes](
	[AdverseEventTypeId] [int] IDENTITY(7,1) NOT NULL,
	[Description] [nvarchar](60) NULL,
 CONSTRAINT [PK_dbo.AdverseEventTypes] PRIMARY KEY CLUSTERED 
(
	[AdverseEventTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AdverseEventTypes] ON
INSERT [dbo].[AdverseEventTypes] ([AdverseEventTypeId], [Description]) VALUES (1, N'arrhythmia')
INSERT [dbo].[AdverseEventTypes] ([AdverseEventTypeId], [Description]) VALUES (2, N'hypertension')
INSERT [dbo].[AdverseEventTypes] ([AdverseEventTypeId], [Description]) VALUES (3, N'tremor')
INSERT [dbo].[AdverseEventTypes] ([AdverseEventTypeId], [Description]) VALUES (6, N'other')
SET IDENTITY_INSERT [dbo].[AdverseEventTypes] OFF
/****** Object:  Table [dbo].[Roles]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(5,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[Rank] [int] NULL,
 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RoleName] ON [dbo].[Roles] 
(
	[RoleName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Roles] ON
INSERT [dbo].[Roles] ([RoleId], [RoleName], [Description], [Rank]) VALUES (1, N'PrincipleInvestigator', N'Principle Investigator', 1)
INSERT [dbo].[Roles] ([RoleId], [RoleName], [Description], [Rank]) VALUES (2, N'SiteInvestigator', N'Study Site Investigator', 2)
INSERT [dbo].[Roles] ([RoleId], [RoleName], [Description], [Rank]) VALUES (3, N'EnrollingClinician', N'Enroling Clinician', 3)
INSERT [dbo].[Roles] ([RoleId], [RoleName], [Description], [Rank]) VALUES (4, N'DatabaseAdmin', N'Database Administrator', NULL)
SET IDENTITY_INSERT [dbo].[Roles] OFF
/****** Object:  Table [dbo].[RespiratorySupportTypes]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RespiratorySupportTypes](
	[RespSupportTypeID] [int] IDENTITY(8,1) NOT NULL,
	[Description] [nvarchar](40) NULL,
	[Explanation] [nvarchar](100) NULL,
	[RandomisationCategory] [int] NULL,
 CONSTRAINT [PK_dbo.RespiratorySupportTypes] PRIMARY KEY CLUSTERED 
(
	[RespSupportTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Description] ON [dbo].[RespiratorySupportTypes] 
(
	[Description] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RespiratorySupportTypes] ON
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (1, N'None', NULL, 1)
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (2, N'Simple oxygen therapy', N'Includes head box, nasal prongs or catheter', 1)
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (3, N'High flow nasal prong oxygen', N'Includes any flows greater than 1 Litre/Kg/minute', 2)
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (4, N'Non-invasive CPAP', N'Includes nasopharyngeal, nasal prong or mask CPAP', 3)
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (5, N'Invasive respiratory support', N'Endotracheal tube with mechanical ventilation or CPAP', 4)
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (6, N'HFOV', N'High frequency oscillation ventilation', 4)
INSERT [dbo].[RespiratorySupportTypes] ([RespSupportTypeID], [Description], [Explanation], [RandomisationCategory]) VALUES (7, N'ECMO', N'Extra-Corporeal Membrane Oxygenation', NULL)
SET IDENTITY_INSERT [dbo].[RespiratorySupportTypes] OFF
/****** Object:  Table [dbo].[StudyCentres]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudyCentres](
	[StudyCentreId] [int] IDENTITY(5,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Abbreviation] [nvarchar](15) NOT NULL,
	[ValidEmailDomains] [nvarchar](100) NOT NULL,
	[SiteRegistrationPwd] [nvarchar](20) NOT NULL,
	[PublicPhoneNumber] [nvarchar](4000) NULL,
	[RecordSystemProviderId] [int] NOT NULL,
	[TimeZoneId] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_dbo.StudyCentres] PRIMARY KEY CLUSTERED 
(
	[StudyCentreId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Name] ON [dbo].[StudyCentres] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RecordSystemProviderId] ON [dbo].[StudyCentres] 
(
	[RecordSystemProviderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SiteRegistrationPwd] ON [dbo].[StudyCentres] 
(
	[SiteRegistrationPwd] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[StudyCentres] ON
INSERT [dbo].[StudyCentres] ([StudyCentreId], [Name], [Abbreviation], [ValidEmailDomains], [SiteRegistrationPwd], [PublicPhoneNumber], [RecordSystemProviderId], [TimeZoneId]) VALUES (1, N'Royal Children''s Hospital Melbourne', N'RCH', N'@rch.org.au', N'rchdab!', N'(+61) 3 9345 5211', 1, N'AUS Eastern Standard Time')
INSERT [dbo].[StudyCentres] ([StudyCentreId], [Name], [Abbreviation], [ValidEmailDomains], [SiteRegistrationPwd], [PublicPhoneNumber], [RecordSystemProviderId], [TimeZoneId]) VALUES (2, N'Starship Children''s Hospital', N'Starship', N'@adhb.govt.nz,@auckland.ac.nz', N'Coffee2go;)', N'(+64) 9 307 4903', 2, N'New Zealand Standard Time')
INSERT [dbo].[StudyCentres] ([StudyCentreId], [Name], [Abbreviation], [ValidEmailDomains], [SiteRegistrationPwd], [PublicPhoneNumber], [RecordSystemProviderId], [TimeZoneId]) VALUES (3, N'Middlemore Hospital', N'MMH', N'@HUARAHI.HEALTH.GOVT.NZ,@middlemore.co.nz,@cmdhb.org.nz', N'P@ssword3', N'(+64) 9 276 0000', 2, N'New Zealand Standard Time')
INSERT [dbo].[StudyCentres] ([StudyCentreId], [Name], [Abbreviation], [ValidEmailDomains], [SiteRegistrationPwd], [PublicPhoneNumber], [RecordSystemProviderId], [TimeZoneId]) VALUES (4, N'Princess Margaret Hospital for Children', N'PMH', N'@health.wa.gov.au', N'P@ssword4', N'(+61) 8 9340 8222', 3, N'W. Australia Standard Time')
SET IDENTITY_INSERT [dbo].[StudyCentres] OFF
/****** Object:  Table [dbo].[Users]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(25,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](150) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[StudyCentreId] [int] NULL,
	[ProfessionalRoleID] [int] NULL,
	[Comment] [nvarchar](150) NULL,
	[IsPublicContact] [bit] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[LastActivityDate] [datetime] NULL,
	[LastLockoutDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[IsLockedOut] [bit] NOT NULL,
	[LastPasswordChangedDate] [datetime] NULL,
	[PasswordVerificationToken] [nvarchar](100) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudyCentreId] ON [dbo].[Users] 
(
	[StudyCentreId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserName] ON [dbo].[Users] 
(
	[UserName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (1, N'Beng', N'Ben.Gelbart@rch.org.au', N'AC4bBoX35r78MWL9OdiF/IBHJ8biJxqwXKAmUhVp7jnZEdS2nazjxedWm+U008wFjA==', N'Ben', N'Gelbart', 1, 4, NULL, 1, 1, 0, CAST(0x0000A1CC001CCA7C AS DateTime), CAST(0x0000A1CC001CDD79 AS DateTime), NULL, CAST(0x0000A1CC001CDD79 AS DateTime), NULL, CAST(0x0000A0D800D6650B AS DateTime), 0, CAST(0x0000A1CC0017D211 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (2, N'Brentm', N'brentm@adhb.govt.nz', N'AHUdWQbWUVMCf1sR4jJEdHuvyPz0ZO+WSMNcJpohPjs15gO938sZ8J9XhbOz08H8Dg==', N'Brent', N'McSharry', 2, 4, NULL, 1, 1, 0, CAST(0x0000A1A201484625 AS DateTime), CAST(0x0000A1DA0173E467 AS DateTime), NULL, CAST(0x0000A1DA0173E467 AS DateTime), NULL, CAST(0x0000A0D800D66512 AS DateTime), 0, NULL, NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (3, N'ClaireS', N'clairesh@adhb.govt.nz', N'AHUdWQbWUVMCf1sR4jJEdHuvyPz0ZO+WSMNcJpohPjs15gO938sZ8J9XhbOz08H8Dg==', N'Claire', N'Sherring', 2, 0, NULL, 1, 1, 0, CAST(0x0000A1C40143AA2B AS DateTime), CAST(0x0000A1DA016F5277 AS DateTime), NULL, CAST(0x0000A1DA016F5277 AS DateTime), NULL, CAST(0x0000A0D800D66517 AS DateTime), 0, NULL, NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (6, N'sivan', N'siva.namachivayam@rch.org.au', N'AK42+wBO3hpoQ5AFbgy9QE9WRDi7PF1OihCD8ldsGedRwTk/sxxfil13hYg53gAhzQ==', N'Siva', N'Namachivayam', 1, 4, NULL, 0, 1, 0, CAST(0x0000A1C00030F183 AS DateTime), CAST(0x0000A1C00030F183 AS DateTime), CAST(0x0000A1C00030F183 AS DateTime), CAST(0x0000A1C00030F183 AS DateTime), NULL, CAST(0x0000A1C00030F183 AS DateTime), 0, CAST(0x0000A1C00030F183 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (7, N'Warwickb', N'Warwick.butt@rch.org.au', N'AAVLjzMJGywC2xyB8Tc15sICq6mLQZtq3Uu8kZ82bnB71PK4Xwt7s8IhWnw+xEuxiQ==', N'Warwick', N'Butt', 1, 4, NULL, 0, 1, 0, CAST(0x0000A1C000314747 AS DateTime), CAST(0x0000A1C000314747 AS DateTime), CAST(0x0000A1C000314747 AS DateTime), CAST(0x0000A1C000314747 AS DateTime), NULL, CAST(0x0000A1C000314747 AS DateTime), 0, CAST(0x0000A1C000314747 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (8, N'Mikes', N'Mike.south@rch.org.au', N'ANEJhzrLaIpLaNS6w12WMcN/uF4+n11k91Qacubnh+ydVlU1hywnlDRBmrgVCpV1Fw==', N'Mike', N'South', 1, 4, NULL, 0, 1, 0, CAST(0x0000A1C000319FB3 AS DateTime), CAST(0x0000A1C0003F3BB9 AS DateTime), CAST(0x0000A1C000319FB3 AS DateTime), CAST(0x0000A1C0003F3BB9 AS DateTime), NULL, CAST(0x0000A1C000319FB3 AS DateTime), 0, CAST(0x0000A1C000319FB3 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (9, N'CarmelD', N'Carmel.Delzoppo@rch.org.au', N'AG5a9h+x2SmHwRKgNejyjzNtmjlZIM6hANiCOOJZCpofASTa5/WEMIvVXCoeHhEpjw==', N'Carmel', N'Delzoppo', 1, 0, NULL, 1, 1, 0, CAST(0x0000A1CB000F9E3B AS DateTime), CAST(0x0000A1CC00175B6A AS DateTime), CAST(0x0000A1C00031F024 AS DateTime), CAST(0x0000A1CC00175B6A AS DateTime), NULL, CAST(0x0000A1C00031F024 AS DateTime), 0, CAST(0x0000A1C50030D80D AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (10, N'TraceyB', N'Tracey.Bushell@adhb.govt.nz', N'AOHH92dTNms6B/WrAfh6mtKTSKlKPEiyO76FEatT8NXXPkr/rhNyPqJmhI+041YebQ==', N'Tracey', N'Bushell', 2, 0, NULL, 0, 1, 0, CAST(0x0000A1C0004DF58E AS DateTime), CAST(0x0000A1CC00037021 AS DateTime), CAST(0x0000A1C00032C8EB AS DateTime), CAST(0x0000A1CC00037021 AS DateTime), NULL, CAST(0x0000A1C00032C8EB AS DateTime), 0, CAST(0x0000A1C0004EB764 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (11, N'MiriamR', N'MiriamR@adhb.govt.nz', N'AHZBJuOWhdc2LKhWwdMmZerdhiybMpkA29A5NUe4zQcp19OnTXlkJGFTy0Y3dhZ5Eg==', N'Miriam', N'Rea', 2, 0, NULL, 0, 1, 0, CAST(0x0000A1C000347A88 AS DateTime), CAST(0x0000A1C7004016BD AS DateTime), CAST(0x0000A1C000347A88 AS DateTime), CAST(0x0000A1C7004016BD AS DateTime), NULL, CAST(0x0000A1C000347A88 AS DateTime), 0, CAST(0x0000A1C300099E8C AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (12, N'JohnB', N'JohnBeca@adhb.govt.nz', N'AB59kCv6wqsCShArPdIAzfZWXesg/723BGtvBMpoxn2itFKP4W6kWpKMioyzYBGffA==', N'John', N'Beca', 2, 4, NULL, 0, 1, 0, CAST(0x0000A1C000351A1C AS DateTime), CAST(0x0000A1C0005B18CD AS DateTime), CAST(0x0000A1C000351A1C AS DateTime), CAST(0x0000A1C0005B04E5 AS DateTime), NULL, CAST(0x0000A1C000351A1C AS DateTime), 0, CAST(0x0000A1C0005B18F2 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (13, N'TWilliams', N'TWilliams@middlemore.co.nz', N'AO5rWc+T2MVb+StFbQbFiIx/RnAW+1R1EMOGJgeOhnqhyx+YpIAPdvlgwX/pz12r3g==', N'Tony', N'Williams', 3, 4, NULL, 1, 1, 0, CAST(0x0000A1C000686877 AS DateTime), CAST(0x0000A1C000686877 AS DateTime), CAST(0x0000A1C000686877 AS DateTime), CAST(0x0000A1C000686877 AS DateTime), NULL, CAST(0x0000A1C000686877 AS DateTime), 0, CAST(0x0000A1C000686877 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (14, N'CarlH', N'Carl.Horsley@cmdhb.org.nz', N'AI5Z1wfIULGdxzfP1U889eFKtxheDKbcHFbM2GMQLEZRBrpGrmo4uGjQiKnogHezTw==', N'Carl', N'Horsley', 3, 4, NULL, 0, 1, 0, CAST(0x0000A1C000719901 AS DateTime), CAST(0x0000A1C000722D6E AS DateTime), CAST(0x0000A1C0006B5A75 AS DateTime), CAST(0x0000A1C00071F16D AS DateTime), NULL, CAST(0x0000A1C0006B5A75 AS DateTime), 0, CAST(0x0000A1C000722D93 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (16, N'RimaS', N'Rima.Song@middlemore.co.nz', N'AGQKZ1ngEeuKspkMsridExojF5SI6jAXn9SrCsEoUbgZSvQd8FjOhbGN47qaTgLpWw==', N'Rima', N'Song', 3, 0, NULL, 0, 1, 0, CAST(0x0000A1C10033B4E0 AS DateTime), CAST(0x0000A1C10033B4E0 AS DateTime), CAST(0x0000A1C10033B4E0 AS DateTime), CAST(0x0000A1C10033B4E0 AS DateTime), NULL, CAST(0x0000A1C10033B4E0 AS DateTime), 0, CAST(0x0000A1C10033B4E0 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (17, N'LauraR', N'Laura.Rust@middlemore.co.nz', N'AL6eQFxR7aSiT1R5vETbK/Ag9N7EZwgS0Tq7jyrx4GXKdmMyxtS7hsk70nob1PCPCw==', N'Laura', N'Rust', 3, 0, NULL, 0, 1, 0, CAST(0x0000A1CB0149E740 AS DateTime), CAST(0x0000A1CB014AC890 AS DateTime), CAST(0x0000A1C100340319 AS DateTime), CAST(0x0000A1CB014A0589 AS DateTime), NULL, CAST(0x0000A1C100340319 AS DateTime), 0, CAST(0x0000A1CB014AC8AC AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (19, N'AnnaT', N'Anna.Tilsley@middlemore.co.nz', N'AICcm0HIOgGGTsBHQnU/InXjesZdZaXSQFIAj1i64HV7G4rgt8GieCJPIcfZGokvEA==', N'Anna', N'Tilsley', 3, 0, NULL, 0, 1, 0, CAST(0x0000A1C1003556F5 AS DateTime), CAST(0x0000A1C4018697D3 AS DateTime), CAST(0x0000A1C1003556F5 AS DateTime), CAST(0x0000A1C4018631E8 AS DateTime), NULL, CAST(0x0000A1C1003556F5 AS DateTime), 0, CAST(0x0000A1C4018697F4 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (20, N'ChantalH', N'Chantal.Hogan@middlemore.co.nz', N'AB+165IUSFVDVpqQNn4kgoTHUy7Ces3QkJwJbOEFwvQsRHffkBQL2tqmWzEQihqFQQ==', N'Chantal', N'Hogan', 3, 0, NULL, 0, 1, 0, CAST(0x0000A1C100366ACF AS DateTime), CAST(0x0000A1C100366ACF AS DateTime), CAST(0x0000A1C100366ACF AS DateTime), CAST(0x0000A1C100366ACF AS DateTime), NULL, CAST(0x0000A1C100366ACF AS DateTime), 0, CAST(0x0000A1C100366ACF AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (21, N'SimonE', N'Simon.Erickson@health.wa.gov.au', N'AIkvONpX6orHI3C9r5Km2LbX4nDvACwH9NLW+ALRnxjSwlzrTdCOvXDjWWccyb1rgg==', N'Simon', N'Erikson', 4, 4, NULL, 1, 1, 0, CAST(0x0000A1C30150A27A AS DateTime), CAST(0x0000A1C8001FDFDB AS DateTime), CAST(0x0000A1C30150A27A AS DateTime), CAST(0x0000A1C8001F353C AS DateTime), NULL, CAST(0x0000A1C30150A27A AS DateTime), 0, CAST(0x0000A1C8001FDFF2 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (22, N'GeoffK', N'Geoff.knight@health.wa.gov.au', N'AGOYFN2qNLCWz9c4I+xezDDKFrhzhlml/IB+cB8Ah66gURECPNoLFIcmraQCUUknXA==', N'Geoff', N'Knight', 4, 4, NULL, 0, 1, 0, CAST(0x0000A1C3015106B5 AS DateTime), CAST(0x0000A1C3015106B5 AS DateTime), CAST(0x0000A1C3015106B5 AS DateTime), CAST(0x0000A1C3015106B5 AS DateTime), NULL, CAST(0x0000A1C3015106B5 AS DateTime), 0, CAST(0x0000A1C3015106B5 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (23, N'DanielA', N'Daniel.alexander@health.wa.gov.au', N'AByBmlJTQ+NR12lULi/fMzTj2jJM7Chy8cIeVk230wsZRe0GxqVJaXToLGWuR2Ty2Q==', N'Daniel', N'Alexander', 4, 4, NULL, 0, 1, 0, CAST(0x0000A1C301513951 AS DateTime), CAST(0x0000A1C301513951 AS DateTime), CAST(0x0000A1C301513951 AS DateTime), CAST(0x0000A1C301513951 AS DateTime), NULL, CAST(0x0000A1C301513951 AS DateTime), 0, CAST(0x0000A1C301513951 AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([UserId], [UserName], [Email], [Password], [FirstName], [LastName], [StudyCentreId], [ProfessionalRoleID], [Comment], [IsPublicContact], [IsApproved], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (24, N'FrankS', N'shannf@netspace.net.au', N'AKgWYjpvLITRO407m1U0TTo6dv6zBzPegn/YPGGTAisFoeFsIgeNk61BsLH2bEovhA==', N'Frank', N'Shann', 1, 4, NULL, 0, 1, 0, CAST(0x0000A1D900A28FE5 AS DateTime), CAST(0x0000A1D900A28FE5 AS DateTime), CAST(0x0000A1D900A28FE5 AS DateTime), CAST(0x0000A1D900A28FE5 AS DateTime), NULL, CAST(0x0000A1D900A28FE5 AS DateTime), 0, CAST(0x0000A1D900A28FE5 AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
/****** Object:  Table [dbo].[ScreenedPatients]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScreenedPatients](
	[ScreenedPatientId] [int] IDENTITY(19,1) NOT NULL,
	[HospitalId] [nvarchar](4000) NOT NULL,
	[StudyCentreId] [int] NOT NULL,
	[AllInclusionCriteriaPresent] [bit] NOT NULL,
	[AllExclusionCriteriaAbsent] [bit] NOT NULL,
	[ConsentRefused] [bit] NOT NULL,
	[ParticipantId] [int] NULL,
	[UserId] [int] NOT NULL,
	[IcuAdmissionDate] [datetime] NOT NULL,
	[Dob] [datetime] NOT NULL,
	[ScreeningDate] [datetime] NOT NULL,
	[NoConsentFreeText] [nvarchar](4000) NULL,
	[NoConsentAttemptId] [int] NULL,
 CONSTRAINT [PK_dbo.ScreenedPatients] PRIMARY KEY CLUSTERED 
(
	[ScreenedPatientId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ParticipantId] ON [dbo].[ScreenedPatients] 
(
	[ParticipantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudyCentreId] ON [dbo].[ScreenedPatients] 
(
	[StudyCentreId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[ScreenedPatients] 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleUsers]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleUsers](
	[Role_RoleId] [int] NOT NULL,
	[User_UserId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RoleUsers] PRIMARY KEY CLUSTERED 
(
	[Role_RoleId] ASC,
	[User_UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Role_RoleId] ON [dbo].[RoleUsers] 
(
	[Role_RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_User_UserId] ON [dbo].[RoleUsers] 
(
	[User_UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 1)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 2)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (4, 2)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 3)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 6)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 7)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 8)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 9)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (4, 9)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 10)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 11)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 12)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 13)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 14)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 16)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 17)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 19)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 20)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 21)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 22)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (2, 23)
INSERT [dbo].[RoleUsers] ([Role_RoleId], [User_UserId]) VALUES (1, 24)
/****** Object:  Table [dbo].[TrialParticipants]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrialParticipants](
	[ParticipantId] [int] IDENTITY(6,1) NOT NULL,
	[HospitalId] [nvarchar](4000) NOT NULL,
	[StudyCentreId] [int] NOT NULL,
	[Dob] [datetime] NOT NULL,
	[Weight] [float] NOT NULL,
	[IcuAdmission] [datetime] NOT NULL,
	[RespSupportTypeID] [int] NOT NULL,
	[MaleGender] [bit] NOT NULL,
	[ChronicLungDisease] [bit] NOT NULL,
	[CyanoticHeartDisease] [bit] NOT NULL,
	[WeeksGestationAtBirth] [int] NOT NULL,
	[IcuDischarge] [datetime] NULL,
	[HospitalDischarge] [datetime] NULL,
	[DaysOfSteroids] [int] NULL,
	[NumberOfAdrenalineNebulisers] [int] NULL,
	[RsvPositive] [bit] NULL,
	[EnrollingClinicianId] [int] NOT NULL,
	[LocalTimeRandomised] [datetime] NOT NULL,
	[InterventionArm] [bit] NOT NULL,
	[BlockNumber] [int] NOT NULL,
	[FamilyName] [nvarchar](80) NULL,
	[HmpvPositive] [bit] NULL,
	[FirstAdrenalineNebAt] [datetime] NULL,
	[LastAdrenalineNebAt] [datetime] NULL,
	[SteroidsForPostExtubationStridor] [bit] NULL,
	[AdrenalineForPostExtubationStridor] [bit] NULL,
 CONSTRAINT [PK_dbo.TrialParticipants] PRIMARY KEY CLUSTERED 
(
	[ParticipantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EnrollingClinicianId] ON [dbo].[TrialParticipants] 
(
	[EnrollingClinicianId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RespSupportTypeID] ON [dbo].[TrialParticipants] 
(
	[RespSupportTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudyCentreId] ON [dbo].[TrialParticipants] 
(
	[StudyCentreId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RespiratorySupportChanges]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RespiratorySupportChanges](
	[RespSupportChangeId] [int] IDENTITY(9,1) NOT NULL,
	[ParticipantId] [int] NOT NULL,
	[ChangeTime] [datetime] NOT NULL,
	[RespiratorySupportTypeId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RespiratorySupportChanges] PRIMARY KEY CLUSTERED 
(
	[RespSupportChangeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ParticipantId] ON [dbo].[RespiratorySupportChanges] 
(
	[ParticipantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RespiratorySupportTypeId] ON [dbo].[RespiratorySupportChanges] 
(
	[RespiratorySupportTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProtocolViolations]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProtocolViolations](
	[ViolationId] [int] IDENTITY(6,1) NOT NULL,
	[ParticipantId] [int] NOT NULL,
	[TimeOfViolation] [datetime] NOT NULL,
	[Details] [nvarchar](4000) NULL,
	[MajorViolation] [bit] NOT NULL,
	[ReportingUserId] [int] NOT NULL,
	[ReportingTimeLocal] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ProtocolViolations] PRIMARY KEY CLUSTERED 
(
	[ViolationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ParticipantId] ON [dbo].[ProtocolViolations] 
(
	[ParticipantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ReportingUserId] ON [dbo].[ProtocolViolations] 
(
	[ReportingUserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParticipantWithdrawals]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParticipantWithdrawals](
	[Id] [int] NOT NULL,
	[Time] [datetime] NOT NULL,
	[Reason] [nvarchar](4000) NULL,
 CONSTRAINT [PK_dbo.ParticipantWithdrawals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Id] ON [dbo].[ParticipantWithdrawals] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParticipantDeaths]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParticipantDeaths](
	[Id] [int] NOT NULL,
	[Time] [datetime] NOT NULL,
	[Details] [nvarchar](4000) NULL,
 CONSTRAINT [PK_dbo.ParticipantDeaths] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Id] ON [dbo].[ParticipantDeaths] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdverseEvents]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdverseEvents](
	[AdverseEventId] [int] IDENTITY(8,1) NOT NULL,
	[ParticipantId] [int] NOT NULL,
	[EventTime] [datetime] NOT NULL,
	[SeverityLevelId] [int] NOT NULL,
	[Sequelae] [bit] NOT NULL,
	[ReportingUserId] [int] NOT NULL,
	[ReportingTimeLocal] [datetime] NOT NULL,
	[FatalEvent] [bit] NOT NULL,
	[Details] [nvarchar](4000) NULL,
	[AdverseEventTypeId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.AdverseEvents] PRIMARY KEY CLUSTERED 
(
	[AdverseEventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdverseEventTypeId] ON [dbo].[AdverseEvents] 
(
	[AdverseEventTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ParticipantId] ON [dbo].[AdverseEvents] 
(
	[ParticipantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ReportingUserId] ON [dbo].[AdverseEvents] 
(
	[ReportingUserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Drugs]    Script Date: 06/12/2013 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Drugs](
	[DrugId] [int] IDENTITY(4,1) NOT NULL,
	[AdverseEventId] [int] NOT NULL,
	[DrugName] [nvarchar](50) NULL,
	[Dosage] [nvarchar](50) NULL,
	[StartDate] [datetime] NOT NULL,
	[StopDate] [datetime] NULL,
	[ReasonsForUse] [nvarchar](4000) NULL,
 CONSTRAINT [PK_dbo.Drugs] PRIMARY KEY CLUSTERED 
(
	[DrugId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdverseEventId] ON [dbo].[Drugs] 
(
	[AdverseEventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF__ScreenedP__AllIn__09DE7BCC]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ((0)) FOR [AllInclusionCriteriaPresent]
GO
/****** Object:  Default [DF__ScreenedP__AllEx__0AD2A005]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ((0)) FOR [AllExclusionCriteriaAbsent]
GO
/****** Object:  Default [DF__ScreenedP__Conse__0BC6C43E]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ((0)) FOR [ConsentRefused]
GO
/****** Object:  Default [DF__ScreenedP__UserI__0CBAE877]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ((0)) FOR [UserId]
GO
/****** Object:  Default [DF__ScreenedP__IcuAd__0DAF0CB0]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [IcuAdmissionDate]
GO
/****** Object:  Default [DF__ScreenedPat__Dob__0EA330E9]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [Dob]
GO
/****** Object:  Default [DF__ScreenedP__Scree__0F975522]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [ScreeningDate]
GO
/****** Object:  Default [DF__AdverseEv__Adver__1367E606]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[AdverseEvents] ADD  DEFAULT ((0)) FOR [AdverseEventTypeId]
GO
/****** Object:  ForeignKey [FK_dbo.StudyCentres_dbo.LocalRecordProviders_RecordSystemProviderId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[StudyCentres]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StudyCentres_dbo.LocalRecordProviders_RecordSystemProviderId] FOREIGN KEY([RecordSystemProviderId])
REFERENCES [dbo].[LocalRecordProviders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudyCentres] CHECK CONSTRAINT [FK_dbo.StudyCentres_dbo.LocalRecordProviders_RecordSystemProviderId]
GO
/****** Object:  ForeignKey [FK_dbo.Users_dbo.StudyCentres_StudyCentreId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.StudyCentres_StudyCentreId] FOREIGN KEY([StudyCentreId])
REFERENCES [dbo].[StudyCentres] ([StudyCentreId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.StudyCentres_StudyCentreId]
GO
/****** Object:  ForeignKey [FK_dbo.ScreenedPatients_dbo.StudyCentres_StudyCentreId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ScreenedPatients_dbo.StudyCentres_StudyCentreId] FOREIGN KEY([StudyCentreId])
REFERENCES [dbo].[StudyCentres] ([StudyCentreId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ScreenedPatients] CHECK CONSTRAINT [FK_dbo.ScreenedPatients_dbo.StudyCentres_StudyCentreId]
GO
/****** Object:  ForeignKey [FK_dbo.ScreenedPatients_dbo.Users_UserId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ScreenedPatients_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ScreenedPatients] CHECK CONSTRAINT [FK_dbo.ScreenedPatients_dbo.Users_UserId]
GO
/****** Object:  ForeignKey [FK_ScreenedPatients_NoConsentAttempts]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ScreenedPatients]  WITH CHECK ADD  CONSTRAINT [FK_ScreenedPatients_NoConsentAttempts] FOREIGN KEY([NoConsentAttemptId])
REFERENCES [dbo].[NoConsentAttempts] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ScreenedPatients] CHECK CONSTRAINT [FK_ScreenedPatients_NoConsentAttempts]
GO
/****** Object:  ForeignKey [FK_dbo.RoleUsers_dbo.Roles_Role_RoleId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[RoleUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RoleUsers_dbo.Roles_Role_RoleId] FOREIGN KEY([Role_RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleUsers] CHECK CONSTRAINT [FK_dbo.RoleUsers_dbo.Roles_Role_RoleId]
GO
/****** Object:  ForeignKey [FK_dbo.RoleUsers_dbo.Users_User_UserId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[RoleUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RoleUsers_dbo.Users_User_UserId] FOREIGN KEY([User_UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleUsers] CHECK CONSTRAINT [FK_dbo.RoleUsers_dbo.Users_User_UserId]
GO
/****** Object:  ForeignKey [FK_dbo.TrialParticipants_dbo.RespiratorySupportTypes_RespSupportTypeID]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[TrialParticipants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrialParticipants_dbo.RespiratorySupportTypes_RespSupportTypeID] FOREIGN KEY([RespSupportTypeID])
REFERENCES [dbo].[RespiratorySupportTypes] ([RespSupportTypeID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TrialParticipants] CHECK CONSTRAINT [FK_dbo.TrialParticipants_dbo.RespiratorySupportTypes_RespSupportTypeID]
GO
/****** Object:  ForeignKey [FK_dbo.TrialParticipants_dbo.StudyCentres_StudyCentreId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[TrialParticipants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrialParticipants_dbo.StudyCentres_StudyCentreId] FOREIGN KEY([StudyCentreId])
REFERENCES [dbo].[StudyCentres] ([StudyCentreId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TrialParticipants] CHECK CONSTRAINT [FK_dbo.TrialParticipants_dbo.StudyCentres_StudyCentreId]
GO
/****** Object:  ForeignKey [FK_dbo.TrialParticipants_dbo.Users_EnrollingClinicianId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[TrialParticipants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrialParticipants_dbo.Users_EnrollingClinicianId] FOREIGN KEY([EnrollingClinicianId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TrialParticipants] CHECK CONSTRAINT [FK_dbo.TrialParticipants_dbo.Users_EnrollingClinicianId]
GO
/****** Object:  ForeignKey [FK_dbo.RespiratorySupportChanges_dbo.RespiratorySupportTypes_RespiratorySupportTypeId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[RespiratorySupportChanges]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RespiratorySupportChanges_dbo.RespiratorySupportTypes_RespiratorySupportTypeId] FOREIGN KEY([RespiratorySupportTypeId])
REFERENCES [dbo].[RespiratorySupportTypes] ([RespSupportTypeID])
GO
ALTER TABLE [dbo].[RespiratorySupportChanges] CHECK CONSTRAINT [FK_dbo.RespiratorySupportChanges_dbo.RespiratorySupportTypes_RespiratorySupportTypeId]
GO
/****** Object:  ForeignKey [FK_dbo.RespiratorySupportChanges_dbo.TrialParticipants_ParticipantId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[RespiratorySupportChanges]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RespiratorySupportChanges_dbo.TrialParticipants_ParticipantId] FOREIGN KEY([ParticipantId])
REFERENCES [dbo].[TrialParticipants] ([ParticipantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RespiratorySupportChanges] CHECK CONSTRAINT [FK_dbo.RespiratorySupportChanges_dbo.TrialParticipants_ParticipantId]
GO
/****** Object:  ForeignKey [FK_dbo.ProtocolViolations_dbo.TrialParticipants_ParticipantId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ProtocolViolations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProtocolViolations_dbo.TrialParticipants_ParticipantId] FOREIGN KEY([ParticipantId])
REFERENCES [dbo].[TrialParticipants] ([ParticipantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProtocolViolations] CHECK CONSTRAINT [FK_dbo.ProtocolViolations_dbo.TrialParticipants_ParticipantId]
GO
/****** Object:  ForeignKey [FK_dbo.ProtocolViolations_dbo.Users_ReportingUserId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ProtocolViolations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProtocolViolations_dbo.Users_ReportingUserId] FOREIGN KEY([ReportingUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ProtocolViolations] CHECK CONSTRAINT [FK_dbo.ProtocolViolations_dbo.Users_ReportingUserId]
GO
/****** Object:  ForeignKey [FK_dbo.ParticipantWithdrawals_dbo.TrialParticipants_Id]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ParticipantWithdrawals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ParticipantWithdrawals_dbo.TrialParticipants_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[TrialParticipants] ([ParticipantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParticipantWithdrawals] CHECK CONSTRAINT [FK_dbo.ParticipantWithdrawals_dbo.TrialParticipants_Id]
GO
/****** Object:  ForeignKey [FK_dbo.ParticipantDeaths_dbo.TrialParticipants_Id]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[ParticipantDeaths]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ParticipantDeaths_dbo.TrialParticipants_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[TrialParticipants] ([ParticipantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParticipantDeaths] CHECK CONSTRAINT [FK_dbo.ParticipantDeaths_dbo.TrialParticipants_Id]
GO
/****** Object:  ForeignKey [FK_dbo.AdverseEvents_dbo.AdverseEventTypes_AdverseEventTypeId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[AdverseEvents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdverseEvents_dbo.AdverseEventTypes_AdverseEventTypeId] FOREIGN KEY([AdverseEventTypeId])
REFERENCES [dbo].[AdverseEventTypes] ([AdverseEventTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdverseEvents] CHECK CONSTRAINT [FK_dbo.AdverseEvents_dbo.AdverseEventTypes_AdverseEventTypeId]
GO
/****** Object:  ForeignKey [FK_dbo.AdverseEvents_dbo.TrialParticipants_ParticipantId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[AdverseEvents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdverseEvents_dbo.TrialParticipants_ParticipantId] FOREIGN KEY([ParticipantId])
REFERENCES [dbo].[TrialParticipants] ([ParticipantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdverseEvents] CHECK CONSTRAINT [FK_dbo.AdverseEvents_dbo.TrialParticipants_ParticipantId]
GO
/****** Object:  ForeignKey [FK_dbo.AdverseEvents_dbo.Users_ReportingUserId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[AdverseEvents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdverseEvents_dbo.Users_ReportingUserId] FOREIGN KEY([ReportingUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AdverseEvents] CHECK CONSTRAINT [FK_dbo.AdverseEvents_dbo.Users_ReportingUserId]
GO
/****** Object:  ForeignKey [FK_dbo.Drugs_dbo.AdverseEvents_AdverseEventId]    Script Date: 06/12/2013 15:17:03 ******/
ALTER TABLE [dbo].[Drugs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Drugs_dbo.AdverseEvents_AdverseEventId] FOREIGN KEY([AdverseEventId])
REFERENCES [dbo].[AdverseEvents] ([AdverseEventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Drugs] CHECK CONSTRAINT [FK_dbo.Drugs_dbo.AdverseEvents_AdverseEventId]
GO
