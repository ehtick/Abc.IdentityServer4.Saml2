// ----------------------------------------------------------------------------
// <copyright file="LoginPageResult.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityModel.Protocols.Saml2;
using Abc.IdentityServer.Saml2.Validation;
using Abc.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.Endpoints.Results
{
    /// <summary>
    /// Result for login page.
    /// </summary>
    /// <seealso cref="IEndpointResult" />
    public class LoginPageResult : IEndpointResult
    {
        private readonly ValidatedSaml2Request _request;
        private IdentityServerOptions _options;
        private IAuthorizationParametersMessageStore _authorizationParametersMessageStore;
        private ISystemClock _clock;
        private IServerUrls _urls;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageResult"/> class.
        /// </summary>
        /// <param name="request">The validated eIDAS light request.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        public LoginPageResult(ValidatedSaml2Request request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        internal LoginPageResult(ValidatedSaml2Request request, IdentityServerOptions options, ISystemClock clock, IServerUrls urls, IAuthorizationParametersMessageStore authorizationParametersMessageStore)
            : this(request)
        {
            _options = options;
            _clock = clock;
            _urls = urls;
            _authorizationParametersMessageStore = authorizationParametersMessageStore;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            var returnUrl = _urls.BasePath.EnsureTrailingSlash() + Constants.ProtocolRoutePaths.SigleSignOnCallback;

            var msg = new Message<IDictionary<string, string[]>>(_request.Saml2RequestMessage.ToDictionary(), _clock.UtcNow.UtcDateTime);
            var id = await _authorizationParametersMessageStore.WriteAsync(msg);
            returnUrl = returnUrl.AddQueryString(Constants.DefaultRoutePathParams.MessageStoreIdParameterName, id);

            var loginUrl = _options.UserInteraction.LoginUrl;
            if (!loginUrl.IsLocalUrl())
            {
                // this converts the relative redirect path to an absolute one if we're 
                // redirecting to a different server
                returnUrl = _urls.Origin.EnsureTrailingSlash() + returnUrl.RemoveLeadingSlash();
            }

            var url = loginUrl.AddQueryString(_options.UserInteraction.LoginReturnUrlParameter, returnUrl);
            context.Response.Redirect(_urls.GetAbsoluteUrl(url));
        }

        private void Init(HttpContext context)
        {
            _options ??= context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _authorizationParametersMessageStore ??= context.RequestServices.GetRequiredService<IAuthorizationParametersMessageStore>();
            _urls ??= context.RequestServices.GetRequiredService<IServerUrls>();
            _clock ??= context.RequestServices.GetRequiredService<ISystemClock>();
        }
    }
}