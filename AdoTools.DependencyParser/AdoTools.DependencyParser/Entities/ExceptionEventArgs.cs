using System;
using Upmc.DevTools.VsTs.Entities;

// ReSharper disable MissingXmlDoc

namespace Upmc.DevTools.Dependency.Parser.Entities
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