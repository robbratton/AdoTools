using System;
using AdoTools.Ado.Entities;

// ReSharper disable MissingXmlDoc

namespace AdoTools.DependencyParser.Entities
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