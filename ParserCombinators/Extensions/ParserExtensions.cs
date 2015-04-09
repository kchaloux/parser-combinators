/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * Extensions.cs
 *   Created on 2/28/2015 @ 6:34 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ParserCombinators
{
    /// <summary>
    /// Contains extension methods related to parsing.
    /// </summary>
    public static partial class ParserExtensions
    {
        /// <summary>
        /// Get the number of elements in a nested <see cref="SequenceResult{T1, T2}"/>,
        /// constructed and matched via Parser Combinators.
        /// </summary>
        /// <typeparam name="T1">Type of the first element in the <see cref="SequenceResult{T1, T2}"/>.</typeparam>
        /// <typeparam name="T2">Type of the second element in the <see cref="SequenceResult{T1, T2}"/>.</typeparam>
        /// <param name="result">The <see cref="SequenceResult{T1, T2}"/> to count the number of elements in.</param>
        /// <returns>The number of elements in a left-leaning nested <see cref="SequenceResult{T1, T2}"/>.</returns>
        public static int Count<T1, T2>(this SequenceResult<T1, T2> result)
        {
            var depth = 0;
            var type = result.GetType();
            while (type.IsGenericType &&
                     type.GetGenericTypeDefinition() == typeof(SequenceResult<,>))
            {
                depth++;
                type = type.GetGenericArguments()[0];
            }

            return depth + 1;
        }

        /// <summary>
        /// Get the item at the nth index in the given nested <see cref="SequenceResult{T1, T2}"/>, 
        /// constructed and matched via Parser Combinators. Uses reflection to access elements dynamically,
        /// allowing the user to get to elements of very long sequences.
        /// </summary>
        /// <typeparam name="T1">Type of the first element in the <see cref="SequenceResult{T1, T2}"/>.</typeparam>
        /// <typeparam name="T2">Type of the second element in the <see cref="SequenceResult{T1, T2}"/>.</typeparam>
        /// <param name="result">The element at the nth index of this nested <see cref="SequenceResult{T1, T2}"/>, as an object.</param>
        /// <param name="index">Index to get the element of.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is less than 0 or greater than the number of elements in the sequence.</exception>
        /// <returns>The element matched in the given sequence at the given index.</returns>
        public static object ItemN<T1, T2>(this SequenceResult<T1, T2> result, int index)
        {
            var count = Count(result);
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be a positive number less than the number of nested elements");
            }

            object current = result;
            for (int i = 0; i < count - index - 1; i++)
            {
                current = current.GetType().GetProperty("First").GetValue(current);
            }

            return (index == 0)
                ? current
                : current.GetType().GetProperty("Next").GetValue(current);
        }
    }
}
