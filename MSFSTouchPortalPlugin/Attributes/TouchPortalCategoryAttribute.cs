using System;

namespace MSFSTouchPortalPlugin.Attributes {
  class TouchPortalCategoryAttribute : Attribute {
    public string Id;
    public string Name;
    public string ImagePath;

    public TouchPortalCategoryAttribute(string id, string name, string imagePath = "") {
      Id = id;
      Name = name;
      ImagePath = imagePath;
    }
  }
}
