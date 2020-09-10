using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Attributes {
  [AttributeUsage(AttributeTargets.Enum)]
  internal class SimNotificationGroupAttribute : Attribute {
    public Groups Group;

    public SimNotificationGroupAttribute(Groups group) {
      Group = group;
    }
  }
}
