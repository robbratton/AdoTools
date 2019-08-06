using System;
using System.Collections.Generic;
using AdoTools.Common.Entities;

namespace AdoTools.Common.Tests.Helpers
{
    public static class CheckResultsGenerator<T> where T : class
    {
        public static CheckResults<T> Generate(
            bool includeError = false,
            bool includeSuccess = false,
            bool includeWarning = false,
            bool includeSuggestion = false,
            T identifier = null)
        {
            var messages = new List<Message<T>>();
            if (includeError)
            {
                messages.Add(MessageGenerator<T>.Generate(MessageLevel.Error, identifier));
            }

            if (includeSuccess)
            {
                messages.Add(MessageGenerator<T>.Generate(MessageLevel.Success, identifier));
            }

            if (includeWarning)
            {
                messages.Add(MessageGenerator<T>.Generate(MessageLevel.Warning, identifier));
            }

            if (includeSuggestion)
            {
                messages.Add(MessageGenerator<T>.Generate(MessageLevel.Suggestion, identifier));
            }

            var output = new CheckResults<T>(messages);

            return output;
        }
    }
}