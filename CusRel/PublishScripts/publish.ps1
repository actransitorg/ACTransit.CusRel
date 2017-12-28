[CmdletBinding()]
param (
    [string]$drive = "E:", #$(throw "-FilePath is required."),    
    [string]$serviceName="SynProcessServer",
    [switch]$force,
    [Parameter(Mandatory=$true)]
    [string]$profile
)

Import-Module SQLPS

function Setup-MSSQLLocalDb {
	 $result = & $sqlLocalDb info MSSQLLocalDB
	 $notFound = $result[0].Contains('is not created.')
	 if ($notFound) {
		Write-Debug "* Creating MSSQLLocalDb"
		& $sqlLocalDb create
	 }
}

function Restart-SqlLocalDb {
	& $sqlLocalDb stop
	& $sqlLocalDb start
}

# function Invoke-SQL {
#     param(
#         [string] $dataSource = "(localdb)\MSSQLLocalDB",
#         [string] $database = "master",
#         [string] $sqlCommand = $(throw "Please specify a query.")
#       )

#     $connectionString = "Data Source=$dataSource;Integrated Security=SSPI;Initial Catalog=$database"
#     $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
#     $command = new-object System.Data.SqlClient.SqlCommand($sqlCommand, $connection)
#     $connection.Open()
# 	$command.ExecuteNonQuery() 
# 	If (!$?) { throw [System.IO.Exception] "Error exeuting $sqlCommand" }
#     $connection.Close()
# }

