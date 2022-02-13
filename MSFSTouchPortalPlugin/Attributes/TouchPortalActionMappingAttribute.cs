using System;

namespace MSFSTouchPortalPlugin.Attributes
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  internal class TouchPortalActionMappingAttribute : Attribute {
    public string ActionId;
    public string[] Values;

    public TouchPortalActionMappingAttribute(string actionId, string value) :
      this(actionId, new [] { value }) { }
    public TouchPortalActionMappingAttribute(string actionId, string value1, string value2) :
      this(actionId, new[] { value1, value2 }) { }
    public TouchPortalActionMappingAttribute(string actionId, string value1, string value2, string value3) :
      this(actionId, new[] { value1, value2, value3 }) { }
    public TouchPortalActionMappingAttribute(string actionId, string[] values = null) {
      ActionId = actionId;
      Values = values ?? Array.Empty<string>();
    }
  }
}
