using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace AdoTools.Common.Extensions
{
    // ReSharper disable once InconsistentNaming
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class IDictionaryExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        ///     Create Tuples from Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static IEnumerable<(TKey, TValue)> Tuples<TKey, TValue>(
            this IDictionary<TKey, TValue> dict)
        {
            return dict.Select(kvp => (kvp.Key, kvp.Value));
        }
    }
}