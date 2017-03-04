
. ./tools/_env.sh

rm -r ./src/artifacts

dotnet --info

dotnet restore

sh ./tools/_build.sh netstandard1.3

sh ./tools/_test.sh netcoreapp1.0

cd $OUTPUT/netcoreapp1.0
dotnet Commons.Perf.dll
cd ../../../../
