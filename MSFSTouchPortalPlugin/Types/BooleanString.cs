using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MSFSTouchPortalPlugin.Types
{
  /// <summary>
  /// A boolean unmutable type which is set using a string value, converting
  /// common English expressions for "truthiness" to a stored bool value.
  /// `true` expressions are:
  ///   any non-zero number, "true", "yes", "y", "on", "enable", "enabled"
  /// It otherwise acts as a bool, also using its bool.ToString() for
  /// string representation (not the string originally used to set the value).
  /// It has implicit conversion operators to bool, int, uint, double, and string,
  /// and can be assigned-to from those types as well.
  /// The default value is False.
  /// </summary>
  internal readonly struct BooleanString
  {
    [MarshalAs(UnmanagedType.U1)]
    private readonly bool _value;
    private static readonly Regex _rx = new Regex(@"^(?:(?=[^1-9]*[1-9])(?=[\d.,-]).*|true|y(?:es)?|on|enabled?)$", RegexOptions.IgnoreCase);

    public BooleanString(bool value) { _value = value; }
    public BooleanString(string value = default) : this(StringToBool(value)) { }

    public static bool StringToBool(string str) {
      return !string.IsNullOrWhiteSpace(str) && _rx.IsMatch(str);
    }

    public static implicit operator BooleanString(string s) => new BooleanString(s);
    public static implicit operator BooleanString(bool b) => new BooleanString(b);
    public static implicit operator BooleanString(int i) => new BooleanString(i != 0);
    public static implicit operator BooleanString(uint u) => new BooleanString(u != 0U);
    public static implicit operator BooleanString(double d) => new BooleanString((int)d != 0);
    public static implicit operator bool(BooleanString b) => b._value;
    public static implicit operator int(BooleanString b) => b._value ? 1 : 0;
    public static implicit operator uint(BooleanString b) => b._value ? 1U : 0U;
    public static implicit operator double(BooleanString b) => b._value ? 1.0 : 0.0;
    public static implicit operator string(BooleanString b) => b.ToString();

    public override string ToString() => _value.ToString();
    public override int GetHashCode() => _value.GetHashCode();
    public override bool Equals(object obj) => _value.Equals(obj);
  }
}
