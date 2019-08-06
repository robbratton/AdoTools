using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using NUnit.Framework;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Upmc.DevTools.VsTs.Tests.ToolTests
{
    [TestFixture]
    public class ReleaseToolTests
    {
        // This must exist or tests will fail!
        private const string ReleaseName = "Workstream-DevBranch";

        private void SetUpIntegration()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            _vsTsTool = new VsTsTool(pat);

            _releaseTool = new ReleaseTool(_vsTsTool);
        }

        private VsTsTool _vsTsTool;

        private ReleaseTool _releaseTool;

        [TestCase(null)]
        [TestCase(ReleaseName)]
        [Category("Integration")]
        public void GetReleaseDefinition_Succeeds(string name)
        {
            SetUpIntegration();

            var definitions = _releaseTool.GetReleaseDefinitions();

            ReleaseDefinition randomDefinition;

            if (name == null)
            {
                var random = new Random(DateTime.Now.Millisecond);

                randomDefinition = definitions.Skip(random.Next(1, definitions.Count()) - 1).First();
            }
            else
            {
                // This may fail if this definition is no longer available.
                randomDefinition = definitions.Single(x => x.Name == name);
            }

            Assert.That(randomDefinition, Is.Not.Null);

            var result = _releaseTool.GetReleaseDefinition(randomDefinition.Id);

            Assert.That(result, Is.Not.Null);

            Assert.Multiple(() =>
            {
                //Console.WriteLine(result.DumpValues(0));

                if (name == "HP-R7-TOKEN-Lab")
                {
                    // Note that these may become incorrect if the release definition is edited.
                    Assert.That(result.VariableGroups.Count, Is.EqualTo(4));
                    Assert.That(result.Environments.Count, Is.EqualTo(26));
                }

                result.Should().BeEquivalentTo(
                    randomDefinition
                    , options => options
                        //.Excluding(x => x.Artifacts)
                        .Excluding(x => x.Environments)
                        //.Excluding(x => x.Properties)
                        //.Excluding(x => x.Triggers)
                        //.Excluding(x => x.Variables)
                        .Excluding(x => x.VariableGroups)
#pragma warning disable 618
                        .Excluding(x => x.RetentionPolicy) //deprecated
#pragma warning restore 618
                );
            });
        }

        [Test]
        [Category("Integration")]
        public void Constructor_Succeeds()
        {
            SetUpIntegration();

            Assert.That(_releaseTool, Is.Not.Null);
        }

        [Test]
        public static void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.That(() => new BuildTool(null), Throws.ArgumentException);
        }

        [Test]
        [Category("Integration")]
        public void GetReleaseDefinition_Throws()
        {
            SetUpIntegration();

            Assert.That(
                () => _releaseTool.GetReleaseDefinition(
                    int.MaxValue),
                Throws.TypeOf<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions.
                    ReleaseDefinitionNotFoundException>());
        }

        [Test]
        [Category("Integration")]
        public void GetReleaseDefinitions_Succeeds()
        {
            SetUpIntegration();

            const string name = ReleaseName;

            var result = _releaseTool.GetReleaseDefinitions(name);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo(name));

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }
    }
}