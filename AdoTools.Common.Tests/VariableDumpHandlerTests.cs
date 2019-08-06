using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AdoTools.Common.Tests
{
    [TestFixture]
    public static class VariableDumpHandlerTests
    {
        [TestCase("Hello", "\"Hello\"")]
        [TestCase(123, "123")]
        [TestCase(null, "(null)")]
        public static void HandleItem_ReturnsExpectedResult(object input, string expectedOutput)
        {
            var result = new VariableDumpHandlers().HandleItem(input, 0);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [TestCase(typeof(string), typeof(string))]
        [TestCase(typeof(string), typeof(int))]
        [TestCase(typeof(string), typeof(float))]
        [TestCase(typeof(string), typeof(double))]
        [TestCase(typeof(string), typeof(bool))]
        [TestCase(typeof(string), typeof(object))]
        [TestCase(typeof(int), typeof(string))]
        //[TestCase(typeof(object), typeof(object))]
        public static void HandleItem_ForDictionary_ReturnsResult(Type type1, Type type2)
        {
            var dictType = typeof(Dictionary<,>).MakeGenericType(type1, type2);
            var input = Activator.CreateInstance(dictType);

            var result = new VariableDumpHandlers().HandleItem(input, 0);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));

            // todo Use expectedResult?
        }

        [Test]
        public static void HandleItem_ForDictionary_WithValues_ReturnsResult()
        {
            var input = new Dictionary<string, string> {{"a", "1"}, {"b", "2"}};

            var result = new VariableDumpHandlers().HandleItem(input, 0);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));

            // todo Use expectedResult?
        }

        [Test]
        public static void GetIndent_Throws(
            [Values(-1, 100)] int input
        )
        {
            var x = new VariableDumpHandlers(10, " ");
            Assert.That(() => x.GetIndent(input), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
