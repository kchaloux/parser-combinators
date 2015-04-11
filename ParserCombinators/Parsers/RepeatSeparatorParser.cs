/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * RepeatSeparatorParser.cs
 *   Created on 2/17/2015 @ 9:28 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.Collections.Generic;
using System.Text;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that repeatedly matches another parser, 
    /// with each match separated by an ignored element.
    /// </summary>
    /// <typeparam name="TResult">Type of data being parsed.</typeparam>
    /// <typeparam name="TSeparator">Type of parser that separates each match.</typeparam>
    public class RepeatSeparatorParser<TResult, TSeparator> : Parser<IReadOnlyList<TResult>>
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Parser{T}"/> to repeat.
        /// </summary>
        public Parser<TResult> Parser { get; private set; }

        /// <summary>
        /// Gets the <see cref="Parser{T}"/> that separates each match.
        /// </summary>
        public Parser<TSeparator> Separator { get; private set; }
        
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
        /// <param name="separator">The <see cref="Parser{T}"/> that separates each match.</param>
        /// <param name="minRepetitions">The minimum number of repetitions to match for this parser to succeed.</param>
        /// <param name="maxRepetitions">The maximum number of repetitions to match.</param>
        public RepeatSeparatorParser(Parser<TResult> parser, Parser<TSeparator> separator, int minRepetitions, int maxRepetitions)
        {
            Parser = parser;
            Separator = separator;
            MinRepetitions = minRepetitions;
            MaxRepetitions = maxRepetitions;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<IReadOnlyList<TResult>> Parse(string input, int index)
        {
            var results = new List<TResult>();
            var i = index;
            var sb = new StringBuilder();
            while (results.Count <= MaxRepetitions)
            {
                if (Separator != null && results.Count > 0)
                {
                    var separatorResult = Separator.Parse(input, i);
                    if (!separatorResult.Success)
                    {
                        break;
                    }
                    sb.Append(separatorResult.Text);
                    i += separatorResult.Text.Length;
                }

                var result = Parser.Parse(input, i);
                if (!result.Success)
                {
                    break;
                }
                results.Add(result.Value);
                sb.Append(result.Text);
                i += result.Text.Length;
            }

            if (results.Count >= MinRepetitions)
            {
                return new ParseSuccess<IReadOnlyList<TResult>>(sb.ToString(), results, index);
            }

            return new ParseFail<IReadOnlyList<TResult>>(
                FailureType.Parsing,
                index,
                string.Concat("Expected to match ", Parser, " at least ", MinRepetitions, " times. Actually matched ", results.Count, " times")); 
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
            return string.Concat("Repeat", repeatString, "(", Parser, "/", Separator, ")");
        }
    }
}
