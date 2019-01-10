﻿using FastApiGatewayDb;
using FastData.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace TestFastApiGateway.Db
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //注册gbk
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("GB2312");
            
            //压缩
            services.AddResponseCompression();

            //注入
            services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            FastMap.InstanceProperties("FastApiGatewayDb.DataModel", "FastApiGatewayDb.dll");
            FastMap.InstanceTable("FastApiGatewayDb.DataModel", "FastApiGatewayDb.dll");
            FastMap.InstanceMap();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<FastApiGatewayDbHandler>();

        }
    }
}
