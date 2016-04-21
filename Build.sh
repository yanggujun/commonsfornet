#!/bin/sh
xbuild /t:rebuild /p:Configuration=Release ./src/Commons.sln
echo "Executing BagTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.BagTest" -parallel none
echo "Executing BimapTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.BimapTest" -parallel none
echo "Executing CollectionTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.CollectionTest" -parallel none
echo "Executing ConcurrentListTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.ConcurrentListTest" -parallel none
echo "Executing HashTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.HashTest" -parallel none
echo "Executing MultiMapTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.MultiMapTest" -parallel none
echo "Executing PriorityQueueTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.PriorityQueueTest" -parallel none
echo "Executing SortedMapTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Collections.SortedMapTest" -parallel none
echo "Executing JsonMapperTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JsonMapperTest" -parallel none
echo "Executing JsonTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JsonTest" -parallel none
echo "Executing JValueTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Json.JValueTest" -parallel none
echo "Executing UtilsTest"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -class "Test.Commons.Utils.UtilsTest" -parallel none
echo "Executing ObjectPoolTest.TestNormalAcquirePoolNotFull"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestNormalAcquirePoolNotFull" -parallel none
echo "Executing ObjectPoolTest.TestAcquireAgain"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestAcquireAgain" -parallel none
echo "Executing ObjectPoolTest.TestAcquireAndReturnAtTheSameTime"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestAcquireAndReturnAtTheSameTime" -parallel none
echo "Executing ObjectPoolTest.TestAcquireExceedsCapacity"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestAcquireExceedsCapacity" -parallel none
echo "Executing ObjectPoolTest.TestAcquireTimeout"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestAcquireTimeout" -parallel none
echo "Executing ObjectPoolTest.TestPoolInitialSize"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolInitialSize" -parallel none
echo "Executing ObjectPoolTest.TestAcquireImmediatelyFailure"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestAcquireImmediatelyFailure" -parallel none
echo "Executing ObjectPoolTest.TestSequenceOperations"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestSequenceOperations" -parallel none
echo "Executing ObjectPoolTest.TestHungrySituation"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestHungrySituation" -parallel none
echo "Executing ObjectPoolTest.TestInitialSizeLessThanZero"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestInitialSizeLessThanZero" -parallel none
echo "Executing ObjectPoolTest.TestMaxSizeLessThanZero"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestMaxSizeLessThanZero" -parallel none
echo "Executing ObjectPoolTest.TestMaxSizeLessThanInitialSize"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestMaxSizeLessThanInitialSize" -parallel none
echo "Executing ObjectPoolTest.TestReturnObjectNotBelongToThePool"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestReturnObjectNotBelongToThePool" -parallel none
echo "Executing ObjectPoolTest.TestReturnSameObjectMoreThanOnce"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestReturnSameObjectMoreThanOnce" -parallel none
echo "Executing ObjectPoolTest.TestAcquireAlways"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestAcquireAlways" -parallel none
echo "Executing ObjectPoolTest.TestKeyedObjectPool"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestKeyedObjectPool" -parallel none
echo "Executing ObjectPoolTest.TestPoolManagerMaxSizeLessThanInitialSize"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolManagerMaxSizeLessThanInitialSize" -parallel none
echo "Executing ObjectPoolTest.TestWithoutFactory"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestWithoutFactory" -parallel none
echo "Executing ObjectPoolTest.TestWithCreator"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestWithCreator" -parallel none
echo "Executing ObjectPoolTest.TestFactoryOverrideCreator"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestFactoryOverrideCreator" -parallel none
echo "Executing ObjectPoolTest.TestDestroy"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestDestroy" -parallel none
echo "Executing ObjectPoolTest.TestPoolManagerAddPoolWithExistingKey"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolManagerAddPoolWithExistingKey" -parallel none
echo "Executing ObjectPoolTest.TestPoolManagerDestroyInvalidKey"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolManagerDestroyInvalidKey" -parallel none
echo "Executing ObjectPoolTest.TestPoolManagerDefaultValue"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolManagerDefaultValue" -parallel none
echo "Executing ObjectPoolTest.TestPoolManagerConcurrentCreatePool"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolManagerConcurrentCreatePool" -parallel none
echo "Executing ObjectPoolTest.TestPoolManagerDestroyEmptyKey"
mono ./src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe ./src/bin/NET45/Test.Commons.dll -method "Test.Commons.Pool.ObjectPoolTest.TestPoolManagerDestroyEmptyKey" -parallel none
