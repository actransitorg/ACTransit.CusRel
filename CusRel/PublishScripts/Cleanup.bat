@echo off
cd ..
del /s /a /f *.vspscc 2>nul
del /s /a /f *.vssscc 2>nul
del /s /a /f *.suo 2>nul
del /s /a /f *.user 2>nul
for /D /R %%D in (*) do rd /s/q %%D\.vs 2>nul
for /D /R %%D in (*) do rd /s/q %%D\obj 2>nul
for /D /R %%D in (*) do rd /s/q %%D\bin 2>nul
for /D /R %%D in (*) do rd /s/q %%D\Debug 2>nul
for /D /R %%D in (*) do rd /s/q %%D\Nuget 2>nul
for /D /R %%D in (*) do rd /s/q %%D\Release 2>nul
for /D /R %%D in (*) do rd /s/q %%D\packages 2>nul
for /D /R %%D in (*) do del /f/s/q %%D\App_Data 2>nul
for /D /R %%D in (App_Data\*) do rd /s/q %%D 2>nul
cd PublishScripts