using System;
using static MSFSTouchPortalPlugin.SimConnectWrapper;

namespace MSFSTouchPortalPlugin.Attributes {
  [AttributeUsage(AttributeTargets.Enum)]
  public class SimNotificationGroupAttribute : Attribute {
    public Groups Group;

    public SimNotificationGroupAttribute(Groups group) {
      Group = group;
    }
  }
}
