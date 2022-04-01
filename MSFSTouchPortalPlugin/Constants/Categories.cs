using MSFSTouchPortalPlugin.Enums;

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

    /// <summary>
    /// Returns the category name for given enum value, or a blank string if the id is invalid..
    /// </summary>
    internal static string CategoryName(Groups catId) {
      if (System.Enum.IsDefined(catId))
        return categoryNames[(int)catId];
      return string.Empty;
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
    /// <param name="catId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
