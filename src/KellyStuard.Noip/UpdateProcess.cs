using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KellyStuard.Noip
{
	public sealed class UpdateProcess
	{
		public UpdateProcess(HttpClient updateclient, string queryString, CancellationToken cancellationToken, ILogger<UpdateProcess> logger)
		{
			_updateClient = updateclient;
			_queryString = queryString;
			_cancellationToken = cancellationToken;
			_logger = logger;
		}

		public async Task Run(TimeSpan interval)
		{
			while (!_cancellationToken.IsCancellationRequested)
			{
				await Process();
				_logger.LogInformation($"Waiting {interval} until next update...");
				await Task.Delay(interval);
			}
		}

		private async Task Process()
		{
			_logger.LogInformation($"Request: {_updateClient.BaseAddress}{_queryString}");
			var result = await _updateClient.GetAsync(_queryString, _cancellationToken);
			_logger.LogInformation($"Response: {result.StatusCode} {result.ReasonPhrase}");

			using (var reader = await result.Content.ReadAsStreamAsync())
			using (var stringReader = new StreamReader(reader))
			{
				var success = true;
				while (!stringReader.EndOfStream)
				{
					success &= await ProcessMessage(await stringReader.ReadLineAsync());
				}
				if (!success)
					throw new ApplicationException("Failed to process update");
			}
		}

		private async Task<bool> ProcessMessage(string message)
		{
			_logger.LogInformation(message);
			var verb = message.Split(' ')[0];

			switch (verb)
			{
				case "good":
					_logger.LogInformation($"{message} - DNS hostname update successful. Followed by a space and the IP address it was updated to.");
					return true;
				case "nochg":
					_logger.LogInformation($"{message} - IP address is current, no update performed. Followed by a space and the IP address that it is currently set to.");
					return true;
				case "nohost":
					_logger.LogError($"{message} - Hostname supplied does not exist under specified account, client exit and require user to enter new login credentials before performing an additional request.");
					return false;
				case "badauth":
					_logger.LogError($"{message} - Invalid username password combination.");
					return false;
				case "badagent":
					_logger.LogError($"{message} - Client disabled. Client should exit and not perform any more updates without user intervention. ");
					return false;
				case "!donator":
					_logger.LogError($"{message} - An update request was sent including a feature that is not available to that particular user such as offline options.");
					return false;
				case "abuse":
					_logger.LogError($"{message} - Username is blocked due to abuse. Either for not following our update specifications or disabled due to violation of the No-IP terms of service. Our terms of service can be viewed at https://www.noip.com/legal/tos. Client should stop sending updates.");
					return false;
				case "911":
					_logger.LogError($"{message} - A fatal error on our side such as a database outage. Retry the update no sooner than 30 minutes.");
					_logger.LogInformation("Waiting 30 minutes due to fatal error...");
					await Task.Delay(TimeSpan.FromMinutes(30));
					return true;
				default:
					_logger.LogError($"{message} - An unknown message was returned.");
					return false;
			}
		}

		private readonly HttpClient _updateClient;
		private readonly string _queryString;
		private readonly CancellationToken _cancellationToken;
		private readonly ILogger<UpdateProcess> _logger;
	}
}
