using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;

namespace MSFSTouchPortalPlugin.Interfaces {
  internal interface IReflectionService {
    Dictionary<string, Enum> GetInternalEvents();
    Dictionary<string, Enum> GetActionEvents();
    Dictionary<Definition, SimVarItem> GetStates();
    Dictionary<string, PluginSetting> GetSettings();
  }
}
