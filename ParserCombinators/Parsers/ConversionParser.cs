/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * ConversionParser.cs
 *   Created on 1/3/2015 @ 5:43 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;

namespace ParserCombinators
{
    /// <summary>
    /// A wrapper around another <see cref="Parser{T}"/> that converts its
    /// result to another format using a given function.
    /// </summary>
    /// <typeparam name="TIn">Type of the original parser.</typeparam>
    /// <typeparam name="TOut">Type of the converted output.</typeparam>
    public class ConversionParser<TIn, TOut> : Parser<TOut>
    {
        #region Properties
        /// <summary>
        /// Gets the Parser to convert the result of.
        /// </summary>
        public Parser<TIn> Parser { get; private set; }

        private readonly Func<TIn, TOut> _convert;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser">Parser to convert the result of.</param>
        /// <param name="convert">Function used to convert the result of the given parser with.</param>
        public ConversionParser(Parser<TIn> parser, Func<TIn, TOut> convert)
        {
            Parser = parser;
            _convert = convert;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<TOut> Parse(string input, int index)
        {
            var result = Parser.Parse(input, index);
            if (!result.Success)
            {
                return new ParseFail<TOut>(index, result.Message);
            }

            try
            {
                return new ParseSuccess<TOut>(result.Text, _convert(result.Value), index);
            }
            catch (Exception ex)
            {
                return new ParseFail<TOut>(index, string.Concat("Failed to convert ", Parser, ": ", ex));
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
            return Parser.ToString();
        }
    }
}
