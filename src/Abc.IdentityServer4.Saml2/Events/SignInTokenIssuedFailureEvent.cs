// ----------------------------------------------------------------------------
// <copyright file="SignInTokenIssuedFailureEvent.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityServer.Saml2.Validation;

namespace Abc.IdentityServer.Saml2.Events
{
    public class SignInTokenIssuedFailureEvent : TokenIssuedFailureEvent
    {
        public SignInTokenIssuedFailureEvent(ValidatedSaml2Request request, string error, string description)
            : base()
        {
            if (request != null)
            {
                ClientId = request.Client?.ClientId;
                ClientName = request.Client?.ClientName;

                if (request.Subject != null && request.Subject.Identity.IsAuthenticated)
                {
                    SubjectId = request.Subject.GetSubjectId();
                }
            }

            Endpoint = Constants.EndpointNames.SingleSignOn;
            Error = error;
            ErrorDescription = description;
        }
    }
}