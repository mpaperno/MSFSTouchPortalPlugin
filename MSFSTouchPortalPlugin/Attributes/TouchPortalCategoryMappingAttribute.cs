using System;

namespace MSFSTouchPortalPlugin.Attributes {
  public class TouchPortalCategoryMappingAttribute : Attribute {
    public string CategoryId;
    public TouchPortalCategoryMappingAttribute(string categoryId) {
      CategoryId = categoryId;
    }
  }
}
