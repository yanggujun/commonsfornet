#!/bin/sh
xbuild /t:rebuild /p:Configuration=Release ./src/Commons.sln
echo "BagTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.BagTest" -parallel none
echo "BimapTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.BimapTest" -parallel none
echo "CollectionTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.CollectionTest" -parallel none
echo "ConcurrentListTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.ConcurrentListTest" -parallel none
echo "HashTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.HashTest" -parallel none
echo "MultiMapTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.MultiMapTest" -parallel none
echo "PriorityQueueTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.PriorityQueueTest" -parallel none
echo "SortedMapTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.SortedMapTest" -parallel none
echo "JsonMapperTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JsonMapperTest" -parallel none
echo "JsonTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JsonTest" -parallel none
echo "JValueTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JValueTest" -parallel none
echo "ObjectPoolTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Pool.ObjectPoolTest" -parallel none
echo "UtilsTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Utils.UtilsTest" -parallel none
