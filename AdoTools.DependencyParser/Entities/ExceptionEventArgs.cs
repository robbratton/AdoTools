using System;
using AdoTools.Ado.Entities;

// ReSharper disable MissingXmlDoc

namespace AdoTools.DependencyParser.Entities
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(SourceInformation sourceInformation, Exception exception)
        {
            SourceInformation = sourceInformation;
            Exception = exception;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public SourceInformation SourceInformation { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Exception Exception { get; set; }
    }
}