using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

// ReSharper disable UnusedMember.Global

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     Implements exceptions for VsTsTool code
    /// </summary>
    [Serializable]
    public class VsTsToolNotFoundException : VsTsToolException
    {        /// <inheritdoc />
        public VsTsToolNotFoundException()
        {
        }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.</summary>
        /// <param name="message">The message that describes the error. </param>
        public VsTsToolNotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a
        ///     reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (
        ///     <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
        /// </param>
        public VsTsToolNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.</summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        ///     The class name is <see langword="null" /> or
        ///     <see cref="P:System.Exception.HResult" /> is zero (0).
        /// </exception>
        protected VsTsToolNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}