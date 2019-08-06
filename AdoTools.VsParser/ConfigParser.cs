using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using AdoTools.Common;
using AdoTools.VsParser.Entities;
using AdoTools.VsParser.Entities.ConfigFile;
using AdoTools.VsParser.Entities.ConfigFile.Components;
using AdoTools.VsParser.Entities.DataConfigurationFile;
using AdoTools.VsParser.Entities.DataConfigurationFile.Components;

// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("AdoTools.VsParser.Tests")]

namespace AdoTools.VsParser
{
    /// <summary>
    ///     Parses Config content from App.config, web.config, dataConfiguration.config.
    /// </summary>
    public static class ConfigParser
    {
        /// <summary>
        ///     Parses the Content of an app.config File
        /// </summary>
        /// <param name="content">File Content</param>
        /// <param name="path">Path or Filename</param>
        /// <returns>Parsed Output</returns>
        public static AppConfigFile ParseAppConfigFileContent(string content, string path)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            var output = new AppConfigFile();

            var document = XElement.Parse(content);

            output.ApplicationSettings = GetApplicationSettings(document);
            var environment = GetEnvironmentFromPath(path);
            SetDeployEnvironment(output.ApplicationSettings, environment);

            return output;
        }

        /// <summary>
        ///     Parses the Content of a dataConfiguration.config file
        /// </summary>
        /// <param name="content">File Content</param>
        /// <param name="path">Path or Filename</param>
        /// <returns>Parsed Result</returns>
        public static DataConfigurationFile ParseDataConfigurationFileContent(string content, string path)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            var output = new DataConfigurationFile();

            var document = XElement.Parse(content);

            var environment = GetEnvironmentFromPath(path);

            output.ConnectionStrings = GetConnectionStringSettings(document);
            SetDeployEnvironment(output.ConnectionStrings, environment);

            output.DatabaseInstances = GetDatabaseInstances(document);
            SetDeployEnvironment(output.DatabaseInstances, environment);

            output.DatabaseTypes = GetDatabaseTypes(document);
            SetDeployEnvironment(output.DatabaseTypes, environment);

