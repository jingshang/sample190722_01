using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using sample190722_01.Models;
using System.Web;
using Amazon.S3;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Reflection;

namespace sample190722_01.Controllers
{
	[DynamoDBTable("ASP.NET_SessionState")]
	class Item
	{
		[DynamoDBHashKey]
		public string SessionId { get; set; }
		public string aaa { get; set; }
	}
	public class HomeController : Controller
	{
		IAmazonS3 S3Client { get; set; }

		public HomeController(IAmazonS3 s3Client, IAmazonDynamoDB db)
		{
			this.S3Client = s3Client;

			var client = new AmazonDynamoDBClient((AmazonDynamoDBConfig)db.Config);
			var context = new DynamoDBContext(client);

			
			//var item = new Item
			//{
			//	SessionId = DateTime.UtcNow.ToString(),
			//	aaa = "Fish"+ DateTime.UtcNow.ToString(),
			//};

			//context.SaveAsync<Item>(item);

		}
		private string GetSessionKey()
		{
			var tc = ((Microsoft.AspNetCore.Session.DistributedSession)HttpContext.Session);

			System.Reflection.FieldInfo fieldInfo = tc.GetType().GetField("_sessionKey",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return fieldInfo.GetValue(tc).ToString();
		}
		public IActionResult Index()
		{
			HttpContext.Session.SetString("key", "Hoge");
			var views = (HttpContext.Session.GetInt32("ViewCount") ?? 0) + 1;
			HttpContext.Session.SetInt32("ViewCount", views);
			ViewData["Message"] = string.Format("You visited {0} times", views);
			ViewData["Message2"] = HttpContext.Session.Id;
			ViewData["Message3"] = GetSessionKey();


			// セッションから文字列を読み込む
			HttpContext.Session.GetString("key");

			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
