
namespace MSFSTouchPortalPlugin.Types
{
  // This container class is currently used for importing "known" SimVars/states from INI files to present in a UI to the user for selection.
  internal class SimVariable
  {
    /// <summary> Unique ID string, used to generate TouchPortal state ID (and possibly other uses). </summary>
    public string Id { get; set; }
    /// <summary> Category for sorting/organizing. </summary>
    public string CategoryId { get; set; }
    /// <summary> Corresponding SimConnect SimVar name. </summary>
    public string SimVarName { get; set; }
    /// <summary> SimConnect unit name. </summary>
    public string Unit { get; set; }
    /// <summary> SimConnect settable value </summary>
    public bool CanSet { get; set; } = false;
    /// <summary> SimConnect expects an index value appended to the name </summary>
    public bool Indexed { get; set; }
    /// <summary> "Friendly" name for UI, typically defaults to SimConnect SimVar name. </summary>
    public string Name { get; set; }
    /// <summary> Long Description. </summary>
    public string Description { get; set; }
    /// <summary> A preformatted string to use in the TP UI when selecting variables. </summary>
    public string TouchPortalSelectorName { get; set; }
  }
}
