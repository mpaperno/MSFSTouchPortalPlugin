namespace MSFSTouchPortalPlugin.Enums
{
  public enum UpdatePeriod // SIMCONNECT_PERIOD
  {
    Never,
    Once,
    VisualFrame,
    SimFrame,
    Second,
    Millisecond,  // custom frequency

    Default = SimFrame  // default value (do not add anything below here)
  }
}
