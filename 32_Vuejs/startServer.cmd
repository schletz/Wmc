@echo off
FOR %%d IN (. Spengernews.Webapi Spengernews.Application) DO (
    rd /S /Q "%%d/bin" 2> nul 
    rd /S /Q "%%d/obj" 2> nul
    rd /S /Q "%%d/.vs" 2> nul
    rd /S /Q "%%d/.vscode" 2> nul
)
:start
dotnet watch run -c Debug --project Spengernews.Webapi
goto start

