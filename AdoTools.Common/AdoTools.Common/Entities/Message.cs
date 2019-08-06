using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Upmc.DevTools.Common.Entities
{
    /// <summary>
    ///     Describes a Message with a Generic Identifier
    /// </summary>
    /// <typeparam name="T">Identifier Type</typeparam>
    public class Message<T> where T : class
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="messageText"></param>
        /// <param name="identifier"></param>
        public Message(MessageLevel messageLevel, string messageText, T identifier = null)
        {
            Identifier = identifier;
            MessageLevel = messageLevel;
            MessageText = messageText;
        }

        /// <summary>
        ///     Identifier Type
        /// </summary>
        public T Identifier { get; }

        /// <summary>
        ///     Level of This Message
        /// </summary>
        public MessageLevel MessageLevel { get; }

        /// <summary>
        ///     Text of This Message
        /// </summary>
        public string MessageText { get; }

        /// <summary>
        ///     ToString Method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var output = "";

            if (Identifier != null)
            {
                output = $"{Identifier} ";
            }

            output += $"{MessageLevel.ToString()} {MessageText}";

            return output;
        }
    }
}