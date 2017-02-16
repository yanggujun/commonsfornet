
. ./tools/_env.sh

rm -r ./src/artifacts

dotnet --info

dotnet restore

sh ./tools/_build.sh netstandard1.3
sh ./tools/_build.sh net40
sh ./tools/_build.sh net45


sh ./tools/_test.sh netcoreapp1.0
sh ./tools/_test.sh net40
sh ./tools/_test.sh net45

cd $OUTPUT/netcoreapp1.0
dotnet run Commons.Perf.dll
cd ../../../../
