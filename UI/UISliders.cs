// <copyright file="UISliders.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// UI sliders.
    /// </summary>
    public static class UISliders
    {
        /// <summary>
        /// Adds an options panel-style slider with a descriptive text label above.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="xPos">Relative x position.</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Descriptive label text.</param>
        /// <param name="min">Slider minimum value.</param>
        /// <param name="max">Slider maximum value.</param>
        /// <param name="step">Slider minimum step.</param>
        /// <param name="defaultValue">Slider initial value.</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600).</param>
        /// <returns>New UI slider with attached labels.</returns>
        public static UISlider AddPlainSlider(UIComponent parent, float xPos, float yPos, string text, float min, float max, float step, float defaultValue, float width = 600f)
        {
            // Add slider component.
            UIPanel sliderPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsSliderTemplate")) as UIPanel;

            // Panel layout.
            sliderPanel.autoLayout = false;
            sliderPanel.autoSize = false;
            sliderPanel.width = width + 50f;
            sliderPanel.height = 65f;

            // Label.
            UILabel sliderLabel = sliderPanel.Find<UILabel>("Label");
            sliderLabel.autoHeight = true;
            sliderLabel.width = width;
            sliderLabel.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
            sliderLabel.relativePosition = Vector3.zero;
            sliderLabel.text = text;

            // Slider configuration.
            UISlider newSlider = sliderPanel.Find<UISlider>("Slider");
            newSlider.minValue = min;
            newSlider.maxValue = max;
            newSlider.stepSize = step;
            newSlider.value = defaultValue;

            // Move default slider position to match resized label.
            newSlider.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
            newSlider.relativePosition = UILayout.PositionUnder(sliderLabel);

            newSlider.width = width;

            // Set position.
            newSlider.parent.relativePosition = new Vector2(xPos, yPos);

            return newSlider;
        }

        /// <summary>
        /// Adds an options panel-style slider with a descriptive text label above and an automatically updating value label immediately to the right.
        /// </summary>
        /// <param name="parent">Panel to add the control to.</param>
        /// <param name="xPos">Relative x position.</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Descriptive label text.</param>
        /// <param name="min">Slider minimum value.</param>
        /// <param name="max">Slider maximum value.</param>
        /// <param name="step">Slider minimum step.</param>
        /// <param name="defaultValue">Slider initial value.</param>
        /// <param name="format">Slider value format.</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600).</param>
        /// <returns>New UI slider with attached labels.</returns>
        public static UISlider AddPlainSliderWithValue(UIComponent parent, float xPos, float yPos, string text, float min, float max, float step, float defaultValue, SliderValueFormat format, float width = 600f)
        {
            // Add slider component.
            UISlider newSlider = AddPlainSlider(parent, xPos, yPos, text, min, max, step, defaultValue, width);
            UIPanel sliderPanel = (UIPanel)newSlider.parent;

            // Value label.
            UILabel valueLabel = sliderPanel.AddUIComponent<UILabel>();
            valueLabel.name = "ValueLabel";
            valueLabel.relativePosition = UILayout.PositionRightOf(newSlider, 8f, 1f);

            // Set initial value and event handler to update on value change.
            valueLabel.text = format.FormatValue(newSlider.value);
            newSlider.eventValueChanged += (c, value) => valueLabel.text = format.FormatValue(value);

            return newSlider;
        }

        /// <summary>
        /// Adds an options panel-style slider with a descriptive text label above and an automatically updating value label immediately to the right.
        /// </summary>
        /// <param name="parent">Panel to add the control to.</param>
        /// <param name="xPos">Relative x position.</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Descriptive label text.</param>
        /// <param name="min">Slider minimum value.</param>
        /// <param name="max">Slider maximum value.</param>
        /// <param name="step">Slider minimum step.</param>
        /// <param name="defaultValue">Slider initial value.</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600).</param>
        /// <returns>New UI slider with attached labels.</returns>
        public static UISlider AddPlainSliderWithValue(UIComponent parent, float xPos, float yPos, string text, float min, float max, float step, float defaultValue, float width = 600f) =>
            AddPlainSliderWithValue(parent, xPos, yPos, text, min, max, step, defaultValue, new SliderValueFormat(valueMultiplier: 1, roundToNearest: step, numberFormat: "N", suffix: null), width);

        /// <summary>
        /// Adds an options panel-style slider with a descriptive text label above and an automatically updating integer value label immediately to the right.
        /// </summary>
        /// <param name="parent">Panel to add the control to.</param>
        /// <param name="xPos">Relative x position.</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Descriptive label text.</param>
        /// <param name="min">Slider minimum value.</param>
        /// <param name="max">Slider maximum value.</param>
        /// <param name="step">Slider minimum step.</param>
        /// <param name="defaultValue">Slider initial value.</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600).</param>
        /// <returns>New UI slider with attached labels.</returns>
        public static UISlider AddPlainSliderWithIntegerValue(UIComponent parent, float xPos, float yPos, string text, float min, float max, float step, float defaultValue, float width = 600f) =>
            AddPlainSliderWithValue(parent, xPos, yPos, text, min, max, step, defaultValue, new SliderValueFormat(valueMultiplier: 1, roundToNearest: 1f, numberFormat: "N0", suffix: null), width);

        /// <summary>
        /// Adds an options panel-style slider with a descriptive text label above and an automatically updating percentage value label immediately to the right (based on range 0.0f - 1.0f).
        /// </summary>
        /// <param name="parent">Panel to add the control to.</param>
        /// <param name="xPos">Relative x position.</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Descriptive label text.</param>
        /// <param name="min">Slider minimum value.</param>
        /// <param name="max">Slider maximum value.</param>
        /// <param name="step">Slider minimum step.</param>
        /// <param name="defaultValue">Slider initial value.</param>
        /// <param name="width">Slider width (excluding value label to right) (default 600).</param>
        /// <returns>New UI slider with attached labels.</returns>
        public static UISlider AddPlainSliderWithPercentage(UIComponent parent, float xPos, float yPos, string text, float min, float max, float step, float defaultValue, float width = 600f) =>
            AddPlainSliderWithValue(parent, xPos, yPos, text, min, max, step, defaultValue, new SliderValueFormat(valueMultiplier: 100f, roundToNearest: 1f, numberFormat: "N0", suffix: "%"), width);

        /// <summary>
        /// Adds a budget-style slider to the specified component.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="xPos">Relative X position.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="width">Slider width.</param>
        /// <param name="maxValue">Slider maximum value.</param>>
        /// <param name="tooltip">Tooltip text (null for none).</param>
        /// <returns>New UISlider.</returns>
        public static UISlider AddBudgetSlider(UIComponent parent, float xPos, float yPos, float width, float maxValue, string tooltip = null)
        {
            // Layout constants.
            const float SliderHeight = 18f;

            // Slider control.
            UISlider newSlider = parent.AddUIComponent<UISlider>();
            newSlider.size = new Vector2(width, SliderHeight);
            newSlider.relativePosition = new Vector2(xPos, yPos);

            // Tooltip.
            if (tooltip != null)
            {
                newSlider.tooltip = tooltip;
            }

            // Slider track.
            UISlicedSprite sliderSprite = newSlider.AddUIComponent<UISlicedSprite>();
            sliderSprite.atlas = UITextures.InGameAtlas;
            sliderSprite.spriteName = "BudgetSlider";
            sliderSprite.size = new Vector2(newSlider.width, 9f);
            sliderSprite.relativePosition = new Vector2(0f, 4f);

            // Slider thumb.
            UISlicedSprite sliderThumb = newSlider.AddUIComponent<UISlicedSprite>();
            sliderThumb.atlas = UITextures.InGameAtlas;
            sliderThumb.spriteName = "SliderBudget";
            newSlider.thumbObject = sliderThumb;

            // Set initial values.
            newSlider.stepSize = 1f;
            newSlider.minValue = 1f;
            newSlider.maxValue = maxValue;

            return newSlider;
        }

        /// <summary>
        /// Provides formatting for slider value displays.
        /// </summary>
        public readonly struct SliderValueFormat
        {
            // Formatting options.
            private readonly float _valueMultiplier;
            private readonly float _roundToNearest;
            private readonly string _numberFormat;
            private readonly string _valueSuffix;

            /// <summary>
            /// Initializes a new instance of the <see cref="SliderValueFormat"/> struct.
            /// </summary>
            /// <param name="valueMultiplier">Muliply the slider value by this amount before displaying (default 1f).</param>
            /// <param name="roundToNearest">Round the displayed value to the nearest increment of this amount (default 1f).</param>
            /// <param name="numberFormat">Use this number format string to format the displayed number (default 'N').</param>
            /// <param name="suffix">An optional suffix to display immediately after the displayed number (default <c>null</c> for none).</param>
            public SliderValueFormat(float valueMultiplier = 1f, float roundToNearest = 1f, string numberFormat = "N", string suffix = null)
            {
                // Populate required fields.
                _valueSuffix = suffix;
                _numberFormat = numberFormat;
                _roundToNearest = roundToNearest;
                _valueMultiplier = valueMultiplier;
            }

            /// <summary>
            /// Formats the given slider value according to current settings.
            /// </summary>
            /// <param name="value">Slider value to format.</param>
            /// <returns>Formatted value string for display.</returns>
            public string FormatValue(float value)
            {
                string valueText = (value * _valueMultiplier).RoundToNearest(_roundToNearest).ToString(_numberFormat);
                if (!string.IsNullOrEmpty(_valueSuffix))
                {
                    valueText += _valueSuffix;
                }

                return valueText;
            }
        }
    }
}