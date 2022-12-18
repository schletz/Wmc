@echo off
REM docker start mariadb 2> null
REM if errorlevel 1 docker run --name mariadb -d -p 13306:3306 -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=mariadb_root_password mariadb:10.10.2

FOR %%d IN (. Spengernews.Webapi Spengernews.Application) DO (
    rd /S /Q "%%d/bin" 2> nul 
    rd /S /Q "%%d/obj" 2> nul
    rd /S /Q "%%d/.vs" 2> nul
    rd /S /Q "%%d/.vscode" 2> nul
)
cd Spengernews.Webapi
:start
dotnet watch run -c Debug
goto start

