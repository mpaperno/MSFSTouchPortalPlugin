using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Attributes
{
  [AttributeUsage(AttributeTargets.All)]
  public class TouchPortalCategoryAttribute : Attribute
  {
    public Groups Id { get; set; }
    public string Name { get; set; }
    public string Imagepath { get; set; }

    public TouchPortalActionAttribute[] Actions { get; set; } = Array.Empty<TouchPortalActionAttribute>();
    public object[] States { get; set; } = Array.Empty<object>();
    public object[] Events { get; set; } = Array.Empty<object>();
    public object[] Connectors { get; set; } = Array.Empty<object>();

    public TouchPortalCategoryAttribute(Groups id) {
      Id = id;
      Name = Constants.Categories.CategoryName(id);
      Imagepath = Constants.Categories.CategoryImage(id);
    }
  }
}
