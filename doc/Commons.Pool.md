###Commons.Pool

  * A light weight generic object pool.
  
  ```csharp
      // When initializing the IoC container...
      IocContainer.Register<PoolManager>();

      //...
      // Somewhere in the application.
      var poolManager = IocContainer.Resolve<IPoolManager>();

      //...

      // Create a new pool.
      var sqlFactory = new DefaultDbConnectionFactory(connectionString);
      var connectionPool = poolManager.NewPoolOf<IDbConnection>()
                                      .InitialSize(0)
                                      .MaxSize(10)
                                      .WithFactory(sqlFactory)
                                      .Instance();

      //...
      var connection = connectionPool.Acquire();
 
      // ...
      // use the connection.
      // ...

      connectionPool.Return(connection);
      //...


      // When pool manager is disposed, the pool is disposed too.
      poolManager.Dispose();
  ```

