/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/


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
