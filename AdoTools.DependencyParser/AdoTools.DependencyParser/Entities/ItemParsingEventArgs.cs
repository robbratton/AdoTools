using System;
using Upmc.DevTools.VsTs.Entities;

// ReSharper disable MissingXmlDoc

namespace Upmc.DevTools.Dependency.Parser.Entities
{
    public class ItemParsingEventArgs : EventArgs
    {
        public ItemParsingEventArgs(SourceInformation sourceInformation)
        {
            SourceInformation = sourceInformation;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public SourceInformation SourceInformation { get; set; }
    }
}