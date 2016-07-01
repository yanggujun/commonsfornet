
export framework=$1
export testFramework=$1

if [ $framework = 'netstandard1.3' ]; then
    export testFramework=netcoreapp1.0
fi


echo 'output'
echo $OUTPUT

dotnet build src/Commons.Utils -c Release --no-dependencies -o "$OUTPUT/$framework" -f $framework --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Collections -c Release --no-dependencies -o "$OUTPUT/$framework" -f $framework --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Json -c Release --no-dependencies -o "$OUTPUT/$framework" -f $framework --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Commons.Pool -c Release --no-dependencies -o "$OUTPUT/$framework" -f $framework --no-incremental --version-suffix $BuildNo --build-profile
dotnet build src/Test.Commons -c Release --no-dependencies -o "$OUTPUT/$testFramework" -f $testFramework --no-incremental --version-suffix $BuildNo --build-profile

