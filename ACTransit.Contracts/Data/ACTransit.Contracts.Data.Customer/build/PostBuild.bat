rem $(ProjectDir)build\PostBuild.bat $(ConfigurationName) $(SolutionDir) $(ProjectName) $(ProjectPath) $(TargetDir) $(ProjectDir)

set ConfigurationName=%1
set SolutionDir=%2
set ProjectName=%3
set ProjectPath=%4
set TargetDir=%5
set ProjectDir=%6

set regex=%ProjectDir%build\regex.exe

set contract.Main=%TargetDir%ACTransit.Contracts.Data.Customer.XML
set contract.Mobile=%TargetDir%ACTransit.Contracts.Data.Customer.Mobile.XML
set contract.Mobile.pattern1=%ProjectDir%build\ACTransit.Contracts.Data.Regex.01.txt
set contract.Mobile.replace1=

echo ConfigurationName: %ConfigurationName%
echo SolutionDir: %SolutionDir%
echo ProjectName: %ProjectName%
echo ProjectPath: %ProjectPath%
echo TargetDir: %TargetDir%
echo ProjectDir: %ProjectDir%

:XMLDOC
copy %contract.Main% %contract.Mobile%
%regex% %contract.Mobile% %contract.Mobile.pattern1% %contract.Mobile.replace1%

:PUBLISH
echo before Publishing..
if not "%ConfigurationName%" == "Nuget" goto DONE
echo Publishing..
%SolutionDir%.nuget\publish.bat "%ProjectName%" "%ProjectPath%"
goto DONE


:DONE
Echo DONE.