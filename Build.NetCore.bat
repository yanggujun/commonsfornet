call rmdir /S /Q ".\src\artifacts"

call dotnet build src\Commons.Utils -c Release --no-dependencies -o "src\artifacts\net40" -f net40 --no-incremental
call dotnet build src\Commons.Utils -c Release --no-dependencies -o "src\artifacts\net45" -f net45 --no-incremental
call dotnet build src\Commons.Utils -c Release --no-dependencies -o "src\artifacts\netstandard1.3" -f netstandard1.3 --no-incremental

call dotnet build src\Commons.Collections -c Release --no-dependencies -o "src\artifacts\net40" -f net40 --no-incremental
call dotnet build src\Commons.Collections -c Release --no-dependencies -o "src\artifacts\net45" -f net45 --no-incremental
call dotnet build src\Commons.Collections -c Release --no-dependencies -o "src\artifacts\netstandard1.3" -f netstandard1.3 --no-incremental


call dotnet build src\Commons.Json -c Release --no-dependencies -o "src\artifacts\net40" -f net40 --no-incremental
call dotnet build src\Commons.Json -c Release --no-dependencies -o "src\artifacts\net45" -f net45 --no-incremental
call dotnet build src\Commons.Json -c Release --no-dependencies -o "src\artifacts\netstandard1.3" -f netstandard1.3 --no-incremental

call dotnet build src\Commons.Pool -c Release --no-dependencies -o "src\artifacts\net40" -f net40 --no-incremental
call dotnet build src\Commons.Pool -c Release --no-dependencies -o "src\artifacts\net45" -f net45 --no-incremental
call dotnet build src\Commons.Pool -c Release --no-dependencies -o "src\artifacts\netstandard1.3" -f netstandard1.3 --no-incremental

call dotnet build src\Test.Commons -c Release --no-dependencies -o "src\artifacts\net40" -f net40 --no-incremental
call dotnet build src\Test.Commons -c Release --no-dependencies -o "src\artifacts\net45" -f net45 --no-incremental
call dotnet build src\Test.Commons -c Release --no-dependencies -o "src\artifacts\netcoreapp1.0" -f netcoreapp1.0 --no-incremental


call PAUSE
