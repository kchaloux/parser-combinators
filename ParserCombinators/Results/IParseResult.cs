/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * IParseResult.cs
 *   Created on 1/1/2015 @ 4:03 PM
 *   Written by kchaloux
 * ========================================================================= */

namespace ParserCombinators
{
    /// <summary>
    /// Interface for a class that contains the results of a parsing operation.
    /// </summary>
    /// <typeparam name="T">Type of data that was parsed.</typeparam>
    public interface IParseResult<out T>
    {
        /// <summary>
        /// Gets the text that was parsed, if successful.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the index that parsing began on.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the length of the text that was parsed.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets whether or not the parsing operation was successful.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Gets the final converted value of a successfully parsed string.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the message attached to this result.
        /// </summary>
        string Message { get; }
    }
}
