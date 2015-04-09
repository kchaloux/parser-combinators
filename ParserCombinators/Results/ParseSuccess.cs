/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * ParseSuccess.cs
 *   Created on 1/1/2015 @ 4:04 PM
 *   Written by kchaloux
 * ========================================================================= */

namespace ParserCombinators
{
    /// <summary>
    /// Represents the result of a successful parsing operation.
    /// </summary>
    /// <typeparam name="T">Type of data that was parsed.</typeparam>
    public sealed class ParseSuccess<T> : IParseResult<T>
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
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the final converted value of a successfully parsed string.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the message attached to this result.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text that was matched.</param>
        /// <param name="value">Value of the parsed text.</param>
        /// <param name="index">Index that the parsing operation began on.</param>
        public ParseSuccess(string text, T value, int index)
        {
            Text = text;
            Value = value;
            Index = index;
            Length = text.Length;
            Success = true;
            Message = "";
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
            return string.Concat("[Success: ", Value, "]");
        }
    }
}
