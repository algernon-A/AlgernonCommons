// <copyright file="StandalonePanelBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using System;
    using AlgernonCommons;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Standalone in-game panel template.
    /// </summary>
    public abstract class StandalonePanelBase : UIPanel
    {
        /// <summary>
        /// Layout margin.
        /// </summary>
        public const float Margin = 5f;

        /// <summary>
        /// Close event handler delegate.
        /// </summary>
        public delegate void CloseEventHandler();

        /// <summary>
        /// Exception occured event.
        /// </summary>
        public event CloseEventHandler EventClose;

        /// <summary>
        /// Gets the panel width.
        /// </summary>
        public abstract float PanelWidth { get; }

        /// <summary>
        /// Gets the panel height.
        /// </summary>
        public abstract float PanelHeight { get; }

        /// <summary>
        /// Gets a value indicating whether the panel's previous position should be remembered after closing.
        /// </summary>
        public virtual bool RememberPosition => true;

        /// <summary>
        /// Gets the panel opacity.
        /// </summary>
        protected virtual float PanelOpacity => 0f;

        /// <summary>
        /// Called by Unity when the object is created.
        /// Used to perform setup.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Size.
            size = new Vector2(PanelWidth, PanelHeight);

            // Set default position.
            ApplyDefaultPosition();
        }

        /// <summary>
        /// Closes the panel.
        /// </summary>
        public void Close()
        {
            try
            {
                // Perform any pre-close actions.
                if (!PreClose())
                {
                    // Not yet safe to close; do nothing.
                    return;
                }
            }
            catch (Exception e)
            {
                // We don't want any exceptions stopping us from closing the panel.
                Logging.LogException(e, "exception closing StandalonePanel");
            }

            // Invoke close event.
            EventClose?.Invoke();
        }

        /// <summary>
        /// Performs any actions required before closing the panel and checks that it's safe to do so.
        /// </summary>
        /// <returns>True if the panel can close now, false otherwise.</returns>
        protected virtual bool PreClose() => true;

        /// <summary>
        /// Gets the panel's default position.
        /// </summary>
        protected virtual void ApplyDefaultPosition()
        {
            relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
        }
    }
}