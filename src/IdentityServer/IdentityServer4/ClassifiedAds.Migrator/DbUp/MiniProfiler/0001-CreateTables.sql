CREATE TABLE [MiniProfilerClientTimings] (
    [RowId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [MiniProfilerId] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NULL,
    [Start] decimal(9,3) NOT NULL,
    [Duration] decimal(9,3) NOT NULL,
    CONSTRAINT [PK_MiniProfilerClientTimings] PRIMARY KEY ([RowId])
);
GO


CREATE TABLE [MiniProfilers] (
    [RowId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [RootTimingId] uniqueidentifier NULL,
    [Name] nvarchar(200) NULL,
    [Started] datetime2 NOT NULL,
    [DurationMilliseconds] decimal(15,1) NOT NULL,
    [User] nvarchar(100) NULL,
    [HasUserViewed] bit NOT NULL,
    [MachineName] nvarchar(100) NULL,
    [CustomLinksJson] nvarchar(max) NULL,
    [ClientTimingsRedirectCount] int NULL,
    CONSTRAINT [PK_MiniProfilers] PRIMARY KEY ([RowId])
);
GO


CREATE TABLE [MiniProfilerTimings] (
    [RowId] int NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [MiniProfilerId] uniqueidentifier NOT NULL,
    [ParentTimingId] uniqueidentifier NULL,
    [Name] nvarchar(200) NULL,
    [DurationMilliseconds] decimal(15,3) NOT NULL,
    [StartMilliseconds] decimal(15,3) NOT NULL,
    [IsRoot] bit NOT NULL,
    [Depth] smallint NOT NULL,
    [CustomTimingsJson] nvarchar(max) NULL,
    CONSTRAINT [PK_MiniProfilerTimings] PRIMARY KEY ([RowId])
);
GO


CREATE UNIQUE INDEX [IX_MiniProfilerClientTimings_Id] ON [MiniProfilerClientTimings] ([Id]);
GO


CREATE INDEX [IX_MiniProfilerClientTimings_MiniProfilerId] ON [MiniProfilerClientTimings] ([MiniProfilerId]);
GO


CREATE UNIQUE INDEX [IX_MiniProfilers_Id] ON [MiniProfilers] ([Id]);
GO


CREATE INDEX [IX_MiniProfilers_User_HasUserViewed] ON [MiniProfilers] ([User], [HasUserViewed]) INCLUDE ([Id], [Started]);
GO


CREATE UNIQUE INDEX [IX_MiniProfilerTimings_Id] ON [MiniProfilerTimings] ([Id]);
GO


CREATE INDEX [IX_MiniProfilerTimings_MiniProfilerId] ON [MiniProfilerTimings] ([MiniProfilerId]);
GO


