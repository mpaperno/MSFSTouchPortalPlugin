using System;

namespace MSFSTouchPortalPlugin.Attributes {
  internal class TouchPortalCategoryMappingAttribute : Attribute {
    public string CategoryId;
    public TouchPortalCategoryMappingAttribute(string categoryId) {
      CategoryId = categoryId;
    }
  }
}
