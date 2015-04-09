/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * SequenceParser.cs
 *   Created on 2/28/2015 @ 6:08 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that matches two other parsers in sequence.
    /// </summary>
    /// <typeparam name="T1">Type of the first parser to match.</typeparam>
    /// <typeparam name="T2">Type of the next parser to match.</typeparam>
    public class SequenceParser<T1, T2> : Parser<SequenceResult<T1, T2>>
    {
        #region Properties
        /// <summary>
        /// Gets the first parser to match in the sequence.
        /// </summary>
        public Parser<T1> Parser1 { get; private set; }

        /// <summary>
        /// Gets the second parser to match in the sequence.
        /// </summary>
        public Parser<T2> Parser2 { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser1">The first parser to match in the sequence.</param>
        /// <param name="parser2">The second parser to match in the sequence</param>
        public SequenceParser(Parser<T1> parser1, Parser<T2> parser2)
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
        public override IParseResult<SequenceResult<T1, T2>> Parse(string input, int index)
        {
            var i = index;
            var result1 = Parser1.Parse(input, i);
            if (!result1.Success)
            {
                return new ParseFail<SequenceResult<T1, T2>>(result1.Index, result1.Message);
            }

            i += result1.Length;
            var result2 = Parser2.Parse(input, i);
            if (!result2.Success)
            {
                return new ParseFail<SequenceResult<T1, T2>>(result2.Index, result2.Message);
            }

            try
            {
                var value = new SequenceResult<T1, T2>(result1.Value, result2.Value);
                return new ParseSuccess<SequenceResult<T1, T2>>(string.Concat(result1.Text, result2.Text), value, index);
            }
            catch (Exception ex)
            {
                return new ParseFail<SequenceResult<T1, T2>>(index,
                    string.Concat("Failed to convert (", result1.Value, ", ", result2.Value, ") to type ",
                        typeof(SequenceResult<T1, T2>).Name, ": ", ex.Message));
            }
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
            return string.Concat(Parser1, " ~ ", Parser2);
        }
    }
}
