/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * OrParser.cs
 *   Created on 2/28/2015 @ 6:16 PM
 *   Written by kchaloux
 * ========================================================================= */

namespace ParserCombinators
{
    /// <summary>
    /// A parser that attempts to match either of two given parsers.
    /// </summary>
    public class OrParser<T> : Parser<T>
    {
        #region Properties
        /// <summary>
        /// Gets the first parser to attempt to match.
        /// </summary>
        public Parser<T> Parser1 { get; private set; }

        /// <summary>
        /// Gets the second parser to attempt to match.
        /// </summary>
        public Parser<T> Parser2 { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser1">The first parser to attempt to match.</param>
        /// <param name="parser2">The second parser to attempt to match.</param>
        public OrParser(Parser<T> parser1, Parser<T> parser2)
        {
            Parser1 = parser1;
            Parser2 = parser2;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<T> Parse(string input, int index)
        {
            var result1 = Parser1.Parse(input, index);
            if (result1.Success)
            {
                return new ParseSuccess<T>(result1.Text, result1.Value, index);
            }

            var result2 = Parser2.Parse(input, index);
            if (result2.Success)
            {
                return new ParseSuccess<T>(result2.Text, result2.Value, index);
            }

            return new ParseFail<T>(index,
                string.Concat("Expected at least one of the following at index ", index, ": ",
                    string.Join(", ", Parser1, Parser2)));
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
            return string.Concat(Parser1, " | ", Parser2);
        }
    }
}
