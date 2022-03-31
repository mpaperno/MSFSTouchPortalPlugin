namespace MSFSTouchPortalPlugin.Enums
{
  public enum UpdateFreq // SIMCONNECT_PERIOD
  {
    Never,
    Once,
    VisualFrame,
    SimFrame,
    Second,
    Milliseconds,  // custom frequency

    Default = SimFrame  // default value (do not add anything below here)
  }
}
