/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * Lazy.cs
 *   Created on 2/17/2015 @ 10:08 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;

namespace ParserCombinators
{
    public static class Lazy
    {
        /// <summary>
        /// Creates a new <see cref="LazyParser{T}"/> from a given function.
        /// </summary>
        /// <param name="generator">Function used to generate an instance of a parser.</param>
        /// <returns>A new <see cref="LazyParser{T}"/> that will resolve an instance of the parser with the given function.</returns>
        /// <remarks>
        /// This is provided as a convenience to avoid having to type out the potentially
        /// incredibly long types of some of the compound parsers when making them lazy.
        /// </remarks>
        public static LazyParser<T> Parser<T>(Func<Parser<T>> generator)
        {
            return new LazyParser<T>(generator);
        }
    }
}
