﻿// ----------------------------------------------------------------------------
// <copyright file="LogoutResponseGenerator.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityModel.Http;
using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Extensions;
using Abc.IdentityServer.Saml2.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.ResponseProcessing
{
    internal class LogoutResponseGenerator : ILogoutResponseGenerator
    {
        private readonly ILogger _logger;
        private readonly Saml2SPOptions _options;
        private readonly IIssuerNameService _issuerNameService;
        private readonly IKeyMaterialService _keys;
        private readonly IClock _clock;

        public LogoutResponseGenerator(
            ILogger<LogoutResponseGenerator> logger,
            Saml2SPOptions options,
            IIssuerNameService issuerNameService,
            IKeyMaterialService keys,
            IClock clock)
        {
            _logger = logger;
            _options = options;
            _issuerNameService = issuerNameService;
            _keys = keys;
            _clock = clock;
        }

        public async Task<HttpSaml2Message2> GenerateResponseAsync(Saml2RequestValidationResult validationResult)
        {
            _logger.LogDebug("Creating SAML2 signout response");

            var validatedRequest = validationResult.ValidatedRequest;

            var credentials = await _keys.GetX509SigningCredentialsAsync();
            var issuer = await _issuerNameService.GetCurrentAsync();
            var issueInstant = _clock.UtcNow.UtcDateTime;

            var signingCredentials = new SigningCredentials(
                credentials.Key,
                validatedRequest.RelyingParty?.SignatureAlgorithm ?? _options.DefaultSignatureAlgorithm,
                validatedRequest.RelyingParty?.DigestAlgorithm ?? _options.DefaultDigestAlgorithm);

            var logoutRequest = validatedRequest.Saml2RequestMessage.Saml2Request as Saml2LogoutRequest;
            var destination = validatedRequest.ReplyUrl;
            var binding = validatedRequest.RelyingParty?.FrontChannelLogoutBinding;

            var logoutResponse = new Saml2LogoutResponse(new Saml2Status(Saml2StatusCode.Success))
            {
                InResponseTo = logoutRequest.Id,
                Issuer = new Saml2NameIdentifier(issuer),
                SigningCredentials = signingCredentials,
                IssueInstant = issueInstant,
                Destination = new Uri(destination),
            };

            var method =
                string.Equals(binding, Abc.IdentityModel.Protocols.Saml2.Saml2Constants.ProtocolBindings.HttpPostString)
                ? HttpDeliveryMethods.PostRequest
                : HttpDeliveryMethods.GetRequest;

            return new HttpSaml2ResponseMessage2(logoutResponse.Destination, logoutResponse, method)
            {
                RelayState = validatedRequest.Saml2RequestMessage.RelayState,
            };
        }
    }
}