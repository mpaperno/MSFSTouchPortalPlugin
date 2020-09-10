using MSFSTouchPortalPlugin.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Interfaces {
  /// <summary>
  /// Handles communication with the Touch Portal
  /// </summary>
  internal interface IPluginService {
    Task RunPluginServices();
    void TryConnect();
    void SetupEventLists(Dictionary<string, Enum> internalEvents, Dictionary<string, Enum> actionEvents, Dictionary<Definition, SimVarItem> states);
  }
}
