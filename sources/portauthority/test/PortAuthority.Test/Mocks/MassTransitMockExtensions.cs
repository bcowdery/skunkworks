using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Moq;
using Moq.Language.Flow;
using Newtonsoft.Json;

namespace PortAuthority.Test.Mocks
{
    /// <summary>
    /// Moq extension methods for working with MassTransit interfaces.
    /// </summary>
    public static class MassTransitMockExtensions
    {
        /// <summary>
        /// Configures a <see cref="ISendEndpoint"/> mock to expect a expected. Returned mock can be verified.
        /// </summary>
        /// <param name="mockSendEndpoint"></param>
        /// <param name="expected"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public static IReturnsResult<ISendEndpoint> SetupMessage<TMessage>(this Mock<ISendEndpoint> mockSendEndpoint, object expected)
            where TMessage : class
        {
            return mockSendEndpoint
                .Setup(x => x.Send<TMessage>(AnonymousType.Matches(expected), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }
    }

    internal static class AnonymousType
    {
        /// <summary>
        /// Constructs a matcher that compares properties between anonymous types.
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static object Matches(object expected)
        {
            return Match.Create(Matcher(expected));
        }

        private static Predicate<object> Matcher(object expected)
        {
            return actual =>
            {
                var expectedProp = expected.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(expected));
                var actualProp = actual.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(actual));

                foreach (var prop in expectedProp)
                {
                    if (!actualProp.ContainsKey(prop.Key))
                        return false;
                    if (prop.Value != null && !prop.Value.Equals(actualProp[prop.Key]))
                        return false;
                }
                return true;
            };
        }        
    }
}
