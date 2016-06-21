#!/bin/sh

OUTPUT=./src/artifacts/rbin

rm -r ./src/artifacts

dotnet restore 

dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental
dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/net40" -f net40 --no-incremental

dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental
dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/net45" -f net45 --no-incremental

dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental

dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/netcoreapp1.0" -f netcoreapp1.0 --no-incremental

dotnet test src/Test.Commons -f netcoreapp1.0 -o "$OUTPUT/netcoreapp1.0" --no-build -parallel none
mono %USERPROFILE%/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console.exe $OUTPUT/net40/Test.Commons.dll
mono %USERPROFILE%/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console.exe $OUTPUT/net45/Test.Commons.dll

mono ./src/.nuget/nuget.exe pack Commons.nuspec -outputdirectory $OUTPUT
mono ./src/.nuget/nuget.exe pack Commons.Json.nuspec -outputdirectory $OUTPUT
mono ./src/.nuget/nuget.exe pack Commons.Pool.nuspec -outputdirectory $OUTPUT


PAUSE
