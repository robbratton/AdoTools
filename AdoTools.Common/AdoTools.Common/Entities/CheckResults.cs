using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.Common.Entities
{
    /// <inheritdoc />
    /// <summary>
    ///     Holds the Result of a set of Build Checks
    /// </summary>
    public class CheckResults<T> : ICloneable where T : class
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public CheckResults()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="messages"></param>
        public CheckResults(IEnumerable<Message<T>> messages) : this()
        {
            Messages.AddRange(messages);
        }

        /// <summary>
        ///     Indicates if There are any Error Messages
        /// </summary>
        public bool HasErrors => Messages.Any(x => x.MessageLevel == MessageLevel.Error);

        /// <summary>
        ///     Indicates if There are any Suggestion Messages
        /// </summary>
        public bool HasSuggestions => Messages.Any(x => x.MessageLevel == MessageLevel.Suggestion);

        /// <summary>
        ///     Indicates if There are any Warning Messages
        /// </summary>
        public bool HasWarnings => Messages.Any(x => x.MessageLevel == MessageLevel.Warning);

        /// <summary>
        ///     Collection of messages
        /// </summary>
        public List<Message<T>> Messages { get; } = new List<Message<T>>();

        /// <summary>
        ///     Indicates If All Results were Successful.
        /// </summary>
        public bool Passed => !Messages.Any() || Messages.All(x => x.MessageLevel == MessageLevel.Success);

        /// <inheritdoc />
        /// <summary>
        ///     Clones the current instance to a new instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var output = new CheckResults<T>(Messages);

            return output;
        }

        /// <summary>
        ///     Merge an existing set of messages into this instance.
        /// </summary>
        /// <param name="messages"></param>
        public void Merge(IEnumerable<Message<T>> messages)
        {
            Messages.AddRange(messages);
        }

        /// <summary>
        ///     Merge an instance into this instance.
        /// </summary>
        /// <param name="item"></param>
        public void Merge(CheckResults<T> item)
        {
            Messages.AddRange(item.Messages);
        }
    }
}