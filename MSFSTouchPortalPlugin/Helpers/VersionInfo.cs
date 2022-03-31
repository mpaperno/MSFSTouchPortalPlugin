
namespace MSFSTouchPortalPlugin.Helpers
{
  /// <summary>
  /// Helper class to get version information about C#/.Net assemblies (DLLs).
  /// </summary>
  internal static class VersionInfo
  {

    /// <summary>
    /// The .dll for version information lookup. Defaults to currently executing assembly.
    /// </summary>
    internal static System.Reflection.Assembly Assembly { get; set; } = System.Reflection.Assembly.GetExecutingAssembly();

    /// <summary>
    /// The .dll file path to use for version information lookup. Defaults to VersionInfo.Assembly.Location
    /// </summary>
    internal static string AssemblyLocation { get; set; } = Assembly.Location;

    /// <summary>
    /// Short name of the DLL specified in Assembly property.
    /// </summary>
    internal static string AssemblyName { get; set; } = Assembly.GetName().Name;

    /// <summary>
    /// Returns an integer which represents version number in hex notation, eg. 1.22.3.0 => 0x1220300
    /// </summary>
    internal static uint GetProductVersionNumber() {
      var vi = GetVersionInfo();
      return (uint)((byte)(vi.ProductMajorPart & 0xFF) << 24 | (byte)(vi.ProductMinorPart & 0xFF) << 16 | (byte)(vi.ProductBuildPart & 0xFF) << 8 | (byte)(vi.ProductPrivatePart & 0xFF));
    }

    /// <summary>
    /// Returns a string representing the full version in dotted notation, eg. 1.22.3.0
    /// </summary>
    internal static string GetProductVersionString() {
      var vi = GetVersionInfo();
      return $"{vi.ProductMajorPart}.{vi.ProductMinorPart}.{vi.ProductBuildPart}.{vi.ProductPrivatePart}";
    }

    /// <summary>
    /// Returns a System.Diagnostics.FileVersionInfo for a file at given location, or AssemblyLocation if location is default/null.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    internal static System.Diagnostics.FileVersionInfo GetVersionInfo(string location = default)
      => System.Diagnostics.FileVersionInfo.GetVersionInfo(location ?? AssemblyLocation);

  }
}
