/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * ToLiteralParser.cs
 *   Created on 1/1/2015 @ 4:15 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;

namespace ParserCombinators
{
    /// <summary>
    /// A Literal parser that matches exact text.
    /// </summary>
    public class LiteralParser : Parser<string>
    {
        #region Properties
        /// <summary>
        /// Gets the Text that this parser will match.
        /// </summary>
        public string Text { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Exact text to match.</param>
        public LiteralParser(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<string> Parse(string input, int index)
        {
            var i = input.IndexOf(Text, index, StringComparison.InvariantCulture);
            if (i == index)
            {
                return new ParseSuccess<string>(Text, Text, index);
            }

            return new ParseFail<string>(
                FailureType.Parsing,
                index,
                string.Concat("Expected \"", Text, "\" at index ", index));
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
            return string.Concat("\"", Text, "\"");
        }
    }
}