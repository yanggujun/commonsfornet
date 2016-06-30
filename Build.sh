export OUTPUT=src/artifacts/rbin

export BuildNo=501

rm -r ./src/artifacts

dotnet --info

dotnet restore

dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental --version-suffix $BuildNo --build-profile


dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental --version-suffix $BuildNo --build-profile

dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental --version-suffix $BuildNo --build-profile


dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/netcoreapp1.0" -f netcoreapp1.0 --no-incremental --version-suffix $BuildNo --build-profile

cd $OUTPUT/netcoreapp1.0
dotnet test ../../../Test.Commons -o ./ --no-build -f netcoreapp1.0 -parallel none 
cd ../../../../

mono "$HOME/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console.exe" $OUTPUT/net40/Test.Commons.dll -parallel none 
mono "$HOME/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console.exe" $OUTPUT/net45/Test.Commons.dll -parallel none 
