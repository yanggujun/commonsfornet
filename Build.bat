set PATH=%PATH%;.\src\.nuget
call "%VS120COMNTOOLS%\vsvars32.bat"
call rmdir /S /Q ".\src\bin"
call msbuild src\Commons.sln /t:Rebuild /p:Configuration=Release;TragetFrameworkVersion=4.5
call sn -R .\src\bin\Commons.Utils.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Collections.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Json.dll .\src\FullKey.snk
call nuget pack Commons.nuspec -OutputDirectory ".\src\bin"
call PAUSE
