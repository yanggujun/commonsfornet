export testFramework=$1

if [ $testFramework = netcoreapp1.0 ]; then
    cd $OUTPUT/netcoreapp1.0
    dotnet test ../../../Commons.Test -o ./ --no-build -f netcoreapp1.0 -parallel none -method $2 -verbose
    cd ../../../../
else
    mono "$HOME/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console.exe" $OUTPUT/$testFramework/Commons.Test.dll -parallel none -method $2 -verbose
fi
