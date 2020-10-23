using System;
using System.Linq;
using Bogus;
using FluentAssertions;
using NUnit.Framework;
using Shipyard.Contracts;
using Shipyard.Contracts.MessageTypes;
using Shipyard.MessageTypes;

namespace Shipyard.Test.Contracts
{
    public class MessageIdTest 
    {
        [Test]
        public void MessageId_For_Email_Should_Be_Repeatable()
        {
            // arrange
            var email = EmailBuilder.CreateNew()
                .AddTo("bob", "bob@test.com")
                .AddTo("fred", "fred@test.com")
                .SetSubject("Foo bar baz")
                .SetContent("Hello %fname%, Welcome to Shipyard!")
                .Build();
            
            // act
            var messageId = MessageId.CreateNew(email.To, email.Subject, email.Content);
            
            // assert            
            messageId.Value.Should().Be("dYBH4XqQmPti2xeDCsB8RunLQz0");
        }

        [Test]
        public void MessageId_For_MixedTypes_Should_Be_Unique()
        {
            // arrange
            var faker = new Faker();
            var tokens = Enumerable.Range(0, 100)
                .Select(i => new
                {
                    Index = i,
                    Word = faker.Lorem.Word(),
                    Int = faker.System.Random.Int(),
                    Guid = faker.System.Random.Guid()
                })
                .ToArray();
            
            // act
            var messageIds = tokens
                .Select(x => MessageId.CreateNew(x.Word, x.Int, x.Guid))
                .ToArray();
            
            // assert            
            messageIds.Should().OnlyHaveUniqueItems(x => x.Value);
        }
    }
}
