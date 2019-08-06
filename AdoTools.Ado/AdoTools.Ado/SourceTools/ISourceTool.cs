using System;
using System.Collections.Generic;
using Upmc.DevTools.VsTs.Entities;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedMemberInSuper.Global

namespace Upmc.DevTools.VsTs.SourceTools
{
    /// <summary>
    ///     Interface for Tools to Access Source Repositories
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISourceTool<T> where T : class
    {
        /// <summary>
        ///     Change the content of a single item.
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="existingItem"></param>
        /// <param name="content"></param>
        void ChangeItemContent(SourceInformation sourceInformation, T existingItem, string content);

        /// <summary>
        ///     Delete an item.
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="existingItem"></param>
        void DeleteItem(SourceInformation sourceInformation, T existingItem);

        /// <summary>
        ///     Get detailed information for all items based on input.
        /// </summary>
        /// <param name="searchInformation"></param>
        /// <returns></returns>
        IEnumerable<T> GetItems(SourceInformation searchInformation);

        /// <summary>
        ///     Get the content for a single item.
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <returns>The content of the item, if found, null if not found and specified</returns>
        string GetItemContent(SourceInformation sourceInformation);

        /// <summary>
        ///     Gets source item if it exists.
        /// </summary>
        /// <param name="searchInformation"></param>
        /// <returns>The found source item, otherwise null.</returns>
        T GetItem(SourceInformation searchInformation);

        /// <summary>
        ///     Map a specific source item to a SourceInformation item.
        /// </summary>
        /// <param name="input">the source item to be mapped</param>
        /// <param name="repositoryName">The repository if this is a Git sourceTool</param>
        /// <param name="branchName">The branch if this is a Git sourceTool</param>
        /// <returns></returns>
        SourceInformation Map(T input, string repositoryName = null, string branchName = null);

        /// <summary>
        ///     Update the content of an item.
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        void UpdateSourceInformation(SourceInformation sourceInformation, T item);
    }
}