export testFramework=$1

if [ $testFramework = netcoreapp1.0 ]; then
    cd $OUTPUT/netcoreapp1.0
    dotnet test ../../../Test.Commons -o ./ --no-build -f netcoreapp1.0 -parallel none 
    cd ../../../../
else
    mono "$HOME/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console.exe" $OUTPUT/$testFramework/Test.Commons.dll -parallel none 
fi
