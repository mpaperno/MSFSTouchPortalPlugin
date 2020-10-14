using System;

namespace MSFSTouchPortalPlugin.Attributes {
  internal class TouchPortalActionMappingAttribute : Attribute {
    public string ActionId;
    public string[] Values;

    public TouchPortalActionMappingAttribute(string actionId, string value) {
      ActionId = actionId;
      Values = new [] { value };
    }

    public TouchPortalActionMappingAttribute(string actionId, string[] values = null) {
      ActionId = actionId;
      Values = values ?? new string [] { };
    }
  }
}
