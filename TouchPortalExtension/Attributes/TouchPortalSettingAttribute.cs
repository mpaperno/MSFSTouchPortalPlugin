using System;

namespace TouchPortalExtension.Attributes
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class TouchPortalSettingAttribute : Attribute
  {
    public string Name { get; set; } = default;
    public string Type { get; set; } = "text";
    public string Default { get; set; } = default;
    public string Description { get; set; }  // for generated docs
    public int MaxLength { get; set; } = int.MinValue;
    public double MinValue { get; set; } = double.NaN;
    public double MaxValue { get; set; } = double.NaN;
    public bool ReadOnly { get; set; } = false;
    public bool IsPassword { get; set; } = false;

    public TouchPortalSettingAttribute() { }
    public TouchPortalSettingAttribute(string name, string description = default, string type = "text", string dflt = default, bool readOnly = false) {
      SetupProperties(name, description, type, dflt, readOnly);
    }

    private void SetupProperties(string name, string descript, string type, string dflt, bool readOnly) {
      Name = name;
      Type = type;
      Description = descript;
      Default = dflt;
      ReadOnly = readOnly;
    }
  }

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class TouchPortalSettingsContainerAttribute : Attribute { }
}
