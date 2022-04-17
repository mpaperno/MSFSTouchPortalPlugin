using Microsoft.FlightSimulator.SimConnect;
using System.Reflection;

namespace MSFSTouchPortalPlugin.Types
{
  /// <summary>
  /// SimConnect request (method invocation) tracking record for storing which request caused a simulator error (such as unknown variable/event name, etc).
  /// </summary>
  internal class RequestTrackingData
  {
    public uint dwSendId;               // the "dwSendId" from SimConnect_GetLastSentPacketID() and SIMCONNECT_RECV_EXCEPTION struct
    public string sMethod;              // method name of the invoker
    public ParameterInfo[] aParamInfo;  // meta data about the invocation method params
    public object[] aArguments;         // parameter values passed in the invoker method
    public SIMCONNECT_EXCEPTION eException;  // associated exception, if any, from SIMCONNECT_RECV_EXCEPTION (default is SIMCONNECT_EXCEPTION.NONE)
    public uint dwExceptionIndex;            // The index number (starting at 1) of the first parameter that caused an error, if any, from SIMCONNECT_RECV_EXCEPTION (0 if unknown)

    public RequestTrackingData(uint sendId, string method, ParameterInfo[] paramInfo, params object[] args) {
      dwSendId = sendId;
      sMethod = method;
      aParamInfo = paramInfo;
      aArguments = args;
      eException = SIMCONNECT_EXCEPTION.NONE;
      dwExceptionIndex = 0;
    }

    // constructs a "null" instance with a valid request ID but no tracking data
    public RequestTrackingData(uint sendId) : this(sendId, null, null) { }

    /// <summary> Returns formated information about the method invocation which triggered the request, and the SimConnect error, if any. </summary>
    public override string ToString() {
      if (sMethod == null) {
        var ret = string.Empty;
        if (eException != SIMCONNECT_EXCEPTION.NONE)
          ret += eException.ToString() + " but: ";
        return ret + "Request record not found for SendId " + dwSendId.ToString();
      }
      var sb = new System.Text.StringBuilder(150);
      if (eException != SIMCONNECT_EXCEPTION.NONE)
        sb.Append(eException.ToString()).Append(" for request: ");
      sb.Append(sMethod).Append('(');
      for (int i = 0, e = aParamInfo.Length; i < e; ++i) {
        if (i > 0)
          sb.Append(", ");
        if (i == dwExceptionIndex - 1)
          sb.Append("[@] ");
        sb.Append(aParamInfo[i].ParameterType.ToString().Split('.')[^1]).Append(' ').
           Append(aParamInfo[i].Name).Append(" = ").
           Append(aArguments[i]);
      }
      sb.Append(')');
      if (dwExceptionIndex > 0 && dwExceptionIndex < uint.MaxValue)
        sb.Append(" ([@] = error source arg. ").Append($"{dwExceptionIndex})");
      return sb.ToString();
    }
  }

}
