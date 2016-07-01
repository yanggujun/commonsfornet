call .\tools\_env.bat

call rmdir /S /Q ".\src\artifacts"

call dotnet --info

call dotnet restore

call .\tools\_build.bat net40
call .\tools\_build.bat net45
call .\tools\_build.bat netstandard1.3

call .\tools\_test.bat net40
call .\tools\_test.bat net45
call .\tools\_test.bat netcoreapp1.0

call .\tools\_pack.bat

