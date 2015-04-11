/* ============================================================================
 * ParserCombinators
 * ----------------------------------------------------------------------------
 * FailureType.cs
 *   Created on 4/11/2015 @ 1:27 PM
 *   Written by kchaloux
 * ========================================================================= */

using System.ComponentModel;

namespace ParserCombinators
{
    /// <summary>
    /// Enumeration for different types of failure.
    /// </summary>
    public enum FailureType
    {
        /// <summary>
        /// A general failure to match the parser.
        /// </summary>
        Parsing,

        /// <summary>
        /// A failure to convert the parsed data.
        /// </summary>
        Conversion,

        /// <summary>
        /// A failure to terminate when expected.
        /// </summary>
        Termination,

        /// <summary>
        /// A failure for matching an <see cref="InverseParser{T}"/>.
        /// </summary>
        Inversion,
    }
}
