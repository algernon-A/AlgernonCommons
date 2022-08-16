// <copyright file="UIListRow.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// An individual UIList row.
    /// </summary>
    public abstract class UIListRow : UIPanel
    {
        /// <summary>
        /// Default display margin.
        /// </summary>
        public const float Margin = 5f;

        // UI components.
        private readonly UISprite _background;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIListRow"/> class.
        /// </summary>
        public UIListRow()
        {
            // Ensure basic behaviour.
            isVisible = true;
            canFocus = true;
            isInteractive = true;

            // Add background panel.
            _background = AddUIComponent<UISprite>();
            _background.relativePosition = Vector2.zero;
            _background.autoSize = false;
            _background.zOrder = 0;
        }

        /// <summary>
        /// Gets the height for this row.
        /// </summary>
        public virtual float RowHeight => UIList.DefaultRowHeight;

        /// <summary>
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public abstract void Display(object data, int rowIndex);

        /// <summary>
        /// Sets the row display to the selected state (highlighted).
        /// </summary>
        public void Select()
        {
            _background.spriteName = "ListItemHighlight";
            _background.color = new Color32(255, 255, 255, 255);
        }

        /// <summary>
        /// Sets the row display to the deselected state.
        /// </summary>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public void Deselect(int rowIndex)
        {
            if (rowIndex % 2 == 0)
            {
                // Darker background for even rows.
                _background.spriteName = null;
            }
            else
            {
                // Lighter background for odd rows.
                _background.spriteName = "UnlockingItemBackground";
                _background.color = new Color32(0, 0, 0, 128);
            }
        }

        /// <summary>
        /// Called when dimensions are changed, including as part of initial setup (required to set correct relative position of label).
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            // Resize background panel to match current size.
            _background.size = this.size;
        }

        /// <summary>
        /// Adds a text label to the current list row.
        /// </summary>
        /// <param name="xPos">Label relative x-position.</param>
        /// <param name="width">Label width.</param>
        /// <param name="textScale">Text scale.</param>
        /// <param name="wordWrap">Wordwrap status (true to enable, false to disable).</param>
        /// <returns>New UILabel.</returns>
        protected UILabel AddLabel(float xPos, float width, float textScale = 0.8f, bool wordWrap = false)
        {
            UILabel newLabel = AddUIComponent<UILabel>();
            newLabel.autoSize = false;
            newLabel.height = RowHeight;
            newLabel.width = width;
            newLabel.verticalAlignment = UIVerticalAlignment.Middle;
            newLabel.clipChildren = true;
            newLabel.wordWrap = wordWrap;
            newLabel.padding.top = 1;
            newLabel.textScale = textScale;
            newLabel.font = UIFonts.Regular;
            newLabel.relativePosition = new Vector2(xPos, 0f);
            return newLabel;
        }
    }
}