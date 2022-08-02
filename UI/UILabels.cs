// <copyright file="UILabels.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// UI labels.
    /// </summary>
    public static class UILabels
    {
        /// <summary>
        /// Adds a plain text label to the specified UI component.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="xPos">Relative x position).</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Label text.</param>
        /// <param name="width">Label width (-1 (default) for autosize).</param>
        /// <param name="textScale">Text scale (default 1.0).</param>
        /// <returns>New text label.</returns>
        public static UILabel AddLabel(UIComponent parent, float xPos, float yPos, string text, float width = -1f, float textScale = 1.0f)
        {
            // Add label.
            UILabel label = parent.AddUIComponent<UILabel>();

            // Set sizing options.
            if (width > 0f)
            {
                // Fixed width.
                label.autoSize = false;
                label.width = width;
                label.autoHeight = true;
                label.wordWrap = true;
            }
            else
            {
                // Autosize.
                label.autoSize = true;
                label.autoHeight = false;
                label.wordWrap = false;
            }

            // Text.
            label.textScale = textScale;
            label.text = text;

            // Position.
            label.relativePosition = new Vector2(xPos, yPos);

            return label;
        }
    }
}