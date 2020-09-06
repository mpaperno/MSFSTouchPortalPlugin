using System;

namespace TouchPortalExtension.Attributes {
  public enum DataType {
    None,
    Text,
    Number,
    Switch,
    Choice
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalActionAttribute : Attribute {
    public string Id;
    public string Name;
    public string Prefix;
    public string Description;
    public string Format;
    public string Type;

    public TouchPortalActionAttribute(string id, string name, string prefix, string description, string format, string type = "communicate") {
      Id = id;
      Name = name;
      Prefix = prefix;
      Description = description;
      Format = format;
      Type = type;
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public class TouchPortalActionChoiceAttribute : Attribute {
    public string[] ChoiceValues;
    public string DefaultValue;

    public TouchPortalActionChoiceAttribute(string[] choiceValues, string defaultValue) {
      ChoiceValues = choiceValues;
      DefaultValue = defaultValue;
    }
  }
}
