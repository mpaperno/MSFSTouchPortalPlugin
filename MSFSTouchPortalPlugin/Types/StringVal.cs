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


using System;
using System.Runtime.InteropServices;

namespace MSFSTouchPortalPlugin.Types
{
  internal readonly struct StringVal : IEquatable<StringVal> {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
    public readonly string Value;

    public const int MAX_SIZE = 256;

    public StringVal(string value = default) { Value = (value.Length > MAX_SIZE ? value.Substring(0, MAX_SIZE) : value); }
    public bool Equals(StringVal other) => other.Value == Value;
    public override bool Equals(object obj) => (obj is StringVal val && Equals(val));
    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(StringVal obj1, StringVal obj2) => obj1.Equals(obj2);
    public static bool operator !=(StringVal obj1, StringVal obj2) => !obj1.Equals(obj2);
  }
}
