using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Extensions;
using Upmc.DevTools.VsTs.Entities;
using Upmc.DevTools.VsTs.SourceTools;

// ReSharper disable PossibleMultipleEnumeration

namespace Upmc.DevTools.VsTs.Tests.ToolTests
{
    [TestFixture]
    public static class FilesystemSourceToolTests
    {
        [Test]
        [Category("Integration")]
        public static void DeleteItem_Throws_WithBadPath()
        {
            // Arrange
            var sourceTool = new FilesystemSourceTool();

            var path = Path.Combine(
                "C:",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid() + ".txt");

            var sourceInformation = new SourceInformation(SourceType.Filesystem, path, false);

            // Act
            Assert.That(
                () => sourceTool.DeleteItem(sourceInformation, null),
                Throws.TypeOf<DirectoryNotFoundException>());
        }

        [Category("Integration")]
        [Test]
        public static void GetItem_ReturnsNullWhenNotFound()
        {
            // Arrange
            var sourceTool = new FilesystemSourceTool();

            var sourceInformation = new SourceInformation(SourceType.Filesystem, @"x:\A\B\C\xxx.txt", false);

            // Act
            var result = sourceTool.GetItem(sourceInformation);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        [Category("Integration")]
        public static void GetItemContent()
        {
            // Arrange
            var sourceTool = new FilesystemSourceTool();

            var sourceInformation = new SourceInformation(SourceType.Filesystem, @"x:\A\B\C\xxx.txt", false);

            // Act
            var result = sourceTool.GetItemContent(sourceInformation);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Category("Integration")]
        [Test]
        public static void GetItems_Succeeds([Values] bool isDirectory)
        {
            // Arrange
            var sourceTool = new FilesystemSourceTool();

            var sourceInformation = isDirectory
                ? new SourceInformation(SourceType.Filesystem, @"c:\git\Upmc.DevTools.Vsts", true)
                : new SourceInformation(
                    SourceType.Filesystem,
                    @"c:\git\Upmc.DevTools.Vsts\Upmc.DevTools.Vsts\Upmc.DevTools.Vsts.csproj",
                    false);

            // Act
            var result = sourceTool.GetItems(
                sourceInformation,
                isDirectory
                    ? "*.csproj"
                    : null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(
                result.Count(),
                Is.EqualTo(
                    isDirectory
                        ? 2
                        : 1));

            try
            {
                var myHandlers = new VariableDumpHandlers();
                Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Console.WriteLine(exception);
            }
        }

        [Category("Integration")]
        [Test]
        public static void GetItems_Throws_WhenNotFound()
        {
            // Arrange
            var sourceTool = new FilesystemSourceTool();

            var sourceInformation = new SourceInformation(SourceType.Filesystem, @"x:\A\B\C", true);

            // Act
            Assert.That(
                () => sourceTool.GetItems(sourceInformation),
                Throws.TypeOf<DirectoryNotFoundException>());
        }

        [Test]
        public static void Map()
        {
            var sourceTool = new FilesystemSourceTool();

            var input = new FileInfo(@"c:\junk\");

            var result = sourceTool.Map(input);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SourcePath, Is.EqualTo(input.FullName));
            Assert.That(result.IsDirectory, Is.EqualTo(true));
        }

        [Test]
        public static void UpdateSourceInformation_Succeeds([Values(true, false)] bool isDirectory)
        {
            var sourceInfo = new SourceInformation(SourceType.Filesystem, "sourcePath", isDirectory);
            var item = new FileInfo("itemPath");

            var x = new FilesystemSourceTool();
            x.UpdateSourceInformation(sourceInfo, item);

            Assert.That(sourceInfo.SourcePath, Is.EqualTo(item.FullName));
        }
    }
}