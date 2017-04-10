export OUTPUT=$PWD/src/artifacts/rbin
export Artifacts=$PWD/src/artifacts
export src=$PWD/src

if [ -d "$Artifacts" ]; then
    rm -rf $Artifacts
fi


dotnet --info

dotnet restore $src/Commons.Utils/Commons.Utils.csproj
dotnet restore $src/Commons.Collections/Commons.Collections.csproj
dotnet restore $src/Commons.Reflect/Commons.Reflect.csproj
dotnet restore $src/Commons.Pool/Commons.Pool.csproj
dotnet restore $src/Commons.Json/Commons.Json.csproj
dotnet restore $src/Commons.Messaging/Commons.Messaging.csproj
dotnet restore $src/Commons.Test/Commons.Test.csproj
dotnet restore $src/Commons.Perf/Commons.Perf.csproj


dotnet build $src/Commons.Utils/Commons.Utils.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Collections/Commons.Collections.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Reflect/Commons.Reflect.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Pool/Commons.Pool.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Json/Commons.Json.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Messaging/Commons.Messaging.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Test/Commons.Test.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netcoreapp1.0 --no-incremental 
dotnet build $src/Commons.Perf/Commons.Perf.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netcoreapp1.0 --no-incremental 

cd $OUTPUT/netstandard1.3
dotnet test $src/Commons.Test/Commons.Test.csproj -o $OUTPUT/netstandard1.3 --no-build -f netcoreapp1.0

dotnet Commons.Perf.dll
