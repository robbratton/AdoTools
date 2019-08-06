using System;
using NLog;

namespace AdoTools.Ado
{
    /// <summary>
    ///     Base for all tools that use Logging
    /// </summary>
    public class LoggerBase
    {
        #region Protected Properties and Fields

        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        protected static Logger Logger;

        #endregion Protected Properties and Fields

        /*
         * For testing:
         * - Some of the methods and properties in this class are marked as internal instead of protected.
         * - The class can't be marked abstract.
         */

        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        protected LoggerBase(Logger logger = null)
        {
            Logger = logger ?? LogManager.GetCurrentClassLogger();

            Logger.Trace("Entering");

            Logger.Trace("Exiting");
        }

        #endregion Constructors
    }
}