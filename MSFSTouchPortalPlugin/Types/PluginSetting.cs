using System;
using TouchPortalExtension.Enums;

namespace MSFSTouchPortalPlugin.Types
{
  public class PluginSetting
  {
    public string SettingID { get; set; }
    public string Name { get; set; } = null;
    public string Default { get; set; } = null;
    public DataType ValueType { get; set; } = DataType.Text;
    public int MaxLength { get; set; } = int.MinValue;
    public double MinValue { get; set; } = double.NaN;
    public double MaxValue { get; set; } = double.NaN;
    public string TouchPortalStateId { get; set; } = null;

    private dynamic _value = null;
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

    public void SetValueFromString(string value) {
      if (ValueType == DataType.Number) {
        if (double.TryParse(value, out var numVal))
          Value = numVal;
      }
      else {
        Value = value;
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
        // string
        else {
          string strVal = Convert.ToString(value);
          if (MaxLength > 0 && !string.IsNullOrEmpty(strVal))
            strVal = strVal[..Math.Min(strVal.Length, MaxLength)];
          _value = strVal;
        }
      }
      catch (Exception e) {
        throw new ArgumentException("Cannot convert value to intended type.", e);
      }
    }

    public int ValueAsInt() => Value == null || ValueType != DataType.Number ? 0 : (int)Value;
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
      ValueType = type;
      MinValue = min;
      MaxValue = max;
      MaxLength = maxLen;
      if (Value == null && defaultValue != null)
        SetValueFromString(defaultValue);
    }
  }
}
