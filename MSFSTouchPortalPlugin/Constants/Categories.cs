/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

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

using MSFSTouchPortalPlugin.Enums;
using System.Linq;

namespace MSFSTouchPortalPlugin.Constants
{
  internal static class Categories
  {

    internal static string CategoryPrefix { get; set; } = "MSFS";
    internal static string NameSeparator { get; set; } = " - ";
    internal static string CategoryDefaultImage { get; set; } = "airplane_takeoff24.png";


    private static readonly string[] categoryNames = new string[]
    {
      /* None, */               "None",
      /* Plugin, */             "Plugin",
      /* AutoPilot, */          "AutoPilot",
      /* Communication, */      "Communication",
      /* Electrical, */         "Electrical",
      /* Engine, */             "Engine",
      /* Environment, */        "Environment",
      /* Failures, */           "Failures",
      /* FlightInstruments, */  "Flight Instruments",
      /* FlightSystems, */      "Flight Systems",
      /* Fuel, */               "Fuel",
      /* SimSystem, */          "System",
    };

    public static string[] ListAll => categoryNames;
    public static string[] ListUsable => categoryNames[2..^0];  // w/out None and Plugin

    /// <summary>
    /// Returns the category name for given enum value, or a blank string if the id is invalid..
    /// </summary>
    internal static string CategoryName(Groups catId) {
      if (System.Enum.IsDefined(catId))
        return categoryNames[(int)catId];
      return string.Empty;
    }

    /// <summary>
    /// Returns the category ID for given name, or Groups.None if the string is invalid..
    /// </summary>
    internal static Groups CategoryId(string name) =>
      categoryNames.ToList().IndexOf(name) is var idx && idx > -1 ? (Groups)idx : Groups.None;

    /// <summary>
    /// Places the category ID for given name in the out parameter, and returns true if string was valid.
    /// </summary>
    internal static bool TryGetCategoryId(string name, out Groups id) {
      if (categoryNames.ToList().IndexOf(name) is var idx && idx > -1) {
        id = (Groups)idx;
        return true;
      }
      id = Groups.None;
      return false;
    }

    /// <summary>
    /// Returns value with the category name prepended to it using the common separator string.
    /// </summary>
    internal static string PrependCategoryName(Groups catId, string value) {
      return CategoryName(catId) + NameSeparator + value;
    }

    /// <summary>
    /// Returns category name with the CategoryPrefix added. Or just the CategoryPrefix if the category id is invalid.
    /// </summary>
    internal static string FullCategoryName(Groups catId) {
      if (CategoryName(catId) is var name && !string.IsNullOrEmpty(name))
        return CategoryPrefix + NameSeparator + name;
      return CategoryPrefix;
    }

    /// <summary>
    /// Returns value with the full category name prepended to it using the common separator string.
    /// </summary>
    internal static string PrependFullCategoryName(Groups catId, string value) {
      return FullCategoryName(catId) + NameSeparator + value;
    }

    /// <summary>
    /// Workaround for legacy issue with mis-named actions in category InstrumentsSystems.Fuel instead of just Fuel.  TODO
    /// </summary>
    internal static string ActionCategoryId(Groups catId) {
      string ret = catId.ToString();
      if (catId == Groups.Fuel)
        return "InstrumentsSystems." + ret;
      return ret;
    }

    internal static string CategoryImage(Groups catId) {
      _ = catId;
      // just the one, for now?
      return CategoryDefaultImage;
    }

  }
}
