-- Script to add IsActive column to Users table
-- Execute this script to add the missing IsActive column

USE SIOS;
GO

-- Add IsActive column to Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsActive')
BEGIN
    ALTER TABLE Users ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT 'IsActive column added to Users table successfully.';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in Users table.';
END
GO

-- Update existing users to be active by default (if any exist)
UPDATE Users SET IsActive = 1 WHERE IsActive IS NULL;
GO

PRINT 'Users table updated successfully.';