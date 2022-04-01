using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Attributes
{
  [AttributeUsage(AttributeTargets.All)]
  public class TouchPortalCategoryAttribute : Attribute
  {
    public Groups Id;

    public TouchPortalCategoryAttribute(Groups id) {
      Id = id;
    }
  }
}
