using System;

namespace TouchPortalExtension.Attributes {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
  public class TouchPortalCategoryAttribute : Attribute {
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
