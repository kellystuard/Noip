using System;
using Xunit;

namespace KellyStuard.Noip.UnitTest
{
	public sealed class QueryStringBuilderFromSettingsTests
	{
		[Fact]
		public void NullSettingsShouldThrow()
		{
			// arrange

			// act
			Action builder = () => QueryStringBuilder.FromSettings(null);

			// assert
			Assert.Throws<ArgumentNullException>("settings", builder);
		}

		[Fact]
		public void NullHostnamesShouldThrow()
		{
			// arrange
			var settings = new Settings();

			// act
			Action builder = () => QueryStringBuilder.FromSettings(settings);

			// assert
			Assert.Throws<ArgumentNullException>("hostnames", builder);
		}

		[Fact]
		public void NoHostnamesShouldReturnEmpty()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "",
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=", result);
		}

		[Fact]
		public void SingleHostnameShouldReturnHostname()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo",
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo", result);
		}

		[Fact]
		public void MultipleHostnamesShouldReturnHostnames()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo,bar",
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo,bar", result);
		}

		[Fact]
		public void HostnameIpShouldReturn()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo",
				MyIp = "127.0.0.1",
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo&myip=127.0.0.1", result);
		}

		[Fact]
		public void HostnameOfflineShouldReturn()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo",
				Offline = true,
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo&offline=YES", result);
		}

		[Fact]
		public void HostnameOnlineShouldReturn()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo",
				Offline = false,
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo&offline=NO", result);
		}

		[Fact]
		public void HostnameIpOfflineShouldReturn()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo",
				MyIp = "127.0.0.1",
				Offline = true,
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo&myip=127.0.0.1&offline=YES", result);
		}

		[Fact]
		public void HostnameIpOnlineShouldReturn()
		{
			// arrange
			var settings = new Settings()
			{
				Hostnames = "foo",
				MyIp = "127.0.0.1",
				Offline = false,
			};
			var builder = QueryStringBuilder.FromSettings(settings);

			// act
			var result = builder.ToString();

			// assert
			Assert.Equal("?hostname=foo&myip=127.0.0.1&offline=NO", result);
		}
	}
}
