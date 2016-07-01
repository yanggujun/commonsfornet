

set TestFramework=%1

If "%TestFramework%" == "netcoreapp1.0" (
    call pushd %OUTPUT%\netcoreapp1.0
    call dotnet test ..\..\..\Test.Commons -o .\ --no-build -f netcoreapp1.0 -parallel none 
    call popd
) ELSE (
    call "%USERPROFILE%\.nuget\packages\xunit.runner.console\2.1.0\tools\xunit.console.exe" %OUTPUT%\%TestFramework%\Test.Commons.dll -parallel none 
)

