using System;

namespace AdoTools.Common.Entities
{
    /// <summary>
    ///     Describes the Level of a Message for Filtering
    /// </summary>
    public enum MessageLevel
    {
        /// <summary>
        ///     Success Message - Check passed.
        /// </summary>
        Success = 0,

        /// <summary>
        ///     Suggestion Message - Something which May Need to Be Changed
        /// </summary>
        Suggestion = 1,

        /// <summary>
        ///     Warning Message - Minor Problem
        /// </summary>
        Warning = 2,

        /// <summary>
        ///     Error Message - Serious Problem
        /// </summary>
        Error = 3
    }
}