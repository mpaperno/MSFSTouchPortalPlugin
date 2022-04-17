
namespace MSFSTouchPortalPlugin.Types
{
  // This container class is currently used for importing "known" SimConnect events/actions from INI files to present in a UI to the user for selection.
  internal class SimEvent
  {
    /// <summary> Unique ID string. </summary>
    public string Id { get; set; }
    /// <summary> Category for sorting/organizing. </summary>
    public string CategoryId { get; set; }
    /// <summary> Corresponding SimConnect Event name. </summary>
    public string SimEventName { get; set; }
    /// <summary> "Friendly" name for UI. </summary>
    public string Name { get; set; }
    /// <summary> Long Description. </summary>
    public string Description { get; set; }
    /// <summary> "Shared Cockpit" and so on... maybe useful later? </summary>
    public string System { get; set; }
    /// <summary> A preformatted string to use in the TP UI when selecting variables. This is set during import. </summary>
    public string TouchPortalSelectorName { get; set; }
  }
}
