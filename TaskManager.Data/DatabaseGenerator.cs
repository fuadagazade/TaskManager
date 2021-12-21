using Dapper;
using System.Data;

namespace TaskManager.Data;

public static class DatabaseGenerator
{
    public static void Install(IDbConnection connection)
    {
        Organizations(connection);
        Users(connection);
        Tasks(connection);
        Assigments(connection);
    }

    private static void Organizations(IDbConnection connection)
    {
        string command = $@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Organizations' AND xtype='U')
                            CREATE TABLE [dbo].[Organizations] (
                                [Id]        INT IDENTITY (1, 1) PRIMARY KEY,
                                [Name]      NVARCHAR (150) NOT NULL,
                                [Tag]       VARCHAR (150) NOT NULL UNIQUE,
                                [Phone]     VARCHAR (15) NULL,
                                [Address]   NVARCHAR (MAX) NULL,
                                [Logo]      VARCHAR (100) NULL,
                                [Status]    int DEFAULT 1
                            );";

        try
        {
            connection.Execute(command);
        }
        catch (Exception ex) { return; }
    }

    private static void Users(IDbConnection connection)
    {
        string command = $@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                            CREATE TABLE [dbo].[Users] (
                                [Id]                INT IDENTITY (1, 1) PRIMARY KEY,
                                [FirstName]         NVARCHAR (150) NOT NULL,
                                [LastName]          NVARCHAR (150) NOT NULL,
                                [Email]             NVARCHAR (200) NOT NULL,
                                [Password]          NVARCHAR (250) NOT NULL,
                                [Image]             VARCHAR (100) NULL,
                                [Role]              int NOT NULL,
                                [OrganizationId]    int FOREIGN KEY REFERENCES Organizations(Id) NOT NULL,
                                [Approved]          bit DEFAULT 0,
                                [Status]            int DEFAULT 1,
                                CONSTRAINT UC_User UNIQUE (Email,OrganizationId)
                            );";

        try
        {
            connection.Execute(command);
        }
        catch (Exception ex) { return; }
    }

    private static void Tasks(IDbConnection connection)
    {
        string command = $@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
                            CREATE TABLE [dbo].[Tasks] (
                                [Id]                INT IDENTITY (1, 1) PRIMARY KEY,
                                [Title]             NVARCHAR (250) NOT NULL,
                                [Description]       NVARCHAR (MAX),
                                [Priority]          int DEFAULT 1,
                                [Point]             int DEFAULT 1,
                                [State]             int DEFAULT 1,
                                [Deadline]          datetime,
                                [OrganizationId]    int FOREIGN KEY REFERENCES Organizations(Id) NOT NULL,
                                [CreatorId]         int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
                                [CreateDate]        datetime DEFAULT GETDATE(),
                                [Status]            int DEFAULT 1
                            );";

        try
        {
            connection.Execute(command);
        }
        catch (Exception ex) { return; }
    }

    private static void Assigments(IDbConnection connection)
    {
        string command = $@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Assigments' AND xtype='U')
                            CREATE TABLE [dbo].[Assigments] (
                                [Id]                INT IDENTITY (1, 1) PRIMARY KEY,
                                [TaskId]            int FOREIGN KEY REFERENCES Tasks(Id) NOT NULL,
                                [UserId]            int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
                                [Status]            int DEFAULT 1
                            );";

        try
        {
            connection.Execute(command);
        }
        catch (Exception ex) { return; }
    }
}