function Create-Databases {
	# drop all databases first in case of inter-database references
	ForEach ($db in $config.Databases) {
		$mdf = "$appDataPath\$db.mdf"
		$ldf = "$appDataPath\$db`_log.ldf"
		$script = "$currentPath\Database\$db`_DDL_DML.sql"
		$dbpath = if ($pathInDbName -eq $true) { $mdf } else { $db }
		$dbfilename = "$appDataPath\$db"
		$cmd = "
		IF EXISTS (SELECT * FROM master.sys.databases WHERE [name] = `'$dbfilename`') 
		BEGIN 
			ALTER DATABASE [$dbfilename] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
			ALTER DATABASE [$dbfilename] SET MULTI_USER WITH ROLLBACK IMMEDIATE;
			DROP DATABASE [$dbfilename]
		END
		IF EXISTS (SELECT * FROM master.sys.databases WHERE [name] = `'$db`') 
		BEGIN 
			ALTER DATABASE [$db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
			ALTER DATABASE [$db] SET MULTI_USER WITH ROLLBACK IMMEDIATE;
			DROP DATABASE [$db]
		END
		IF EXISTS (SELECT * FROM master.sys.databases WHERE [name] = `'$mdf`') 
		BEGIN 
			ALTER DATABASE [$mdf] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
			ALTER DATABASE [$mdf] SET MULTI_USER WITH ROLLBACK IMMEDIATE;
			DROP DATABASE [$mdf]
		END
		"
		Invoke-SqlCmd -Query $cmd -serverinstance "(localdb)\MSSQLLocalDB" -database "master"
		if (Test-Path $mdf) {
			Write-Debug "* Force removing $mdf and $ldf"
			Remove-Item "$mdf"
			Remove-Item "$ldf"
		}
	}
	ForEach ($db in $config.Databases) {
		$mdf = "$appDataPath\$db.mdf"
		$ldf = "$appDataPath\$db`_log.ldf"
		$script = "$currentPath\Database\$db`_DDL_DML.sql"
		$dbpath = if ($pathInDbName -eq $true) { $mdf } else { $db }
		$dbfilename = "$appDataPath\$db"
		$cmd = "
		CREATE DATABASE [$dbpath] ON PRIMARY
			( NAME = N`'$db`', FILENAME = N`'$mdf`' , SIZE = 1024KB , FILEGROWTH = 1024KB )
		LOG ON 
			( NAME = N`'$db`_log`', FILENAME = N`'$ldf`' , SIZE = 1024KB , FILEGROWTH = 1024KB )"
		Write-Debug "* Creating $db..."
		Invoke-SqlCmd -Query $cmd -serverinstance "(localdb)\MSSQLLocalDB" -database "master"
		Write-Debug "* Running $script"		
		Invoke-SqlCmd -InputFile $script -serverinstance "(localdb)\MSSQLLocalDB" -database $db
	}
}

function Publish-Required {                  
    if (!(Test-Path $appDataPath)){
        throw "Database path not found! $appDataPath"            
    }                
    if (!(Test-Path $publishedPath)){
		Write-Debug "* Could not find $publishedPath"
        return $true
    }                
    $content = Get-Content $publishedPath                
    if (!($content -eq $appDataPath)){
		Write-Debug "* Zero sized $publishedPath"
        return $true
    }
	$files=Get-ChildItem "$appDataPath\*.mdf" -ErrorAction Stop #| Measure-Object
    if ($files -eq $NULL) {
		Write-Debug "* No database files found"
	} 
	
	if (-not ($files.Length -eq $dbCount)) {
		Write-Debug "* Database actual count: $files.Length"
        return $true
    }
    return $false
}

function Get-CurrentPath {
	return "$(Split-Path $PSCommandPath -Parent)";
}

function Get-SqlPath {
	$SqlPath = "$($env:ProgramW6432)\Microsoft SQL Server"
	if (!(test-path $SqlPath)) { $SqlPath = "$($env:ProgramFiles)\Microsoft SQL Server" }
	if (!(test-path $SqlPath)) { throw [Exception] "Could not find SQL Server installation" }
	return $SqlPath;
}

function Get-SqlLocalDb {
	$ErrorActionPreference= "SilentlyContinue"
	$sqlLocalDb = (Get-ChildItem -Path $SqlPath -Filter SqlLocalDb.exe -Recurse | Sort-Object LastAccessTime -Descending | Select-Object -First 1).FullName
	$ErrorActionPreference = "Stop";
	if ($sqlLocalDb -eq $null) { throw [Exception] "Could not find SqlLocalDb.exe" }
	return $sqlLocalDb;
}

function Get-Config {
    $configFile = "$currentPath\profiles\$profile.ps1"
	if (!(test-path $configFile)) { throw [Exception] "Cannot find $configFile" }
	return gc $configFile | Out-String | iex
}

$DebugPreference = "Continue"
Clear-Host
Write-Debug "##################################################################" 
Try
{
    $currentPath = Get-CurrentPath
    Set-Location $currentPath
	Write-Debug "* Current Path: $currentPath" 
	$SqlPath = Get-SqlPath
	Write-Debug "* Sql Path: $SqlPath"
	$sqlLocalDb = Get-SqlLocalDb
	Write-Debug "* SqlLocalDb Path: $sqlLocalDb"
    $config = Get-Config
}
Catch
{
	write-host "Caught an exception, rolling back." -ForegroundColor Red
	write-host "Exception Type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
	write-host "Exception Message: $($_.Exception.Message)" -ForegroundColor Red
    return -2
}
$scriptPath = "$($config.ScriptPath)"
Write-Debug "* Script Path: $scriptPath"
$dbCount = [int]$($config.DbCount)
Write-Debug "* Database count: $dbCount"
$databaseFilesPath = "$($config.DatabaseFilesPath)"
Write-Debug "* Database Files Path: $databaseFilesPath"
$appDataPath = "$($config.AppDataPath)"
Write-Debug "* App_Data Path: $appDataPath"
$publishedPath = [System.IO.Path]::GetFullPath("$appDataPath\published.txt")
Write-Debug "* Published Path: $publishedPath"
$pathInDbName = "$($config.PathInDbName)"
Write-Debug "* Path In DbName: $pathInDbName"
$testMode = "$($config.TestMode)"
Write-Debug "* Test Mode: $testMode"
	  
if ($force) {
	Write-Debug "* Forcing publish by deleting $publishedPath"
	Remove-Item $publishedPath
}

if (Publish-Required) {
    Try
    {
		Write-Debug "* Publish required"
		# $archiveDbPath = "$appDataPath\Archive_" + (get-date -Format "yyyyMMdd_HHmmss")  +"\"
		# if (!(Test-Path $archiveDbPath)){
		# 	New-Item -ItemType directory $archiveDbPath
		# }
		# "* Archiving into $archiveDbPath"
		
		Setup-MSSQLLocalDb 
		Restart-SqlLocalDb
		# Move-Item "$appDataPath\*.*" $archiveDbPath
		# Copy-Item "$databaseFilesPath\*.mdf" $appDataPath
		# Copy-Item "$databaseFilesPath\*.ldf" $appDataPath
		# Set-ItemProperty "$appDataPath\*.*" -name IsReadOnly -value $false
		Write-Debug "* Creating $publishedPath"
		$appDataPath | out-file $publishedPath
		Create-Databases
		Push-Location

		#$files = Get-ChildItem "$scriptPath\*.sql" -ErrorAction Stop #| Measure-Object
        #$files | ForEach-Object -ErrorAction SilentlyContinue { 
        #    "-----------------------------------------------------------------------"
		#	"$($config["SqlExecutePath"]) --testmode=$testMode --scriptdir=$scriptPath --datadir=$appDataPath\ --currentdir=$appDataPath $_"
		#	& $($config["SqlExecutePath"]) --testmode=$testMode --scriptdir=$scriptPath --datadir=$appDataPath\ --currentdir=$appDataPath $_
		#	if ($LASTEXITCODE -ne 0){
		#		throw [System.IO.Exception] "sqlexecute failed with error code $LASTEXITCODE."
		#	}
        #}
    }
    Catch
    {
		write-host "Caught an exception, rolling back." -ForegroundColor Red
		write-host "Exception Type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
		write-host "Exception Message: $($_.Exception.Message)" -ForegroundColor Red

		Remove-Item $publishedPath
		Remove-Item "$appDataPath\*.*" | Where { ! $_.PSIsContainer }
		Copy-Item "$archiveDbPath\*.*" $appDataPath
		Remove-Item $archiveDbPath -Recurse
    }
	Pop-Location
    return               
}
else {
	Write-Debug "Publish is not required"
}
    

