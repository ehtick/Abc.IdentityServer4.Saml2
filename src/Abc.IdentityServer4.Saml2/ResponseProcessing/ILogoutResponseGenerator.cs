﻿// ----------------------------------------------------------------------------
// <copyright file="ILogoutResponseGenerator.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Saml2.Validation;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.ResponseProcessing
{
    internal interface ILogoutResponseGenerator
    {
        Task<HttpSaml2Message2> GenerateResponseAsync(Saml2RequestValidationResult validationResult);
    }
}