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
    public static class MassTransitTestExtensions
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

    /// <summary>
    /// Internal utility for comparing anonymous types
    /// </summary>
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
            // Build a predicate that accepts the (actual) object and compares it
            // to the known expected value. Compare individual properties because
            // C# gives each anon object it's own unique type - Equals() will not work here.
            return actual =>
            {
                var expectedProp = expected.GetType()
                    .GetProperties()
                    .ToDictionary(x => x.Name, x => x.GetValue(expected));
                
                var actualProp = actual.GetType()
                    .GetProperties()
                    .ToDictionary(x => x.Name, x => x.GetValue(actual));

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
