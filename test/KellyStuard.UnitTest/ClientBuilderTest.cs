using System;
using System.Text;
using Xunit;

namespace KellyStuard.Noip.UnitTest
{
	public sealed class ClientBuilderTests
	{
		[Fact]
		public void NewBuilderNullsShouldThrow()
		{
			// arrange

			// act
			Action result = () => new ClientBuilder(null, null, null);

			// assert
			Assert.Throws<ArgumentNullException>("updateUrl", result);
		}

		[Fact]
		public void NewBuilderNoUpdateUrlShouldThrow()
		{
			// arrange

			// act
			Action result = () => new ClientBuilder(null, "foo", "bar");

			// assert
			Assert.Throws<ArgumentNullException>("updateUrl", result);
		}

		[Fact]
		public void NewBuilderNoUsernameShouldThrow()
		{
			// arrange

			// act
			Action result = () => new ClientBuilder(new Uri("http://example.com", UriKind.Absolute), null, "bar");

			// assert
			Assert.Throws<ArgumentNullException>("username", result);
		}

		[Fact]
		public void NewBuilderNoPasswordShouldThrow()
		{
			// arrange

			// act
			Action result = () => new ClientBuilder(new Uri("http://example.com", UriKind.Absolute), "foo", null);

			// assert
			Assert.Throws<ArgumentNullException>("password", result);
		}

		[Fact]
		public void NewBuilderShouldCreate()
		{
			// arrange

			// act
			var result = new ClientBuilder(new Uri("http://example.com", UriKind.Absolute), "foo", "bar");

			// assert
			Assert.NotNull(result);
		}

		[Fact]
		public void BuildingShouldReturn()
		{
			// arrange
			var builder = new ClientBuilder(new Uri("http://example.com", UriKind.Absolute), "foo", "bar");

			// act
			var result = builder.Build();

			// assert
			Assert.NotNull(result);
			Assert.Equal("KellyStuard NoIp Docker/1.0 foo", result.DefaultRequestHeaders.UserAgent.ToString());
			Assert.Equal("Basic", result.DefaultRequestHeaders.Authorization.Scheme);
			Assert.Equal(Convert.ToBase64String(Encoding.ASCII.GetBytes("foo:bar")), result.DefaultRequestHeaders.Authorization.Parameter);
		}

		[Fact]
		public void SettingsNullShouldThrow()
		{
			// arrange

			// act
			Action result = () => ClientBuilder.FromSettings(null);

			// assert
			Assert.Throws<ArgumentNullException>("settings", result);
		}

		[Fact]
		public void SettingsEmptyShouldThrow()
		{
			// arrange
			var settings = new Settings()
			{
				UpdateUrl = null,
			};

			// act
			Action result = () => ClientBuilder.FromSettings(settings);

			// assert
			Assert.Throws<ArgumentNullException>("uriString", result);
		}

		[Fact]
		public void SettingsUsernamePasswordShouldReturn()
		{
			// arrange
			var settings = new Settings()
			{
				Username = "foo",
				Password = "bar",
			};
			var builder = ClientBuilder.FromSettings(settings);

			// act
			var result = builder.Build();

			// assert
			Assert.NotNull(result);
			Assert.Equal("KellyStuard NoIp Docker/1.0 foo", result.DefaultRequestHeaders.UserAgent.ToString());
			Assert.Equal("Basic", result.DefaultRequestHeaders.Authorization.Scheme);
			Assert.Equal(Convert.ToBase64String(Encoding.ASCII.GetBytes("foo:bar")), result.DefaultRequestHeaders.Authorization.Parameter);
		}
	}
}
