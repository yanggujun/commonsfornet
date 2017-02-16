$OUTPUT=".\src\artifacts\rbin"
$Artifacts=".\src\artifacts"

if ( Test-Path -Path "$Artifacts" ) {
    ri $Artifacts -recurse -force
}

$StartDate = [datetime]::ParseExact("02/15/2015", "MM/dd/yyyy", [System.Globalization.CultureInfo]::InvariantCulture)
$Today = (Get-Date)
$BuildNo = ($Today - $StartDate).Days

$version = "0.2.4.$BuildNo"

$BuildInfo = "Building project Commons, version: {0}" -f $version

echo $BuildInfo

dotnet --info
dotnet restore

dotnet build .\src\Commons.Utils -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental --build-profile
dotnet build .\src\Commons.Collections -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental --build-profile
dotnet build .\src\Commons.Pool -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental --build-profile
dotnet build .\src\Commons.Json -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental --build-profile
dotnet build .\src\Commons.Test -c Release --no-dependencies -o "$OUTPUT\net40" -f net40 --no-incremental --build-profile


dotnet build .\src\Commons.Utils -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental --build-profile
dotnet build .\src\Commons.Collections -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental --build-profile
dotnet build .\src\Commons.Pool -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental --build-profile
dotnet build .\src\Commons.Json -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental --build-profile
dotnet build .\src\Commons.Test -c Release --no-dependencies -o "$OUTPUT\net45" -f net45 --no-incremental --build-profile

dotnet build .\src\Commons.Utils -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental --build-profile
dotnet build .\src\Commons.Collections -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental --build-profile
dotnet build .\src\Commons.Pool -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental --build-profile
dotnet build .\src\Commons.Json -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental --build-profile
dotnet build .\src\Commons.Messaging -c Release --no-dependencies -o "$OUTPUT\netstandard1.3" -f netstandard1.3 --no-incremental --build-profile
dotnet build .\src\Commons.Test -c Release --no-dependencies -o "$OUTPUT\netcoreapp1.0" -f netcoreapp1.0 --no-incremental --build-profile
dotnet build .\src\Commons.Perf -c Release --no-dependencies -o "$OUTPUT\netcoreapp1.0" -f netcoreapp1.0 --no-incremental --build-profile

pushd $OUTPUT\netcoreapp1.0
dotnet test ..\..\..\Commons.Test -o .\ --no-build -f netcoreapp1.0 -parallel none 
popd


$xunit = "$env:userprofile\.nuget\packages\xunit.runner.console\2.1.0\tools\xunit.console.exe"

&$xunit "$OUTPUT\net40\Commons.Test.dll" -parallel none 
&$xunit "$OUTPUT\net45\Commons.Test.dll" -parallel none 

$nuget = ".\src\.nuget\NuGet.exe"


&$nuget pack Commons.nuspec -outputdirectory $OUTPUT -version $version
&$nuget pack Commons.Json.nuspec -outputdirectory $OUTPUT -version $version
&$nuget pack Commons.Pool.nuspec -outputdirectory $OUTPUT -version $version

pushd $OUTPUT\netcoreapp1.0
dotnet Commons.Perf.dll
popd
