set OUTPUT=src\artifacts\binaries

call rmdir /S /Q ".\src\artifacts"

call dotnet restore 

call dotnet build src\Commons.Utils -c Release --no-dependencies -o "%OUTPUT%\net40" -f net40 --no-incremental
call dotnet build src\Commons.Collections -c Release --no-dependencies -o "%OUTPUT%\net40" -f net40 --no-incremental
call dotnet build src\Commons.Json -c Release --no-dependencies -o "%OUTPUT%\net40" -f net40 --no-incremental
call dotnet build src\Commons.Pool -c Release --no-dependencies -o "%OUTPUT%\net40" -f net40 --no-incremental
call dotnet build src\Test.Commons -c Release --no-dependencies -o "%OUTPUT%\net40" -f net40 --no-incremental

call dotnet build src\Commons.Utils -c Release --no-dependencies -o "%OUTPUT%\net45" -f net45 --no-incremental
call dotnet build src\Commons.Collections -c Release --no-dependencies -o "%OUTPUT%\net45" -f net45 --no-incremental
call dotnet build src\Commons.Json -c Release --no-dependencies -o "%OUTPUT%\net45" -f net45 --no-incremental
call dotnet build src\Commons.Pool -c Release --no-dependencies -o "%OUTPUT%\net45" -f net45 --no-incremental
call dotnet build src\Test.Commons -c Release --no-dependencies -o "%OUTPUT%\net45" -f net45 --no-incremental

call dotnet build src\Commons.Utils -c Release --no-dependencies -o "%OUTPUT%\netstandard1.3" -f netstandard1.3 --no-incremental
call dotnet build src\Commons.Collections -c Release --no-dependencies -o "%OUTPUT%\netstandard1.3" -f netstandard1.3 --no-incremental
call dotnet build src\Commons.Json -c Release --no-dependencies -o "%OUTPUT%\netstandard1.3" -f netstandard1.3 --no-incremental
call dotnet build src\Commons.Pool -c Release --no-dependencies -o "%OUTPUT%\netstandard1.3" -f netstandard1.3 --no-incremental

call dotnet build src\Test.Commons -c Release --no-dependencies -o "%OUTPUT%\netcoreapp1.0" -f netcoreapp1.0 --no-incremental

call dotnet test src\Test.Commons -f netcoreapp1.0 -o "%OUTPUT%\netcoreapp1.0" --no-build
call %USERPROFILE%\.nuget\packages\xunit.runner.console\2.1.0\tools\xunit.console.exe %OUTPUT%\net40\Test.Commons.dll
call %USERPROFILE%\.nuget\packages\xunit.runner.console\2.1.0\tools\xunit.console.exe %OUTPUT%\net45\Test.Commons.dll

.\src\.nuget\nuget pack Commons.nuspec -outputdirectory %OUTPUT%
.\src\.nuget\nuget pack Commons.Json.nuspec -outputdirectory %OUTPUT%
.\src\.nuget\nuget pack Commons.Pool.nuspec -outputdirectory %OUTPUT%


call PAUSE
