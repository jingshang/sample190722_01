using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SessionProvider;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace sample190722_01
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
			  .SetBasePath(env.ContentRootPath)
			  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			  .AddEnvironmentVariables();
			Configuration = builder.Build();

		}
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// 設定ファイル読込
			var c = Configuration.GetAWSOptions();
			services.AddDefaultAWSOptions(c);

			// AWSのサービスを登録
			services.AddAWSService<IAmazonS3>();
			services.AddAWSService<IAmazonDynamoDB>();
			services.AddAWSService<IAmazonSimpleSystemsManagement>();

			// DynamoDBでセッションを管理する
			services.AddDistributedDynamoDbCache(o => { o.TableName = "ASP.NET_SessionState"; });
			services.AddAWSService<IAmazonDynamoDB>();
			services.AddSession();

			//PsXmlRepositoryのシングルトンコピーをサービスミドルウェアコレクションに追加
			//services.AddSingleton<IXmlRepository, PsXmlRepository>();//1.クラスのインスタンスをサービス登録
			////データ保護オプションを構成、PsXmlRepositoryリポジトリを使用して暗号化キーをXMLとして保存します。
			//var sp = services.BuildServiceProvider();//2.サービスをインスタンス化
			//services.AddDataProtection().AddKeyManagementOptions(o => o.XmlRepository = sp.GetService<IXmlRepository>());// 

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
