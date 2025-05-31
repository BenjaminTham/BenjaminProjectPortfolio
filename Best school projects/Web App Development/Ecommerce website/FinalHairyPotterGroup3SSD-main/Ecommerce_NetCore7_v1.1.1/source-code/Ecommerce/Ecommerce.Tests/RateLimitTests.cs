using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
/*
namespace Ecommerce.Tests
{
    public class GeneralRateLimitTests : IClassFixture<WebApplicationFactory<TestStartup>>
    {
        private readonly HttpClient _client;

        public GeneralRateLimitTests(WebApplicationFactory<TestStartup> factory)
        {
            _client = factory.CreateClient();
        }

        // Test for general rate limiting across all endpoints
        [Fact]
        public async Task TestGeneralRateLimit()
        {
            for (int i = 0; i < 101; i++)
            {
                var response = await _client.GetAsync("/api/test");
                if (i < 100)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode);
                }
            }
        }
    }

    public class CheckoutRateLimitTests : IClassFixture<WebApplicationFactory<TestStartup>>
    {
        private readonly HttpClient _client;

        public CheckoutRateLimitTests(WebApplicationFactory<TestStartup> factory)
        {
            _client = factory.CreateClient();
        }

        // Test for rate limiting on the /checkout endpoint
        [Fact]
        public async Task TestCheckoutRateLimit()
        {
            for (int i = 0; i < 51; i++)
            {
                var response = await _client.PostAsync("/checkout", null);
                if (i < 50)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode);
                }
            }
        }
    }

    public class LoginRateLimitTests : IClassFixture<WebApplicationFactory<TestStartup>>
    {
        private readonly HttpClient _client;

        public LoginRateLimitTests(WebApplicationFactory<TestStartup> factory)
        {
            _client = factory.CreateClient();
        }

        // Test for rate limiting on the /my/login endpoint
        [Fact]
        public async Task TestLoginRateLimit()
        {
            for (int i = 0; i < 101; i++)
            {
                var response = await _client.PostAsync("/my/login", null);
                if (i < 100)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode);
                }
            }
        }
    }

    public class RegisterRateLimitTests : IClassFixture<WebApplicationFactory<TestStartup>>
    {
        private readonly HttpClient _client;

        public RegisterRateLimitTests(WebApplicationFactory<TestStartup> factory)
        {
            _client = factory.CreateClient();
        }

        // Test for rate limiting on the /my/register endpoint
        [Fact]
        public async Task TestRegisterRateLimit()
        {
            for (int i = 0; i < 101; i++)
            {
                var response = await _client.PostAsync("/my/register", null);
                if (i < 100)
                {
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    Assert.Equal(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode);
                }
            }
        }
    }
}
*/