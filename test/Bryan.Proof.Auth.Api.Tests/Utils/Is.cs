using FluentAssertions.Equivalency;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using Xunit.Sdk;

namespace Bryan.Proof.Auth.Api.Tests.Utils;
internal class Is
{
    public static ref T? Equivalent<T>(T? obj, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config)
        => ref ArgumentMatcher.Enqueue(new IsEquivalent<T>(obj, config));

    internal class IsEquivalent<T>: IArgumentMatcher<T>, IDescribeNonMatches
    {
        private readonly T? _otherObj;
        private readonly Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> _config;
        private string? _error;

        public IsEquivalent(T? otherObj, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config)
        {
            _otherObj = otherObj;
            _config = config;
        }

        public string DescribeFor(object? argument) => _error!;

        public bool IsSatisfiedBy(T? argument)
        {
            try
            {
                argument.Should().BeEquivalentTo(_otherObj, config: _config!);
                return true;
            }
            catch (XunitException ex) when (ex.Source == "FluentAssertions")
            {
                _error = ex.Message;
                return false;
            }
        }
    }
}
