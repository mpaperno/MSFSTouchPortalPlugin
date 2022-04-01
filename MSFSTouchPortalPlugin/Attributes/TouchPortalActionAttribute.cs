using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Attributes
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalActionAttribute : Attribute
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Prefix { get; set; }
    public string Description { get; set; }
    public string Format { get; set; }
    public string Type { get; set; }
    public bool HasHoldFunctionality { get; set; }

    public TouchPortalActionAttribute(string id, string name, string prefix, string description, string format, bool holdable = false) {
      SetupProperties(id, name, prefix, description, format, holdable, "communicate");
    }

    private void SetupProperties(string id, string name, string prefix, string description, string format, bool holdable, string type) {
      Id = id;
      Name = name;
      Prefix = prefix;
      Description = description;
      Format = format;
      Type = type;
      HasHoldFunctionality = holdable;
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public class TouchPortalActionDataAttribute : Attribute
  {
    //public string Id { get; set; }
    public DataType ValueType { get; set; }
    public virtual string Label { get; set; } = "Action";
    public virtual bool AllowDecimals { get; set; } = true;  // this default will prevent includsion in entry.tp by json generator
    public virtual double MinValue { get; set; } = double.NaN;
    public virtual double MaxValue { get; set; } = double.NaN;
    public virtual string[] ChoiceValues { get; set; }
    public string Type
    {
      get {
        switch (ValueType) {
          case DataType.Number:
            return "number";
          case DataType.Switch:
            return "switch";
          case DataType.Choice:
            return "choice";
          case DataType.Text:
          default:
            return "text";
        }
      }
    }

    protected dynamic _defaultValue;

    public dynamic GetDefaultValue() {
      try {
        switch (ValueType) {
          case DataType.Number:
            return Convert.ToDouble(_defaultValue);
          case DataType.Switch:
            return Convert.ToBoolean(_defaultValue);
          default:
            return Convert.ToString(_defaultValue);
        }
      }
      catch {
        switch (ValueType) {
          case DataType.Number:
            return 0.0;
          case DataType.Switch:
            return false;
          default:
            return string.Empty;
        }
      }
    }

    protected TouchPortalActionDataAttribute(DataType type) {
      ValueType = type;
    }
  }

  public class TouchPortalActionTextAttribute : TouchPortalActionDataAttribute
  {
    public virtual string DefaultValue
    {
      get { return (string)GetDefaultValue(); }
      set { _defaultValue = value; }
    }

    public TouchPortalActionTextAttribute(string defaultValue = "") : base(DataType.Text) {
      DefaultValue = defaultValue;
    }

    public TouchPortalActionTextAttribute(string defaultValue, int minValue, int maxValue) : base(DataType.Text) {
      DefaultValue = defaultValue;
      MinValue = minValue;
      MaxValue = maxValue;
      AllowDecimals = false;
    }

    protected TouchPortalActionTextAttribute(DataType type, string defaultValue = "") : this(defaultValue) {
      ValueType = type;
    }
  }

  public class TouchPortalActionChoiceAttribute : TouchPortalActionTextAttribute
  {

    public TouchPortalActionChoiceAttribute(string[] choiceValues, string defaultValue = default) : base(DataType.Choice, defaultValue) {
      ChoiceValues = choiceValues;
      if (defaultValue == default && choiceValues?.Length > 0)
        DefaultValue = choiceValues[0];
    }
  }

  public class TouchPortalActionSwitchAttribute : TouchPortalActionDataAttribute
  {
    public bool DefaultValue
    {
      get { return (bool)GetDefaultValue(); }
      set { _defaultValue = value; }
    }

    public TouchPortalActionSwitchAttribute(bool defaultValue = false) : base(DataType.Switch) {
      DefaultValue = defaultValue;
    }
  }

  public class TouchPortalActionNumericAttribute : TouchPortalActionDataAttribute
  {
    public double DefaultValue
    {
      get { return (double)GetDefaultValue(); }
      set { _defaultValue = value; }
    }

    public TouchPortalActionNumericAttribute(double defaultValue = 0.0, double minValue = int.MinValue, double maxValue = int.MaxValue, bool allowDecimals = false) : base(DataType.Number) {
      DefaultValue = defaultValue;
      MinValue = minValue;
      MaxValue = maxValue;
      AllowDecimals = allowDecimals;
    }
  }

}
