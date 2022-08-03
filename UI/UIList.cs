﻿// <copyright file="UIList.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using System;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Based on SamSamT's original work as implemented in boformer's Building Themes mod and AJD3's Ploppable RICO.
    /// </summary>
    public class UIList : UIComponent
    {
        // Layout constants.
        private const float ScrollBarWidth = 10f;

        // UI components.
        private UIPanel _contentPanel;
        private UIScrollbar _scrollbar;
        private FastList<UIListRow> _rows;

        // List type.
        private Type _rowType;

        // Layout variables.
        private float _rowHeight = 20f;

        // Current selection.
        private int _selectedIndex = -1;

        // Row position.
        private int _currentPosition = 0;

        // List data.
        private FastList<object> _data;

        // Event handling.
        private bool _ignoreScrolling = false;

        /// <summary>
        /// Selection changed event (returns currently selected object, or null if none).
        /// </summary>
        public event PropertyChangedEventHandler<object> EventSelectionChanged;

        /// <summary>
        /// Gets or sets the height of each row (minimum 1).
        /// </summary>
        public float RowHeight
        {
            get => _rowHeight;

            set
            {
                // Minimum value check.
                if (value < 1)
                {
                    Logging.Error("invalid height ", value, " passed to UIList.RowHeight");
                    return;
                }

                // Don't do anything if no change.
                if (_rowHeight != value)
                {
                    // Recalculate rows if height changes.
                    _rowHeight = value;
                    EnsureRows();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current position of the list, i.e. the index number of the top displayed row.
        /// </summary>
        public int CurrentPosition
        {
            get => _currentPosition;

            set
            {
                // Can't move to a position past the last full screen of rows.
                int newPosition = value;
                int maxPosition = _data.m_size - _rows.m_size;
                if (newPosition > maxPosition)
                {
                    newPosition = maxPosition;
                }

                // Can't move below zero, either (this also catches underflows from maxPosition adjustment above).
                if (newPosition < 0)
                {
                    newPosition = 0;
                }

                // Don't do anything else if no change.
                if (newPosition != _currentPosition)
                {
                    // Set current position.
                    _currentPosition = newPosition;

                    // Update display with new position.
                    Display(newPosition);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected item (-1 to clear selection).
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;

            set
            {
                // Don't do anything if no change, or if there's no data.
                if (value == _selectedIndex | _data == null | _rows == null)
                {
                    return;
                }

                // Ignore illegal values.
                if (value >= _data.m_size)
                {
                    Logging.Error("invalid position ", value, " passed to UIList.SelectedIndex");
                    return;
                }

                // Deselect current selection.
                int selectedRow = _selectedIndex - _currentPosition;
                if (selectedRow >= 0 & selectedRow < _rows.m_size)
                {
                    _rows[selectedRow].Deselect(selectedRow);
                }

                // Update value.
                _selectedIndex = value;

                // Is there an active selection, i.e. index is 0 or greater?
                if (value >= 0)
                {
                    // Yes - if the selected index is outside the current visibility range, move the list position to show it.
                    if (value < _currentPosition | value > _currentPosition + _rows.m_size)
                    {
                        CurrentPosition = value;
                    }

                    // Select new item.
                    selectedRow = value - _currentPosition;
                    _rows[selectedRow].Select();
                }

                // Invoke selection changed event.
                EventSelectionChanged?.Invoke(this, SelectedItem);
            }
        }

        /// <summary>
        /// Gets the currently selected object (null if none).
        /// </summary>
        public object SelectedItem
        {
            get
            {
                // Return null if no valid selection.
                if (_selectedIndex < 0 | _selectedIndex > _data.m_size)
                {
                    return null;
                }

                return _data[_selectedIndex];
            }
        }

        /// <summary>
        /// Gets the number of currently displayed rows.
        /// </summary>
        public int RowCount => _rows?.m_size ?? 0;

        /// <summary>
        /// Gets or sets the list of data objects to display.
        /// Retains current list position where possible.
        /// </summary>
        public FastList<object> Data
        {
            get => _data;

            set
            {
                // Don't do anything if the data hasn't changed.
                if (_data != value)
                {
                    _data = value;

                    // Update the scrollbar to reflect the new data size.
                    UpdateScrollbar();

                    // Refresh the list.
                    Display(_currentPosition);
                }
            }
        }

        /// <summary>
        /// Gets or sets the panel background sprite.
        /// </summary>
        public string BackgroundSprite
        {
            get => _contentPanel?.backgroundSprite;

            set => _contentPanel.backgroundSprite = value;
        }

        /// <summary>
        /// Adds a UIList to the specified parent and peforms intial setup.
        /// Use this instead of AddUIComponent.
        /// This is necessary because MonoBehaviours cannot be generic nor can they have constructor parameters, so there's no way to pass the type param.
        /// </summary>
        /// <typeparam name="TRow">Row type.</typeparam>
        /// <param name="parent">Parent component.</param>
        /// <returns>New UIList.</returns>
        public static UIList AddUIList<TRow>(UIComponent parent)
            where TRow : UIListRow => AddUIList<UIList, TRow>(parent);

        /// <summary>
        /// Adds a UIList of the specified type to the specified parent and peforms intial setup.
        /// Use this instead of AddUIComponent.
        /// This is necessary because MonoBehaviours cannot be generic nor can they have constructor parameters, so there's no way to pass the type param.
        /// </summary>
        /// <typeparam name="TList">List type.</typeparam>
        /// <typeparam name="TRow">Row type.</typeparam>
        /// <param name="parent">Parent component.</param>
        /// <returns>New UIList of specified type.</returns>
        public static TList AddUIList<TList, TRow>(UIComponent parent)
            where TList : UIList
            where TRow : UIListRow
        {
            TList uiList = parent.AddUIComponent<TList>();
            uiList._rowType = typeof(TRow);
            uiList.Setup();
            return uiList;
        }

        /// <summary>
        /// Display the list data, with the specified list index at the top row.
        /// </summary>
        /// <param name="listPosition">List data item index for the top row.</param>
        public void Display(int listPosition)
        {
            // Don't do anything if no data.
            if (_data == null)
            {
                return;
            }

            // Iterate through each row and set its display.
            for (int i = 0; i < _rows.m_size; ++i)
            {
                // Do we have data for this row?
                int dataIndex = i + listPosition;
                if (dataIndex < _data.m_size)
                {
                    // Data available - enable row and display data.
                    _rows[i].Display(_data[dataIndex], i);
                    _rows[i].enabled = true;
                }
                else
                {
                    // Data not available; disable this row.
                    _rows[i].enabled = false;
                }

                // Set row relative position and width.
                _rows[i].relativePosition = new Vector2(0f, i * _rowHeight);
                _rows[i].width = width;
            }

            // Highlight selected row.
            int selectedRow = _selectedIndex - _currentPosition;
            if (selectedRow > 0 & selectedRow < _rows.m_size)
            {
                _rows[selectedRow].Select();
            }

            // Update scrollbar indication.
            UpdateScrollIndication();
        }

        /// <summary>
        /// Sets the selection to the given object and type.
        /// If no item is found, clears the selection.
        /// </summary>
        /// <typeparam name="TItem">Item type.</typeparam>
        /// <param name="target">Target object to find.</param>
        public void FindItem<TItem>(TItem target)
        {
            // Iterate through the rows list.
            for (int i = 0; i < _data.m_buffer.Length; ++i)
            {
                // Look for a match.
                if (_data.m_buffer[i] is TItem thisItem && thisItem.Equals(target))
                {
                    // Found a match; set the selected index to this one.
                    SelectedIndex = i;

                    // Done here; return.
                    return;
                }
            }

            // If we got here, we didn't find a match; clear the selection.
            SelectedIndex = -1;
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            // Reset selected index.
            SelectedIndex = -1;

            // Clear data.
            Data.Clear();

            // Disable all rows.
            for (int i = 0; i < _rows.m_size; ++i)
            {
                _rows[i].enabled = false;
            }

            // Update the scrollbar to reflect the new empty state.
            UpdateScrollbar();
        }

        /// <summary>
        /// Called by Unity when the object is destroyed.
        /// Performs cleanup and destroys all child components.
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();

            // Null checks.
            if (_contentPanel != null)
            {
                // Destroy rows.
                if (_rows != null)
                {
                    for (int i = 0; i < _rows.m_size; ++i)
                    {
                        Destroy(_rows[i]);
                    }
                }

                // Destroy primary components.
                Destroy(_contentPanel);
                Destroy(_scrollbar);
            }
        }

        /// <summary>
        /// Handles mouse wheel scrolling.
        /// </summary>
        /// <param name="p">Event parameter.</param>
        protected override void OnMouseWheel(UIMouseEventParameter p)
        {
            base.OnMouseWheel(p);

            // Move the list position by 1.
            CurrentPosition -= Mathf.CeilToInt(p.wheelDelta);
        }

        /// <summary>
        /// Called by the game when the component size changes.
        /// Also used for intitial setup.
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            // Don't do anything if content hasn't been set up.
            if (_contentPanel == null)
            {
                return;
            }

            // Resize content panel.
            _contentPanel.width = width - ScrollBarWidth;
            _contentPanel.height = height;

            // Reesize and position scrollbar.
            _scrollbar.height = height;
            _scrollbar.trackObject.height = height;
            _scrollbar.AlignTo(this, UIAlignAnchor.TopRight);

            // Recalculate rows.
            EnsureRows();
        }

        /// <summary>
        /// Row clicked event handler.
        /// </summary>
        /// <param name="component">Calling component.</param>
        /// <param name="mouseEvent">Mouse event parameter.</param>
        private void OnRowClicked(UIComponent component, UIMouseEventParameter mouseEvent)
        {
            // Establish number of selectable components - the smaller of either the visible items or the list content.
            int lastItem = Mathf.Min(_data.m_size, _rows.m_size);

            // Iterate through visible rows, checking to see if it matches the calling item.
            for (int i = 0; i < lastItem; ++i)
            {
                if (component == _rows[i])
                {
                    // Found a match - set the selected index accordingly (correcting for current list position).
                    SelectedIndex = i + _currentPosition;
                    return;
                }
            }
        }

        /// <summary>
        /// Updates the scrollbar to match current panel size and object list.
        /// </summary>
        private void UpdateScrollbar()
        {
            // Safety checks.
            if (_data == null || _rows == null)
            {
                return;
            }

            // Number of rows outside the visible area.
            int extraRows = _data.m_size - _rows.m_size;

            if (extraRows > 0)
            {
                // Show scrollbar.
                _scrollbar.Show();

                // Set scrollbar thumb size - ratio of visible rows to total rows..
                float fullDataHeight = _data.m_size * _rowHeight;
                _scrollbar.scrollSize = height * height / fullDataHeight;
                _scrollbar.minValue = 0f;
                _scrollbar.maxValue = height;
                _scrollbar.incrementAmount = Mathf.Max(1f, (height - _scrollbar.scrollSize) / extraRows);
            }
            else
            {
                _scrollbar.Hide();
            }

            UpdateScrollIndication();
        }

        /// <summary>
        /// Updates the position indicated by the scrollbar.
        /// </summary>
        private void UpdateScrollIndication()
        {
            // Don't do anything if ignoring scroll events.
            if (!_ignoreScrolling)
            {
                // Ignore scrolling events while we update.
                _ignoreScrolling = true;
                _scrollbar.value = _scrollbar.incrementAmount * _currentPosition;
                _ignoreScrolling = false;
            }
        }

        /// <summary>
        /// Scrolls the list in response to scrollbar change.
        /// </summary>
        /// <param name="value">New scrollbar value.</param>
        private void Scroll(float value)
        {
            // Don't do anything if ignoring scroll events.
            if (!_ignoreScrolling)
            {
                // Ignore scrolling events while we update.
                _ignoreScrolling = true;
                CurrentPosition = Mathf.RoundToInt(value / _scrollbar.incrementAmount);
                _ignoreScrolling = false;
            }
        }

        /// <summary>
        /// Checks to confirm that we've got the correct number of rows displayed.
        /// </summary>
        private void EnsureRows()
        {
            // Calculate required number of rows.
            int requiredRows = Mathf.CeilToInt(height / _rowHeight);

            // Initialize rows fastlist if required.
            if (_rows == null)
            {
                _rows = new FastList<UIListRow>();
                _rows.SetCapacity(requiredRows);
            }
            else if (_rows.m_size < requiredRows)
            {
                // Existing row list, but we need to add more.
                for (int i = _rows.m_size; i < requiredRows; ++i)
                {
                    // Add new row and event handler.
                    UIListRow newRow = _contentPanel.AddUIComponent(_rowType) as UIListRow;
                    newRow.eventClick += OnRowClicked;
                    _rows.Add(newRow);
                }
            }
            else if (_rows.m_size > requiredRows)
            {
                // Existing row list, but we need to remove some.
                for (int i = requiredRows; i < _rows.m_size; ++i)
                {
                    Destroy(_rows[i]);
                }

                // Reduce list capacity to the new limit.
                _rows.SetCapacity(requiredRows);
            }

            // Update the scrollbar.
            UpdateScrollbar();
        }

        /// <summary>
        /// Sets up the UI components.
        /// </summary>
        private void Setup()
        {
            // Don't do anything if we're already setup.
            if (_contentPanel != null)
            {
                return;
            }

            // Add content panel - same size as this, but with space to the right for the scrollbar.
            _contentPanel = AddUIComponent<UIPanel>();
            _contentPanel.autoSize = false;
            _contentPanel.autoLayout = false;
            _contentPanel.relativePosition = Vector2.zero;
            _contentPanel.clipChildren = true;

            // Scrollbar.
            _scrollbar = UIScrollbars.AddScrollbar(this);
            _scrollbar.autoHide = false;

            // Set up size and positioning.
            OnSizeChanged();

            // Update scrolled position.
            _scrollbar.eventValueChanged += (control, value) => Scroll(value);
        }
    }
}