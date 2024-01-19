﻿using System.Windows;
    /// <summary>
    /// Represents an image control that can be used as a command button.
    /// </summary>
    public class CommandImage : Image

        /// <summary>
        /// Gets or sets the command associated with this control.
        /// </summary>
        public ICommand Command

        /// <summary>
        /// Overrides the OnMouseLeftButtonDown method to execute the Command if it is not null and can be executed.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs parameter.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)