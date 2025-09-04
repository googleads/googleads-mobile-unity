using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace GoogleMobileAds.Common
{
  // TODO: b/431227569 - Use this class when the SDK compiles in dev/test mode.
  public class TestInsightsEmitter : IInsightsEmitter
  {
      // LINT.IfChange
      private const string _DEFAULT_INSIGHTS_FILE_NAME = "unity_insights.log";
      // LINT.ThenChange(//depot/google3/javatests/com/google/android/apps/internal/admobsdk/mediumtest/unityplugin/UnityTestUtils.java)

      private readonly string _filePath;
      private readonly bool _canWrite = true;
      private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

      /**
       * Creates a new TestInsightsEmitter.
       *
       * @param filePath The file path to write the insights to. If null, the file will be written
       *     to the external storage directory.
       */
      public TestInsightsEmitter(string filePath = null)
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

      public void Emit(string insight)
      {
          if (!_canWrite)
          {
            return;
          }

          _lock.EnterWriteLock();
          try
          {
              Debug.Log("Writing insight: " + insight);

              // Writing needs to be synchronous for the read cursor to consume insights in order.
              File.AppendAllText(_filePath, insight + Environment.NewLine);
          }
          finally
          {
              _lock.ExitWriteLock();
          }
      }
  }
}
