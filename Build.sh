#!/bin/sh
xbuild /t:rebuild /p:Configuration=Release ./src/Commons.sln
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll
