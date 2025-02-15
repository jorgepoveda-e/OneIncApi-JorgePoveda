-- Step 1: Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'oneinc')
BEGIN
    CREATE DATABASE oneinc;
END
GO

-- Step 2: Use the newly created database
USE oneinc;
GO

-- Step 3: Create the Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        FirstName NVARCHAR(128) NOT NULL,
        LastName NVARCHAR(128),
        Email NVARCHAR(255) NOT NULL UNIQUE,
        DateOfBirth DATE NOT NULL,
        PhoneNumber CHAR(10) NOT NULL CHECK (PhoneNumber LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    );

    -- Step 4: Create an index on Email for faster lookups
    CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
END
GO
