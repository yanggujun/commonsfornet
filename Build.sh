export OUTPUT=$PWD/src/artifacts/rbin
export Artifacts=$PWD/src/artifacts
export src=$PWD/src if [ -d "$Artifacts" ]; then
    rm -rf $Artifacts
fi


dotnet --info

dotnet restore $src/Commons.Utils/Commons.Utils_NC.csproj
dotnet restore $src/Commons.Collections/Commons.Collections_NC.csproj
dotnet restore $src/Commons.Reflect/Commons.Reflect_NC.csproj
dotnet restore $src/Commons.Pool/Commons.Pool_NC.csproj
dotnet restore $src/Commons.Json/Commons.Json_NC.csproj
dotnet restore $src/Commons.Test/Commons.Test_NC.csproj
dotnet restore $src/Commons.Perf/Commons.Perf.csproj


dotnet build $src/Commons.Utils/Commons.Utils_NC.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Collections/Commons.Collections_NC.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Reflect/Commons.Reflect_NC.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Pool/Commons.Pool_NC.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Json/Commons.Json_NC.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src/Commons.Test/Commons.Test_NC.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netcoreapp3.1 --no-incremental 
dotnet build $src/Commons.Perf/Commons.Perf.csproj -c Release --no-dependencies -o "$OUTPUT/netstandard1.3" -f netcoreapp3.1 --no-incremental 

cd $OUTPUT/netstandard1.3
dotnet test $src/Commons.Test/Commons.Test_NC.csproj -o $OUTPUT/netstandard1.3 --no-build -f netcoreapp3.1 -p:ParallelizeTestCollections=false

dotnet Commons.Perf.dll
