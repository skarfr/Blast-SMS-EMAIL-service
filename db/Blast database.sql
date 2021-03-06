USE [Blast]

CREATE TABLE [dbo].[MessagesOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sender] [varchar](250) NOT NULL,
	[SenderName] [varchar](50) NULL,
	[Receiver] [varchar](255) NOT NULL,
	[ReceiverName] [varchar](50) NULL,
	[CustomerId] [varchar](25) NULL,
	[Subject] [varchar](250) NULL,
	[Message] [varchar](max) NOT NULL,
	[AddTime] [datetime] NOT NULL CONSTRAINT [DF_MessagesOut_AddTime]  DEFAULT (getdate()),
	[SentTime] [datetime] NULL,
	[LastCheckTime] [datetime] NULL,
	[Status] [varchar](10) NOT NULL CONSTRAINT [DF_MessagesOut_Status]  DEFAULT ('send'),
	[MessageType] [varchar](10) NOT NULL,
	[Provider] [varchar](50) NULL,
	[IdMessageProvider] [varchar](150) NULL,
	[Reason] [varchar](max) NULL,
 CONSTRAINT [PK_MessageOut] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[Providers](
	[Provider] [varchar](50) NOT NULL,
	[Url] [varchar](500) NOT NULL,
	[Parameters] [varchar](max) NULL,
	[MessageType] [varchar](10) NOT NULL,
	[RequestEncoding] [varchar](30) NOT NULL,
	[RequestMIMEType] [varchar](150) NOT NULL,
	[RegexSuccess] [varchar](max) NOT NULL,
	[Enable] [bit] NOT NULL,
	[CheckUrl] [varchar](250) NULL,
 CONSTRAINT [PK_Providers] PRIMARY KEY CLUSTERED 
(
	[Provider] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[ProvidersStatus](
	[Provider] [varchar](50) NOT NULL,
	[Id] [varchar](150) NOT NULL,
	[Action] [varchar](10) NOT NULL,
	[Description] [varchar](250) NOT NULL,
 CONSTRAINT [PK_ProvidersStatus] PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[Id] ASC,
	[Action] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



CREATE PROCEDURE [dbo].[report_BLAST]
		@PhoneNumber  varchar(20) = '' -- Or CustomerId
AS
BEGIN

  SET NOCOUNT ON;
  IF @PhoneNumber = '%' SET @PhoneNumber = '' -- so the report can not display the whole phone numbers db
  IF IsNull(@PhoneNumber,'') <> '' SET @PhoneNumber = @PhoneNumber+'%'
  
  SELECT [Id]
        ,[MessageType]    AS [Type]
        ,[CustomerId]
        ,[Subject]
        ,CONVERT(VARCHAR(8000), [Message]) AS [Message]
        ,[Status]
        ,[SentTime]
        ,[AddTime]
        ,[Sender]
        ,[SenderName]
        ,[Receiver]
        ,[ReceiverName]
        ,[LastCheckTime]
        ,[Provider]
        ,[IdMessageProvider]
        --,[Reason]
  FROM MessagesOut WITH (nolock)
  WHERE [receiver] LIKE @PhoneNumber OR [CustomerId] LIKE @PhoneNumber
  ORDER BY [senttime] DESC
   
END
GO
