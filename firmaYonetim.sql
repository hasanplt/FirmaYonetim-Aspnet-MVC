USE [master]
GO
/****** Object:  Database [FirmaYonetim]    Script Date: 23.07.2022 11:38:36 ******/
CREATE DATABASE [FirmaYonetim]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FirmaYonetim', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\FirmaYonetim.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FirmaYonetim_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\FirmaYonetim_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [FirmaYonetim] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FirmaYonetim].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FirmaYonetim] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FirmaYonetim] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FirmaYonetim] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FirmaYonetim] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FirmaYonetim] SET ARITHABORT OFF 
GO
ALTER DATABASE [FirmaYonetim] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FirmaYonetim] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FirmaYonetim] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FirmaYonetim] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FirmaYonetim] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FirmaYonetim] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FirmaYonetim] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FirmaYonetim] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FirmaYonetim] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FirmaYonetim] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FirmaYonetim] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FirmaYonetim] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FirmaYonetim] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FirmaYonetim] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FirmaYonetim] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FirmaYonetim] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FirmaYonetim] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FirmaYonetim] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [FirmaYonetim] SET  MULTI_USER 
GO
ALTER DATABASE [FirmaYonetim] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FirmaYonetim] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FirmaYonetim] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FirmaYonetim] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FirmaYonetim] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FirmaYonetim] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [FirmaYonetim] SET QUERY_STORE = OFF
GO
USE [FirmaYonetim]
GO
/****** Object:  User [hasanyan]    Script Date: 23.07.2022 11:38:36 ******/
CREATE USER [hasanyan] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [hasan2]    Script Date: 23.07.2022 11:38:36 ******/
CREATE USER [hasan2] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [hasanyan]
GO
ALTER ROLE [db_owner] ADD MEMBER [hasan2]
GO
/****** Object:  Table [dbo].[Activity]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Activity](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[CompanyId] [uniqueidentifier] NULL,
	[AddressId] [uniqueidentifier] NULL,
	[ContactId] [uniqueidentifier] NULL,
	[ActivityTypeId] [uniqueidentifier] NULL,
	[Title] [nvarchar](50) NULL,
	[Text] [text] NULL,
	[Date] [datetime] NULL,
	[Status] [tinyint] NULL,
	[StatusUpdateDateTime] [datetime] NULL,
	[CreatedDateTime] [datetime] NULL,
	[IsMailSend] [bit] NULL,
 CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActivityType]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityType](
	[Id] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](50) NULL,
	[IsDelete] [bit] NULL,
	[IsDeleteDateTime] [datetime] NULL,
	[CreatedByUserId] [uniqueidentifier] NULL,
	[CreatedDateTime] [datetime] NULL,
 CONSTRAINT [PK_ActivityType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [uniqueidentifier] NOT NULL,
	[CompanyId] [uniqueidentifier] NULL,
	[Name] [nvarchar](30) NULL,
	[Province] [varchar](50) NULL,
	[District] [varchar](50) NULL,
	[AddressDetail] [text] NULL,
	[Phone1] [varchar](25) NULL,
	[Phone2] [varchar](25) NULL,
	[Phone3] [varchar](25) NULL,
	[Email] [varchar](50) NULL,
	[IsDelete] [bit] NULL,
	[IsDeleteDateTime] [datetime] NULL,
	[CreatedDateTime] [datetime] NULL,
	[CreatedByUserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Title] [nvarchar](50) NULL,
	[CreatedByUserId] [uniqueidentifier] NULL,
	[CreateDateTime] [datetime] NULL,
	[UpdateDateTime] [datetime] NULL,
	[UpdatedByUserId] [uniqueidentifier] NULL,
	[IsDelete] [bit] NULL,
	[IsDeleteDateTime] [datetime] NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contact]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[Id] [uniqueidentifier] NOT NULL,
	[AddressId] [uniqueidentifier] NULL,
	[Title] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Surname] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[GSM] [nvarchar](25) NULL,
	[LandPhone] [nvarchar](25) NULL,
	[LandPhoneInternal] [nvarchar](25) NULL,
	[IsDelete] [bit] NULL,
	[IsDeleteDateTime] [datetime] NULL,
	[CreatedDateTime] [datetime] NULL,
	[CreatedByUserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SharedCompany]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SharedCompany](
	[Id] [uniqueidentifier] NOT NULL,
	[CompanyId] [uniqueidentifier] NULL,
	[WhoSharedUserId] [uniqueidentifier] NULL,
	[SeeUserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_SharedCompany] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ToDoList]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ToDoList](
	[Id] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](100) NULL,
	[Date] [datetime] NULL,
	[CreatedByUserId] [uniqueidentifier] NULL,
	[CreatedDateTime] [datetime] NULL,
	[IsDelete] [bit] NULL,
	[IsDeleteDateTime] [datetime] NULL,
 CONSTRAINT [PK_ToDoList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Surname] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[LockCount] [tinyint] NULL,
	[LockDateTime] [datetime] NULL,
	[CreateDateTime] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserWrongLoginLog]    Script Date: 23.07.2022 11:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserWrongLoginLog](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[WrongPassword] [nvarchar](50) NULL,
	[CreateDateTime] [datetime] NULL,
 CONSTRAINT [PK_UserWrongLoginLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_Activity_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_Activity_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_Activity_IsMailSend]  DEFAULT ((0)) FOR [IsMailSend]
GO
ALTER TABLE [dbo].[ActivityType] ADD  CONSTRAINT [DF_ActivityType_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ActivityType] ADD  CONSTRAINT [DF_ActivityType_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[ActivityType] ADD  CONSTRAINT [DF_ActivityType_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF_Address_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF_Address_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF_Address_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_Company_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_Company_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_Company_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Contact] ADD  CONSTRAINT [DF_Contact_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Contact] ADD  CONSTRAINT [DF_Contact_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Contact] ADD  CONSTRAINT [DF_Contact_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[SharedCompany] ADD  CONSTRAINT [DF_SharedCompany_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ToDoList] ADD  CONSTRAINT [DF_ToDoList_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ToDoList] ADD  CONSTRAINT [DF_ToDoList_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO
ALTER TABLE [dbo].[UserWrongLoginLog] ADD  CONSTRAINT [DF_UserWrongLoginLog_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserWrongLoginLog] ADD  CONSTRAINT [DF_UserWrongLoginLog_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO
ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_ActivityType] FOREIGN KEY([ActivityTypeId])
REFERENCES [dbo].[ActivityType] ([Id])
GO
ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_ActivityType]
GO
ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_Address]
GO
ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_Company]
GO
ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([Id])
GO
ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_Contact]
GO
ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_User]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Company]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_User] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_User]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_User] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_User]
GO
ALTER TABLE [dbo].[Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Contact] CHECK CONSTRAINT [FK_Contact_Address]
GO
ALTER TABLE [dbo].[Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_User] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Contact] CHECK CONSTRAINT [FK_Contact_User]
GO
ALTER TABLE [dbo].[SharedCompany]  WITH CHECK ADD  CONSTRAINT [FK_SharedCompany_User] FOREIGN KEY([WhoSharedUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[SharedCompany] CHECK CONSTRAINT [FK_SharedCompany_User]
GO
ALTER TABLE [dbo].[SharedCompany]  WITH CHECK ADD  CONSTRAINT [FK_SharedCompany_User1] FOREIGN KEY([SeeUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[SharedCompany] CHECK CONSTRAINT [FK_SharedCompany_User1]
GO
ALTER TABLE [dbo].[ToDoList]  WITH CHECK ADD  CONSTRAINT [FK_ToDoList_User] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ToDoList] CHECK CONSTRAINT [FK_ToDoList_User]
GO
ALTER TABLE [dbo].[UserWrongLoginLog]  WITH CHECK ADD  CONSTRAINT [FK_UserWrongLoginLog_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserWrongLoginLog] CHECK CONSTRAINT [FK_UserWrongLoginLog_User]
GO
USE [master]
GO
ALTER DATABASE [FirmaYonetim] SET  READ_WRITE 
GO
