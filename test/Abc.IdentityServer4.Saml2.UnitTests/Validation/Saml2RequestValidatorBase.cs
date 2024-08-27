using Abc.IdentityServer.Extensions;
using Abc.IdentityServer.Saml2.Stores;

namespace Abc.IdentityServer.Saml2.Validation.UnitTests
{
    public abstract class Saml2RequestValidatorBase
    {
        protected readonly Saml2RequestValidator validator;
        protected readonly IClock clock;

        public Saml2RequestValidatorBase()
        {
            var options = TestIdentityServerOptions.Create();
            var relayingPartyStore = new InMemoryRelyingPartyStore(new []
            {
                new RelyingParty
                {
                    EntityId = "urn:test",
                }
            });
            var clients = new InMemoryClientStore(new[]
            {
                  new Client
                    {
                        ClientId = "urn:test",
                        ClientName = "SAML2 Client",
                        ProtocolType = Ids.IdentityServerConstants.ProtocolTypes.Saml2p,
                        Enabled = true,
                        RedirectUris = { "https://saml2/callback" },
                        FrontChannelLogoutUri = "https://saml2/signout",
                    },
                    new Client
                    {
                        ClientName = "Code Client",
                        Enabled = true,
                        ClientId = "codeclient",
                    },
                });

                var uriValidator = new StrictRedirectUriValidator();

            var userSession = new MockUserSession();

            clock = new StubClock();
            validator = new Saml2RequestValidator(
                TestLogger.Create<Saml2RequestValidator>(),
                clients,
                userSession,
                uriValidator,
                options,
                clock,
                relayingPartyStore
                );
        }
     }
}