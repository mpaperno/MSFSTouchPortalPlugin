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
    public string Id { get; set; }
    public string Name { get; set; }
    public string Prefix { get; set; }
    public string Description { get; set; }
    public string Format { get; set; }
    public string Type { get; set; }

    public TouchPortalActionAttribute(string id, string name, string prefix, string description, string format) {
      SetupProperties(id, name, prefix, description, format, "communicate");
    }

    private void SetupProperties(string id, string name, string prefix, string description, string format, string type) {
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
    public string[] ChoiceValues { get; set; }
    public string DefaultValue { get; set; }

    public TouchPortalActionChoiceAttribute(string[] choiceValues, string defaultValue) {
      ChoiceValues = choiceValues;
      DefaultValue = defaultValue;
    }
  }
}
