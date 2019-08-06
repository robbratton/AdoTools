using System;
using System.IO;
using NUnit.Framework;

namespace AdoTools.Common.Tests
{
    [TestFixture]
    public static class IgnoreExceptionsHelperTests
    {
        private static string ExampleMethod(Type exceptionType)
        {
            if (exceptionType != typeof(string))
            {
                var toThrow = Activator.CreateInstance(exceptionType);

                throw (Exception) toThrow;
            }

            return Guid.NewGuid().ToString();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ExampleClass
        {
            public ExampleClass(Type exceptionType)
            {
                if (exceptionType != typeof(string))
                {
                    var toThrow = Activator.CreateInstance(exceptionType);

                    throw (Exception) toThrow;
                }

                Value = Guid.NewGuid().ToString();
            }

            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            // ReSharper disable once MemberCanBePrivate.Local
            public string Value { get; set; }
        }

        [Test]
        public static void CheckException_HandlesAggregateExceptions()
        {
            var innerException = new InvalidOperationException("Testing Inner");
            var input = new AggregateException("Aggregate", innerException);

            Assert.That(() => IgnoreExceptionsHelper.CheckException(
                    input,
                    new[] {typeof(ArgumentException)}),
                Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Testing Inner"));
        }

        [Test]
        public static void DoMethodIgnoringExceptions_Returns_Null_WithIgnoredException1()
        {
            var result = IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                new Func<Type, string>(ExampleMethod),
                new[] {typeof(ArgumentException)},
                typeof(ArgumentException));

            Assert.That(result, Is.Null);
        }

        [Test]
        public static void DoMethodIgnoringExceptions_Returns_Null_WithIgnoredException2()
        {
            var result = IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                new Func<string, string>(File.ReadAllText),
                new[] {typeof(FileNotFoundException)},
                "junk filename");

            Assert.That(result, Is.Null);
        }

        [Test]
        public static void DoMethodIgnoringExceptions_Returns_Value_WithNoException1()
        {
            var result = IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                new Func<Type, string>(ExampleMethod),
                new[] {typeof(ArgumentException)},
                typeof(string));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public static void DoMethodIgnoringExceptions_Throw_WithBadException()
        {
            Assert.That(
                () => IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                    new Func<Type, string>(ExampleMethod),
                    new[] {typeof(string)},
                    typeof(InvalidOperationException)),
                Throws.ArgumentException
            );
        }

        [Test]
        public static void DoMethodIgnoringExceptions_Throws_WithNonIgnoredException1()
        {
            Assert.That(
                () => IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                    new Func<Type, string>(ExampleMethod),
                    new[] {typeof(ArgumentException)},
                    typeof(InvalidOperationException)),
                Throws.InvalidOperationException
            );
        }

        [Test]
        public static void DoMethodIgnoringExceptions_Throws_WithNonIgnoredException2()
        {
            Assert.That(
                () => IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                    new Func<string, string>(File.ReadAllText),
                    new[] {typeof(ArgumentNullException)},
                    "junk filename"),
                Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public static void DoNewIgnoringExceptions_Returns_Null_WithIgnoredException1()
        {
            var result = IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<ExampleClass>(
                new[] {typeof(ArgumentException)},
                typeof(ArgumentException));

            Assert.That(result, Is.Null);
        }

        [Test]
        public static void DoNewIgnoringExceptions_Returns_Null_WithIgnoredException2()
        {
            var result = IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<Uri>(
                new[] {typeof(UriFormatException)},
                "junk uri");

            Assert.That(result, Is.Null);
        }

        [Test]
        public static void DoNewIgnoringExceptions_Returns_Value_WithNoException1()
        {
            var result = IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<ExampleClass>(
                new[] {typeof(ArgumentException)},
                typeof(string));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Length, Is.GreaterThan(0));
        }

        [Test]
        public static void DoNewIgnoringExceptions_Returns_Value_WithNoException2()
        {
            var result = IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<Uri>(
                new[] {typeof(ArgumentException)},
                "http://www.google.com");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AbsoluteUri, Is.Not.Null);
            Assert.That(result.AbsoluteUri.Length, Is.GreaterThan(0));
        }

        [Test]
        public static void DoNewIgnoringExceptions_Throws_WithBadException()
        {
            Assert.That(
                () => IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<ExampleClass>(
                    new[] {typeof(string)},
                    typeof(InvalidOperationException)),
                Throws.ArgumentException
            );
        }

        [Test]
        public static void DoNewIgnoringExceptions_Throws_WithNonIgnoredException1()
        {
            Assert.That(
                () => IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<ExampleClass>(
                    new[] {typeof(ArgumentException)},
                    typeof(InvalidOperationException)),
                Throws.InvalidOperationException
            );
        }

        [Test]
        public static void DoNewIgnoringExceptions_Throws_WithNonIgnoredException2()
        {
            Assert.That(
                () => IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<Uri>(
                    new[] {typeof(ArgumentException)},
                    "Junk Uri"),
                Throws.TypeOf<UriFormatException>()
            );
        }
    }
}