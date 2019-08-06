using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global

namespace AdoTools.Common
{
    /// <summary>
    ///     Handlers used when dumping variables. This is a class so that parts can be overridden.
    /// </summary>
    public class VariableDumpHandlers : IVariableDumpHandlers
    {
        public readonly string IndentString;

        public readonly int MaxDepth;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="indentString"></param>
        public VariableDumpHandlers(
            int maxDepth = 10,
            string indentString = "  ")
        {
            MaxDepth = maxDepth;
            IndentString = indentString;
        }

        /// <summary>
        ///     Decides the type and handles it appropriately.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual string HandleItem(
            object input,
            int indent
        )
        {
            var output = new StringBuilder();

            if (input == null)
            {
                output.Append("(null)");
            }
            else
            {
                switch (input)
                {
                    case string stringValue:
                        output.Append("\"").Append(stringValue).Append("\"");
                        break;

                    case ValueType generalValue:
                        output.Append(generalValue);
                        break;

                    case IDictionary<string, string> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary<string, int> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary<string, float> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary<string, double> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary<string, bool> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary<int, string> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary<string, object> dictionaryValue:
                        output.Append(HandleDictionary(dictionaryValue, indent));
                        break;

                    case IDictionary dictionaryValue:
                        // todo Handle other dictionary cases?
                        output.Append(HandleEnumerable(dictionaryValue, indent));
                        break;

                    case IEnumerable enumerableValue:
                        output.Append(HandleEnumerable(enumerableValue, indent));
                        break;

                    default:
                        output.Append(HandleObject(input, indent));
                        break;
                }
            }

            return output.ToString();
        }

        /// <summary>
        ///     Handles a generic dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="items"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual string HandleDictionary<TKey, TValue>(
            IDictionary<TKey, TValue> items,
            int indent
        )
        {
            var output = new StringBuilder();

            output.AppendLine("");

            for (var index = 0; index < items.Count; index++)
            {
                var item = items.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                output.Append(GetIndent(indent));
                output.Append("(Key) ").Append(HandleItem(itemKey, indent));
                output.Append(": (Value) ");
                output.AppendLine(HandleItem(itemValue, indent));
            }

            return output.ToString();
        }

        /// <summary>
        ///     Handle Enumerable items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual string HandleEnumerable(
            IEnumerable items,
            int indent
        )
        {
            var output = new StringBuilder();

            foreach (var item in items)
            {
                output.Append(GetIndent(indent)).AppendLine(HandleItem(item, indent + 1));
            }

            return output.ToString();
        }

        /// <summary>
        ///     Handle an object's properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual string HandleObject<T>(
            T input,
            int indent
        ) where T : class
        {
            var output = new StringBuilder();

            var type = input.GetType();

            // Finish the previous line.
            output.AppendLine("");

            var propertyInfos =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var property in propertyInfos)
            {
                output.Append(GetIndent(indent + 1)).Append(property.Name).Append(": ");

                object value;
                try
                {
                    value = property.GetValue(input, null);
                }
                catch (Exception)
                {
                    value = "(Unknown)";
                }

                output.AppendLine(HandleItem(value, indent + 1));
            }

            return output.ToString();
        }

        /// <summary>
        ///     Get a string representing the indent input.
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual string GetIndent(int indent)
        {
            if (indent < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indent), indent, "must be >= 0");
            }

            if (indent > MaxDepth)
            {
                throw new ArgumentOutOfRangeException(nameof(indent), indent, "must be <= " + MaxDepth);
            }

            var output = new StringBuilder();

            for (var i = 0; i < indent; i++)
            {
                output.Append(IndentString);
            }

            return output.ToString();
        }
    }
}