/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * EndParser.cs
 *   Created on 4/11/2015 @ 1:12 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Runtime.CompilerServices;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that fails if it is does not terminate at the end of the input.
    /// </summary>
    /// <typeparam name="T">Type of data being parsed.</typeparam>
    public class EndParser<T> : Parser<T>
    {
        #region Properties
        /// <summary>
        /// Gets the parser to match at the end of the input.
        /// </summary>
        public Parser<T> Parser { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser">The parser to match at the end of the input.</param>
        public EndParser(Parser<T> parser)
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
            if (!result.Success)
            {
                return result;
            }

            var i = index + result.Length;
            if (i == input.Length)
            {
                return result;
            }

            return new ParseFail<T>(
                FailureType.Termination,
                i,
                string.Concat("Expected EOF at index ", i, ", found '", input[i]));
        }

        /// <summary>
        /// Creates a new <see cref="EndParser{T}"/> that wraps the current parser
        /// and fails if it does not terminate at the end of the given input.
        /// </summary>
        /// <returns>
        /// A new <see cref="EndParser{T}"/> that fails if the current 
        /// parser does not terminate at the end of the given input.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Parser<T> End()
        {
            return this;
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
            return Parser.ToString();
        }
    }
}
