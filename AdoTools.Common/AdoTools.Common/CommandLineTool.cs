using System;
using System.Diagnostics;
using System.IO;
// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     Tool for Executing Command-Line Items
    /// </summary>
    public class CommandLineTool : ICommandLineTool
    {
        /// <inheritdoc />
        public Process ExecuteCommand(string command, string arguments = "", string workingDirectory = null,
            int waitMilliseconds = 60000)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException($"{nameof(command)} must not be null or whitespace.", nameof(command));
            }

            if (waitMilliseconds < 0)
            {
                throw new ArgumentException($"{nameof(waitMilliseconds)} must be a positive integer including zero.",
                    nameof(waitMilliseconds));
            }

            if (workingDirectory != null && !Directory.Exists(workingDirectory))
            {
                throw new ArgumentException($"{nameof(workingDirectory)} must exist.", nameof(workingDirectory));
            }

            var processInfo = new ProcessStartInfo(command, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            if (workingDirectory != null)
            {
                processInfo.WorkingDirectory = workingDirectory;
            }

            var output = Process.Start(processInfo);

            if (output != null && !output.HasExited && waitMilliseconds > 0)
            {
                var waitResult = output.WaitForExit(waitMilliseconds);
                if (!waitResult)
                {
                    output.Kill();
                }
            }

            return output;
        }
    }
}