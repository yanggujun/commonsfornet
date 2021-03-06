$OUTPUT= Join-Path -Path $pwd -ChildPath "src\artifacts\rbin"
$Artifacts=Join-Path -Path $pwd -ChildPath "src\artifacts"
$src=Join-Path -Path $pwd -ChildPath "src"

if ( Test-Path -Path "$Artifacts" ) {
    ri $Artifacts -recurse -force
}

$StartDate = [datetime]::ParseExact("02/15/2015", "MM/dd/yyyy", [System.Globalization.CultureInfo]::InvariantCulture)
$Today = (Get-Date)
$BuildNo = ($Today - $StartDate).Days

$version = "0.4.0.$BuildNo"

$BuildInfo = "Building project Commons, version: {0}" -f $version

echo $BuildInfo

dotnet --info
dotnet restore $src\Commons.Utils\Commons.Utils_DNF.csproj
dotnet restore $src\Commons.Collections\Commons.Collections_DNF.csproj
dotnet restore $src\Commons.Reflect\Commons.Reflect_DNF.csproj
dotnet restore $src\Commons.Pool\Commons.Pool_DNF.csproj
dotnet restore $src\Commons.Json\Commons.Json_DNF.csproj
dotnet restore $src\Commons.Test\Commons.Test_DNF.csproj
dotnet restore $src\Commons.Perf\Commons.Perf.csproj

dotnet restore $src\Commons.Utils\Commons.Utils_NC.csproj
dotnet restore $src\Commons.Collections\Commons.Collections_NC.csproj
dotnet restore $src\Commons.Reflect\Commons.Reflect_NC.csproj
dotnet restore $src\Commons.Pool\Commons.Pool_NC.csproj
dotnet restore $src\Commons.Json\Commons.Json_NC.csproj
dotnet restore $src\Commons.Test\Commons.Test_NC.csproj
dotnet restore $src\Commons.Perf\Commons.Perf.csproj


dotnet build $src\Commons.Utils\Commons.Utils_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental 
dotnet build $src\Commons.Collections\Commons.Collections_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental 
dotnet build $src\Commons.Reflect\Commons.Reflect_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental 
dotnet build $src\Commons.Pool\Commons.Pool_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental 
dotnet build $src\Commons.Json\Commons.Json_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental 
dotnet build $src\Commons.Test\Commons.Test_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental 


dotnet build $src\Commons.Utils\Commons.Utils_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental 
dotnet build $src\Commons.Collections\Commons.Collections_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental 
dotnet build $src\Commons.Reflect\Commons.Reflect_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental 
dotnet build $src\Commons.Pool\Commons.Pool_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental 
dotnet build $src\Commons.Json\Commons.Json_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental 
dotnet build $src\Commons.Test\Commons.Test_DNF.csproj -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental 

dotnet build $src\Commons.Utils\Commons.Utils_NC.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src\Commons.Collections\Commons.Collections_NC.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src\Commons.Reflect\Commons.Reflect_NC.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src\Commons.Pool\Commons.Pool_NC.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src\Commons.Json\Commons.Json_NC.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental 
dotnet build $src\Commons.Test\Commons.Test_NC.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netcoreapp2.1 --no-incremental 
dotnet build $src\Commons.Perf\Commons.Perf.csproj -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netcoreapp2.1 --no-incremental 

pushd $OUTPUT\netstandard1.3
dotnet test $src\Commons.Test\Commons.Test_NC.csproj -o $OUTPUT\netstandard1.3 --no-build -f netcoreapp2.1 -p:ParallelizeTestCollections=false
popd


$xunit = "$env:userprofile\.nuget\packages\xunit.runner.console\2.1.0\tools\xunit.console.exe"


&$xunit "$OUTPUT\net40\Commons.Test.dll" -parallel none 
&$xunit "$OUTPUT\net45\Commons.Test.dll" -parallel none 

$nuget = "$src\.nuget\NuGet.exe"


&$nuget pack Commons.nuspec -outputdirectory $OUTPUT -version $version
&$nuget pack Commons.Json.nuspec -outputdirectory $OUTPUT -version $version
&$nuget pack Commons.Pool.nuspec -outputdirectory $OUTPUT -version $version

pushd $OUTPUT\netstandard1.3
dotnet Commons.Perf.dll
popd
