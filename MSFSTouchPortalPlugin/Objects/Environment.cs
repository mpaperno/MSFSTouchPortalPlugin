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

    [TouchPortalAction("AntiIceAdjust", "Anti Ice System Switches", "NOTE: Structural and Propeller De Ice can only be Toggled.", "{0} {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE",         "Anti Ice Switches", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG1",    "Engine 1 Anti Ice", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG2",    "Engine 2 Anti Ice", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG3",    "Engine 3 Anti Ice", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG4",    "Engine 4 Anti Ice", "Toggle")]
    [TouchPortalActionMapping("PITOT_HEAT_TOGGLE",       "Pitot Heat",        "Toggle")]
    [TouchPortalActionMapping("WINDSHIELD_DEICE_TOGGLE", "Windshield De Ice", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_STRUCTURAL_DEICE", "Structural De Ice", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_PROPELLER_DEICE",  "Propeller De Ice",  "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_ON",             "Anti Ice Switches", "On")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1",       "Engine 1 Anti Ice", "On", 1)]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2",       "Engine 2 Anti Ice", "On", 1)]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3",       "Engine 3 Anti Ice", "On", 1)]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4",       "Engine 4 Anti Ice", "On", 1)]
    [TouchPortalActionMapping("PITOT_HEAT_ON",           "Pitot Heat",        "On")]
    [TouchPortalActionMapping("WINDSHIELD_DEICE_ON",     "Windshield De Ice", "On")]
    [TouchPortalActionMapping("ANTI_ICE_OFF",            "Anti Ice Switches", "Off")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1",       "Engine 1 Anti Ice", "Off", 0)]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2",       "Engine 2 Anti Ice", "Off", 0)]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3",       "Engine 3 Anti Ice", "Off", 0)]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4",       "Engine 4 Anti Ice", "Off", 0)]
    [TouchPortalActionMapping("PITOT_HEAT_OFF",          "Pitot Heat",        "Off")]
    [TouchPortalActionMapping("WINDSHIELD_DEICE_OFF",    "Windshield De Ice", "Off")]
    public static readonly object ANTI_ICE_ADJUST;

    [TouchPortalAction("AntiIceSet", "Anti Ice System Set", true,
      "Set {0} to Value {1}",
      "Set {0}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("1", 0, 16384, AllowDecimals = false)]
    [TouchPortalActionMapping("ANTI_ICE_SET",              "Anti Ice Switches (0/1)")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1",         "Engine 1 Anti Ice (0/1)")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2",         "Engine 2 Anti Ice (0/1)")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3",         "Engine 3 Anti Ice (0/1)")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4",         "Engine 4 Anti Ice (0/1)")]
    [TouchPortalActionMapping("ANTI_ICE_GRADUAL_SET",      "All Engines Anti Ice (0 - 16384)")]
    [TouchPortalActionMapping("ANTI_ICE_GRADUAL_SET_ENG1", "Engine 1 Anti Ice (0 - 16384)")]
    [TouchPortalActionMapping("ANTI_ICE_GRADUAL_SET_ENG2", "Engine 2 Anti Ice (0 - 16384)")]
    [TouchPortalActionMapping("ANTI_ICE_GRADUAL_SET_ENG3", "Engine 3 Anti Ice (0 - 16384)")]
    [TouchPortalActionMapping("ANTI_ICE_GRADUAL_SET_ENG4", "Engine 4 Anti Ice (0 - 16384)")]
    [TouchPortalActionMapping("PITOT_HEAT_SET",            "Pitot Heat (0/1)")]
    [TouchPortalActionMapping("WINDSHIELD_DEICE_SET",      "Windshield De Ice (0/1)")]
    public static readonly object ANTI_ICE_SET;


    #region DEPRECATED

    [TouchPortalAction("AntiIce", "Anti-Ice", "Anti Ice - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_ON", "On")]
    [TouchPortalActionMapping("ANTI_ICE_OFF", "Off")]
    public static readonly object AntiIce;

    [TouchPortalAction("AntiIceEng", "Anti-Ice Engine", "Anti Ice Engine {0} Toggle", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG4", "4")]
    public static readonly object AntiIceEng;

    [TouchPortalAction("AntiIceEngSet", "Anti-Ice Engine Set", "Anti Ice Engine {0} - {1}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionSwitch()]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4", "4")]
    public static readonly object ANTI_ICE_ENGINE;

    [TouchPortalAction("StructuralDeIce", "Structural De-ice", "Toggle Structural DeIce", Deprecated = true)]
    [TouchPortalActionMapping("TOGGLE_STRUCTURAL_DEICE")]
    public static readonly object STRUCTURAL_DEICE;

    [TouchPortalAction("PropellerDeIce", "Propeller De-ice", "Toggle Propeller DeIce", Deprecated = true)]
    [TouchPortalActionMapping("TOGGLE_PROPELLER_DEICE")]
    public static readonly object PROPELLER_DEICE;

    [TouchPortalAction("PitotHeat", "Pitot Heat", "Pitot Heat - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("PITOT_HEAT_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PITOT_HEAT_ON", "On")]
    [TouchPortalActionMapping("PITOT_HEAT_OFF", "Off")]
    public static readonly object PITOT_HEAT;

    #endregion
  }
}
