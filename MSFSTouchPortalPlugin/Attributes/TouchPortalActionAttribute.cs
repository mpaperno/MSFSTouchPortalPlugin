using System;

namespace MSFSTouchPortalPlugin.Attributes {
  class TouchPortalActionAttribute : Attribute {
    public string Id;
    public string Name;
    public string Prefix;
    public string Type;
    public string Description;
    public string Format;

    public TouchPortalActionAttribute(string id, string name, string prefix, string description, string format, string type = "communicate") {
      Id = id;
      Name = name;
      Prefix = prefix;
      Description = description;
      Format = format;
      Type = type;
    }
  }
}
