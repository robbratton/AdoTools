using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using NLog;
using Upmc.DevTools.VsParser.Entities.ConfigFile;
using Upmc.DevTools.VsParser.Entities.DataConfigurationFile.Components;

namespace Upmc.DevTools.Dependency.Parser
{
    /// <summary>
    ///     Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        private static readonly Regex RedisConnectionStringPattern = new Regex(@"^([^:,]+(:[0-9]+)?)(,password=(.+))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Converts an AppSettingsJsonFile's ConnectionStrings into a set of ConnectionStringSettings
        /// </summary>
        /// <param name="input">AppSettingsJsonFile</param>
        /// <returns>Collection of ConnectionStringSettings</returns>
        public static IEnumerable<ConnectionStringSetting> GetConnectionStrings(this AppSettingsJsonFile input)
        {
            Logger.Trace("Entering");

            var output = new List<ConnectionStringSetting>();

            foreach (var cs in input.ConnectionStrings)
            {
                var name = cs.Key;
                var connectionString = cs.Value;
                try
                {
                    var builder = new SqlConnectionStringBuilder(connectionString);

                    var newItem = new ConnectionStringSetting
                    {
                        ApplicationName = builder.ApplicationName,
                        Database = builder.InitialCatalog,
                        Name = name,
                        Password = builder.Password,
                        Server = builder.DataSource,
                        UserId = builder.UserID
                    };

                    output.Add(newItem);
                }
                catch (ArgumentException)
                {
                    // RedisConnection follows the format localhost[:999][,password=xxx].
                    if (name.Equals("RedisConnection", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var matches = RedisConnectionStringPattern.Match(connectionString);
                        if (matches.Success)
                        {
                            var server = matches.Groups[1].Value;
                            var password = matches.Groups[4].Value;

                            var newItem = new ConnectionStringSetting
                            {
                                Name = name,
                                Password = password,
                                Server = server
                            };

                            output.Add(newItem);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return output;
        }
    }
}