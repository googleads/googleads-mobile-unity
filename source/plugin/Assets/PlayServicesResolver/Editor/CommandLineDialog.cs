// <copyright file="CommandLineDialog.cs" company="Google Inc.">
// Copyright (C) 2016 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

namespace GooglePlayServices
{
    using System.Collections.Generic;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System;
    using UnityEditor;
    using UnityEngine;

    public class CommandLineDialog : TextAreaDialog
    {
        /// <summary>
        /// Forwards the output of the currently executing command to a CommandLineDialog window.
        /// </summary>
        public class ProgressReporter : CommandLine.LineReader
        {
            /// <summary>
            /// Used to scale the progress bar by the number of lines reported by the command
            /// line tool.
            /// </summary>
            public int maxProgressLines;

            // Queue of command line output lines to send to the main / UI thread.
            private System.Collections.Queue textQueue = null;
            // Number of lines reported by the command line tool.
            private volatile int linesReported;
            // Command line tool result, set when command line execution is complete.
            private volatile CommandLine.Result result = null;

            /// <summary>
            /// Event called on the main / UI thread when the outstanding command line tool
            /// completes.
            /// </summary>
            public event CommandLine.CompletionHandler Complete;

            /// <summary>
            /// Construct a new reporter.
            /// </summary>
            public ProgressReporter(CommandLine.IOHandler handler = null)
            {
                textQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue());
                maxProgressLines = 0;
                linesReported = 0;
                LineHandler += CommandLineIOHandler;
                Complete = null;
            }

            // Count the number of newlines and carriage returns in a string.
            private int CountLines(string str)
            {
                return str.Split(new char[] { '\n', '\r' }).Length - 1;
            }

            /// <summary>
            /// Called from RunCommandLine() tool to report the output of the currently
            /// executing commmand.
            /// </summary>
            /// <param name="process">Executing process.</param>
            /// <param name="stdin">Standard input stream.</param>
            /// <param name="data">Data read from the standard output or error streams.</param>
            private void CommandLineIOHandler(Process process, StreamWriter stdin,
                                              CommandLine.StreamData data)
            {
                if (process.HasExited || data.data == null) return;
                // Count lines in stdout.
                if (data.handle == 0) linesReported += CountLines(data.text);
                // Enqueue data for the text view.
                textQueue.Enqueue(System.Text.Encoding.UTF8.GetString(data.data));
            }

            /// <summary>
            /// Called when the currently executing command completes.
            /// </summary>
            public void CommandLineToolCompletion(CommandLine.Result result)
            {
                this.result = result;
            }

            /// <summary>
            /// Called from CommandLineDialog in the context of the main / UI thread.
            /// </summary>
            public void Update(CommandLineDialog window)
            {
                if (textQueue.Count > 0)
                {
                    List<string> textList = new List<string>();
                    while (textQueue.Count > 0) textList.Add((string)textQueue.Dequeue());
                    string bodyText = window.bodyText + String.Join("", textList.ToArray());
                    // Really weak handling of carriage returns.  Truncates to the previous
                    // line for each newline detected.
                    while (true)
                    {
                        // Really weak handling carriage returns for progress style updates.
                        int carriageReturn = bodyText.LastIndexOf("\r");
                        if (carriageReturn < 0 || bodyText.Substring(carriageReturn, 1) == "\n")
                        {
                            break;
                        }
                        string bodyTextHead = "";
                        int previousNewline = bodyText.LastIndexOf("\n", carriageReturn,
                                                                   carriageReturn);
                        if (previousNewline >= 0)
                        {
                            bodyTextHead = bodyText.Substring(0, previousNewline + 1);
                        }
                        bodyText = bodyTextHead + bodyText.Substring(carriageReturn + 1);
                    }
                    window.bodyText = bodyText;
                    if (window.autoScrollToBottom)
                    {
                        window.scrollPosition.y = Mathf.Infinity;
                    }
                    window.Repaint();
                }
                if (maxProgressLines > 0)
                {
                    window.progress = (float)linesReported / (float)maxProgressLines;
                }
                if (result != null)
                {
                    window.progressTitle = "";
                    if (Complete != null)
                    {
                        Complete(result);
                        Complete = null;
                    }
                }
            }
        }

        public volatile float progress;
        public string progressTitle;
        public string progressSummary;
        public volatile bool autoScrollToBottom;

        /// <summary>
        /// Event delegate called from the Update() method of the window.
        /// </summary>
        public delegate void UpdateDelegate(CommandLineDialog window);

        public event UpdateDelegate UpdateEvent;

        private bool progressBarVisible;

        /// <summary>
        /// Create a dialog box which can display command line output.
        /// </summary>
        /// <returns>Reference to the new window.</returns>
        public static CommandLineDialog CreateCommandLineDialog(string title)
        {
            CommandLineDialog window = (CommandLineDialog)EditorWindow.GetWindow(
                typeof(CommandLineDialog), true, title);
            window.Initialize();
            return window;
        }

        /// <summary>
        /// Initialize all members of the window.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            progress = 0.0f;
            progressTitle = "";
            progressSummary = "";
            UpdateEvent = null;
            progressBarVisible = false;
            autoScrollToBottom = false;
        }

        /// <summary>
        /// Asynchronously execute a command line tool in this window, showing progress
        /// and finally calling the specified delegate on completion from the main / UI thread.
        /// </summary>
        /// <param name="toolPath">Tool to execute.</param>
        /// <param name="arguments">String to pass to the tools' command line.</param>
        /// <param name="completionDelegate">Called when the tool completes.</param>
        /// <param name="workingDirectory">Directory to execute the tool from.</param>
        /// <param name="ioHandler">Allows a caller to provide interactive input and also handle
        /// both output and error streams from a single delegate.</param>
        /// <param name="maxProgressLines">Specifies the number of lines output by the
        /// command line that results in a 100% value on a progress bar.</param>
        /// <returns>Reference to the new window.</returns>
        public void RunAsync(
            string toolPath, string arguments,
            CommandLine.CompletionHandler completionDelegate,
            string workingDirectory = null, Dictionary<string, string> envVars = null,
            CommandLine.IOHandler ioHandler = null, int maxProgressLines = 0)
        {
            CommandLineDialog.ProgressReporter reporter = new CommandLineDialog.ProgressReporter();
            reporter.maxProgressLines = maxProgressLines;
            // Call the reporter from the UI thread from this window.
            UpdateEvent += reporter.Update;
            // Connect the user's delegate to the reporter's completion method.
            reporter.Complete += completionDelegate;
            // Connect the caller's IoHandler delegate to the reporter.
            reporter.DataHandler += ioHandler;
            // Disconnect the reporter when the command completes.
            CommandLine.CompletionHandler reporterUpdateDisable =
                (CommandLine.Result unusedResult) => { this.UpdateEvent -= reporter.Update; };
            reporter.Complete += reporterUpdateDisable;
            CommandLine.RunAsync(toolPath, arguments, reporter.CommandLineToolCompletion,
                                 workingDirectory: workingDirectory, envVars: envVars,
                                 ioHandler: reporter.AggregateLine);
        }

        /// <summary>
        /// Call the update event from the UI thread, optionally display / hide the progress bar.
        /// </summary>
        protected virtual void Update()
        {
            if (UpdateEvent != null) UpdateEvent(this);
            if (progressTitle != "")
            {
                progressBarVisible = true;
                EditorUtility.DisplayProgressBar(progressTitle, progressSummary,
                                                 progress);
            }
            else if (progressBarVisible)
            {
                progressBarVisible = false;
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
