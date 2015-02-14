set PATH=%PATH%;.\src\.nuget
call "%VS120COMNTOOLS%\vsvars32.bat"
call msbuild src\Commons.sln /t:Rebuild /p:Configuration=Release;TragetFrameworkVersion=4.5.1
call sn -R .\src\bin\Commons.Utils.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Collections.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Json.dll .\src\FullKey.snk
