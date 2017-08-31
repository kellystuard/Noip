using System;
using System.Net;

namespace KellyStuard.Noip
{
	public sealed class Settings
	{
		public string UpdateUrl { get; set; } = "https://dynupdate.no-ip.com/nic/update";
		/// <summary>
		/// Required. Username associated with the hosts that are to be updated. No-IP uses an email address as the
		/// username. Email addresses will be no longer than 50 characters.
		/// </summary>
		public string Username { get; set; }
		/// <summary>
		/// Required. Password associated with the hosts that are to be updated.
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// Required. The hostname(s) (host.domain.com) or group(s) (group_name) to be updated. If updating multiple
		/// hostnames or groups use a comma separated list: hostname=host1.domain.com,group1,host2.domain.com. Results
		/// are returned in the order you submitted them to the API per line.
		/// </summary>
		public string Hostnames { get; set; }
		/// <summary>
		/// Optional. The IP address to which the host(s) will be set. If no IP address is supplied the WAN address
		/// connecting to our system will be used. Clients behind NAT, for example, would not need to supply an IP
		/// address.
		/// </summary>
		public string MyIp { get; set; }
		/// <summary>
		/// Optional. Sets the current host to offline status. Offline settings are an Enhanced / No-IP Plus feature.
		/// When offline mode is enabled, the host will use whatever offline method is selected on the No-IP.com
		/// website for that host. Possible values are YES and NO with default of NO. If an update request is
		/// performed on an offline host, the host is removed from the offline state.
		/// </summary>
		public bool? Offline { get; set; }
		/// <summary>
		/// Optional. Time span between IP updates. Minimum time is 5 minutes. Default is 5 days.
		/// </summary>
		public TimeSpan Interval { get; set; } = TimeSpan.FromDays(5);
	}
}
