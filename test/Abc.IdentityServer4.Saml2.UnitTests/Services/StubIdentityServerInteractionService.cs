using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.Services.UnitTests
{
    internal class StubIdentityServerInteractionService : IIdentityServerInteractionService
    {
        public Task<string> CreateLogoutContextAsync()
        {
            throw new NotImplementedException();
        }

        public Task DenyAuthorizationAsync(AuthorizationRequest request, AuthorizationError error, string errorDescription = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Grant>> GetAllUserGrantsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthorizationRequest> GetAuthorizationContextAsync(string returnUrl)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorMessage> GetErrorContextAsync(string errorId)
        {
            throw new NotImplementedException();
        }

        public Task<LogoutRequest> GetLogoutContextAsync(string logoutId)
        {
            throw new NotImplementedException();
        }

        public Task GrantConsentAsync(AuthorizationRequest request, ConsentResponse consent, string subject = null)
        {
            throw new NotImplementedException();
        }

        public bool IsValidReturnUrl(string returnUrl)
        {
            throw new NotImplementedException();
        }

        public Task RevokeTokensForCurrentSessionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RevokeUserConsentAsync(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
