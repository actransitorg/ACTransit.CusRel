
set dotnetVer=NET462
set SolutionDir=%1
set TargetDir=%2
set ContractsDir=%SolutionDir%..\ACTransit.Contracts\Data\
echo SolutionDir=%SolutionDir%
echo TargetDir=%TargetDir%
echo ContractsDir=%ContractsDir%

dir %ContractsDir%ACTransit.Contracts.Data*.XML /b /s

FOR /F %%i IN ('dir %ContractsDir%ACTransit.Contracts.Data*.XML /b /s') DO (
	copy %%i %TargetDir%
)
del %TargetDir%ACTransit.CusRel.Public.API.dll.CodeAnalysisLog.xml
exit 0