using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;

namespace MSFSTouchPortalPlugin.Interfaces
{
  internal interface IReflectionService {
    Dictionary<string, ActionEventType> GetActionEvents();
    Dictionary<string, PluginSetting> GetSettings();
    ref readonly Dictionary<Enum, dynamic> GetClientEventIdToNameMap();
    string GetSimEventNameById(Enum id);
    string GetSimEventNameById(uint id);
    string GetSimEventNameById(int id);
  }
}
