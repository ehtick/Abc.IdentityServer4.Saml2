﻿// ----------------------------------------------------------------------------
// <copyright file="Saml2BuilderExtensions.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityServer.Extensions;
using Abc.IdentityServer.Saml2;
using Abc.IdentityServer.Saml2.Endpoints;
using Abc.IdentityServer.Saml2.ResponseProcessing;
using Abc.IdentityServer.Saml2.Services;
using Abc.IdentityServer.Saml2.Stores;
using Abc.IdentityServer.Saml2.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Saml2BuilderExtensions
    {
        public static IIdentityServerBuilder AddSaml2(this IIdentityServerBuilder builder)
        {
            return AddSaml2<NoRelyingPartyStore>(builder);
        }

        public static IIdentityServerBuilder AddSaml2<TStore>(this IIdentityServerBuilder builder)
            where TStore : class, IRelyingPartyStore
        {
#if IDS4
            builder.Services.AddTransient<IServerUrls, DefaultServerUrls>();
            builder.Services.AddTransient<IIssuerNameService, DefaultIssuerNameService>();
#endif
            builder.Services.AddTransient<IMetadataResponseGenerator, MetadataResponseGenerator>();
            builder.Services.AddTransient<ISignInResponseGenerator, SignInResponseGenerator>();
            builder.Services.AddTransient<ISaml2RequestValidator, Saml2RequestValidator>();
            builder.Services.AddTransient<ISignInInteractionResponseGenerator, SignInInteractionResponseGenerator>();
            //builder.Services.AddTransient<ISignOutValidator, SignOutValidator>();
            builder.Services.AddTransient<Abc.IdentityServer.Saml2.Services.IClaimsService, Abc.IdentityServer.Saml2.Services.DefaultClaimsService>();
            builder.Services.AddTransient<Ids.Services.IReturnUrlParser, Saml2ReturnUrlParser>();

            // to support federated logout, use iframe, only redirect binding support
            //builder.Services.Decorate<IdentityServer4.Services.ILogoutNotificationService, LogoutNotificationService>();
            // _OR_
            // to support federated logout, use iframe 
            builder.Services.AddTransient<ISaml2LogoutNotificationService, Saml2LogoutNotificationService>();
            builder.Services.AddTransient<ISaml2EndSessionRequestValidator, Saml2EndSessionRequestValidator>();
            builder.Services.Decorate<Ids.Services.IIdentityServerInteractionService, Saml2IdentityServerInteractionService>();

            builder.Services.AddTransient<ILogoutRequestGenerator, LogoutRequestGenerator>();
            builder.Services.AddTransient<ILogoutResponseGenerator, LogoutResponseGenerator>();

            builder.Services.TryAddTransient<IRelyingPartyStore, TStore>();

            builder.Services.AddTransient<IArtifactStore, DefaultArtifactStore>();

            builder.Services.AddTransient<Abc.IdentityModel.Protocols.Saml2.ISaml2TokenToSerializerAdaptor, Abc.IdentityModel.Protocols.Saml2.Saml2TokenToSerializerAdaptor>();
            builder.Services.AddTransient(s =>
            {
                var adaptor = s.GetRequiredService<Abc.IdentityModel.Protocols.Saml2.ISaml2TokenToSerializerAdaptor>();
                var options = s.GetRequiredService<IOptions<Saml2SPOptions>>().Value;
                var tokenValidationParameters = options.TokenValidationParameters;

                // UNDONE: disabled signature validation
                tokenValidationParameters = null;

                return new Abc.IdentityModel.Protocols.Saml2.HttpSaml2MessageSerializer(new Abc.IdentityModel.Protocols.Saml2.Saml2ProtocolSerializer(tokenValidationParameters, adaptor, null), tokenValidationParameters);
            });

            builder.Services.AddSingleton(
                resolver => resolver.GetRequiredService<IOptions<Saml2SPOptions>>().Value);

            builder.AddEndpoint<Saml2SingleSignOnEndpoint>(Constants.EndpointNames.SingleSignOn, Constants.ProtocolRoutePaths.SingleSignOn.EnsureLeadingSlash());
            builder.AddEndpoint<Saml2SingleSignOnCallbackEndpoint>(Constants.EndpointNames.SingleSignOnCallback, Constants.ProtocolRoutePaths.SigleSignOnCallback.EnsureLeadingSlash());
            builder.AddEndpoint<Saml2SingleLogOutCallbackEndpoint>(Constants.EndpointNames.SingleLogoutServiceCallback, Constants.ProtocolRoutePaths.SingleLogoutServiceCallback.EnsureLeadingSlash());
            builder.AddEndpoint<Saml2MetadataEndpoint>(Constants.EndpointNames.Metadata, Constants.ProtocolRoutePaths.Metadata.EnsureLeadingSlash());
            builder.AddEndpoint<Saml2ArtifactResolutionEndpoint>(Constants.EndpointNames.ArtefactResolutionService, Constants.ProtocolRoutePaths.ArtefactResolutionService.EnsureLeadingSlash());
            builder.AddEndpoint<Saml2EndSessionCallbackEndpoint>(Constants.EndpointNames.EndSessionCallback, Constants.ProtocolRoutePaths.EndSessionCallback.EnsureLeadingSlash());

            return builder;
        }

        public static IIdentityServerBuilder AddSaml2(this IIdentityServerBuilder builder, Action<Saml2SPOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder.AddSaml2();
        }

        public static IIdentityServerBuilder AddSaml2(this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<Saml2SPOptions>(configuration);
            return builder.AddSaml2();
        }

        public static IIdentityServerBuilder AddInMemoryRelyingParties(this IIdentityServerBuilder builder, IEnumerable<RelyingParty> relyingParties)
        {
            builder.Services.AddSingleton(relyingParties);
            builder.Services.AddSingleton<IRelyingPartyStore, InMemoryRelyingPartyStore>();
            return builder;
        }
    }
}