using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AutoMapper;
using NLog;
using AdoTools.VsParser.Entities.ConfigFile.Components;
using AdoTools.VsParser.Entities.DataConfigurationFile.Components;
using AdoTools.VsParser.Entities.Project;
using AdoTools.Ado.Entities;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverQueried.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

[assembly: InternalsVisibleTo("AdoTools.DependencyParser.Tests")]

namespace AdoTools.DependencyParser.Entities
{
    /// <inheritdoc />
    /// <summary>
    ///     Metadata for project files (*.sln)
    /// </summary>
    public class ProjectMetadata : Project
    {
        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Map a project to a ProjectMetadata
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ProjectMetadata MapFromProject(Project item)
        {
            _logger.Trace("Entering");

            var output = new ProjectMetadata();

            MapFromProjectToProjectMetadata(item, output);

            _logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Update this instance from another.
        /// </summary>
        /// <param name="item"></param>
        public void Update(ProjectMetadata item)
        {
            _logger.Trace("Entering");

            FullMapper.Map(item, this);

            _logger.Trace("Exiting");
        }

        /// <summary>
        ///     Update this instance from a project.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateFromProject(Project item)
        {
            _logger.Trace("Entering");

            PartialMapper.Map(item, this);

            _logger.Trace("Exiting");
        }

        private static void MapFromProjectToProjectMetadata(Project item, ProjectMetadata output)
        {
            _logger.Trace("Entering");

            PartialMapper.Map(item, output);

            _logger.Trace("Exiting");
        }

        #region Properties

        private static readonly IMapper FullMapper;

        private static readonly IMapper PartialMapper;

        /// <summary>
        ///     Source Information
        /// </summary>
        public SourceInformation SourceInformation { get; set; }

        /// <summary>
        ///     Application Settings
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public List<ApplicationSetting> AppSettings { get; set; } = new List<ApplicationSetting>();

        /// <summary>
        ///     Database Connection Strings
        /// </summary>
        public List<ConnectionStringSetting> ConnectionStrings { get; set; } = new List<ConnectionStringSetting>();

        /// <summary>
        ///     Database Instances (Enterprise Library only)
        /// </summary>
        public List<DatabaseInstance> DatabaseInstances { get; set; } = new List<DatabaseInstance>();

        /// <summary>
        ///     Database Types (Enterprise Library only)
        /// </summary>
        public List<DatabaseType> DatabaseTypes { get; set; } = new List<DatabaseType>();

        #endregion Properties

        #region Constructors

        static ProjectMetadata()
        {
            _logger.Trace("Entering");

            var partialConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<Project, ProjectMetadata>()
                        .ForMember(x => x.SourceInformation, opt => opt.Ignore())
                        ; // todo Is there a way to tell this mapper to use the ProjectMetadata mapper?
                    cfg.AllowNullCollections = true;
                    cfg.AllowNullDestinationValues = true;
                }
            );
            PartialMapper = partialConfig.CreateMapper();

            var fullConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<ProjectMetadata, ProjectMetadata>();
                    cfg.AllowNullCollections = true;
                    cfg.AllowNullDestinationValues = true;
                }
            );
            FullMapper = fullConfig.CreateMapper();

            _logger.Trace("Exiting");
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger">Optional NLog Logger</param>
        public ProjectMetadata(Logger logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();

            _logger.Trace("Entering");
            _logger.Trace("Exiting");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        public ProjectMetadata() : this(null)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create a new instance optionally with parameters and from a project.
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="project"></param>
        /// <param name="logger">Optional Logger</param>
        public ProjectMetadata(
            SourceInformation sourceInformation,
            Project project = null,
            Logger logger = null) : this(logger)
        {
            _logger.Trace("Entering");

            if (project != null)
            {
                MapFromProjectToProjectMetadata(project, this);
            }

            if (sourceInformation != null)
            {
                sourceInformation.AssertIsValid();
                SourceInformation = sourceInformation;
            }

            _logger.Trace("Exiting");
        }

        #endregion Constructors
    }
}