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
    SetSimVar,
    AddCustomSimVar,
    AddKnownSimVar,
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
