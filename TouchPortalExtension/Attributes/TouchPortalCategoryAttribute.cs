using System;

namespace TouchPortalExtension.Attributes {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
  public class TouchPortalCategoryAttribute : Attribute {
    public string Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }

    public TouchPortalCategoryAttribute(string id, string name) {
      SetupProperties(id, name, string.Empty);
    }

    private void SetupProperties(string id, string name, string imagePath) {
      Id = id;
      Name = name;
      ImagePath = imagePath;
    }
  }
}
