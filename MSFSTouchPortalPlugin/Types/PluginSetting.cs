using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Types
{
  public class PluginSetting
  {
    public string SettingID { get; set; }
    public string Name { get; set; } = null;
    public string Description { get; set; }  // for generated docs
    public string Default { get; set; } = null;
    public int MaxLength { get; set; } = int.MinValue;
    public double MinValue { get; set; } = double.NaN;
    public double MaxValue { get; set; } = double.NaN;

    public bool ReadOnly { get; set; } = false;    // for TP definition (maybe also future use)
    public bool IsPassword { get; set; } = false;  // for TP definition
    public string TouchPortalStateId { get; set; } = null;  // Id of corresponding TP State, if any
    public string TouchPortalType { get; private set; } = "text";     // TP definition type equivalent, "text" or "number".

    private dynamic _value = null;
    private DataType _type = DataType.Text;

    public dynamic Value
    {
      get {
        if (_value == null && Default != null)
          SetValueFromString(Default);
        return _value;
      }
      set {
        SetValueDynamic(value);
      }
    }

    public DataType ValueType {
      get => _type;
      set {
        _type = value;
        TouchPortalType = _type == DataType.Text ? "text" : "number";  // treat Switch as numeric and Choice is not supported
        if (_type == DataType.Switch) {
          MinValue = 0;
          MaxValue = 1;
        }
      }
    }

    public void SetValueFromString(string value) {
      switch (ValueType) {
        case DataType.Number: {
            if (double.TryParse(value, out var numVal))
              Value = numVal;
            break;
          }

        case DataType.Switch:
          Value = (bool)new BooleanString(value);
          break;

        default:
          Value = value;
          break;
      }
    }

    public void SetValueDynamic(dynamic value) {
      try {
        if (ValueType == DataType.Number) {
          double realVal = Convert.ToDouble(value);
          if (!double.IsNaN(MinValue))
            realVal = Math.Max(realVal, MinValue);
          if (!double.IsNaN(MaxValue))
            realVal = Math.Min(realVal, MaxValue);
          _value = realVal;
        }
        // string or "switch" bool
        else {
          string strVal = Convert.ToString(value);
          if (MaxLength > 0 && !string.IsNullOrEmpty(strVal))
            strVal = strVal[..Math.Min(strVal.Length, MaxLength)];
          if (ValueType == DataType.Switch)
            _value = (bool)new BooleanString(strVal);
          else
            _value = strVal;
        }
      }
      catch (Exception e) {
        throw new ArgumentException("Cannot convert value to intended type.", e);
      }
    }

    public int ValueAsInt() => Value == null || ValueType == DataType.Text ? 0 : (int)Value;
    public bool ValueAsBool() => Value == null ? false : ValueType == DataType.Text ? new BooleanString(ValueAsStr()) : ValueType == DataType.Number ? ValueAsInt() != 0 : (bool)Value;
    public double ValueAsDbl() => Value == null ? double.NaN : (double)Value;
    public string ValueAsStr() => Value == null ? string.Empty : Value.ToString();

    public PluginSetting(string id, DataType type = DataType.Text) { SetProperties(id, null, null, type); }
    public PluginSetting(string id, double minValue, double maxValue, string defaultValue = null) { SetProperties(id, null, defaultValue, DataType.Number, minValue, maxValue); }
    public PluginSetting(string id, int maxLength, string defaultValue = null) { SetProperties(id, null, defaultValue, DataType.Text, double.NaN, double.NaN, maxLength); }
    public PluginSetting(string id, string defaultValue, DataType type = DataType.Text) { SetProperties(id, default, defaultValue, type); }
    public PluginSetting(string id, string name, string defaultValue, DataType type = DataType.Text) { SetProperties(id, name, defaultValue, type); }

    private void SetProperties(string id, string name, string defaultValue, DataType type, double min = double.NaN, double max = double.NaN, int maxLen = int.MinValue) {
      SettingID = id;
      ValueType = type;
      Name = name;
      Default = defaultValue;
      MinValue = min;
      MaxValue = max;
      MaxLength = maxLen;
      ValueType = type;   // after min/max
      if (Value == null && defaultValue != null)
        SetValueFromString(defaultValue);
    }
  }
}
