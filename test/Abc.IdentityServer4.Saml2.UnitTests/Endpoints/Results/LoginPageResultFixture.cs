using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Extensions;
using Abc.IdentityServer.Saml2.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Xunit;

namespace Abc.IdentityServer.Saml2.Endpoints.Results.UnitTests
{
    public class LoginPageResultFixture
    {
        private LoginPageResult _target;
        private ValidatedSaml2Request _request;
        private IdentityServerOptions _options;
        private IClock _clock = new StubClock();
        private DefaultHttpContext _context;
        private AuthorizationParametersMessageStoreMock _authorizationParametersMessageStore;
        private IServerUrls _urls;

        public LoginPageResultFixture()
        {
            _context = new DefaultHttpContext();
            _context.RequestServices = new ServiceCollection().BuildServiceProvider();

            _options = new IdentityServerOptions();
            _options.UserInteraction.LoginUrl = "~/login";
            _options.UserInteraction.LoginReturnUrlParameter = "returnUrl";

            _authorizationParametersMessageStore = new AuthorizationParametersMessageStoreMock();

            _request = new ValidatedSaml2Request();
            _request.Saml2RequestMessage = new HttpSaml2RequestMessage2(new Uri("https://server"), "some_request", IdentityModel.Http.HttpDeliveryMethods.PostRequest);

            _urls = new MockServerUrls()
            {
                Origin = "https://server",
                BasePath = "/".RemoveTrailingSlash(), // as in DefaultServerUrls
            };
        }

        [Fact]
        public void login_ctor()
        {
            Action action = () =>
            {
                _target = new LoginPageResult(null, _options, _clock, _urls, _authorizationParametersMessageStore);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task login_should_redirect_to_login_page_and_passs_info()
        {
            _target = new LoginPageResult(_request, _options, _clock, _urls, _authorizationParametersMessageStore);

            await _target.ExecuteAsync(_context);

            _authorizationParametersMessageStore.Messages.Count.Should().Be(1);
            _context.Response.StatusCode.Should().Be(302);

            var location = _context.Response.Headers["Location"].First();
            location.Should().StartWith("https://server/login");

            var query = QueryHelpers.ParseQuery(new Uri(location).Query);
            query["returnUrl"].First().Should().StartWith("/saml2/callback");
            query["returnUrl"].First().Should().Contain("?authzId=" + _authorizationParametersMessageStore.Messages.First().Key);
        }
    }
}