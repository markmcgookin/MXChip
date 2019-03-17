IF object_id('SensorData', 'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[SensorData]
	(
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[Partition] [varchar] (3) NOT NULL,
        [Type] [nvarchar](200) NOT NULL,
        [Value] [nvarchar](200) NOT NULL,
		[Created] [datetime] NOT NULL,
		[Modified] [datetime] NOT NULL,
		CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END