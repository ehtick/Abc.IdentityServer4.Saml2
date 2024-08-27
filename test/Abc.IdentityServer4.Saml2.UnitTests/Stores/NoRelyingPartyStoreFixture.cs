using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Abc.IdentityServer.Saml2.Stores.UnitTests
{
    public class NoRelyingPartyStoreFixture
    {
        private NoRelyingPartyStore _target;

        public NoRelyingPartyStoreFixture()
        {
            _target = new NoRelyingPartyStore();
        }

        [Fact()]
        public async Task FindRelyingPartyByRealmTest()
        {
            var relyingParty = await _target.FindRelyingPartyByEntityIdAsync("");
            relyingParty.Should().BeNull();
        }
    }
}