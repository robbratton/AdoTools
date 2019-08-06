using System;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedMemberInSuper.Global

namespace AdoTools.Common
{
    public interface IVariableDumpHandlers
    {
        string GetIndent(int indent);
        string HandleDictionary<TKey, TValue>(IDictionary<TKey, TValue> items, int indent);
        string HandleEnumerable(IEnumerable items, int indent);
        string HandleItem(object input, int indent);
        string HandleObject<T>(T input, int indent) where T : class;
    }
}