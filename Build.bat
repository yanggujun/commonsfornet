set PATH=%PATH%;.\src\.nuget
call "%VS120COMNTOOLS%\vsvars32.bat"
call "%VS140COMNTOOLS%\vsvars32.bat"
call rmdir /S /Q ".\src\bin"

call msbuild src\Commons.sln /t:Rebuild /p:Configuration=Release;Platform="Any CPU";Framework=NET45
call sn -R .\src\bin\NET45\Commons.Utils.dll .\src\FullKey.snk
call sn -R .\src\bin\NET45\Commons.Collections.dll .\src\FullKey.snk
call sn -R .\src\bin\NET45\Commons.Json.dll .\src\FullKey.snk
call sn -R .\src\bin\NET45\Commons.Pool.dll .\src\FullKey.snk

call msbuild src\Commons.sln /t:Rebuild /p:Configuration=Release;Platform="Any CPU";Framework=NET40
call sn -R .\src\bin\NET40\Commons.Utils.dll .\src\FullKey.snk
call sn -R .\src\bin\NET40\Commons.Collections.dll .\src\FullKey.snk
call sn -R .\src\bin\NET40\Commons.Json.dll .\src\FullKey.snk
call sn -R .\src\bin\NET40\Commons.Pool.dll .\src\FullKey.snk

call src\packages\xunit.runner.console.2.1.0\tools\xunit.console.exe src\bin\NET45\Test.Commons.dll 
call src\packages\xunit.runner.console.2.1.0\tools\xunit.console.exe src\bin\NET40\Test.Commons.dll

call nuget pack Commons.nuspec -OutputDirectory ".\src\bin"
call nuget pack Commons.Pool.nuspec -OutputDirectory ".\src\bin"
call nuget pack Commons.Json.nuspec -OutputDirectory ".\src\bin"
call PAUSE
