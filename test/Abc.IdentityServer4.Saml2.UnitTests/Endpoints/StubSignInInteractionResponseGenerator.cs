using Abc.IdentityServer.Saml2.ResponseProcessing;
using Abc.IdentityServer.Saml2.Validation;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.Endpoints.UnitTests
{
    internal class StubSignInInteractionResponseGenerator : ISignInInteractionResponseGenerator
    {
        internal InteractionResponse Response { get; set; } = new InteractionResponse();

        public Task<InteractionResponse> ProcessInteractionAsync(ValidatedSaml2Request request, ConsentResponse consent = null)
        {
            return Task.FromResult(Response);
        }
    }
}