            return output;
        }

        /// <summary>
        ///     Parse an AppSettings*.json File
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static AppSettingsJsonFile ParseAppSettingsJsonFileContent(string content)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            var output = JsonConvert.DeserializeObject<AppSettingsJsonFile>(content);

            return output;
        }

        /// <summary>
        ///     Parse the content of a web.config file.
        /// </summary>
        /// <param name="content">File Content to be Parsed</param>
        /// <param name="path">Path or filename</param>
        /// <returns>Parsed Output</returns>
        public static WebConfigFile ParseWebConfigFileContent(string content, string path)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            var output = new WebConfigFile();

            var document = XElement.Parse(content);

            output.AppSettings = GetAppSettings(document);

            var environment = GetEnvironmentFromPath(path);
            SetDeployEnvironment(output.AppSettings, environment);

            return output;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        internal static DeployEnvironment GetEnvironmentFromPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path)
                && !path.EndsWith(".config", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("Value must end with .config", nameof(path));
            }

            var output = DeployEnvironment.None;

            if (!string.IsNullOrWhiteSpace(path))
            {
                var match = Regex.Match(path, @"\.([^.]+)\.config$", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    output = DeployEnvironmentHelper.Map(match.Groups[1].Value);
                }
            }

            return output;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        internal static void SetDeployEnvironment(
            IEnumerable<ISettingWithEnvironment> settings,
            DeployEnvironment environment)
        {
            foreach (var setting in settings)
            {
                setting.Environment = environment;
            }
        }

        /// <summary>
        ///     Gets settings from an app.config file.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static List<ApplicationSetting> GetApplicationSettings(XContainer document)
        {
            var output = new List<ApplicationSetting>();

            var baseNode = document.Descendants()
                .FirstOrDefault(
                    d => d.Name.LocalName.Equals("applicationSettings", StringComparison.CurrentCultureIgnoreCase));

            if (baseNode != null)
            {
                output.AddRange(
                    baseNode.Descendants()
                        .Where(
                            x => x.Name.LocalName.EndsWith(
                                "Properties.Settings",
                                StringComparison.CurrentCultureIgnoreCase))
                        .SelectMany(
                            settingsGroup => settingsGroup.Descendants()
                                .Where(
                                    x => x.Name.LocalName.Equals("setting", StringComparison.CurrentCultureIgnoreCase)),
                            (_, setting) => new ApplicationSetting(
                                setting.Attribute("name")?.Value,
                                setting.Descendants()
                                    .FirstOrDefault(
                                        x => x.Name.LocalName.Equals(
                                            "value",
                                            StringComparison.CurrentCultureIgnoreCase))
                                    ?.Value,
                                setting.Attribute("serializeAs")?.Value
                            )));
            }

            return output;
        }

        /// <summary>
        ///     Gets settings from a web.config file.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static List<ApplicationSetting> GetAppSettings(XContainer document)
        {
            var output = new List<ApplicationSetting>();

            var baseNode = document.Descendants()
                .FirstOrDefault(d => d.Name.LocalName.Equals("appSettings", StringComparison.CurrentCultureIgnoreCase));

            if (baseNode != null)
            {
                output.AddRange(
                    baseNode.Descendants("add")
                        .Select(
                            setting => new ApplicationSetting(
                                setting.Attribute("key")?.Value,
                                setting.Attribute("value")?.Value)
                        ));
            }

            return output;
        }

        private static List<ConnectionStringSetting> GetConnectionStringSettings(XContainer document)
        {
            var output = new List<ConnectionStringSetting>();

            var baseNode = document.Descendants()
                .FirstOrDefault(
                    d => d.Name.LocalName.Equals("connectionStrings", StringComparison.CurrentCultureIgnoreCase));

            if (baseNode != null)
            {
                foreach (var connectionString in baseNode.Descendants()
                    .Where(x => x.Name.LocalName.Equals("connectionString", StringComparison.CurrentCultureIgnoreCase)))
                {
                    var newItem = new ConnectionStringSetting
                    {
                        Name = connectionString.Attribute("name")?.Value
                    };

                    foreach (var parameter in connectionString.Descendants()
                        .Where(x => x.Name.LocalName.Equals("parameter")))
                    {
                        var name = parameter.Attribute("name")?.Value;
                        var value = parameter.Attribute("value")?.Value;

                        if (name != null)
                        {
                            switch (name.ToLower())
                            {
                                case "server":
                                    newItem.Server = value;

                                    break;

                                case "database":
                                    newItem.Database = value;

                                    break;

                                case "user id":
                                    newItem.UserId = value;

                                    break;

                                case "password":
                                    newItem.Password = value;

                                    break;

                                case "application name":
                                    newItem.ApplicationName = value;

                                    break;

                                default:

                                    throw new InvalidOperationException($"Parameter {name} is not supported.");
                            }
                        }
                    }

                    output.Add(newItem);
                }
            }

            return output;
        }

        private static List<DatabaseInstance> GetDatabaseInstances(XContainer document)
        {
            var output = new List<DatabaseInstance>();

            var baseNode = document.Descendants().FirstOrDefault(d => d.Name.LocalName.Equals("instances"));

            if (baseNode != null)
            {
                output.AddRange(
                    baseNode.Descendants()
                        .Select(
                            instance => new DatabaseInstance
                            {
                                Name = instance.Attribute("name")?.Value,
                                Type = instance.Attribute("type")?.Value
                            }));
            }

            return output;
        }

        private static List<DatabaseType> GetDatabaseTypes(XContainer document)
        {
            var output = new List<DatabaseType>();

            var baseNode = document.Descendants()
                .FirstOrDefault(
                    d => d.Name.LocalName.Equals("databaseTypes", StringComparison.CurrentCultureIgnoreCase));

            if (baseNode != null)
            {
                output.AddRange(
                    baseNode.Descendants()
                        .Select(
                            databaseType => new DatabaseType
                            {
                                Name = databaseType.Attribute("name")?.Value,
                                Type = databaseType.Attribute("type")?.Value
                            }));
            }

            return output;
        }
    }
}