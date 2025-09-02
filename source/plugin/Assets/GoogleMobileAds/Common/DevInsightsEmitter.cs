using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace GoogleMobileAds.Common
{
  // Emits insights when the SDK compiles in dev build mode.
  // TODO: b/431227569 - We should add tracing to the Unity plugin, like for the Decagon SDK:
  // http://google3/java/com/google/android/libraries/ads/mobile/sdk/internal/tracing/ExportingDebugTraceMonitor.kt.
  public class InsightsEmitter : IInsightsEmitter
  {
      // LINT.IfChange
      private const string _DEFAULT_INSIGHTS_FILE_NAME = "unity_insights.jsonl";
      // LINT.ThenChange(//depot/google3/javatests/com/google/android/apps/internal/admobsdk/mediumtest/unityplugin/UnityTestUtils.java)

      private readonly string _filePath;
      private readonly bool _canWrite = true;
      private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

      /**
       * Creates a new InsightsEmitter.
       *
       * @param filePath The file path to write the insights to. If null, the file will be written
       *     to the external storage directory.
       */
      public InsightsEmitter(string filePath = null)
      {
          _filePath = filePath ?? Path.Combine(Application.persistentDataPath,
              _DEFAULT_INSIGHTS_FILE_NAME);
          Debug.Log("Unity insights will be written to: " + _filePath);

          try
          {
            // No-op if the directory and subdirectories already exist.
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
          } catch (Exception e) {
            Debug.LogError("Failed to create directory for Unity insights (no insights will be " +
                "written): " + e.Message);
            _canWrite = false;
          }
      }

      public void Emit(Insight insight)
      {
          if (!_canWrite)
          {
            return;
          }

          _lock.EnterWriteLock();
          try
          {
              Debug.Log("Writing insight: " + insight.ToString());

              // Writing needs to be synchronous for the read cursor to consume insights in order.
              File.AppendAllText(_filePath, insight.ToJson() + Environment.NewLine);
          }
          finally
          {
              _lock.ExitWriteLock();
          }
      }
  }
}
