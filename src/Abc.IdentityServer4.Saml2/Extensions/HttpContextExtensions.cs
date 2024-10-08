﻿// ----------------------------------------------------------------------------
// <copyright file="HttpContextExtensions.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityServer.Saml2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Extensions
{
    internal static class HttpContextExtensions
    {
        public static string GetClientIpAddress(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString();
        }

        internal static async Task<string> GetIdentityServerSignoutFrameCallbackUrlAsync(this HttpContext context, LogoutMessage logoutMessage = null)
        {
            var userSession = context.RequestServices.GetRequiredService<IUserSession>();
            var user = await userSession.GetUserAsync();
            var currentSubId = user?.GetSubjectId();

            LogoutNotificationContext endSessionMsg = null;

            // if we have a logout message, then that take precedence over the current user
            if (logoutMessage != null && (logoutMessage.ClientId.IsPresent() || logoutMessage.ClientIds?.Any() == true))
            {
                if (logoutMessage?.ClientIds?.Any() == true)
                {
                    var clientIds = logoutMessage?.ClientIds;

                    // check if current user is same, since we might have new clients (albeit unlikely)
                    // investigate this case, possible when returned from idp
                    //if (currentSubId == logoutMessage?.SubjectId)
                    //{
                    //    clientIds = clientIds.Union(await userSession.GetClientListAsync()).Distinct();
                    //}

                    endSessionMsg = new LogoutNotificationContext
                    {
                        SubjectId = logoutMessage.SubjectId,
                        SessionId = logoutMessage.SessionId,
                        ClientIds = clientIds,
                    };
                }
            }
            else if (currentSubId != null)
            {
                // see if current user has any clients they need to sign out of
                var clientIds = await userSession.GetClientListAsync();
                if (clientIds.Any())
                {
                    endSessionMsg = new LogoutNotificationContext
                    {
                        SubjectId = currentSubId,
                        SessionId = await userSession.GetSessionIdAsync(),
                        ClientIds = clientIds,
                    };
                }
            }

            if (endSessionMsg != null)
            {
                var clock = context.RequestServices.GetRequiredService<IClock>();
                var urls = context.RequestServices.GetRequiredService<IServerUrls>();
                var msg = new Message<LogoutNotificationContext>(endSessionMsg, clock.UtcNow.UtcDateTime);

                var endSessionMessageStore = context.RequestServices.GetRequiredService<IMessageStore<LogoutNotificationContext>>();
                var id = await endSessionMessageStore.WriteAsync(msg);

                var signoutIframeUrl = urls.BaseUrl.EnsureTrailingSlash() + Constants.ProtocolRoutePaths.EndSessionCallback;
                signoutIframeUrl = signoutIframeUrl.AddQueryString(Constants.DefaultRoutePathParams.EndSessionCallback, id);

                return signoutIframeUrl;
            }

            // no sessions, so nothing to cleanup
            return null;
        }
    }
}