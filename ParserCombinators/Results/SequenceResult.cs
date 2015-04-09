/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * SequenceResult.cs
 *   Created on 2/28/2015 @ 6:00 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Collections.Generic;
using System.Linq;

namespace ParserCombinators
{
    /// <summary>
    /// A container for the two results of a sequence parse operation.
    /// </summary>
    /// <typeparam name="T1">Type of the first value matched in the sequence.</typeparam>
    /// <typeparam name="T2">Type of the next value matched in the sequence.</typeparam>
    public sealed class SequenceResult<T1, T2>
    {
        #region Properties
        /// <summary>
        /// The value of the first match in this Sequence.
        /// </summary>
        public T1 First { get; private set; }

        /// <summary>
        /// The value of the next match in this Sequence.
        /// </summary>
        public T2 Next { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="first">The first element in this sequence.</param>
        /// <param name="next">The next element in this sequence.</param>
        internal SequenceResult(T1 first, T2 next)
        {
            First = first;
            Next = next;
        }

        private string _toString;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (_toString == null)
            {
                var elements = new Stack<string>();
                object current = this;
                while (current.GetType().IsGenericType &&
                       current.GetType().GetGenericTypeDefinition() == typeof(SequenceResult<,>))
                {
                    var element = current.GetType().GetProperty("Next").GetValue(current);
                    elements.Push(GetElementString(element));
                    current = current.GetType().GetProperty("First").GetValue(current);
                }
                elements.Push(GetElementString(current));
                _toString = string.Concat("Seq(", string.Join(" ~ ", elements), ")");
            }
            return _toString;
        }

        private string GetElementString(object element)
        {
            var visualElement = element is string
                        ? string.Concat("\"", element.ToString(), "\"")
                        : element.ToString();
            return visualElement;
        }
    }
}
