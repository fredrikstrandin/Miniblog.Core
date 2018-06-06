using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Multiblog.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace XUnit.Test
{
    public class BlogApi
    {
        private const string DiscoveryEndpoint = "https://server/.well-known/openid-configuration";
        private const string TokenEndpoint = "https://server/connect/token";

        private TestServer _identityServerServer;
        private HttpClient _identiyClient { get; }
        public HttpClient _apiClient { get; }

        private readonly HttpMessageHandler _handler;

        public BlogApi()
        {
            var identityServerBuilder = new WebHostBuilder()
              .UseContentRoot(@"C:\git\Multiblog.Core\src\Multiblog.OAuth")
              .UseEnvironment("Development")
              .UseStartup<Host.Startup>()
              .UseApplicationInsights();

            _identityServerServer = new TestServer(identityServerBuilder);

            _handler = _identityServerServer.CreateHandler();
            _identiyClient = _identityServerServer.CreateClient();

            var apiBuilder = new WebHostBuilder()
              .UseContentRoot(@"C:\git\Multiblog.Core\src\Multiblog.Core")
              .UseEnvironment("Test")
              .UseStartup<Multiblog.Core.Startup>()
              .ConfigureServices(services => 
                {
                    services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                        .AddIdentityServerAuthentication(q =>
                        {
                            q.Authority = "https://server";
                            q.JwtBackChannelHandler = _identityServerServer.CreateHandler();
                            q.IntrospectionBackChannelHandler = _identityServerServer.CreateHandler();
                            q.IntrospectionDiscoveryHandler = _identityServerServer.CreateHandler();
                        });
                })
              .UseApplicationInsights();

            
            var apiServer = new TestServer(apiBuilder);
            
            _apiClient = apiServer.CreateClient();
            
            var client = new TokenClient(
                TokenEndpoint,
                "MetaWeblog.client",
                "KanBandyBliKul",
                innerHttpMessageHandler: _handler);

            Task<TokenResponse> task = client.RequestResourceOwnerPasswordAsync("strandin72@gmail.com", "vemvet", "blog");
            task.Wait();

            TokenResponse response = task.Result;

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);

        }

        [Fact]
        public async Task GetComment()
        {
            HttpResponseMessage responseApi = await _apiClient.GetAsync("/api/Comment");

            Assert.Equal(HttpStatusCode.OK, responseApi.StatusCode);
        }

        [Fact]
        public async Task PostComment()
        {
            var formData = new Dictionary<string, string>
            {
                { "MessageId", Guid.NewGuid().ToString() },
                { "MessageText", "Test av Post" }
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Comment")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            var ApiResponse = await _apiClient.SendAsync(postRequest);

            ApiResponse.EnsureSuccessStatusCode();

            var responseString = await ApiResponse.Content.ReadAsStringAsync();

            MessageRespons respons = JsonConvert.DeserializeObject<MessageRespons>(await ApiResponse.Content.ReadAsStringAsync());
            Assert.True(respons.Success);
        }
    }
}
