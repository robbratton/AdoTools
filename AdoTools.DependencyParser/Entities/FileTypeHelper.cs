using System;
using System.IO;
using NLog;
using AdoTools.Common;
// ReSharper disable UnusedMember.Global

namespace AdoTools.DependencyParser.Entities
{
    /// <summary>
    ///     Helps with FileType Enumerations
    /// </summary>
    public static class FileTypeHelper
    {
        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Maps a string to a FileType
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileType MapType(string path)
        {
            Validators.AssertIsNotNullOrWhitespace(path, nameof(path));

            Logger.Trace("Entering");

            var output = FileType.Unknown;

            var filename = Path.GetFileName(path);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);

            // ReSharper disable once PossibleNullReferenceException
            if (extension.Equals(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                output = FileType.Solution;
            }
            // ReSharper disable once StringLiteralTypo
            else if (extension.Equals(".sqlproj", StringComparison.InvariantCultureIgnoreCase))
            {
                output = FileType.SqlProject;
            }
            else if (extension.Equals(".csproj", StringComparison.InvariantCultureIgnoreCase))
            {
                output = FileType.CsProject;
            }
            else if (extension.Equals(".config", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(filenameWithoutExtension))
                {
                    output = filenameWithoutExtension.Equals(
                        "packages",
                        StringComparison.InvariantCultureIgnoreCase)
                        ? FileType.PackageConfig
                        : FileType.AppWebDataConfig;
                }
            }
            else if (extension.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
            {
                if (filename != null && filename.Equals("AssemblyInfo.cs", StringComparison.InvariantCultureIgnoreCase))
                {
                    output = FileType.AssemblyInfo;
                }
            }
            else if (extension.Equals(".json", StringComparison.InvariantCultureIgnoreCase))
            {
                if (filenameWithoutExtension != null
                    && filenameWithoutExtension.StartsWith("appsettings", StringComparison.InvariantCultureIgnoreCase))
                {
                    output = FileType.AppsettingsJson;
                }
            }

            return output;
        }
    }
}