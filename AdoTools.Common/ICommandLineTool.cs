using System;
using System.Diagnostics;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace AdoTools.Common
{
    /// <summary>
    ///     Interface for the CommandLineTool
    /// </summary>
    public interface ICommandLineTool
    {
        /// <summary>
        ///     Execute a command in a process.
        /// </summary>
        /// <param name="command">The command or filename to be executed.</param>
        /// <param name="arguments">Optional arguments in a string.</param>
        /// <param name="workingDirectory">Optional working directory.</param>
        /// <param name="waitMilliseconds">
        ///     The maximum number of milliseconds to wait for the process to exit.
        ///     If the process hasn't existed before this time elapses, it will be killed.
        ///     Set to zero to not wait and simply return the created process.
        /// </param>
        /// <returns>The process created.</returns>
        Process ExecuteCommand(string command, string arguments = "", string workingDirectory = null,
            int waitMilliseconds = 60000);
    }
}