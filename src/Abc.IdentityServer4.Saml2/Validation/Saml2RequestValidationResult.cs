// ----------------------------------------------------------------------------
// <copyright file="Saml2RequestValidationResult.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

namespace Abc.IdentityServer.Saml2.Validation
{
    public class Saml2RequestValidationResult : Ids.Validation.ValidationResult
    {
        public Saml2RequestValidationResult(ValidatedSaml2Request validatedRequest)
        {
            IsError = false;
            ValidatedRequest = validatedRequest ?? throw new System.ArgumentNullException(nameof(validatedRequest));
        }

        public Saml2RequestValidationResult(ValidatedSaml2Request validatedRequest, string error, string errorDescription = null)
        {
            IsError = true;
            ValidatedRequest = validatedRequest ?? throw new System.ArgumentNullException(nameof(validatedRequest));
            Error = error;
            ErrorDescription = errorDescription;
        }

        public ValidatedSaml2Request ValidatedRequest { get; }
    }
}