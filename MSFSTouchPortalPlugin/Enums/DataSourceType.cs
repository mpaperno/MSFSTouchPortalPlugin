
namespace MSFSTouchPortalPlugin.Enums
{
  public enum DataSourceType : short
  {
    None = 0,
    DefaultFile,  // loaded from default file(s) from plugin distro
    UserFile,     // loaded from user-provided file, either at startup or dynamically from Action
    Dynamic,      // added (or edited) by user via TP Action
    Imported,     // for meta data
  }
}
