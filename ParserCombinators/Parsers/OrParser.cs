/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * OrParser.cs
 *   Created on 2/28/2015 @ 6:16 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Runtime.CompilerServices;

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

            var failure1 = (ParseFail<T>)result1;
            var failure2 = (ParseFail<T>)result2;

            if (failure1.FailureType == FailureType.Termination)
            {
                return failure1;
            }

            if (failure2.FailureType == FailureType.Termination)
            {
                return failure2;
            }

            return new ParseFail<T>(
                failure2.FailureType,
                index,
                string.Concat("Expected at least one of the following at index ", index, ": ",
                    string.Join(", ", Parser1, Parser2)));
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
            return new OrParser<T>(Parser1.End(), Parser2.End());
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
