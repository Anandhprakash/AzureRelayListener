using System;
using Microsoft.Azure.Relay;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;


namespace AzureRelayListener
{
	class Program
	{
		//set your connection parameters
		//private const string Relaynamespace = "{Relaynamespace}.servicebus.windows.net";
		//private const string CoonectionName = "{HybridconnectionName}";
		//private const string KeyName = "{SASKeyName}";
		//private const string Key = "{SASKey}";

		//set your connection parameters
		private const string RelayNamespace = "azurerelaydemo.servicebus.windows.net";//"{Relaynamespace}.servicebus.windows.net";
		private const string ConnectionName = "relayhbconn";
		private const string KeyName = "cListen";
		private const string Key = "6tC9TndOvb35jwASF356q96RRrBgssaL63K1F98j904=";									

		public static void Main(string[] args)
		{
			RunAsync().GetAwaiter().GetResult();
		}

		private static async Task RunAsync()
		{
			var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
			var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);

			//Subscribe to the status events
			listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
			listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
			listener.Online += (o, e) => { Console.WriteLine("Online"); };
			var testData = string.Empty;
			
			//Provide an HTTP request handler
			listener.RequestHandler = (context) =>
			{
				//Do something with context.Request.Url, HttpMethod, Header, InputStream..
				context.Response.StatusCode = HttpStatusCode.OK;
				context.Response.StatusDescription = "OK";
				using (var sw = new StreamWriter(context.Response.OutputStream))
				{
					//string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Promo.txt");
					string path = @"d:\Promo.txt";
					//string[] files = File.ReadAllLines(path);
					string text = System.IO.File.ReadAllText(path);
					sw.WriteLine(text);				
				}

				//The context MUST be closed here
				context.Response.Close();
			};
		// Opening the listener establishes the control channel to
			// the Azure Relay service. The control channel is continuously 
			// maintained, and is reestablished when connectivity is disrupted.
			await listener.OpenAsync();
			Console.WriteLine("Server listening");

			// Start a new thread that will continuously read the console.
			await Console.In.ReadLineAsync();

			// Close the listener after you exit the processing loop.
			await listener.CloseAsync();
		}

		/*private static async Task RunAsync()
		{
			var cts = new CancellationTokenSource();

			var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
			var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);

			// Subscribe to the status events.
			listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
			listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
			listener.Online += (o, e) => { Console.WriteLine("Online"); };

			// Provide an HTTP request handler
			listener.RequestHandler = (context) =>
			{
				// Do something with context.Request.Url, HttpMethod, Headers, InputStream...
				context.Response.StatusCode = HttpStatusCode.OK;
				context.Response.StatusDescription = "OK, This is pretty neat";
				using (var sw = new StreamWriter(context.Response.OutputStream))
				{
					sw.WriteLine("hello welcome!");
				}

				// The context MUST be closed here
				context.Response.Close();
			};

			// Opening the listener establishes the control channel to
			// the Azure Relay service. The control channel is continuously 
			// maintained, and is reestablished when connectivity is disrupted.
			await listener.OpenAsync();
			Console.WriteLine("Server listening");

			// Start a new thread that will continuously read the console.
			await Console.In.ReadLineAsync();

			// Close the listener after you exit the processing loop.
			await listener.CloseAsync();
		}*/
	}
}
