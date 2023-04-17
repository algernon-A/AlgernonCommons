// <copyright file="OptionsPanelBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using ColossalFramework.UI;

    /// <summary>
    /// Mod options panel base class.
    /// </summary>
    public abstract class OptionsPanelBase : UIPanel
    {
        // Setup status.
        private bool _setup = false;

        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Called by the game when visibility changes.
        /// </summary>
        protected override void OnVisibilityChanged()
        {
            // Setup panel if we haven't already.
            if (!_setup && isVisible)
            {
                Setup();

                // Update status.
                _setup = true;
            }

            base.OnVisibilityChanged();
        }
    }
}