// ----------------------------------------------------------------------------
// <copyright file="Saml2EndSessionCallbackValidationResult.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityServer.Saml2.Services;
using System.Collections.Generic;

namespace Abc.IdentityServer.Saml2.Validation
{
    /// <summary>
    /// Validation result for end session callback requests.
    /// </summary>
    /// <seealso cref="ValidationResult" />
    public class Saml2EndSessionCallbackValidationResult : Ids.Validation.ValidationResult
    {
        /// <summary>
        /// Gets the client front-channel logout requests.
        /// </summary>
        public IEnumerable<Saml2LogoutRequest> FrontChannelLogoutRequests { get; set; }
    }
}