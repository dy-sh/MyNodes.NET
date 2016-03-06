
@echo off
SET DNX_FOLDER=dnx-clr-win-x86.1.0.0-rc1-update1
SET "LOCAL_DNX=%~dp0approot\runtimes\%DNX_FOLDER%\bin\dnx.exe"

IF EXIST %LOCAL_DNX% (
  SET "DNX_PATH=%LOCAL_DNX%"
)

for %%a in (%DNX_HOME%) do (
    IF EXIST %%a\runtimes\%DNX_FOLDER%\bin\dnx.exe (
        SET "HOME_DNX=%%a\runtimes\%DNX_FOLDER%\bin\dnx.exe"
        goto :continue
    )
)

:continue

IF "%HOME_DNX%" NEQ "" (
  SET "DNX_PATH=%HOME_DNX%"
)

IF "%DNX_PATH%" == "" (
  SET "DNX_PATH=dnx.exe"
)

@"%DNX_PATH%" --project "%~dp0approot\src\WebController" --configuration Release web %*

pause