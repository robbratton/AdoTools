using System;
using AdoTools.Common.Entities;

namespace AdoTools.Common.Tests.Helpers
{
    public static class MessageGenerator<T> where T : class
    {
        public static Message<T> Generate(
            MessageLevel messageLevel = MessageLevel.Warning,
            T identifier = null,
            string message = null)
        {
            if (message == null)
            {
                message = $"{messageLevel.ToString()} message";
            }

            return new Message<T>(messageLevel, message, identifier);
        }
    }
}