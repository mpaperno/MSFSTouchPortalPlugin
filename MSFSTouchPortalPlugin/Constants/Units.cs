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
      Enum, mask, flags, integer, position16k, position32k, position128, Bco16, second, seconds, minute, minutes, hour, hours, day, days, hourover10, hoursover10, year, years
    };

    private static readonly string[] _booleanUnits = new string[] { Bool, Boolean };

    /// <summary>
    /// Returns true if the unit string corresponds to a string type.
    /// </summary>
    internal static bool IsStringType(string unit) => unit != null_unit && unit.ToLower() == String;
    /// <summary>
    /// Returns true if the unit string corresponds to an integer type.
    /// </summary>
    internal static bool IsIntegralType(string unit) => unit != null_unit && _integralUnits.Contains(unit);
    /// <summary>
    /// Returns true if the unit string corresponds to a boolean type.
    /// </summary>
    internal static bool IsBooleanType(string unit) => unit != null_unit && _booleanUnits.Contains(unit);
    /// <summary>
    /// Returns true if the unit string corresponds to a real (float/double) type.
    /// </summary>
    internal static bool IsRealType(string unit) => unit != null_unit && !IsStringType(unit) && !IsBooleanType(unit) && !IsIntegralType(unit);

    internal static string NormalizedUnit(string unit) {
      if (ListAll.FirstOrDefault(u => u.ToLower() == unit.ToLower()) is var result && result != null)
        return result;
      return null;
    }

    #region Unit Names

    internal const string null_unit = "";
    internal const string integer = "integer";  // not std unit, like "number" but for ints

    internal const string amp = "amp";
    internal const string ampere = "ampere";
    internal const string amperes = "amperes";
    internal const string amps = "amps";
    internal const string angl16 = "angl16";
    internal const string angl32 = "angl32";
    internal const string atm = "atm";
    internal const string atmosphere = "atmosphere";
    internal const string atmospheres = "atmospheres";
    internal const string bar = "bar";
    internal const string bars = "bars";
    internal const string Bco16 = "Bco16";
    internal const string bel = "bel";
    internal const string bels = "bels";
    internal const string Bool = "Bool";
    internal const string Boolean = "Boolean";
    internal const string boostcmHg = "boost cmHg";
    internal const string boostinHg = "boost inHg";
    internal const string boostpsi = "boost psi";
    internal const string celsius = "celsius";
    internal const string celsiusfs7egt = "celsius fs7 egt";
    internal const string celsiusfs7oiltemp = "celsius fs7 oil temp";
    internal const string celsiusscaler1256 = "celsius scaler 1/256";
    internal const string celsiusscaler16k = "celsius scaler 16k";
    internal const string celsiusscaler256 = "celsius scaler 256";
    internal const string centimeter = "centimeter";
    internal const string centimeterofmercury = "centimeter of mercury";
    internal const string centimeters = "centimeters";
    internal const string centimetersofmercury = "centimeters of mercury";
    internal const string cm = "cm";
    internal const string cm2 = "cm2";
    internal const string cm3 = "cm3";
    internal const string cmHg = "cmHg";
    internal const string cubiccentimeter = "cubic centimeter";
    internal const string cubiccentimeters = "cubic centimeters";
    internal const string cubicfeet = "cubic feet";
    internal const string cubicfoot = "cubic foot";
    internal const string cubicinch = "cubic inch";
    internal const string cubicinches = "cubic inches";
    internal const string cubickilometer = "cubic kilometer";
    internal const string cubickilometers = "cubic kilometers";
    internal const string cubicmeter = "cubic meter";
    internal const string cubicmeters = "cubic meters";
    internal const string cubicmile = "cubic mile";
    internal const string cubicmiles = "cubic miles";
    internal const string cubicmillimeter = "cubic millimeter";
    internal const string cubicmillimeters = "cubic millimeters";
    internal const string cubicyard = "cubic yard";
    internal const string cubicyards = "cubic yards";
    internal const string cucm = "cu cm";
    internal const string cuft = "cu ft";
    internal const string cuin = "cu in";
    internal const string cukm = "cu km";
    internal const string cum = "cu m";
    internal const string cumm = "cu mm";
    internal const string cuyd = "cu yd";
    internal const string day = "day";
    internal const string days = "days";
    internal const string decibel = "decibel";
    internal const string decibels = "decibels";
    internal const string decimile = "decimile";
    internal const string decimiles = "decimiles";
    internal const string decinmile = "decinmile";
    internal const string decinmiles = "decinmiles";
    internal const string degree = "degree";
    internal const string degreeangl16 = "degree angl16";
    internal const string degreeangl32 = "degree angl32";
    internal const string degreelatitude = "degree latitude";
    internal const string degreelongitude = "degree longitude";
    internal const string degreepersecond = "degree per second";
    internal const string degreepersecondang16 = "degree per second ang16";
    internal const string degrees = "degrees";
    internal const string degreesangl16 = "degrees angl16";
    internal const string degreesangl32 = "degrees angl32";
    internal const string degreeslatitude = "degrees latitude";
    internal const string degreeslongitude = "degrees longitude";
    internal const string degreespersecond = "degrees per second";
    internal const string degreespersecondang16 = "degrees per second ang16";
    internal const string Enum = "Enum";
    internal const string fahrenheit = "fahrenheit";
    internal const string farenheit = "farenheit";
    internal const string feet = "feet";
    internal const string feetminute = "feet/minute";
    internal const string feetperminute = "feet per minute";
    internal const string feetpersecond = "feet per second";
    internal const string feetpersecondsquared = "feet per second squared";
    internal const string feetsecond = "feet/second";
    internal const string flags = "flags";
    internal const string foot = "foot";
    internal const string footpersecondsquared = "foot per second squared";
    internal const string footpound = "foot pound";
    internal const string footpounds = "foot pounds";
    internal const string FrequencyADFBCD32 = "Frequency ADF BCD32";
    internal const string FrequencyBCD16 = "Frequency BCD16";
    internal const string FrequencyBCD32 = "Frequency BCD32";
    internal const string fs7chargingamps = "fs7 charging amps";
    internal const string fs7oilquantity = "fs7 oil quantity";
    internal const string ft = "ft";
    internal const string ft2 = "ft2";
    internal const string ft3 = "ft3";
    internal const string ftlbpersecond = "ft lb per second";
    internal const string ftlbs = "ft-lbs";
    internal const string ftmin = "ft/min";
    internal const string gallon = "gallon";
    internal const string gallonperhour = "gallon per hour";
    internal const string gallons = "gallons";
    internal const string gallonsperhour = "gallons per hour";
    internal const string geepound = "geepound";
    internal const string geepounds = "geepounds";
    internal const string GForce = "GForce";
    internal const string GForce624scaled = "G Force 624 scaled";
    internal const string GLOBALPdeltaheadingrate = "GLOBALP->delta_heading_rate";
    internal const string GLOBALPeng1manifoldpressure = "GLOBALP->eng1.manifold_pressure";
    internal const string GLOBALPeng1oilprs = "GLOBALP->eng1.oil_prs";
    internal const string GLOBALPeng1oiltmp = "GLOBALP->eng1.oil_tmp";
    internal const string GLOBALPverticalspeed = "GLOBALP->vertical_speed";
    internal const string gph = "gph";
    internal const string grad = "grad";
    internal const string grads = "grads";
    internal const string half = "half";
    internal const string halfs = "halfs";
    internal const string hectopascal = "hectopascal";
    internal const string hectopascals = "hectopascals";
    internal const string Hertz = "Hertz";
    internal const string hour = "hour";
    internal const string hourover10 = "hour over 10";
    internal const string hours = "hours";
    internal const string hoursover10 = "hours over 10";
    internal const string Hz = "Hz";
    internal const string inch = "in";
    internal const string in2 = "in2";
    internal const string in3 = "in3";
    internal const string inches = "inches";
    internal const string inchesofmercury = "inches of mercury";
    internal const string inchofmercury = "inch of mercury";
    internal const string inHg = "inHg";
    internal const string inHg64over64k = "inHg 64 over 64k";
    internal const string kelvin = "kelvin";
    internal const string keyframe = "keyframe";
    internal const string keyframes = "keyframes";
    internal const string kg = "kg";
    internal const string kgfmeter = "kgf meter";
    internal const string kgfmeters = "kgf meters";
    internal const string KgFSqCm = "KgFSqCm";
    internal const string KHz = "KHz";
    internal const string kilogram = "kilogram";
    internal const string kilogramforcepersquarecentimeter = "kilogram force per square centimeter";
    internal const string kilogrammeter = "kilogram meter";
    internal const string kilogrammeters = "kilogram meters";
    internal const string kilogrammetersquared = "kilogram meter squared";
    internal const string kilogrampercubicmeter = "kilogram per cubic meter";
    internal const string kilogrampersecond = "kilogram per second";
    internal const string kilograms = "kilograms";
    internal const string kilogramsmetersquared = "kilograms meter squared";
    internal const string kilogramspercubicmeter = "kilograms per cubic meter";
    internal const string kilogramspersecond = "kilograms per second";
    internal const string Kilohertz = "Kilohertz";
    internal const string kilometer = "kilometer";
    internal const string kilometerhour = "kilometer/hour";
    internal const string kilometerperhour = "kilometer per hour";
    internal const string kilometers = "kilometers";
    internal const string kilometershour = "kilometers/hour";
    internal const string kilometersperhour = "kilometers per hour";
    internal const string kilopascal = "kilopascal";
    internal const string km = "km";
    internal const string km2 = "km2";
    internal const string km3 = "km3";
    internal const string knot = "knot";
    internal const string knots = "knots";
    internal const string knotscaler128 = "knot scaler 128";
    internal const string knotsscaler128 = "knots scaler 128";
    internal const string kPa = "kPa";
    internal const string kph = "kph";
    internal const string lbffeet = "lbf-feet";
    internal const string lbs = "lbs";
    internal const string liter = "liter";
    internal const string literperhour = "liter per hour";
    internal const string liters = "liters";
    internal const string litersperhour = "liters per hour";
    internal const string m = "m";
    internal const string m2 = "m2";
    internal const string m3 = "m3";
    internal const string mach = "mach";
    internal const string mach3d2over64k = "mach 3d2 over 64k";
    internal const string machs = "machs";
    internal const string mask = "mask";
    internal const string mbar = "mbar";
    internal const string mbars = "mbars";
    internal const string Megahertz = "Megahertz";
    internal const string meter = "meter";
    internal const string metercubed = "meter cubed";
    internal const string metercubedpersecond = "meter cubed per second";
    internal const string meterlatitude = "meter latitude";
    internal const string meterperminute = "meter per minute";
    internal const string meterpersecond = "meter per second";
    internal const string meterpersecondscaler256 = "meter per second scaler 256";
    internal const string meterpersecondsquared = "meter per second squared";
    internal const string meters = "meters";
    internal const string meterscaler256 = "meter scaler 256";
    internal const string meterscubed = "meters cubed";
    internal const string meterscubedpersecond = "meters cubed per second";
    internal const string metersecond = "meter/second";
    internal const string meterslatitude = "meters latitude";
    internal const string metersperminute = "meters per minute";
    internal const string meterspersecond = "meters per second";
    internal const string meterspersecondscaler256 = "meters per second scaler 256";
    internal const string meterspersecondsquared = "meters per second squared";
    internal const string metersscaler256 = "meters scaler 256";
    internal const string meterssecond = "meters/second";
    internal const string MHz = "MHz";
    internal const string mile = "mile";
    internal const string mileperhour = "mile per hour";
    internal const string miles = "miles";
    internal const string milesperhour = "miles per hour";
    internal const string millibar = "millibar";
    internal const string millibars = "millibars";
    internal const string millibarscaler16 = "millibar scaler 16";
    internal const string millibarsscaler16 = "millibars scaler 16";
    internal const string millimeter = "millimeter";
    internal const string millimeterofmercury = "millimeter of mercury";
    internal const string millimeterofwater = "millimeter of water";
    internal const string millimeters = "millimeters";
    internal const string millimetersofmercury = "millimeters of mercury";
    internal const string millimetersofwater = "millimeters of water";
    internal const string minute = "minute";
    internal const string minuteperround = "minute per round";
    internal const string minutes = "minutes";
    internal const string minutesperround = "minutes per round";
    internal const string mm2 = "mm2";
    internal const string mm3 = "mm3";
    internal const string mmHg = "mmHg";
    internal const string morethanahalf = "more_than_a_half";
    internal const string mph = "mph";
    internal const string ms = "m/s";
    internal const string nauticalmile = "nautical mile";
    internal const string nauticalmiles = "nautical miles";
    internal const string newtonmeter = "newton meter";
    internal const string newtonmeters = "newton meters";
    internal const string newtonpersquaremeter = "newton per square meter";
    internal const string newtonspersquaremeter = "newtons per square meter";
    internal const string niceminuteperround = "nice minute per round";
    internal const string niceminutesperround = "nice minutes per round";
    internal const string Nm = "Nm";
    internal const string nmile = "nmile";
    internal const string nmiles = "nmiles";
    internal const string number = "number";
    internal const string numbers = "numbers";
    internal const string Pa = "Pa";
    internal const string part = "part";
    internal const string pascal = "pascal";
    internal const string pascals = "pascals";
    internal const string percent = "percent";
    internal const string percentage = "percentage";
    internal const string percentover100 = "percent over 100";
    internal const string percentscaler16k = "percent scaler 16k";
    internal const string percentscaler2pow23 = "percent scaler 2pow23";
    internal const string percentscaler32k = "percent scaler 32k";
    internal const string perdegree = "per degree";
    internal const string perhour = "per hour";
    internal const string perminute = "per minute";
    internal const string perradian = "per radian";
    internal const string persecond = "per second";
    internal const string position = "position";
    internal const string position128 = "position 128";
    internal const string position16k = "position 16k";
    internal const string position32k = "position 32k";
    internal const string pound = "pound";
    internal const string poundalfeet = "poundal feet";
    internal const string poundforcepersquarefoot = "pound-force per square foot";
    internal const string poundforcepersquareinch = "pound-force per square inch";
    internal const string poundperhour = "pound per hour";
    internal const string pounds = "pounds";
    internal const string poundscaler256 = "pound scaler 256";
    internal const string poundsperhour = "pounds per hour";
    internal const string poundsscaler256 = "pounds scaler 256";
    internal const string pph = "pph";
    internal const string psf = "psf";
    internal const string psfscaler16k = "psf scaler 16k";
    internal const string psi = "psi";
    internal const string psi4over16k = "psi 4 over 16k";
    internal const string psifs7oilpressure = "psi fs7 oil pressure";
    internal const string psiscaler16k = "psi scaler 16k";
    internal const string quart = "quart";
    internal const string quarts = "quarts";
    internal const string radian = "radian";
    internal const string radianpersecond = "radian per second";
    internal const string radians = "radians";
    internal const string radianspersecond = "radians per second";
    internal const string rankine = "rankine";
    internal const string ratio = "ratio";
    internal const string revolutionperminute = "revolution per minute";
    internal const string revolutionsperminute = "revolutions per minute";
    internal const string round = "round";
    internal const string rounds = "rounds";
    internal const string rpm = "rpm";
    internal const string rpm1over16k = "rpm 1 over 16k";
    internal const string rpms = "rpms";
    internal const string scaler = "scaler";
    internal const string second = "second";
    internal const string seconds = "seconds";
    internal const string slug = "slug";
    internal const string slugfeetsquared = "slug feet squared";
    internal const string Slugft3 = "Slug/ft3";
    internal const string Slugpercubicfeet = "Slug per cubic feet";
    internal const string Slugpercubicfoot = "Slug per cubic foot";
    internal const string slugs = "slugs";
    internal const string slugsfeetsquared = "slugs feet squared";
    internal const string Slugspercubicfeet = "Slugs per cubic feet";
    internal const string Slugspercubicfoot = "Slugs per cubic foot";
    internal const string sqcm = "sq cm";
    internal const string sqft = "sq ft";
    internal const string sqin = "sq in";
    internal const string sqkm = "sq km";
    internal const string sqm = "sq m";
    internal const string sqmm = "sq mm";
    internal const string squarecentimeter = "square centimeter";
    internal const string squarecentimeters = "square centimeters";
    internal const string squarefeet = "square feet";
    internal const string squarefoot = "square foot";
    internal const string squareinch = "square inch";
    internal const string squareinches = "square inches";
    internal const string squarekilometer = "square kilometer";
    internal const string squarekilometers = "square kilometers";
    internal const string squaremeter = "square meter";
    internal const string squaremeters = "square meters";
    internal const string squaremile = "square mile";
    internal const string squaremiles = "square miles";
    internal const string squaremillimeter = "square millimeter";
    internal const string squaremillimeters = "square millimeters";
    internal const string squareyard = "square yard";
    internal const string squareyards = "square yards";
    internal const string sqyd = "sq yd";
    internal const string String = "string";
    internal const string third = "third";
    internal const string thirds = "thirds";
    internal const string times = "times";
    internal const string volt = "volt";
    internal const string volts = "volts";
    internal const string Watt = "Watt";
    internal const string Watts = "Watts";
    internal const string yard = "yard";
    internal const string yards = "yards";
    internal const string yd2 = "yd2";
    internal const string yd3 = "yd3";
    internal const string year = "year";
    internal const string years = "years";

    #endregion

    #region Unit List Arrays

    // eliminates duplicates and obscure stuff
    static public readonly string[] ListUsable = new string[]
    {
      null_unit,
      amp,
      amps,
      atm,
      atmospheres,
      bar,
      Bco16,
      bels,
      Bool,
      celsius,
      cm,
      cm2,
      cm3,
      cmHg,
      cucm,
      cuft,
      cuin,
      cukm,
      cum,
      cumm,
      cuyd,
      days,
      decibels,
      decimiles,
      decinmiles,
      degrees,
      degreeslatitude,
      degreeslongitude,
      degreespersecond,
      Enum,
      fahrenheit,
      farenheit,
      feet,
      feetminute,
      feetperminute,
      feetpersecond,
      feetpersecondsquared,
      feetsecond,
      flags,
      foot,
      footpersecondsquared,
      FrequencyADFBCD32,
      FrequencyBCD16,
      FrequencyBCD32,
      ft,
      ft2,
      ft3,
      ftlbpersecond,
      ftlbs,
      ftmin,
      gallons,
      geepounds,
      GForce,
      gph,
      grads,
      halfs,
      hectopascals,
      hours,
      hoursover10,
      Hz,
      inch,
      in2,
      in3,
      inches,
      inHg,
      kelvin,
      keyframes,
      kg,
      kgfmeter,
      kgfmeters,
      KgFSqCm,
      KHz,
      kilogrammeters,
      kilograms,
      kilogramsmetersquared,
      kilogramspercubicmeter,
      kilogramspersecond,
      kilometershour,
      kilometersperhour,
      kilopascal,
      km,
      km2,
      km3,
      knot,
      knots,
      knotsscaler128,
      kPa,
      kph,
      lbffeet,
      lbs,
      liters,
      litersperhour,
      m,
      m2,
      m3,
      mach,
      mach3d2over64k,
      mask,
      mbars,
      meters,
      metercubed,
      metercubedpersecond,
      metersecond,
      meterslatitude,
      meterperminute,
      meterpersecond,
      meterpersecondscaler256,
      meterpersecondsquared,
      meterscaler256,
      metersecond,
      MHz,
      miles,
      milesperhour,
      millibars,
      millibarscaler16,
      millibarsscaler16,
      millimeters,
      minutes,
      minutesperround,
      mm2,
      mm3,
      mmHg,
      morethanahalf,
      mph,
      ms,
      nauticalmiles,
      newtonspersquaremeter,
      niceminutesperround,
      Nm,
      nmiles,
      number,
      Pa,
      part,
      pascals,
      percent,
      percentage,
      percentover100,
      percentscaler16k,
      percentscaler2pow23,
      percentscaler32k,
      perdegree,
      perhour,
      perminute,
      perradian,
      persecond,
      position,
      position128,
      position16k,
      position32k,
      poundalfeet,
      poundforcepersquarefoot,
      poundforcepersquareinch,
      pounds,
      poundscaler256,
      poundsperhour,
      poundsscaler256,
      pph,
      psf,
      psfscaler16k,
      psi,
      psi4over16k,
      psiscaler16k,
      quarts,
      radians,
      radianspersecond,
      rankine,
      ratio,
      rounds,
      rpm,
      rpm1over16k,
      scaler,
      seconds,
      Slugft3,
      slugs,
      slugsfeetsquared,
      Slugspercubicfeet,
      Slugspercubicfoot,
      sqcm,
      sqft,
      sqin,
      sqkm,
      sqm,
      sqmm,
      squaremile,
      squaremiles,
      squaremillimeter,
      squaremillimeters,
      sqyd,
      String,
      thirds,
      times,
      volts,
      Watts,
      yards,
      yd2,
      yd3,
      years,
    };

    static public readonly string[] ListAll = new string[]
    {
      null_unit,
      amp,
      ampere,
      amperes,
      amps,
      angl16,
      angl32,
      atm,
      atmosphere,
      atmospheres,
      bar,
      bars,
      Bco16,
      bel,
      bels,
      Bool,
      Boolean,
      boostcmHg,
      boostinHg,
      boostpsi,
      celsius,
      celsiusfs7egt,
      celsiusfs7oiltemp,
      celsiusscaler1256,
      celsiusscaler16k,
      celsiusscaler256,
      centimeter,
      centimeterofmercury,
      centimeters,
      centimetersofmercury,
      cm,
      cm2,
      cm3,
      cmHg,
      cubiccentimeter,
      cubiccentimeters,
      cubicfeet,
      cubicfoot,
      cubicinch,
      cubicinches,
      cubickilometer,
      cubickilometers,
      cubicmeter,
      cubicmeters,
      cubicmile,
      cubicmiles,
      cubicmillimeter,
      cubicmillimeters,
      cubicyard,
      cubicyards,
      cucm,
      cuft,
      cuin,
      cukm,
      cum,
      cumm,
      cuyd,
      day,
      days,
      decibel,
      decibels,
      decimile,
      decimiles,
      decinmile,
      decinmiles,
      degree,
      degreeangl16,
      degreeangl32,
      degreelatitude,
      degreelongitude,
      degreepersecond,
      degreepersecondang16,
      degrees,
      degreesangl16,
      degreesangl32,
      degreeslatitude,
      degreeslongitude,
      degreespersecond,
      degreespersecondang16,
      Enum,
      fahrenheit,
      farenheit,
      feet,
      feetminute,
      feetperminute,
      feetpersecond,
      feetpersecondsquared,
      feetsecond,
      flags,
      foot,
      footpersecondsquared,
      footpound,
      footpounds,
      FrequencyADFBCD32,
      FrequencyBCD16,
      FrequencyBCD32,
      fs7chargingamps,
      fs7oilquantity,
      ft,
      ft2,
      ft3,
      ftlbpersecond,
      ftlbs,
      ftmin,
      gallon,
      gallonperhour,
      gallons,
      gallonsperhour,
      geepound,
      geepounds,
      GForce,
      GForce624scaled,
      GLOBALPdeltaheadingrate,
      GLOBALPeng1manifoldpressure,
      GLOBALPeng1oilprs,
      GLOBALPeng1oiltmp,
      GLOBALPverticalspeed,
      gph,
      grad,
      grads,
      half,
      halfs,
      hectopascal,
      hectopascals,
      Hertz,
      hour,
      hourover10,
      hours,
      hoursover10,
      Hz,
      inch,
      in2,
      in3,
      inches,
      inchesofmercury,
      inchofmercury,
      inHg,
      inHg64over64k,
      kelvin,
      keyframe,
      keyframes,
      kg,
      kgfmeter,
      kgfmeters,
      KgFSqCm,
      KHz,
      kilogram,
      kilogramforcepersquarecentimeter,
      kilogrammeter,
      kilogrammeters,
      kilogrammetersquared,
      kilogrampercubicmeter,
      kilogrampersecond,
      kilograms,
      kilogramsmetersquared,
      kilogramspercubicmeter,
      kilogramspersecond,
      Kilohertz,
      kilometer,
      kilometerhour,
      kilometerperhour,
      kilometers,
      kilometershour,
      kilometersperhour,
      kilopascal,
      km,
      km2,
      km3,
      knot,
      knots,
      knotscaler128,
      knotsscaler128,
      kPa,
      kph,
      lbffeet,
      lbs,
      liter,
      literperhour,
      liters,
      litersperhour,
      m,
      m2,
      m3,
      mach,
      mach3d2over64k,
      machs,
      mask,
      mbar,
      mbars,
      Megahertz,
      meter,
      metercubed,
      metercubedpersecond,
      meterlatitude,
      meterperminute,
      meterpersecond,
      meterpersecondscaler256,
      meterpersecondsquared,
      meters,
      meterscaler256,
      meterscubed,
      meterscubedpersecond,
      metersecond,
      meterslatitude,
      metersperminute,
      meterspersecond,
      meterspersecondscaler256,
      meterspersecondsquared,
      metersscaler256,
      meterssecond,
      MHz,
      mile,
      mileperhour,
      miles,
      milesperhour,
      millibar,
      millibars,
      millibarscaler16,
      millibarsscaler16,
      millimeter,
      millimeterofmercury,
      millimeterofwater,
      millimeters,
      millimetersofmercury,
      millimetersofwater,
      minute,
      minuteperround,
      minutes,
      minutesperround,
      mm2,
      mm3,
      mmHg,
      morethanahalf,
      mph,
      ms,
      nauticalmile,
      nauticalmiles,
      newtonmeter,
      newtonmeters,
      newtonpersquaremeter,
      newtonspersquaremeter,
      niceminuteperround,
      niceminutesperround,
      Nm,
      nmile,
      nmiles,
      number,
      numbers,
      Pa,
      part,
      pascal,
      pascals,
      percent,
      percentage,
      percentover100,
      percentscaler16k,
      percentscaler2pow23,
      percentscaler32k,
      perdegree,
      perhour,
      perminute,
      perradian,
      persecond,
      position,
      position128,
      position16k,
      position32k,
      pound,
      poundalfeet,
      poundforcepersquarefoot,
      poundforcepersquareinch,
      poundperhour,
      pounds,
      poundscaler256,
      poundsperhour,
      poundsscaler256,
      pph,
      psf,
      psfscaler16k,
      psi,
      psi4over16k,
      psifs7oilpressure,
      psiscaler16k,
      quart,
      quarts,
      radian,
      radianpersecond,
      radians,
      radianspersecond,
      rankine,
      ratio,
      revolutionperminute,
      revolutionsperminute,
      round,
      rounds,
      rpm,
      rpm1over16k,
      rpms,
      scaler,
      second,
      seconds,
      slug,
      slugfeetsquared,
      Slugft3,
      Slugpercubicfeet,
      Slugpercubicfoot,
      slugs,
      slugsfeetsquared,
      Slugspercubicfeet,
      Slugspercubicfoot,
      sqcm,
      sqft,
      sqin,
      sqkm,
      sqm,
      sqmm,
      squarecentimeter,
      squarecentimeters,
      squarefeet,
      squarefoot,
      squareinch,
      squareinches,
      squarekilometer,
      squarekilometers,
      squaremeter,
      squaremeters,
      squaremile,
      squaremiles,
      squaremillimeter,
      squaremillimeters,
      squareyard,
      squareyards,
      sqyd,
      String,
      third,
      thirds,
      times,
      volt,
      volts,
      Watt,
      Watts,
      yard,
      yards,
      yd2,
      yd3,
      year,
      years,
    };

    #endregion
  }
}
