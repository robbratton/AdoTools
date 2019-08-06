using System;
using NUnit.Framework;
using AdoTools.Common.Extensions;
// ReSharper disable UnusedMember.Global

namespace AdoTools.Common.Tests.Extensions
{
    [TestFixture]
    public static class DumpValuesExtensionTests
    {
        /// <summary>
        ///     Class to be used as input to DumpValues
        /// </summary>
        private class TestClass
        {
#pragma warning disable 414
            /// <summary>
            ///     Not a property
            /// </summary>
            private readonly string _privateField = "This shouldn't be shown.";
#pragma warning restore 414

#pragma warning disable 414
            /// <summary>
            ///     Not a property
            /// </summary>
            public string PublicField = "This shouldn't be shown.";
#pragma warning restore 414

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="name"></param>
            public TestClass(string name)
            {
                Name = name;
                Items = new[] {"test1", "test2"};
                Date = DateTime.Now;
            }

            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Name { get; }

            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string[] Items { get; }

            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public DateTime Date { get; }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Class to override normal handlers used by DumpValues
        /// </summary>
        private class TestHandlers : VariableDumpHandlers
        {
            public override string GetIndent(int indent)
            {
                return $"[indent{indent}]";
            }
        }

        [Test]
        public static void DumpValues_With_Class()
        {
            const string myName = "My Name";
            var item = new TestClass(myName);

            var result = item.DumpValues(0);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            Assert.That(result.Contains("Name"), Is.True);
            Assert.That(result.Contains("Items"), Is.True);
            Assert.That(result.Contains("Date"), Is.True);
            Assert.That(result.Contains("be shown"), Is.False);

            Assert.That(result.Contains(myName), Is.True);

            Console.WriteLine(result);
        }

        [Test]
        public static void DumpValues_With_CustomClass()
        {
            const string myName = "My Name";
            var item = new TestClass(myName);
            var myTestHandlers = new TestHandlers();

            var result = item.DumpValues(0, handlers: myTestHandlers);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            Assert.That(result.Contains("Name"), Is.True);
            Assert.That(result.Contains("Items"), Is.True);
            Assert.That(result.Contains("Date"), Is.True);
            Assert.That(result.Contains("be shown"), Is.False);
            Assert.That(result.Contains("[indent"), Is.True);

            Assert.That(result.Contains(myName), Is.True);

            Console.WriteLine(result);
        }

        [Test]
        public static void DumpValues_With_Null()
        {
            TestClass item = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var result = item.DumpValues(0);

            Assert.That(result, Is.EqualTo("(null)"));

            Console.WriteLine(result);
        }
    }
}