/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * OrParser.cs
 *   Created on 2/28/2015 @ 6:16 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Collections.Generic;
using System.Linq;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that attempts to match at least one of a set of given parsers.
    /// </summary>
    public class OrParser<T> : Parser<T>
    {
        #region Properties
        /// <summary>
        /// Gets the list of parsers to potentially match.
        /// </summary>
        public IReadOnlyList<Parser<T>> Parsers { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parsers">The list of parsers to potentially match.</param>
        public OrParser(IEnumerable<Parser<T>> parsers)
        {
            Parsers = parsers.ToList();
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<T> Parse(string input, int index)
        {
            IParseResult<T> result = new ParseFail<T>(
                FailureType.Parsing,
                index,
                string.Concat("Expected at least one of the following at index ", index, ": ",
                    string.Join(", ", Parsers)));

            foreach (var parser in Parsers)
            {
                result = parser.Parse(input, index);
                if (result.Success)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="OrParser{T}"/> that will
        /// attempt to match the given parser at the current index
        /// if it fails to match this one.
        /// </summary>
        /// <param name="or">Other parser to match upon failing this parser.</param>
        /// <returns>A new <see cref="OrParser{T}"/>.</returns>
        public override Parser<T> Or(Parser<T> or)
        {
            return new OrParser<T>(Parsers.Concat(new[] { or }));
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
            return string.Join(" | ", Parsers);
        }
    }
}
