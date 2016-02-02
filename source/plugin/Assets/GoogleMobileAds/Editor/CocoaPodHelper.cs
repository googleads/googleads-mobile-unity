using System;
using System.Diagnostics;
using System.IO;

namespace GoogleMobileAds
{
    public class CocoaPodHelper
    {
        public static string Update(string projDir)
        {
            if (!Directory.Exists(projDir))
            {
                throw new Exception("project not found: " + projDir);
            }

            string podPath = ExecuteCommand("which", "pod", null);
            if (podPath.Equals(""))
            {
                throw new Exception("pod executable not found");
            }
            return ExecuteCommand(podPath.Trim(), "update", projDir);
        }

        private static string ExecuteCommand(string command, string argument, string workingDir)
        {
            using (var process = new Process())
            {
                if (!process.StartInfo.EnvironmentVariables.ContainsKey("LANG"))
                {
                    process.StartInfo.EnvironmentVariables.Add("LANG", "en_US.UTF-8");
                }

                string path = process.StartInfo.EnvironmentVariables["PATH"];
                if(!path.Contains("/usr/local/bin"))
                {
                    path = path + ":/usr/local/bin";
                    process.StartInfo.EnvironmentVariables.Remove("PATH");
                    process.StartInfo.EnvironmentVariables.Add("PATH", path);
                }

                if (workingDir != null)
                {
                    process.StartInfo.WorkingDirectory = workingDir;
                }
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = argument;
                UnityEngine.Debug.Log("Executing " + command + " argument: " +
                    process.StartInfo.Arguments);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                try
                {
                    process.Start();
                    process.StandardError.ReadToEnd();
                    var stdOutput = process.StandardOutput.ReadToEnd();
                    var stdError = process.StandardError.ReadToEnd();

                    UnityEngine.Debug.Log("command stdout: " + stdOutput);

                    if (stdError != null && stdError.Length > 0)
                    {
                        UnityEngine.Debug.LogError("command stderr: " + stdError);
                    }

                    if (!process.WaitForExit(10 * 1000))
                    {
                        throw new Exception("command did not exit in a timely fashion");
                    }

                    return stdOutput;

                }
                catch (Exception e)
                {
                    throw new Exception("Encountered unexpected error while running pod", e);
                }
            }
        }
    }
}
