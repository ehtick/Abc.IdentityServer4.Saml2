using System;

namespace Abc.IdentityServer.Extensions
{
    internal class StubClock : IClock
    {
        public Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;
        public DateTimeOffset UtcNow => new DateTimeOffset(UtcNowFunc());
    }
}