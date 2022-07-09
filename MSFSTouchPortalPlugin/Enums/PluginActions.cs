namespace MSFSTouchPortalPlugin.Enums
{
  // IDs for handling internal events
  public enum PluginActions : short
  {
    None = 0,

    // Action IDs
    Connection,
    ActionRepeatInterval,

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
    UpdateVarValue,
    RemoveSimVar,
    SaveSimVars,
    LoadSimVars,

    // Action choice mapping IDs
    ToggleConnection,
    Connect,
    Disconnect,
    ReloadStates,
    UpdateHubHopPresets,
    UpdateLocalVarsList,

    ActionRepeatIntervalInc,
    ActionRepeatIntervalDec,
    ActionRepeatIntervalSet,

    SaveCustomSimVars,
    SaveAllSimVars,
  }
}
