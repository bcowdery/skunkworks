using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Moq;

namespace PortAuthority.Test.Utils
{
    /// <summary>
    /// Moq extension methods for working with MassTransit interfaces.
    /// </summary>
    public static class MassTransitTestExtensions
    {
        /// <summary>
        /// Configures a <see cref="ISendEndpoint"/> mock to expect a expected. Returned mock can be verified.
        /// </summary>
        /// <param name="mockEndpointProvider"></param>
        /// <param name="expected"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public static Mock<ISendEndpoint> SetupMessage<TMessage>(this Mock<ISendEndpointProvider> mockEndpointProvider, object expected)
            where TMessage : class
        {
            if (!EndpointConvention.TryGetDestinationAddress<TMessage>(out var destinationAddress))
            {
                // this is just a mock, so the real value shouldn't matter here as long as we're not mixing
                // integration tests and mock tests in the same suite. Use the same convention as the
                // MassTransitInMemoryTestHarness just in case...
                EndpointConvention.Map<TMessage>(new Uri("http://localhost/input-queue"));
            }
         
            var mockEndpoint = new Mock<ISendEndpoint>();
       
            mockEndpointProvider
                .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                .Returns(Task.FromResult(mockEndpoint.Object))
                .Verifiable();
            
            mockEndpoint
                .Setup(x => x.Send<TMessage>(AnonymousType.Matches<TMessage>(expected), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockEndpoint;
        }
    }

    /// <summary>
    /// Internal utility for comparing anonymous types
    /// </summary>
    public static class AnonymousType
    {
        /// <summary>
        /// Constructs a matcher that compares properties between anonymous types.
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static TMessage Matches<TMessage>(object expected)
            where TMessage : class
        {
            return Match.Create<TMessage>(
                CreateMatcher<TMessage>(expected),
                () => AnonymousType.Matches<TMessage>(expected));
        }

        /// <summary>
        /// Constructs a predicate function that matches to an expected message value.
        /// </summary>
        /// <param name="expectedMessage"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        private static Func<object, Type, bool> CreateMatcher<TMessage>(object expectedMessage) =>
            (actualMessage, parameterType) =>
            {
                // actual message is of the same type as the generic TMessage
                if (actualMessage == null || !parameterType.IsInstanceOfType(actualMessage))
                {
                    return false;
                }
                
                // anonymous objects have a hidden generated type and cannot be compared using Equals() unless
                // both objets were generated from the same method in the same assembly. Compare objects using 
                // property matching so that we can match any two objects with a similar structure.
                var expectedProperties = expectedMessage.GetType()
                    .GetProperties()
                    .ToDictionary(x => x.Name, x => x.GetValue(expectedMessage));                
                
                var actualProperties = actualMessage.GetType()
                    .GetProperties()
                    .ToDictionary(x => x.Name, x => x.GetValue(actualMessage));
                
                foreach (var expectedProp in expectedProperties)
                {
                    if (!actualProperties.ContainsKey(expectedProp.Key))
                        return false;
                    if (expectedProp.Value == null && actualProperties[expectedProp.Key] != null)
                        return false;
                    if (expectedProp.Value != null && !expectedProp.Value.Equals(actualProperties[expectedProp.Key]))
                        return false;
                }
                
                return true;
            };
    }
}
