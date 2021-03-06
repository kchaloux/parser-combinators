﻿/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * ParseFail.cs
 *   Created on 1/1/2015 @ 4:05 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.ComponentModel;
using System.Linq;

namespace ParserCombinators
{
    /// <summary>
    /// Represents the result of a failed parsing operation.
    /// </summary>
    /// <typeparam name="T">Type of data that failed to parse.</typeparam>
    public sealed class ParseFail<T> : IParseResult<T>
    {
        /// <summary>
        /// Gets the text that was parsed, if successful.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the index that parsing began on.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the length of the text that was parsed.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets whether or not the parsing operation was successful.
        /// </summary>
        public bool Success { get { return false; } }

        /// <summary>
        /// Gets the final converted value of a successfully parsed string.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the message attached to this result.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the general cause of the failure.
        /// </summary>
        public FailureType FailureType { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="failureType">The general cause of the failure.</param>
        /// <param name="index">Index of the string that the parsing operation began on.</param>
        /// <param name="message">A message associated with this failed parsing operation.</param>
        public ParseFail(FailureType failureType, int index, string message = "")
            : this(new ParseSuccess<T>("", default(T), index), failureType, message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">Another result to wrap and convert to a failure.</param>
        /// <param name="failureType">The general cause of the failure.</param>
        /// <param name="message">A message associated with this failed parsing operation.</param>
        public ParseFail(IParseResult<T> result, FailureType failureType, string message = "")
        {
            Text = result.Text;
            Index = result.Index;
            Length = 0;
            Value = result.Value;
            Message = message;
            FailureType = failureType;
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
            return string.Concat("[Failure(", FailureType, "): ", Message, "]");
        }
    }
}
