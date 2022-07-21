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

namespace MSFSTouchPortalPlugin.Objects.Failures
{
  [TouchPortalCategory(Groups.Failures)]
  internal static class FailuresMapping {

    [TouchPortalAction("Failures", "Failures", "Toggle Failure - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_ELECTRICAL_FAILURE", "Electrical")]
    [TouchPortalActionMapping("TOGGLE_VACUUM_FAILURE", "Vacuum")]
    [TouchPortalActionMapping("TOGGLE_PITOT_BLOCKAGE", "Pitot")]
    [TouchPortalActionMapping("TOGGLE_STATIC_PORT_BLOCKAGE", "Static Port")]
    [TouchPortalActionMapping("TOGGLE_HYDRAULIC_FAILURE", "Hydraulic")]
    [TouchPortalActionMapping("TOGGLE_TOTAL_BRAKE_FAILURE", "Total Brake")]
    [TouchPortalActionMapping("TOGGLE_LEFT_BRAKE_FAILURE", "Left Brake")]
    [TouchPortalActionMapping("TOGGLE_RIGHT_BRAKE_FAILURE", "Right Brake")]
    [TouchPortalActionMapping("TOGGLE_ENGINE1_FAILURE", "Engine 1")]
    [TouchPortalActionMapping("TOGGLE_ENGINE2_FAILURE", "Engine 2")]
    [TouchPortalActionMapping("TOGGLE_ENGINE3_FAILURE", "Engine 3")]
    [TouchPortalActionMapping("TOGGLE_ENGINE4_FAILURE", "Engine 4")]
    public static readonly object FAILURES;
  }
}
