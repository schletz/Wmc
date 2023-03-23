@echo off
chcp 65001
echo Warning: All local changes will be reset. Press CTRL+C to cancel.
pause

REM Get the current branch
FOR /F "tokens=*" %%a IN ('git branch --show-current') DO (SET current_branch=%%a)

REM Check if there are any uncommitted changes
SET has_changes=0
FOR /F "tokens=* delims=* " %%a IN ('git status --porcelain') DO (
    SET has_changes=1
)
IF %has_changes% == 1 (
    echo You have uncommitted changes. Please commit or stash them before running this script.
    exit /b 1
)

REM Reset all branches
FOR /F "tokens=*" %%a IN ('git branch') DO (
    echo Resetting branch %%a...
    git checkout %%a && git clean -df && git reset --hard origin/%%a || (
        echo Failed to reset branch %%a.
        exit /b 1
    )
)

REM Go back to the current branch
git checkout %current_branch%
echo You are now on branch %current_branch%
