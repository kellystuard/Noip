using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace KellyStuard.Noip
{
	public sealed class Program
	{
		public static async Task Main(string[] args)
		{
			var programCancel = new CancellationTokenSource();
			// handle SIGTERM on linux and fix run as a service on linux
			// https://github.com/Microsoft/vsts-agent/pull/242/commits/a0524568ebb010acc7339ea1fbb603f3b4b87788
			System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += (c) =>
			{
				programCancel.Cancel();
				programCancel.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string>
				{
					{"Hostnames:0", "foo3" },
					{"Hostnames:1", "bar3" },
					{"Username", "test3@example.com" },
					{"Password", "abc123" },
					{"MyIp", "192.168.100.101" },
					{"Offline", "true" },
				})
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build()
			;

			var loggerFactory = new LoggerFactory()
				//.AddConfiguration(configuration.GetSection("Logging"))
				.AddConsole(LogLevel.Information)
			;
			var logger = loggerFactory.CreateLogger<Program>();
			logger.LogInformation("Starting process");
			try
			{
				var settings = new Settings();
				configuration.Bind(settings);

				var process = new UpdateProcess(
					ClientBuilder.FromSettings(settings).Build(),
					QueryStringBuilder.FromSettings(settings).ToString(),
					programCancel.Token,
					loggerFactory.CreateLogger<UpdateProcess>()
				);
				await process.Run(settings.Interval);
			}
			catch (ApplicationException ae)
			{
				logger.LogCritical(ae, "Application failure.");
			}
			catch (ArgumentException ae)
			{
				logger.LogError(ae, $"Configuration value of {ae.ParamName} is invalid. {ae.Message}.");
			}
			catch (InvalidOperationException ie)
			{
				logger.LogError(ie, $"Configuration value is invalid. {ie.Message}.");
			}
		}
	}
}
