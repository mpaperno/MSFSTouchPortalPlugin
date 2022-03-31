
using System;
using System.Runtime.InteropServices;

namespace MSFSTouchPortalPlugin.Types
{
  internal readonly struct StringVal : IEquatable<StringVal> {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public readonly string Value;

    public bool Equals(StringVal other) => other.Value == Value;
    public override bool Equals(object obj) => (obj is StringVal && Equals((StringVal)obj));
    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(StringVal obj1, StringVal obj2) => obj1.Equals(obj2);
    public static bool operator !=(StringVal obj1, StringVal obj2) => !obj1.Equals(obj2);
  }
}
