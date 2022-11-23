@echo off
REM Löscht alle temporären Visual Studio Dateien

FOR %%d IN (webapi) DO (
    rd /S /Q "%%d/bin" 2> nul 
    rd /S /Q "%%d/obj" 2> nul
    rd /S /Q "%%d/.vs" 2> nul
    rd /S /Q "%%d/.vscode" 2> nul
)

FOR %%d IN (spengernews) DO (
  rd /S /Q "%%d/node_modules" 2> nul
)

