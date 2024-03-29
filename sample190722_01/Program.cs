﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sample190722_01
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("hosting.json", optional: true)
				.Build();

			//CreateWebHostBuilder(args).Build().Run();
			var host = new WebHostBuilder()
				.UseKestrel(options =>
				{
					// 最大接続数は100件
					options.Limits.MaxConcurrentConnections = 100;
					// リクエスト数は100Mまで
					options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
				})
				.UseUrls("http://*:5000", "https://*:5001")
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseKestrel()
				.UseStartup<Startup>();
	}
}
