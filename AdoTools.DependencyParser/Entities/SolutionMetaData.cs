using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper;
using NLog;
using AdoTools.VsParser.Entities;
using AdoTools.Ado.Entities;
// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

[assembly: InternalsVisibleTo("AdoTools.DependencyParser.Tests")]

namespace AdoTools.DependencyParser.Entities
{
    /// <inheritdoc />
    /// <summary>
    ///     Metadata for Solution files (*.sln)
    /// </summary>
    public class SolutionMetadata : Solution
    {
        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Map a Solution to a SolutionMetadata
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SolutionMetadata MapFromSolution(Solution item)
        {
            _logger.Trace("Entering");

            var output = new SolutionMetadata();

            MapFromSolutionToSolutionMetadata(item, output);

            _logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Update this instance from another.
        /// </summary>
        /// <param name="item"></param>
        public void Update(SolutionMetadata item)
        {
            _logger.Trace("Entering");

            FullMapper.Map(item, this);

            if (item.ProjectMetadatas != null)
            {
                ProjectMetadatas.AddRange(item.Projects.Cast<ProjectMetadata>());
            }

            _logger.Trace("Exiting");
        }

        /// <summary>
        ///     Update this instance from a Solution.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateFromSolution(Solution item)
        {
            _logger.Trace("Entering");

            PartialMapper.Map(item, this);

            _logger.Trace("Exiting");
        }

        private static void MapFromSolutionToSolutionMetadata(Solution item, SolutionMetadata output)
        {
            _logger.Trace("Entering");

            PartialMapper.Map(item, output);

            if (item.Projects != null)
            {
                output.ProjectMetadatas = new List<ProjectMetadata>();

                foreach (var project in item.Projects)
                {
                    output.Projects.Add(new ProjectMetadata(null, project));
                }
            }

            _logger.Trace("Exiting");
        }

        #region Properties

        private static readonly IMapper FullMapper;

        private static readonly IMapper PartialMapper;

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        ///     Name of the Solution
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Projects in this Solution
        /// </summary>
        public List<ProjectMetadata> ProjectMetadatas { get; set; } = new List<ProjectMetadata>();

        /// <summary>
        ///     SourceInformation for this Solution
        /// </summary>
        public SourceInformation SourceInformation { get; set; }

        #endregion Properties

        #region Constructors

        static SolutionMetadata()
        {
            _logger.Trace("Entering");

            var partialConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<Solution, SolutionMetadata>()
                        .ForMember(x => x.Name, opt => opt.Ignore())
                        .ForMember(x => x.SourceInformation, opt => opt.Ignore())
                        .ForMember(
                            x => x.Projects,
                            opt => opt
                                .Ignore()); // todo Is there a way to tell this mapper to use the ProjectMetadata mapper?
                    cfg.AllowNullCollections = true;
                    cfg.AllowNullDestinationValues = true;
                }
            );
            PartialMapper = partialConfig.CreateMapper();

            var fullConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<SolutionMetadata, SolutionMetadata>();
                    cfg.AllowNullCollections = true;
                    cfg.AllowNullDestinationValues = true;
                }
            );
            FullMapper = fullConfig.CreateMapper();

            _logger.Trace("Exiting");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        public SolutionMetadata() : this(null)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger">Optional NLog Logger</param>
        public SolutionMetadata(Logger logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();

            _logger.Trace("Entering");
            _logger.Trace("Exiting");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create a new instance optionally with parameters and from a Solution.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceInformation"></param>
        /// <param name="solution"></param>
        /// <param name="logger">Optional NLog Logger</param>
        public SolutionMetadata(
            string name = null,
            SourceInformation sourceInformation = null,
            Solution solution = null,
            Logger logger = null) : this(logger)
        {
            _logger.Trace("Entering");

            if (solution != null)
            {
                MapFromSolutionToSolutionMetadata(solution, this);
            }

            if (name != null)
            {
                Name = name;
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