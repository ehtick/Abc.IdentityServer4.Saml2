using IdentityServer4.Configuration;
using Abc.IdentityServer.Extensions;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Abc.IdentityServer4.Saml2.Endpoints.Results
{
    internal class LogoutPageResult : IEndpointResult
    {
        private readonly string requestId;
        private readonly string logoutId;
        private IdentityServerOptions _options;
        private IServerUrls _urls;

        public LogoutPageResult(string requestId, string logoutId)
        {
            this.requestId = requestId;
            this.logoutId = logoutId;
        }

        internal LogoutPageResult(IdentityServerOptions options, IServerUrls urls)
        {
            _options = options;
            _urls = urls;
        }

        public Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            var redirectUrl = _options.UserInteraction.LogoutUrl;
            
            if (logoutId != null)
            {
                redirectUrl = redirectUrl.AddQueryString(_options.UserInteraction.LogoutIdParameter, logoutId);
            }

            //if (!string.IsNullOrWhiteSpace(requestId))
            //{
            //    redirectUrl = redirectUrl.AddQueryString(samlOptions.UserInteraction.RequestIdParameter, RequestId);
            //}

            context.Response.Redirect(_urls.GetAbsoluteUrl(redirectUrl));
            return Task.CompletedTask;
        }

        private void Init(HttpContext context)
        {
            _options ??= context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _urls ??= context.RequestServices.GetRequiredService<IServerUrls>();
        }
    }
}
