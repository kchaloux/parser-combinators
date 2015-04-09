/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * RepeatParser.cs
 *   Created on 2/17/2015 @ 8:32 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that repeatedly matches another parser.
    /// </summary>
    /// <typeparam name="T">Type of data being parsed.</typeparam>
    public class RepeatParser<T> : Parser<IReadOnlyList<T>>
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Parser{T}"/> to repeat.
        /// </summary>
        public Parser<T> Parser { get; private set; }

        /// <summary>
        /// Gets the minimum number of repetitions to match for this parser to succeed.
        /// </summary>
        public int MinRepetitions { get; private set; }

        /// <summary>
        /// Gets the maximum number of repetitions to match.
        /// </summary>
        public int MaxRepetitions { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser">The <see cref="Parser{T}"/> to repeat.</param>
        /// <param name="minRepetitions">The minimum number of repetitions to match for this parser to succeed.</param>
        /// <param name="maxRepetitions">The maximum number of repetitions to match.</param>
        public RepeatParser(Parser<T> parser, int minRepetitions, int maxRepetitions)
        {
            Parser = parser;
            MinRepetitions = minRepetitions;
            MaxRepetitions = maxRepetitions;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<IReadOnlyList<T>> Parse(string input, int index)
        {
            var results = new List<T>();
            var i = index;
            var sb = new StringBuilder();
            while (results.Count <= MaxRepetitions)
            {
                var result = Parser.Parse(input, i);
                if (!result.Success)
                {
                    break;
                }
                results.Add(result.Value);
                sb.Append(result.Text);
                i += result.Text.Length;
            }

            return (results.Count >= MinRepetitions)
                ? (IParseResult<IReadOnlyList<T>>)new ParseSuccess<IReadOnlyList<T>>(sb.ToString(), results, index)
                : new ParseFail<IReadOnlyList<T>>(index, 
                    string.Concat("Expected to match ", Parser, " at least ", MinRepetitions, " times. Actually matched ", results.Count, " times.")); 
        }

        /// <summary>
        /// Create a new <see cref="RepeatSeparatorParser{T, TSep}"/> that will
        /// match and ignore the given separator in between repetitions.
        /// </summary>
        /// <typeparam name="TSep">Type of <see cref="Parser{TSep}"/> used to separate each repetition.</typeparam>
        /// <param name="separator">The <see cref="Parser{T}"/> used to separate each repetition.</param>
        /// <returns>A new <see cref="RepeatSeparatorParser{T, TSep}"/> that will match repetitions with an ignored separator.</returns>
        public Parser<IReadOnlyList<T>> WithSep<TSep>(Parser<TSep> separator)
        {
            return new RepeatSeparatorParser<T, TSep>(Parser, separator, MinRepetitions, MaxRepetitions);
        }

        /// <summary>
        /// Create a new <see cref="RepeatSeparatorParser{T, TSep}"/> that will
        /// match and ignore the given text in between repetitions.
        /// </summary>
        /// <param name="text">The text used to separate each repetition.</param>
        /// <returns>A new <see cref="RepeatSeparatorParser{T, TSep}"/> that will match repetitions with an ignored separator.</returns>
        public Parser<IReadOnlyList<T>> WithSep(string text)
        {
            return new RepeatSeparatorParser<T, string>(Parser, new LiteralParser(text), MinRepetitions, MaxRepetitions);
        }

        /// <summary>
        /// Create a new <see cref="RepeatSeparatorParser{T, TSep}"/> that will
        /// match and ignore a regular expression created from the given pattern in between repetitions.
        /// </summary>
        /// <param name="pattern">The pattern that defines a regular expression used to separate each repetition.</param>
        /// <returns>A new <see cref="RepeatSeparatorParser{T, TSep}"/> that will match repetitions with an ignored separator.</returns>
        public Parser<IReadOnlyList<T>> WithSepPattern(string pattern)
        {
            return new RepeatSeparatorParser<T, string>(Parser, new RegexParser(pattern), MinRepetitions, MaxRepetitions);
        }

        /// <summary>
        /// Create a new <see cref="RepeatSeparatorParser{T, TSep}"/> that will
        /// match and ignore a regular expression in between repetitions.
        /// </summary>
        /// <param name="regex">The regular expression used to separate each repetition.</param>
        /// <returns>A new <see cref="RepeatSeparatorParser{T, TSep}"/> that will match repetitions with an ignored separator.</returns>
        public Parser<IReadOnlyList<T>> WithSep(Regex regex)
        {
            return new RepeatSeparatorParser<T, string>(Parser, new RegexParser(regex), MinRepetitions, MaxRepetitions);
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
            string repeatString;
            if (MinRepetitions > 0 && MaxRepetitions == int.MaxValue)
            {
                repeatString = string.Concat("[", MinRepetitions, "- ]");
            }
            else if (MinRepetitions <= 0 && MaxRepetitions < int.MaxValue)
            {
                repeatString = string.Concat("[ -", MaxRepetitions, "]");
            }
            else
            {
                repeatString = string.Concat("[", MinRepetitions, "-", MaxRepetitions, "]");
            }
            return string.Concat("Repeat", repeatString, "(", Parser, ")");
        }
    }
}
