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
    public string Id { get; set; }
    public Enum EnumId { get; set; } = default;
    public string Name { get; set; }
    public string Description { get; set; }
    public string Format { get; set; }
    public string ConnectorFormat { get; set; } = null;
    public System.Reflection.MemberInfo ParentObject { get; set; } = null;
    public bool Deprecated { get; set; } = false;  // exclude from generated entry.tp and docs if true, but preserve mappings for backwards compat.
    public TouchPortalActionDataAttribute[] Data { get; set; } = Array.Empty<TouchPortalActionDataAttribute>();
    public TouchPortalActionMappingAttribute[] Mappings { get; set; } = Array.Empty<TouchPortalActionMappingAttribute>();

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
    public string Prefix { get; set; } = "MSFS";
    public string Type { get; set; } = "communicate";
    public bool HasHoldFunctionality { get; set; }

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
    public double DefaultMin { get; set; } = -16384;
    public double DefaultMax { get; set; } = 16384;
    public double MinValue { get; set; } = float.MinValue;
    public double MaxValue { get; set; } = float.MaxValue;
    public int RangeStartIndex { get; set; } = -1;
    public bool AllowDecimals { get; set; } = true;
    public bool UseFeedback { get; set; } = true;

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
    public virtual string Id { get; set; }
    public virtual string Label { get; set; } = "Action";
    public virtual bool AllowDecimals { get; set; } = true;  // this default will prevent inclusion in entry.tp by json generator
    public virtual double MinValue { get; set; } = double.NaN;
    public virtual double MaxValue { get; set; } = double.NaN;
    public virtual string[] ChoiceValues { get; set; }

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

    public TouchPortalActionTextAttribute(string defaultValue, double minValue, double maxValue) : base(DataType.Text) {
      DefaultValue = defaultValue;
      MinValue = minValue;
      MaxValue = maxValue;
      AllowDecimals = false;
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

    public TouchPortalActionChoiceAttribute(string choiceValue, string defaultValue = default) : this(new [] { choiceValue }, defaultValue) { }
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
