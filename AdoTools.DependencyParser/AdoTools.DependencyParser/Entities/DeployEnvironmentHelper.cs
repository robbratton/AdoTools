using System;
using NLog;
using Upmc.DevTools.Common;

namespace Upmc.DevTools.Dependency.Parser.Entities
{
    /// <summary>
    ///     Methods to help with using the DeployEnvironment enumeration
    /// </summary>
    public static class DeployEnvironmentHelper
    {
        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Maps a string environment to <see cref="DeployEnvironment" />
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>
        ///     <see cref="DeployEnvironment" />
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static DeployEnvironment MapEnvironment(string environment)
        {
            Validators.AssertIsNotNullOrWhitespace(environment, nameof(environment));

            Logger.Trace("Entering");

            DeployEnvironment output;

            switch (environment.ToLower())
            {
                case "debug":
                    output = DeployEnvironment.None;

                    break;

                case "dev":
                case "development":
                    output = DeployEnvironment.Development;

                    break;

                case "lab":
                    output = DeployEnvironment.Lab;

                    break;

                case "prod":
                    output = DeployEnvironment.Production;

                    break;

                // ReSharper disable once StringLiteralTypo
                case "prodsupport":
                    output = DeployEnvironment.ProdSupport;

                    break;

                case "qa":
                    output = DeployEnvironment.Qa;

                    break;

                case "training":
                    output = DeployEnvironment.Training;

                    break;

                default:

                    throw new ArgumentException(nameof(environment));
            }

            return output;
        }
    }
}