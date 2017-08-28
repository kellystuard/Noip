using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace KellyStuard.Noip
{
	public sealed class ClientBuilder
	{
		public static ClientBuilder FromSettings(Settings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			return new ClientBuilder(
				new Uri(settings.UpdateUrl, UriKind.Absolute),
				settings.Username,
				settings.Password
			);
		}

		public ClientBuilder(Uri updateUrl, string username, string password)
		{
			_updateUrl = updateUrl ?? throw new ArgumentNullException(nameof(updateUrl));
			_username = username ?? throw new ArgumentNullException(nameof(username));
			_password = password ?? throw new ArgumentNullException(nameof(password));
		}

		public HttpClient Build()
		{
			var result = new HttpClient();
			result.BaseAddress = _updateUrl;
			result.DefaultRequestHeaders.UserAgent.ParseAdd($"KellyStuard NoIp Docker/1.0 {_username}");
			result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
				"Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"))
			);
			return result;
		}

		private readonly Uri _updateUrl;
		private readonly string _username;
		private readonly string _password;
	}
}
