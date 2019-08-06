using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;
using AdoTools.Common.Entities;
using AdoTools.Common.Extensions;
using AdoTools.DependencyParser.Entities;
using AdoTools.VsParser;
using AdoTools.VsParser.Entities.Project;
using AdoTools.Ado;
using AdoTools.Ado.Entities;
using AdoTools.Ado.SourceTools;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("AdoTools.DependencyParser.Tests")]

namespace AdoTools.DependencyParser
{
    /// <summary>
    ///     Handles parsing of all source items supported by instances of ISourceTool.
    /// </summary>
    /// <typeparam name="T">Source item type</typeparam>
    public sealed class ParserGeneric<T> where T : class
    {
        /// <summary>
        ///     Process source code
        /// </summary>
        /// <param name="searchInformation">Start point</param>
        /// <param name="pathsToIgnore">
        ///     (optional) collection of paths to ignore. These will be converted to Regular expressions
        ///     with case ignored.
        /// </param>
        /// <returns>
        ///     <see cref="SolutionMetadata" />
        /// </returns>
        public IEnumerable<SolutionMetadata> ProcessSource(
            SourceInformation searchInformation,
            IEnumerable<string> pathsToIgnore = null)
        {
            _logger.Trace("Entering");

            IEnumerable<Regex> regexPaths = null;

            if (pathsToIgnore != null)
            {
                regexPaths = pathsToIgnore.Select(x => new Regex(x, RegexOptions.Compiled | RegexOptions.IgnoreCase));
            }

            return ProcessSource(searchInformation, regexPaths);
        }

        /// <summary>
        ///     Process source code
        /// </summary>
        /// <param name="searchInformation">Start point</param>
        /// <param name="pathsToIgnore">(optional) collection of regex paths to ignore.</param>
        /// <returns>
        ///     <see cref="SolutionMetadata" />
        /// </returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public IEnumerable<SolutionMetadata> ProcessSource(
            SourceInformation searchInformation,
            IEnumerable<Regex> pathsToIgnore = null)
        {
            _logger.Trace("Entering");

            var sourceTool = GetSourceTool();

            // Get information on all the files under the search directory for later use.
            var sw = Stopwatch.StartNew();
            var rawItems = sourceTool.GetItems(searchInformation);
            sw.Stop();
            _logger.Trace($"GetItems took {sw.Elapsed}");

            _allItems = rawItems
                .Select(
                    x => new SourceInformation(
                        sourceTool.Map(x, searchInformation.GitRepositoryName, searchInformation.GitBranch)))
                .Where(x => pathsToIgnore == null || pathsToIgnore.All(z => !z.IsMatch(x.SourcePath)));

            var solutions = FindSolutions();

            var output = new ConcurrentBag<SolutionMetadata>();

            var exceptions = new ConcurrentQueue<Exception>();

            Parallel.ForEach(
                solutions,
                _performanceConfiguration.ParallelOptions,
                solution =>
                {
                    try
                    {
                        var solutionMetadata = ProcessSolution(solution);

                        output.Add(solutionMetadata);
                    }
                    catch (Exception e)
                    {
                        OnExceptionThrown(new ExceptionEventArgs(solution, e));

                        exceptions.Enqueue(e);
                    }
                }
            );

            if (exceptions.Any())
            {
                throw new AggregateException($"Some source processing failed for {searchInformation}", exceptions);
            }

            return output.ToList();
        }

