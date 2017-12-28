USE master;
GO

-------------------------------------------------------------------------------
-- Enable CLR on the Server

sp_configure 'show advanced options', 1;  
GO  
RECONFIGURE;  
GO  
sp_configure 'clr enabled', 1;  
GO  
RECONFIGURE;  
GO 

----------------------------------------------------------------------------------------------------
-- Create strong signing key (assumes P:\ACTransit.Projects is your TFS $/ACTransit.Projects folder)

CREATE ASYMMETRIC KEY CLRBuildInfoKey
FROM FILE = 'P:\ACTransit.Projects\trunk\ACTransit.CLR\FileUtility\FileUtility2016\strongKey.snk'
ENCRYPTION BY PASSWORD = 'SomePasswordHere123'
GO

CREATE LOGIN CLRBuildInfoLogin FROM ASYMMETRIC KEY CLRBuildInfoKey;
GRANT UNSAFE ASSEMBLY TO CLRBuildInfoLogin; 
GO

-------------------------------------------------------------------------------
-- Enable TrustWorthy on the database or create a key and login for the DLL

USE GTFS;
GO
ALTER DATABASE GTFS SET TRUSTWORTHY ON;
GO
CREATE USER CLRBuildInfo FOR LOGIN CLRBuildInfoLogin
GO

-------------------------------------------------------------------------------
-- Add the assemblies

IF OBJECT_ID('.dbo.UtilityCreateEmptyFile') IS NOT NULL DROP PROCEDURE dbo.UtilityCreateEmptyFile
GO
IF OBJECT_ID('.dbo.UtilityCreateDirectory') IS NOT NULL DROP PROCEDURE dbo.UtilityCreateDirectory
GO
IF OBJECT_ID('.dbo.UtilityCreateZipFile') IS NOT NULL DROP PROCEDURE dbo.UtilityCreateZipFile
GO
IF OBJECT_ID('.dbo.UtilityCopyFile') IS NOT NULL DROP PROCEDURE dbo.UtilityCopyFile
GO
IF OBJECT_ID('.dbo.UtilityDeleteFile') IS NOT NULL DROP PROCEDURE dbo.UtilityDeleteFile
GO
IF OBJECT_ID('.dbo.UtilityDeleteDirectory') IS NOT NULL DROP PROCEDURE dbo.UtilityDeleteDirectory
GO
IF OBJECT_ID('.dbo.UtilityRenameDirectory') IS NOT NULL DROP PROCEDURE dbo.UtilityRenameDirectory
GO
IF OBJECT_ID('.dbo.UtilityRenameFile') IS NOT NULL DROP PROCEDURE dbo.UtilityRenameFile
GO
IF OBJECT_ID('.dbo.UtilityUnzipFile') IS NOT NULL DROP PROCEDURE dbo.UtilityUnzipFile
GO
IF OBJECT_ID('.dbo.UtilityUnzipFileToPath') IS NOT NULL DROP PROCEDURE dbo.UtilityUnzipFileToPath
GO

DROP ASSEMBLY IF EXISTS [FileUtilityAssembly]
GO
DROP ASSEMBLY IF EXISTS [System.IO.Compression.ZipFile]
GO
DROP ASSEMBLY IF EXISTS [System.IO.Compression.FileSystem]
GO
DROP ASSEMBLY IF EXISTS [System.IO.Compression]
GO

CREATE ASSEMBLY [System.IO.Compression]  
AUTHORIZATION [CLRBuildInfo] 
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IO.Compression.dll' -- might need to change paths with future .NET framework upgrades (e.g. 5.0)
WITH PERMISSION_SET = UNSAFE; 
GO 

CREATE ASSEMBLY [System.IO.Compression.FileSystem]
AUTHORIZATION [CLRBuildInfo] 
FROM N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IO.Compression.FileSystem.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE ASSEMBLY FileUtilityAssembly
AUTHORIZATION [CLRBuildInfo] 
FROM 'P:\ACTransit.Projects\trunk\ACTransit.CLR\FileUtility\FileUtility2016\bin\Debug\FileUtility.dll'  -- NEED TO COMPILE FIRST
WITH PERMISSION_SET = UNSAFE 
GO

-------------------------------------------------------------------------------
-- Create the procedures to execute the assemblies


CREATE PROCEDURE dbo.UtilityCreateDirectory
(
	 @Directory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCreateDirectory
				@Directory = N'C:\test-directory'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CreateDirectory
GO



CREATE PROCEDURE dbo.UtilityCreateEmptyFile
(
	 @File NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCreateEmptyFile
				@file = N'C:\test-directory\test.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CreateEmptyFile
GO



CREATE PROCEDURE dbo.UtilityCreateZipFile
(
	 @SourceDirectoryName NVARCHAR(MAX)
	,@DestinationArchiveFileName NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCreateZipFile
				@SourceDirectoryName = N'C:\test-directory'
			   ,@DestinationArchiveFileName = N'C:\test-directory\test.zip'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CreateZipFile
GO



CREATE PROCEDURE dbo.UtilityCopyFile
(
	 @SourceFile NVARCHAR(MAX)
	,@DestinationFile NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityCopyFile
				@SourceFile = N'C:\test-directory\test.txt'
			   ,@DestinationFile = N'C:\test-directory\test-copy.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].CopyFile
GO





CREATE PROCEDURE dbo.UtilityRenameFile
(
	 @SourceFile NVARCHAR(MAX),
	 @DestinationFile NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityRenameFile 
			@SourceFile = N'C:\test-directory\test-copy.txt'
		   ,@DestinationFile = N'C:\test-directory\test-renamed.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].RenameFile
GO



CREATE PROCEDURE dbo.UtilityUnzipFile
(
	 @ZipFile NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityUnzipFile 
			@ZipFile = N'C:\test-directory\test.zip'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].Unzip
GO



CREATE PROCEDURE dbo.UtilityUnzipFileToPath
(
	 @ZipFile NVARCHAR(MAX)
	,@ExtractDirectory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityUnzipFileToPath 
			@ZipFile = N'C:\test-directory\test.zip', 
			@ExtractDirectory = N'C:\test-directory\test-extract'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].UnzipToPath
GO



CREATE PROCEDURE dbo.UtilityDeleteFile
(
	 @File NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:
		
		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityDeleteFile 
			@File = N'C:\test-directory\test.txt'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].DeleteFile
GO





CREATE PROCEDURE dbo.UtilityRenameDirectory
(
	 @SourceDirectory NVARCHAR(MAX)
    ,@DestinationDirectory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityRenameDirectory 
			@SourceDirectory = N'C:\test-directory'
		   ,@DestinationDirectory = N'C:\test-directory-renamed'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].RenameDirectory
GO



CREATE PROCEDURE dbo.UtilityDeleteDirectory
(
	 @Directory NVARCHAR(MAX)
)
AS
	/* Sample Db Exec:

		DECLARE @Result INT
		EXEC @Result = GTFS.dbo.UtilityDeleteDirectory 
			@Directory = N'C:\test-directory-renamed'
		PRINT @Result
	*/
EXTERNAL NAME FileUtilityAssembly.[FileUtility.FileUtility].DeleteDirectory
GO

