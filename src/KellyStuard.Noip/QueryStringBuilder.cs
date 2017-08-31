using System;
using System.Collections.Generic;
using System.Text;

namespace KellyStuard.Noip
{
	/// <summary>
	/// A chainable query string helper class.
	/// Example usage :
	/// string strQuery = QueryString.Current.Add("id", "179").ToString();
	/// string strQuery = new QueryString().Add("id", "179").ToString();
	/// </summary>
	public sealed class QueryStringBuilder
	{
		public static QueryStringBuilder FromSettings(Settings settings)
		{
			if (settings == null) throw new ArgumentNullException(nameof(settings));

			return new QueryStringBuilder(
				settings.Hostnames,
				settings.MyIp,
				settings.Offline
			);
		}

		public QueryStringBuilder() { }

		public QueryStringBuilder(string hostnames, string myip, bool? offline)
		{
			Add("hostname", hostnames?.Split(',') ?? throw new ArgumentNullException(nameof(hostnames)));
			if (myip != null)
				Add("myip", myip);
			if (offline.HasValue)
				Add("offline", (offline.Value) ? "YES" : "NO");
		}

		/// <summary>
		/// Adds or appends to an existing a query string parameter to the query string.
		/// </summary>
		/// <param name="name">The query string parameter name.</param>
		/// <param name="values">The query string parameter value.</param>
		public void Add(string name, params string[] values)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (values == null) throw new ArgumentNullException(nameof(values));

			foreach (var value in values)
				Add(name, value);
		}

		/// <summary>
		/// Adds or appends to an existing a query string parameter to the query string.
		/// </summary>
		/// <param name="name">The query string parameter name.</param>
		/// <param name="value">The query string parameter value.</param>
		public void Add(string name, string value)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (value == null) throw new ArgumentNullException(nameof(value));

			if (queryEntries.ContainsKey(name))
				queryEntries[name] += "," + Uri.EscapeDataString(value);
			else
				queryEntries.Add(name, Uri.EscapeDataString(value));
		}

		/// <summary>
		/// Removes a query string parameter from the query string.
		/// </summary>
		/// <param name="name">The query string parameter name.</param>
		public bool Remove(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			return queryEntries.Remove(name);
		}

		/// <summary>
		/// Clears the query string.
		/// </summary>
		public void Reset()
		{
			queryEntries.Clear();
		}

		/// <summary>
		/// Outputs the query string.
		/// </summary>
		/// <returns>The encoded querystring as it would appear in a URL.</returns>
		public override string ToString()
		{
			var builder = new StringBuilder();
			foreach (var item in queryEntries)
				builder
					.Append("&").Append(item.Key)
					.Append('=').Append(item.Value)
				;

			if (builder.Length != 0)
				builder[0] = '?';

			return builder.ToString();
		}

		private readonly Dictionary<string, string> queryEntries = new Dictionary<string, string>();
	}
}
