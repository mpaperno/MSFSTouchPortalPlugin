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

using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Tests")]
namespace MSFSTouchPortalPlugin.Constants {
  internal static class Units {

    private static readonly string[] _integralUnits = new string[] {
      "enum", "mask", "flags", "integer",
      "position", "position 16k", "position 32k", "position 128",
      "frequency bcd16", "frequency bcd32", "bco16", "bcd16", "bcd32",
      "seconds", "minutes", "hours", "days", "years",
      "celsius scaler 16k", "celsius scaler 256",
      //"degree angl16", "degree angl32" not used
    };

    private static readonly string[] _booleanUnits = new string[] { "bool", "boolean" };

    /// <summary>
    /// Returns true if the unit string corresponds to a string type.
    /// </summary>
    internal static bool IsStringType(string unit) => unit.ToLower() == "string";
    /// <summary>
    /// Returns true if the unit string corresponds to an integer type.
    /// </summary>
    internal static bool IsIntegralType(string unit) => _integralUnits.Contains(unit.ToLower());
    /// <summary>
    /// Returns true if the unit string corresponds to a boolean type.
    /// </summary>
    internal static bool IsBooleanType(string unit) => _booleanUnits.Contains(unit.ToLower());
    /// <summary>
    /// Returns true if the unit string corresponds to a real (float/double) type.
    /// </summary>
    internal static bool IsRealType(string unit) => !IsStringType(unit) && !IsBooleanType(unit) && !IsIntegralType(unit);

  }
}
