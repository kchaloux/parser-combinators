/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * InverseParser.cs
 *   Created on 2/17/2015 @ 7:24 PM
 *   Written by kchaloux
 * ========================================================================= */

namespace ParserCombinators
{
    /// <summary>
    /// A parser that fails if the parser it wraps succeeds.
    /// </summary>
    /// <typeparam name="T">Type of data being parsed.</typeparam>
    public class InverseParser<T> : Parser<T>
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Parser{T}"/> to fail upon matching.
        /// </summary>
        public Parser<T> Parser { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser">Parser to avoid matching.</param>
        public InverseParser(Parser<T> parser)
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
                return new ParseSuccess<T>("", default(T), index);
            }
            
            return new ParseFail<T>(
                result,
                FailureType.Inversion,
                string.Concat("Expected not to match ", Parser, " at index ", index));
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
            return string.Concat("!(", Parser, ")");
        }
    }
}
