// <copyright file="CommandLine.cs" company="Google Inc.">
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
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif  // UNITY_EDITOR

    public static class CommandLine
    {
        /// <summary>
        /// Result from Run().
        /// </summary>
        public class Result
        {
            /// String containing the standard output stream of the tool.
            public string stdout;
            /// String containing the standard error stream of the tool.
            public string stderr;
            /// Exit code returned by the tool when execution is complete.
            public int exitCode;
        };

        /// <summary>
        /// Called when a RunAsync() completes.
        /// </summary>
        public delegate void CompletionHandler(Result result);

        /// <summary>
        /// Text and byte representations of an array of data.
        /// </summary>
        public class StreamData
        {
            /// <summary>
            /// Handle to the stream this was read from.
            /// e.g 0 for stdout, 1 for stderr.
            /// </summary>
            public int handle = 0;
            /// <summary>
            /// Text representation of "data".
            /// </summary>
            public string text = "";
            /// <summary>
            /// Array of bytes or "null" if no data is present.
            /// </summary>
            public byte[] data = null;
            /// <summary>
            /// Whether this marks the end of the stream.
            /// </summary>
            public bool end;

            /// <summary>
            /// Initialize this instance.
            /// </summary>
            /// <param name="handle">Stream identifier.</param>
            /// <param name="text">String</param>
            /// <param name="data">Bytes</param>
            /// <param name="end">Whether this is the end of the stream.</param>
            public StreamData(int handle, string text, byte[] data, bool end)
            {
                this.handle = handle;
                this.text = text;
                this.data = data;
                this.end = end;
            }

            /// <summary>
            /// Get an empty StreamData instance.
            /// </summary>
            public static StreamData Empty
            {
                get { return new StreamData(0, "", null, false); }
            }
        }

        /// <summary>
        /// Called when data is received from either the standard output or standard error
        /// streams with a reference to the current standard input stream to enable simulated
        /// interactive input.
        /// </summary>
        /// <param name="process">Executing process.</param>
        /// <param name="stdin">Standard input stream.</param>
        /// <param name="stream">Data read from the standard output or error streams.</param>
        public delegate void IOHandler(Process process, StreamWriter stdin, StreamData streamData);

        /// <summary>
        /// Asynchronously execute a command line tool, calling the specified delegate on
        /// completion.
        /// </summary>
        /// <param name="toolPath">Tool to execute.</param>
        /// <param name="arguments">String to pass to the tools' command line.</param>
        /// <param name="completionDelegate">Called when the tool completes.</param>
        /// <param name="workingDirectory">Directory to execute the tool from.</param>
        /// <param name="envVars">Additional environment variables to set for the command.</param>
        /// <param name="ioHandler">Allows a caller to provide interactive input and also handle
        /// both output and error streams from a single delegate.</param>
        public static void RunAsync(
            string toolPath, string arguments, CompletionHandler completionDelegate,
            string workingDirectory = null,
            Dictionary<string, string> envVars = null,
            IOHandler ioHandler = null)
        {
            Thread thread = new Thread(new ThreadStart(() => {
                    Result result = Run(toolPath, arguments, workingDirectory, envVars: envVars,
                                        ioHandler: ioHandler);
                    completionDelegate(result);
                }));
            thread.Start();
        }

        /// <summary>
        /// Asynchronously reads binary data from a stream using a configurable buffer.
        /// </summary>
        private class AsyncStreamReader
        {
            /// <summary>
            /// Delegate called when data is read from the stream.
            /// <param name="streamData">Data read from the stream.</param>
            /// </summary>
            public delegate void Handler(StreamData streamData);
            /// <summary>
            /// Event which is signalled when data is received.
            /// </summary>
            public event Handler DataReceived;

            // Signalled when a read completes.
            private AutoResetEvent readEvent = null;
            // Handle to the stream.
            private int handle;
            // Stream to read.
            private Stream stream;
            // Buffer used to read data from the stream.
            private byte[] buffer;
            // Whether reading is complete.
            volatile bool complete = false;

            /// <summary>
            /// Initialize the reader.
            /// </summary>
            /// <param name="stream">Stream to read.</param>
            /// <param name="bufferSize">Size of the buffer to read.</param>
            public AsyncStreamReader(int handle, Stream stream, int bufferSize)
            {
                readEvent = new AutoResetEvent(false);
                this.handle = handle;
                this.stream = stream;
                buffer = new byte[bufferSize];
            }

            /// <summary>
            /// Get the handle of the stream associated with this reader.
            /// </summary>
            public int Handle
            {
                get
                {
                    return handle;
                }
            }


            /// <summary>
            /// Start reading.
            /// </summary>
            public void Start()
            {
                if (!complete) (new Thread(new ThreadStart(Read))).Start();
            }

            /// <summary>
            /// Read from the stream until the end is reached.
            /// </summary>
            private void Read()
            {
                while (!complete)
                {
                    stream.BeginRead(
                        buffer, 0, buffer.Length, (asyncResult) => {
                            int bytesRead = stream.EndRead(asyncResult);
                            if (!complete)
                            {
                                complete = bytesRead == 0;
                                if (DataReceived != null)
                                {
                                    byte[] copy = new byte[bytesRead];
                                    Array.Copy(buffer, copy, copy.Length);
                                    DataReceived(new StreamData(
                                        handle, System.Text.Encoding.UTF8.GetString(copy), copy,
                                        complete));
                                }
                            }
                            readEvent.Set();
                        }, null);
                    readEvent.WaitOne();
                }
            }

            /// <summary>
            /// Create a set of readers to read the specified streams, handles are assigned
            /// based upon the index of each stream in the provided array.
            /// </summary>
            /// <param name="streams">Streams to read.</param>
            /// <param name="bufferSize">Size of the buffer to use to read each stream.</param>
            public static AsyncStreamReader[] CreateFromStreams(Stream[] streams, int bufferSize)
            {
                AsyncStreamReader[] readers = new AsyncStreamReader[streams.Length];
                for (int i = 0; i < streams.Length; i++)
                {
                    readers[i] = new AsyncStreamReader(i, streams[i], bufferSize);
                }
                return readers;
            }
        }

        /// <summary>
        /// Multiplexes data read from multiple AsyncStreamReaders onto a single thread.
        /// </summary>
        private class AsyncStreamReaderMultiplexer
        {
            /// Used to wait on items in the queue.
            private AutoResetEvent queuedItem = null;
            /// Queue of Data read from the readers.
            private System.Collections.Queue queue = null;
            /// Active stream handles.
            private HashSet<int> activeStreams;

            /// <summary>
            /// Called when all streams reach the end or the reader is shut down.
            /// </summary>
            public delegate void CompletionHandler();


            /// <summary>
            /// Called when all streams reach the end or the reader is shut down.
            /// </summary>
            public event CompletionHandler Complete;

            /// <summary>
            /// Handler called from the multiplexer's thread.
            /// </summary>
            public event AsyncStreamReader.Handler DataReceived;

            /// <summary>
            /// Create the multiplexer and attach it to the specified handler.
            /// </summary>
            /// <param name="readers">Readers to read.</param>
            /// <param name="handler">Called for queued data item.</param>
            /// <param name="complete">Called when all readers complete.</param>
            public AsyncStreamReaderMultiplexer(AsyncStreamReader[] readers,
                                                AsyncStreamReader.Handler handler,
                                                CompletionHandler complete = null)
            {
                queuedItem = new AutoResetEvent(false);
                queue = System.Collections.Queue.Synchronized(new System.Collections.Queue());
                activeStreams = new HashSet<int>();
                foreach (AsyncStreamReader reader in readers)
                {
                    reader.DataReceived += HandleRead;
                    activeStreams.Add(reader.Handle);
                }
                DataReceived += handler;
                if (complete != null) Complete += complete;
                (new Thread(new ThreadStart(PollQueue))).Start();
            }

            /// <summary>
            /// Shutdown the multiplexer.
            /// </summary>
            public void Shutdown()
            {
                lock (activeStreams)
                {
                    activeStreams.Clear();
                }
                queuedItem.Set();
            }

            // Handle stream read notification.
            private void HandleRead(StreamData streamData)
            {
                queue.Enqueue(streamData);
                queuedItem.Set();
            }

            // Poll the queue.
            private void PollQueue()
            {
                while (activeStreams.Count > 0)
                {
                    queuedItem.WaitOne();
                    while (queue.Count > 0)
                    {
                        StreamData data = (StreamData)queue.Dequeue();
                        if (data.end)
                        {
                            lock(activeStreams)
                            {
                                activeStreams.Remove(data.handle);
                            }
                        }
                        if (DataReceived != null) DataReceived(data);
                    }
                }
                if (Complete != null) Complete();
            }
        }

        /// <summary>
        /// Aggregates lines read by AsyncStreamReader.
        /// </summary>
        public class LineReader
        {
            // List of data keyed by stream handle.
            private Dictionary<int, List<StreamData>> streamDataByHandle =
                new Dictionary<int, List<StreamData>>();

            /// <summary>
            /// Event called with a new line of data.
            /// </summary>
            public event IOHandler LineHandler;

            /// <summary>
            /// Event called for each piece of data received.
            /// </summary>
            public event IOHandler DataHandler;

            /// <summary>
            /// Initialize the instance.
            /// </summary>
            /// <param name="handler">Called for each line read.</param>
            public LineReader(IOHandler handler = null)
            {
                if (handler != null) LineHandler += handler;
            }

            /// <summary>
            /// Retrieve the currently buffered set of data.
            /// This allows the user to retrieve data before the end of a stream when
            /// a newline isn't present.
            /// </summary>
            /// <param name="handle">Handle of the stream to query.</param>
            /// <returns>List of data for the requested stream.</return>
            public List<StreamData> GetBufferedData(int handle)
            {
                List<StreamData> handleData;
                return streamDataByHandle.TryGetValue(handle, out handleData) ?
                    new List<StreamData>(handleData) : new List<StreamData>();
            }

            /// <summary>
            /// Flush the internal buffer.
            /// </summary>
            public void Flush()
            {
                foreach (List<StreamData> handleData in streamDataByHandle.Values)
                {
                    handleData.Clear();
                }
            }

            /// <summary>
            /// Aggregate the specified list of StringBytes into a single structure.
            /// </summary>
            /// <param name="handle">Stream handle.</param>
            /// <param name="dataStream">Data to aggregate.</param>
            public static StreamData Aggregate(List<StreamData> dataStream)
            {
                // Build a list of strings up to the newline.
                List<string> stringList = new List<string>();
                int byteArraySize = 0;
                int handle = 0;
                bool end = false;
                foreach (StreamData sd in dataStream)
                {
                    stringList.Add(sd.text);
                    byteArraySize += sd.data.Length;
                    handle = sd.handle;
                    end |= sd.end;
                }
                string concatenatedString = String.Join("", stringList.ToArray());

                // Concatenate the list of bytes up to the StringBytes before the
                // newline.
                byte[] byteArray = new byte[byteArraySize];
                int byteArrayOffset = 0;
                foreach (StreamData sd in dataStream)
                {
                    Array.Copy(sd.data, 0, byteArray, byteArrayOffset, sd.data.Length);
                    byteArrayOffset += sd.data.Length;
                }
                return new StreamData(handle, concatenatedString, byteArray, end);
            }

            /// <summary>
            /// Delegate method which can be attached to AsyncStreamReader.DataReceived to
            /// aggregate data until a new line is found before calling LineHandler.
            /// </summary>
            public void AggregateLine(Process process, StreamWriter stdin, StreamData data)
            {
                if (DataHandler != null) DataHandler(process, stdin, data);
                bool linesProcessed = false;

                if (data.data != null)
                {
                    // Simplify line tracking by converting linefeeds to newlines.
                    data.text = data.text.Replace("\r\n", "\n").Replace("\r", "\n");

                    List<StreamData> aggregateList = GetBufferedData(data.handle);
                    aggregateList.Add(data);
                    bool complete = false;
                    while (!complete)
                    {
                        List<StreamData> newAggregateList = new List<StreamData>();
                        int textDataIndex = 0;
                        int aggregateListSize = aggregateList.Count;
                        complete = true;

                        foreach (StreamData textData in aggregateList)
                        {
                            bool end = data.end && (++textDataIndex == aggregateListSize);
                            newAggregateList.Add(textData);
                            int newlineOffset = textData.text.Length;
                            if (!end)
                            {
                                newlineOffset = textData.text.IndexOf("\n");
                                if (newlineOffset < 0) continue;
                                newAggregateList.Remove(textData);
                            }

                            StreamData concatenated = Aggregate(newAggregateList);

                            // Flush the aggregation list and store the overflow.
                            newAggregateList.Clear();
                            if (!end)
                            {
                                // Add the remaining line to the concatenated data.
                                concatenated.text += textData.text.Substring(0, newlineOffset + 1);
                                // Save the line after the newline in the buffer.
                                newAggregateList.Add(new StreamData(
                                    data.handle, textData.text.Substring(newlineOffset + 1),
                                    textData.data, false));
                                complete = false;
                            }

                            // Report the data.
                            if (LineHandler != null) LineHandler(process, stdin, concatenated);
                            linesProcessed = true;
                        }
                        aggregateList = newAggregateList;
                    }
                    streamDataByHandle[data.handle] = aggregateList;
                }
                // If no lines were processed call the handle to allow it to look ahead.
                if (!linesProcessed && LineHandler != null)
                {
                    LineHandler(process, stdin, StreamData.Empty);
                }
            }
        }

        /// <summary>
        /// Execute a command line tool.
        /// </summary>
        /// <param name="toolPath">Tool to execute.</param>
        /// <param name="arguments">String to pass to the tools' command line.</param>
        /// <param name="workingDirectory">Directory to execute the tool from.</param>
        /// <param name="envVars">Additional environment variables to set for the command.</param>
        /// <param name="ioHandler">Allows a caller to provide interactive input and also handle
        /// both output and error streams from a single delegate.</param>
        /// <returns>CommandLineTool result if successful, raises an exception if it's not
        /// possible to execute the tool.</returns>
        public static Result Run(string toolPath, string arguments, string workingDirectory = null,
                                 Dictionary<string, string> envVars = null,
                                 IOHandler ioHandler = null)
        {
            System.Text.Encoding inputEncoding = Console.InputEncoding;
            System.Text.Encoding outputEncoding = Console.OutputEncoding;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            if (envVars != null)
            {
                foreach (var env in envVars)
                {
                    process.StartInfo.EnvironmentVariables[env.Key] = env.Value;
                }
            }
            process.StartInfo.RedirectStandardInput = (ioHandler != null);
            process.StartInfo.FileName = toolPath;
            process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;

            process.Start();

            // If an I/O handler was specified, call it with no data to provide a process and stdin
            // handle before output data is sent to it.
            if (ioHandler != null) ioHandler(process, process.StandardInput, StreamData.Empty);

            AutoResetEvent complete = new AutoResetEvent(false);
            List<string>[] stdouterr = new List<string>[] { new List<string>(),
                                                            new List<string>() };
            // Read raw output from the process.
            AsyncStreamReader[] readers = AsyncStreamReader.CreateFromStreams(
                new Stream[] { process.StandardOutput.BaseStream,
                               process.StandardError.BaseStream }, 1);
            new AsyncStreamReaderMultiplexer(
                readers,
                (StreamData data) => {
                    stdouterr[data.handle].Add(data.text);
                    if (ioHandler != null) ioHandler(process, process.StandardInput, data);
                },
                complete: () => { complete.Set(); });
            foreach (AsyncStreamReader reader in readers) reader.Start();

            process.WaitForExit();
            // Wait for the reading threads to complete.
            complete.WaitOne();

            Result result = new Result();
            result.stdout = String.Join("", stdouterr[0].ToArray());
            result.stderr = String.Join("", stdouterr[1].ToArray());
            result.exitCode = process.ExitCode;
            Console.InputEncoding = inputEncoding;
            Console.OutputEncoding = outputEncoding;
            return result;
        }

        /// <summary>
        /// Split a string into lines using newline, carriage return or a combination of both.
        /// </summary>
        /// <param name="multilineString">String to split into lines</param>
        public static string[] SplitLines(string multilineString)
        {
            return Regex.Split(multilineString, "\r\n|\r|\n");
        }

#if UNITY_EDITOR
        /// <summary>
        /// Get an executable extension.
        /// </summary>
        /// <returns>Platform specific extension for executables.</returns>
        public static string GetExecutableExtension()
        {
            return (UnityEngine.RuntimePlatform.WindowsEditor ==
                    UnityEngine.Application.platform) ? ".exe" : "";
        }

        /// <summary>
        /// Locate an executable in the system path.
        /// </summary>
        /// <param name="exeName">Executable name without a platform specific extension like
        /// .exe</param>
        /// <returns>A string to the executable path if it's found, null otherwise.</returns>
        public static string FindExecutable(string executable)
        {
            string which = (UnityEngine.RuntimePlatform.WindowsEditor ==
                            UnityEngine.Application.platform) ? "where" : "which";
            try
            {
                Result result = Run(which, executable, Environment.CurrentDirectory);
                if (result.exitCode == 0)
                {
                    return SplitLines(result.stdout)[0];
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("'" + which + "' command is not on path.  " +
                                      "Unable to find executable '" + executable +
                                      "' (" + e.ToString() + ")");
            }
            return null;
        }
#endif  // UNITY_EDITOR
    }
}
