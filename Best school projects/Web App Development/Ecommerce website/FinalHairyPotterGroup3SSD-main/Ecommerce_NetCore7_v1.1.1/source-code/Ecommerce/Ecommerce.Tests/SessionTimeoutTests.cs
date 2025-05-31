using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Tests
{
    public class SessionTimeoutTests
    {
        private readonly Mock<RequestDelegate> _nextMock; // Mock for the next middleware in the pipeline
        private readonly Mock<ILogger<SessionTimeoutMiddleware>> _loggerMock; // Mock for the logger
        private readonly SessionTimeoutMiddleware _middleware; // Instance of the middleware to be tested

        public SessionTimeoutTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<SessionTimeoutMiddleware>>();
            _middleware = new SessionTimeoutMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        // Creates a DefaultHttpContext with necessary service dependencies
        private DefaultHttpContext CreateHttpContext()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_loggerMock.Object); // Add logger to service collection
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var context = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            // Mock the response to capture the redirect location
            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupProperty(r => r.StatusCode); // Setup property to mock StatusCode
            responseMock.Setup(r => r.Redirect(It.IsAny<string>())).Callback<string>(location => context.Response.Headers["Location"] = location);
            responseMock.Setup(r => r.Headers).Returns(new HeaderDictionary());
            responseMock.Setup(r => r.HasStarted).Returns(false);
            context.Features.Set<IHttpResponseFeature>(new TestHttpResponseFeature(responseMock.Object));

            return context;
        }

        // Test case to ensure that expired sessions are redirected to the logout page
        [Fact]
        public async Task Invoke_SessionExpired_RedirectsToLogout()
        {
            var context = CreateHttpContext();
            var sessionMock = new Mock<ISession>();
            byte[] lastActivityBytes = BitConverter.GetBytes(DateTime.UtcNow.AddMinutes(-16).Ticks);

            sessionMock.Setup(s => s.TryGetValue("LastActivityTime", out lastActivityBytes)).Returns(true);
            context.Session = sessionMock.Object;
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser") }, "mock"));

            await _middleware.Invoke(context);

            Assert.Equal("/logout", context.Response.Headers["Location"].ToString());
            sessionMock.Verify(s => s.Clear(), Times.Once);
        }

        // Test case to ensure that active sessions continue processing
        [Fact]
        public async Task Invoke_SessionNotExpired_ContinuesProcessing()
        {
            var context = CreateHttpContext();
            var sessionMock = new Mock<ISession>();
            byte[] lastActivityBytes = BitConverter.GetBytes(DateTime.UtcNow.AddMinutes(-10).Ticks);

            sessionMock.Setup(s => s.TryGetValue("LastActivityTime", out lastActivityBytes)).Returns(true);
            context.Session = sessionMock.Object;
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser") }, "mock"));

            await _middleware.Invoke(context);

            _nextMock.Verify(next => next(context), Times.Once);
            sessionMock.Verify(s => s.Set("LastActivityTime", It.IsAny<byte[]>()), Times.Once);
        }

        // Test case to ensure that the first activity initializes the last activity time
        [Fact]
        public async Task Invoke_FirstTimeActivity_InitializesLastActivityTime()
        {
            var context = CreateHttpContext();
            var sessionMock = new Mock<ISession>();
            byte[] outBytes = null;

            sessionMock.Setup(s => s.TryGetValue("LastActivityTime", out outBytes)).Returns(false);
            context.Session = sessionMock.Object;
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser") }, "mock"));

            await _middleware.Invoke(context);

            _nextMock.Verify(next => next(context), Times.Once);
            sessionMock.Verify(s => s.Set("LastActivityTime", It.IsAny<byte[]>()), Times.Once);
        }

        // Test case to ensure that the middleware constructor initializes correctly
        [Fact]
        public void Middleware_Constructor_Should_Initialize()
        {
            Assert.NotNull(_middleware);
        }

        // Test case to ensure that authenticated users without a session initialize the session
        [Fact]
        public async Task Invoke_AuthenticatedUserWithoutSession_ShouldInitializeSession()
        {
            var context = CreateHttpContext();
            var sessionMock = new Mock<ISession>();
            byte[] outBytes = null;

            sessionMock.Setup(s => s.TryGetValue("LastActivityTime", out outBytes)).Returns(false);
            context.Session = sessionMock.Object;
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser") }, "mock"));

            await _middleware.Invoke(context);

            _nextMock.Verify(next => next(context), Times.Once);
            sessionMock.Verify(s => s.Set("LastActivityTime", It.IsAny<byte[]>()), Times.Once);
        }

        // Test case to ensure that unauthenticated users continue processing without session handling
        [Fact]
        public async Task Invoke_UnauthenticatedUser_ShouldContinueProcessing()
        {
            var context = CreateHttpContext();
            var sessionMock = new Mock<ISession>();

            context.Session = sessionMock.Object;
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());

            await _middleware.Invoke(context);

            _nextMock.Verify(next => next(context), Times.Once);
            sessionMock.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        }

        // Define a test implementation of IHttpResponseFeature to allow setting a mock HttpResponse.
        private class TestHttpResponseFeature : IHttpResponseFeature
        {
            public TestHttpResponseFeature(HttpResponse response)
            {
                Body = response.Body;
                Headers = response.Headers;
                StatusCode = response.StatusCode;
                HasStarted = response.HasStarted;
            }

            public int StatusCode { get; set; }
            public string ReasonPhrase { get; set; }
            public IHeaderDictionary Headers { get; set; }
            public Stream Body { get; set; }
            public bool HasStarted { get; set; }

            public void OnStarting(Func<object, Task> callback, object state) { }
            public void OnCompleted(Func<object, Task> callback, object state) { }
        }
    }
}
