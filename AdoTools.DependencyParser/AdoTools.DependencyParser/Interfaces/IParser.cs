// ReSharper disable UnusedMember.Global

using System;

namespace Upmc.DevTools.Dependency.Parser.Interfaces
{
    /// <summary>
    ///     Provides parsing services for VC and Git repositories.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        ///     Parse a VC Source Control Top-Level Folder
        /// </summary>
        /// <param name="sourcePath">Source PathRelativeToSolution</param>
        /// <param name="metaDataInSourceControl">True to access metadata in source control</param>
        /// <param name="outputFolder">Metadata Folder in Filesystem</param>
        /// <exception cref="ArgumentException"></exception>
        void ParseVcSourceTopLevelFolder(string sourcePath, bool metaDataInSourceControl, string outputFolder);

        /// <summary>
        ///     Process a Git repository.
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="metadataInSourceControl"></param>
        /// <param name="outputFolder"></param>
        void ProcessGitRepositories(string branchName, bool metadataInSourceControl, string outputFolder);
    }
}