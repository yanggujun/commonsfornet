
set Framework=%1

set TestFramework=%1

If "%1" == "netstandard1.3" (
    set TestFramework=netcoreapp1.0
)

call dotnet build src\Commons.Utils -c Release --no-dependencies -o "%OUTPUT%\%Framework%" -f %Framework% --no-incremental --version-suffix %BuildNo% --build-profile
call dotnet build src\Commons.Collections -c Release --no-dependencies -o "%OUTPUT%\%Framework%" -f %Framework% --no-incremental --version-suffix %BuildNo% --build-profile
call dotnet build src\Commons.Json -c Release --no-dependencies -o "%OUTPUT%\%Framework%" -f %Framework% --no-incremental --version-suffix %BuildNo% --build-profile
call dotnet build src\Commons.Pool -c Release --no-dependencies -o "%OUTPUT%\%Framework%" -f %Framework% --no-incremental --version-suffix %BuildNo% --build-profile
call dotnet build src\Commons.Test -c Release --no-dependencies -o "%OUTPUT%\%TestFramework%" -f %TestFramework% --no-incremental --version-suffix %BuildNo% --build-profile

