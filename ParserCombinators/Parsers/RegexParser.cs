/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * ToRegexParser.cs
 *   Created on 1/1/2015 @ 4:15 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Text.RegularExpressions;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that matches text using a Regular Expression.
    /// </summary>
    public class RegexParser : Parser<string>
    {
        private readonly Regex _regex;
     
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="regex">Regular expression to match with.</param>
        public RegexParser(Regex regex)
        {
            _regex = regex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pattern">Regular expression pattern to match with.</param>
        public RegexParser(string pattern)
        {
            _regex = new Regex(pattern);
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<string> Parse(string input, int index)
        {
            var match = _regex.Match(input, index);
            return (match.Success && match.Index == index)
                    ? (IParseResult<string>)new ParseSuccess<string>(match.Value, match.Value, index)
                    : new ParseFail<string>(index,
                        string.Concat("Expected /", _regex, "/ at index ", index));
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
            return string.Concat("/", _regex, "/");
        }
    }
}
