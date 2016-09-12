// <copyright file="TextAreaDialog.cs" company="Google Inc.">
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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Window which displays a scrollable text area and two buttons at the bottom.
    /// </summary>
    public class TextAreaDialog : EditorWindow
    {
        /// <summary>
        /// Delegate type, called when a button is clicked.
        /// </summary>
        public delegate void ButtonClicked(TextAreaDialog dialog);

        /// <summary>
        /// Delegate called when a button is clicked.
        /// </summary>
        public ButtonClicked buttonClicked;

        /// <summary>
        /// Whether this window should be modal.
        /// NOTE: This emulates modal behavior by re-acquiring focus when it's lost.
        /// </summary>
        public bool modal = true;

        /// <summary>
        /// Set the text to display in the summary area of the window.
        /// </summary>
        public string summaryText = "";

        /// <summary>
        /// Set the text to display on the "yes" (left-most) button.
        /// </summary>
        public string yesText = "";

        /// <summary>
        /// Set the text to display on the "no" (left-most) button.
        /// </summary>
        public string noText = "";

        /// <summary>
        /// Set the text to display in the scrollable text area.
        /// </summary>
        public string bodyText = "";

        /// <summary>
        /// Result of yes / no button press.  true if the "yes" button was pressed, false if the
        /// "no" button was pressed.  Defaults to "false".
        /// </summary>
        public bool result = false;

        /// <summary>
        /// Current position of the scrollbar.
        /// </summary>
        public Vector2 scrollPosition;

        /// <summary>
        /// Get the existing text area window or create a new one.
        /// </summary>
        /// <param name="title">Title to display on the window.</param>
        /// <returns>Reference to this class</returns>
        public static TextAreaDialog CreateTextAreaDialog(string title)
        {
            TextAreaDialog window = (TextAreaDialog)EditorWindow.GetWindow(typeof(TextAreaDialog),
                                                                           true, title, true);
            window.Initialize();
            return window;
        }

        public virtual void Initialize()
        {
            yesText = "";
            noText = "";
            summaryText = "";
            bodyText = "";
            result = false;
            scrollPosition = new Vector2(0, 0);
            minSize = new Vector2(300, 200);
            position = new Rect(UnityEngine.Screen.width / 3, UnityEngine.Screen.height / 3,
                                minSize.x * 2, minSize.y * 2);
        }

        // Draw the GUI.
        protected virtual void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Label(summaryText, EditorStyles.boldLabel);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            // Unity text elements can only display up to a small X number of characters (rumors
            // are ~65k) so generate a set of labels one for each subset of the text being
            // displayed.
            int bodyTextOffset = 0;
            System.Collections.Generic.List<string> bodyTextList =
                new System.Collections.Generic.List<string>();
            const int chunkSize = 5000;  // Conservative chunk size < 65k characters.
            while (bodyTextOffset < bodyText.Length)
            {
                int readSize = chunkSize;
                readSize = bodyTextOffset + readSize >= bodyText.Length ?
                    bodyText.Length - bodyTextOffset : readSize;
                bodyTextList.Add(bodyText.Substring(bodyTextOffset, readSize));
                bodyTextOffset += readSize;
            }
            foreach (string bodyTextChunk in bodyTextList)
            {
                GUILayout.Label(bodyTextChunk, EditorStyles.wordWrappedLabel);
            }
            GUILayout.EndScrollView();

            bool yesPressed = false;
            bool noPressed = false;
            GUILayout.BeginHorizontal();
            if (yesText != "") yesPressed = GUILayout.Button(yesText);
            if (noText != "") noPressed = GUILayout.Button(noText);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            // If yes or no buttons were pressed, call the buttonClicked delegate.
            if (yesPressed || noPressed)
            {
                if (yesPressed)
                {
                    result = true;
                }
                else if (noPressed)
                {
                    result = false;
                }
                if (buttonClicked != null) buttonClicked(this);
            }
        }

        // Optionally make the dialog modal.
        protected virtual void OnLostFocus()
        {
            if (modal) Focus();
        }
    }

}
