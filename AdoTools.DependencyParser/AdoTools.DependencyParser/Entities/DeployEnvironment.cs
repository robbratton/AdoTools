using System;

namespace Upmc.DevTools.Dependency.Parser.Entities
{
    /// <summary>
    ///     The environment where the item will be deployed
    /// </summary>
    public enum DeployEnvironment
    {
        /// <summary>
        ///     Undefined
        /// </summary>
        None,

        /// <summary>
        ///     Development, Dev
        /// </summary>
        Development,

        /// <summary>
        ///     Lab
        /// </summary>
        Lab,

        /// <summary>
        ///     Quality Assurance
        /// </summary>
        Qa,

        /// <summary>
        ///     Production Support
        /// </summary>
        ProdSupport,

        /// <summary>
        ///     Training
        /// </summary>
        Training,

        /// <summary>
        ///     Production
        /// </summary>
        Production
    }
}