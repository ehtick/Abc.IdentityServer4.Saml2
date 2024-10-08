﻿using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Saml2.ResponseProcessing;
using Abc.IdentityServer.Saml2.Validation;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.Endpoints.UnitTests
{
    internal class StubSignInResponseGenerator : ISignInResponseGenerator
    {
        internal HttpSaml2Message2 Result { get; set; }

        public Task<HttpSaml2Message2> GenerateResponseAsync(Saml2RequestValidationResult validationResult)
        {
            return Task.FromResult(Result);
        }
    }
}