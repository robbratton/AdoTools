using System;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using Upmc.DevTools.Common;
using Upmc.DevTools.VsTs.SourceTools;

namespace Upmc.DevTools.VsTs.Tests.SourceToolTests
{
    [TestFixture]
    [Explicit]
    public class GitToolCommandLineTests
    {
        [SetUp]
        public void SetUp()
        {
            _commandLineToolSucceedsMock = new Mock<ICommandLineTool>();
            _commandLineToolSucceedsMock
                .Setup(
                    x =>
                        x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Process.Start("cmd", "/c echo Hi > nul"));

            _commandLineToolFailsMock = new Mock<ICommandLineTool>();
            _commandLineToolFailsMock
                .Setup(
                    x =>
                        x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                // ReSharper disable once StringLiteralTypo
                .Returns(Process.Start("cmd", "/c dir alkdjshalkjfhaskflhsakflhsfhfkskhlsfsahsakshakshsas > nul"));

            _commandLineToolThrowsMock = new Mock<ICommandLineTool>();
            _commandLineToolThrowsMock
                .Setup(
                    x =>
                        x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new JunkException());
        }

        [TestCase(null, "path")]
        [TestCase(GoodUriString, null)]
        [TestCase(GoodUriString, "")]
        public void Constructor_Throws(string serverUri, string testPath)
        {
            Uri uri = null;

            if (serverUri != null)
            {
                uri = new Uri(serverUri);
            }

            // ReSharper disable once ObjectCreationAsStatement
            Assert.That(
                () => new GitToolCommandLine(_commandLineToolSucceedsMock.Object, uri, testPath),
                Throws.ArgumentException);
        }

        private const string GoodUriString = "http://www.google.com";

        private Mock<ICommandLineTool> _commandLineToolFailsMock;

        private Mock<ICommandLineTool> _commandLineToolSucceedsMock;

        private Mock<ICommandLineTool> _commandLineToolThrowsMock;

        [Test]
        public void Add_Fails()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolFailsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Add(), Throws.TypeOf<InvalidOperationException>());
            _commandLineToolFailsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Add_Succeeds()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolSucceedsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Add(), Throws.Nothing);
            _commandLineToolSucceedsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Add_Throws()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolThrowsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Add(), Throws.TypeOf<JunkException>());
            _commandLineToolThrowsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Branch_Fails()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolFailsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Branch("testBranch"), Throws.TypeOf<InvalidOperationException>());
            _commandLineToolFailsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Branch_Succeeds()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolSucceedsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Branch("testBranch"), Throws.Nothing);
            _commandLineToolSucceedsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Branch_Throws()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolThrowsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Branch("testBranch"), Throws.TypeOf<JunkException>());
            _commandLineToolThrowsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void CheckOut_Fails()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolFailsMock.Object, serverUri, basePath);

            Assert.That(() => tool.CheckOut("testBranch"), Throws.TypeOf<InvalidOperationException>());
            _commandLineToolFailsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void CheckOut_Succeeds([Values] bool branchProvided)
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolSucceedsMock.Object, serverUri, basePath);

            var branchName = branchProvided
                ? "testBranch"
                : null;

            Assert.That(() => tool.CheckOut(branchName), Throws.Nothing);
            _commandLineToolSucceedsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Clone_Fails()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolFailsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Clone(), Throws.TypeOf<InvalidOperationException>());
            _commandLineToolFailsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Clone_Succeeds()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var tool = new GitToolCommandLine(_commandLineToolSucceedsMock.Object, serverUri, basePath);

            Assert.That(() => tool.Clone(), Throws.Nothing);
            _commandLineToolSucceedsMock.Verify(
                x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);
        }

        [Test]
        public void Constructor_Succeeds()
        {
            var serverUri = new Uri(GoodUriString);
            const string basePath = "/";

            var result = new GitToolCommandLine(_commandLineToolSucceedsMock.Object, serverUri, basePath);

            Assert.That(result, Is.Not.Null);
        }
    }

    // ReSharper disable UnusedMember.Global
    /// <inheritdoc />
    public class JunkException : Exception
    {
        public JunkException()
        {
        }

        public JunkException(string message) : base(message)
        {
        }

        public JunkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    // ReSharper restore UnusedMember.Global
}