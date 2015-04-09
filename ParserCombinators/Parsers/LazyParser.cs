/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * LazyParser.cs
 *   Created on 2/17/2015 @ 9:05 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;

namespace ParserCombinators
{
    /// <summary>
    /// A parser that defers the evaluation of its
    /// inner parser until its first call to <see cref="Parse(string, int)"/>.
    /// </summary>
    public class LazyParser<T> : Parser<T>
    {
        private readonly Func<Parser<T>> _generate;
        private Parser<T> _parser;
        private bool _isParserGenerated;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="generate">A function used to generate an instance of a parser.</param>
        public LazyParser(Func<Parser<T>> generate)
        {
            _generate = generate;
            _parser = null;
            _isParserGenerated = false;
        }

        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public override IParseResult<T> Parse(string input, int index)
        {
            if (!_isParserGenerated)
            {
                _parser = _generate();
                _isParserGenerated = true;
            }

            return _parser.Parse(input, index);
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
            return "[Lazy]";
        }
    }
}
