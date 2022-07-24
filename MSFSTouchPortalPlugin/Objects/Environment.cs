/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
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

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.Environment)]
  internal static class EnvironmentMapping
  {
    #region Anti-Ice

    [TouchPortalAction("AntiIce", "Anti-Ice", "Anti Ice - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_ON", "On")]
    [TouchPortalActionMapping("ANTI_ICE_OFF", "Off")]
    public static readonly object AntiIce;

    [TouchPortalAction("AntiIceEng", "Anti-Ice Engine", "Anti Ice Engine {0} Toggle")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG4", "4")]
    public static readonly object AntiIceEng;

    [TouchPortalAction("AntiIceEngSet", "Anti-Ice Engine Set", "Anti Ice Engine {0} - {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionSwitch()]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4", "4")]
    public static readonly object ANTI_ICE_ENGINE;

    [TouchPortalAction("StructuralDeIce", "Structural De-ice", "Toggle Structural DeIce")]
    [TouchPortalActionMapping("TOGGLE_STRUCTURAL_DEICE")]
    public static readonly object STRUCTURAL_DEICE;

    [TouchPortalAction("PropellerDeIce", "Propeller De-ice", "Toggle Propeller DeIce")]
    [TouchPortalActionMapping("TOGGLE_PROPELLER_DEICE")]
    public static readonly object PROPELLER_DEICE;

    #endregion

    #region Pitot Heat

    [TouchPortalAction("PitotHeat", "Pitot Heat", "Pitot Heat - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("PITOT_HEAT_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PITOT_HEAT_ON", "On")]
    [TouchPortalActionMapping("PITOT_HEAT_OFF", "Off")]
    public static readonly object PITOT_HEAT;

    #endregion
  }
}
