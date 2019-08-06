using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using AdoTools.Common.Entities;
using AdoTools.Common.Tests.Helpers;

namespace AdoTools.Common.Tests.Entities
{
    [TestFixture]
    public class CheckResultsTests
    {
        [Test]
        public static void Clone_ReturnsExpectedResult()
        {
            var input = CheckResultsGenerator<object>.Generate();

            var result = (CheckResults<object>) input.Clone();

            result.Should().BeEquivalentTo(input);
        }

        [Test]
        public static void Constructor_ReturnsExpectedResult(
            [Values] bool includeError,
            [Values] bool includeSuccess,
            [Values] bool includeWarning,
            [Values] bool includeSuggestion)
        {
            var result =
                CheckResultsGenerator<object>.Generate(includeError, includeSuccess, includeWarning, includeSuggestion);

            Assert.That(result.HasErrors, Is.EqualTo(includeError));
            Assert.That(result.HasSuggestions, Is.EqualTo(includeSuggestion));
            Assert.That(result.HasWarnings, Is.EqualTo(includeWarning));
            Assert.That(result.Passed, Is.EqualTo(!includeError && !includeSuggestion && !includeWarning));
        }

        [Test]
        public void MergeMessages_ReturnsExpectedResult()
        {
            // Arrange
            var original =
                CheckResultsGenerator<object>.Generate(includeSuccess: true); // Keep this one for comparison.
            var result = (CheckResults<object>) original.Clone(); // Clone to working instance.
            result.Should().BeEquivalentTo(original);
            result.Should().NotBeSameAs(original);

            var input = new List<Message<object>>
            {
                MessageGenerator<object>.Generate(MessageLevel.Suggestion),
                MessageGenerator<object>.Generate(MessageLevel.Error),
                MessageGenerator<object>.Generate()
            };

            // Act
            result.Merge(input);

            // Assert
            Assert.That(() => result.Should().BeEquivalentTo(original), Throws.TypeOf<AssertionException>());

            Assert.That(result.Messages.Count, Is.EqualTo(original.Messages.Count + input.Count));
        }

        [Test]
        public static void MergeObject_ReturnsExpectedResult()
        {
            var original =
                CheckResultsGenerator<object>.Generate(includeSuccess: true); // Keep this one for comparison.
            var result = (CheckResults<object>) original.Clone(); // Clone to working instance.
            result.Should().BeEquivalentTo(original);
            result.Should().NotBeSameAs(original);

            var input = CheckResultsGenerator<object>.Generate(true); // Item to be merged.
            input.Should().NotBeSameAs(original);
            input.Should().NotBeSameAs(result);

            result.Merge(input);

            // ReSharper disable ImplicitlyCapturedClosure
            Assert.That(() => result.Should().BeEquivalentTo(original), Throws.TypeOf<AssertionException>());
            Assert.That(() => result.Should().BeEquivalentTo(input), Throws.TypeOf<AssertionException>());
            // ReSharper restore ImplicitlyCapturedClosure

            Assert.That(result.Messages.Count, Is.EqualTo(original.Messages.Count + input.Messages.Count));
        }
    }
}