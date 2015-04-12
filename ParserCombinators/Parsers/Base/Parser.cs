/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * Parser.cs
 *   Created on 1/1/2015 @ 3:53 PM
 *   Written by kchaloux
 * ========================================================================= */

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ParserCombinators
{
    /// <summary>
    /// A common base for all Parser types.
    /// </summary>
    /// <typeparam name="T">Type of data being parsed.</typeparam>
    public abstract class Parser<T>
    {
        /// <summary>
        /// Attempts to match an input string starting at the given index.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <param name="index">Index to begin matching at.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        public abstract IParseResult<T> Parse(string input, int index);

        /// <summary>
        /// Attempts to match an input string, starting at the first character.
        /// </summary>
        /// <param name="input">String to match.</param>
        /// <returns>An <see cref="IParseResult{T}"/> containing the parsed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParseResult<T> Parse(string input)
        {
            return Parse(input, 0);
        }

        /// <summary>
        /// Return a new <see cref="InverseParser{T}"/> that will
        /// fail if this parser succeeds.
        /// </summary>
        /// <returns>An <see cref="InverseParser{T}"/> wrapping this parser.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<T> Not()
        {
            return new InverseParser<T>(this);
        }

        /// <summary>
        /// Return a new <see cref="OptionalParser{T}"/> that
        /// will make matching this parser optional.
        /// </summary>
        /// <returns>An <see cref="OptionalParser{T}"/> wrapping this parser.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<T> Optional()
        {
            return new OptionalParser<T>(this);
        }

        /// <summary>
        /// Creates a new <see cref="SequenceParser{T1, T2}"/> that will
        /// chain this together with another parser, so that each much be
        /// matched in sequence to succeed.
        /// </summary>
        /// <typeparam name="TNext">Type of <see cref="Parser{T}"/> to chain in sequence with this one.</typeparam>
        /// <param name="next">The next parser to chain in sequence with this one.</param>
        /// <returns>A new <see cref="SequenceParser{T1, T2}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<SequenceResult<T, TNext>> Then<TNext>(Parser<TNext> next)
        {
            return new SequenceParser<T, TNext>(this, next);
        }

        /// <summary>
        /// Creates a new <see cref="SequenceParser{T1, T2}"/> that will
        /// chain this together with another parser, so that each much be
        /// matched in sequence to succeed.
        /// </summary>
        /// <param name="text">The literal text to parse next in the chain.</param>
        /// <returns>A new <see cref="SequenceParser{T1, T2}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<SequenceResult<T, string>> Then(string text)
        {
            return new SequenceParser<T, string>(this, new LiteralParser(text));
        }

        /// <summary>
        /// Creates a new <see cref="SequenceParser{T1, T2}"/> that will
        /// chain this together with another parser, so that each much be
        /// matched in sequence to succeed.
        /// </summary>
        /// <param name="regex">A regular expression to parse next in the chain.</param>
        /// <returns>A new <see cref="SequenceParser{T1, T2}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<SequenceResult<T, string>> Then(Regex regex)
        {
            return new SequenceParser<T, string>(this, new RegexParser(regex));
        }

        /// <summary>
        /// Creates a new <see cref="SequenceParser{T1, T2}"/> that will
        /// chain this together with another parser, so that each much be
        /// matched in sequence to succeed.
        /// </summary>
        /// <param name="pattern">A pattern defining regular expression to parse next in the chain.</param>
        /// <returns>A new <see cref="SequenceParser{T1, T2}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<SequenceResult<T, string>> ThenPattern(string pattern)
        {
            return new SequenceParser<T, string>(this, new RegexParser(pattern));
        }

        /// <summary>
        /// Creates a new <see cref="OrParser{T}"/> that will
        /// attempt to match the given parser at the current index
        /// if it fails to match this one.
        /// </summary>
        /// <param name="or">Other parser to match upon failing this parser.</param>
        /// <returns>A new <see cref="OrParser{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Parser<T> Or(Parser<T> or)
        {
            return new OrParser<T>(new[] { this, or });
        }

        /// <summary>
        /// MakeLazy a new <see cref="RepeatParser{T}"/> that will
        /// repeatedly match this parser as many times as possible.
        /// </summary>
        /// <returns>A <see cref="RepeatParser{T}"/> that matches this parser as many times as possible.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RepeatParser<T> Repeat()
        {
            return new RepeatParser<T>(this, 0, int.MaxValue);
        }

        /// <summary>
        /// MakeLazy a new <see cref="RepeatParser{T}"/> that
        /// will repeatedly match this parser once or more.
        /// </summary>
        /// <returns>A <see cref="RepeatParser{T}"/> that matches this parser once or more.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RepeatParser<T> Repeat1()
        {
            return new RepeatParser<T>(this, 1, int.MaxValue);
        }

        /// <summary>
        /// Creates a new <see cref="ConversionParser{T, TOut}"/> that will
        /// attempt to convert the result of this parser into a new form.
        /// </summary>
        /// <typeparam name="TOut">Type of data that <see cref="ConversionParser{T, TOut}"/> will yield.</typeparam>
        /// <param name="convert">Function to convert this result into its desired form.</param>
        /// <returns>A new <see cref="ConversionParser{T, TOut}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser<TOut> As<TOut>(Func<T, TOut> convert)
        {
            return new ConversionParser<T, TOut>(this, convert);
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
        public virtual Parser<T> End()
        {
            return new EndParser<T>(this);
        }
    }
}