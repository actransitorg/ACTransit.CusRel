function Get-Profile {
    $sourceCodeRoot = "P:\ACTransit.Projects\trunk\ACTransit.CusRel\"
    $targetPath = "C:\github\ACTransit.CusRel-temp"
    $gitRepositoryName = "ACTransit.CusRel"
    $gitRepositoryUrl = "https://github.com/actransitorg/ACTransit.CusRel.git"
    $scriptPath = $($pwd.Path)
    $scriptName = "CusRel"
	#$repoPath = [System.IO.Path]::GetFullPath("$scriptPath\..")
    $databaseFilesPath = "$scriptPath\Database"
    $solutionPath = [System.IO.Path]::GetFullPath("$scriptPath\..")
    $startupPath = "$solutionPath\ACTransit.CusRel.Website"
    $appDataPath = "$startupPath\App_Data"
    #$sqlExecutePath = (Get-ChildItem -Path "$repoPath\*SQLExecute.exe" -Recurse | Sort-Object LastAccessTime -Descending | Select-Object -First 1).FullName
    $targetCodeRoot = "$targetPath\$scriptName"
    $publishScripts = "$targetCodeRoot\PublishScripts"
    $sourceCodeDirectories = @(
		"..\ACTransit.Contracts\Data\ACTransit.Contracts.Data.Common",
		"..\ACTransit.Contracts\Data\ACTransit.Contracts.Data.CusRel",
		"..\ACTransit.Contracts\Data\ACTransit.Contracts.Data.Customer",
		"..\ACTransit.Contracts\Data\ACTransit.Contracts.Data.Schedules",
		"..\ACTransit.Framework\ACTransit.Framework",
		"..\ACTransit.Framework\ACTransit.Framework.DataAccess",
		"..\ACTransit.Framework\ACTransit.Framework.Logging",
		"..\ACTransit.Framework\ACTransit.Framework.Web",
		"..\ACTransit.Entities\Entities.CustomerRelations",
        "..\ACTransit.Entities\Entities.Employee",
        "..\ACTransit.Entities\Entities.MapsSchedules",
        "..\ACTransit.Entities\Entities.Scheduling",
        "..\ACTransit.Entities\Entities.Scheduling.Gtfs",
        "..\ACTransit.Entities\Entities.Transportation",
		"..\ACTransit.Entities\DataAccess.CustomerRelations",
        "..\ACTransit.Entities\DataAccess.Employee",
        "..\ACTransit.Entities\DataAccess.MapsSchedules",
        "..\ACTransit.Entities\DataAccess.Scheduling",
        "..\ACTransit.Entities\DataAccess.Scheduling.Gtfs",
        "..\ACTransit.Entities\DataAccess.Transportation",
		"ACTransit.CusRel.Repositories",
		"ACTransit.CusRel.Services",
		"ACTransit.CusRel.Tests",
		"ACTransit.CusRel.ServiceHost",
        "ACTransit.CusRel.Website",
        "ACTransit.CusRel.Public.API",
        "..\ACTransit.CLR\FileUtility\FileUtility2016",
        ("..\..\Github_trunk\.nuget", ".nuget"),
		("..\..\Github_trunk\docs", "docs"),
		("..\..\Github_trunk\SSRS", "SSRS"),
		("..\..\Github_trunk\PublishScripts", "PublishScripts")
    )
    $sourceCodeFiles = 
        @("ACTransit.CusRel.sln",
          ".tfignore",
          ("..\..\Github_trunk\LICENSE.MD", "..\LICENSE.MD"),
          ("README.MD", "..\README.MD")
    )
    $targetRemove = 
        @("*.vspscc","*.vssscc", "*.suo", "*.user", ".vs", "obj", "bin", "Debug", "Nuget", "Release", "packages", ".gradle", ".idea", ".vscode",
            "Training.docx", "SQlExecute.exe.config", "*.pubxml", "Web.Release.config", "Training")
            
    $result = @{ `
        ScriptPath = $scriptPath
        ScriptName = $scriptName
        #SqlExecutePath = $sqlExecutePath
        DatabaseFilesPath = $databaseFilesPath
        SolutionPath = $solutionPath
        StartupPath = $startupPath
        AppDataPath = $appDataPath		
        DbCount = 6
        TestMode = "false"
        PathInDbName = $false
        SourceCodeRoot = $sourceCodeRoot
        TargetPath = $targetPath
        TargetCodeRoot = $targetCodeRoot
        PublishScripts = $publishScripts
        SourceCodeDirectories = $sourceCodeDirectories
        SourceCodeFiles = $sourceCodeFiles
        TargetRemove = $targetRemove
        SearchReplace = @()
        Databases = @("EmployeeDW","PublicSchedule","SchedulingDW","GTFS","TransportationDW","CusRel")  # order matters here
        GitRepositoryName = $gitRepositoryName
        GitRepositoryUrl = $gitRepositoryUrl
        Finalize = $null
    } |  ConvertTo-Json |  ConvertFrom-Json 
    $result.SearchReplace = (Get-Content "profiles\$scriptName.json") -join "`n" | ConvertFrom-Json
    $result.Finalize = {
        # Clean unused database files
        Get-ChildItem | Where-Object { $_.Name -notmatch '^CusRel_DDL_DML.sql|EmployeeDW_DDL_DML.sql|PublicSchedule_DDL_DML.sql|SchedulingDW_DDL_DML.sql|GTFS_DDL_DML.sql|TransportationDW_DDL_DML.sql$' } | Remove-Item        
    }
    $result
}

Get-Profile