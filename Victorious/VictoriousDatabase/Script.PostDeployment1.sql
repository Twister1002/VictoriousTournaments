/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

DECLARE @count int
SET @count = 1

-- Platform inserts

INSERT Platforms SELECT 'Xbox'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Xbox')
SET @count = @count + 1

INSERT Platforms SELECT 'Xbox 360'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Xbox 360')
SET @count = @count + 1

INSERT Platforms SELECT'Xbox One'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Xbox One')
SET @count = @count + 1

INSERT Platforms SELECT 'PlayStation 1'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'PlayStation 1')
SET @count = @count + 1

INSERT Platforms SELECT 'PlayStation 2'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'PlayStation 2')
SET @count = @count + 1

INSERT Platforms SELECT 'PlayStation 3'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'PlayStation 3')
SET @count = @count + 1

INSERT Platforms SELECT 'PlayStation 4'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'PlayStation 4')
SET @count = @count + 1

INSERT Platforms SELECT 'PC'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'PC')
SET @count = @count + 1

INSERT Platforms SELECT 'Nintendo 64'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Nintendo 64')
SET @count = @count + 1

INSERT Platforms SELECT 'Super NES'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Super NES')
SET @count = @count + 1

INSERT Platforms SELECT 'NES'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'NES')
SET @count = @count + 1

INSERT Platforms SELECT 'GameCube'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'GameCube')
SET @count = @count + 1

INSERT Platforms SELECT 'Wii'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Wii')
SET @count = @count + 1

INSERT Platforms SELECT 'Wii U'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Wii U')
SET @count = @count + 1

INSERT Platforms SELECT 'Nintendo Switch'
	WHERE NOT EXISTS (SELECT @count FROM dbo.Platforms WHERE PlatformName = 'Nintendo Switch')
SET @count = @count + 1


-- Game inserts

SET @count = 1

INSERT GameTypes SELECT 'League Of Legends'
	WHERE NOT EXISTS (SELECT @count FROM dbo.GameTypes WHERE Title = 'League Of Legends')

INSERT GameTypes SELECT 'Rocket League'
	WHERE NOT EXISTS (SELECT @count FROM dbo.GameTypes WHERE Title = 'Rocket League')