        private ISourceTool<T> GetSourceTool()
        {
            ISourceTool<T> output = null;
            VsTsTool vsTsTool = null;

            if (_overrideSourceTool != null)
            {
                output = _overrideSourceTool;
            }
            else
            {
                if (_sourceType == SourceType.TfsVc || _sourceType == SourceType.TfsGit)
                {
                    vsTsTool = new VsTsTool(
                        _personalAccessToken,
                        _organization,
                        _projectName,
                        _logger,
                        _performanceConfiguration);
                }

                switch (_sourceType)
                {
                    case SourceType.None:
                        break;
                    case SourceType.TfsVc:
                        output = (ISourceTool<T>) new VcSourceTool(vsTsTool, _logger, _performanceConfiguration);
                        break;
                    case SourceType.TfsGit:
                        output = (ISourceTool<T>) new GitSourceTool(vsTsTool, _logger, _performanceConfiguration);
                        break;
                    case SourceType.Filesystem:
                        output = (ISourceTool<T>) new FilesystemSourceTool(_logger, _performanceConfiguration);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return output;
        }

        internal static void ProcessConfigFile(
            ProjectMetadata projectMetadata,
            string filename,
            string extension,
            string content)
        {
            _logger.Trace("Entering");

            // ReSharper disable once PossibleNullReferenceException
            if (filename.StartsWith("app.", StringComparison.CurrentCultureIgnoreCase)
                &&
                extension.EndsWith("config", StringComparison.CurrentCultureIgnoreCase))
            {
                var configFile = ConfigParser.ParseAppConfigFileContent(content, filename);
                projectMetadata.AppSettings.AddRange(configFile.ApplicationSettings);
            }
            else if (filename.StartsWith("web.", StringComparison.CurrentCultureIgnoreCase)
                     &&
                     extension.Equals(".config", StringComparison.CurrentCultureIgnoreCase))
            {
                var configFile = ConfigParser.ParseWebConfigFileContent(content, filename);
                projectMetadata.AppSettings.AddRange(configFile.AppSettings);
            }
            else if (filename.StartsWith("dataConfiguration", StringComparison.CurrentCultureIgnoreCase)
                     &&
                     extension.Equals(".config", StringComparison.CurrentCultureIgnoreCase))
            {
                var configFile = ConfigParser.ParseDataConfigurationFileContent(content, filename);
                projectMetadata.ConnectionStrings = configFile.ConnectionStrings;
                projectMetadata.DatabaseInstances = configFile.DatabaseInstances;
                projectMetadata.DatabaseTypes = configFile.DatabaseTypes;
            }
            else if (filename.StartsWith("appSettings", StringComparison.CurrentCultureIgnoreCase)
                     &&
                     extension.Equals(".json", StringComparison.CurrentCultureIgnoreCase))
            {
                var configFile = ConfigParser.ParseAppSettingsJsonFileContent(content);
                projectMetadata.ConnectionStrings = configFile.GetConnectionStrings().ToList();
                projectMetadata.DatabaseInstances = null;
                projectMetadata.DatabaseTypes = null;
            }
        }

        internal void ProcessPackagesConfig(ProjectMetadata projectMetadata, SourceInformation projectDirectory)
        {
            _logger.Trace("Entering");

            var packagesConfig = Tools.GetChildFileInformation(
                new SourceInformation(projectDirectory),
                "packages.config");

            var sourceTool = GetSourceTool();

            var sw = Stopwatch.StartNew();
            var content = sourceTool.GetItemContent(packagesConfig);
            sw.Stop();
            _logger.Trace($"GetItemContent took {sw.Elapsed}");

            if (content != null)
            {
                projectMetadata.PackageReferences =
                    PackageConfigParser.ParsePackagesConfigFileContent(content).ToList();
            }
        }

        private void ProcessGitVersionFile(SolutionMetadata solutionMetadata)
        {
            _logger.Trace("Entering");

            var gitVersionInformation = Tools.GetChildFileInformation(
                solutionMetadata.SourceInformation,
                "GitVersion.yml");

            if (gitVersionInformation != null)
            {
                var sourceTool = GetSourceTool();

                var sw = Stopwatch.StartNew();
                var content = sourceTool.GetItemContent(gitVersionInformation);
                sw.Stop();
                _logger.Trace($"GetItemContent took {sw.Elapsed}");

                if (content != null)
                {
                    solutionMetadata.GitVersion = GitVersionParser.ParseGitVersionContent(content);
                }
            }
        }

        private IEnumerable<SourceInformation> FindSolutions()
        {
            _logger.Trace("Entering");

            var output = _allItems
                .Where(x => x.SourcePath.ToLower().EndsWith(".sln", StringComparison.CurrentCultureIgnoreCase));

            return output;
        }

        private void ProcessAssemblyInfo(Project projectMetadata, SourceInformation projectDirectory)
        {
            _logger.Trace("Entering");

            var propertiesInfo = Tools.GetChildDirectoryInformation(
                new SourceInformation(projectDirectory),
                "properties");
            // ReSharper disable once StringLiteralTypo
            var assemblyInfo = Tools.GetChildFileInformation(propertiesInfo, "assemblyinfo.cs");

            var sourceTool = GetSourceTool();

            var sw = Stopwatch.StartNew();
            var content = sourceTool.GetItemContent(assemblyInfo);
            sw.Stop();
            _logger.Trace($"GetItemContent took {sw.Elapsed}");

            if (content != null)
            {
                projectMetadata.AssemblyInformation = AssemblyInfoParser.ParseFileContent(content).ToList();
            }
        }

        private void ProcessConfigFiles(ProjectMetadata projectMetadata, SourceInformation projectSourceInformation)
        {
            _logger.Trace("Entering");

            var configFiles = _allItems
                .Where(
                    x =>
                        x.SourcePath.StartsWith(
                            projectSourceInformation.SourcePath,
                            StringComparison.CurrentCultureIgnoreCase)
                        && x.SourcePath.ToLower().EndsWith(".config", StringComparison.CurrentCultureIgnoreCase)
                        && !x.SourcePath.ContainsInsensitive("packages.config")
                );

            var exceptions = new ConcurrentQueue<Exception>();

            foreach (var configFile in configFiles)
            {
                try
                {
                    var filename = Path.GetFileName(configFile.SourcePath);
                    var extension = Path.GetExtension(configFile.SourcePath);

                    var sourceTool = GetSourceTool();

                    var sw = Stopwatch.StartNew();
                    var content = sourceTool.GetItemContent(configFile);
                    sw.Stop();
                    _logger.Trace($"GetItemContent took {sw.Elapsed}");

                    ProcessConfigFile(projectMetadata, filename, extension, content);
                }
                catch (Exception e)
                {
                    OnExceptionThrown(new ExceptionEventArgs(configFile, e));

                    exceptions.Enqueue(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Some config file processing failed for {projectSourceInformation}.",
                    exceptions);
            }
        }

        private ProjectMetadata ProcessProject(SourceInformation projectSourceInformation)
        {
            _logger.Trace("Entering");

            OnItemProcessing(new ItemParsingEventArgs(projectSourceInformation));

            Tools.FixPathSeparators(projectSourceInformation);
            Tools.FixPathEnding(projectSourceInformation);

            var filename = Path.GetFileName(projectSourceInformation.SourcePath);

            var sourceTool = GetSourceTool();

            var sw = Stopwatch.StartNew();
            var projectContent = sourceTool.GetItemContent(projectSourceInformation);
            sw.Stop();
            _logger.Trace($"Getting items took {sw.Elapsed}");

            var project = CsProjectParser.ParseProjectFileContent(projectContent, filename);

            var output = ProjectMetadata.MapFromProject(project);
            output.SourceInformation = projectSourceInformation;

            return output;
        }

        private SolutionMetadata ProcessSolution(SourceInformation solutionSourceInformation)
        {
            _logger.Trace("Entering");

            var sourceTool = GetSourceTool();

            var sw = Stopwatch.StartNew();
            var content = sourceTool.GetItemContent(solutionSourceInformation);
            sw.Stop();
            _logger.Trace($"GettItemContent took {sw.Elapsed}");

            var solutionParser = new SolutionParser();
            var solution = solutionParser.ParseSolutionFileContent(content);
            var name = Path.GetFileNameWithoutExtension(solutionSourceInformation.SourcePath);
            var solutionMetadata = new SolutionMetadata(name, solutionSourceInformation, solution);

            OnItemProcessing(new ItemParsingEventArgs(solutionSourceInformation));

            if (_parseGitVersionFiles)
            {
                ProcessGitVersionFile(solutionMetadata);
            }

            if (solutionMetadata.Projects != null)
            {
                var exceptions = new ConcurrentQueue<Exception>();

                Parallel.ForEach(
                    solutionMetadata.Projects,
                    _performanceConfiguration.ParallelOptions,
                    project =>
                    {
                        var projectSourceInformation =
                            new SourceInformation(solutionSourceInformation)
                            {
                                SourcePath = Tools.GetDirectoryInformation(solutionSourceInformation).SourcePath
                                             + project.PathRelativeToSolution
                            };

                        try
                        {
                            if (project.PathRelativeToSolution.EndsWith(
                                ".csproj",
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                var projectMetadata = ProcessProject(projectSourceInformation);
                                projectMetadata.SourceInformation = projectSourceInformation;

                                var projectDirectory = Tools.GetDirectoryInformation(projectSourceInformation);

                                if (_parsePackageConfigFiles)
                                {
                                    ProcessPackagesConfig(projectMetadata, projectDirectory);
                                }

                                if (_parseConfigFiles)
                                {
                                    ProcessConfigFiles(projectMetadata, projectDirectory);
                                }

                                if (_parseAssemblyInfoFiles)
                                {
                                    ProcessAssemblyInfo(projectMetadata, projectDirectory);
                                }

                                solutionMetadata.ProjectMetadatas.Add(projectMetadata);
                            }

                            // todo Parse SQL Projects?
                        }
                        catch (Exception e)
                        {
                            OnExceptionThrown(new ExceptionEventArgs(projectSourceInformation, e));

                            exceptions.Enqueue(e);
                        }
                    }
                );

                if (exceptions.Any())
                {
                    throw new AggregateException(
                        $"Some project processing failed for {solutionSourceInformation}.",
                        exceptions);
                }
            }

            return solutionMetadata;
        }

        #region Constructors

        /// <summary>
        ///     Instance constructor
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="projectName"></param>
        /// <param name="parsePackageConfigFiles">
        ///     Set to false if you don't need information from these files to improve
        ///     performance.
        /// </param>
        /// <param name="parseConfigFiles">Set to false if you don't need information from these files to improve performance.</param>
        /// <param name="parseAssemblyInfoFiles">
        ///     Set to false if you don't need information from these files to improve
        ///     performance.
        /// </param>
        /// <param name="parseGitVersionFiles">
        ///     Set to false if you don't need information from these files to improve
        ///     performance.
        /// </param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        /// <param name="sourceType"></param>
        /// <param name="personalAccessToken"></param>
        public ParserGeneric(
            SourceType sourceType,
            string personalAccessToken = null,
            string organization = null,
            string projectName = null,
            bool parsePackageConfigFiles = true,
            bool parseConfigFiles = true,
            bool parseAssemblyInfoFiles = true,
            bool parseGitVersionFiles = true,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        )
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();

            _logger.Trace("Entering");

            _performanceConfiguration = performanceConfiguration ?? new PerformanceConfiguration();

            _sourceType = sourceType;

            _personalAccessToken = personalAccessToken;
            _organization = organization;
            _projectName = projectName;

            _parseAssemblyInfoFiles = parseAssemblyInfoFiles;
            _parseConfigFiles = parseConfigFiles;
            _parsePackageConfigFiles = parsePackageConfigFiles;
            _parseGitVersionFiles = parseGitVersionFiles;
        }

        /// <summary>
        ///     For testing purposes only.
        /// </summary>
        /// <param name="sourceTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        [ExcludeFromCodeCoverage]
        public ParserGeneric(ISourceTool<T> sourceTool = null, Logger logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();

            _logger.Trace("Entering");

            if (!Assembly.GetCallingAssembly()
                .GetName()
                .Name.EndsWith("Tests", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException(
                    "This constructor is for testing only. It will throw an exception if it is called from an assembly which is not named *.Tests.");
            }

            _parsePackageConfigFiles = true;
            _parseAssemblyInfoFiles = true;
            _parseConfigFiles = true;

            _overrideSourceTool = sourceTool;

            _performanceConfiguration = new PerformanceConfiguration(
                new ParallelOptions{ MaxDegreeOfParallelism = 1 }
            );
        }

        #endregion Constructors

        #region Events

        /// <summary>
        ///     This will be raised when an internal exception is raised. The exceptions will still be returned inside an aggregate
        ///     exception.
        /// </summary>
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event EventHandler<ExceptionEventArgs> ExceptionThrown;

        /// <summary>
        ///     This will be raised as each source item's processing is started.
        /// </summary>
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event EventHandler<ItemParsingEventArgs> ItemProcessing;

        private void OnExceptionThrown(ExceptionEventArgs e)
        {
            _logger.Trace("Entering");

            var handler = ExceptionThrown;
            handler?.Invoke(this, e);

            _logger.Trace("Exiting");
        }

        private void OnItemProcessing(ItemParsingEventArgs e)
        {
            _logger.Trace("Entering");

            var handler = ItemProcessing;
            handler?.Invoke(this, e);

            _logger.Trace("Exiting");
        }

        #endregion Events

        #region Private Fields

        private readonly bool _parseAssemblyInfoFiles;

        private readonly bool _parseConfigFiles;

        private readonly bool _parseGitVersionFiles;

        private readonly bool _parsePackageConfigFiles;

        private IEnumerable<SourceInformation> _allItems;

        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static Logger _logger;

        private readonly PerformanceConfiguration _performanceConfiguration;
        private readonly SourceType _sourceType;
        private readonly string _organization;
        private readonly string _personalAccessToken;
        private readonly string _projectName;

        /// <summary>
        /// Used instead of instantiating a new source tool for testing.
        /// </summary>
        private readonly ISourceTool<T> _overrideSourceTool;

        #endregion Private Fields
    }
}