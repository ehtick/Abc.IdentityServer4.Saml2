﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Http
{
    internal class MockHttpContextAccessor : IHttpContextAccessor
    {
        private HttpContext _context = new DefaultHttpContext();
        public MockAuthenticationService AuthenticationService { get; set; } = new MockAuthenticationService();

        public MockAuthenticationSchemeProvider Schemes { get; set; } = new MockAuthenticationSchemeProvider();

        public MockHttpContextAccessor(
            IdentityServerOptions options = null,
            IUserSession userSession = null,
            IMessageStore<LogoutNotificationContext> endSessionStore = null,
            IServerUrls serverUrls = null)
        {
            options = options ?? TestIdentityServerOptions.Create();

            var services = new ServiceCollection();
            services.AddSingleton(options);

            services.AddSingleton<IAuthenticationSchemeProvider>(Schemes);
            services.AddSingleton<IAuthenticationService>(AuthenticationService);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = Schemes.Default;
            });

#if NET8_0_OR_GREATER
            services.AddTransient<IClock, Abc.IdentityServer.Extensions.StubClock>();
#endif

            if (userSession == null)
            {
                services.AddScoped<IUserSession, DefaultUserSession>();
            }
            else
            {
                services.AddSingleton(userSession);
            }

            if (serverUrls == null)
            {
                services.AddScoped<IServerUrls, DefaultServerUrls>();
            }
            else
            {
                services.AddSingleton(serverUrls);
            }

            if (endSessionStore == null)
            {
                services.AddTransient<IMessageStore<LogoutNotificationContext>, ProtectedDataMessageStore<LogoutNotificationContext>>();
            }
            else
            {
                services.AddSingleton(endSessionStore);
            }

            _context.RequestServices = services.BuildServiceProvider();
        }

        public HttpContext HttpContext
        {
            get
            {
                return _context;
            }

            set
            {
                _context = value;
            }
        }
    }
}
