/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Attributes
{

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalActionBaseAttribute : Attribute
  {
    public string Id;
    public Enum EnumId = default;
    public string Name;
    public string Description;
    public string Format;
    public string ConnectorFormat = null;
    public System.Reflection.MemberInfo ParentObject = null;
    public bool LayoutAsForm = false;   // place all action data fields on separate lines in a vertical form-style layout (TP v4+)
    public int FormLabelWidth = 150;    // how wide the label column should be in form layouts
    public bool Deprecated = false;     // exclude from generated entry.tp and docs if true, but preserve mappings for backwards compat.
    public int UserValueIndex = 0;      // index at which to insert user-provided value(s) in relation to any static values; -1 means append to end.
    public TouchPortalActionDataAttribute[] Data = Array.Empty<TouchPortalActionDataAttribute>();
    public TouchPortalActionMappingAttribute[] Mappings = Array.Empty<TouchPortalActionMappingAttribute>();

    public TouchPortalActionBaseAttribute(string id, string name, string description, string format, string connectorFormat = null)
    {
      SetupProperties(id, default, name, description, format, connectorFormat);
    }

    public TouchPortalActionBaseAttribute(PluginActions id, string name, string description, string format, string connectorFormat = null)
    {
      SetupProperties(id.ToString(), id, name, description, format, connectorFormat);
    }

    private void SetupProperties(string id, Enum eId, string name, string description, string format, string connectorFormat = null)
    {
      Id = id;
      EnumId = eId;
      Name = name;
      Description = description;
      Format = format;
      ConnectorFormat = connectorFormat;
    }
  }

  public class TouchPortalActionAttribute : TouchPortalActionBaseAttribute
  {
    public string Prefix = Configuration.PluginConfig.PLUGIN_NAME_PREFIX;
    public string Type = "communicate";
    public bool HasHoldFunctionality = false;
    public string OnHoldFormat = null;

    public TouchPortalActionAttribute(string id, string name, string format, bool holdable = false) :
      base(id, name, string.Empty, format)
    {
      HasHoldFunctionality = holdable;
    }

    public TouchPortalActionAttribute(string id, string name, string description, string format, bool holdable = false) :
      base(id, name, description, format)
    {
      HasHoldFunctionality = holdable;
    }

    public TouchPortalActionAttribute(string id, string name, bool holdable, string format, string connectorFormat) :
      base(id, name, string.Empty, format, connectorFormat)
    {
      HasHoldFunctionality = holdable;
    }

    public TouchPortalActionAttribute(string id, string name, string description, bool holdable, string format, string connectorFormat) :
      base(id, name, description, format, connectorFormat)
    {
      HasHoldFunctionality = holdable;
    }

    public TouchPortalActionAttribute(PluginActions id, string name, string format, bool holdable = false) :
      base(id, name, string.Empty, format)
    {
      HasHoldFunctionality = holdable;
    }

    public TouchPortalActionAttribute(PluginActions id, string name, string description, string format, bool holdable = false) :
      base(id, name, description, format)
    {
      HasHoldFunctionality = holdable;
    }

    public TouchPortalActionAttribute(PluginActions id, string name, string description, string format, string connectorFormat, bool holdable) :
      base(id, name, description, format, connectorFormat)
    {
      HasHoldFunctionality = holdable;
    }

  }

  public class TouchPortalConnectorAttribute : TouchPortalActionBaseAttribute
  {
    public TouchPortalConnectorAttribute(PluginActions id, string name, string description, string format) :
      base(id, name, description, format) { }
    public TouchPortalConnectorAttribute(string id, string name, string description, string format) :
      base(id, name, description, format)
    { }
  }

  // -----------------------------------
  // TouchPortalConnectorMetaAttribute
  // -----------------------------------

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalConnectorMetaAttribute : Attribute
  {
    public double DefaultMin = -16384;
    public double DefaultMax = 16384;
    public double MinValue = float.MinValue;
    public double MaxValue = float.MaxValue;
    public int RangeStartIndex = -1;
    public bool AllowDecimals = true;
    public bool UseFeedback = true;
    public bool InsertValueRange = true;

    public TouchPortalConnectorMetaAttribute() { }
    public TouchPortalConnectorMetaAttribute(bool decimals, bool feedback = true)
    {
      AllowDecimals = decimals;
      UseFeedback = feedback;
    }
    public TouchPortalConnectorMetaAttribute(double defaultMin, double defaultMax, bool decimals = true, bool feedback = true)
    {
      DefaultMin = defaultMin;   DefaultMax = defaultMax;
      AllowDecimals = decimals;  UseFeedback = feedback;
    }
    public TouchPortalConnectorMetaAttribute(double defaultMin, double defaultMax, double minValue, double maxValue, bool decimals = true, bool feedback = true)
    {
      DefaultMin = defaultMin;  DefaultMax = defaultMax;
      MinValue = minValue;      MaxValue = maxValue;
      AllowDecimals = decimals; UseFeedback = feedback;
    }
  }

  // -----------------------------------
  // Data Attributes
  // -----------------------------------

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public class TouchPortalActionDataAttribute : Attribute
  {
    //public string Id { get; set; }
    public DataType ValueType { get; set; }
    public string Type => ValueType.ToString().ToLower();
    public string Id;
    public string Label = "Action";    // the labels are used for formatting action data fields on individual lines in TP v4+
    public string LabelSuffix = null;
    public bool AllowDecimals = true;  // this default will prevent inclusion in entry.tp by json generator
    public double MinValue = double.NaN;
    public double MaxValue = double.NaN;
    public string[] ChoiceValues;
    public bool UsedInMapping = true;  // for Choice type meta data creation
    public bool SkipForValIndex = false;  // not an action's main value field (used in injected on-hold data fields)

    protected dynamic _defaultValue;

    public dynamic GetDefaultValue() {
      try {
        return ValueType switch {
          DataType.Number => Convert.ToDouble(_defaultValue),
          DataType.Switch => Convert.ToBoolean(_defaultValue),
          _               => Convert.ToString(_defaultValue),
        };
      }
      catch {
        return ValueType switch {
          DataType.Number => 0.0,
          DataType.Switch => false,
          _               => string.Empty,
        };
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

    public TouchPortalActionTextAttribute(string defaultValue, double minValue, double maxValue, bool allowDecimals = false) : base(DataType.Text) {
      DefaultValue = defaultValue;
      MinValue = minValue;
      MaxValue = maxValue;
      AllowDecimals = allowDecimals;
    }

    protected TouchPortalActionTextAttribute(DataType type, string defaultValue = "") : this(defaultValue) {
      ValueType = type;
    }
  }

  public class TouchPortalActionFileAttribute : TouchPortalActionTextAttribute
  {
    public TouchPortalActionFileAttribute(string defaultValue = default) : base(DataType.File, defaultValue) { }
  }

  public class TouchPortalActionFolderAttribute : TouchPortalActionTextAttribute
  {
    public TouchPortalActionFolderAttribute(string defaultValue = default) : base(DataType.Folder, defaultValue) { }
  }

  public class TouchPortalActionColorAttribute : TouchPortalActionTextAttribute
  {
    public TouchPortalActionColorAttribute(string defaultValue = default) : base(DataType.Color, defaultValue) { }
  }

  public class TouchPortalActionChoiceAttribute : TouchPortalActionTextAttribute
  {
    public TouchPortalActionChoiceAttribute() : base(DataType.Choice) {
      ChoiceValues = Array.Empty<string>();
    }

    public TouchPortalActionChoiceAttribute(string[] choiceValues, string defaultValue = default) : base(DataType.Choice, defaultValue) {
      ChoiceValues = choiceValues;
      if (defaultValue == default && choiceValues?.Length > 0)
        DefaultValue = choiceValues[0];
    }

    public TouchPortalActionChoiceAttribute(string[] choiceValues, int defaultValue) : this(choiceValues) {
      if (defaultValue > -1 && defaultValue < choiceValues.Length)
        DefaultValue = choiceValues[defaultValue];
    }

    public TouchPortalActionChoiceAttribute(string choiceValue, string defaultValue = default) : this(new [] { choiceValue }, defaultValue) { }
  }

  public class TouchPortalActionSwitchAttribute : TouchPortalActionChoiceAttribute
  {
    public TouchPortalActionSwitchAttribute(bool defaultValue = false) : base(new [] { "On", "Off" }, defaultValue ? 0 : 1) {
      UsedInMapping = false;
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
