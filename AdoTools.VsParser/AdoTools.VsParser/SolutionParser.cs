using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Upmc.DevTools.Common;
using Upmc.DevTools.VsParser.Entities;
using Upmc.DevTools.VsParser.Entities.Project;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser
{
    /// <summary>
    ///     Parses solution (*.sln) content.
    /// </summary>
    public class SolutionParser
    {
        private readonly List<SectionParseInfo> _parseInformation;

        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public SolutionParser()
        {
            // Set Up Parsers
            _parseInformation = new List<SectionParseInfo>
            {
                new SectionParseInfo("^Microsoft Visual Studio Solution File", SolutionFileLineParser),
                new SectionParseInfo("^VisualStudioVersion", VisualStudioVersionLineParser),
                new SectionParseInfo("^MinimumVisualStudioVersion", MinVisualStudioVersionLineParser),
                new SectionParseInfo("^Project", "^EndProject", ProjectLinesParser)
            };
        }

        #endregion Constructors

        /// <summary>
        ///     Parse a solution file's content and return the
        ///     parsed output.
        /// </summary>
        /// <param name="content">
        ///     Content from a Visual Studio solution file.
        /// </param>
        /// <returns></returns>
        public Solution ParseSolutionFileContent(string content)
        {
            Validators.AssertIsNotNullOrWhitespace(content, nameof(content));

            var output = new Solution();

            var lines = new Queue<string>(content.Split('\r', '\n'));

            var inSection = false;
            SectionParseInfo parseInfo = null;
            var isSingleLine = true;
            var sectionContent = new StringBuilder();

            while (lines.Count > 0)
            {
                var line = lines.Dequeue();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var matchingStartParseInfo = _parseInformation.FirstOrDefault(p => p.StartPattern.IsMatch(line));

                if (matchingStartParseInfo != null)
                {
                    inSection = true;
                    parseInfo = matchingStartParseInfo;
                    isSingleLine = matchingStartParseInfo.IsSingleLine;
                }

                if (inSection)
                {
                    sectionContent.AppendLine(line);

                    if (isSingleLine
                        || _parseInformation.Any(
                            p => !p.IsSingleLine && p.EndPattern != null && p.EndPattern.IsMatch(line)))
                    {
                        // This is the last line of a section.

                        // ParseFileContent the previous
                        // section, if any.
                        if (sectionContent.Length > 0)
                        {
                            parseInfo.Parser(output, sectionContent.ToString());
                        }

                        // Start the next section.
                        sectionContent = new StringBuilder();
                        parseInfo = null;
                        inSection = false;
                    }

                    // ReSharper disable once RedundantIfElseBlock
                    else
                    {
                        // Continue processing lines in the
                        // current section.
                    }
                }

                // ReSharper disable once RedundantIfElseBlock
                else
                {
                    // Ignore lines not in a defined section.
                }
            }

            return output;
        }

        private class SectionParseInfo
        {
            /// <summary>
            ///     Constructor for single-line items
            /// </summary>
            /// <param name="startPattern">
            ///     The regular expression for the first line of
            ///     the section
            /// </param>
            /// <param name="parser">
            ///     The parser to process the section
            /// </param>
            public SectionParseInfo(string startPattern, Action<Solution, string> parser)
            {
                StartPattern = new Regex(startPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                IsSingleLine = true;
                Parser = parser;
            }

            /// <param name="startPattern">
            ///     The regular expression for the first line of
            ///     the section
            /// </param>
            /// <param name="endPattern">
            ///     The regular expression for the last line of
            ///     the section
            /// </param>
            /// <param name="parser">
            ///     The parser to process the section
            /// </param>
            /// <summary>Constructor</summary>
            public SectionParseInfo(string startPattern, string endPattern, Action<Solution, string> parser)
            {
                StartPattern = new Regex(startPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                EndPattern = new Regex(endPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                IsSingleLine = false;
                Parser = parser;
            }

            /// <summary>
            ///     The pattern that exists on the last line of a
            ///     section, if any
            /// </summary>
            public Regex EndPattern { get; }

            /// <summary>
            ///     Indicates if the section is only a single line.
            /// </summary>
            public bool IsSingleLine { get; }

            /// <summary>
            ///     The parser to handle this section
            /// </summary>
            public Action<Solution, string> Parser { get; }

            /// <summary>
            ///     The pattern that exists on the first line of
            ///     a section
            /// </summary>
            public Regex StartPattern { get; }
        }

        #region Section Parsers

        private static void MinVisualStudioVersionLineParser(Solution solution, string content)
        {
            var parseRegex = new Regex(@"^MinimumVisualStudioVersion = ([\d.]+)");

            var result = parseRegex.Match(content);

            if (result.Success)
            {
                solution.MinimumVisualStudioVersion = result.Groups[1].Value;
            }
        }

        private static void ProjectLinesParser(Solution solution, string content)
        {
            var parseRegex = new Regex(@"^Project\(""{(.+)}""\) = ""(.+)"", ""(.+)"", ""{(.+)}""");

            var result = parseRegex.Match(content);

            if (result.Success)
            {
                solution.Projects.Add(
                    new Project
                    {
                        TypeIds = new List<Guid> {Guid.Parse(result.Groups[1].Value)},
                        NameInSolution = result.Groups[2].Value,
                        PathRelativeToSolution = result.Groups[3].Value,
                        Id = Guid.Parse(result.Groups[4].Value)
                    }
                );
            }
        }

        private static void SolutionFileLineParser(Solution solution, string content)
        {
            var parseRegex = new Regex(@"^Microsoft Visual Studio Solution File, Format Version ([\d.]+)");

            var result = parseRegex.Match(content);

            if (result.Success)
            {
                solution.FormatVersion = result.Groups[1].Value;
            }
        }

        private static void VisualStudioVersionLineParser(Solution solution, string content)
        {
            var parseRegex = new Regex(@"^VisualStudioVersion = ([\d.]+)");

            var result = parseRegex.Match(content);

            if (result.Success)
            {
                solution.VisualStudioVersion = result.Groups[1].Value;
            }
        }

        #endregion Section Parsers
    }
}

/* Example file
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.27004.2009
MinimumVisualStudioVersion = 10.0.40219.1
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Upmc.DevTools.CsProject", "Upmc.DevTools.CsProject\Upmc.DevTools.CsProject.csproj", "{A7BACD69-F323-4416-B5D7-B85D7F5CA44C}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Upmc.DevTools.CsProject.Tests", "Upmc.DevTools.CsProject.Tests\Upmc.DevTools.CsProject.Tests.csproj", "{FED7C5D5-46D0-4C69-965C-ECEDCAFD4B3B}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{A7BACD69-F323-4416-B5D7-B85D7F5CA44C}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A7BACD69-F323-4416-B5D7-B85D7F5CA44C}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A7BACD69-F323-4416-B5D7-B85D7F5CA44C}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{A7BACD69-F323-4416-B5D7-B85D7F5CA44C}.Release|Any CPU.Build.0 = Release|Any CPU
		{FED7C5D5-46D0-4C69-965C-ECEDCAFD4B3B}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{FED7C5D5-46D0-4C69-965C-ECEDCAFD4B3B}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{FED7C5D5-46D0-4C69-965C-ECEDCAFD4B3B}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{FED7C5D5-46D0-4C69-965C-ECEDCAFD4B3B}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {7A7230F4-1166-433E-B1EB-484A412BEFB7}
	EndGlobalSection
EndGlobal
*/