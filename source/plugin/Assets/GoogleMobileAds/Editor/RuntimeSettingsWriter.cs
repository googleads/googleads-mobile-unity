using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GoogleMobileAds.Editor
{
  /// <summary>
  /// Writes settings that are needed at runtime, currently the UMP consent debug settings
  /// (DebugGeography and TestDeviceHashedIds) configured in
  /// <see cref="GoogleMobileAdsSettings"/> to an XML file under StreamingAssets. Files under
  /// StreamingAssets are copied as-is into the built player so they can be read at runtime.
  /// </summary>
  internal static class RuntimeSettingsWriter
  {
    // Project-relative directory of the runtime settings file.
    internal const string OutputDirectory = "Assets/StreamingAssets/GoogleMobileAds";

    internal const string OutputFileName = "RuntimeSettings.xml";

    /// <summary>
    /// Writes the runtime settings from <paramref name="settings"/> to
    /// <see cref="OutputDirectory"/>/<see cref="OutputFileName"/>.
    /// </summary>
    internal static void Write(GoogleMobileAdsSettings settings)
    {
      Write(OutputDirectory, settings.DebugGeography, settings.TestDeviceHashedIds);
    }

    /// <summary>
    /// Writes the given runtime settings to an XML file in <paramref name="directory"/>,
    /// overwriting any existing file.
    /// </summary>
    internal static void Write(string directory, int debugGeography,
                               IEnumerable<string> testDeviceHashedIds)
    {
      Directory.CreateDirectory(directory);
      CreateXml(debugGeography, testDeviceHashedIds).Save(Path.Combine(directory, OutputFileName));
    }

    /// <summary>
    /// Creates the runtime settings XML document, e.g.:
    /// <code>
    /// &lt;RuntimeSettings&gt;
    ///   &lt;DebugGeography&gt;1&lt;/DebugGeography&gt;
    ///   &lt;TestDeviceHashedIds&gt;
    ///     &lt;TestDeviceHashedId&gt;33BE2250B43518CCDA7DE426D04EE231&lt;/TestDeviceHashedId&gt;
    ///   &lt;/TestDeviceHashedIds&gt;
    /// &lt;/RuntimeSettings&gt;
    /// </code>
    /// DebugGeography is the integer value of <c>GoogleMobileAds.Ump.Api.DebugGeography</c>.
    /// </summary>
    internal static XDocument CreateXml(int debugGeography, IEnumerable<string> testDeviceHashedIds)
    {
      return new XDocument(
          new XDeclaration("1.0", "utf-8", null),
          new XElement("RuntimeSettings",
              new XElement("DebugGeography", debugGeography),
              new XElement("TestDeviceHashedIds",
                  SanitizeTestDeviceHashedIds(testDeviceHashedIds)
                      .Select(id => new XElement("TestDeviceHashedId", id)))));
    }

    // Trims IDs and drops empty entries (e.g. rows added but not yet filled in).
    private static List<string> SanitizeTestDeviceHashedIds(IEnumerable<string> testDeviceHashedIds)
    {
      return (testDeviceHashedIds ?? Enumerable.Empty<string>())
          .Where(id => !string.IsNullOrWhiteSpace(id))
          .Select(id => id.Trim())
          .ToList();
    }
  }
}
