#!/bin/sh
xbuild /t:rebuild /p:Configuration=Release ./src/Commons.sln
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.BagTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.BimapTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.CollectionTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.ConcurrentListTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.HashTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.MultiMapTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.PriorityQueueTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.SortedMapTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JsonMapperTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JsonTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JValueTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Pool.ObjectPoolTest" -parallel none
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Utils.UtilsTest" -parallel none
