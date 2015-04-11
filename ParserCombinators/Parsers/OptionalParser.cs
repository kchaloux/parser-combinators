/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * OptionalParser.cs
 *   Created on 2/17/2015 @ 8:05 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Runtime.CompilerServices;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that makes matching another parser optional. This
    /// parser will always succeed.
    /// </summary>
    /// <typeparam name="T">Type of data being parsed.</typeparam>
    public class OptionalParser<T> : Parser<T>
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Parser{T}"/> to optionally match.
        /// </summary>
        public Parser<T> Parser { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser">Parser to avoid matching.</param>
        public OptionalParser(Parser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<T> Parse(string input, int index)
        {
            var result = Parser.Parse(input, index);
            return (result.Success)
                ? result
                : new ParseSuccess<T>("", default(T), index);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Concat("(", Parser, ")?");
        }
    }
}
