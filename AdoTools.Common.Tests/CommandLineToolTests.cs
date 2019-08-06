using System;
using NUnit.Framework;

namespace AdoTools.Common.Tests
{
    [TestFixture]
    public static class CommandLineToolTests
    {
        [TestCase(null, null, null, 1)]
        [TestCase("dir", null, null, -11)]
        [TestCase("dir", null, @"X:\A\B\C\D", 1)]
        public static void ExecuteCommand_Throws(string command, string args, string workingDirectory, int waitMilliseconds)
        {
            var commandLineTool = new CommandLineTool();

            Assert.That(
                () => commandLineTool.ExecuteCommand(command, args, workingDirectory, waitMilliseconds),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.Contains(" must "));
        }

        [Test]
        public static void Constructor_Succeeds()
        {
            var result = new CommandLineTool();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        [Category("Integration")]
        public static void ExecuteCommand__ReturnsExpectedResult_WhenCommandFails()
        {
            var commandLineTool = new CommandLineTool();

            var result =
                commandLineTool.ExecuteCommand("cmd", "/c dir ThisIsAJunkFileNameAndShouldNotExist", ".", 1000);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasExited, Is.True);
            Assert.That(result.ExitCode, Is.EqualTo(1));
            Assert.That(result.StandardOutput.BaseStream.CanRead, Is.True);
            Assert.That(result.StandardError.BaseStream.CanRead, Is.True);

            var errorOutput = result.StandardError.ReadToEnd();
            var standardOutput = result.StandardOutput.ReadToEnd();

            Assert.That(errorOutput.Length, Is.GreaterThan(0));
            StringAssert.Contains("File Not Found", errorOutput);
            Assert.That(standardOutput.Length, Is.GreaterThan(0));
            StringAssert.Contains("Directory of", standardOutput);
        }

        [Test]
        [Category("Integration")]
        public static void ExecuteCommand_ReturnsExpectedResult_WhenCommandSucceeds()
        {
            var commandLineTool = new CommandLineTool();

            var result = commandLineTool.ExecuteCommand("cmd", "/c echo Hi", ".", 1000);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasExited, Is.True);
            Assert.That(result.ExitCode, Is.EqualTo(0));
            Assert.That(result.StandardOutput.BaseStream.CanRead, Is.True);
            Assert.That(result.StandardError.BaseStream.CanRead, Is.True);

            var errorOutput = result.StandardError.ReadToEnd();
            var standardOutput = result.StandardOutput.ReadToEnd();

            Assert.That(errorOutput.Length, Is.EqualTo(0));
            Assert.That(standardOutput.Length, Is.GreaterThan(0));
            StringAssert.Contains("Hi", standardOutput);
        }
    }
}