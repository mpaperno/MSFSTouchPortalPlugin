using System;

namespace MSFSTouchPortalPlugin.Attributes {
  public class TouchPortalActionMappingAttribute : Attribute {
    public string ActionId;
    public string Value;

    public TouchPortalActionMappingAttribute(string actionId, string value = "") {
      ActionId = actionId;
      Value = value;
    }
  }
}
