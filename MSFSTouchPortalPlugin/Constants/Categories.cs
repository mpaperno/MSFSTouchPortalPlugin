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
using System.Collections.Generic;

namespace MSFSTouchPortalPlugin.Enums
{
  public enum Groups
  {
    None = 0,
    Plugin,
    StatesEditor,
    // categories for custom states
    AutoPilot,
    Camera,
    Communication,
    Electrical,
    Engine,
    Environment,
    Failures,
    FlightInstruments,
    FlightSystems,
    Fuel,
    Miscellaneous,
    SimSystem,
  }
}

namespace MSFSTouchPortalPlugin.Constants
{
  internal static class Categories
  {

    internal static string CategoryPrefix { get; set; } = "MSFS";
    internal static string NameSeparator { get; set; } = " - ";
    internal static string CategoryDefaultImage { get; set; } = "airplane_takeoff24.png";


    private static readonly List<string> categoryNames = new()
    {
      /* None, */               "None",
      /* Plugin, */             "Plugin",
      /* StatesEditor, */       "Custom States & Variables",
      /* AutoPilot, */          "AutoPilot",
      /* Camera, */             "Camera & Views",
      /* Communication, */      "Radio & Navigation",
      /* Electrical, */         "Electrical",
      /* Engine, */             "Engine",
      /* Environment, */        "Environment",
      /* Failures, */           "Failures",
      /* FlightInstruments, */  "Flight Instruments",
      /* FlightSystems, */      "Flight Systems",
      /* Fuel, */               "Fuel",
      /* Miscellaneous */       "Miscellaneous",
      /* SimSystem, */          "Simulator System",
    };

    private static readonly Dictionary<string, Groups> nameIdMap = new()
    {
      { categoryNames[(int)Groups.None],              Groups.None },
      { categoryNames[(int)Groups.Plugin],            Groups.Plugin },
      { categoryNames[(int)Groups.StatesEditor],      Groups.StatesEditor },
      { categoryNames[(int)Groups.AutoPilot],         Groups.AutoPilot },
      { categoryNames[(int)Groups.Camera],            Groups.Camera },
      { categoryNames[(int)Groups.Communication],     Groups.Communication },
      { "Communication",                              Groups.Communication },  // legacy
      { categoryNames[(int)Groups.Electrical],        Groups.Electrical },
      { categoryNames[(int)Groups.Engine],            Groups.Engine },
      { categoryNames[(int)Groups.Environment],       Groups.Environment },
      { categoryNames[(int)Groups.Failures],          Groups.Failures },
      { categoryNames[(int)Groups.FlightInstruments], Groups.FlightInstruments },
      { categoryNames[(int)Groups.FlightSystems],     Groups.FlightSystems },
      { categoryNames[(int)Groups.Fuel],              Groups.Fuel },
      { categoryNames[(int)Groups.Miscellaneous],     Groups.Miscellaneous },
      { categoryNames[(int)Groups.SimSystem],         Groups.SimSystem },
      { "System",                                     Groups.SimSystem },  // legacy
    };

    private static readonly List<string> usableCategoryNames = categoryNames.GetRange((int)Groups.AutoPilot, categoryNames.Count - (int)Groups.AutoPilot);

    public static IReadOnlyCollection<string> ListAll => categoryNames;
    public static IReadOnlyCollection<string> ListUsable => usableCategoryNames;  // w/out None and Plugin
    public static readonly List<Groups> InternalActionCategories = new() { Groups.Plugin, Groups.StatesEditor };

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
    internal static Groups CategoryId(string name) => nameIdMap.GetValueOrDefault(name, Groups.None);

    /// <summary>
    /// Places the category ID for given name in the out parameter, and returns true if string was valid.
    /// </summary>
    internal static bool TryGetCategoryId(string name, out Groups id) {
      if (nameIdMap.TryGetValue(name, out id))
        return true;
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
    /// Maps category IDs to strings, possibly aliased for public entry.tp usage.
    /// Also workaround for legacy issue with mis-named actions in category InstrumentsSystems.Fuel instead of just Fuel.
    /// </summary>
    internal static string ActionCategoryId(Groups catId) {
      if (catId == Groups.StatesEditor)
        return Groups.Plugin.ToString();
      if (catId == Groups.Fuel)
        return "InstrumentsSystems." + catId.ToString();
      return catId.ToString();
    }

    internal static string CategoryImage(Groups catId) {
      _ = catId;
      // just the one, for now?
      return CategoryDefaultImage;
    }

  }
}
