using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.WebApi;

#pragma warning disable 1591

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace AdoTools.Ado.Entities
{
    /// <summary>
    ///     Task Group
    /// </summary>
    [Obsolete("Find official definition in Microsoft API")]
    public class TaskGroupX
    {
        public string Category { get; set; }

        public string Comment { get; set; }

        public object CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string DefinitionType { get; set; }

        public IEnumerable<TaskGroupDefinitionX> Groups { get; set; }

        public string Id { get; set; }

        public string InstanceNameFormat { get; set; }

        public object ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string Name { get; set; }

        public int Revision { get; set; }

        public IEnumerable<string> RunsOn { get; set; }

        public IEnumerable<TaskGroupStepX> Tasks { get; set; }

        public object Version { get; set; }
    }

    /// <summary>
    ///     Task Group Definition
    /// </summary>
    [Obsolete("Find official definition in Microsoft API")]
    public class TaskGroupDefinitionX
    {
        public string DisplayName { get; set; }

        public bool IsExpanded { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string VisibleRule { get; set; }
    }

    /// <summary>
    ///     Task Group Step
    /// </summary>
    [Obsolete("Find official definition in Microsoft API")]
    public class TaskGroupStepX
    {
        public bool AlwaysRun { get; set; }

        public string Condition { get; set; }

        public bool ContinueOnError { get; set; }

        public string DisplayName { get; set; }

        public bool Enabled { get; set; }

        public object Inputs { get; set; }

        public TaskDefinitionReference Task { get; set; }

        public int TimeoutInMinutes { get; set; }
    }
}