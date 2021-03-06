USE [ProjectManager]
GO
/****** Object:  Table [dbo].[ParentTask]    Script Date: 13-06-2019 14:06:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParentTask](
	[Parent_ID] [int] IDENTITY(1,1) NOT NULL,
	[Parent_Task] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Parent_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Project]    Script Date: 13-06-2019 14:06:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[Project_ID] [int] IDENTITY(1,1) NOT NULL,
	[Project] [nvarchar](50) NULL,
	[Start Date] [datetime] NULL,
	[End Date] [datetime] NULL,
	[Priority] [int] NULL,
 CONSTRAINT [PK__Project__1CB92FE368A85B33] PRIMARY KEY CLUSTERED 
(
	[Project_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Task]    Script Date: 13-06-2019 14:06:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[Task_ID] [int] IDENTITY(1,1) NOT NULL,
	[Parent_ID] [int] NULL,
	[Project_ID] [int] NULL,
	[Task] [nvarchar](50) NULL,
	[Start_Date] [datetime2](7) NULL,
	[End_Date] [datetime2](7) NULL,
	[Priority] [int] NULL,
	[Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Task_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 13-06-2019 14:06:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[User_ID] [int] IDENTITY(1,1) NOT NULL,
	[First Name] [nvarchar](50) NOT NULL,
	[Last Name] [nvarchar](50) NOT NULL,
	[Employee_ID] [nchar](10) NOT NULL,
	[Task_ID] [int] NULL,
	[Project_ID] [int] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Task] ADD  DEFAULT ((0)) FOR [Status]
GO
