// <copyright file="StandalonePanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Standalone in-game panel template.
    /// </summary>
    public abstract class StandalonePanel : StandalonePanelBase
    {
        // Layout constants.
        private const float CloseButtonSize = 35f;

        // Panel components.
        private UILabel _titleLabel;
        private UISprite _iconSprite;

        // Title text.
        private string _titleText = null;

        /// <summary>
        /// Gets the panel's title.
        /// </summary>
        protected abstract string PanelTitle { get; }

        /// <summary>
        /// Sets the panel's title text.
        /// </summary>
        protected string TitleText
        {
            set
            {
                _titleText = value;

                // Set label title if it's created.
                if (_titleLabel != null)
                {
                    _titleLabel.text = value;
                }
            }
        }

        /// <summary>
        /// Gets the title label X-position.
        /// </summary>
        protected virtual float TitleXPos => CloseButtonSize;

        /// <summary>
        /// Gets the panel opacity.
        /// </summary>
        protected override float PanelOpacity => 1f;

        /// <summary>
        /// Called by Unity when the object is created.
        /// Used to perform setup.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Basic behaviour.
            autoLayout = false;
            canFocus = true;
            isInteractive = true;

            // Appearance.
            atlas = UITextures.InGameAtlas;
            backgroundSprite = "UnlockingPanel2";
            opacity = PanelOpacity;

            // Size.
            size = new Vector2(PanelWidth, PanelHeight);

            // Drag bar.
            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.width = width;
            dragHandle.height = height;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;
            dragHandle.SendToBack();

            // Title label.
            _titleLabel = UILabels.AddLabel(this, TitleXPos, 13f, _titleText ?? PanelTitle, PanelWidth - TitleXPos - CloseButtonSize, alignment: UIHorizontalAlignment.Center);
            _titleLabel.SendToBack();

            // Close button.
            UIButton closeButton = AddUIComponent<UIButton>();
            closeButton.relativePosition = new Vector2(width - CloseButtonSize, 2);
            closeButton.atlas = UITextures.InGameAtlas;
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.eventClick += (c, p) => Close();
        }

        /// <summary>
        /// Sets the icon sprite for the decorative icon (top-left).
        /// </summary>
        /// <param name="spriteAtlas">Sprite icon atlas.</param>
        /// <param name="spriteName">Sprite icon name.</param>
        /// <param name="spriteSize">Spirite size (square, default 32).</param>
        protected void SetIcon(UITextureAtlas spriteAtlas, string spriteName, float spriteSize = 32f)
        {
            // Create sprite if it doesn't already exist.
            if (_iconSprite == null)
            {
                // Decorative icon (top-left).
                _iconSprite = AddUIComponent<UISprite>();
            }

            // Update sprite with new values.
            _iconSprite.relativePosition = new Vector2(Margin, Margin);
            _iconSprite.height = spriteSize;
            _iconSprite.width = spriteSize;
            _iconSprite.atlas = spriteAtlas;
            _iconSprite.spriteName = spriteName;
        }
    }
}