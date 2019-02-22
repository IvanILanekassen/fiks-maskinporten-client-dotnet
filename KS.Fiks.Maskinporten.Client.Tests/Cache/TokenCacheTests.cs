using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Ks.Fiks.Maskinporten.Client.Tests.Cache
{
    public class TokenCacheTests
    {
        private TokenCacheFixture _fixture;

        public TokenCacheTests()
        {
            _fixture = new TokenCacheFixture();
        }

        [Fact]
        public async Task ReturnsValueFromGetterAtFirstCall()
        {
            var sut = _fixture.CreateSut();

            var expectedValue = "a value";

            var actualValue = await sut.GetToken("key", () => Task.FromResult(expectedValue));

            actualValue.Should().Be(expectedValue);
        }

        [Fact]
        public async Task ReturnsValueFromCacheInSecondCallIfWithinTimeLimit()
        {
            var sut = _fixture.WithExpirationTime(TimeSpan.FromMinutes(1)).CreateSut();

            var expectedValue = "a value";

            var firstValue = await sut.GetToken("key", () => Task.FromResult(expectedValue));
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            var secondValue = await sut.GetToken("key", () => Task.FromResult("should not get"));

            secondValue.Should().Be(expectedValue);
        }

        [Fact]
        public async Task ReturnsNewValueIfCallIsOutsideTimeLimit()
        {
            var sut = _fixture.WithExpirationTime(TimeSpan.FromMilliseconds(10)).CreateSut();

            var expectedValue = "a value";

            var firstValue = await sut.GetToken("key", () => Task.FromResult("should not get"));
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            var secondValue = await sut.GetToken("key", () => Task.FromResult(expectedValue));

            secondValue.Should().Be(expectedValue);
        }
    }
}