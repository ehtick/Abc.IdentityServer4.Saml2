﻿// ----------------------------------------------------------------------------
// <copyright file="LogoutNotificationService.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityModel.Http;
using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Saml2.ResponseProcessing;
using Abc.IdentityServer.Saml2.Stores;
using Abc.IdentityServer.Saml2.Validation;
using Abc.IdentityServer.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.Services
{
    internal class LogoutNotificationService : ILogoutNotificationService
    {
        private readonly ILogoutNotificationService _logoutNotificationService;
        private readonly IRelyingPartyStore _relyingPartyStore;
        private readonly IClientStore _clientStore;
        private readonly ILogger _logger;
        private readonly ILogoutRequestGenerator _requestGenerator;
        private readonly HttpSaml2MessageSerializer _serializer;

        public LogoutNotificationService(
            ILogoutNotificationService logoutNotificationService, 
            IRelyingPartyStore relyingPartyStore,
            IClientStore clientStore,
            ILogoutRequestGenerator requestGenerator,
            HttpSaml2MessageSerializer serializer,
            ILogger<LogoutNotificationService> logger)
        {
            _logoutNotificationService = logoutNotificationService;
            _relyingPartyStore = relyingPartyStore;
            _clientStore = clientStore;
            _requestGenerator = requestGenerator;
            _serializer = serializer;
            _logger = logger;
        }

        public Task<IEnumerable<BackChannelLogoutRequest>> GetBackChannelLogoutNotificationsAsync(LogoutNotificationContext context)
        {
            return _logoutNotificationService.GetBackChannelLogoutNotificationsAsync(context);
        }

        public async Task<IEnumerable<string>> GetFrontChannelLogoutNotificationsUrlsAsync(LogoutNotificationContext context)
        {
            var frontChannelUrls = new List<string>();
            frontChannelUrls.AddRange(await _logoutNotificationService.GetFrontChannelLogoutNotificationsUrlsAsync(context));

            // add SAML2 clients who support redirect binding
            foreach (var cid in context.ClientIds)
            {
                var participant = (Saml2SessionParticipant)cid;
                var clientId = participant.ClientId;

                var client = await _clientStore.FindEnabledClientByIdAsync(clientId);
                if (client is null || client.ProtocolType != Ids.IdentityServerConstants.ProtocolTypes.Saml2p || !client.FrontChannelLogoutUri.IsPresent())
                {
                    continue;
                }

                var relyingParty = await _relyingPartyStore.FindRelyingPartyByEntityIdAsync(clientId);

                var sloBinding = relyingParty?.FrontChannelLogoutBinding ?? Saml2Constants.ProtocolBindings.HttpRedirectString;
                if (sloBinding != Saml2Constants.ProtocolBindings.HttpRedirectString)
                {
                    _logger.LogWarning($"Cannot generate SLO request for service provider '{clientId}'. Only support Redirect binding");
                    continue;
                }

                var vr = new Saml2RequestValidationResult(new ValidatedSaml2Request()
                {
                    Subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("sub", context.SubjectId) })),
                    RelyingParty = relyingParty,
                    SessionParticipant = participant,
                    ReplyUrl = client.FrontChannelLogoutUri,
                });

                var sloMessage = await _requestGenerator.GenerateRequestAsync(vr);

                if ((sloMessage.HttpMethods & HttpDeliveryMethods.GetRequest) == HttpDeliveryMethods.GetRequest)
                {
                    frontChannelUrls.Add(_serializer.GetRequestUrl(sloMessage));
                }
            }

            if (frontChannelUrls.Any())
            {
                var msg = frontChannelUrls.Aggregate(
                            new StringBuilder(),
                            (a, b) => a.Append(", ").Append(b),
                            a => a.Remove(0, 2).ToString());
                _logger.LogDebug("Client front-channel logout URLs: {0}", msg);
            }
            else
            {
                _logger.LogDebug("No client front-channel logout URLs");
            }

            return frontChannelUrls;
        }
    }
}