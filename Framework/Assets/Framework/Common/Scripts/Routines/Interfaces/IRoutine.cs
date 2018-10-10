// <copyright file="IRoutine.cs" company="Synergy88 Digital Inc.">
// Copyright (c) Synergy88 Digital Inc. All Rights Reserved.
// </copyright>
// <author>Elmer Nocon</author>

using uPromise;

namespace Framework.Common.Routines
{
    public interface IRoutine
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        Promise Run();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns></returns>
        Promise Stop();

        #endregion Methods
    }
}