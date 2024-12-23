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

using System.Text.RegularExpressions;

namespace MSFSTouchPortalPlugin.Helpers
{
  internal static class Utils
  {

    internal static bool TryConvertDouble(object value, out double dVal)
    {
      if (value is double) {
        dVal = (double)value;
        return true;
      }
      try {
        dVal = System.Convert.ToDouble(value);
        return true;
      }
      catch {
        return double.TryParse(value.ToString(), out dVal);
      }
    }


    static readonly Regex RxFindFirstSentence = new Regex(@"\.(?:\W|$)", RegexOptions.Multiline | RegexOptions.Compiled);

    internal static string FormatDescriptionForSelector(string desc)
    {
      var maxLen = Configuration.Settings.SelectorsMaxLineLen.IntValue;
      if (maxLen <= 0)
        return string.Empty;

      var match = RxFindFirstSentence.Match(desc);
      if (match.Success)
        desc = desc[0..(System.Math.Min(match.Index + 1, maxLen))] + (match.Index > maxLen ? "..." : "");
      else if (desc.Length > maxLen)
        desc = desc[0..maxLen] + "...";
      else
        desc += '.';
      return "\n " + desc.Replace('\n', ' ');
    }

  }
}
