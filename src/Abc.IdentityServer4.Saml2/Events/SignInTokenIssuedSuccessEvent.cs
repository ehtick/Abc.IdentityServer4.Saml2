// ----------------------------------------------------------------------------
// <copyright file="SignInTokenIssuedSuccessEvent.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Extensions;
using Abc.IdentityServer.Saml2.Validation;
using System.Collections.Generic;

namespace Abc.IdentityServer.Saml2.Events
{
    public class SignInTokenIssuedSuccessEvent : TokenIssuedSuccessEvent
    {
        public SignInTokenIssuedSuccessEvent(HttpSaml2Message2 responseMessage, Saml2RequestValidationResult request)
            : base()
        {
            ClientId = request.ValidatedRequest.Client?.ClientId;
            ClientName = request.ValidatedRequest.Client?.ClientName;
            Endpoint = Constants.EndpointNames.SingleSignOn;
            SubjectId = request.ValidatedRequest.Subject?.GetSubjectId();
            Scopes = request.ValidatedRequest.ValidatedResources?.RawScopeValues.ToSpaceSeparatedString();

            var tokens = new List<Token>();
            tokens.Add(new Token("SecurityToken", (responseMessage as IHttpSaml2EncodedMessage)?.Data));
            Tokens = tokens;
        }
    }
}