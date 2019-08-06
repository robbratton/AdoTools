using System;
using System.Linq;
using FluentAssertions;
using Microsoft.TeamFoundation.Build.WebApi;
using NUnit.Framework;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Entities;
using Upmc.DevTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Upmc.DevTools.VsTs.Tests.ToolTests
{
    [TestFixture]
    public class BuildToolTests
    {
        private PerformanceConfiguration _pc;

        [Test]
        [Category("Integration")]
        public void Constructor_Succeeds()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            Assert.That(realBuildTool, Is.Not.Null);
        }

        [Test]
        public static void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.That(() => new BuildTool(null), Throws.ArgumentException);
        }

        [Test]
        [Repeat(2)]
        [Category("Integration")]
        public void GetBuildDefinition_Succeeds()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            var definitions = realBuildTool.GetBuildDefinitionReferences();

            var random = new Random(DateTime.Now.Millisecond);

            var randomDefinition = definitions.Skip(random.Next(1, definitions.Count()) - 1).First();

            var result = realBuildTool.GetBuildDefinition(randomDefinition.Id) as BuildDefinitionReference;

            Assert.That(result, Is.Not.Null);

            result.Should().BeEquivalentTo(
                randomDefinition,
                options => options
                    .IncludingNestedObjects() // Nested objects should be equivalent.
                    .Excluding(x => x.Links) // Links are specific to each item and won't match.
                //.Excluding(x => x.BuildNumberFormat)
                //.Excluding(x => x.Comment)
                //.Excluding(x => x.Description)
                //.Excluding(x => x.DropLocation)
                //.Excluding(x => x.JobAuthorizationScope)
                //.Excluding(x => x.JobTimeoutInMinutes)
                //.Excluding(x => x.JobCancelTimeoutInMinutes)
                //.Excluding(x => x.BadgeEnabled)
                //.Excluding(x => x.Steps)
                //.Excluding(x => x.Process)
                //.Excluding(x => x.Options)
                //.Excluding(x => x.Repository)
                //.Excluding(x => x.ProcessParameters)
                //.Excluding(x => x.Triggers)
                //.Excluding(x => x.Variables)
                //.Excluding(x => x.VariableGroups)
                //.Excluding(x => x.Counters)
                //.Excluding(x => x.Demands)
                //.Excluding(x => x.RetentionRules)
                //.Excluding(x => x.Properties)
                //.Excluding(x => x.Tags)
            );

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }

        [Test]
        [Category("Integration")]
        public void GetBuildDefinition_Throws()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            Assert.That(
                () => realBuildTool.GetBuildDefinition(
                    int.MaxValue),
                Throws.TypeOf<AggregateException>().With.InnerException.TypeOf<DefinitionNotFoundException>());
        }

        [Test]
        [Category("Integration")]
        public void GetBuildDefinitionReferences__by_name_Succeeds()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            var result = realBuildTool.GetBuildDefinitionReferences("Upmc.DevTools.Vsts-PullRequest");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            Console.WriteLine(result.DumpValues(0));
        }

        [Test]
        [Category("Integration")]
        public void GetBuildDefinitionReferences_Succeeds()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            var result = realBuildTool.GetBuildDefinitionReferences();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            Console.WriteLine($"Found {result.Count()} build definition references");
            Console.WriteLine("");

            Console.WriteLine(result.DumpValues(0));
        }

        [Test]
        [Category("Integration")]
        public void GetBuildDefinitions_by_name_Succeeds()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            var result = realBuildTool.GetBuildDefinitions("Upmc.DevTools.Vsts-PullRequest");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            Console.WriteLine(result.DumpValues(0));
        }

        [Test]
        [Category("Integration")]
        public void GetBuildDefinitions_Succeeds()
        {
            var realBuildTool = Helpers.SetUpRealBuildTool(out _, _pc);

            var result = realBuildTool.GetBuildDefinitions();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            Console.WriteLine($"Found {result.Count()} build definitions");
            Console.WriteLine("");

            Console.WriteLine(result.DumpValues(0));
        }

        [SetUp]
        public void SetUp()
        {
            _pc = Helpers.SetUpPerformanceConfiguration(retryDelay: 20);
        }
    }
}