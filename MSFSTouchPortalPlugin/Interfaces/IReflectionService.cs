using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;

namespace MSFSTouchPortalPlugin.Interfaces
{
  internal interface IReflectionService {
    Dictionary<string, ActionEventType> GetActionEvents();
    Dictionary<string, PluginSetting> GetSettings();
    ref readonly Dictionary<Enum, SimEventRecord> GetClientEventIdToNameMap();
    string GetSimEventNameById(Enum id);
    string GetSimEventNameById(uint id);
    string GetSimEventNameById(int id);
    void AddSimEventNameMapping(Enum id, SimEventRecord record);
    IEnumerable<TouchPortalActionAttribute> GetActionAttributes(Groups catId);
    IEnumerable<TouchPortalCategoryAttribute> GetCategoryAttributes();
  }
}
