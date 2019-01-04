﻿
 nuget url: https://www.nuget.org/packages/Fast.ApiGateway/
 ```csharp
//ConfigureServices 方法里 注入api网关
services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

//Configure方法里 使用api网关 
 app.UseMiddleware<FastApiGatewayDbHandler>();


```
