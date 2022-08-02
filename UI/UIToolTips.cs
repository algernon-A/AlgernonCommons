// <copyright file="UIToolTips.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Custom tooltips.
    /// </summary>
    public static class UIToolTips
    {
        // Cache.
        private static UILabel _wordWrapTooltip;

        /// <summary>
        /// Gets a word-wrapping text-label tooltip box.
        /// </summary>
        public static UILabel WordWrapToolTip
        {
            get
            {
                if (_wordWrapTooltip == null)
                {
                    _wordWrapTooltip = WordWrapToolTipBox();
                }

                return _wordWrapTooltip;
            }
        }

        /// <summary>
        /// Creates a word-wrapping tooltip box.
        /// </summary>
        /// <returns>New tooltip box.</returns>
        private static UILabel WordWrapToolTipBox()
        {
            // Create GameObject and attach new UILabel.
            GameObject tooltipGameObject = new GameObject("AlgernonWordWrapTooltip");
            tooltipGameObject.transform.parent = UIView.Find("DefaultTooltip").gameObject.transform.parent;
            UILabel tipBox = tooltipGameObject.AddComponent<UILabel>();

            // Size.
            tipBox.autoSize = true;
            tipBox.minimumSize = new Vector2(500f, 12f);
            tipBox.wordWrap = true;

            // Mimic game's default tooltop.
            tipBox.padding = new RectOffset(23, 23, 5, 5);
            tipBox.verticalAlignment = UIVerticalAlignment.Middle;
            tipBox.pivot = UIPivotPoint.BottomLeft;
            tipBox.arbitraryPivotOffset = new Vector2(-3, 6);

            // Appearance.
            tipBox.backgroundSprite = "InfoDisplay";

            // Start hidden and off to the side.
            tipBox.transformPosition = new Vector2(-2f, -2f);
            tipBox.isVisible = false;

            return tipBox;
        }
    }
}