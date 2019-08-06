using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Moq;
using NUnit.Framework;
using AdoTools.Common;
using AdoTools.Common.Extensions;
using AdoTools.Ado.Entities;
using AdoTools.Ado.SourceTools;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.Ado.Tests.SourceToolTests
{
    [TestFixture]
    public class VcSourceToolTests
    {
        private static VcSourceTool SetUpFake()
        {
            var vsTsTool = new VsTsTool(
                TestData.TokenFake,
                TestData.OrganizationFake,
                TestData.ProjectFake);

            return new VcSourceTool(vsTsTool);
        }

        private static VcSourceTool SetUpIntegration()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            return new VcSourceTool(vsTsTool);
        }

        private static VcSourceTool SetUpMockSourceTool(
            out Mock<VsTsTool> vsTsToolMock,
            out Mock<TfvcHttpClient> tfVcClientMock)
        {
            var temp = new Mock<TfvcHttpClient>(
                new Uri("https://www.google.com"),
                new VssCredentials());
            temp.Setup(
                    x => x
                        .CreateChangesetAsync(
                            It.IsAny<TfvcChangeset>(),
                            It.IsAny<string>(),
                            It.IsAny<object>(),
                            It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new TfvcChangesetRef()));
            var lazyTfvcClientMock = new Lazy<TfvcHttpClient>(() => temp.Object);

            vsTsToolMock = new Mock<VsTsTool>(
                TestData.TokenFake,
                TestData.OrganizationFake,
                TestData.ProjectFake,
                null,
                null);
            vsTsToolMock.SetupGet(y => y.TfVcClient).Returns(lazyTfvcClientMock);

            tfVcClientMock = temp;

            return new VcSourceTool(vsTsToolMock.Object);
        }

        [Test]
        public static void ChangeItemContent_Succeeds()
        {
            var vcSourceTool = SetUpMockSourceTool(out var vsTsToolMock, out var tfVcClientMock);

            var sourceInformation = new SourceInformation(SourceType.TfsVc, "$/Apollo/junkPath", true);
            var existingItem = new TfvcItem();
            Assert.That(
                () => vcSourceTool.ChangeItemContent(sourceInformation, existingItem, "junk content"),
                Throws.Nothing);

            vsTsToolMock.VerifyGet(x => x.TfVcClient);
            tfVcClientMock.Verify(
                x => x.CreateChangesetAsync(
                    It.IsAny<TfvcChangeset>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public static void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.That(() => new VcSourceTool(null), Throws.ArgumentException);
        }

        [Test]
        public static void DeleteItem_Succeeds()
        {
            var vcSourceTool = SetUpMockSourceTool(out var vsTsToolMock, out var tfVcClientMock);

            var sourceInformation = new SourceInformation(SourceType.TfsVc, "$/Apollo/junkPath", true);
            var existingItem = new TfvcItem();
            Assert.That(() => vcSourceTool.DeleteItem(sourceInformation, existingItem), Throws.Nothing);

            vsTsToolMock.VerifyGet(x => x.TfVcClient);
            tfVcClientMock.Verify(
                x => x.CreateChangesetAsync(
                    It.IsAny<TfvcChangeset>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Category("Integration")]
        [Test]
        public static void GetItem_ReturnsNullWhenNotFound()
        {
            // Arrange
            var sourceTool = SetUpIntegration();

            var sourceInformation = new SourceInformation(SourceType.TfsVc, "$/A/B/C/xxx.txt", false);

            // Act
            var result = sourceTool.GetItem(sourceInformation);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Category("Integration")]
        [Test]
        public static void GetItemContent_ReturnsNullWhenNotFound()
        {
            // Arrange
            var sourceTool = SetUpIntegration();

            var sourceInformation = new SourceInformation(SourceType.TfsVc, "$/A/B/C/xxx.txt", false);

            // Act
            var result = sourceTool.GetItemContent(sourceInformation);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Category("Integration")]
        [Test]
        public static void GetItems_Succeeds()
        {
            // Arrange
            var sourceTool = SetUpIntegration();

            var sourceInformation = new SourceInformation(
                SourceType.TfsVc,
                "$/Apollo/HP/Source/Tools/DependencyGrapher",
                true);

            // Act
            var result = sourceTool.GetItems(sourceInformation);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(2));

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }

        [Category("Integration")]
        [Test]
        public static void GetItems_Throws_WhenNotFound()
        {
            // Arrange
            var sourceTool = SetUpIntegration();

            var sourceInformation = new SourceInformation(SourceType.TfsVc, "$/JunkProject/A/B/C", true);

            // Act
            Assert.That(
                () => sourceTool.GetItems(sourceInformation),
                Throws.TypeOf<AggregateException>()
                //.With.InnerException.TypeOf<Microsoft.VisualStudio.Services.Common.VssServiceException>()
            );
        }

        [Test]
        public void Map([Values] bool isFolder)
        {
            var sourceTool = SetUpFake();

            var input = new TfvcItem
            {
                Path = "testPath",
                IsFolder = isFolder
            };

            var result = sourceTool.Map(input, "repository", "branch");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SourcePath, Is.EqualTo(input.Path));
            Assert.That(result.IsDirectory, Is.EqualTo(input.IsFolder));
        }

        [Test]
        public void UpdateSourceInformation_Succeeds([Values(true, false)] bool isDirectory)
        {
            var sourceInfo = new SourceInformation(SourceType.TfsVc, "sourcePath", isDirectory);
            var item = new TfvcItem
            {
                Path = "itemPath"
            };

            var sourceTool = SetUpFake();

            sourceTool.UpdateSourceInformation(sourceInfo, item);

            Assert.That(sourceInfo, Is.Not.Null);
            Assert.That(sourceInfo.GitRepositoryName, Is.EqualTo(sourceInfo.GitRepositoryName));
            Assert.That(sourceInfo.GitBranch, Is.EqualTo(sourceInfo.GitBranch));
            Assert.That(sourceInfo.SourcePath, Is.EqualTo(item.Path));
            Assert.That(sourceInfo.SourceType, Is.EqualTo(sourceInfo.SourceType));
        }
    }
}