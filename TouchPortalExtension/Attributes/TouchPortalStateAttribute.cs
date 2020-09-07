using System;

namespace TouchPortalExtension.Attributes {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalStateAttribute : Attribute {
    public string Id;
    public string Type;
    public string Description;
    public string Default;

    public TouchPortalStateAttribute(string id, string type = "", string description = "", string defaultValue = "") {
      Id = id;
      Type = type;
      Description = description;
      Default = defaultValue;
    }
  }
}
