using System;

namespace TouchPortalExtension.Attributes {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalStateAttribute : Attribute {
    public string Id { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Default { get; set; }

    public TouchPortalStateAttribute(string id, string type, string description, string defaultValue) {
      SetupProperties(id, type, description, defaultValue);
    }

    private void SetupProperties(string id, string type, string description, string defaultValue) {
      Id = id;
      Type = type;
      Description = description;
      Default = defaultValue;
    }
  }
}
