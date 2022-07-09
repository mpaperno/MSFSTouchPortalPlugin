namespace MSFSTouchPortalPlugin.Enums
{
  // IDs for handling internal events
  public enum PluginActions : short
  {
    None = 0,

    // Action IDs
    Connection,
    ActionRepeatInterval,
    UpdateHubHopPresets,

    SetCustomSimEvent,
    SetKnownSimEvent,
    SetHubHopEvent,
    SetSimVar,
    SetVariable,
    ExecCalcCode,

    AddCustomSimVar,
    AddKnownSimVar,
    AddNamedVariable,
    AddCalculatedValue,
    RemoveSimVar,
    SaveSimVars,
    LoadSimVars,

    // Action choice mapping IDs
    ToggleConnection,
    Connect,
    Disconnect,
    ReloadStates,

    ActionRepeatIntervalInc,
    ActionRepeatIntervalDec,
    ActionRepeatIntervalSet,

    SaveCustomSimVars,
    SaveAllSimVars,
  }
}
