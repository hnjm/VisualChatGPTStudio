﻿using EnvDTE;
using EnvDTE80;
using JeffPires.VisualChatGPTStudio.Options;
using LibGit2Sharp;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using VisualChatGPTStudioShared.ToolWindows.CodeReview;
using UserControl = System.Windows.Controls.UserControl;
    /// Represents a control for code review in a terminal window.
    /// </summary>
    public partial class TerminalWindowCodeReviewControl : UserControl
        #region Properties
        private OptionPageGridGeneral options;

        #endregion Properties
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TerminalWindowCodeReviewControl class.
        /// This constructor ensures the MdXaml library is loaded successfully by creating an instance of MarkdownScrollViewer.
        /// </summary>
            //It is necessary for the MdXaml library load successfully
            MdXaml.MarkdownScrollViewer _ = new();

        #endregion Constructors
        #region Event Handlers
        /// <summary>
        /// Handles the click event of the code review button. It initiates the process of fetching current Git changes.
        /// </summary>
                //Do nothing
            }
        /// Handles the click event of the cancel button. It disables the buttons and signals a cancellation request to the CancellationTokenSource.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        /// Handles the Click event of the btnExpandAll button. It expands all items in the CodeReviews collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExpandAll_Click(object sender, RoutedEventArgs e)
        /// Handles the Click event of the btnCollapseAll button. It collapses all items in the CodeReviews collection by setting their IsExpanded property to false.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The RoutedEventArgs instance containing the event data.</param>
        private void btnCollapseAll_Click(object sender, RoutedEventArgs e)
        /// Handles the click event on the diff view button, retrieves the associated file name from the button's tag,
        /// finds the corresponding code review item, and displays the difference between the original and altered code.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private async void btnDiffView_Click(object sender, RoutedEventArgs e)
        /// Handles the RequestNavigate event of a Hyperlink control, opening the file specified by the URI and marking the event as handled.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RequestNavigateEventArgs"/> instance containing the event data.</param>
        private async void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                DTE2 dte2 = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE");
                dte2.MainWindow.Activate();
                EnvDTE.Window w = dte2.ItemOperations.OpenFile(e.Uri.OriginalString, EnvDTE.Constants.vsViewKindCode);

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        /// Handles the mouse wheel event on a text box to scroll up or down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A MouseWheelEventArgs that contains the event data.</param>
        private void txtCodeReview_PreviewMouseWheel(object sender, MouseWheelEventArgs e)

        #endregion Event Handlers
        #region Methods
        /// <summary>
        /// Initializes control with specified options and package.
        /// </summary>
        /// <param name="options">The general options for the control.</param>
        /// Enables or disables specific buttons and toggles visibility of certain UI elements based on the reviewing state.
        /// </summary>
        /// <param name="reviewing">Indicates whether the reviewing mode is active.</param>
        private void EnableDisableButtons(bool reviewing)
        /// Performs a code review on a given patch entry change asynchronously, utilizing an AI-based chat service for generating the code review comments and separating the original and altered code segments.
        /// </summary>
        /// <param name="change">The patch entry changes containing the code to be reviewed.</param>
        /// <returns>A task that represents the asynchronous operation, resulting in a CodeReviewItem containing the review details.</returns>
        private async Task<CodeReviewItem> DoCodeReview(PatchEntryChanges change)

        /// <summary>
        /// Replaces variant C# code block tags with a standardized tag for consistency.
        /// </summary>
        /// <param name="code">The code review content.</param>
        /// <returns>The modified code review content with standardized code block tags.</returns>
        private static string ReplaceCSharpCodeTag(string code)
        {
            return Regex.Replace(code, "```(c#|csharp)", "```c", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Retrieves the full path of a file from a given partial path by searching through the project items in the Visual Studio solution.
        /// </summary>
        /// <param name="partialPath">The partial path or file name to search for.</param>
        /// <returns>The full path of the file if found; otherwise, returns the original partial path.</returns>
        private string GetFullPathFromPartial(string partialPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE");

            string fullPath = string.Empty;

            foreach (Project project in dte.Solution.Projects)
            {
                fullPath = FindFileInProjectItems(project.ProjectItems, partialPath);

                if (!string.IsNullOrWhiteSpace(fullPath))
                {
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return partialPath;
            }

            return fullPath;
        }

        /// <summary>
        /// Searches for a file within a collection of project items that matches a specified partial path.
        /// </summary>
        /// <param name="items">The collection of project items to search through.</param>
        /// <param name="partialPath">The partial path of the file to find.</param>
        /// <returns>The full path of the found file or an empty string if the file is not found.</returns>
        private string FindFileInProjectItems(ProjectItems items, string partialPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (ProjectItem item in items)
            {
                if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    string result = FindFileInProjectItems(item.ProjectItems, partialPath);

                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
                else if (item.FileCount > 0)
                {
                    for (short i = 1; i <= item.FileCount; i++)
                    {
                        string filePath = item.FileNames[i];

                        if (filePath.Replace('\\', '/').EndsWith(partialPath, StringComparison.OrdinalIgnoreCase))
                        {
                            return filePath;
                        }
                    }
                }
            }

            return string.Empty;
        }

        #endregion Methods                         