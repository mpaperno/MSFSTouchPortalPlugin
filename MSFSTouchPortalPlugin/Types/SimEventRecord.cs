using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Types
{
  public struct SimEventRecord
  {
    public Groups GroupId;
    public string EventName;
    public SimEventRecord(Groups id, string name) {
      GroupId = id;
      EventName = name;
    }
  }
}
